/****************************************************
    文件：BattleSys.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/25 11:27:54
    功能：战斗界面
*****************************************************/
using PEProtocol;
using UnityEngine;

public class CopySys : SystemRoot
{
    public CopyWind CopyWind;
    public static CopySys Instance = null;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        Debug.Log(Instance.GetType());
    }

    public void ResponseBattle(NetMsg netMsg)
    {
        GameRoot.Instance.SetPlayerDataByBattle(netMsg.ResponseBattle);

        MainCitySys.Instance.MainCityWind.SetWindState(false);
        SetCopyWindState(false);
        BattleSys.Instance.StartBattle(netMsg.ResponseBattle.BattleId);
    }
    public void EnterCopyWind()
    {
        CopyWind.SetWindState();
    }

    public void SetCopyWindState(bool isActive = true)
    {
        CopyWind.SetWindState(isActive);
    }
}