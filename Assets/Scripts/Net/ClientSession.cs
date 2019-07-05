/****************************************************
    文件：ClientSession.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/9 14:18:35
    功能：Nothing
*****************************************************/
using PENet;
using PEProtocol;
public class ClientSession : PESession<NetMsg>
{
    protected override void OnConnected()
    {
        GameRoot.AddTips("连接服务器成功");
        PeRoot.Log("Connect To Server.||连接服务器成功.");
        PeRoot.Log("开始发送消息");
    }

    protected override void OnReciveMsg(NetMsg msg)
    {
        PeRoot.Log("RcvPack CMD:" + (Command)msg.cmd);
        NetSvc.Instance.AddNetPack(msg);
    }

    protected override void OnDisConnected()
    {
        GameRoot.AddTips("服务器断开连接");
        PeRoot.Log("DisConnect To Server.||服务器断开连接.");
    }
}