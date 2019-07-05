/****************************************************
    文件：StateHit.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:50:35
    功能：受伤状态
*****************************************************/
using UnityEngine;

public class StateHit : IState
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.CurrentAnimState = AnimState.Hit;
        entityBase.RemoveSkillCb();
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        
    }

    public void Process(EntityBase entityBase, params object[] args)
    {
        if (entityBase.EntityType==EntityType.Player)
        {
            entityBase.CanRlsSkill = false;
        }
        entityBase.SetDir(Vector2.zero);
        entityBase.SetAction(ConstRoot.ActionHit);

        if (entityBase.EntityType==EntityType.Player)
        {
            AudioSource charAudio = entityBase.GetAudio();
            AudioSvc.Instance.PlayCharAudio(ConstRoot.PlayerHurtAudio, charAudio);
        }

        TimeSvc.Instance.AddTimeTask((int timeId) =>
        {
            entityBase.SetAction(ConstRoot.ActionDefault);
            entityBase.Idle();
        }, GetHitAnimTime(entityBase)*100f);
    }

    private float GetHitAnimTime(EntityBase entity)
    {
        AnimationClip[] clips = entity.GetAnimationClips();
        foreach (var t in clips)
        {
            string clipName = t.name;
            if (clipName.Contains("Hit")|| clipName.Contains("hit"))
            {
                return t.length;
            }
        }
        return 1;
    }
}