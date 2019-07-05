/****************************************************
    文件：MainCityWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/12 9:39:6
    功能：主城UI界面
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainCityWind : WindowRoot
{
    #region Player
    public Text FightText;//战斗力
    public Text PowerText;//体力
    public Image FillAmountByPower;//体力进度条
    public Text LevelText;//等级
    public Text NameText;//名字
    public Text VipText;//VIP等级
    public Text ExpText;//经验值百分比

    public Text CoinText;
    public Text DiamondText;
    public Text CrystalText;
    #endregion
    public Image AutoGuide;
    public Transform ExpPrgTransform;//经验值进度条

    public Animation Anim;//任务栏收缩动画
    private bool isMenuOpen = true;//任务栏收缩状态
    private AutoGuideCfg autoGuideCfg;
    #region MoveControler
    public Image TouchImage;//摇杆区域
    public Image DirBg;//摇杆背景
    public Image DirPoint;//摇杆中心点
    private float pointDis;//摇杆中心点偏移
    private Vector2 startPosition = Vector2.zero;//按下位置
    private Vector2 originPosition = Vector2.zero;//摇杆初始点
    #endregion

    protected override void InitWind()
    {
        base.InitWind();
        pointDis = Screen.height * 1.0f / ConstRoot.ScreenStandardHeight * ConstRoot.ScreenOriginDis;
        originPosition = DirBg.transform.position;
        SetActive(DirPoint, false);
        TouchEvents();
        RefreshUi();
    }
    /// <summary>
    /// 角色信息面板
    /// </summary>
    public void RefreshUi()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(FightText, PeRoot.GetFightByProps(playerData));
        SetText(PowerText, playerData.Power + "/" + PeRoot.GetPowerLimit(playerData.Level));
        FillAmountByPower.fillAmount = playerData.Power * 1.0f / PeRoot.GetPowerLimit(playerData.Level);
        SetText(LevelText, playerData.Level);
        SetText(NameText, playerData.Name);
        SetText(VipText, playerData.Vip);
        int expPrg = (int)(playerData.Exp * 1.0f / PeRoot.GetExpUpValByLv(playerData.Level) * 100);
        SetText(ExpText, expPrg + "%");
        int index = expPrg / 10;
        SetText(CoinText, playerData.Coin);
        SetText(DiamondText, playerData.Diamond);
        SetText(CrystalText, playerData.Crystal);
        #region ExpPrg
        //GridLayoutGroup gridLayoutGroup = ExpPrgTransform.GetComponent<GridLayoutGroup>();
        //float globalRate = 1.0f * ConstRoot.ScreenStandardHeight / Screen.height;
        //float screenWidth = Screen.width * globalRate;
        //float width = (screenWidth - 1);
        //gridLayoutGroup.cellSize = new Vector2(width, 11);

        for (int i = 0; i < ExpPrgTransform.childCount; i++)
        {
            Image image = ExpPrgTransform.GetChild(i).GetComponent<Image>();
            if (i < index)
            {
                image.fillAmount = 1;
            }
            else if (i == index)
            {
                image.fillAmount = expPrg % 10 * 1.0f / 10;
            }
            else
            {
                image.fillAmount = 0;
            }
        }
        #endregion
        //设置自动任务图标
        autoGuideCfg = ResSvc.GetGuideCfgData(playerData.GuideId);
        if (autoGuideCfg!=null)
        {
            SetGuideBtnIcon(autoGuideCfg.NpcId);
        }
        else
        {
            SetGuideBtnIcon(-1);
        }
    }

    private void SetGuideBtnIcon(int npcId)
    {
        string spritPath = "";
        Image img = AutoGuide.GetComponent<Image>();
        switch (npcId)
        {
            case ConstRoot.NpcWiseMan:
                spritPath = PathDefine.WiseManHead;
                break;
            case ConstRoot.NpcGeneral:
                spritPath = PathDefine.GeneralHead;
                break;
            case ConstRoot.NpcArtisan:
                spritPath = PathDefine.ArtisanHead;
                break;
            case ConstRoot.NpcTrader:
                spritPath = PathDefine.TraderHead;
                break;
            default:
                spritPath = PathDefine.TaskHead;
                break;
        }
        SetSprite(img, spritPath);
    }

    #region ClickButton
    public void ClickMenuBtn()  //任务栏收放
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiExitBtnAudio);
        isMenuOpen = !isMenuOpen;
        AnimationClip audioClip = null;
        audioClip = Anim.GetClip(isMenuOpen ? "OpenTask" : "CloseTask");
        Anim.Play(audioClip.name);
    }
    public void ClickHeadBtn()//打开角色属性面板
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiOpenBtnAudio);
        MainCitySys.Instance.OpenPlayerDataWind();
    } 
    public void ClickGuideBtn()// 开始导航任务
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        if (autoGuideCfg != null)
        {
            MainCitySys.Instance.RunTask(autoGuideCfg);
        }
        else
        {
            GameRoot.AddTips("敬请期待...");
        }
    }
    public void ClickStrongBtn()//强化界面
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        MainCitySys.Instance.OpenStrongWind();
    }
    public void ClickChatBtn()//聊天界面
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        MainCitySys.Instance.OpenChatWind();
    }
    public void ClickBuyPowerBtn()//购买体力
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        MainCitySys.Instance.OpenBuyWind(0);
    }
    public void ClickBuyCoinBtn()//购买金币
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        MainCitySys.Instance.OpenBuyWind(1);
    }
    public void ClickTaskBtn()//日常任务奖励
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        MainCitySys.Instance.OpenTaskWind();
    }
    public void ClickBattleBtn()//战斗界面
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        MainCitySys.Instance.EnterCopyWind();
    }
    #endregion

    #region 控制遥杆事件
    /// <summary>
    /// 控制遥杆事件
    /// </summary>
    public void TouchEvents()
    {
        OnClickDown(TouchImage.gameObject, (PointerEventData evt) =>
        {
            startPosition = evt.position;
            SetActive(DirPoint);
            DirBg.transform.position = evt.position;
        });
        OnClickUp(TouchImage.gameObject, (PointerEventData evt) =>
        {
            DirBg.transform.position = originPosition;
            SetActive(DirPoint, false);
            DirPoint.transform.localPosition = Vector2.zero;
            MainCitySys.Instance.SetMoveDir(Vector2.zero);
        });
        OnDragEvt(TouchImage.gameObject, (PointerEventData evt) =>
         {
             Vector2 dir = evt.position - startPosition;
             float len = dir.magnitude;
             if (len > pointDis)
             {
                 Vector2 clampDir = Vector2.ClampMagnitude(dir, pointDis);
                 DirPoint.transform.position = startPosition + clampDir;
             }
             else
             {
                 DirPoint.transform.position = evt.position;
             }
             MainCitySys.Instance.SetMoveDir(dir.normalized);
         });
    }
    #endregion
}