/****************************************************
    文件：BattleWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/25 11:28:15
    功能：战斗界面
*****************************************************/
using PEProtocol;
using UnityEngine.UI;

public class CopyWind : WindowRoot 
{
    public Button[] Battles;
    private PlayerData playerData = null;
    protected override void InitWind()
    {
        base.InitWind();
        playerData = GameRoot.Instance.PlayerData;
        RefreshUi();
    }

    private void RefreshUi()
    {
        int battleId = playerData.Battle;
        for (int i = 0; i < Battles.Length; i++)
        {
            if (i<battleId%10000)
            {
                SetActive(Battles[i].gameObject);
            }
            else
            {
                SetActive(Battles[i].transform,false);
            }
        }
    }

    public void ClickBattlesBtn(int battleId)
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);

        int power = ResSvc.GetMapCfgData(battleId).Power;
        if (power>playerData.Power)
        {
            GameRoot.AddTips("体力不足");
        }
        else
        {
            NetSvc.SendMsg(new NetMsg
            {
                cmd = (int) Command.RequestBattle,
                RequestBattle = new RequestBattle
                {
                    BattleId = battleId
                }
            });
        }
    }
    public void ClickCloseBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetWindState(false);
    }
}