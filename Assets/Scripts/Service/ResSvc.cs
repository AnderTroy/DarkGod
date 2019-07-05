/****************************************************
    文件：ResSvc.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/6 14:10:7
    功能：资源加载服务
*****************************************************/
using System;
using System.Collections.Generic;
using System.Xml;
using PEProtocol;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResSvc : MonoBehaviour
{
    public static ResSvc Instance = null;
    public AsyncOperation SceneAsync;

    public void InitSvc()
    {
        Instance = this;
        InitRdNameCfg(PathDefine.RdNameCfg);
        InitMonsterCfgData(PathDefine.MonsterCfg);
        InitMapCfgData(PathDefine.MapCfg);
        InitGuideCfgData(PathDefine.GuideCfg);
        InitStrongCfgData(PathDefine.StrongCfg);
        InitTaskCfgData(PathDefine.TaskRewardCfg);
        InitSkillActionCfgData(PathDefine.SkillActionCfg);
        InitSkillCfgData(PathDefine.SkillCfg);
        InitSkillMoveCfgData(PathDefine.SkillMoveCfg);
        Debug.Log(Instance.GetType());
    }

    public void ResetCfg()
    {
        skillCfgDictionary.Clear();
        InitSkillCfgData(PathDefine.SkillCfg);
        skillMoveCfgDictionary.Clear();
        InitSkillMoveCfgData(PathDefine.SkillMoveCfg);
        Debug.Log("Reset");
    }

    private Action prgV = null;

    public void AsyncLoadScene(string sceneName, Action loaded)
    {
        GameRoot.Instance.LoadIn.SetWindState();
        SceneAsync = SceneManager.LoadSceneAsync(sceneName);
        SceneAsync.allowSceneActivation = false;
        prgV = () =>
        {
            float val = SceneAsync.progress;
            GameRoot.Instance.LoadIn.SetProgress(val);
            if (val == 1)
            {
                loaded?.Invoke(); //if(loaded!=null) loaded();
                prgV = null;
                SceneAsync = null;
                GameRoot.Instance.LoadIn.SetWindState(false);
            }
        };
    }

    void Update()
    {
        prgV?.Invoke();
    }

    private readonly Dictionary<string, AudioClip> _audioDictionary = new Dictionary<string, AudioClip>(); //音效

    public AudioClip LoadAudio(string path, bool isSave = false)
    {
        AudioClip audioClip = null;
        if (!_audioDictionary.TryGetValue(path, out audioClip)) //从字典中读取数据
        {
            audioClip = Resources.Load<AudioClip>(path);
            if (isSave)
            {
                _audioDictionary.Add(path, audioClip); //添加值
            }
        }

        return audioClip;
    }

    private readonly Dictionary<string, GameObject> _playerDictionary = new Dictionary<string, GameObject>(); //角色

    public GameObject LoadPrefab(string path, bool isSave = false)
    {
        GameObject prefab = null;
        if (!_playerDictionary.TryGetValue(path, out prefab)) //从字典中读取数据
        {
            prefab = Resources.Load<GameObject>(path);
            if (isSave)
            {
                _playerDictionary.Add(path, prefab); //添加值
            }
        }

        GameObject gameObject = null;
        if (prefab != null)
        {
            gameObject = Instantiate(prefab);
        }

        return gameObject;
    }

    private readonly Dictionary<string, Sprite> _spriteDictionary = new Dictionary<string, Sprite>(); //图片

    public Sprite LoadSprite(string path, bool isSave = false)
    {
        Sprite sprite = null;
        if (!_spriteDictionary.TryGetValue(path, out sprite)) //从字典中读取数据
        {
            sprite = Resources.Load<Sprite>(path);
            if (isSave)
            {
                _spriteDictionary.Add(path, sprite); //添加值
            }
        }

        return sprite;
    }

    #region InitCfgs

    #region 随机名字资源

    private readonly List<string> surnameList = new List<string>();
    private readonly List<string> manList = new List<string>();
    private readonly List<string> womanList = new List<string>();

    private void InitRdNameCfg(string path)
    {
        TextAsset xml = Resources.Load<TextAsset>(path);
        if (!xml) //判断是否读取到xml文件
        {
            Debug.LogError("xml file:" + path + " not exist");
        }
        else
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml.text); //使用unity资源系统加载xml文件
            //解析xml文件
            XmlNodeList nodeList = doc.SelectSingleNode("root")?.ChildNodes; //获取根节点下的所有子节点的list
            if (nodeList != null)
                for (int i = 0; i < nodeList.Count; i++)
                {
                    XmlElement element = (XmlElement) nodeList[i]; //将该节点转化为一个XmlElement
                    if (element != null && element.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    if (element != null)
                    {
                        //element.GetAttributeNode("ID")?.InnerText 从XmlElement里获取名字为“ID”的数据
                        int id = Convert.ToInt32(element.GetAttributeNode("ID")?.InnerText);
                    }

                    foreach (XmlElement e in nodeList[i].ChildNodes)
                    {
                        switch (e.Name)
                        {
                            case "surname":
                                surnameList.Add(e.InnerText);
                                break;
                            case "man":
                                manList.Add(e.InnerText);
                                break;
                            case "woman":
                                womanList.Add(e.InnerText);
                                break;
                        }
                    }
                }
        }
    }

    public string GetRdNameData(bool man = true)
    {
        string rdName = surnameList[PETools.RDInt(0, surnameList.Count - 1)];
        rdName += man ? manList[PETools.RDInt(0, manList.Count - 1)] : womanList[PETools.RDInt(0, womanList.Count - 1)];
        return rdName;
    }

    #endregion

    #region 地图资源

    private readonly Dictionary<int, MapCfg> _mapCfgDictionary = new Dictionary<int, MapCfg>(); //实例化字典

    public void InitMapCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    MapCfg mapCfgData = new MapCfg
                    {
                        Id = id,
                        MonsterDataLst = new List<MonsterData>(),
                    };

                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "mapName":
                                mapCfgData.MapName = element.InnerText;
                                break;
                            case "sceneName":
                                mapCfgData.SceneName = element.InnerText;
                                break;
                            case "mainCamPos":
                            {
                                string[] valArr = element.InnerText.Split(',');
                                mapCfgData.MainCamPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "mainCamRote":
                            {
                                string[] valArr = element.InnerText.Split(',');
                                mapCfgData.MainCamRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornPos":
                            {
                                string[] valArr = element.InnerText.Split(',');
                                mapCfgData.PlayerBornPos = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornRote":
                            {
                                string[] valArr = element.InnerText.Split(',');
                                mapCfgData.PlayerBornRote = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "playerBornScale":
                            {
                                string[] valArr = element.InnerText.Split(',');
                                mapCfgData.PlayerBornScale = new Vector3(float.Parse(valArr[0]), float.Parse(valArr[1]),
                                    float.Parse(valArr[2]));
                            }
                                break;
                            case "power":
                            {
                                mapCfgData.Power = int.Parse(element.InnerText);
                            }
                                break;
                            case "monsterLst":
                            {
                                var valArray = element.InnerText.Split('#');
                                for (int waveIndex = 0; waveIndex < valArray.Length; waveIndex++)
                                {
                                    if (waveIndex == 0)
                                    {
                                        continue;
                                    }

                                    string[] tempArray = valArray[waveIndex].Split('|');
                                    for (int j = 0; j < tempArray.Length; j++)
                                    {
                                        if (j == 0)
                                        {
                                            continue;
                                        }

                                        string[] array = tempArray[j].Split(',');
                                        MonsterData monsterData = new MonsterData
                                        {
                                            Id = int.Parse(array[0]),
                                            MonsterWave = waveIndex,
                                            MonsterIndex = j,
                                            MonsterCfg = GetMonsterCfgData(int.Parse(array[0])),
                                            MonsterPos = new Vector3(float.Parse(array[1]), float.Parse(array[2]),
                                                float.Parse(array[3])),
                                            MonsterRote = new Vector3(0, float.Parse(array[4]), 0),
                                            Level=int.Parse((array[5])),
                                        };
                                        mapCfgData.MonsterDataLst.Add(monsterData);
                                    }
                                }

                            }
                                break;
                            case "coin":
                                mapCfgData.Coin = int.Parse(element.InnerText);
                                break;
                            case "exp":
                                mapCfgData.Exp = int.Parse(element.InnerText);
                                break;
                            case "crystal":
                                mapCfgData.Crystal = int.Parse(element.InnerText);
                                break;
                        }
                    }

                    _mapCfgDictionary.Add(id, mapCfgData);
                }
        }
    }

    public MapCfg GetMapCfgData(int id)
    {
        if (_mapCfgDictionary.TryGetValue(id, out var data))
        {
            return data;
        }

        return null;
    }

    #endregion

    #region 自动寻路引导

    private readonly Dictionary<int, AutoGuideCfg> guideCfgDictionary = new Dictionary<int, AutoGuideCfg>(); //实例化字典

    public void InitGuideCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    AutoGuideCfg autoGuideCfg = new AutoGuideCfg
                    {
                        Id = id
                    };
                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "npcID":
                                autoGuideCfg.NpcId = int.Parse(element.InnerText);
                                break;
                            case "dilogArr":
                                autoGuideCfg.DilogArr = element.InnerText;
                                break;
                            case "actID":
                                autoGuideCfg.ActId = int.Parse(element.InnerText);
                                break;
                            case "coin":
                                autoGuideCfg.Coin = int.Parse(element.InnerText);
                                break;
                            case "exp":
                                autoGuideCfg.Exp = int.Parse(element.InnerText);
                                break;
                        }
                    }

                    guideCfgDictionary.Add(id, autoGuideCfg);
                }
        }
    }

    public AutoGuideCfg GetGuideCfgData(int id)
    {
        if (guideCfgDictionary.TryGetValue(id, out var autoGuide))
        {
            return autoGuide;
        }

        return null;
    }

    #endregion

    #region 强化属性

    private readonly Dictionary<int, Dictionary<int, StrongCfg>> StrongCfgDictionary =
        new Dictionary<int, Dictionary<int, StrongCfg>>(); //实例化字典

    public void InitStrongCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    if (xmlElement != null)
                    {
                        int id = Convert.ToInt32(xmlElement.GetAttributeNode("ID")?.InnerText);
                        StrongCfg strongCfg = new StrongCfg
                        {
                            Id = id
                        };
                        foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                        {
                            int ele = int.Parse(element.InnerText);
                            switch (element.Name)
                            {
                                case "pos":
                                    strongCfg.Pos = ele;
                                    break;
                                case "starlv":
                                    strongCfg.StartLevel = ele;
                                    break;
                                case "addhp":
                                    strongCfg.AddHp = ele;
                                    break;
                                case "addhurt":
                                    strongCfg.AddHurt = ele;
                                    break;
                                case "adddef":
                                    strongCfg.AddDefense = ele;
                                    break;
                                case "minlv":
                                    strongCfg.MinLevel = ele;
                                    break;
                                case "coin":
                                    strongCfg.Coin = ele;
                                    break;
                                case "crystal":
                                    strongCfg.Crystal = ele;
                                    break;
                            }
                        }

                        if (StrongCfgDictionary.TryGetValue(strongCfg.Pos, out var dic))
                        {
                            dic.Add(strongCfg.StartLevel, strongCfg);
                        }
                        else
                        {
                            dic = new Dictionary<int, StrongCfg> {{strongCfg.StartLevel, strongCfg}};
                            StrongCfgDictionary.Add(strongCfg.Pos, dic);
                        }
                    }
                }
        }
    }

    public StrongCfg GetStrongCfgData(int pos, int starLevel)
    {
        StrongCfg strongData = null;
        if (StrongCfgDictionary.TryGetValue(pos, out var dic))
        {
            if (dic.ContainsKey(starLevel))
            {
                strongData = dic[starLevel];
            }
        }

        return strongData;
    }

    public int GetPropAddValPreLevel(int pos, int starLevel, int type)
    {
        int val = 0;
        if (StrongCfgDictionary.TryGetValue(pos, out var posDictionary))
        {
            for (int i = 0; i < starLevel; i++)
            {
                if (posDictionary.TryGetValue(i, out var strongData))
                {
                    switch (type)
                    {
                        case 1:
                            val += strongData.AddHp;
                            break;
                        case 2:
                            val += strongData.AddHurt;
                            break;
                        case 3:
                            val += strongData.AddDefense;
                            break;
                    }
                }
            }
        }

        return val;
    }

    #endregion

    #region 日常任务奖励

    private readonly Dictionary<int, TaskRewardCfg> taskCfgDictionary = new Dictionary<int, TaskRewardCfg>(); //实例化字典

    public void InitTaskCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    TaskRewardCfg taskCfg = new TaskRewardCfg
                    {
                        Id = id
                    };
                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "taskName":
                                taskCfg.TaskName = element.InnerText;
                                break;
                            case "count":
                                taskCfg.Count = int.Parse(element.InnerText);
                                break;
                            case "coin":
                                taskCfg.Coin = int.Parse(element.InnerText);
                                break;
                            case "diamond":
                                taskCfg.Diamond = int.Parse(element.InnerText);
                                break;
                            case "exp":
                                taskCfg.Exp = int.Parse(element.InnerText);
                                break;
                            case "path":
                                taskCfg.Path = element.InnerText;
                                break;
                        }
                    }

                    taskCfgDictionary.Add(id, taskCfg);
                }
        }
    }

    public TaskRewardCfg GetTaskCfgData(int id)
    {
        if (taskCfgDictionary.TryGetValue(id, out var taskReward))
        {
            return taskReward;
        }

        return null;
    }

    #endregion

    #region 技能配置

    private readonly Dictionary<int, SkillCfg> skillCfgDictionary = new Dictionary<int, SkillCfg>();

    public void InitSkillCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    SkillCfg skillCfg = new SkillCfg
                    {
                        Id = id,
                        SkillMoveLst = new List<int>(),
                        SkillActionLst = new List<int>(),
                        SkillDamageLst = new List<int>(),
                    };
                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "skillName":
                                skillCfg.SkillName = element.InnerText;
                                break;
                            case "skillTime":
                                skillCfg.SkillTime = int.Parse(element.InnerText);
                                break;
                            case "cdTime":
                                skillCfg.SkillCdTime = int.Parse(element.InnerText);
                                break;
                            case "isCombo":
                                skillCfg.IsComboSkill = element.InnerText.Equals("1");
                                break;
                            case "isCollide":
                                skillCfg.IsCollide = element.InnerText.Equals("1");
                                break;
                            case "isBreak":
                                skillCfg.IsBreak = element.InnerText.Equals("1");
                                break;
                            case "aniAction":
                                skillCfg.AnimAction = int.Parse(element.InnerText);
                                break;
                            case "fx":
                                skillCfg.EftName = element.InnerText;
                                break;
                            case "dmgType":
                                if (element.InnerText.Equals("1"))
                                {
                                    skillCfg.DamageType = DamageType.Ad;
                                }
                                else if (element.InnerText.Equals("2"))
                                {
                                    skillCfg.DamageType = DamageType.Ap;
                                }
                                else
                                {
                                    PeRoot.Log("Damage Error Code.");
                                }
                                break;
                            case "skillMoveLst":
                                string[] skillMoveArray = element.InnerText.Split('|');
                                foreach (var t in skillMoveArray)
                                {
                                    if (t != "")
                                    {
                                        skillCfg.SkillMoveLst.Add(int.Parse(t));
                                    }
                                }
                                break;
                            case "skillActionLst":
                                string[] skillActionArray = element.InnerText.Split('|');
                                foreach (var t in skillActionArray)
                                {
                                    if (t != "")
                                    {
                                        skillCfg.SkillActionLst.Add(int.Parse(t));
                                    }
                                }
                                break;
                            case "skillDamageLst":
                                string[] skillDamageLst = element.InnerText.Split('|');
                                foreach (var t in skillDamageLst)
                                {
                                    if (t != "")
                                    {
                                        skillCfg.SkillDamageLst.Add(int.Parse(t));
                                    }
                                }
                                break;
                        }
                    }

                    skillCfgDictionary.Add(id, skillCfg);
                }
        }
    }
    public SkillCfg GetSkillCfgData(int id)
    {
        return skillCfgDictionary.TryGetValue(id, out var skill) ? skill : null;
    }

    #endregion

    #region 技能伤害配置

    private readonly Dictionary<int, SkillActionCfg> _skillActionCfgDictionary = new Dictionary<int, SkillActionCfg>();

    public void InitSkillActionCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement)xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    SkillActionCfg skillActionCfg = new SkillActionCfg
                    {
                        Id = id,
                    };
                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "delayTime":
                                skillActionCfg.DelayTime = int.Parse(element.InnerText);
                                break;
                            case "radius":
                                skillActionCfg.Radius = float.Parse(element.InnerText);
                                break;
                            case "angle":
                                skillActionCfg.Angle = int.Parse(element.InnerText);
                                break;
                        }
                    }
                    _skillActionCfgDictionary.Add(id, skillActionCfg);
                }
        }
    }
    public SkillActionCfg GetSkillActionCfgData(int id)
    {
        return _skillActionCfgDictionary.TryGetValue(id, out var skillAction) ? skillAction : null;
    }

    #endregion

    #region 技能移动配置

    private readonly Dictionary<int, SkillMoveCfg> skillMoveCfgDictionary = new Dictionary<int, SkillMoveCfg>();

    public void InitSkillMoveCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    SkillMoveCfg skillMoveCfg = new SkillMoveCfg
                    {
                        Id = id
                    };
                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "delayTime":
                                skillMoveCfg.DelayTime = int.Parse(element.InnerText);
                                break;
                            case "moveTime":
                                skillMoveCfg.MoveTime = int.Parse(element.InnerText);
                                break;
                            case "moveDis":
                                skillMoveCfg.MoveDis = float.Parse(element.InnerText);
                                break;
                        }
                    }

                    skillMoveCfgDictionary.Add(id, skillMoveCfg);
                }
        }
    }

    public SkillMoveCfg GetSkillMoveCfgData(int id)
    {
        return skillMoveCfgDictionary.TryGetValue(id, out var skillMove) ? skillMove : null;
    }

    #endregion

    #region 怪物资源

    private readonly Dictionary<int, MonsterCfg> _monsterCfgDictionary = new Dictionary<int, MonsterCfg>();

    public void InitMonsterCfgData(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (!textAsset)
        {
            PeRoot.Log("Xml file:" + path + "not exist", PEProtocol.LogType.LogError);
        }
        else
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);

            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("root")?.ChildNodes;

            if (xmlNodeList != null)
                for (int i = 0; i < xmlNodeList.Count; i++)
                {
                    XmlElement xmlElement = (XmlElement) xmlNodeList[i];
                    if (xmlElement != null && xmlElement.GetAttributeNode("ID") == null)
                    {
                        continue;
                    }

                    int id = Convert.ToInt32(xmlElement?.GetAttributeNode("ID")?.InnerText);
                    MonsterCfg monsterCfgData = new MonsterCfg
                    {
                        Id = id,
                        MonsterAttribute=new BattleAttribute(),
                    };

                    foreach (XmlElement element in xmlNodeList[i].ChildNodes)
                    {
                        switch (element.Name)
                        {
                            case "mName":
                                monsterCfgData.MonsterName = element.InnerText;
                                break;
                            case "resPath":
                                monsterCfgData.ResPath = element.InnerText;
                                break;
                            case "skillID":
                                monsterCfgData.SkillId = int.Parse(element.InnerText);
                                break;
                            case "mType":
                                if (element.InnerText.Equals("1"))
                                {
                                    monsterCfgData.MonsterType = MonsterType.Normal;
                                }
                                else if (element.InnerText.Equals("2"))
                                {
                                    monsterCfgData.MonsterType = MonsterType.Boss;
                                }
                                break;
                            case "isStop":
                                monsterCfgData.IsStop = int.Parse(element.InnerText)==1;
                                break;
                            case "atkDis":
                                monsterCfgData.AttackDis = float.Parse(element.InnerText);
                                break;
                            case "hp":
                                monsterCfgData.MonsterAttribute.Hp = int.Parse(element.InnerText);
                                break;
                            case "ad":
                                monsterCfgData.MonsterAttribute.Ad = int.Parse(element.InnerText);
                                break;
                            case "ap":
                                monsterCfgData.MonsterAttribute.Ap = int.Parse(element.InnerText);
                                break;
                            case "addef":
                                monsterCfgData.MonsterAttribute.AdDefense = int.Parse(element.InnerText);
                                break;
                            case "apdef":
                                monsterCfgData.MonsterAttribute.ApDefense = int.Parse(element.InnerText);
                                break;
                            case "dodge":
                                monsterCfgData.MonsterAttribute.Dodge = int.Parse(element.InnerText);
                                break;
                            case "pierce":
                                monsterCfgData.MonsterAttribute.Pierce = int.Parse(element.InnerText);
                                break;
                            case "critical":
                                monsterCfgData.MonsterAttribute.Critical = int.Parse(element.InnerText);
                                break;
                        }
                    }

                    _monsterCfgDictionary.Add(id, monsterCfgData);
                }
        }
    }

    public MonsterCfg GetMonsterCfgData(int id)
    {
        if (_monsterCfgDictionary.TryGetValue(id, out var data))
        {
            return data;
        }

        return null;
    }

    #endregion

    #endregion
}