/****************************************************
    文件：StateBorn.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:49:38
    功能：出生状态
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateBorn : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.CurrentAnimState = AnimState.Born;
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        entityBase.SetAction(ConstRoot.ActionBorn);
        TimeSvc.Instance.AddTimeTask((int timeId) => { entityBase.SetAction(ConstRoot.ActionDefault); }, 500);
    }
}