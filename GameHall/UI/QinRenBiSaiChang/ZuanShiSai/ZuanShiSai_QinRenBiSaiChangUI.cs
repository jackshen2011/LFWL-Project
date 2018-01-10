using UnityEngine;
using System.Collections;
using System;

class ZuanShiSai_QinRenBiSaiChangUI : ListUiDtCtrl
{
    /// <summary>
    /// 开始报名按键置灰.
    /// </summary>
    public GameObject ZhiHuiBtImg;
    /// <summary>
    /// 已报名按键.
    /// </summary>
    public GameObject YiBaoMingBt;
    /// <summary>
    /// 插图sprite.
    /// </summary>
    //public Sprite[] ChaTuSpArray;
    /// <summary>
    /// 报名所需费用.
    /// </summary>
    int BaoMingFei = 0;
    /// <summary>
    /// 比赛倒计时.
    /// </summary>
    int DaoJiShiVal = 0;
    /// <summary>
    /// 比赛倒计时的最大值.
    /// </summary>
    int MaxDaoJiShiVal = 0;
    /// <summary>
    /// 比赛场次的索引信息.
    /// </summary>
    int BiSaiId;
    void Start()
    {
        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.pQinRenBiSaiChangDlg.OnUpdateBiSaiDaoJiShi += OnUpdateBiSaiDaoJiShi;
    }
    
    /// <summary>
    /// 更新倒计时.
    /// </summary>
    void OnUpdateBiSaiDaoJiShi(int val)
    {
        ShowBiSaiDaoJiShi(val);
    }

    /// <summary>
    /// 显示倒计时.
    /// </summary>
    void ShowBiSaiDaoJiShi(int val)
    {
        if (ZhiHuiBtImg.activeInHierarchy)
        {
            return;
        }

        int rv = DaoJiShiVal - val;
        if (rv >= 60)
        {
            UiText[2].text = (rv / 60).ToString("d2") + ":" + (rv % 60).ToString("d2");
        }
        else if (rv >= 0)
        {
            UiText[2].text = "00:" + (rv % 60).ToString("d2");
            if (rv == 0)
            {
                DaoJiShiVal = MaxDaoJiShiVal;
                YiBaoMingBt.SetActive(false);
            }
        }
        else
        {
            UiText[2].text = "0";
        }
    }

    /// <summary>
    /// args[x]: 0 -> 30/15元钻石赛, 1 开始时间, 2 结束时间, 3 报名费用, 4 比赛倒计时的最大值.
    /// 5 暂未开始, 6 是否已经报名, 7 比赛场次的id, 8 比赛倒计时(秒).
    /// </summary>
    public override void ShowListUiDt(object[] args)
    {
        //报名或已报名.
        //UiText[0].text = "50";
        //UiText[1].text = "10:00-23:59";
        //UiText[2].text = "10:00";
        //UiText[3].text = "2";
        //BaoMingCoin = 20000;
        //ZhiHuiBtImg.SetActive(false);

        BaoMingFei = (int)args[3];
        UiText[0].text = (string)args[0];
        DateTime startTime = (DateTime)args[1];
        DateTime endTime = (DateTime)args[2];
        YiBaoMingBt.SetActive((bool)args[6]);
        BiSaiId = (int)args[7];

        bool isZanWeiKaiShi = !(bool)args[5];
        if (!isZanWeiKaiShi)
        {
            MaxDaoJiShiVal = (int)args[4];
            DaoJiShiVal = (int)args[8];
            ShowBiSaiDaoJiShi(0);
        }
        else
        {
            UiText[2].text = "暂未开始";
        }
        UiText[1].text = startTime.ToString("HH:mm") + "-" + endTime.ToString("HH:mm");
        UiText[3].text = BaoMingFei.ToString();
        //UiImg[0].sprite = ChaTuSpArray[coinNum == 50 ? 0 : 1];
        ZhiHuiBtImg.SetActive(isZanWeiKaiShi);
    }

    /// <summary>
    /// 当报名按键被点击.
    /// </summary>
    public void OnClickBt()
    {
        Debug.Log("Unity: OnClickBt");
        //MainRoot._gPlayerData.nCoinNum = 0; MainRoot._gPlayerData.GemCount = 0; //test
        if (MainRoot._gPlayerData.GemCount < BaoMingFei)
        {
            MainRoot._gUIModule.pUnModalUIControl.SetActiveIsGotoQinRenBiSaiGame(true);
            MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01);
            return;
        }
        
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallQinRenBiSaiChangDlgDt(BiSaiId);
        }
    }
}