/****************************************************
    文件：ItemEntityHp.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/29 22:22:4
    功能：血条
*****************************************************/
using UnityEngine;
using UnityEngine.UI;

public class ItemEntityHp : MonoBehaviour 
{
    public Image HpGray;
    public Image HpRed;

    public Animation CriticalAnim;
    public Text TextCritical;

    public Animation DodgeAnim;
    public Text TextDodge;

    public Animation HpAnim;
    public Text TextHp;

    private RectTransform _rect;
    private Transform _rootTrans;
    private readonly float _scaleRate = 1.0f * ConstRoot.ScreenStandardHeight / Screen.height;
    private int _valHp;
    private float _currentPrg;
    private float _targetPrg;
    private void Update()
    {
        Vector3 screenPos = Camera.main.WorldToScreenPoint(_rootTrans.position);
        _rect.anchoredPosition = screenPos * _scaleRate;

        UpDatePrgMaxToMin();
        HpGray.fillAmount = _currentPrg;
    }

    private void UpDatePrgMaxToMin()
    {
        if (Mathf.Abs(_currentPrg - _targetPrg) < ConstRoot.AccelerationHpSpeed * Time.deltaTime)
        {
            _currentPrg = _targetPrg;
        }
        else if (_currentPrg>_targetPrg)
        {
            _currentPrg -= ConstRoot.AccelerationHpSpeed * Time.deltaTime;
        }
        else
        {
            _currentPrg += ConstRoot.AccelerationHpSpeed * Time.deltaTime;
        }
    }
    public void SetCritical(int critical)
    {
        CriticalAnim.Stop();
        TextCritical.text = "暴击" + critical;
        CriticalAnim.Play();
    }
    public void SetDodge()
    {
        DodgeAnim.Stop();
        TextDodge.text = "闪避";
        DodgeAnim.Play();
    }
    public void SetHurt(int hurt)
    {
        HpAnim.Stop();
        TextHp.text = "-" + hurt;
        HpAnim.Play();
    }

    public void SetItemInfo(Transform trans,int hp)
    {
        _rect = transform.GetComponent<RectTransform>();
        _rootTrans = trans;
        _valHp = hp;
        HpGray.fillAmount = 1;
        HpRed.fillAmount = 1;
    }

    public void SetVal(int oldVal,int newVal)
    {
        _currentPrg = oldVal * 1.0f / _valHp;
        _targetPrg = newVal * 1.0f / _valHp;
        HpRed.fillAmount = _targetPrg;
    }
}