/****************************************************
    文件：WindowsRoots.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/6 19:53:10
    功能：UI窗口界面基类
*****************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = System.Diagnostics.Debug;

public class WindowRoot : MonoBehaviour
{
    protected ResSvc ResSvc ;
    protected AudioSvc AudioSvc ;
    protected NetSvc NetSvc ;
    protected TimeSvc TimeSvc ;
    /// <summary>
    /// 启用和禁用UI界面
    /// </summary>
    public void SetWindState(bool isActive = true)
    {
        if (gameObject.activeSelf != isActive)
        {
            SetActive(gameObject, isActive);
        }
        if (isActive)
        {
            InitWind();
        }
        else
        {
            ClearWind();
        }
    }

    public bool GetWindState()
    {
        return gameObject.activeSelf;
    }
    protected virtual void InitWind()
    {
        ResSvc = ResSvc.Instance;
        AudioSvc = AudioSvc.Instance;
        NetSvc = NetSvc.Instance;
        TimeSvc = TimeSvc.Instance;
    }

    protected virtual void ClearWind()
    {
        ResSvc = null;
        AudioSvc = null;
        NetSvc = null;
        TimeSvc = null;
    }
    #region Tool Functions

    protected void SetActive(GameObject go, bool isActive = true)
    {
        go.SetActive(isActive);
    }
    protected void SetActive(Transform trans, bool state = true)
    {
        trans.gameObject.SetActive(state);
    }
    protected void SetActive(RectTransform rectTrans, bool state = true)
    {
        rectTrans.gameObject.SetActive(state);
    }
    protected void SetActive(Image img, bool state = true)
    {
        img.transform.gameObject.SetActive(state);
    }
    protected void SetActive(Text txt, bool state = true)
    {
        txt.transform.gameObject.SetActive(state);
    }
    protected void SetText(Text txt, string context = "")
    {
        txt.text = context;
    }
    protected void SetText(Transform trans, int num = 0)
    {
        SetText(trans.GetComponent<Text>(), num);
    }
    protected void SetText(Transform trans, string context = "")
    {
        SetText(trans.GetComponent<Text>(), context);
    }
    protected void SetText(Text txt, int num = 0)
    {
        SetText(txt, num.ToString());
    }
    protected void SetSprite(Image image, string path)
    {
        Sprite sprite = ResSvc.LoadSprite(path, true);
        image.sprite = sprite;
    }
    protected T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        T t = go.GetComponent<T>();
        if (t == null)
        {
            t = go.AddComponent<T>();
        }
        return t;
    }
    protected Transform GetTransform(Transform trans, string thisName)
    {
        Debug.Assert(trans != null, nameof(trans) + " != null");
        return trans.Find(thisName);
    }

    #endregion

    #region Click Evts

    protected void OnClick(GameObject go, Action<object> action,object args)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.OnClick = action;
        listener.args = args;
    }
    protected void OnClickDown(GameObject go, Action<PointerEventData> action)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.OnClickDown = action;
    }
    protected void OnClickUp(GameObject go, Action<PointerEventData> action)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.OnClickUp = action;
    }
    protected void OnDragEvt(GameObject go, Action<PointerEventData> action)
    {
        PEListener listener = GetOrAddComponent<PEListener>(go);
        listener.OnDragEvt = action;
    }
    #endregion
}