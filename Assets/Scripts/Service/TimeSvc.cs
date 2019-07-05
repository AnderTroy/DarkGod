/****************************************************
    文件：TimeSvc.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/22 8:43:26
    功能：计时服务
*****************************************************/
using System;
using PEProtocol;
using UnityEngine;

public class TimeSvc : SystemRoot
{
    public static TimeSvc Instance = null;
    private PETimer PETimer;
    public void InitSvc()
    {
        Instance = this;
        Debug.Log(Instance.GetType());
        PETimer = new PETimer();
        PETimer.SetLog((string info) => { PeRoot.Log(info); });//设置输出日志
    }
    public void Update()
    {
        PETimer.Update();
    }

    public int AddTimeTask(Action<int> callBack, double delay, PETimeUnit timeUnit = PETimeUnit.Millisecond,
        int count = 1)
    {
        return PETimer.AddTimeTask(callBack, delay, timeUnit, count);
    }

    public double GetNowTime()
    {
        return PETimer.GetMillisecondsTime();
    }

    public void DelTask(int timeId)
    {
        PETimer.DeleteTimeTask(timeId);
    }
}