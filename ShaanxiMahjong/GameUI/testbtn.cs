using UnityEngine;
using System.Collections;

class testbtn : UnModalUIBase
{
    /**
     * JIE_SUAN_TEST_PT -> 普通结算界面测试.
     */
    public enum TestMaJiangGame
    {
        Null = -100,
        JIE_SUAN_TEST_PT = 27,
        GAO_JI_JIE_SUAN_TEST,
        GAME_SHOP_TEST,
        SHOP_CARD_TEST,
        CaptureUIScreenshot,
        SYS_MSG_OPEN,
        SYS_MSG_CLOSE,
    }
    public TestMaJiangGame MJSt = TestMaJiangGame.Null;
    public int btntype = -1;
    string showtext;

    GameObject temp;
    public TingPaiTishiUI tptishi;
    public TextBase t;
    public ChiPengHuTips cph;



    public void test_tishi(int length)
    {
        if (tptishi != null)
        {
            if (tptishi.isActiveAndEnabled)
            {
                tptishi.HideTingPaiTishi();
            }
            else
            {
                Vector3[] tparams = new Vector3[length];
                for (int i = 0; i < length; i++)
                {
                    tparams[i] = new Vector3(i+1, Mathf.Min(i * 2, 4), i % 9);
                }
                tptishi.ShowTingPaiTishi(tparams);

            }

        }
        else
        {
            temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/TingPaiTishiUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
            tptishi = temp.GetComponent<TingPaiTishiUI>();
            tptishi.Initial();
            test_tishi(btntype);
        }

        //MainRoot._gGameRoomCenter.gGameRoom.pMsgText.SetSystemMsgTextWithDestroy("请选择炮数！", 8.0f, temp, (UnModalUIBase)tp);
    }
    public void InitialTestBtn(string s,int n)
    {
         showtext = s;
        t.text = showtext;
        btntype = n;

    }

    public void chipenghushow(int btntype)
    {

        if (cph != null)
        {
            if (cph.isActiveAndEnabled)
            {
                cph.HideChiPengHuTips();
            }
            else
            {
                switch (btntype)
                {
                    case 10: //吃碰胡1
                        cph.ShowChiPengHuTips(false, false, false, false);
                        break;
                    case 11: //吃碰胡2
                        cph.ShowChiPengHuTips(false, false, false, true);
                        break;
                    case 12: //吃碰胡3
                        cph.ShowChiPengHuTips(false, false, true, true);
                        break;
                    case 13: //吃碰胡4
                        cph.ShowChiPengHuTips(false, true, true, true);
                        break;
                    case 14: //吃碰胡5
                        cph.ShowChiPengHuTips(true, true, true, true);
                        break;
                    default:
                        Debug.Log("Unity:"+"Errro type");
                        break;
                }


            }
        }
        else
        {
            temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/ChiPengHuTips"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
            cph = temp.GetComponent<ChiPengHuTips>();
            cph.Initial();
            switch (btntype)
            {
                case 10: //吃碰胡1
                    cph.ShowChiPengHuTips(false, false, false, false);
                    break;
                case 11: //吃碰胡2
                    cph.ShowChiPengHuTips(false, false, false, true);
                    break;
                case 12: //吃碰胡3
                    cph.ShowChiPengHuTips(false, false, true, true);
                    break;
                case 13: //吃碰胡4
                    cph.ShowChiPengHuTips(false, true, true, true);
                    break;
                case 14: //吃碰胡5
                    cph.ShowChiPengHuTips(true, true, true, true);
                    break;
                default:
                    Debug.Log("Unity:"+"Errro type");
                    break;
            }
        }
    }


    public void ClickBtnGoGoGo()
    {
        switch (btntype)
        {
            case 0: //下炮
                //test_xiapao();
                break;
            case 1: //提示
                test_tishi(btntype);
                break;
            case 2: //提示
                test_tishi(btntype);
                break;
            case 3: //提示
                test_tishi(btntype);
                break;
            case 4: //提示
                test_tishi(btntype);
                break;
            case 5: //提示
                test_tishi(btntype);
                break;
            case 6: //提示
                test_tishi(btntype);
                break;
            case 7: //提示
                test_tishi(btntype);
                break;
            case 8: //提示
                test_tishi(btntype);
                break;
            case 9: //提示
                test_tishi(btntype);
                break;
            case 10: //吃碰胡1
                chipenghushow(btntype);
                break;
            case 11: //吃碰胡2
                chipenghushow(btntype);
                break;
            case 12: //吃碰胡3
                chipenghushow(btntype);
                break;
            case 13: //吃碰胡4
                chipenghushow(btntype);
                break;
            case 14: //吃碰胡5
                chipenghushow(btntype);
                break;
            case 20: //登录
                Debug.Log("Unity:"+"WeChat Authorize Login!");
				MainRoot._gWeChat_Module.GetWeChatUserInfo();	//获取微信用户信息
				break;
            case 21: //创建房间
                Debug.LogError("Unity:"+"Need Net Call 3");
                MainRoot._gMainRoot.OnNetCallByServer(3,new object[1]);
                break;
            case 22: //匹配成功
                Debug.LogError("Unity:"+"Need Net Call 4");
                MainRoot._gMainRoot.OnNetCallByServer(0x50BFA852, new object[1]);
                break;
            case 23: //都下炮
                Debug.LogError("Unity:"+"Need Net Call 5");
                MainRoot._gMainRoot.OnNetCallByServer(5,new object[1]);
                break;
            case 24: //掷骰子
                Debug.LogError("Unity:"+"Need Net Call 6");
                MainRoot._gMainRoot.OnNetCallByServer(6,new object[1]);
                break;
            case 25: //自己接牌
                MainRoot._gMainRoot.OnNetCallByServer(7, new object[1] { 0});
                break;
            case 26: //流局
                Debug.LogError("Unity:"+"Need Net Call 7");
                MainRoot._gMainRoot.OnNetCallByServer(15, new object[1]);
                break;
            //打牌测试
            case 100:
                MainRoot._gMainRoot.OnNetCallByServer(7, new object[1] {1});
                break;
            case 101:
                MainRoot._gMainRoot.OnNetCallByServer(8, new object[1] { 1 });
                break;
            case 102:
                MainRoot._gMainRoot.OnNetCallByServer(9, new object[1] { 1 });
                break;
            case 103:
                MainRoot._gMainRoot.OnNetCallByServer(10, new object[1] { 1 });
                break;
            case 104:
                MainRoot._gMainRoot.OnNetCallByServer(7, new object[1] { 2 });
                break;
            case 105:
                MainRoot._gMainRoot.OnNetCallByServer(8, new object[1] { 2 });
                break;
            case 106:
                MainRoot._gMainRoot.OnNetCallByServer(9, new object[1] { 2 });
                break;
            case 107:
                MainRoot._gMainRoot.OnNetCallByServer(10, new object[1] { 2 });
                break;
            case 108:
                MainRoot._gMainRoot.OnNetCallByServer(7, new object[1] { 3 });
                break;
            case 109:
                MainRoot._gMainRoot.OnNetCallByServer(8, new object[1] { 3 });
                break;
            case 110:
                MainRoot._gMainRoot.OnNetCallByServer(9, new object[1] { 3 });
                break;
            case 111:
                MainRoot._gMainRoot.OnNetCallByServer(10, new object[1] { 3 });
                break;
            case 112:
                MainRoot._gMainRoot.OnNetCallByServer(11, new object[1] { 1 });
                break;
            case 113:
                MainRoot._gMainRoot.OnNetCallByServer(13, new object[1] { 1 });
                break;
            case 114:
                MainRoot._gMainRoot.OnNetCallByServer(11, new object[1] { 2 });
                break;
            case 115:
                MainRoot._gMainRoot.OnNetCallByServer(13, new object[1] { 2 });
                break;
            case 116:
                MainRoot._gMainRoot.OnNetCallByServer(11, new object[1] { 3 });
                break;
            case 117:
                MainRoot._gMainRoot.OnNetCallByServer(13, new object[1] { 3 });
                break;
            case 118:
                MainRoot._gMainRoot.OnNetCallByServer(14, 1);
                break;
            case 119:
                MainRoot._gMainRoot.OnNetCallByServer(14, 2);
                break;
            case 120:
                MainRoot._gMainRoot.OnNetCallByServer(14, 3);
                break;
			case 121: //微信登录
				Debug.Log("Unity:"+"WeChat GetWeChatUserInfo!");
				MainRoot._gWeChat_Module.GetWeChatUserInfo();   //获取微信用户信息
				break;
			case 122: //微信好友分享
				Debug.Log("Unity:"+"ShareInfoToWeChat!");
				MainRoot._gWeChat_Module.ShareInfoToWeChat();   //微信好友分享
				break;
			case 123: //微信朋友圈分享
				Debug.Log("Unity:"+"ShareInfoToWeChatMoments!");
				MainRoot._gWeChat_Module.ShareInfoToWeChatMoments();   //微信朋友圈分享
				break;
			case 124: //内部登录
                string str = "玩家1";
				Debug.Log("Unity:"+str + ":!!!!!!!!登录");
                if(RoomCardNet.RoomCardNetClientModule.netModule != null &&
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLogin(str, "test");
                }
				break;
			case 125: //内登1001
                string str1 = "玩家2";
                Debug.Log("Unity:"+str1 + ":!!!!!!!!登录");
                if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLogin(str1, "test");
                }
                break;
			case 126: //内登1002
                string str2 = "玩家3";
                Debug.Log("Unity:"+str2 + ":!!!!!!!!登录");
                if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLogin(str2, "test");
                }
                break;
			case 127: //内登1003
                string str3 = "玩家4";
                Debug.Log("Unity:"+str3 + ":!!!!!!!!登录");
                if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                     RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLogin(str3, "test");
                }
                break;
			case 128: //内登1004
                string str4 = "玩家5";
                Debug.Log("Unity:"+str4 + ":!!!!!!!!登录");
                if (RoomCardNet.RoomCardNetClientModule.netModule != null &&
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerLogin(str4, "test");
                }
                break;
			case 27: //截图分享
				Debug.Log("Unity:" + "截图分享");
				MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnGaoJiJieSuanObj();
				break;

			default:

                break;
        }
    }
	// Update is called once per frame
	void Update()
    {
	}
}
