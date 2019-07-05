/****************************************************
    文件：MainCitySys.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/12 9:39:51
    功能：主城业务系统
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.AI;

public class MainCitySys : SystemRoot
{
    public MainCityWind MainCityWind;
    public PlayerDataWind PlayerDataWind;
    public GuideWind GuideWind;
    public StrongWind StrongWind;
    public ChatWind ChatWind;
    public BuyWind BuyWind;
    public TaskWind TaskWind;
    public CopyWind CopyWind;
    private Transform RawShowCam;
    private PlayerController playerController;
    private AutoGuideCfg taskData;
    private Transform[] npcTransforms;
    private NavMeshAgent nav;
    private GameObject player;
    public static MainCitySys Instance = null;
    public override void InitSys()
    {
        base.InitSys();
        Instance = this;
        Debug.Log(Instance.GetType());
    }
    /// <summary>
    /// 加载主城场景资源
    /// </summary>
    public void EnterMainCity()
    {
        MapCfg mapCfgData = ResSvc.GetMapCfgData(ConstRoot.MainCityMapId);
        ResSvc.AsyncLoadScene(mapCfgData.SceneName, () =>
        {
            AudioSvc.PlayBgAudioMusic(ConstRoot.MainCityBgAudio);//播放背景音乐
            LoadPlayer(mapCfgData);  //加载角色
            MainCityWind.SetWindState();//打开主城 UI
            if (RawShowCam != null) //设置相机
            {
                RawShowCam.gameObject.SetActive(false);
            }

            GameObject npcPosition = GameObject.Find("NPCposition");
            MainCityMapData cityMapData = npcPosition.GetComponent<MainCityMapData>();
            npcTransforms = cityMapData.NpcTransforms;
        });
    }
    
    /// <summary>
    /// 加载角色人物，相机位置
    /// </summary>
    private void LoadPlayer(MapCfg mapData)
    {
        //角色位置，旋转，大小
        player = ResSvc.LoadPrefab(PathDefine.AssassinCityPlayerPrefab, true);
        player.transform.position = mapData.PlayerBornPos;
        player.transform.localEulerAngles = mapData.PlayerBornRote;
        player.transform.localScale = mapData.PlayerBornScale;
        //相机位置，旋转
        Camera.main.transform.position = mapData.MainCamPos;
        Camera.main.transform.localEulerAngles = mapData.MainCamRote;

        playerController = player.GetComponent<PlayerController>();
        playerController.Init();
        nav = player.GetComponent<NavMeshAgent>();
    }
    public void SetMoveDir(Vector2 dir)
    {
        StopNavGuideTask();
        playerController.SetBlend(dir == Vector2.zero ? ConstRoot.BlendIdle : ConstRoot.BlendMove);
        playerController.Dir = dir;
    }

    #region ChatWind
    public void OpenChatWind()
    {
        StopNavGuideTask();
        ChatWind.SetWindState();
    }
    public void PshChat(NetMsg netMsg)
    {
        ChatWind.AddChatMsg(netMsg.PshChat.Name, netMsg.PshChat.Chat);
    }
    #endregion

    #region PlayerWind
    public void OpenPlayerDataWind()
    {
        StopNavGuideTask();
        if (RawShowCam == null)
        {
            RawShowCam = GameObject.FindGameObjectWithTag("RawShowCam").transform;
        }
        //设置相机与人物相对位置
        RawShowCam.localPosition = playerController.transform.position + playerController.transform.forward * 2.8f +
                                   new Vector3(0, 1.25f, 0);
        RawShowCam.localEulerAngles = new Vector3(0, 180 + playerController.transform.localEulerAngles.y, 0);
        RawShowCam.gameObject.SetActive(true);
        PlayerDataWind.SetWindState();
    }
    public void ClosePlayerDataWind()
    {
        RawShowCam.gameObject.SetActive(false);
        PlayerDataWind.SetWindState(false);
    }

    private float startRotation = 0;
    public void SetPlayerRote(float rotation)
    {
        playerController.transform.localEulerAngles = new Vector3(0, startRotation + rotation, 0);
    }

    public void SetStartRote()
    {
        startRotation = playerController.transform.localEulerAngles.y;
    }
    #endregion

    #region GuideWind
    private bool isNavGuide = false;
    public void RunTask(AutoGuideCfg guideCfg)
    {
        if (guideCfg != null)
        {
            taskData = guideCfg;
        }

        if (!nav.isActiveAndEnabled)
        {
            nav.enabled = true;
        }

        if (taskData.NpcId != -1)
        {
            float dis = Vector3.Distance(playerController.transform.position, npcTransforms[guideCfg.NpcId].position);
            if (dis < 0.5f)
            {
                isNavGuide = false;
                nav.isStopped = true;
                playerController.SetBlend(ConstRoot.BlendIdle);
                player.GetComponent<CharacterController>().enabled = true;
                nav.enabled = false;
                OpenGuideWind();
            }
            else
            {
                isNavGuide = true;
                nav.enabled = true;
                nav.speed = ConstRoot.PlayerMoveSpeed;
                playerController.SetBlend(ConstRoot.BlendMove);
                player.GetComponent<CharacterController>().enabled = false;
                nav.SetDestination(npcTransforms[guideCfg.NpcId].position);
            }
        }
        else
        {
            OpenGuideWind();
        }
    }
    private void Update()
    {
        if (isNavGuide)
        {
            IsArriveNavPos();
            playerController.SetCamFollower();
        }
    }
    private void StopNavGuideTask()
    {
        if (isNavGuide)
        {
            player.GetComponent<CharacterController>().enabled = true;
            isNavGuide = false;
            nav.isStopped = true;
            playerController.SetBlend(ConstRoot.BlendIdle);
            nav.enabled = false;
        }
    }
    private void IsArriveNavPos()
    {
        float dis = Vector3.Distance(playerController.transform.position, npcTransforms[taskData.NpcId].position);
        if (dis < 0.5f)
        {
            player.GetComponent<CharacterController>().enabled = true;
            isNavGuide = false;
            nav.isStopped = true;
            playerController.SetBlend(ConstRoot.BlendIdle);
            nav.enabled = false;
            OpenGuideWind();
        }
    }
    public void OpenGuideWind()
    {
        GuideWind.SetWindState();
    }

    public AutoGuideCfg GetAutoTaskData()
    {
        return taskData;
    }

    public void ResponseGuide(NetMsg netMsg)
    {
        ResponseGuide data = netMsg.ResponseGuide;
        GameRoot.AddTips(ConstRoot.Color("任务奖励 金币 + " + taskData.Coin + "  经验 + " + taskData.Exp, TextColor.Blue));
        switch (taskData.ActId)
        {
            case 0:
                //与智者对话
                //OpenGuideWind();
                break;
            case 1:
                //进入副本
                EnterCopyWind();
                break;
            case 2:
                //进入强化界面
                OpenStrongWind();
                break;
            case 3:
                //进入体力购买
                OpenBuyWind(0);
                break;
            case 4:
                //进入金币铸造
                OpenBuyWind(1);
                break;
            case 5:
                //进入世界聊天
                OpenChatWind();
                break;
        }
        GameRoot.Instance.SetPlayerDataByGuide(data);
        MainCityWind.RefreshUi();
    }
    #endregion

    #region StrongWind
    public void OpenStrongWind()
    {
        StopNavGuideTask();
        StrongWind.SetWindState();
    }
    public void ResStrong(NetMsg netMsg)
    {
        int fight = PeRoot.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.Instance.SetPlayerDataByStrong(netMsg.ResponseStrong);
        int fightNow = PeRoot.GetFightByProps(GameRoot.Instance.PlayerData);
        GameRoot.AddTips(ConstRoot.Color("战力提升 " + (fightNow - fight), TextColor.Blue));

        StrongWind.UpDataUi();
        MainCityWind.RefreshUi();
    }
    #endregion

    #region BuyWind
    public void OpenBuyWind(int type)
    {
        StopNavGuideTask();
        BuyWind.setBuyType(type);
        BuyWind.SetWindState();
    }
    public void ResBuy(NetMsg netMsg)
    {
        ResponseBuy data = netMsg.ResponseBuy;
        GameRoot.Instance.SetPlayerDataByBuy(data);
        GameRoot.AddTips("购买成功！");

        MainCityWind.RefreshUi();
        BuyWind.SetWindState(false);

        if (netMsg.PshTask!=null)
        {
            GameRoot.Instance.SetPlayerDataByTaskPsh(netMsg.PshTask);

            if (TaskWind.GetWindState())
            {
                TaskWind.RefreshUi();
            }
        }
    }
    #endregion

    #region Power
    public void PshPower(NetMsg netMsg)
    {
        PshPower data = netMsg.PshPower;
        GameRoot.Instance.SetPlayerDataByPower(data);
        if (MainCityWind.GetWindState())
        {
            MainCityWind.RefreshUi();
        }
    }
    #endregion

    #region TaskWind
    public void OpenTaskWind()
    {
        StopNavGuideTask();
        TaskWind.SetWindState();
    }

    public void ResponseTaskReward(NetMsg netMsg)
    {
        ResponseTask data = netMsg.ResponseTask;
        GameRoot.Instance.SetPlayerDataByTask(data);

        TaskWind.RefreshUi();
        MainCityWind.RefreshUi();
    }

    public void PshTaskPrangs(NetMsg netMsg)
    {
        PshTask data = netMsg.PshTask;
        GameRoot.Instance.SetPlayerDataByTaskPsh(data);

        if (TaskWind.GetWindState())
        {
            TaskWind.RefreshUi();
        }
    }
    #endregion

    #region Entity CopyWind
    public void EnterCopyWind()
    {
        StopNavGuideTask();
        CopySys.Instance.EnterCopyWind();
    }
    #endregion
}