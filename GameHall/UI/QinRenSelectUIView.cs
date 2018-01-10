using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System;
using RoomCardNet;

/*
 *	
 *  
 *
 *	by GWB
 *
 */

namespace MoleMole
{
    class QinRenSelectUIContext : BaseContext
    {
        public QinRenSelectUIContext() : base(UIType.QinRenSelectUI)
        {
           // ViewType = UIType.MainUI;
        }
    }

    class QinRenSelectUIView : AnimateView,i_CallBack
    {
        public int m_Text;
        public bool b_isClick;
        /// <summary>
        /// GameHall场景里玩家的头像,昵称等信息管理.
        /// </summary>
        public PlayerInfoCtrl pPlayerInfoCtrl;
        /*[SerializeField]
        private Button _buttonHighScore;
        [SerializeField]
        private Button _buttonOption;
        */
        public QinRenSelectUIView()
        {
            b_isClick = false;
            //int ii = 1;
        }

        public void Start()
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView = this;
            pPlayerInfoCtrl.SetPlayerInfoParent(transform);
            m_Text = 1;
            CoinNumMin = new int[3];
            CoinNumMax = new int[3];
            CoinNumMin[0] = 2000;
            CoinNumMax[0] = 100000;
            CoinNumMin[1] = 20000;
            CoinNumMax[1] = 500000;
            CoinNumMin[2] = 50000;
            CoinNumMax[2] = 5000000;
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

        public override void OnResume(BaseContext context)
        {
            b_isClick = false;
            _animator.SetTrigger("OnEnter");
        }

        public void ClickCloseButton()
        {
            if (!b_isClick)
            {
                b_isClick = true;
                Singleton<ContextManager>.Instance.Pop();
                if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null) {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.ResetMainUIDt();
                }
            }
        }
		/// <summary>
		/// 点击秦人麻将馆的消息响应函数
		/// </summary>
        public void OnQinRenSelectLevelA()
        {
            CheckPlayerCoinNum(SelectRoomEnum.RoomLevel01);
            //Debug.Log("Unity:"+"OnQinRenSelectLevelA");
        }

        public void OnQinRenSelectLevelB()
        {
            CheckPlayerCoinNum(SelectRoomEnum.RoomLevel02);
            //Debug.Log("Unity:"+"OnQinRenSelectLevelB");
        }

        public void OnQinRenSelectLevelC()
        {
            CheckPlayerCoinNum(SelectRoomEnum.RoomLevel03);
            //Debug.Log("Unity:"+"OnQinRenSelectLevelC");
        }

        public void OnQinRenQuickStartBt()
        {
            CheckPlayerCoinNum(SelectRoomEnum.QuickStart);
            //Debug.Log("Unity:"+"OnQinRenSelect quickStartBt!");
        }
        public void OnClickZhanJiBt()
        {
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
        /// 点击帮助按键.
        /// </summary>
        public void OnClickGameHelpBt()
        {
            Debug.Log("Unity: OnClickGameHelpBt!");
            MainRoot._gUIModule.pUnModalUIControl.SpawnGameHelpDlg();
        }
        public void MakeSureCallBack()
        {
            Singleton<ContextManager>.Instance.Pop();

        }
        public void CancelCallBack()
        {
            Singleton<ContextManager>.Instance.Pop();

        }
        /**
         * 最小金币数量.
         */
        int[] CoinNumMin;
        /**
         * 最大金币数量.
         */
        int[] CoinNumMax;
        int PlayerCoinNum = 200;
        public enum SelectRoomEnum
        {
            QuickStart = -1,
            RoomLevel01,
            RoomLevel02,
            RoomLevel03,
        }
        /**
         * selectSt -> 房间等级.
         * selectSt == RoomLevel01 -> 初级房.
         * selectSt == RoomLevel02 -> 中级房.
         * selectSt == RoomLevel03 -> 高级房.
         * selectSt == QuickStart -> 快速匹配房间.
         * rv == 0 -> 允许玩家进入房间.
         * rv == 1 -> 在初级房里点击后,金币不足时检测玩家是否有未领取的福利金.
         * rv == 2 -> 当金币大于等于当前所选房间最大需求金币时,提示玩家选择是否进入更高级别的房间.
         * rv == 3 -> 弹出商城购买界面.
         */
        public byte CheckPlayerCoinNum(SelectRoomEnum selectSt)
        {
            byte rv = 0;
            if (MainRoot._gPlayerData != null) {
                CoinNumMin = MainRoot._gPlayerData.GetGameCoinNumMinAy();
                CoinNumMax = MainRoot._gPlayerData.GetGameCoinNumMaxAy();
                PlayerCoinNum = MainRoot._gPlayerData.nCoinNum;

                if (MainRoot._gPlayerData.relinkUserRoomData != null)
                {//说明玩家有未完成的牌局
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnLoginPaiJuWeiWanChengDlg();//点击请求重新连入牌局
                    return rv;
                }
            }
            Debug.Log("Unity:"+"CheckPlayerCoinNum -> CoinNum " + PlayerCoinNum);

            switch (selectSt) {
            case SelectRoomEnum.RoomLevel01:
                if (PlayerCoinNum < CoinNumMin[0]) {
                    //检测今天是否有福利金没有领取.
                    Debug.Log("Unity:"+"Check fuLiJin!");
                    rv = 1;
                    if (!MainRoot._gPlayerData.GetCanBringJiuJiCoin()) {
                        MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01);
                    }
                    else {
                        MainRoot._gUIModule.pUnModalUIControl.SpawnFuLiJinDlg();
                    }
                }
                if (PlayerCoinNum >= CoinNumMin[0] && PlayerCoinNum < CoinNumMax[0]) {
                        //进入初级房间.
                        MainRoot._gUIModule.pUnModalUIControl.MakePlayerGotoGoldRoom(0);
                }
                if (PlayerCoinNum >= CoinNumMax[0]) {
                        //弹出对话框,使玩家选择是否进入高一级别的房间.
                        rv = 2;
                        MainRoot._gUIModule.pUnModalUIControl.SpawnJinBiChaoChuDlg(SelectRoomEnum.RoomLevel01) ;
                }
                break;
            case  SelectRoomEnum.RoomLevel02:
                if (PlayerCoinNum < CoinNumMin[1]) {
                    //弹出商城购买界面.
                    Debug.Log("Unity:"+"Show shop!");
                    rv = 3;
                    MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin02);
                }
                if (PlayerCoinNum >= CoinNumMin[1] && PlayerCoinNum < CoinNumMax[1]) {
                        //进入中级房间.
                        MainRoot._gUIModule.pUnModalUIControl.MakePlayerGotoGoldRoom(1);
                }
                if (PlayerCoinNum >= CoinNumMax[1]) {
                        //弹出对话框,使玩家选择是否进入高一级别的房间.
                        rv = 2;
                        MainRoot._gUIModule.pUnModalUIControl.SpawnJinBiChaoChuDlg(SelectRoomEnum.RoomLevel02);
                }
                break;
            case  SelectRoomEnum.RoomLevel03:
                if (PlayerCoinNum < CoinNumMin[2]) {
                    //弹出商城购买界面.
                    Debug.Log("Unity:"+"Show shop!");
                    rv = 3;
                    MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin03);
                }
                if (PlayerCoinNum >= CoinNumMin[2]) {
                        //进入高级房间.
                        MainRoot._gUIModule.pUnModalUIControl.MakePlayerGotoGoldRoom(2);
                }
                break;
            case  SelectRoomEnum.QuickStart:
                if (PlayerCoinNum < CoinNumMin[0]) {
                    //检测今天是否有福利金没有领取.
                    //弹出商城购买界面.
                    if (CheckIsDisplayFuLiJin()) {
                        Debug.Log("Unity:"+"Show fuLiJin!");
                        rv = 1;
                        MainRoot._gUIModule.pUnModalUIControl.SpawnFuLiJinDlg();
                    }
                    else {
                        Debug.Log("Unity:"+"Show shop!");
                        rv = 3;
                        MainRoot._gUIModule.pUnModalUIControl.SpawnShopCoinDlg(ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01);
                    }
                }
                if (PlayerCoinNum >= CoinNumMax[1]) {
                        //进入高级房间.
                        MainRoot._gUIModule.pUnModalUIControl.MakePlayerGotoGoldRoom(2);
                }
                else {
                    if (PlayerCoinNum >= CoinNumMax[0]) {
                            //进入中级房间.
                            MainRoot._gUIModule.pUnModalUIControl.MakePlayerGotoGoldRoom(1);
                    }
                    else {
                        if (PlayerCoinNum >= CoinNumMin[0]) {
                            //进入初级房间.
                            MainRoot._gUIModule.pUnModalUIControl.MakePlayerGotoGoldRoom(0);
                        }
                    }
                }
                break;
            }
            return rv;
        }

        bool CheckIsDisplayFuLiJin()
        {
            if (!MainRoot._gPlayerData.GetCanBringJiuJiCoin()) {
                return false;
            }
            return true;
        }
    }
}
