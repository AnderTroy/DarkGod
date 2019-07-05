/****************************************************
    文件：LoginWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/6 14:8:47
    功能：登入业务系统
*****************************************************/
using PEProtocol;
using UnityEngine;
public class LoginSys : SystemRoot
{
    public static LoginSys Instance = null;
    public LogInWind LogInWind;
    public CreateWind CreateWind;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        Debug.Log(Instance.GetType());
    }
    public void EnterLoginSys()
    {
        //异步加载场景
        //并显示加载的进度
        ResSvc.AsyncLoadScene(ConstRoot.StartScene, () =>
        {
            LogInWind.SetWindState();//加载登入界面
            AudioSvc.PlayBgAudioMusic(ConstRoot.BgAudio);//播放背景音乐
        });
    }

    public void RspLogin(NetMsg netMsg)
    {
        GameRoot.AddTips("登入成功");
        GameRoot.Instance.SetPlayerData(netMsg.ResponseLogin);
        if (netMsg.ResponseLogin.PlayerData.Name=="")
        {
            CreateWind.SetWindState();//打开角色创建界面
        }
        else
        {
            //进入游戏
            MainCitySys.Instance.EnterMainCity();
        }
        LogInWind.SetWindState(false);//关闭登入界面
    }
    public void RspRename(NetMsg msg)
    {
        GameRoot.Instance.SetPlayerName(msg.ResponseName.Name);
        //跳转场景进入主城  打开主城的界面
        MainCitySys.Instance.EnterMainCity();
        //关闭创建界面
        CreateWind.SetWindState(false);
    }
}