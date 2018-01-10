using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using MoleMole;
using RoomCardNet;

/// <summary>
/// 非模态界面基类，各种UI都需要从此类继承，尤其是需要调用标准提示界面的
/// </summary>
class UnModalUIBase : MonoBehaviourIgnoreGui
{
    //-------------------声明委托-----------------
    public delegate void SysMsgDelegate(int n);  
    //-------------------定义委托----------------
	/// <summary>
	/// 标准系统提示关闭条件达成后调用的委托
	/// </summary>
    public SysMsgDelegate delegate_waitevent;

	/// <summary>
	/// 非模态界面索引号，用于管理维护，自行控制创建与删除的可以不赋值
	/// </summary>
    public int nUnModalUIIndex = -1;
	/// <summary>
	/// 管理SystemMessage的数组，避免重复
	/// </summary>
	public SystemMsgText[] SysMsgArray;

	public int nSysMsgArrayLenth = -1;
	/// <summary>
	/// 非模态界面的删除管理
	/// </summary>
	public virtual void DestroyThis()
    {
        if (!MainRoot._gUIModule.pUnModalUIControl.DeletOneInList(nUnModalUIIndex) && gameObject != null)
        {
            DestroyObject(gameObject);
        }
    }
	/// <summary>
	/// 当标准提示界面倒计时结束后调用进行通知
	/// </summary>
	/// <param name="n"></param>
    public void OnSysMsgTextTimeOverCallFunc(int n)
    {
        //Debug.Log("Unity:"+"On SysMsgText TimeOver:"+n.ToString());
    }
	/// <summary>
	/// 当发生特定事件，需要标准提示界面进行处理时，对创建标准提示界面的界面进行调用
	/// </summary>
	/// <param name="n"></param>
	public virtual void OnSysMsgWaitEventHappen(int n)
    {
        if (n!=0 && delegate_waitevent!=null)
        {
            delegate_waitevent(n);
        }
    }
}
/// <summary>
/// 非模态界面管理器
/// </summary>
class UnModalUIControl : MonoBehaviour
{
    /// <summary>
    /// GameHall场景中BiSaiSelectUIView对象.
    /// </summary>
    [HideInInspector]
    public BiSaiSelectUIView pBiSaiSelectUIView;
    /// <summary>
    /// GameHall场景的UI主界面.
    /// </summary>
    [HideInInspector]
    public MainUIScript pMainUIScript;
	/// <summary>
	/// 麻将房间主界面
	/// </summary>
    [HideInInspector]
    public GameUIView pGameUIView;
	/// <summary>
	/// 创建朋友圈麻将房界面
	/// </summary>
    [HideInInspector]
	public CreatMjUI pCreatMjUI;
    /// <summary>
    /// 玩家战绩面板.
    /// </summary>
    [HideInInspector]
    public PlayerZhanJiPanelCtrl pPlayerZhanJiPanelCtrl;
    /// <summary>
    /// 游戏设置面板.
    /// </summary>
    [HideInInspector]
    public GameSetPanelCtrl pGameSetPanelCtrl;
	/// <summary>
	/// 广播输入对话框
	/// </summary>
	public EnsureDlg pGlobalChatDlg;
    /// <summary>
    /// 非模态窗口List
    /// </summary>
    public List<UnModalUIBase> m_UnModalUIList = new List<UnModalUIBase>();
    int nUnModalUIIndex = 0;
    // Use this for initialization
    public int GetOneUnModalUIIndex()
    {
        nUnModalUIIndex++;
        return nUnModalUIIndex;
    }
    public bool AddMyToUnModalUIList(UnModalUIBase p)
    {
        bool bn = true;

        UnModalUIBase ptemp;
        ptemp = m_UnModalUIList.Find(delegate (UnModalUIBase ps) { return ps.nUnModalUIIndex == p.nUnModalUIIndex; });
        if (ptemp == null)
        {
            m_UnModalUIList.Add(p);
            bn = true;
        }
        else
        {
            bn = false;
        }
        //Debug.Log("Unity:"+bn.ToString()+":"+p.nUnModalUIIndex.ToString()+":"+ m_UnModalUIList.Count.ToString());
        return bn;
    }
    public int JustShowMeOnly(UnModalUIBase p)
    {
        int n = 0, length = m_UnModalUIList.Count;
        //Debug.Log("Unity:"+"--------JustShowMeOnly:" + m_UnModalUIList.Count.ToString());
        UnModalUIBase temp;
        for (int i = 0; i < length; i++)
        {
            if (m_UnModalUIList[0]==null)
            {
                break;
            }
            temp = m_UnModalUIList[0];
            m_UnModalUIList.RemoveAt(0);
            DestroyObject(temp.gameObject);
        }

        m_UnModalUIList.Add(p);
        n = nUnModalUIIndex;
        nUnModalUIIndex++;
        return n;
    }
    public bool DeletOneInList(int nindex)
    {
        UnModalUIBase ptemp;
        int n;
        n = m_UnModalUIList.FindIndex(delegate (UnModalUIBase ps) { return ps.nUnModalUIIndex == nindex; });
        if (n >=0)
        {
            ptemp = m_UnModalUIList[n];
            //Debug.Log("Unity:"+"--------DeletOneInList:" + nindex.ToString() + ":" + ptemp.nUnModalUIIndex.ToString() + ":" + m_UnModalUIList.Count.ToString());
            m_UnModalUIList.RemoveAt(n);
            DestroyObject(ptemp.gameObject);
            //Debug.Log("Unity:"+"--------DeletOneInList:" +  m_UnModalUIList.Count.ToString());
            return true;
        }
        //Debug.Log("Unity:"+"DeletOneInList:" + nindex.ToString() +  ":" + m_UnModalUIList.Count.ToString());
        return false;
    }
    /// <summary>
    /// 网络重新连接后,显示网络不稳定.
    /// </summary>
    public void SpawnSMWangLuoBuWenDing()
    {
        if (IsDisConnect)
        {
            return;
        }
        IsDisConnect = true;

        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            //Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform;
            //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
            //    && !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick) {
            //    trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
            //}
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(16, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//网络重新连接后,显示网络不稳定.
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null) {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(16, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//网络重新连接后,显示网络不稳定.
        }
    }

    /// <summary>
    /// ios内购转圈圈
    /// </summary>
    public void SpawnSMShangChengGouMaiDengDai()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(78, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//ios内购转圈圈
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(78, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//ios内购转圈圈.
        }
    }
    public void DeleteSMShangChengGouMaiDengDai()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.OnSysMsgWaitEventHappen(78);//删除ios内购转圈圈
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(78);//删除ios内购转圈圈
        }
    }
    /// <summary>
    /// 产生"今日海选比赛已结束"系统消息.
    /// </summary>
    public void SpawnJinRiHaiXuanSaiOver()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(67, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"今日海选比赛已结束"系统消息.
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(67, MainRoot._gUIModule.pUnModalUIControl.pGameUIView, trParent);//产生"今日海选比赛已结束"系统消息.
        }
    }

    /// <summary>
    /// 收到"您被房主踢出了比赛场"的系统消息.
    /// </summary>
    public void OnReceivedNetBeiTiChuBiSai()
    {
        Debug.Log("Unity: OnReceivedNetBeiTiChuBiSai");
        SpawnSMBeiTiChuBiSai();
    }

    /// <summary>
    /// 产生"您被房主踢出了比赛场"的系统消息.
    /// </summary>
    public void SpawnSMBeiTiChuBiSai()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"您被房主踢出了比赛场"的系统消息.
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//产生"您被房主踢出了比赛场"的系统消息.
        }
    }

    /// <summary>
    /// 产生"您开启的比赛场数量已达上限"的系统消息.
    /// </summary>
    public void SpawnSMBiSaiChangNumToMax()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"您开启的比赛场数量已达上限"的系统消息.
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//产生"您开启的比赛场数量已达上限"的系统消息.
        }
    }

    /// <summary>
    /// 产生"该房间已开始游戏，无法加入"的系统消息.
    /// </summary>
    //public void SpawnSMBiSaiChangYiKaiShi()
    //{
    //    if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
    //    {
    //        Transform trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
    //        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//产生"该房间已开始游戏，无法加入"的系统消息.
    //    }
    //    if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
    //    {
    //        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(63, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//产生"该房间已开始游戏，无法加入"的系统消息.
    //    }
    //}

    /// <summary>
    /// 网络是否有过中断.
    /// </summary>
    bool IsDisConnect = false;
    /// <summary>
    /// 网络中断,显示网络重新链接.
    /// </summary>
    public void SpawnSMWangLuoChongXinLianJie()
    {
        if (!IsDisConnect) {
            return;
        }
        IsDisConnect = false;

        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(17, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript, trParent);//网络中断,显示网络重新链接.
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.OnSysMsgWaitEventHappen(16);
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null) {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(17, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//网络中断,显示网络重新链接.
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(16);

            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
            {
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallTaoTaiSaiClickStartBiSaiBt(MainRoot._gRoomData.cCurRoomData.IsZongJueSaiRoom == true ? 1 : 0,
                        MainRoot._gRoomData.cCurRoomData.eMultiRaceType);
                }
            }
        }
    }


    /// <summary>
    /// 产生领取救济金面板.
    /// </summary>
    public void SpawnFuLiJinDlg()
    {
        if (!MainRoot._gPlayerData.GetCanBringJiuJiCoin())
        {
            //显示商店,购买金币.
            Debug.Log("Unity:" + "Display shop!");
            return;
        }

        GameObject obj = null;
        EnsureDlg dlg = null;
        Transform trParent = null;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform;
        }
        int fuLiJinCount = MainRoot._gPlayerData.GetJiuJiCount();
        switch (fuLiJinCount)
        {
            case 0:
                obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-FuLiJin01-TuiChu"), trParent, false);
                dlg = obj.GetComponent<EnsureDlg>();
                dlg.Initial(EnsureDlg.EnsureKind.FuLiJin01);
                break;
            case 1:
                obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-FuLiJin02-TuiChu"), trParent, false);
                dlg = obj.GetComponent<EnsureDlg>();
                dlg.Initial(EnsureDlg.EnsureKind.FuLiJin02);
                break;
        }
    }
    /// <summary>
    /// 产生金币购买面板.
    /// </summary>
    public void SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum eShopCoin = ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01)
    {
        ShopManageCtrl shopManage = new ShopManageCtrl();
        int diamondMin = shopManage.GetMinDiamondToCoin(eShopCoin);
        if (diamondMin > MainRoot._gPlayerData.GemCount)
        {
            //用户钻石不足,请充钻石购买金币.
            SpawnShopDiamondCoinDlg(eShopCoin);
            return;
        }

        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
        //{
        //    trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
        //}
        //if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        //{
        //    trParent = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform;
        //}
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-ShopCoin"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.ShopCoinDlg);
        ShopCoinDlgCtrl shopPanel = obj.GetComponent<ShopCoinDlgCtrl>();
        shopPanel.SetShopCoinImg(eShopCoin);
    }

    /// <summary>
    /// 产生充钻石购买金币面板.
    /// </summary>
    public void SpawnShopDiamondCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum eShopCoin = ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01)
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
        //{
        //    trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
        //}
        //if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        //{
        //    trParent = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform;
        //}
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-ShopDiamondCoin"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.ShopDiamondCoinDlg);
        ShopDiamondCoinDlgCtrl shopPanel = obj.GetComponent<ShopDiamondCoinDlgCtrl>();
        shopPanel.SetShopCoinImg(eShopCoin);
    }
    /// <summary>
    /// 商城钻石换金币或房卡状态.
    /// </summary>
    [HideInInspector]
    public EnsureDlg.EnsureKind ShopDiamondState = EnsureDlg.EnsureKind.ShopPanelDlg;
    /// <summary>
    /// 是否选择进入秦人比赛场.
    /// </summary>
    bool IsGotoQinRenBiSaiGame = false;
    public void SetActiveIsGotoQinRenBiSaiGame(bool isActive)
    {
        IsGotoQinRenBiSaiGame = isActive;
    }
    /// <summary>
    /// 商城将钻石自动换为金币或房卡.
    /// </summary>
    public void ShopDiamondToCoinCard()
    {
        Debug.Log("Unity: ShopDiamondToCoinCard -> ShopDiamondState is " + ShopDiamondState);
        switch (ShopDiamondState)
        {
            case EnsureDlg.EnsureKind.ShopDiamondCoinDlg:
                {
                    ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.COIN120000);
                    break;
                }
            case EnsureDlg.EnsureKind.ShopDiamondCardDlg:
                {
                    ShopSendMsgToServer((int)CommonLibrary.PayDefine.PAYGOOD.CARD5);
                    break;
                }
        }
        ShopDiamondState = EnsureDlg.EnsureKind.ShopPanelDlg; //reset

        if (IsGotoQinRenBiSaiGame)
        {
            SetActiveIsGotoQinRenBiSaiGame(false);
            MakePlayerIntoQinRenBiSaiGame();
        }
    }

    /// <summary>
    /// 使玩家进入秦人比赛场游戏场景.
    /// </summary>
    public void MakePlayerIntoQinRenBiSaiGame()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShaanxiMahjong")
        {
            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_OfficialRoom;
            MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong"); //直接创建秦人比赛赛房间，进入游戏场景.
        }
    }

    /// <summary>
    /// 商城购买道具发送网络消息到服务端.
    /// </summary>
    public void ShopSendMsgToServer(int payIndex)
    {
#if UNITY_ANDROID
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            //去微信下单
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerPayRequest(payIndex, CommonLibrary.PayDefine.PayChannel.PayChannel_Android_WeiXin);
        }
#elif UNITY_IPHONE
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            //去微信下单
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerIAPRequest(payIndex,  CommonLibrary.PayDefine.PayChannel.PayChannel_IOS_Official);
        }
#endif
    }

    QinRenSelectUIView.SelectRoomEnum JiBiChaoChuSelectRoom;
    /// <summary>
    /// 产生金币超出面板.
    /// </summary>
    public void SpawnJinBiChaoChuDlg(QinRenSelectUIView.SelectRoomEnum slRoom)
    {
        JiBiChaoChuSelectRoom = slRoom;
        Transform trParent = null;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform;
        }
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ED-JinBiChaoChu-TuiChu"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.JinBiChaoChu);
    }
    //ED-QiTaDengLu
    /// <summary>
    /// 产生其它登陆(重复登陆)对话框.
    /// </summary>
    public void SpawnQiTaDengLuDlg()
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        if (trParent == null)
        {
            Debug.LogWarning("Unity: SpawnQiTaDengLuDlg -> trParent is null!");
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/ED-QiTaDengLu"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.QiTaDengLuDlg);
    }
    /// <summary>
    /// 秦人麻将馆战绩信息.
    /// </summary>
    public object[] CoinRoomZhanJiArgs;
    /// <summary>
    ///房卡房麻将馆战绩信息.
    /// </summary>
    public object[] CardRoomZhanJiArgs;
    byte ZhangJiCount;
    /// <summary>
    /// 检测是否显示玩家战绩面板.
    /// </summary>
    public void CheckIsSpawnPlayerZhanJiPanel(object[] args = null)
    {
        switch (ZhangJiCount)
        {
            case 0:
                {
                    CoinRoomZhanJiArgs = args;
                    break;
                }
            case 1:
                {
                    CardRoomZhanJiArgs = args;
                    break;
                }
        }
        ZhangJiCount++;

        if (ZhangJiCount >= 2)
        {
            ZhangJiCount = 0;
            pPlayerZhanJiPanelCtrl.InitShowPlayerZhanJi(false);
        }
    }
    /// <summary>
    /// 产生玩家战绩面板.
    /// </summary>
    public void SpawnPlayerZhanJiPanel(bool isTestZhanJi = false)
    {
        Transform trParent = null;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
        }
        if (trParent == null)
        {
            Debug.LogWarning("Unity: SpawnPlayerZhanJiPanel -> trParent is null!");
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/PlayerZhanJiPanel/PlayerZhanJiPanelCtrl"), trParent, false);
        pPlayerZhanJiPanelCtrl = obj.GetComponent<PlayerZhanJiPanelCtrl>();
        pPlayerZhanJiPanelCtrl.InitPlayerZhanJiPanel(isTestZhanJi);
    }
    /// <summary>
    /// 产生游戏设置面板.
    /// </summary>
    public void SpawnGameSetPanel()
    {
        Transform trParent = null;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
            //    && !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick)
            //{
            //    trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
            //}
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform;
        }
        if (trParent == null)
        {
            Debug.LogWarning("Unity: SpawnGameSetPanel -> trParent is null!");
            return;
        }
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/GameSetPanel/ED-GameSetPanel"), trParent, false);
        pGameSetPanelCtrl = obj.GetComponent<GameSetPanelCtrl>();
        pGameSetPanelCtrl.InitGameSetPanel();
    }
    /// <summary>
    /// 产生游戏帮助面板.
    /// </summary>
    public void SpawnGameHelpDlg()
    {
        Transform trParent = null;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
            //    && !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick)
            //{
            //    trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
            //}
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform;
        }
        if (trParent == null)
        {
            Debug.LogWarning("Unity: SpawnGameHelpDlg -> trParent is null!");
            return;
        }
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameHelpDlg/ED-GameHelpDlg"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.GameHelpDlg);
    }
    /// <summary>
    /// 产生游戏消息面板.
    /// </summary>
    public void SpawnGameMsgDlg()
    {
        Transform trParent = null;
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
        {
            trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
            //    && !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick)
            //{
            //    trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.transform;
            //}
        }
        if (trParent == null)
        {
            Debug.LogWarning("Unity: SpawnGameMsgDlg -> trParent is null!");
            return;
        }
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/GameMsgDlg/ED-GameMsgDlg"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.GameMsgDlg);
    }
    /// <summary>
    /// 金币超出按键被点击.
    /// </summary>
    public void OnPlayerClickJiBiChaoChuBt()
    {
        switch (JiBiChaoChuSelectRoom)
        {
            case QinRenSelectUIView.SelectRoomEnum.RoomLevel01:
				int[] coinMax = MainRoot._gPlayerData.GetGameCoinNumMaxAy();
				if (MainRoot._gPlayerData.nCoinNum< coinMax[1])
				{
					JiBiChaoChuSelectRoom = QinRenSelectUIView.SelectRoomEnum.RoomLevel02;
					MakePlayerGotoGoldRoom(1);
				}
				else
				{
					JiBiChaoChuSelectRoom = QinRenSelectUIView.SelectRoomEnum.RoomLevel03;
					MakePlayerGotoGoldRoom(2);
				}
				break;
            case QinRenSelectUIView.SelectRoomEnum.RoomLevel02:
                JiBiChaoChuSelectRoom = QinRenSelectUIView.SelectRoomEnum.RoomLevel03;
                MakePlayerGotoGoldRoom(2);
                break;
        }
    }
    /// <summary>
    /// 使玩家进入金币房.
    /// </summary>
    public void MakePlayerGotoGoldRoom(byte roomLevel)
    {
        Debug.Log("Unity:" + "MakePlayerGotoGoldRoom -> roomLevel " + roomLevel);
        MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_Gold;
        MainRoot._gRoomData.cCacheRoomData.nGoldRoomLevel = roomLevel;
        switch (roomLevel)
        {
            case 0:
                MainRoot._gRoomData.cCacheRoomData.nDiFen = 500;
                break;
            case 1:
                MainRoot._gRoomData.cCacheRoomData.nDiFen = 1500;
                break;
            case 2:
                MainRoot._gRoomData.cCacheRoomData.nDiFen = 3000;
                break;
        }

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShaanxiMahjong")
        {
            MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong"); //直接创建金币房间，进入游戏场景
        }

        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerAddGoldRoom(roomLevel);
        }
    }

    /// <summary>
    ///游戏比赛排行的数据信息.
    /// </summary>
    [HideInInspector]
    public object[] GameBiSaiPaiHangArgs;
    /// <summary>
    /// 设置比赛模式下玩家的排行榜数据信息.
    /// </summary>
    public void SetGameBiSaiPaiHangDt(object[] args)
    {
        GameBiSaiPaiHangArgs = args;
    }
    
    /// <summary>
    /// 自建比赛房间里玩家列表数据面板.
    /// </summary>
    [HideInInspector]
    public MyRoomPlayerPanelDlg pMyRoomPlayerPanelDlg;
    /// <summary>
    /// 产生自建比赛玩家列表数据面板.
    /// </summary>
    public void SpawnMyRoomPlayerPanelDlg()
    {
        Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
        if (trParent == null)
        {
            Debug.LogWarning("Unity: SpawnMyRoomPlayerPanelDlg -> trParent is null!");
            return;
        }
        if (pMyRoomPlayerPanelDlg != null)
        {
            return;
        }

        //这里向服务端发送请求自建比赛房间玩家列表数据的消息.

        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/MyRoomRuKou/ED-MyRoomPlayerPanel"), trParent, false);
        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
        dlg.Initial(EnsureDlg.EnsureKind.MyRoomPlayerPanel);
        pMyRoomPlayerPanelDlg = obj.GetComponent<MyRoomPlayerPanelDlg>();
        pMyRoomPlayerPanelDlg.ShowMyRoomPlayerListInfo(); //test.
    }
}