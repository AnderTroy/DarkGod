/****************************************************
	文件：BaseData.cs
	作者：AnderTroy
	邮箱: 1329524041@qq.com
	日期：2019/05/13 19:46   	
	功能：配置数据类
*****************************************************/

using System.Collections.Generic;
using UnityEngine;
public class BaseData<T>
{
    public int Id;
}
public class BattleAttribute
{
    public int Hp;
    public int Ad;
    public int Ap;
    public int AdDefense;//护甲
    public int ApDefense;//魔抗
    public int Dodge;//闪避概率
    public int Pierce;//穿透比率
    public int Critical;//暴击概率
}
public class MapCfg : BaseData<MapCfg>
{
    public string MapName;//地图名称
    public string SceneName;//场景名称
    public Vector3 MainCamPos;//相机位置
    public Vector3 MainCamRote;//相机旋转
    public Vector3 PlayerBornPos;//角色位置
    public Vector3 PlayerBornRote;//角色旋转
    public Vector3 PlayerBornScale;//角色大小
    public int Power;
    public List<MonsterData> MonsterDataLst;

    public int Coin;
    public int Exp;
    public int Crystal;
}

public class MonsterData : BaseData<MonsterData>
{
    public int MonsterWave;//批次
    public int MonsterIndex;//序号
    public MonsterCfg MonsterCfg;
    public Vector3 MonsterPos;
    public Vector3 MonsterRote;
    public int Level;
}
public class AutoGuideCfg : BaseData<AutoGuideCfg>
{
    public int NpcId;         //触发任务目标NPC索引号
    public string DilogArr;   //对话内容
    public int ActId;          
    public int Coin;          
    public int Exp;
}

public class StrongCfg : BaseData<StrongCfg>
{
    public int Pos;
    public int StartLevel;
    public int AddHp;
    public int AddHurt;
    public int AddDefense;
    public int MinLevel;
    public int Coin;
    public int Crystal;
}

public class TaskRewardCfg : BaseData<TaskRewardCfg>
{
    public string TaskName;
    public int Coin;
    public int Count;
    public int Exp;
    public int Diamond;
    public string Path;
}
public class TaskRewardData : BaseData<TaskRewardData>
{
    public int Prangs;
    public bool Tasked;
}
public class SkillCfg : BaseData<SkillCfg>
{
    public string SkillName;
    public int SkillTime;
    public int SkillCdTime;
    public bool IsComboSkill;
    public bool IsCollide;
    public bool IsBreak;
    public int AnimAction;
    public string EftName;
    public DamageType DamageType;
    public List<int> SkillMoveLst;
    public List<int> SkillActionLst;
    public List<int> SkillDamageLst;
}
public class SkillActionCfg : BaseData<SkillActionCfg>
{
    public int DelayTime;//伤害持续时间点
    public float Radius;//半径
    public int Angle;//角度
}
public class SkillMoveCfg : BaseData<SkillMoveCfg>
{
    public int DelayTime;//技能持续时间
    public int MoveTime;//技能移动时间
    public float MoveDis;
}
public class MonsterCfg : BaseData<SkillMoveCfg>
{
    public string MonsterName;
    public string ResPath;
    public BattleAttribute MonsterAttribute;
    public int SkillId;
    public float AttackDis;
    public bool IsStop;//怪物是否能被攻击中断当前状态
    public MonsterType MonsterType;//1 普通，2 Boss
}