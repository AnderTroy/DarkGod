/****************************************************
    文件：CreateWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/7 22:57:47
    功能：角色创建界面
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
public class CreateWind : WindowRoot
{
    public InputField IptName;
    protected override void InitWind()
    {
        base.InitWind();
        IptName.text = ResSvc.Instance.GetRdNameData(true);
    }

    public void ClickRdNameButton()
    {
        GameRoot.AddTips("随机生成...");
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        IptName.text = ResSvc.Instance.GetRdNameData(Random.Range(0, 1) == 0);
    }

    public void ClickEnterButton()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiLoginBtn);
        if (IptName.text!=""&&IptName.text.Length<=5)
        {
            NetMsg msg = new NetMsg
            {
                cmd = (int)Command.RequestName,
                RequestName=new RequestName
                {
                    Name=IptName.text
                }
            };
            NetSvc.SendMsg(msg);
        }
        else if (IptName.text.Length>5)
        {
            GameRoot.AddTips("名字长度不符合规则");
        }
        else
        {
            GameRoot.AddTips("请输入名字");
        }
    }
}