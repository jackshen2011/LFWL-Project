using UnityEngine;
using System.Collections;

class MyRoomWaitPanel : MonoBehaviour
{
    /// <summary>
    /// 内容.
    /// </summary>
    public TextBase NeiRongTx;
    /// <summary>
    /// 已加人数.
    /// </summary>
    public TextBase YiJiaPlayerTx;
    /// <summary>
    /// 还需人数.
    /// </summary>
    public TextBase HaiXuPlayerTx;
    /// <summary>
    /// 初始化.
    /// </summary>
    public void Init()
    {
        NeiRongTx.text = "32人/8局制";
        YiJiaPlayerTx.text = "0";
        HaiXuPlayerTx.text = "32";
    }

    /// <summary>
    /// 显示自建房等待界面信息.
    /// args[x]: 0 房间最大人数, 1 最大局数, 2 已加入的人数.
    /// </summary>
    void ShowPanelInfo(object[] args)
    {
        int maxPlayer = 32;
        int maxJuShu = 8;
        int playerNum = 0;
        //maxPlayer = (int)args[0];
        //maxJuShu = (int)args[1];
        //playerNum = (int)args[2];
        int playerNumNeed = maxPlayer - playerNum;
        NeiRongTx.text = maxPlayer + "人/" + maxJuShu + "局制";
        YiJiaPlayerTx.text = playerNum.ToString();
        HaiXuPlayerTx.text = HaiXuPlayerTx.ToString();
    }
}