/****************************************************
    文件：LogInWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/6 19:48:57
    功能：加载登入注册界面 
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class LogInWind : WindowRoot
{
    public InputField iptAcct;//账号
    public InputField iptPass;//密码
    
    protected override void InitWind()
    {
        base.InitWind();
        if (PlayerPrefs.HasKey("Acct")&&PlayerPrefs.HasKey("Pass"))
        {
            iptAcct.text = PlayerPrefs.GetString("Acct");
            iptPass.text = PlayerPrefs.GetString("Pass");
        }
        else
        {
            iptAcct.text = "";
            iptPass.text = "";
        }
    }
    //进入游戏按钮
    public void ClickEnterButton()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiLoginBtn);
        string acct = iptAcct.text;
        string pass = iptPass.text;
        if (acct!=""&&pass!="")
        { 
            PlayerPrefs.SetString("Acct", acct);
            PlayerPrefs.SetString("Pass", pass);
            //发送网络消息，请求登入
            NetMsg netMsg = new NetMsg
            {
                cmd =(int)Command.RequestLogin,
                RequestLogin = new RequestLogin
                {
                    Acct=acct,
                    Pass=pass
                }
            };
            NetSvc.SendMsg(netMsg);
        }
        else
        {
            GameRoot.AddTips("账号密码不能为空");
        }
    }
    //公告栏按钮
    public void ClikTipsButton()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        GameRoot.AddTips("功能模块未开发...");
    }
}