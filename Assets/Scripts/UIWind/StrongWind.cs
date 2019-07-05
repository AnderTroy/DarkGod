/****************************************************
    文件：StrongWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/18 23:59:32
    功能：强化界面
*****************************************************/
using PEProtocol;
using PENet;
using UnityEngine;
using UnityEngine.UI;

public class StrongWind : WindowRoot
{
    public Image TopStrongBg;
    public Text TipText;
    public Text TipHp;
    public Text TipHurt;
    public Text TipDefense;
    public Text TipHpLast;
    public Text TipHurtLast;
    public Text TipDefenseLast;
    public Text LevelNeed;
    public Text CoinNeed;
    public Text CrystalNeed;
    public Text CoinHave;
    public Text CrystalHave;
    public Transform StarBack;
    public Transform PosButtonTrans;
    public Transform PosButtonItems;
    public Transform Need;
    public Transform BackFront;
    private readonly Image[] images = new Image[6];
    private readonly Image[] imagesItems = new Image[6];
    private PlayerData playerData;
    private int currentIndex;
    private StrongCfg strongCfgData;
    protected override void InitWind()
    {
        base.InitWind();
        playerData = GameRoot.Instance.PlayerData;
        RegClickEvts();
        ClickPosItem(0);
    }

    private void RegClickEvts()
    {
        for (int i = 0; i < PosButtonTrans.childCount; i++)
        {
            Image image = PosButtonTrans.GetChild(i).GetComponent<Image>();
            Image imageItem = PosButtonItems.GetChild(i).GetComponent<Image>();
            OnClick(image.gameObject, (object args) =>
            {
                ClickPosItem((int)args);
                AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
            }, i);
            images[i] = image;
            imagesItems[i] = imageItem;
        }
    }

    private void ClickPosItem(int index)
    {
        //PeRoot.Log("Click Item:" + index);
        currentIndex = index;
        for (int i = 0; i < images.Length; i++)
        {
            Transform transform = images[i].transform;
            if (i == currentIndex)
            {
                images[i].color = new Color(1, 1, 1, 1);
                imagesItems[i].color = new Color(1, 1, 1, 0);
            }
            else
            {
                images[i].color = new Color(1, 1, 1, 0);
                imagesItems[i].color = new Color(1, 1, 1, 1);
            }
        }
        RefreshItem();
    }
    public void CloseWindBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetWindState(false);
    }

    public void ClickStrongBtn()
    {
        AudioSvc.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        if (playerData.StrongArray[currentIndex]<10)
        {
            if (playerData.Level<strongCfgData.MinLevel)
            {
                GameRoot.AddTips("等级不够！");
                return;
            }
            else if (playerData.Coin < strongCfgData.Coin)
            {
                GameRoot.AddTips("金币不够！");
                return;
            }
            else if (playerData.Crystal < strongCfgData.Crystal)
            {
                GameRoot.AddTips("水晶不够！");
                return;
            }

            NetSvc.SendMsg(new NetMsg
            {
                cmd = (int) Command.ReqStrong,
                RequestStrong=new RequestStrong
                {
                    pos=currentIndex,
                }
            });
        }
        else
        {
            GameRoot.AddTips("已满级！");
        }
    }

    public void UpDataUi()
    {
        ClickPosItem(currentIndex);
    }
    private void RefreshItem()
    {
        SetText(TipText, playerData.StrongArray[currentIndex] + "星级");
        switch (currentIndex)
        {
            case 0:
                SetSprite(TopStrongBg, PathDefine.HeadIcon);
                break;
            case 1:
                SetSprite(TopStrongBg, PathDefine.BodyIcon);
                break;
            case 2:
                SetSprite(TopStrongBg, PathDefine.YaoDaiIcon);
                break;
            case 3:
                SetSprite(TopStrongBg, PathDefine.HandIcon);
                break;
            case 4:
                SetSprite(TopStrongBg, PathDefine.LegIcon);
                break;
            case 5:
                SetSprite(TopStrongBg, PathDefine.FootIcon);
                break;
        }

        int curtStarLevel = playerData.StrongArray[currentIndex];
        for (int i = 0; i < StarBack.childCount; i++)
        {
            Image image = StarBack.GetChild(i).GetComponent<Image>();
            SetSprite(image, i < curtStarLevel ? PathDefine.StarIcon2 : PathDefine.StarIcon1);
        }

        int nextStartLevel = curtStarLevel + 1;
        int sumAddHp = ResSvc.GetPropAddValPreLevel(currentIndex, nextStartLevel, 1);
        int sumAddHurt = ResSvc.GetPropAddValPreLevel(currentIndex, nextStartLevel, 2);
        int sumAddDefense = ResSvc.GetPropAddValPreLevel(currentIndex, nextStartLevel, 3);
        SetText(TipHp, "+" + sumAddHp);
        SetText(TipHurt, "+" + sumAddHurt);
        SetText(TipDefense, "+" + sumAddDefense);

        strongCfgData = ResSvc.GetStrongCfgData(currentIndex, nextStartLevel);
        if (strongCfgData != null)
        {
            SetActive(BackFront);
            SetActive(Need);
            SetActive(TipHpLast);
            SetActive(TipHurtLast);
            SetActive(TipDefenseLast);

            SetText(TipHpLast, "+" + strongCfgData.AddHp);
            SetText(TipHurtLast, "+" + strongCfgData.AddHurt);
            SetText(TipDefenseLast, "+" + strongCfgData.AddDefense);

            SetText(LevelNeed, strongCfgData.MinLevel);
            SetText(CoinNeed, strongCfgData.Coin);
            SetText(CrystalNeed, strongCfgData.Crystal);
        }
        else
        {
            SetActive(BackFront,false);
            SetActive(Need, false);
            SetActive(TipHpLast, false);
            SetActive(TipHurtLast, false);
            SetActive(TipDefenseLast, false);
        }
        SetText(CoinHave, playerData.Coin);
        SetText(CrystalHave, playerData.Crystal);
    }
}