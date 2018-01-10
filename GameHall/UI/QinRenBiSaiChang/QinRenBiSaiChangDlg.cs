using UnityEngine;
using System.Collections;

/// <summary>
/// 秦人比赛场(红包赛)入口界面.
/// </summary>
class QinRenBiSaiChangDlg : MonoBehaviour
{
    /// <summary>
    /// 秦人比赛场数据(比赛场次数量、插图、房间信息、规则、倒计时、报名按键信息和状态).
    /// 参数格式:
    /// args[x]: 3 数据长度.
    /// </summary>
    public object[] QinRenBiSaiChangDt;
    bool IsStartBiSaiDaoJiShi;
    float TimeBiSaiLastVal;
    public delegate void EventHandel(int val);
    public event EventHandel OnUpdateBiSaiDaoJiShi;
    int _TimeBiSaiDaoJiShiVal;
    /// <summary>
    /// 倒计时计数.
    /// </summary>
    int TimeBiSaiDaoJiShi
    {
        set
        {
            if (OnUpdateBiSaiDaoJiShi != null)
            {
                OnUpdateBiSaiDaoJiShi(value);
            }
            _TimeBiSaiDaoJiShiVal = value;
        }

        get
        {
            return _TimeBiSaiDaoJiShiVal;
        }
    }
    //void Start()
    //{
    //    ShowQinRenBiSaiChangInfo(); //test
    //}

    void Update()
    {
        if (!IsStartBiSaiDaoJiShi)
        {
            return;
        }

        if (Time.time - TimeBiSaiLastVal >= 1f)
        {
            TimeBiSaiLastVal = Time.time;
            TimeBiSaiDaoJiShi++;
        }
    }

    /// <summary>
    /// 显示秦人比赛场入口界面信息.
    /// </summary>
    public void ShowQinRenBiSaiChangInfo(object[] args = null)
    {
        TimeBiSaiDaoJiShi = 0;
        IsStartBiSaiDaoJiShi = true;
        TimeBiSaiLastVal = Time.time;

        QinRenBiSaiChangDt = args;
        int listDtCount = 0;
        listDtCount = (int)args[3];
        //listDtCount = 10; //test.
        if (listDtCount <= 0)
        {
            return;
        }
        VerticalListUICtrl verListUI = GetComponent<VerticalListUICtrl>();
        VerticalListUICtrl.ObjListConfigDt configDt = new VerticalListUICtrl.ObjListConfigDt();
        configDt.CountObj = listDtCount;
        configDt.ObjName = "QinRenBiSaiUI";
        configDt.ObjPrefab = "Prefab/QinRenBiSaiChang/" + configDt.ObjName;
        verListUI.CreateObjList(configDt);
    }

    /// <summary>
    /// 收到点击报名按键的消息.
    /// arg -> 房间id.
    /// </summary>
    public void OnReceivedBaoMingBtMsg(int arg)
    {
        MainRoot._gUIModule.pUnModalUIControl.MakePlayerIntoQinRenBiSaiGame();
    }
}