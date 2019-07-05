/****************************************************
    文件：PlayerDataWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/15 11:21:12
    功能：角色信息展示界面
*****************************************************/
using UnityEngine;
using PEProtocol;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class PlayerDataWind : WindowRoot
{
    #region 角色属性面板
    public Text InfoText;
    public Text ExpText;
    public Text PowerText;
    public Text JobText;
    public Text FightText;
    public Text HpText;
    public Text HurtText;
    public Text DefenseText;

    public Text DataHpText;
    public Text DataAdText;
    public Text DataApText;
    public Text DataAdDefenseText;
    public Text DataApDefenseText;
    public Text DataDodgeText;
    public Text DataPierceText;
    public Text DataCriticalText;

    public Transform DerailTransform;
    public RawImage RawShow;
    #endregion

    private Vector2 startPoint=Vector2 .zero;
    protected override void InitWind()
    {
        base.InitWind();
        RegTouchEvt();
        SetActive(DerailTransform, false);
        RefreshUi();
    }
    //通过网络从缓存层中获取角色信息
    private void RefreshUi()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(InfoText, playerData.Name + " LV." + playerData.Level);
        SetText(ExpText, playerData.Exp + "/" + PeRoot.GetExpUpValByLv(playerData.Level));
        SetText(PowerText, playerData.Power + "/" + PeRoot.GetPowerLimit(playerData.Level));

        SetText(JobText, "暗夜刺客");
        SetText(FightText,PeRoot.GetFightByProps(playerData));
        SetText(HpText, playerData.Hp);
        SetText(HurtText,(playerData.Ad + playerData.Ap).ToString());
        SetText(DefenseText, (playerData.AdDefense + playerData.ApDefense).ToString ());

        //detail TODO
        SetText(DataHpText, playerData.Hp);
        SetText(DataAdText, playerData.Ad);
        SetText(DataApText, playerData.Ap);
        SetText(DataAdDefenseText, playerData.AdDefense);
        SetText(DataApDefenseText, playerData.ApDefense);
        SetText(DataDodgeText, playerData.Dodge + "%");
        SetText(DataPierceText, playerData.Pierce + "%");
        SetText(DataCriticalText, playerData.Critical + "%");
    }
    //关闭角色属性界面
    public void ClickCloseBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiOpenBtnAudio);
        MainCitySys.Instance.ClosePlayerDataWind();
    }
    //打开详细属性界面
    public void ClickDetailBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetActive(DerailTransform);
    }
    //关闭详细属性界面
    public void ClickCloseDetailBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetActive(DerailTransform,false);
    }
    //旋转角色事件
    private void RegTouchEvt()
    {
        OnClickDown(RawShow.gameObject, (PointerEventData evt) =>
        {
            startPoint = evt.position;
            MainCitySys.Instance.SetStartRote();
        });
        OnDragEvt(RawShow.gameObject, (PointerEventData evt) =>
        {
            float rotation = (startPoint.x - evt.position.x) * 0.4f;
            MainCitySys.Instance.SetPlayerRote(rotation);
        });
    }
}