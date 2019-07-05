/****************************************************
    文件：StateDie.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:51:7
    功能：死亡状态
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateDie : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.CurrentAnimState = AnimState.Die;
        entityBase.RemoveSkillCb();
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        entityBase.SetAction(ConstRoot.ActionDie);
        entityBase.GetController().enabled = false;
        TimeSvc.Instance.AddTimeTask((int timeId) =>
        {
            entityBase.SetActive(false);
        }, ConstRoot.DieTimeLength);
    }
}