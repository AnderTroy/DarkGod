/****************************************************
    文件：BattlesWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:16:56
    功能：战斗场景界面
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BattlesWind : WindowRoot
{
    public Text HpText;
    public Image FillAmountByHp;
    public Text MpText;
    public Image FillAmountByMp;
    public Text LevelText;
    public Text NameText;
    public Text ExpText;//经验值百分比
    public Transform ExpPrgTransform;//经验值进度条

    public Transform TransBossHpBar;
    public Image BossHpGray;
    public Image BossHpRed;
    public Text BossName;
    private int _hpSum;
    private float _currentPrg = 1f;
    private float _targetPrg = 1f;
    #region Skill 1
    public Image Sk1Cd;
    public Text Sk1CdText;
    private bool _isSk1Cd = false;
    private float _sk1CdTime;
    private int _sk1Number;
    private float _sk1FillCount = 0;
    private float _sk1NumberCount = 0;
    #endregion

    #region Skill 2
    public Image Sk2Cd;
    public Text Sk2CdText;
    private bool _isSk2Cd = false;
    private float _sk2CdTime;
    private int _sk2Number;
    private float _sk2FillCount = 0;
    private float _sk2NumberCount = 0;
    #endregion

    #region Skill 3
    public Image Sk3Cd;
    public Text Sk3CdText;
    private bool _isSk3Cd = false;
    private float _sk3CdTime;
    private int _sk3Number;
    private float _sk3FillCount = 0;
    private float _sk3NumberCount = 0;
    #endregion

    #region MoveControler
    public Image TouchImage;//摇杆区域
    public Image DirBg;//摇杆背景
    public Image DirPoint;//摇杆中心点
    private float _pointDis;//摇杆中心点偏移
    private Vector2 _startPosition = Vector2.zero;//按下位置
    private Vector2 _originPosition = Vector2.zero;//摇杆初始点
    #endregion

    [HideInInspector]
    public Vector2 CurrentDir;//记录移动方向
    protected override void InitWind()
    {
        base.InitWind();
        _pointDis = Screen.height * 1.0f / ConstRoot.ScreenStandardHeight * ConstRoot.ScreenOriginDis;
        _originPosition = DirBg.transform.position;
        SetActive(DirPoint, false);
        TouchEvents();
        _sk1CdTime = ResSvc.GetSkillCfgData(101).SkillCdTime / 1000.0f;
        _sk2CdTime = ResSvc.GetSkillCfgData(102).SkillCdTime / 1000.0f;
        _sk3CdTime = ResSvc.GetSkillCfgData(103).SkillCdTime / 1000.0f;
        _hpSum = GameRoot.Instance.PlayerData.Hp;
        SetText(HpText, _hpSum + "/" + _hpSum);
        FillAmountByHp.fillAmount = 1;

        SetBossHpState(false);
        RefreshUi();
    }
    public void RefreshUi()
    {
        PlayerData playerData = GameRoot.Instance.PlayerData;
        SetText(LevelText, playerData.Level);
        SetText(NameText, playerData.Name);

        #region ExpPrg
        int expPrg = (int)(playerData.Exp * 1.0f / PeRoot.GetExpUpValByLv(playerData.Level) * 100);
        SetText(ExpText, expPrg + "%");
        int index = expPrg / 10;
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
    }
    #region 控制遥杆事件
    public void TouchEvents()// 控制遥杆事件
    {
        OnClickDown(TouchImage.gameObject, (PointerEventData evt) =>
        {
            _startPosition = evt.position;
            SetActive(DirPoint);
            DirBg.transform.position = evt.position;
        });
        OnClickUp(TouchImage.gameObject, (PointerEventData evt) =>
        {
            DirBg.transform.position = _originPosition;
            SetActive(DirPoint, false);
            DirPoint.transform.localPosition = Vector2.zero;

            CurrentDir = Vector2.zero;
            BattleSys.Instance.SetMoveDir(CurrentDir);
        });
        OnDragEvt(TouchImage.gameObject, (PointerEventData evt) =>
        {
            Vector2 dir = evt.position - _startPosition;
            float len = dir.magnitude;
            if (len > _pointDis)
            {
                Vector2 clampDir = Vector2.ClampMagnitude(dir, _pointDis);
                DirPoint.transform.position = _startPosition + clampDir;
            }
            else
            {
                DirPoint.transform.position = evt.position;
            }

            CurrentDir = dir.normalized;
            BattleSys.Instance.SetMoveDir(CurrentDir);
        });
    }
    #endregion

    public void SetSelfHpBtVal(int val)
    {
        SetText(HpText, val + "/" + _hpSum);
        FillAmountByHp.fillAmount = val * 1.0f / _hpSum;
    }
    public void SetHurt(int hurt)
    {
        SetText(HpText, (_hpSum-hurt) + "/" + _hpSum);
        FillAmountByHp.fillAmount = (_hpSum - hurt) * 1.0f / _hpSum;
    }
    public void SetBossHpState(bool state, float prg = 1)
    {
        MonsterCfg monster = ResSvc.GetMonsterCfgData(2001);
        SetActive(TransBossHpBar,state);
        BossHpGray.fillAmount = prg;
        BossHpRed.fillAmount = prg;
        BossName.text = "Lv.2 "+monster.MonsterName;
    }

    public void SetBossHpBarVal(int oldHp,int newHp,int sumHp)
    {
        _currentPrg = oldHp * 1.0f / sumHp;
        _targetPrg = newHp * 1.0f / sumHp;
        BossHpRed.fillAmount = _targetPrg;
    }

    private void BlendBossHp()
    {
        if (Mathf.Abs(_currentPrg - _targetPrg) < ConstRoot.AccelerationHpSpeed * Time.deltaTime)
        {
            _currentPrg = _targetPrg;
        }
        else if (_currentPrg > _targetPrg)
        {
            _currentPrg -= ConstRoot.AccelerationHpSpeed * Time.deltaTime;
        }
        else
        {
            _currentPrg += ConstRoot.AccelerationHpSpeed * Time.deltaTime;
        }
    }
    #region Click Skill
    public void ClickNormalAtk()
    {
        BattleSys.Instance.ReqReleaseSkill(0);
    }
    public void ClickSkill1Atk()
    {
        if (_isSk1Cd == false&&GetCanRlsSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(1);
            _isSk1Cd = true;
            SetActive(Sk1Cd);
            Sk1Cd.fillAmount = 1;
            _sk1Number = (int)_sk1CdTime;
            SetText(Sk1CdText, _sk1Number);
        }
    }
    public void ClickSkill2Atk()
    {
        if (_isSk2Cd == false && GetCanRlsSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(2);
            _isSk2Cd = true;
            SetActive(Sk2Cd);
            Sk2Cd.fillAmount = 1;
            _sk2Number = (int)_sk2CdTime;
            SetText(Sk2CdText, _sk2Number);
        }
    }
    public void ClickSkill3Atk()
    {
        if (_isSk3Cd == false && GetCanRlsSkill())
        {
            BattleSys.Instance.ReqReleaseSkill(3);
            _isSk3Cd = true;
            SetActive(Sk3Cd);
            Sk3Cd.fillAmount = 1;
            _sk3Number = (int)_sk3CdTime;
            SetText(Sk3CdText, _sk3Number);
        }
    }
    public void ClickHead()
    {
        //ResSvc.Instance.ResetCfg();
        BattleSys.Instance.BattleMgr.IsPause = true;
        BattleSys.Instance.SetBattleEndWinsSate(BattleEndType.Pause);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            BattleSys.Instance.ReqReleaseSkill(1);
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            BattleSys.Instance.ReqReleaseSkill(2);
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            BattleSys.Instance.ReqReleaseSkill(3);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            BattleSys.Instance.ReqReleaseSkill(0);
        }
        float delta = Time.deltaTime;
        if (_isSk1Cd)
        {
            _sk1FillCount += delta;
            if (_sk1FillCount >= _sk1CdTime)
            {
                _isSk1Cd = false;
                SetActive(Sk1Cd, false);
                _sk1FillCount = 0;
            }
            else
            {
                Sk1Cd.fillAmount = 1 - _sk1FillCount / _sk1CdTime;
            }

            _sk1NumberCount += delta;
            if (_sk1NumberCount >= 1)
            {
                _sk1NumberCount -= 1;
                _sk1Number -= 1;
                SetText(Sk1CdText, _sk1Number);
            }
        }

        if (_isSk2Cd)
        {
            _sk2FillCount += delta;
            if (_sk2FillCount >= _sk2CdTime)
            {
                _isSk2Cd = false;
                SetActive(Sk2Cd, false);
                _sk2FillCount = 0;
            }
            else
            {
                Sk2Cd.fillAmount = 1 - _sk2FillCount / _sk2CdTime;
            }

            _sk2NumberCount += delta;
            if (_sk2NumberCount >= 1)
            {
                _sk2NumberCount -= 1;
                _sk2Number -= 1;
                SetText(Sk2CdText, _sk2Number);
            }
        }

        if (_isSk3Cd)
        {
            _sk3FillCount += delta;
            if (_sk3FillCount >= _sk3CdTime)
            {
                _isSk3Cd = false;
                SetActive(Sk3Cd, false);
                _sk3FillCount = 0;
            }
            else
            {
                Sk3Cd.fillAmount = 1 - _sk3FillCount / _sk3CdTime;
            }

            _sk3NumberCount += delta;
            if (_sk3NumberCount >= 1)
            {
                _sk3NumberCount -= 1;
                _sk3Number -= 1;
                SetText(Sk3CdText, _sk3Number);
            }
        }

        if (TransBossHpBar.gameObject.activeSelf)
        {
            BlendBossHp();
            BossHpGray.fillAmount = _currentPrg;
        }
    }

    public bool GetCanRlsSkill()
    {
        return BattleSys.Instance.BattleMgr.CanRlsSkill();
    }
    #endregion
}