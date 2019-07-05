/****************************************************
	文件：NetSvc.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/09 14:24   	
	功能：网络服务模块
*****************************************************/
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using PENet;
using PEProtocol;
public class NetSvc : MonoBehaviour
{
    public static NetSvc Instance = null;
    PESocket<ClientSession, NetMsg> pESocket = null;
    private static readonly string obj = "lock";
    private readonly Queue<NetMsg> netMsgQueue = new Queue<NetMsg>();
    public void InitSvc()
    {
        Instance = this;
        Debug.Log(Instance.GetType());

        pESocket = new PESocket<ClientSession, NetMsg>();
        pESocket.SetLog(true, (string msg, int lv) =>
        {
            switch (lv)
            {
                case 0:
                    msg = "Log:" + msg;
                    Debug.Log(msg);
                    break;
                case 1:
                    msg = "LogWarning:" + msg;
                    Debug.LogWarning(msg);
                    break;
                case 2:
                    msg = "LogError:" + msg;
                    Debug.LogError(msg);
                    break;
                case 3:
                    msg = "Info:" + msg;
                    Debug.Log(msg);
                    break;
            }
        });
        string hostName = Dns.GetHostName();
        string ipName = "";
        IPAddress[] iPAddress = Dns.GetHostAddresses(hostName);
        foreach (IPAddress ipa in iPAddress)
        {
            if (ipa.AddressFamily == AddressFamily.InterNetwork)
            {
                ipName = ipa.ToString();
            }
        }
        pESocket.StartAsClient(ipName, IpCfg.SrvPort);
    }

    public void SendMsg(NetMsg netMsg)
    {
        if (pESocket.session!=null)
        {
            pESocket.session.SendMsg(netMsg);
        }
        else
        {
            GameRoot.AddTips("服务器未连接");
            InitSvc();
        }
    }

    public void AddNetPack(NetMsg netMsg)
    {
        lock (obj)
        {
            netMsgQueue.Enqueue(netMsg);
        }
    }
    private void Update()
    {
        lock (obj)
        {
            if (netMsgQueue.Count > 0)
            {
                NetMsg msg = netMsgQueue.Dequeue();
                ProcessMsg(msg);
            }
        }
    }
    private void ProcessMsg(NetMsg msg)
    {
        if (msg.err != (int)ErrorCode.None)
        {
            switch ((ErrorCode)msg.err)
            {
                case ErrorCode.ServerDataError:
                    PeRoot.Log("服务器数据异常", PEProtocol.LogType.LogError);
                    GameRoot.AddTips("客户端数据异常");
                    break;
                case ErrorCode.UpdateDbError:
                    PeRoot.Log("数据库更新异常", PEProtocol.LogType.LogError);
                    GameRoot.AddTips("网络不稳定");
                    break;
                case ErrorCode.AcctIsOnline:
                    GameRoot.AddTips("当前账号已经上线");
                    break;
                case ErrorCode.ClientDataError:
                    PeRoot.Log("客户端数据异常",PEProtocol.LogType.LogError);
                    break;
                case ErrorCode.WrongPass:
                    GameRoot.AddTips("密码错误");
                    break;
                case ErrorCode.LackLevel:
                    GameRoot.AddTips("角色等级不足");
                    break;
                case ErrorCode.LackCoin:
                    GameRoot.AddTips("金币不足");
                    break;
                case ErrorCode.LackCrystal:
                    GameRoot.AddTips("水晶不足");
                    break;
                case ErrorCode.LockDiamond:
                    GameRoot.AddTips("钻石不足");
                    break;
                case ErrorCode.LackPower:
                    GameRoot.AddTips("体力不足");
                    break;
            }
            return;
        }
        switch ((Command)msg.cmd)
        {
            case Command.ResponseLogin:
                LoginSys.Instance.RspLogin(msg);
                break;
            case Command.ResponseName:
                LoginSys.Instance.RspRename(msg);
                break;
            case Command.RspGuide:
                MainCitySys.Instance.ResponseGuide(msg);
                break;
            case Command.RspStrong:
                MainCitySys.Instance.ResStrong(msg);
                break;
            case Command.PshChat:
                MainCitySys.Instance.PshChat(msg);
                break;
            case Command.ResponseBuy:
                MainCitySys.Instance.ResBuy(msg);
                break;
            case Command.PshPower:
                MainCitySys.Instance.PshPower(msg);
                break;
            case Command.ResponseTask:
                MainCitySys.Instance.ResponseTaskReward(msg);
                break;
            case Command.PshTask:
                MainCitySys.Instance.PshTaskPrangs(msg);
                break;
            case Command.ResponseBattle:
                CopySys.Instance.ResponseBattle(msg);
                break;
            case Command.ResponseBattleEnd:
                BattleSys.Instance.ResponseBattleEnd(msg);
                break;
        }
    }
}

