/****************************************************
    文件：EntityMonster.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:53:39
    功能：怪物逻辑实体
*****************************************************/

using UnityEngine;

public class EntityMonster : EntityBase
{
    public EntityMonster()
    {
        EntityType = EntityType.Monster;
    }
    private float _checkTime = 2;
    private float _checkCountTime = 0;

    private float _attackTime = 2;
    private float _attackCountTime = 0;


    private bool _runAi = true;
    public MonsterData MonsterData;
    public override void SetBattleAttribute(BattleAttribute attribute)
    {
        int level = MonsterData.Level;
        BattleAttribute battleAttribute = new BattleAttribute
        {
            Hp = attribute.Hp * level,
            Ad = attribute.Ad * level,
            Ap = attribute.Ap * level,
            AdDefense = attribute.AdDefense * level,
            ApDefense = attribute.ApDefense * level,
            Dodge = attribute.Dodge * level,
            Pierce = attribute.Pierce * level,
            Critical = attribute.Critical * level,
        };

        BattleAttribute = battleAttribute;
        Hp = battleAttribute.Hp;
    }
    /// <summary>
    /// Monsters Ai 逻辑开发
    /// </summary>
    public override void TickAiLogic()
    {
        if (_runAi==false)
        {
            return;
        }

        if (CurrentAnimState == AnimState.Idle || CurrentAnimState == AnimState.Move)
        {
            if (BattleMgr.IsPause)
            {
                Idle();
                return;
            }
            var delta = Time.deltaTime;
            _checkCountTime += delta;
            if (_checkCountTime < _checkTime)
            {
                return;
            }
            else
            {
                Vector2 dir = CalcTargetDir();//计算目标位置
                                              //判断目标是否在攻击范围
                if (!InAttackRadius())
                {
                    //不在：设置移动方向，进入移动状态
                    SetDir(dir);
                    Move();
                }
                else
                {
                    //在：则停止移动，进入攻击状态
                    SetDir(Vector2.zero);
                    _attackCountTime += _checkCountTime;//确保移动过程是时间也加上
                    if (_attackCountTime > _attackTime)
                    {
                        //判断攻击间隔
                        //达到攻击时间，转向并攻击
                        SetAttackRotation(dir);
                        Attack(MonsterData.MonsterCfg.SkillId);
                        _attackCountTime = 0;
                    }
                    else
                    {
                        //未达到攻击时间，Idle等待
                        Idle();
                    }
                    _checkCountTime = 0;
                    _checkTime = PETools.RDInt(1, 5) * 1.0f / 10;
                }
            }
        }
    }

    private bool InAttackRadius()
    {
        EntityPlayer entityPlayer = BattleMgr.EntitySelfPlayer;
        if (entityPlayer == null || entityPlayer.CurrentAnimState == AnimState.Die)
        {
            _runAi = false;
            return false;

        }
        else
        {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            target.y = 0;
            self.y = 0;
            float dis = Vector3.Distance(target, self);
            return dis <= MonsterData.MonsterCfg.AttackDis;
        }
    }
    public override Vector2 CalcTargetDir()
    {
        EntityPlayer entityPlayer = BattleMgr.EntitySelfPlayer;
        if (entityPlayer == null || entityPlayer.CurrentAnimState == AnimState.Die)
        {
            _runAi = false;
            return Vector2.zero;
        }
        else
        {
            Vector3 target = entityPlayer.GetPos();
            Vector3 self = GetPos();
            return new Vector2(target.x - self.x, target.z - self.z).normalized;
        }
    }

    public override bool GetBreakState()
    {
        if (MonsterData.MonsterCfg.IsStop)
        {
            return SkillCfg == null || SkillCfg.IsBreak;
        }
        else
        {
            return false;
        }
    }

    public override void SetHpVal(int oldVal, int newVal)
    {
        if (MonsterData.MonsterCfg.MonsterType==MonsterType.Boss)
        {
            BattleSys.Instance.BattlesWind.SetBossHpBarVal(oldVal, newVal, BattleAttribute.Hp);
        }
        else
        {
            base.SetHpVal(oldVal, newVal);
        }
    }
}
