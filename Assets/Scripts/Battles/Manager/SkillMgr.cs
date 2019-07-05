/****************************************************
    文件：SkillMgr.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:48:35
    功能：技能管理器
*****************************************************/

using System;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class SkillMgr : MonoBehaviour
{
    private ResSvc _resSvc;
    private TimeSvc _timeSvc;
    public void Init()
    {
        _resSvc = ResSvc.Instance;
        _timeSvc = TimeSvc.Instance;
        PeRoot.Log("Init SkillMgr Done");
    }
    public void SkillAttack(EntityBase entityBase, int skillId)
    {
        entityBase.SkillMoveList.Clear();
        entityBase.SkillActionList.Clear();
        AttackDamage(entityBase, skillId);
        AttackEft(entityBase, skillId);
    }
    public void AttackDamage(EntityBase entityBase, int skillId)
    {
        SkillCfg skillCfg = _resSvc.GetSkillCfgData(skillId);
        List<int> actionLst = skillCfg.SkillActionLst;
        int sum = 0;
        for (var i = 0; i < actionLst.Count; i++)
        {
            SkillActionCfg actionCfg = _resSvc.GetSkillActionCfgData(actionLst[i]);
            sum += actionCfg.DelayTime; //计算延时时间
            int index = i;
            if (sum > 0)
            {
                int attackId = _timeSvc.AddTimeTask((int timeId) =>
                {
                    if (entityBase == null) return;
                    SkillAction(entityBase, skillCfg, index);
                    entityBase.RemoveActionCb(timeId);
                }, sum);
                entityBase.SkillActionList.Add(attackId);
            }
            else
            {
                SkillAction(entityBase, skillCfg, index);
            }
        }
    }
    public void AttackEft(EntityBase caster, int skillId)//技能效果表现
    {
        SkillCfg skillCfg = _resSvc.GetSkillCfgData(skillId);

        if (!skillCfg.IsCollide)
        {
            //忽略碰撞
            Physics.IgnoreLayerCollision(10, 11);
            TimeSvc.Instance.AddTimeTask((int timeId) => { Physics.IgnoreLayerCollision(10, 11, false); },
                skillCfg.SkillTime);
        }

        if (caster.EntityType == EntityType.Player)
        {
            if (caster.GetDirInput() == Vector2.zero)
            {
                //自动攻击最近的怪物
                Vector2 dir = caster.CalcTargetDir();
                if (dir != Vector2.zero)
                {
                    caster.SetAttackRotation(dir);
                }
            }
            else
            {
                caster.SetAttackRotation(caster.GetDirInput(), true);
            }
        }
        caster.SetAction(skillCfg.AnimAction);
        caster.SetEft(skillCfg.EftName, skillCfg.SkillTime);

        CalcSkillMove(caster, skillCfg);

        caster.CanController = false;
        caster.SetDir(Vector2.zero);//停止方向控制移动

        if (!skillCfg.IsBreak)
        {
            caster.EntityState = EntityState.BaseState;
        }

        caster.SkillEndId = _timeSvc.AddTimeTask((int timeId) =>
        {
            caster.Idle();
        }, skillCfg.SkillTime);
    }

    public void SkillAction(EntityBase caster, SkillCfg skillCfg, int index)
    {

        SkillActionCfg actionCfg = _resSvc.GetSkillActionCfgData(skillCfg.SkillActionLst[index]);
        int damage = skillCfg.SkillDamageLst[index];

        if (caster.EntityType == EntityType.Player)
        {
            //获取场景里的怪物实体
            List<EntityMonster> entityMonsters = caster.BattleMgr.EntityMonsters();
            foreach (var temp in entityMonsters)
            {
                EntityMonster monster = temp;
                if (InRadius(caster.GetPos(), monster.GetPos(), actionCfg.Radius)
                    && InAngle(caster.GetTrans(), monster.GetPos(), actionCfg.Angle))
                {
                    CalcDamage(caster, monster, skillCfg, damage);
                }
            }
        }
        else if (caster.EntityType == EntityType.Monster)
        {
            EntityPlayer player = caster.BattleMgr.EntitySelfPlayer;
            if (InRadius(caster.GetPos(), player.GetPos(), actionCfg.Radius)
                && InAngle(caster.GetTrans(), player.GetPos(), actionCfg.Angle))
            {
                CalcDamage(caster, player, skillCfg, damage);
            }
        }
    }

    private bool InRadius(Vector3 player, Vector3 monster, float radius)
    {
        float dis = Vector3.Distance(player, monster);
        return dis < radius;
    }
    private bool InAngle(Transform player, Vector3 monster, float angle)
    {
        if (!(Math.Abs(angle - 360) > 0))
        {
            return true;
        }
        else
        {
            Vector3 start = player.forward;
            Vector3 dir = (monster - player.position).normalized;
            float angles = Vector3.Angle(start, dir);
            return angles <= angle / 2;
        }
    }

    private readonly System.Random _random = new System.Random();
    private void CalcDamage(EntityBase caster, EntityBase target, SkillCfg skillCfg, int damage)
    {
        int damageSum = damage;
        if (skillCfg.DamageType == DamageType.Ad)
        {
            //计算闪避概率
            int dodgeNum = PETools.RDInt(1, 100, _random);
            if (dodgeNum <= target.BattleAttribute.Dodge)
            {
                //PeRoot.Log("闪避Rate:" + dodgeNum + "/" + target.BattleAttribute.Dodge);
                target.SetDodge();
                return;
            }
            //属性加成
            damageSum += caster.BattleAttribute.Ad;
            //物理抗性
            int defense = (int)((1 - caster.BattleAttribute.Pierce / 100.0f) * target.BattleAttribute.AdDefense);
            damageSum -= defense;
            //暴击伤害
            int criticalNum = PETools.RDInt(1, 100, _random);
            if (criticalNum <= caster.BattleAttribute.Critical)
            {
                float criticalRate = 1 + (PETools.RDInt(1, 100, _random) / 100.0f);
                damageSum = (int)criticalRate * damageSum;
                //PeRoot.Log("暴击Rate:" + criticalNum + "/" + caster.BattleAttribute.Critical);
            }
            //PeRoot.Log("伤害Rate:" + damageSum);
        }
        else if (skillCfg.DamageType == DamageType.Ap)
        {
            //属性加成
            damageSum += caster.BattleAttribute.Ap;
            //魔法抗性
            int defense = (int)((1 - caster.BattleAttribute.Pierce / 100.0f) * target.BattleAttribute.ApDefense);
            damageSum -= defense;
            //暴击伤害
            int criticalNum = PETools.RDInt(1, 100, _random);
            if (criticalNum <= caster.BattleAttribute.Critical)
            {
                float criticalRate = 1 + (PETools.RDInt(1, 100, _random) / 100.0f);
                damageSum = (int)criticalRate * damageSum;
                target.SetCritical(damageSum);
                //PeRoot.Log("暴击Rate:" + criticalNum + "/" + caster.BattleAttribute.Critical);
            }
        }
        if (damageSum < 0)
        {
            damageSum = 0;
            target.SetDodge();
            return;
        }
        target.SetHurt(damageSum);
        if (target.Hp < damageSum)
        {
            target.Hp = 0;
            target.Die();

            if (target.EntityType==EntityType.Player)
            {
                target.BattleMgr.EndBattle(false, 0);
                target.BattleMgr.EntitySelfPlayer = null;
            }
            else if(target.EntityType == EntityType.Monster)
            {
                target.BattleMgr.RemoveMonster(target.NameCon);
            }
            
        }
        else
        {
            target.Hp -= damageSum;
            if (caster.EntityState == EntityState.None&&target.GetBreakState())
            {
                target.Hit();
            }
        }
    }
    private void CalcSkillMove(EntityBase caster, SkillCfg skillCfg)//技能延时
    {
        List<int> skillMoveList = skillCfg.SkillMoveLst;//技能分段列表
        int sum = 0;
        for (int i = 0; i < skillMoveList.Count; i++)
        {
            SkillMoveCfg skillMoveCfg = _resSvc.GetSkillMoveCfgData(skillCfg.SkillMoveLst[i]);
            float speed = skillMoveCfg.MoveDis / (skillMoveCfg.MoveTime / 1000f);
            sum += skillMoveCfg.DelayTime;
            if (sum > 0)
            {
                int moveId=_timeSvc.AddTimeTask((int timeId) =>
                {
                    caster.SetSkillMoveState(true, speed);
                    caster.RemoveMoveCb(timeId);
                }, sum);
                caster.SkillMoveList.Add(moveId);
            }
            else
            {
                caster.SetSkillMoveState(true, speed);
            }

            sum += skillMoveCfg.MoveTime;
            int stopId = _timeSvc.AddTimeTask((int timeId) =>
            {
                caster.SetSkillMoveState(false);
                caster.RemoveMoveCb(timeId);
            }, sum);
            caster.SkillMoveList.Add(stopId);
        }
    }
}