/****************************************************
    文件：DynamicWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/7 17:20:11
    功能：动态元素窗口Tips
*****************************************************/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DynamicWind : WindowRoot
{
    public Animation TipsAnimation;
    public Animation SelfAnim;
    public Text TextTips;
    public Transform HpItemRoot;


    private bool _isTipsShow = false;
    private readonly Queue<string> _tipsQueue = new Queue<string>();//创建队列
    private readonly Dictionary<string, ItemEntityHp> _itemDic = new Dictionary<string, ItemEntityHp>();
    protected override void InitWind()
    {
        base.InitWind();
        SetActive(TextTips, false);
    }

    #region Add Tips
    public void AddTips(string tips)
    {
        lock (_tipsQueue)
        {
            _tipsQueue.Enqueue(tips);//添加元素
        }
    }
    void Update()
    {
        if (_tipsQueue.Count > 0 && _isTipsShow == false && _tipsQueue.Count <= 2)
        {
            lock (_tipsQueue)
            {
                string tips = _tipsQueue.Dequeue();//移除并返回在 Queue 的开头的对象。
                _isTipsShow = true;
                SetTips(TipsAnimation, TextTips, tips);
            }
        }
        else if (_tipsQueue.Count > 2)
        {
            lock (_tipsQueue)
            {
                string tips = _tipsQueue.Dequeue();
            }
        }
    }
    private void SetTips(Animation anim, Text text, string tips)
    {
        SetActive(text);
        SetText(text, tips);

        AnimationClip animationClip = anim.GetClip("Tips");
        anim.Play();
        //通过协程达到延迟关闭激活状态
        StartCoroutine(AnimPlayDone(animationClip.length, () =>
        {
            SetActive(text, false);
            _isTipsShow = false;
        }));
    }

    private IEnumerator AnimPlayDone(float sec, Action closed)
    {
        yield return new WaitForSeconds(sec);
        // if (closed!=null){ closed(); } 
        closed?.Invoke();//简化委托调用
    }
    #endregion

    public void AddHpItemInfo(string itemName,Transform trans, int hp)
    {
        if (_itemDic.TryGetValue(itemName, out var item))
        {
            return;
        }
        else
        {
            GameObject gameObj = ResSvc.LoadPrefab(PathDefine.HpItemPrefab);
            gameObj.transform.SetParent(HpItemRoot);
            gameObj.transform.localPosition = new Vector3(0, -1000, 0);
            ItemEntityHp entityHp = gameObj.GetComponent<ItemEntityHp>();
            entityHp.SetItemInfo(trans,hp);
            _itemDic.Add(itemName, entityHp);
        }
    }
    public void RemoveHpItemInfo(string itemName)
    {
        if (_itemDic.TryGetValue(itemName, out var item))
        {
            Destroy(item.gameObject);
            _itemDic.Remove(itemName);
        }
    }

    public void RemoveAllHpItemInfo()
    {
        foreach (var item in _itemDic)
        {
            Destroy(item.Value.gameObject);
        }

        _itemDic.Clear();
    }
    public void SetDodge(string key)
    {
        if (_itemDic.TryGetValue(key, out var item))
        {
            item.SetDodge();
        }
    }
    public void SetCritical(string key,int critical)
    {
        if (_itemDic.TryGetValue(key, out var item))
        {
            item.SetCritical(critical);
        }
    }
    public void SetHurt(string key,int hurt)
    {
        if (_itemDic.TryGetValue(key, out var item))
        {
            item.SetHurt(hurt);
        }
    }
    public void SetHpVal(string key, int oldVal,int newVal)
    {
        if (_itemDic.TryGetValue(key, out var item))
        {
            item.SetVal(oldVal,newVal);
        }
    }

    public void SetSelfDodge()
    {
        SelfAnim.Stop();
        SelfAnim.Play();
    }
}