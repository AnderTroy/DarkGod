/****************************************************
    文件：StateAttack.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:50:26
    功能：攻击技能状态
*****************************************************/

public class StateAttack : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Enter StateAttack.");
        entityBase.CurrentAnimState = AnimState.Attack;
        entityBase.SkillCfg = ResSvc.Instance.GetSkillCfgData((int) args[0]);
    }
    public void Exit(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Exit StateAttack");
        entityBase.ExitCurtSkill();
    }
    public void Process(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Process StateAttack");

        if (entityBase.EntityType==EntityType.Player)
        {
            entityBase.CanRlsSkill = false;
        }
        entityBase.SkillAttack((int)args[0]);//技能特效
    }
}