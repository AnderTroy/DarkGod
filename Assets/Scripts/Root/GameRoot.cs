/****************************************************
    文件：GameRoot.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/6 12:17:32
    功能：游戏启动入口，初始化各个系统，保存核心数据
*****************************************************/
using PEProtocol;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public LoadingWind LoadIn;//加载界面引用
    public DynamicWind DynamicWind;//动态Tips提示
    public static GameRoot Instance = null;
    private void Start()
    {
        Instance = this;
        Debug.Log(Instance.GetType());
        DontDestroyOnLoad(this);//在场景切换时不删除此物体
        ClearUiRoot();//初始化开始UI界面
        Init();//初始化服务
    }
    #region 初始化开始UI界面
    private void ClearUiRoot()
    {
        Transform canvas = transform.Find("Canvas");
        for (int i = 0; i < canvas.childCount; i++)
        {
            canvas.GetChild(i).gameObject.SetActive(false);
        }
    }
    #endregion
    private void Init()
    {
        #region Reset服务模块
        //网络服务
        NetSvc netSvc = GetComponent<NetSvc>();
        netSvc.InitSvc();
        //资源加载服务
        ResSvc resSvc = GetComponent<ResSvc>();
        resSvc.InitSvc();
        //声音播放
        AudioSvc audioSvc = GetComponent<AudioSvc>();
        audioSvc.InitSvc();
        //计时器
        TimeSvc timeSvc = GetComponent<TimeSvc>();
        timeSvc.InitSvc();
        #endregion

        //登入业务系统
        LoginSys login = GetComponent<LoginSys>();
        login.InitSys();
        MainCitySys mainCitySys = GetComponent<MainCitySys>();
        mainCitySys.InitSys();
        CopySys copySys = GetComponent<CopySys>();
        copySys.InitSys();
        BattleSys battleSys = GetComponent<BattleSys>();
        battleSys.InitSys();
        DynamicWind.SetWindState();//启用小提示窗口
        //进入游戏加载场景UI
        login.EnterLoginSys();
    }
    /// <summary>
    /// 添加提示，播放动画
    /// </summary>
    public static void AddTips(string tips)
    {
        Instance.DynamicWind.AddTips(tips);
    }

    public PlayerData PlayerData { get; private set; }
    /// <summary>
    /// 获得服务器的玩家游戏数据
    /// </summary>
    public void SetPlayerData(ResponseLogin responseLogin)
    {
        PlayerData = responseLogin.PlayerData;
    }
    public void SetPlayerName(string thisName)
    {
        PlayerData.Name = thisName;
    }

    public void SetPlayerDataByGuide(ResponseGuide data)
    {
        PlayerData.Coin = data.Coin;
        PlayerData.Level = data.Level;
        PlayerData.Exp = data.Exp;
        PlayerData.GuideId = data.GuideId;
    }
    public void SetPlayerDataByStrong(ResponseStrong data)
    {
        PlayerData.Coin = data.Coin;
        PlayerData.Crystal = data.Crystal;
        PlayerData.Hp = data.Hp;
        PlayerData.Ad = data.Ad;
        PlayerData.Ap = data.Ap;
        PlayerData.AdDefense = data.AdDefense;
        PlayerData.ApDefense = data.ApDefense;
        PlayerData.StrongArray = data.StrongArray;
    }

    public void SetPlayerDataByBuy(ResponseBuy data)
    {
        PlayerData.Diamond = data.Diamond;
        PlayerData.Coin = data.Coin;
        PlayerData.Power = data.Power;
    }
    public void SetPlayerDataByPower(PshPower data)
    {
        PlayerData.Power = data.Power;
    }
    public void SetPlayerDataByTask(ResponseTask data)
    {
        PlayerData.Exp = data.Exp;
        PlayerData.Coin = data.Coin;
        PlayerData.Diamond = data.Diamond;
        PlayerData.Level = data.Level;
        PlayerData.TaskArray = data.TaskArray;
    }
    public void SetPlayerDataByTaskPsh(PshTask data)
    {
        PlayerData.TaskArray = data.TaskArray;
    }

    public void SetPlayerDataByBattle(ResponseBattle data)
    {
        PlayerData.Power = data.Power;
    }

    public void SetPlayerDataByBattleEnd(ResponseBattleEnd data)
    {
        PlayerData.Coin = data.Coin;
        PlayerData.Level = data.Level;
        PlayerData.Exp = data.Exp;
        PlayerData.Crystal = data.Crystal;
        PlayerData.Battle = data.BattleId;
    }
}