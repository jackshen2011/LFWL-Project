using UnityEngine;
using System.Collections;

/// <summary>
/// 海选赛入场券控制面板.
/// </summary>
public class HaiXuanRuChangQuanDlg : MonoBehaviour
{
    /// <summary>
    /// 点击海选赛入场券购买按键.
    /// </summary>
    public void OnClickGouMaiRuChangQuanBt()
    {
        Debug.Log("Unity: OnClickGouMaiRuChangQuanBt!");
        //这里添加向服务端发送购买海选入场券按键消息.
    }

    /// <summary>
    /// 收到服务端海选赛入场券购买按键的返回消息.
    /// </summary>
    public void OnReceivedGouMaiRuChangQuanBtMsg(int arg)
    {
        switch (arg)
        {
            case 0: //购买成功.
                {
                    Debug.Log("Unity: HaiXuangRuChangQuan gouMai chengGong!");
                    MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pHaiXuanRuKouDlg.MakePlayerIntoHaiXuanSai();
                    break;
                }
            case 1: //购买失败.
                {
                    Debug.Log("Unity: HaiXuangRuChangQuan gouMai shiBai!");
                    break;
                }
            default:
                {
                    Debug.Log("Unity: OnReceivedGouMaiRuChangQuanBtMsg -> arg was wrong! arg == " + arg);
                    break;
                }
        }
    }
}