using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class HuaShangDaZhuanPanCtrl : MonoBehaviour
{
    /// <summary>
    /// 转盘数据信息.
    /// </summary>
    //class ZhuanPanMoneyDt
    //{
    //    public int Index;
    //    public int Money;
    //    public ZhuanPanMoneyDt(int indexVal, int moneyVal)
    //    {
    //        Index = indexVal;
    //        Money = moneyVal;
    //    }
    //}
    //List<ZhuanPanMoneyDt> ZPMoneyDtListA = new List<ZhuanPanMoneyDt>(7) { new ZhuanPanMoneyDt(0, 10),
    //    new ZhuanPanMoneyDt(1, 5),
    //    new ZhuanPanMoneyDt(2, 3),
    //    new ZhuanPanMoneyDt(3, 1),
    //    new ZhuanPanMoneyDt(4, 2),
    //    new ZhuanPanMoneyDt(5, 4),
    //    new ZhuanPanMoneyDt(6, 8),
    //};
    //List<ZhuanPanMoneyDt> ZPMoneyDtListB = new List<ZhuanPanMoneyDt>(7) { new ZhuanPanMoneyDt(6, 8),
    //    new ZhuanPanMoneyDt(7, 5),
    //    new ZhuanPanMoneyDt(8, 3),
    //    new ZhuanPanMoneyDt(9, 1),
    //    new ZhuanPanMoneyDt(10, 2),
    //    new ZhuanPanMoneyDt(11, 4),
    //    new ZhuanPanMoneyDt(0, 10),
    //};
    /// <summary>
    /// 转盘指针.
    /// </summary>
    public Transform PointRcTr;
    /// <summary>
    /// 加速度.
    /// </summary>
    float AccVal = -1000f;
    /// <summary>
    /// 指针运动的总路程.
    /// </summary>
    float DisVal = 0f;
    /// <summary>
    /// 指针运动的当前路程.
    /// </summary>
    float DisValCur = 0f;
    /// <summary>
    /// 是否转动指针.
    /// </summary>
    bool IsRunPoint;
    /// <summary>
    /// 是否匀速转动指针.
    /// </summary>
    bool IsYunSuRunPoint = false;
    /// <summary>
    /// 最多抽奖次数.
    /// </summary>
    int MaxCountRunPoint = 1;
    int CountRunPoint = 0; //已经抽奖的次数.
    /// <summary>
    /// 初速度.
    /// </summary>
    public float StartSpeed = -500f;
    float TimeRunPoint = 0f;
    float MaxTimeRunPoint = 0f;
    EnsureDlg DlgCom;
    public void Init()
    {
        DlgCom = GetComponent<EnsureDlg>();
        DlgCom.btn1.SetActive(true);
        DlgCom.btn3.SetActive(false);
    }

    /// <summary>
    /// 开始匀速转动指针.
    /// </summary>
    public void StartYunSuRunPoint()
    {
        if (MainRoot._gPlayerData == null)
        {
            Debug.LogError("Unity:" + "_gPlayerData Not Initial!");
            return;
        }
        if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
        {
            Debug.Log("Unity:" + "Please login!");
            return;
        }
        if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
        {
            Debug.Log("Unity: player is not login!");
            return;
        }

        if (CountRunPoint >= MaxCountRunPoint)
        {
            return;
        }

        if (IsRunPoint)
        {
            return;
        }
        IsYunSuRunPoint = true;
        CountRunPoint++;
        DlgCom.btn1.SetActive(false); //Go按键
        DlgCom.btn3.SetActive(true);  //Go按键置灰
        DlgCom.btn2.SetActive(false); //关闭按键
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallHuaShangZhuanPan_GetPointIndex();
        }
    }

    /// <summary>
    /// 初始化运动指针.
    /// arg: [0, 11] 转盘的索引,该数据是由服务器返回的.
    /// </summary>
    public void InitMovePoint(int arg)
    {
        int indexVal = arg % 12;
        //int rv = Random.Range(0, 100) % 2;
        //ZhuanPanMoneyDt moneyDt = null;
        //if (rv == 0)
        //{
        //    moneyDt = ZPMoneyDtListA.Find(delegate (ZhuanPanMoneyDt moneyDtTmp) { return moneyDtTmp.Money.Equals(val); });
        //}
        //else
        //{
        //    moneyDt = ZPMoneyDtListB.Find(delegate (ZhuanPanMoneyDt moneyDtTmp) { return moneyDtTmp.Money.Equals(val); });
        //}
        //indexVal = moneyDt.Index;
        DisVal = (4 * 360f) + (indexVal * 30f);
        MaxTimeRunPoint = 2f * (DisVal / Mathf.Abs(StartSpeed));
        AccVal = -(StartSpeed / MaxTimeRunPoint);
        DisValCur = 0f;
        TimeRunPoint = 0f;
        IsRunPoint = true;
    }

    void Update()
    {
        if (IsYunSuRunPoint)
        {
            float disZ = Time.deltaTime * StartSpeed;
            PointRcTr.localEulerAngles += new Vector3(0f, 0f, disZ);
            //Debug.Log("Unity: localEulerAngles " + PointRcTr.localEulerAngles);
            if (IsRunPoint)
            {
                if (Mathf.Abs(PointRcTr.localEulerAngles.z) <= Mathf.Abs(disZ)
                    || Mathf.Abs(360f - PointRcTr.localEulerAngles.z) <= Mathf.Abs(disZ))
                {
                    IsYunSuRunPoint = false;
                    PointRcTr.localEulerAngles = Vector3.zero;
                }
            }
            return;
        }

        if (IsRunPoint)
        {
            CheckIsRunPoint();
        }
    }

    /// <summary>
    /// 检测是否匀变速转动指针.
    /// </summary>
    void CheckIsRunPoint()
    {
        TimeRunPoint += Time.deltaTime;
        DisValCur = (StartSpeed * TimeRunPoint) + 0.5f * AccVal * Mathf.Pow(TimeRunPoint, 2f);
        if (Mathf.Abs(DisValCur) >= DisVal || TimeRunPoint >= MaxTimeRunPoint)
        {
            IsRunPoint = false;
            DisValCur = DisVal;
            MaxTimeRunPoint = 0f;
            DlgCom.btn2.SetActive(true);
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgQRGongZhongHao();
            return;
        }
        PointRcTr.localEulerAngles = new Vector3(0f, 0f, DisValCur);
    }
}