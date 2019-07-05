/****************************************************
    文件：StateMgr.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:48:48
    功能：状态管理器
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class StateMgr : MonoBehaviour
{
    private readonly Dictionary<AnimState, IState> _fsm = new Dictionary<AnimState, IState>();
    public void Init()
    {
        _fsm.Add(AnimState.Born, new StateBorn());
        _fsm.Add(AnimState.Idle, new StateIdle());
        _fsm.Add(AnimState.Move, new StateMove());
        _fsm.Add(AnimState.Hit, new StateHit());
        _fsm.Add(AnimState.Die, new StateDie());
        _fsm.Add(AnimState.Attack, new StateAttack());

        PeRoot.Log("Init StateMgr Done.");
    }

    public void ChangeStatus(EntityBase entityBase, AnimState animTargetState, params object[] args)
    {
        if (entityBase.CurrentAnimState==animTargetState)
        {
            return;
        }

        if (_fsm.ContainsKey(animTargetState))
        {
            if (entityBase.CurrentAnimState != AnimState.None)
            {
                _fsm[entityBase.CurrentAnimState].Exit(entityBase,args);
            }

            _fsm[animTargetState].Enter(entityBase, args);
            _fsm[animTargetState].Process(entityBase, args);
        }
    }
}