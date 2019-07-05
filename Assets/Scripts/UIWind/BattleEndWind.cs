/****************************************************
    文件：BattleEndWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/6/2 13:1:36
    功能：战斗结束界面
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;

public class BattleEndWind : WindowRoot
{
    public Transform WinTrans;
    public Transform PauseTrans;
    public Transform LoseTrans;

    public Text TextTime;
    public Text TextRewHp;
    public Text TextReward;
    private ResSvc _resSvcThis;
    private NetSvc _netSvcThis;
    private PlayerData _playerData = null;
    private BattleEndType _endType = BattleEndType.None;
    protected override void InitWind()
    {
        base.InitWind();
        _resSvcThis = ResSvc.Instance;
        _netSvcThis = NetSvc.Instance;
        _playerData = GameRoot.Instance.PlayerData;
        RefreshUi();
    }

    private void RefreshUi()
    {
        switch (_endType)
        {
            case BattleEndType.Win:
                MapCfg mapCfg = ResSvc.GetMapCfgData(_battleId);
                int min = _costTime / 60;
                int sec = _costTime % 60;
                int coin = mapCfg.Coin;
                int crystal = mapCfg.Crystal;
                int exp = mapCfg.Exp;
                
                SetText(TextTime, min/60 + ":" + sec);
                SetText(TextRewHp, _restHp+ "");
                SetText(TextReward,
                    ConstRoot.Color(coin+"" , TextColor.Red) + "金币 " + ConstRoot.Color(exp+"", TextColor.Red) +"经验 "+
                    ConstRoot.Color(crystal + "", TextColor.Red) + "水晶 ");

                TimeSvc.AddTimeTask((int timeId) =>
                {
                    SetActive(WinTrans);
                    TimeSvc.AddTimeTask((int timeId1) =>
                    {
                        AudioSvc.PlayUiAudioMusic(ConstRoot.BattleWinBgAudio);
                        TimeSvc.AddTimeTask((int timeId2) =>
                        {
                            AudioSvc.PlayUiAudioMusic(ConstRoot.BattleWinBgAudio);
                            TimeSvc.AddTimeTask((int timeId3) =>
                            {
                                AudioSvc.PlayUiAudioMusic(ConstRoot.BattleWinBgAudio);
                                TimeSvc.AddTimeTask((int timeId4) =>
                                {
                                    AudioSvc.PlayUiAudioMusic(ConstRoot.BattleItemBgAudio);
                                }, 200);
                            }, 270);
                        }, 270);
                    }, 325);
                }, 1000);
                break;
            case BattleEndType.Pause:
                SetActive(PauseTrans);
                break;
            case BattleEndType.Lose:
                SetActive(LoseTrans);
                AudioSvc.PlayUiAudioMusic(ConstRoot.BattleLoseBgAudio);
                break;
        }
    }
    public void SetWindSate(BattleEndType endType)
    {
        _endType = endType;
    }

    public void ClickClose()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        BattleSys.Instance.BattleMgr.IsPause = false;
        SetWindState(false);
    }
    public void ClickBackCity()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        //todo
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
    }
    public void ClickBackBattle()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        //todo
        MainCitySys.Instance.EnterMainCity();
        BattleSys.Instance.DestroyBattle();
        MainCitySys.Instance.EnterCopyWind();
    }
    public void ClickNextBattle()
    {
        int battleId = _playerData.Battle;
        if (battleId>=10006)
        {
            battleId = 10006;
        }
        int power = _resSvcThis.GetMapCfgData(battleId).Power;
        if (power > _playerData.Power)
        {
            GameRoot.AddTips("体力不足");
            MainCitySys.Instance.EnterMainCity();
            BattleSys.Instance.DestroyBattle();
        }
        else
        {
            _netSvcThis.SendMsg(new NetMsg
            {
                cmd = (int)Command.RequestBattle,
                RequestBattle = new RequestBattle
                {
                    BattleId = battleId
                }
            });
        }
        BattleSys.Instance.DestroyBattle();
    }
    public void ClickAgineBattle()
    {
        BattleSys.Instance.DestroyBattle();
        int battleId = _playerData.Battle-1;
        int power = _resSvcThis.GetMapCfgData(battleId).Power;
        if (power > _playerData.Power)
        {
            GameRoot.AddTips("体力不足");
            MainCitySys.Instance.EnterMainCity();
            BattleSys.Instance.DestroyBattle();
        }
        else
        {
            _netSvcThis.SendMsg(new NetMsg
            {
                cmd = (int)Command.RequestBattle,
                RequestBattle = new RequestBattle
                {
                    BattleId = battleId
                }
            });
        }
    }

    private int _battleId;
    private int _costTime;
    private int _restHp;
    public void SetBattleEndData(int battleId,int costTime,int restHp)
    {
        _battleId = battleId;
        _costTime = costTime;
        _restHp = restHp;
    }
}

public enum BattleEndType
{
    None,
    Win,
    Pause,
    Lose,
}