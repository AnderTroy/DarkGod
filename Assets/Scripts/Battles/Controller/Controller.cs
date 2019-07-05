/****************************************************
    文件：Controller.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 10:1:0
    功能：表现实体控制器抽象基类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    protected bool IsMove = false;
    private Vector2 _dir = Vector2.zero;
    protected Transform CameraTransform;//相机位置
    public Vector2 Dir
    {
        get => _dir;
        set
        {
            IsMove = value != Vector2.zero;
            _dir = value;
        }
    }
    public Animator Animator;//动画控制器
    public CharacterController Character;//控制器
    public Transform HpTrans;
    protected Dictionary<string, GameObject> EftDictionary = new Dictionary<string, GameObject>();
    protected TimeSvc TimeSvc;
    protected bool IsSkillMove = false;
    protected float SkillMoveSpeed = 0;
    public virtual void Init()
    {
        TimeSvc = TimeSvc.Instance;
    }

    public virtual void SetBlend(float blend)
    {
        Animator.SetFloat("Blend", blend);
    }
    public virtual void SetAction(int action)
    {
        Animator.SetInteger("Action", action);
    }
    public virtual void SetEft(string eftName, float actionTime)
    {
    }

    public void SetSkillMove(bool isMove, float skillMoveSpeed = 0f)
    {
        IsSkillMove = isMove;
        SkillMoveSpeed = skillMoveSpeed;
    }

    public virtual void SetAttackRotationLocal(Vector2 localDir)
    {
        float angle = Vector2.SignedAngle(localDir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
    public virtual void SetAttackRotationCam(Vector2 camDir)
    {
        float angle = Vector2.SignedAngle(camDir, new Vector2(0, 1)) + CameraTransform.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }
}