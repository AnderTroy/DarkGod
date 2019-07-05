/****************************************************
    文件：EntityBase.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:52:10
    功能：逻辑实体基类
*****************************************************/

using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class EntityBase
{
    public AnimState CurrentAnimState = AnimState.None;
    public StateMgr StateMgr;
    public SkillMgr SkillMgr;
    public BattleMgr BattleMgr;
    protected Controller Controller = null;
    public SkillCfg SkillCfg;
    [HideInInspector]
    public bool CanController = true;
    public bool CanRlsSkill = true;
    public BattleAttribute BattleAttribute { get; protected set; }
    private int _hp;
    public Queue<int> comboQueue = new Queue<int>();
    public int nextSkillId = 0;
    public string NameCon { get; set; }
    public int Hp
    {
        get => _hp;
        set
        {
            PeRoot.Log("Hp:" + Hp + "to :" + value);
            SetHpVal(_hp, value);
            _hp = value;
        }
    }
    //技能位移回调ID
    public List<int> SkillMoveList = new List<int>();
    //技能伤害计算回调ID
    public List<int> SkillActionList = new List<int>();
    public int SkillEndId = -1;
    public EntityType EntityType = EntityType.None;
    public EntityState EntityState = EntityState.None;
    public AnimationClip[] GetAnimationClips()
    {
        return Controller != null ? Controller.Animator.runtimeAnimatorController.animationClips : null;
    }

    public AudioSource GetAudio()
    {
        return Controller.GetComponent<AudioSource>();
    }
    public void SetController(Controller controller)
    {
        Controller = controller;
    }
    public void SetActive(bool isActive = true)
    {
        if (Controller != null)
        {
            Controller.gameObject.SetActive(isActive);
        }
    }
    public CharacterController GetController()
    {
        return Controller.gameObject.GetComponent<CharacterController>();
    }
    public void Born()
    {
        StateMgr.ChangeStatus(this, AnimState.Born, null);
    }
    public void Idle()
    {
        StateMgr.ChangeStatus(this, AnimState.Idle, null);
    }
    public void Move()
    {
        StateMgr.ChangeStatus(this, AnimState.Move, null);
    }
    public void Attack(int skillId)
    {
        StateMgr.ChangeStatus(this, AnimState.Attack, skillId);
    }
    public void Hit()
    {
        StateMgr.ChangeStatus(this, AnimState.Hit, null);
    }
    public void Die()
    {
        StateMgr.ChangeStatus(this, AnimState.Die, null);
    }
    public void ExitCurtSkill()
    {
        CanController = true;
 

        if (SkillCfg!=null)
        {
            if (!SkillCfg.IsBreak)
            {
                EntityState = EntityState.None;
            }

            if (SkillCfg.IsComboSkill)
            {
                nextSkillId = comboQueue.Count > 0 ? comboQueue.Dequeue() : 0;
            }
            SkillCfg = null;
        }
        SetAction(ConstRoot.ActionDefault);
    }

    public virtual void SetBattleAttribute(BattleAttribute attribute)
    {
        Hp = attribute.Hp;
        BattleAttribute = attribute;
    }
    public virtual void SetAttackRotation(Vector2 dir, bool isOffset = false)
    {
        if (Controller != null)
        {
            if (isOffset)
            {
                Controller.SetAttackRotationCam(dir);
            }
            else
            {
                Controller.SetAttackRotationLocal(dir);
            }
        }
    }
    public virtual Vector2 CalcTargetDir()
    {
        return Vector2.zero;
    }
    public virtual void SetBlend(float blend)
    {
        if (Controller != null)
        {
            Controller.SetBlend(blend);
        }
    }
    public virtual void SetDir(Vector2 dir)
    {
        if (Controller != null)
        {
            Controller.Dir = dir;
        }
    }
    public virtual void SetAction(int action)
    {
        if (Controller != null)
        {
            Controller.SetAction(action);
        }
    }
    public virtual void SetEft(string eftName, float actionTime)
    {
        if (Controller != null)
        {
            Controller.SetEft(eftName, actionTime);
        }
    }
    public virtual void SetSkillMoveState(bool isMove, float speed = 0f)
    {
        if (Controller != null)
        {
            Controller.SetSkillMove(isMove, speed);
        }
    }
    public virtual void SkillAttack(int skillId)
    {
        SkillMgr.SkillAttack(this, skillId);
    }
    public virtual Vector2 GetDirInput()
    {
        return Vector2.zero;
    }
    public virtual Vector3 GetPos()
    {
        return Controller.transform.position;
    }
    public virtual Transform GetTrans()
    {
        return Controller.transform;
    }

    #region 战斗信息显示
    public virtual void SetDodge()
    {
        if (Controller != null)
        {
            GameRoot.Instance.DynamicWind.SetDodge(NameCon);
        }
    }
    public virtual void SetCritical(int critical)
    {
        if (Controller != null)
        {
            GameRoot.Instance.DynamicWind.SetCritical(NameCon, critical);
        }
    }
    public virtual void SetHurt(int hurt)
    {
        if (Controller != null)
        {
            GameRoot.Instance.DynamicWind.SetHurt(NameCon, hurt);
        }
    }
    public virtual void SetHpVal(int oldVal, int newVal)
    {
        if (Controller != null)
        {
            GameRoot.Instance.DynamicWind.SetHpVal(NameCon, oldVal, newVal);
        }
    }
    #endregion

    public virtual void TickAiLogic()
    {

    }
    public virtual bool GetBreakState()
    {
        return true;
    }

    public void RemoveMoveCb(int timeId)
    {
        int index = -1;
        for (int i = 0; i < SkillMoveList.Count; i++)
        {
            if (SkillMoveList[i]==timeId)
            {
                index = i;
                break;
            }
        }
        if (index!=-1)
        {
            SkillMoveList.RemoveAt(index);
        }
    }
    public void RemoveActionCb(int timeId)
    {
        int index = -1;
        for (int i = 0; i < SkillActionList.Count; i++)
        {
            if (SkillActionList[i] == timeId)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            SkillActionList.RemoveAt(index);
        }
    }

    public void RemoveSkillCb()
    {
        SetDir(Vector2.zero);
        SetSkillMoveState(false);
        foreach (var timeId in SkillMoveList)
        {
            TimeSvc.Instance.DelTask(timeId);
        }
        foreach (var timeId in SkillActionList)
        {
            TimeSvc.Instance.DelTask(timeId);
        }
        //攻击中断 删除定时回调
        if (SkillEndId != -1)
        {
            TimeSvc.Instance.DelTask(SkillEndId);
            SkillEndId = -1;
        }
        SkillMoveList.Clear();
        SkillActionList.Clear();
        //清空连招数据
        if (nextSkillId != 0 || comboQueue.Count > 0)
        {
            nextSkillId = 0;
            comboQueue.Clear();

            BattleMgr.LastAttackTime = 0;
            BattleMgr.ComboIndex = 0;
        }
    }

}