/****************************************************
    文件：IState.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:49:18
    功能：状态接口
*****************************************************/

public enum AnimState
{
    None,
    Born,
    Idle,
    Move,
    Attack,
    Hit,
    Die,
}
public interface IState
{
    void Enter(EntityBase entityBase,params object[] args);
    void Process(EntityBase entityBase, params object[] args);
    void Exit(EntityBase entityBase, params object[] args);

}