/****************************************************
    文件：BattleMgr.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:47:18
    功能：游戏战斗入口
*****************************************************/

using System;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class BattleMgr : MonoBehaviour
{
    private ResSvc _resSvc;
    private AudioSvc _audioSvc;

    private StateMgr _stateMgr;
    private SkillMgr _skillMgr;
    private MapMgr _mapMgr;

    public EntityPlayer EntitySelfPlayer;
    private MapCfg _mapCfg;
    private readonly Dictionary<string, EntityMonster> _monsterDic = new Dictionary<string, EntityMonster>();
    public bool TriggerCheck = true;
    public bool IsPause = false;
    public void RemoveMonster(string key)
    {
        if (_monsterDic.TryGetValue(key, out var entityMonster))
        {
            _monsterDic.Remove(key);
            GameRoot.Instance.DynamicWind.RemoveHpItemInfo(key);
        }
    }
    public void Init(int mapId,Action action=null)
    {
        _resSvc = ResSvc.Instance;
        _audioSvc = AudioSvc.Instance;

        _stateMgr = gameObject.AddComponent<StateMgr>();
        _stateMgr.Init();
        _skillMgr = gameObject.AddComponent<SkillMgr>();
        _skillMgr.Init();

        _mapCfg = _resSvc.GetMapCfgData(mapId);
        _resSvc.AsyncLoadScene(_mapCfg.SceneName, () =>
        {
            GameObject map = GameObject.FindGameObjectWithTag("MapRoot");
            _mapMgr = map.GetComponent<MapMgr>();
            _mapMgr.Init(this);

            map.transform.localPosition = Vector3.zero;
            map.transform.localScale = Vector3.one;

            Camera.main.transform.localPosition = _mapCfg.MainCamPos;
            Camera.main.transform.localEulerAngles = _mapCfg.MainCamRote;
            _audioSvc.PlayBgAudioMusic(ConstRoot.BattleBgAudio);

            LoadPlayer(_mapCfg);
            EntitySelfPlayer.Idle();
            BattleSys.Instance.SetBattleWindState();

            ActiveCurrentBathMonsters();//激活第一批怪物

            action?.Invoke();
        });
    }

    private void LoadPlayer(MapCfg mapCfg)//加载角色
    {
        GameObject player = _resSvc.LoadPrefab(PathDefine.AssassinPlayerPrefab);

        player.transform.position = mapCfg.PlayerBornPos;
        player.transform.localEulerAngles = mapCfg.PlayerBornRote;
        player.transform.localScale = mapCfg.PlayerBornScale;
        PlayerData playerData = GameRoot.Instance.PlayerData;
        BattleAttribute attribute = new BattleAttribute
        {
            Hp = playerData.Hp,
            Ad = playerData.Ad,
            Ap = playerData.Ap,
            AdDefense = playerData.AdDefense,
            ApDefense = playerData.ApDefense,
            Dodge = playerData.Dodge, //闪避概率
            Pierce = playerData.Pierce, //穿透比率
            Critical = playerData.Critical, //暴击概率
        };
        EntitySelfPlayer = new EntityPlayer
        {
            BattleMgr = this,
            StateMgr = _stateMgr,
            SkillMgr = _skillMgr,
        };
        EntitySelfPlayer.SetBattleAttribute(attribute);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.Init();
        EntitySelfPlayer.SetController(playerController);//获取角色控制器
    }

    public void LoadMonsterByWaveId(int wave)//加载怪物
    {
        foreach (var monsterData in _mapCfg.MonsterDataLst)
        {
            if (monsterData.MonsterWave == wave)
            {
                GameObject monster = _resSvc.LoadPrefab(monsterData.MonsterCfg.ResPath, true);
                monster.transform.localPosition = monsterData.MonsterPos;
                monster.transform.localEulerAngles = monsterData.MonsterRote;
                monster.transform.localScale = new Vector3(1.5f,1.5f,1.5f);
                monster.name = "Monster" + monsterData.MonsterWave + "_" + monsterData.MonsterIndex;

                EntityMonster entityMonster = new EntityMonster
                {
                    BattleMgr = this,
                    StateMgr = _stateMgr,
                    SkillMgr = _skillMgr,
                    MonsterData = monsterData,
                };
                entityMonster.MonsterData = monsterData;
                //设置怪物属性
                entityMonster.SetBattleAttribute(monsterData.MonsterCfg.MonsterAttribute);
                entityMonster.NameCon = monster.name;

                MonsterController monsterCon = monster.GetComponent<MonsterController>();
                monsterCon.Init();
                entityMonster.SetController(monsterCon);
                monster.SetActive(false);
                _monsterDic.Add(monster.name, entityMonster);

                switch (monsterData.MonsterCfg.MonsterType)
                {
                    case MonsterType.Normal:
                        GameRoot.Instance.DynamicWind.AddHpItemInfo(monster.name, monsterCon.HpTrans.transform, entityMonster.Hp);
                        break;
                    case MonsterType.Boss:
                        BattleSys.Instance.BattlesWind.SetBossHpState(true);
                        break;
                }
            }
        }
    }
    private void Update()
    {
        foreach (var item in _monsterDic)
        {
            EntityMonster monster = item.Value;
            monster.TickAiLogic();
        }

        //检测当前怪物是否死亡
        if (_mapMgr!=null)
        {
            if (_monsterDic.Count==0&&TriggerCheck)
            {
                bool isExit = _mapMgr.SetNextTriggerOn();
                TriggerCheck = false;
                if (!isExit)
                {
                    //战斗胜利 TODO
                    EndBattle(true, EntitySelfPlayer.Hp);
                }
            }
        }
    }

    public void EndBattle(bool isWin,int restHp)
    {
        IsPause = true;
        AudioSvc.Instance.StopBgMusic();
        BattleSys.Instance.EndBattle(isWin, restHp);
    }
    public void ActiveCurrentBathMonsters()
    {
        TimeSvc.Instance.AddTimeTask((int timeId) =>
        {
            foreach (var temp in _monsterDic)
            {
                temp.Value.SetActive();
                temp.Value.Born();
                TimeSvc.Instance.AddTimeTask((int tiId) => { temp.Value.Idle(); }, 2000);
            }
        }, 0);
    }
    public List<EntityMonster> EntityMonsters()
    {
        List<EntityMonster> monsterLst = new List<EntityMonster>();
        foreach (var temp in _monsterDic)
        {
            monsterLst.Add(temp.Value);
        }

        return monsterLst;
    }

    #region 技能释放 角色控制管理
    public void SetSelfPlayerMoveDir(Vector2 dir)
    {
        //设置玩家移动
        if (EntitySelfPlayer.CanController == false)
        {
            return;
        }

        if (EntitySelfPlayer.CurrentAnimState==AnimState.Idle|| EntitySelfPlayer.CurrentAnimState == AnimState.Move)
        {
            if (dir == Vector2.zero)
            {
                EntitySelfPlayer.Idle();
            }
            else
            {
                EntitySelfPlayer.Move();
                EntitySelfPlayer.SetDir(dir);
            }
        }
        
    }
    public void ReqReleaseSkill(int index)
    {
        switch (index)
        {
            case 0:
                ReleaseNormalAtk();
                break;
            case 1:
                ReleaseSkill1();
                break;
            case 2:
                ReleaseSkill2();
                break;
            case 3:
                ReleaseSkill3();
                break;
        }
    }

    public int ComboIndex = 0;
    public double LastAttackTime = 0;
    private readonly int[] _comboArray = new int[] { 111, 112, 113, 114, 115 };
    private void ReleaseNormalAtk()
    {
        //PeRoot.Log("Click Normal Atk");
        if (EntitySelfPlayer.CurrentAnimState == AnimState.Attack)
        {
            double nowAttackTime = TimeSvc.Instance.GetNowTime();
            if (nowAttackTime - LastAttackTime < ConstRoot.ComboSpace && Math.Abs(LastAttackTime) > 0)
            {
                if (_comboArray[ComboIndex] != _comboArray[_comboArray.Length - 1])
                {
                    ComboIndex += 1;
                    EntitySelfPlayer.comboQueue.Enqueue(_comboArray[ComboIndex]);
                    LastAttackTime = nowAttackTime;
                }
                else
                {
                    LastAttackTime = 0;
                    ComboIndex = 0;
                }
            }
        }
        else if (EntitySelfPlayer.CurrentAnimState == AnimState.Idle || EntitySelfPlayer.CurrentAnimState == AnimState.Move)
        {
            ComboIndex = 0;
            LastAttackTime = TimeSvc.Instance.GetNowTime();
            EntitySelfPlayer.Attack(_comboArray[ComboIndex]);
        }
    }
    private void ReleaseSkill1()
    {
        EntitySelfPlayer.Attack(101);
    }
    private void ReleaseSkill2()
    {
        //PeRoot.Log("Click Skill2");
        EntitySelfPlayer.Attack(102);
    }
    private void ReleaseSkill3()
    {
        //PeRoot.Log("Click Skill3");
        EntitySelfPlayer.Attack(103);
    }
    public Vector2 GetDirInput()
    {
        return BattleSys.Instance.GetDirInput();
    }

    public bool CanRlsSkill()
    {
        return EntitySelfPlayer.CanRlsSkill;
    }
    #endregion
}