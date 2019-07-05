/****************************************************
    文件：ConstRoot.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/7 14:15:27
    功能：常量配置
*****************************************************/
public enum TextColor
{
    Red,
    Green,
    Blue,
    Yellow
}

public enum DamageType
{
    None,
    Ad,
    Ap,
}

public enum EntityType
{
    None,
    Player,
    Monster,
}
public enum EntityState
{
    None,
    BaseState,//霸体
    //TODO
}
public enum MonsterType
{
    None,
    Normal,
    Boss,
    //TODO
}


public class ConstRoot
{
    private const string ColorRed = "<color=#FF0000FF>";
    private const string ColorGreen = "<color=#00FF00FF>";
    private const string ColorBlue = "<color=#00B4FFFF>";
    private const string ColorYellow = "<color=#FFFF00FF>";
    private const string ColorEnd = "</color>";
    public static string Color(string str, TextColor color)
    {
        string result = "";
        switch (color)
        {
            case TextColor.Red:
                result = ColorRed + str + ColorEnd;
                break;
            case TextColor.Green:
                result = ColorGreen + str + ColorEnd;
                break;
            case TextColor.Blue:
                result = ColorBlue + str + ColorEnd;
                break;
            case TextColor.Yellow:
                result = ColorYellow + str + ColorEnd;
                break;
        }
        return result;
    }
    //场景名称||ID
    public const string StartScene = "StartScene";//开始场景
    public const int MainCityMapId = 10000;//主城ID
    //public const string MainCityScene = "SceneMainCity";//主城场景
    
    //背景音效
    public const string BgAudio = "bgLogin";//登入界面
    public const string MainCityBgAudio = "bgMainCity";//主城界面
    public const string BattleBgAudio = "bgHuangYe";//主城界面

    public const string BattleLoseBgAudio = "fblose";
    public const string BattleWinBgAudio = "fbwin";
    public const string BattleItemBgAudio = "fbitem";

    //按键音效
    public const string UiLoginBtn = "uiLoginBtn";//登录按钮
    public const string UiClickBtn = "uiClickBtn";//常规UI点击
    public const string UiOpenBtnAudio = "uiOpenPage";//按键开
    public const string UiCloseBtnAudio = "uiCloseBtn";//按键关
    public const string UiExitBtnAudio = "uiExtenBtn";//按键退出

    public const string PlayerHurtAudio = "assassin_Hit";

    //遥杆常量
    public const int ScreenStandardWidth = 2160;//屏幕宽度
    public const int ScreenStandardHeight = 1080;//屏幕盖度
    public const int ScreenOriginDis = 100;//标准拖拽距离
    
    //运动混合参数 
    public const int BlendIdle = 0;//待机状态
    public const int BlendMove = 1;//运动状态
    public const int ActionDefault = -1;
    public const int ActionBorn = 0;
    public const int ActionDie = 100;
    public const int ActionHit = 101;

    public const int DieTimeLength = 3000;

    public const int PlayerMoveSpeed = 8;//角色移动速度
    public const int MonsterMoveSpeed = 3;//怪物移动速度
    public const float AccelerationSpeed = 5;//运动平滑加速度

    public const float AccelerationHpSpeed = 0.25f;

    //AutoGuideNPC
    public const int NpcWiseMan = 0;
    public const int NpcGeneral = 1;
    public const int NpcArtisan = 2;
    public const int NpcTrader = 3;
    
    public const int ComboSpace = 500;
}