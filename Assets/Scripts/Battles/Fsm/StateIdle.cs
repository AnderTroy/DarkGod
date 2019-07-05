/****************************************************
    文件：StateIdle.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:49:55
    功能：待机状态
*****************************************************/
using PEProtocol;
using UnityEngine;

public class StateIdle : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.CurrentAnimState = AnimState.Idle;
        entityBase.SetDir(Vector2.zero);

        entityBase.SkillEndId = -1;
        //PeRoot.Log("Enter StateIdle.");
    }
    public void Exit(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Exit StateIdle");
    }
    public void Process(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Process StateIdle");
        if (entityBase.nextSkillId != 0)
        {
            entityBase.Attack(entityBase.nextSkillId);
        }
        else
        {
            if (entityBase.EntityType == EntityType.Player)
            {
                entityBase.CanRlsSkill = true;
            }
            if (entityBase.GetDirInput() != Vector2.zero)
            {
                entityBase.Move();
                entityBase.SetDir(entityBase.GetDirInput());
            }
            else
            {
                entityBase.SetBlend(ConstRoot.BlendIdle);
            }
        }

    }
}