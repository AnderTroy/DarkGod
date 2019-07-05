/****************************************************
    文件：TargetData.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/6/1 22:23:54
    功能：地图触发数据
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetData : MonoBehaviour
{
    public MapMgr MapMgr;
    public int TriggerIndex;
    public void OnTriggerExit(Collider other)
    {
        if (other.tag=="Player")
        {
            if (MapMgr!=null)
            {
                MapMgr.TriggerMonsterBorn(this,TriggerIndex);
            }
        }
    }
}