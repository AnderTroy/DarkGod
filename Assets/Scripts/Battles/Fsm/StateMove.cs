/****************************************************
    文件：StateMove.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/27 9:50:11
    功能：移动状态
*****************************************************/
using PEProtocol;

public class StateMove : IState 
{
    public void Enter(EntityBase entityBase, params object[] args)
    {
        entityBase.CurrentAnimState = AnimState.Move;
        //PeRoot.Log("Enter StateMove.");
    }

    public void Exit(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Exit StateMove");
    }
    public void Process(EntityBase entityBase, params object[] args)
    {
        //PeRoot.Log("Process StateMove");
        entityBase.SetBlend(ConstRoot.BlendMove);
    }

}