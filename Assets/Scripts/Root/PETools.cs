/****************************************************
    文件：PETools.cs
	作者：AnderTroy
    邮箱: 1329524041@qq.com
    日期：2019/5/8 10:31:0
    功能：工具
*****************************************************/
public class PETools
{
    /// <summary>
    /// 生成一个随机数，并返回它
    /// </summary>
    public static int RDInt(int min, int max, System.Random random = null)
    {
        if (random==null)
        {
            random = new System.Random();
        }
        int val = random.Next(min, max + 1);
        return val;
    }
}