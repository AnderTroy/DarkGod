/****************************************************
    文件：ChatWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/20 23:34:21
    功能：聊天界面
*****************************************************/
using System.Collections.Generic;
using PEProtocol;
using UnityEngine.UI;

public class ChatWind : WindowRoot
{
    public InputField InputField;
    public Text ChatText;
    public Image WorldImage;
    public Image GuildImage;
    public Image FriendImage;

    private List<string> chatList = new List<string>();
    private int type;
    private enum ChatType
    {
        World,
        Guild,
        Friend
    }
    protected override void InitWind()
    {
        base.InitWind();
        type = (int)ChatType.World;
        RefreshUi();
    }

    public void AddChatMsg(string name,string chat)
    {
        chatList.Add(ConstRoot.Color(name + ":", TextColor.Blue) + chat);
        if (chatList.Count>14)
        {
            chatList.RemoveAt(0);
        }

        if (GetWindState())
        {
            RefreshUi();
        }
    }
    private void RefreshUi()
    {
        if (type==(int)ChatType.World)
        {
            string chatMsg = "";
            foreach (var t in chatList)
            {
                chatMsg += t + "\n";
            }
            SetText(ChatText, chatMsg);
            SetSprite(WorldImage, PathDefine.BtnTypeIcon1);
            SetSprite(GuildImage, PathDefine.BtnTypeIcon2);
            SetSprite(FriendImage, PathDefine.BtnTypeIcon2);
        }
        else if (type==(int)ChatType.Guild)
        {
            SetText(ChatText, "尚未加入公会。");
            SetSprite(WorldImage, PathDefine.BtnTypeIcon2);
            SetSprite(GuildImage, PathDefine.BtnTypeIcon1);
            SetSprite(FriendImage, PathDefine.BtnTypeIcon2);
        }
        else if (type==(int)ChatType.Friend)
        {
            SetText(ChatText, "暂无好友。");
            SetSprite(WorldImage, PathDefine.BtnTypeIcon2);
            SetSprite(GuildImage, PathDefine.BtnTypeIcon2);
            SetSprite(FriendImage, PathDefine.BtnTypeIcon1);
        }
    }

    private bool canSend = true;
    public void ClickSendBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        if (!canSend)
        {
            GameRoot.AddTips("五秒后才能发送消息。");
            return;
        }
        if (!string.IsNullOrEmpty(InputField.text)&&InputField.text!=" ")
        {
            if (InputField .text.Length>15)
            {
                GameRoot.AddTips("字数不能超过十五个");
            }
            else
            {
                NetMsg netMsg = new NetMsg
                {
                    cmd = (int) Command.SendChat,
                    SendChat =new SendChat
                    {
                        Chat=InputField.text
                    }
                };
                InputField.text = "";
                NetSvc.SendMsg(netMsg);
                canSend = false;
                TimeSvc.AddTimeTask((int timeId) => { canSend = true; },5,PETimeUnit.Second);
            }
        }
        else
        {
            GameRoot.AddTips("尚未输入聊天信息");
        }
    }
    public void ClickWorldBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        type = (int) ChatType.World;
        RefreshUi();
    }
    public void ClickGuildBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        type = (int)ChatType.Guild;
        RefreshUi();
    }
    public void ClickFriendBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        type = (int)ChatType.Friend;
        RefreshUi();
    }
    public void ClickCloseWind()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetWindState(false);
    }
}