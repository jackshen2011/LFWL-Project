using UnityEngine;
using System.Collections;
using MoleMole;
using System;

/// <summary>
/// 玩家战绩界面控制.
/// </summary>
class PlayerZhanJiPanelCtrl : MonoBehaviour
{
    /// <summary>
    /// COIN_PANEL -> 金币房战绩面板.
    /// CARD_PANEL -> 房卡房战绩面板.
    /// </summary>
    public enum ZhanJiPanelEnum
    {
        COIN_PANEL,
        CARD_PANEL,
    }
    ZhanJiPanelEnum SelectPanelSt = ZhanJiPanelEnum.COIN_PANEL;
    /// <summary>
    /// 金币房战绩面板.
    /// </summary>
    public GameObject CoinPanel;
    /// <summary>
    /// 房卡房战绩面板.
    /// </summary>
    public GameObject CardPanel;
    public ImageBase BtnCoinImg;
    public ImageBase BtnCardImg;
    CardZhanJiListUICtrl CardZhanJiList;
    /// <summary>
    /// 初始化战绩界面.
    /// </summary>
    public void InitPlayerZhanJiPanel(bool isTestZhanJi)
    {
        EnsureDlg enDlg = GetComponent<EnsureDlg>();
        enDlg.Initial(EnsureDlg.EnsureKind.PlayerZhanJiPanelDlg);
        ShowPlayerZhanJiPanel(ZhanJiPanelEnum.COIN_PANEL);
    }

    /// <summary>
    /// 初始化战绩数据面板.
    /// </summary>
    public void InitShowPlayerZhanJi(bool isTestZhanJi)
    {
        CoinZhanJiListUICtrl coinZhanJiList = GetComponent<CoinZhanJiListUICtrl>();
        coinZhanJiList.CreateZhanJiList(isTestZhanJi);
        CardZhanJiList = GetComponent<CardZhanJiListUICtrl>();
        CardZhanJiList.CreateZhanJiList(isTestZhanJi);
        ShowPlayerZhanJiPanel(ZhanJiPanelEnum.COIN_PANEL);
    }
    /// <summary>
    /// 显示玩家战绩面板.
    /// </summary>
    public void ShowPlayerZhanJiPanel(ZhanJiPanelEnum panelSt)
    {
        bool isDisplayCoinPanel = panelSt == ZhanJiPanelEnum.COIN_PANEL ? true : false;
        CoinPanel.SetActive(isDisplayCoinPanel);
        CardPanel.SetActive(!isDisplayCoinPanel);
        SelectPanelSt = panelSt;

        BtnCoinImg.color = new Color(1f, 1f, 1f, panelSt == ZhanJiPanelEnum.COIN_PANEL ? 1f : 0f);
        BtnCardImg.color = new Color(1f, 1f, 1f, panelSt == ZhanJiPanelEnum.CARD_PANEL ? 1f : 0f);
    }
    int CurentIndexVal = 3;
    /// <summary>
    /// 当点击房卡房的分时战绩按键.
    /// indexVal -> 0 一周, 1 一月, 2 三月, 其它 所有.
    /// </summary>
    public void OnClickCardTimeZhanJiBt(int indexVal)
    {
        if (CurentIndexVal == indexVal)
        {
            return;
        }
        CurentIndexVal = indexVal;
        Debug.Log("Unity: OnClickCardTimeZhanJiBt -> indexVal " + indexVal);
        RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerGetRecordTotal(1, (byte)indexVal);
    }
    /// <summary>
    /// 当收到玩家分时战绩信息.
    /// </summary>
    public void OnReceivedPlayerTimeZhanJi(object[] args = null)
    {
        CardZhanJiList.ShowPlayerTimeZhanJi(args);
    }
    /// <summary>
    /// 已重载.计算两个日期的时间间隔,返回的是时间间隔的日期差的绝对值.
    /// </summary>
    /// <returns></returns>
    public string DateDiff(DateTime timeVal)
    {
        string dateDiff = null;
        try
        {
            TimeSpan ts1 = new TimeSpan(DateTime.Now.Ticks);
            TimeSpan ts2 = new TimeSpan(timeVal.Ticks);
            TimeSpan ts = ts1.Subtract(ts2).Duration();
            if (ts.Days > 365)
            {
                dateDiff = "一年以上";
            }
            else if (ts.Days > 180)
            {
                dateDiff = "一年内";
            }
            else if (ts.Days > 60)
            {
                dateDiff = "半年内";
            }
            else if (ts.Days > 30)
            {
                dateDiff = "2个月内";
            }
            else if (ts.Days > 15)
            {
                dateDiff = "1个月内";
            }
            else if (ts.Days > 4)
            {
                dateDiff = "15天内";
            }
            else if (ts.Days > 2)
            {
                dateDiff = "4天内";
            }
            else if (ts.Days > 1)
            {
                dateDiff = "2天内";
            }
            else
            {
                if (ts.Hours > 12)
                {
                    dateDiff = "1天内";
                }
                else if (ts.Hours > 6)
                {
                    dateDiff = "12小时内";
                }
                else if (ts.Hours > 4)
                {
                    dateDiff = "6小时内";
                }
                else if (ts.Hours > 2)
                {
                    dateDiff = "4小时内";
                }
                else if (ts.Hours > 1)
                {
                    dateDiff = "2小时内";
                }
                else if (ts.Hours > 0)
                {
                    dateDiff = "1小时内";
                }
                else
                {
                    if (ts.Minutes > 30)
                    {
                        dateDiff = "1小时内";
                    }
                    else if (ts.Minutes > 10)
                    {
                        dateDiff = "30分钟内";
                    }
                    else
                    {
                        dateDiff = "刚刚";
                    }
                }
            }
            /*dateDiff = ts.Days.ToString() + "天"
                    + ts.Hours.ToString() + "小时"
                    + ts.Minutes.ToString() + "分钟"
                    + ts.Seconds.ToString() + "秒";*/
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Unity: DateDiff -> " + ex.ToString());
        }
        return dateDiff;
    }
}