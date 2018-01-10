using UnityEngine;
using System.Collections;

/// <summary>
/// 秦人比赛场玩家排行界面.
/// </summary>
class QinRenBiSaiPaiHangDlg : MonoBehaviour
{
    /// <summary>
    /// PaiHang_PANEL -> 排行面板.
    /// JiangLi_PANEL -> 奖励面板.
    /// </summary>
    public enum BiSaiEnum
    {
        PaiHang_PANEL,
        JiangLi_PANEL,
    }
    /// <summary>
    /// 排行面板.
    /// </summary>
    public GameObject PaiHangPanel;
    /// <summary>
    /// 奖励面板.
    /// </summary>
    public GameObject JiangLiPanel;
    public ImageBase BtnPaiHangImg;
    public ImageBase BtnJiangLiImg;
    /// <summary>
    /// 秦人比赛场数据(插图、房间信息、规则、倒计时、报名按键).
    /// 参数格式:
    /// args[x]:  
    /// </summary>
    public object[] QinRenBiSaiPaiHangDt;
    void Start()
    {
        ShowQinRenBiSaiPaiHangInfo(); //test
    }
    /// <summary>
    /// 显示秦人比赛场入口界面信息.
    /// </summary>
    public void ShowQinRenBiSaiPaiHangInfo(object[] args = null)
    {
        QinRenBiSaiPaiHangDt = args;
        int listDtCount = 0;
        //for (int i = 0; i < 8; i++)
        //{
        //    if ((int)HaiXuanPaiHangDt[(i * 3) + 3] != 0)
        //    {
        //        listDtCount++;
        //    }
        //    else
        //    {
        //        break;
        //    }
        //}

        listDtCount = 10; //test.
        if (listDtCount <= 0)
        {
            return;
        }
        VerticalListUICtrl verListUI = GetComponent<VerticalListUICtrl>();
        VerticalListUICtrl.ObjListConfigDt configDt = new VerticalListUICtrl.ObjListConfigDt();
        configDt.CountObj = listDtCount;
        configDt.ObjName = "QinRenBiSaiPaiHangUI";
        configDt.ObjPrefab = "Prefab/QinRenBiSaiPaiHang/" + configDt.ObjName;
        verListUI.CreateObjList(configDt);
    }

    /// <summary>
    /// 显示秦人比赛排行对应面板.
    /// </summary>
    public void ShowShopPanel(BiSaiEnum panelSt)
    {
        PaiHangPanel.SetActive(panelSt == BiSaiEnum.PaiHang_PANEL ? true : false);
        JiangLiPanel.SetActive(panelSt == BiSaiEnum.JiangLi_PANEL ? true : false);

        BtnPaiHangImg.color = new Color(1f, 1f, 1f, panelSt == BiSaiEnum.PaiHang_PANEL ? 1f : 0f);
        BtnJiangLiImg.color = new Color(1f, 1f, 1f, panelSt == BiSaiEnum.JiangLi_PANEL ? 1f : 0f);
    }
}