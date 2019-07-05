/****************************************************
    文件：PEListener.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/13 10:51:18
    功能：UI事件监听
*****************************************************/
using System;
using UnityEngine;
using UnityEngine.EventSystems;
public class PEListener : MonoBehaviour,IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public Action<object> OnClick;
    public Action<PointerEventData> OnClickDown;
    public Action<PointerEventData> OnClickUp;
    public Action<PointerEventData> OnDragEvt;
    public object args;
    public void OnPointerClick(PointerEventData eventData) => OnClick?.Invoke(args);

    public void OnPointerDown(PointerEventData eventData) => OnClickDown?.Invoke(eventData);

    public void OnPointerUp(PointerEventData eventData) => OnClickUp?.Invoke(eventData);

    public void OnDrag(PointerEventData eventData) => OnDragEvt?.Invoke(eventData);
}