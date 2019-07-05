/****************************************************
    文件：GuideWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/17 11:56:51
    功能：引导对话界面
*****************************************************/
using UnityEngine.UI;
using PEProtocol;

public class GuideWind : WindowRoot
{
    public Text NameText;
    public Text TalkText;
    public Image ImageIcon;

    private PlayerData playerData;
    private AutoGuideCfg autoData;
    private string[] dialogArr;
    private int index;

    protected override void InitWind()
    {
        base.InitWind();
        playerData = GameRoot.Instance.PlayerData;
        autoData = MainCitySys.Instance.GetAutoTaskData();
        dialogArr = autoData.DilogArr.Split('#');
        index = 1;
        SetTalk();
    }

    private void SetTalk()
    {
        string[] talkArr = dialogArr[index].Split('|');
        if (talkArr[0] == "0")
        {
            SetSprite(ImageIcon, PathDefine.SelfIcon);
            SetText(NameText, playerData.Name);
        }
        else
        {
            switch (autoData.NpcId)
            {
                case 0:
                    SetSprite(ImageIcon, PathDefine.WiseManIcon);
                    SetText(NameText, "教主");
                    break;
                case 1:
                    SetSprite(ImageIcon, PathDefine.GeneralIcon);
                    SetText(NameText, "将军");
                    break;
                case 2:
                    SetSprite(ImageIcon, PathDefine.ArtisanIcon);
                    SetText(NameText, "工匠");
                    break;
                case 3:
                    SetSprite(ImageIcon, PathDefine.TraderIcon);
                    SetText(NameText, "奸商");
                    break;
                default:
                    SetSprite(ImageIcon, PathDefine.GuideIcon);
                    SetText(NameText, "圣人");
                    break;
            }
        }

        ImageIcon.SetNativeSize();
        SetText(TalkText, talkArr[1].Replace("$name", playerData.Name));
    }
    public void ClickNextBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot .UiClickBtn);

        index += 1;
        if (index == dialogArr.Length)
        {
            //发送任务引导完成信息
            NetMsg netMsg = new NetMsg
            {
                cmd = (int) Command.ReqGuide,
                RequestGuide = new RequestGuide
                {
                    GuideId = autoData.Id,
                }
            };
            NetSvc.SendMsg(netMsg);
            SetWindState(false);
        }
        else
        {
            SetTalk();
        }
    }
}