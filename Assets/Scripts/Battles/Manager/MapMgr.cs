/****************************************************
    文件：MapMgr.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:48:21
    功能：地图怪物动态生成
*****************************************************/
using System.Collections;
using System.Collections.Generic;
using PEProtocol;
using UnityEngine;

public class MapMgr : MonoBehaviour
{
    private int waveIndex = 1;
    public BattleMgr BattleMgr;
    public TargetData[] TargetDatas;
    public void Init(BattleMgr battle)
    {
        BattleMgr = battle;
        //实例化第一批怪物
        BattleMgr.LoadMonsterByWaveId(waveIndex);
        PeRoot.Log("Init MapMgr Done.");
    }

    public void TriggerMonsterBorn(TargetData targetData,int index)
    {
        if (BattleMgr!=null)
        {
            BoxCollider box = targetData.GetComponent<BoxCollider>();
            box.isTrigger = false;
            BattleMgr.LoadMonsterByWaveId(index);
            BattleMgr.ActiveCurrentBathMonsters();
            BattleMgr.TriggerCheck = true;
        }
    }

    public bool SetNextTriggerOn()
    {
        waveIndex += 1;
        foreach (var t in TargetDatas)
        {
            if (t.TriggerIndex==waveIndex)
            {
                BoxCollider box = t.GetComponent<BoxCollider>();
                box.isTrigger = true;
                return true;
            }
        }
        return false;
    }
}