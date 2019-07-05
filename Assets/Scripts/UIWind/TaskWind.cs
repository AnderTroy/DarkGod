/****************************************************
    文件：TaskWind.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/23 15:17:12
    功能：日常任务奖励界面
*****************************************************/
using PEProtocol;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskWind : WindowRoot
{
    public Transform ScrollTrans;
    private PlayerData playerData = null;
    private List<TaskRewardData> taskDataList = new List<TaskRewardData>();
    protected override void InitWind()
    {
        base.InitWind();
        playerData = GameRoot.Instance.PlayerData;
        RefreshUi();
    }

    public void RefreshUi()
    {
        taskDataList.Clear();
        List<TaskRewardData> todoList = new List<TaskRewardData>();//未完成
        List<TaskRewardData> doneList = new List<TaskRewardData>();//已完成
        List<TaskRewardData> firstList = new List<TaskRewardData>();//已完成
        //1|0|0
        foreach (string item in playerData.TaskArray)
        {
            string[] taskInfo = item.Split('|');
            TaskRewardData taskRewardData = new TaskRewardData
            {
                Id = int.Parse(taskInfo[0]),
                Prangs = int.Parse(taskInfo[1]),
                Tasked = taskInfo[2].Equals("1"),
            };
            TaskRewardCfg taskRewardCfg = new TaskRewardCfg();
            if (taskRewardData.Tasked)
            {
                doneList.Add(taskRewardData);
            }
            else if (taskRewardData.Prangs==taskRewardCfg.Count)
            {
                firstList.Add(taskRewardData);
            }
            else
            {
                todoList.Add(taskRewardData);
            }
        }

        taskDataList.AddRange(todoList);
        taskDataList.AddRange(firstList);
        taskDataList.AddRange(doneList);

        for (int i = 0; i < ScrollTrans.childCount; i++)
        {
            Destroy(ScrollTrans.GetChild(i).gameObject);
        }

        for (int i = 0; i < taskDataList.Count; i++)
        {
            GameObject gameObject = ResSvc.LoadPrefab(PathDefine.TaskItemPrefab);
            gameObject.transform.SetParent(ScrollTrans);
            gameObject.transform.localPosition = Vector3.zero;
            gameObject.transform.localScale = Vector3.one;
            gameObject.name = "TaskItem_" + i;

            TaskRewardData taskData = taskDataList[i];
            TaskRewardCfg taskCfg = ResSvc.Instance.GetTaskCfgData(taskData.Id);

            SetText(GetTransform(gameObject.transform, "TaskName"), taskCfg.TaskName);
            SetText(GetTransform(gameObject.transform, "Filled/PrgText"), taskData.Prangs+"/"+taskCfg.Count);
            SetText(GetTransform(gameObject.transform, "CoinText"), taskCfg.Coin);
            SetText(GetTransform(gameObject.transform, "ExpText"), taskCfg.Exp);
            SetText(GetTransform(gameObject.transform, "DiamondText"), taskCfg.Diamond);
            Image taskImage = GetTransform(gameObject.transform, "TaskImage").GetComponent<Image>();
            SetSprite(taskImage, taskCfg.Path);
            Image imagePrangs = GetTransform(gameObject.transform, "Filled/FillAmount").GetComponent<Image>();
            float val = taskData.Prangs * 1.0f / taskCfg.Count;
            imagePrangs.fillAmount = val;

            Button taskBtn = GetTransform(gameObject.transform, "Task").GetComponent<Button>();
            taskBtn.onClick.AddListener(() => { ClickTaskBtn(gameObject.name); });

            Transform transComp = GetTransform(gameObject.transform, "Task/Tasked");

            if (taskData.Tasked)
            {
                taskBtn.interactable = false;
                SetActive(transComp);
            }
            else
            {
                SetActive(transComp, false);
                taskBtn.interactable = taskData.Prangs==taskCfg.Count;
            }
        }


    }

    private void ClickTaskBtn(string taskName)
    {
        AudioSvc.Instance.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        string[] nameArray = taskName.Split('_');
        int index = int.Parse(nameArray[1]);
        NetMsg netMsg = new NetMsg
        {
            cmd = (int) Command.RequestTask,
            RequestTask = new RequestTask
            {
                TaskId = taskDataList[index].Id
            }
        };
        NetSvc.SendMsg(netMsg);
        TaskRewardCfg taskCfg = ResSvc.GetTaskCfgData(taskDataList[index].Id);
        int coin = taskCfg.Coin;
        int exp = taskCfg.Exp;
        int diamond = taskCfg.Diamond;
        GameRoot.AddTips(ConstRoot.Color("获得奖励：", TextColor.Blue) +
                         ConstRoot.Color(" 金币 +" + coin + " 经验 +" + exp + "钻石" + diamond, TextColor.Green));
    }
    public void ClickCloseBtn()
    {
        AudioSvc.Instance.PlayUiAudioMusic(ConstRoot.UiClickBtn);
        SetWindState(false);
    }
}