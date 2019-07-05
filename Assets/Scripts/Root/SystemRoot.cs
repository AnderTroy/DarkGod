/****************************************************
    文件：SystemRoot.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/7 11:28:48
    功能：系统基类
*****************************************************/
using UnityEngine;
public class SystemRoot : MonoBehaviour
{
    protected ResSvc ResSvc;
    protected AudioSvc AudioSvc;
    protected NetSvc NetSvc;
    protected TimeSvc TimeSvc;
    public virtual void InitSys()
    {
        ResSvc = ResSvc.Instance;
        AudioSvc = AudioSvc.Instance;
        NetSvc = NetSvc.Instance;
        TimeSvc = TimeSvc.Instance;
    }
}