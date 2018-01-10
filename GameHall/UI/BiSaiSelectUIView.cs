using UnityEngine;
using System.Collections;

namespace MoleMole
{
    class BiSaiSelectUIViewContext : BaseContext
    {
        public BiSaiSelectUIViewContext() : base(UIType.BiSaiSelectUI)
        {
            // ViewType = UIType.MainUI;
        }
    }
    class BiSaiSelectUIView : AnimateView
    {
        OneRoomData.DataDefine_MultiRaceType eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_NULL;
        /// <summary>
        /// 是否开放社区麻将赛.
        /// </summary>
        [HideInInspector]
        public bool IsOpenSheQuMaJiangSai = false;
        /// <summary>
        /// GameHall场景里玩家的头像,昵称等信息管理.
        /// </summary>
        public PlayerInfoCtrl pPlayerInfoCtrl;
        /// <summary>
        /// 跳过该面板时隐藏的对象.
        /// </summary>
        public GameObject[] HiddenObjArray;
        [HideInInspector]
        public bool b_isClick;
        /// <summary>
        /// 秦人比赛控制对象.
        /// </summary>
        [HideInInspector]
        public QinRenBiSaiCtrl pQinRenBiSaiCtrl;
        /// <summary>
        /// 秦人比赛场(红包赛)入口UI面板控制对象.
        /// </summary>
        [HideInInspector]
        public QinRenBiSaiChangDlg pQinRenBiSaiChangDlg;
        /// <summary>
        /// 秦人比赛场(钻石赛)入口UI面板控制对象.
        /// </summary>
        [HideInInspector]
        public ZuanShiSai_QinRenBiSaiChangDlg pZuanShiSaiQinRenBiSaiChangDlg;
        /// <summary>
        /// 自建比赛入口UI面板控制对象.
        /// </summary>
        [HideInInspector]
        public MyRoomRuKouDlg pMyRoomRuKouDlg;
        /// <summary>
        /// 海选入口UI面板控制对象.
        /// </summary>
        [HideInInspector]
        public HaiXuanRuKouDlg pHaiXuanRuKouDlg;
        /// <summary>
        /// 淘汰赛入口对话框.
        /// </summary>
        [HideInInspector]
        public TaoTaiSaiRuKouDlg pTaoTaiSaiRuKouDlg;
        /// <summary>
        /// 总决赛入口对话框.
        /// </summary>
        [HideInInspector]
        public ZongJueSaiRuKouDlg pZongJueSaiRuKouDlg;
        /// <summary>
        /// 是否跳过该界面.
        /// </summary>
        [HideInInspector]
        public bool IsTiaoGuoThisPanel = true;
        bool IsOpenOfficialRoom = true;
        void Awake()
        {
            IsOpenOfficialRoom = true; //官方比赛(红包赛).
            IsOpenSheQuMaJiangSai = false; //社区麻将赛开关.
            IsTiaoGuoThisPanel = false; //是否跳过本界面开关.
            MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView = this;
            if (IsTiaoGuoThisPanel)
            {
                foreach (var item in HiddenObjArray)
                {
                    item.SetActive(false);
                }
            }
        }

        void Start()
        {
            if (MainRoot._gPlayerData != null && MainRoot._gPlayerData.IsOpenQinRenBiSaiRuKouDlg)
            {
                MainRoot._gPlayerData.IsOpenQinRenBiSaiRuKouDlg = false;
                OnClickLMTBt(); //秦人比赛开始前退出游戏后直接进入秦人比赛入口面板.
            }

            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.IsOpenBiSaiSelectUIView)
            {
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.IsOpenBiSaiSelectUIView = false;
                //Invoke("ClickCloseButton", 0.5f);
            }
            else
            {
                CheckIsTiaoGuoThisPanel(false);
            }
        }

        public override void OnEnter(BaseContext context)
        {
            b_isClick = false;
            base.OnEnter(context);
        }

        public override void OnExit(BaseContext context)
        {
            base.OnExit(context);
        }

        public override void OnPause(BaseContext context)
        {
            _animator.SetTrigger("OnExit");
        }
        
        public void ClickCloseButton()
        {
            if (!b_isClick)
            {
                b_isClick = true;
                Singleton<ContextManager>.Instance.Pop();
                if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ResetMainUIDt();
                }
            }
        }

        /// <summary>
        /// 点击推广按键.
        /// </summary>
        public void OnClickTuiGuangBt()
        {
            Debug.Log("Unity: OnClickTuiGuangBt...");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnGameTuiGuangDlg();
        }
        /// <summary>
        /// 点击引导按键.
        /// </summary>
        public void OnClickYinDaoBt()
        {
            Debug.Log("Unity: OnClickYinDaoBt!");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnGameYinDaoDlg();
        }
        /// <summary>
        /// 点击分享按键.
        /// </summary>
        public void OnClickFenXiangBt()
        {
            Debug.Log("Unity: OnClickFenXiangBt!");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnGameFenXiangDlg();
        }
        public void OnClickGameShopBt()
        {
            if (RoomCardNet.RoomCardNetClientModule.netModule == null)
            {
                Debug.Log("Unity:" + "Please login!");
                return;
            }
            Debug.Log("Unity:" + "OnClickGameShopBt...");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.CARD_PANEL, transform.parent);
        }
        /// <summary>
        /// 主界面邀请微信好友
        /// </summary>
        public void OnInviteFriendBtnClick()
        {
            if (RoomCardNet.RoomCardNetClientModule.netModule == null)
            {
                Debug.Log("Unity:" + "Please login!");
                return;
            }
            if (MainRoot._gWeChat_Module == null)
            {
                Debug.Log("Unity:" + "_gWeChat_Module is null!");
                return;
            }
            MainRoot._gWeChat_Module.ShareInfoToWeChat();
        }
        public void OnClickZhanJiBt()
        {
            Debug.Log("Unity:" + "OnClickZhanJiBt");
            //遮挡主界面，防止多次点击
            //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ShowOneSysMsgText(47, MainRoot._gUIModule.pUnModalUIControl.pMainUIScript);
            MainRoot._gUIModule.pUnModalUIControl.SpawnPlayerZhanJiPanel();
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerGetRecordList(0);
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerGetRecordList(1);
        }
        /// <summary>
        /// 点击消息按键.
        /// </summary>
        public void OnClickMsgBt()
        {
            Debug.Log("Unity: OnClickMsgBt!");
            MainRoot._gUIModule.pUnModalUIControl.SpawnGameMsgDlg();
        }
        /// <summary>
        /// 点击规则按键.
        /// </summary>
        public void OnClickGuiZeBt()
        {
            Debug.Log("Unity: OnClickGuiZeBt!");
            MainRoot._gUIModule.pUnModalUIControl.SpawnGameHelpDlg();
        }
        /// <summary>
        /// 点击设置按键.
        /// </summary>
        public void OnClickGameSetBt()
        {
            Debug.Log("Unity: OnClickGameSetBt!");
            MainRoot._gUIModule.pUnModalUIControl.SpawnGameSetPanel();
        }

        /// <summary>
        /// 点击LMT(秦人比赛场/紅包赛)按键.
        /// </summary>
        public void OnClickLMTBt()
        {
            Debug.Log("Unity: OnClickLMTBt!");
            if (!IsOpenOfficialRoom)
            {
                return;
            }
            SpawnQinRenBiSaiRuKouDlg();
        }
        
        /// <summary>
        /// 点击自由赛(好友比赛场/社区麻将赛)按键.
        /// </summary>
        public void OnClickZiYouSaiBt()
        {
            Debug.Log("Unity: OnClickZiYouSaiBt!");
            ShowSheQuMaJiangSaiPanel();
            //SpawnZiJianBiSaiRuKouDlg();
        }

        /// <summary>
        /// 点击主题赛按键.
        /// </summary>
        public void OnClickZhuTiSaiBt()
        {
            Debug.Log("Unity: OnClickZhuTiSaiBt!");
            //这里添加向服务端发送主题赛按键被点击的消息.
            switch (eMultiRaceType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_NULL:
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace;
                        if (RoomCardNet.RoomCardNetClientModule.netModule != null
                            && RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                        {
                            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallBiSaiRoomZhuTiSaiBt(eMultiRaceType);
                        }
                        break;
                    }
            }
            //OnReceivedZhuTiSaiBtMsg(0); //test.
        }

        /// <summary>
        /// 接受到服务端返回主题赛按键的信息.
        /// biSaiType: 0 比赛时间未到, 1 海选赛, 2 淘汰赛, 3 总决赛.
        /// biSaiState: 0 比赛时间未到, 1 海选赛开始, 2 小组赛准备, 3 小组赛开始.
        /// biSaiChangCi: -1/0 第一场, 1 第二场, 2 第三场, 3 第四场.
        /// </summary>
        public void OnReceivedZhuTiSaiBtMsg(int biSaiType, int biSaiState, int biSaiChangCi)
        {
            //biSaiType = 0; //test
            //biSaiState = 2; //test
            switch (biSaiType)
            {
                case 1:
                    {
                        SpawnHaiXuanRuKouDlg(biSaiChangCi, eMultiRaceType);
                        break;
                    }
                case 2:
                    {
                        SpawnTaoTaiSaiRuKouDlg(biSaiState, biSaiChangCi, eMultiRaceType);
                        break;
                    }
                case 3:
                    {
                        SpawnZongJueSaiRuKouDlg(biSaiState, biSaiChangCi, eMultiRaceType);
                        break;
                    }
                default:
                    {
                        //比赛时间未到.
                        Debug.Log("Unity: OnReceivedZhuTiSaiBtMsg -> biSaiType " + biSaiType);
                        MainRoot._gUIModule.pUnModalUIControl.pBiSaiSelectUIView.CheckIsTiaoGuoThisPanel(true);
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiWeiKaiShi();
                        break;
                    }
            }

            if (MainRoot._gPlayerData.relinkUserRoomData != null)
            {//说明玩家有未完成的牌局
                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnLoginPaiJuWeiWanChengDlg();//点击请求重新连入牌局
            }
            eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_NULL;
        }

        /// <summary>
        /// 产生秦人比赛入口面板.
        /// </summary>
        public void SpawnQinRenBiSaiRuKouDlg()
        {
            if (pQinRenBiSaiCtrl != null)
            {
                return;
            }

            //这里向服务端发送请求亲人比赛场数据的消息.
            if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
            {
                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallOfficialRoomListInfo();
            }

            Transform trParent = MainRoot._gUIModule.pMainCanvas.transform;
            GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/QinRenBiSaiChang/ED-QinRenBiSaiChang"), trParent, false);
            EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
            dlg.Initial(EnsureDlg.EnsureKind.QinRenBiSaiChangDlg);
            pQinRenBiSaiCtrl = obj.GetComponent<QinRenBiSaiCtrl>();
            pQinRenBiSaiChangDlg = pQinRenBiSaiCtrl.pQinRenBiSaiChangDlg;
            pZuanShiSaiQinRenBiSaiChangDlg = pQinRenBiSaiCtrl.pZuanShiSaiQinRenBiSaiChangDlg;
        }

        /// <summary>
        /// 产生自建比赛入口面板.
        /// </summary>
        public void SpawnZiJianBiSaiRuKouDlg()
        {
            Transform trParent = null;
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
            {
                trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            }
            if (trParent == null)
            {
                Debug.LogWarning("Unity: SpawnHaiXuanRuKouDlg -> trParent is null!");
                return;
            }
            if (pMyRoomRuKouDlg != null)
            {
                return;
            }

            //这里向服务端发送请求自建比赛房间数据的消息.

            GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/MyRoomRuKou/ED-MyRoomRuKou"), trParent, false);
            EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
            dlg.Initial(EnsureDlg.EnsureKind.MyRoomRuKou);
            pMyRoomRuKouDlg = obj.GetComponent<MyRoomRuKouDlg>();
            pMyRoomRuKouDlg.ShowZiJianFangListInfo(); //test.
        }

        /// <summary>
        /// 产生海选入口面板.
        /// </summary>
        public void SpawnHaiXuanRuKouDlg(int biSaiChangCi, OneRoomData.DataDefine_MultiRaceType multiRaceType)
        {
            Transform trParent = null;
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
            {
                trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            }
            if (trParent == null)
            {
                Debug.LogWarning("Unity: SpawnHaiXuanRuKouDlg -> trParent is null!");
                return;
            }
            if (pHaiXuanRuKouDlg != null)
            {
                return;
            }

            //这里向服务端发送请求海选排行榜数据的消息.
            if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
            {
                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.GetHaiXuanChartsList(multiRaceType);
            }

            string biSaiRuKouPrefab = "";
            switch (multiRaceType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        biSaiRuKouPrefab = "Prefab/HaiXuanRuKou/ED-HaiXuanRuKou-SheQuSai";
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        biSaiRuKouPrefab = "Prefab/HaiXuanRuKou/ED-HaiXuanRuKou";
                        break;
                    }
            }
            GameObject obj = (GameObject)Instantiate(Resources.Load(biSaiRuKouPrefab), trParent, false);
            EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
            dlg.Initial(EnsureDlg.EnsureKind.HaiXuanRuKouDlg);
            pHaiXuanRuKouDlg = obj.GetComponent<HaiXuanRuKouDlg>();
            pHaiXuanRuKouDlg.Init(biSaiChangCi, multiRaceType);
        }

        /// <summary>
        /// 产生淘汰赛入口面板.
        /// </summary>
        public void SpawnTaoTaiSaiRuKouDlg(int biSaiState, int changCi, OneRoomData.DataDefine_MultiRaceType biSaiType)
        {
            Transform trParent = null;
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
            {
                trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            }
            if (trParent == null)
            {
                Debug.LogWarning("Unity: SpawnTaoTaiSaiRuKouDlg -> trParent is null!");
                return;
            }
            if (pTaoTaiSaiRuKouDlg != null)
            {
                return;
            }

            string biSaiRuKouPrefab = "";
            switch (biSaiType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        biSaiRuKouPrefab = "Prefab/TaoTaiSaiRuKou/ED-TaoTaiSaiRuKou-SheQuSai";
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        biSaiRuKouPrefab = "Prefab/TaoTaiSaiRuKou/ED-TaoTaiSaiRuKou";
                        break;
                    }
            }
            GameObject obj = (GameObject)Instantiate(Resources.Load(biSaiRuKouPrefab),
                                transform.parent, false);
            EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
            dlg.Initial(EnsureDlg.EnsureKind.TaoTaiSaiRuKou);
            pTaoTaiSaiRuKouDlg = obj.GetComponent<TaoTaiSaiRuKouDlg>();
            pTaoTaiSaiRuKouDlg.InitTaoTaiSaiDlg(biSaiState, changCi, eMultiRaceType);
        }

        /// <summary>
        /// 产生总决赛入口面板.
        /// </summary>
        public void SpawnZongJueSaiRuKouDlg(int biSaiState, int biSaiChangCi, OneRoomData.DataDefine_MultiRaceType biSaiType)
        {
            Transform trParent = null;
            if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
            {
                trParent = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.transform.parent;
            }
            if (trParent == null)
            {
                Debug.LogWarning("Unity: SpawnZongJueSaiRuKouDlg -> trParent is null!");
                return;
            }
            if (pZongJueSaiRuKouDlg != null)
            {
                return;
            }
            
            string biSaiRuKouPrefab = "";
            switch (biSaiType)
            {
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                    {
                        biSaiRuKouPrefab = "Prefab/ZongJueSaiRuKou/ED-ZongJueSaiRuKou-SheQuSai";
                        break;
                    }
                case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                    {
                        biSaiRuKouPrefab = "Prefab/ZongJueSaiRuKou/ED-ZongJueSaiRuKou";
                        break;
                    }
            }
            GameObject obj = (GameObject)Instantiate(Resources.Load(biSaiRuKouPrefab),
                                transform.parent, false);
            EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
            dlg.Initial(EnsureDlg.EnsureKind.ZongJueSaiRuKou);
            pZongJueSaiRuKouDlg = obj.GetComponent<ZongJueSaiRuKouDlg>();
            pZongJueSaiRuKouDlg.InitZongJueSaiDlg(biSaiState, biSaiChangCi, eMultiRaceType);
        }

        OneRoomData.RoomType RoomType = OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan;
        /// <summary>
        /// 检测是否跳过该面板.
        /// </summary>
        public void CheckIsTiaoGuoThisPanel(bool isClickBackBt)
        {
            //RoomType = OneRoomData.RoomType.RoomType_OfficialRoom;
            if (IsTiaoGuoThisPanel)
            {
                if (isClickBackBt)
                {
                    //ClickCloseButton();
                }
                else
                {
                    switch (RoomType)
                    {
                        case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
                        case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                            {
                                OnClickZhuTiSaiBt();
                                break;
                            }
                        case OneRoomData.RoomType.RoomType_OfficialRoom:
                            {
                                OnClickLMTBt();
                                break;
                            }
                    }
                }
            }
        }

        /// <summary>
        /// 显示社区麻将赛面板.
        /// </summary>
        void ShowSheQuMaJiangSaiPanel()
        {
            if (IsOpenSheQuMaJiangSai)
            {
                switch (eMultiRaceType)
                {
                    case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_NULL:
                    case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                        {
                            eMultiRaceType = OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1;
                            if (RoomCardNet.RoomCardNetClientModule.netModule != null
                                && RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                            {
                                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallBiSaiRoomZhuTiSaiBt(eMultiRaceType);
                            }
                            break;
                        }
                }
            }
        }
    }
}