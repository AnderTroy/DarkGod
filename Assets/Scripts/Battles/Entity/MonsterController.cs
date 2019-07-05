/****************************************************
    文件：MonsterController.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:54:11
    功能：怪物控制 表现实体
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterController : Controller 
{
    //AI逻辑表现
    private void Update()
    {
        if (IsMove)
        {
            SetDir();
            SetMove();
        }
    }
    private void SetDir()
    {
        float angle = Vector2.SignedAngle(Dir, new Vector2(0, 1));
        Vector3 eulerAngles = new Vector3(0, angle, 0);
        transform.localEulerAngles = eulerAngles;
    }

    private void SetMove()
    {
        Character.Move(transform.forward * Time.deltaTime * ConstRoot.MonsterMoveSpeed);
        Character.Move(Vector3.down * Time.deltaTime * ConstRoot.MonsterMoveSpeed);
    }
}