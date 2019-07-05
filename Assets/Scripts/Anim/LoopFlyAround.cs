/****************************************************
    文件：LoopFlyAround.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/7 10:4:17
    功能：飞龙动画循环
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoopFlyAround : MonoBehaviour 
{
    private Animation anim;
    private void Awake()
    {
        anim = GetComponent<Animation>();
    }
    private void Start ()
    {
        InvokeRepeating("LoopFly", 0, 10);
    }
    private void LoopFly()
    {
        anim.Play();
    }
}