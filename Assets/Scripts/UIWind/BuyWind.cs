/****************************************************
    文件：BuyWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/22 9:42:33
    功能：购买界面
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class BuyWind : WindowRoot
{
    public Text InfoText;
    public Button SureBtn;
    private int buyType;
    public void setBuyType(int type)
    {
        buyType = type;
    }
    protected override void InitWind()
    {
        base.InitWind();
        SureBtn.interactable = true;
        RefreshUi();
    }

    private void RefreshUi()
    {
        switch (buyType)
        {
            case 0:
                InfoText.text= "是否花费" + ConstRoot.Color("10钻石", TextColor.Red) + "购买" + ConstRoot.Color("100体力", TextColor.Green) + "?";
                break;
            case 1:
                InfoText.text = "是否花费" + ConstRoot.Color("10钻石", TextColor.Red) + "购买" + ConstRoot.Color("1000金币", TextColor.Green) + "?";
                break;
        }
    }

    public void ClickSureBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);

        //发送网络购买消息 
        NetMsg msg = new NetMsg
        {
            cmd = (int)Command.RequestBuy,
            RequestBuy = new RequestBuy
            {
                Type = buyType,
                DiamondPay = 10
            }
        };
        NetSvc.SendMsg(msg);
        SureBtn.interactable = false;
    }

    public void ClickCloseBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetWindState(false);
    }
}