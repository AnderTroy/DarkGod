/****************************************************
    文件：LoadingWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/6 14:7:39
    功能：加载进度条
*****************************************************/
using UnityEngine;
using UnityEngine.UI;
public class LoadingWind : WindowRoot
{
    public Text TextTip;//文字提示
    public Text TxtPrg;//百分比数值
    public Image Load;//进度条
    public Image ImHead;//粒子进度条
    private float tempProgress;//临时变量，储存进度变化
    private float loadWidth;//保存进度条的长度
    protected override void InitWind()
    {
        base.InitWind();
        tempProgress = 0;
        loadWidth = Load.GetComponent<RectTransform>().sizeDelta.x;
        SetText(TextTip, "Tips:加载游戏...");//TextTip.text = "Tips:这里是一条小提示...";
        SetText(TxtPrg, "0%");//TxtPrg.text = "0%";
        Load.fillAmount = 0;//初始化进度条
        ImHead.transform.localPosition = new Vector3(-890f, 0, 0);
    }
    public void SetProgress(float prg)
    {
        tempProgress = Mathf.Lerp(tempProgress, prg, Time.deltaTime*10);//设置进度条渐变到目标值
        SetText(TxtPrg, (int) (tempProgress * 100) + "%");
        SetText(TxtPrg, (int) (tempProgress / 9 * 10 * 100) + "%");
        Load.fillAmount = tempProgress / 9 * 10;
        float pointX = loadWidth * tempProgress / 9 * 10 - 890;
        ImHead.GetComponent<RectTransform>().anchoredPosition = new Vector2(pointX, 0);
        if (tempProgress / 9 * 10 >= 0.995)//当进度到99.5%时，设置进度为100%，跳转下个场景
        {
            SetText(TxtPrg, 100 + "%");
            SetText(TxtPrg, 100 + "%");
            Load.fillAmount = 1;
            ImHead.GetComponent<RectTransform>().anchoredPosition = new Vector2(885, 0);
            ResSvc.Instance.SceneAsync.allowSceneActivation = true;
        }
    }
}