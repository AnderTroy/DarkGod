/****************************************************
    文件：BattleSys.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:31:34
    功能：战斗业务系统
*****************************************************/
using PEProtocol;
using UnityEngine;

public class BattleSys : SystemRoot
{
    public BattlesWind BattlesWind;
    public BattleEndWind BattleEndWind;
    [HideInInspector]
    public BattleMgr BattleMgr;

    [HideInInspector]
    public int BattleId;
    private double _startTime;
    public static BattleSys Instance = null;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        Debug.Log(Instance.GetType());
    }

    public void StartBattle(int mapId)
    {
        BattleId = mapId;
        GameObject gameObj = new GameObject
        {
            name = "BattleRoot"
        };
        gameObj.transform.SetParent(GameRoot.Instance.transform);
        BattleMgr = gameObj.AddComponent<BattleMgr>();

        BattleMgr.Init(mapId, () => { _startTime = TimeSvc.GetNowTime(); });
    }
    
    public void SetBattleWindState(bool isActive=true)
    {
        BattlesWind.SetWindState(isActive);
    }

    public void SetBattleEndWinsSate(BattleEndType endType,bool isActive = true)
    {
        BattleEndWind.SetWindSate(endType);
        BattleEndWind.SetWindState(isActive);
    }
    public void EndBattle(bool isWin, int restHp)
    {
        BattlesWind.SetWindState(false);
        GameRoot.Instance.DynamicWind.RemoveAllHpItemInfo();

        if (isWin)
        {
            double endTime = TimeSvc.GetNowTime();
            NetMsg netMsg = new NetMsg
            {
                cmd = (int)Command.RequestBattleEnd,
                RequestBattleEnd = new RequestBattleEnd
                {
                    IsWin=true,
                    BattleId = BattleId,
                    RestHp=restHp,
                    CostTime=(int)(endTime-_startTime),
                }
            };
            NetSvc.SendMsg(netMsg);
        }
        else
        {
            SetBattleEndWinsSate(BattleEndType.Lose);
        }
    }

    public void DestroyBattle()
    {
        SetBattleEndWinsSate(BattleEndType.Pause,false);
        SetBattleWindState(false);
        GameRoot.Instance.DynamicWind.RemoveAllHpItemInfo();
        Destroy(BattleMgr.gameObject);
    }
    public void ResponseBattleEnd(NetMsg netMsg)
    {
        ResponseBattleEnd data = netMsg.ResponseBattleEnd;
        GameRoot.Instance.SetPlayerDataByBattleEnd(netMsg.ResponseBattleEnd);

        BattleEndWind.SetBattleEndData(data.Battle, data.CostTime, data.RestHp);
        SetBattleEndWinsSate(BattleEndType.Win);
    }
    public void SetMoveDir(Vector2 dir)
    {
        BattleMgr.SetSelfPlayerMoveDir(dir);
    }
    public void ReqReleaseSkill(int index)
    {
        BattleMgr.ReqReleaseSkill(index);
    }

    public Vector2 GetDirInput()
    {
        return BattlesWind.CurrentDir;
    }
}