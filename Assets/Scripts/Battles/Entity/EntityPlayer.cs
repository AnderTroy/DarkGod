/****************************************************
    文件：EntityPlayer.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:53:20
    功能：玩家逻辑实体
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityPlayer : EntityBase 
{
    public EntityPlayer()
    {
        EntityType = EntityType.Player;
    }
    public override Vector2 GetDirInput()
    {
        return BattleMgr.GetDirInput();
    }
    public override Vector2 CalcTargetDir()
    {
        EntityMonster monster = FindClosedTarget();
        if (monster!=null)
        {
            Vector3 target = monster.GetPos();
            Vector3 self = GetPos();
            Vector2 dir = new Vector2(target.x - self.x, target.z - self.z);
            return dir.normalized;
        }
        else
        {
            return Vector2.zero;
        }
    }
    public override void SetHpVal(int oldVal, int newVal)
    {
        BattleSys.Instance.BattlesWind.SetSelfHpBtVal(newVal);
    }
    public override void SetDodge()
    {
        GameRoot.Instance.DynamicWind.SetSelfDodge();
    }
    public override void SetHurt(int hurt)
    {
        BattleSys.Instance.BattlesWind.SetHurt(hurt);
    }
    private EntityMonster FindClosedTarget()
    {
        List<EntityMonster> list = BattleMgr.EntityMonsters();
        if (list==null||list.Count==0)
        {
            return null;
        }
        Vector3 self = GetPos();
        EntityMonster targetMonster = null;
        float dis = 0;
        for (var i = 0; i < list.Count; i++)
        {
            var target = list[i].GetPos();
            if (i==0)
            {
                dis = Vector3.Distance(self, target);
                targetMonster = list[0];
            }
            else
            {
                var calcdis = Vector3.Distance(self, target);
                if (!(dis > calcdis)) continue;
                dis = calcdis;
                targetMonster = list[i];
            }
        }
        return targetMonster;
    }
}