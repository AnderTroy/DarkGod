/****************************************************
    文件：PlayerController.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/13 19:36:3
    功能：角色控制 表现实体
*****************************************************/
using System;
using UnityEngine;

public class PlayerController : Controller 
{
    public GameObject AttackNomEft1;
    public GameObject AttackNomEft2;
    public GameObject AttackNomEft3;
    public GameObject AttackNomEft4;
    public GameObject AttackNomEft5;
    
    public GameObject AttackEft1;
    public GameObject AttackEft2;
    public GameObject AttackEft3;

    private Vector3 _cameraOffset;//相机与人物距离偏移
    
    private float _targetBlend;
    private float _currentBlend;

    public override void Init()
    {
        base.Init();
        CameraTransform = Camera.main.transform;
        _cameraOffset = CameraTransform.position - transform.position;

        if (AttackNomEft1 != null)
        {
            EftDictionary.Add(AttackNomEft1.name, AttackNomEft1);
        }
        if (AttackNomEft2 != null)
        {
            EftDictionary.Add(AttackNomEft2.name, AttackNomEft2);
        }
        if (AttackNomEft3 != null)
        {
            EftDictionary.Add(AttackNomEft3.name, AttackNomEft3);
        }
        if (AttackNomEft4 != null)
        {
            EftDictionary.Add(AttackNomEft4.name, AttackNomEft4);
        }
        if (AttackNomEft5 != null)
        {
            EftDictionary.Add(AttackNomEft5.name, AttackNomEft5);
        }

        if (AttackEft1!=null)
        {
            EftDictionary.Add(AttackEft1.name, AttackEft1);
        }
        if (AttackEft2!=null)
        {
            EftDictionary.Add(AttackEft2.name, AttackEft2);
        }
        if (AttackEft3!=null)
        {
            EftDictionary.Add(AttackEft3.name, AttackEft3);
        }
    }

    private void Update()
    {
        if (Math.Abs(_currentBlend - _targetBlend) > 0)
        {
            UpDateMixBlend();
        }
        if (IsMove)
        {
            SetDir();            //平滑相机跟随
            SetMove();           //角色移动
            SetCamFollower();    //相机跟随
        }

        if (IsSkillMove)
        {
            SetSkillMove();
            SetCamFollower();
        }
    }
    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1)) + CameraTransform.eulerAngles.y;
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles; 
    }

    private void SetMove()
    {
        Character.Move(transform.forward * Time.deltaTime * ConstRoot.PlayerMoveSpeed);
    }
    private void SetSkillMove()
    {
        Character.Move(transform.forward * Time.deltaTime * SkillMoveSpeed);
    }

    public void SetCamFollower()
    {
        if (CameraTransform!=null)
        {
            CameraTransform.position = transform.position + _cameraOffset;
        }
    }
    public void UpDateMixBlend()
    {
        if (Mathf .Abs(_currentBlend -_targetBlend)<ConstRoot.AccelerationSpeed*Time.deltaTime)
        {
            _currentBlend = _targetBlend;
        }
        else if (_currentBlend>_targetBlend)
        {
            _currentBlend -= ConstRoot.AccelerationSpeed * Time.deltaTime;
        }
        else
        {
            _currentBlend += ConstRoot.AccelerationSpeed * Time.deltaTime;
        }
        Animator.SetFloat("Blend", _currentBlend);
    }

    public override void SetBlend(float blend)
    {
        _targetBlend = blend;
    }
    public override void SetEft(string eftName,float actionTime)//播放粒子特效
    {
        if (EftDictionary.TryGetValue(eftName,out var thisGameObject))
        {
            thisGameObject.SetActive(true);
            TimeSvc.AddTimeTask((int timeId) =>
            {
                thisGameObject.SetActive(false);
            }, actionTime);
        }
    }
}