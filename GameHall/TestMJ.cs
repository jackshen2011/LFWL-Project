using UnityEngine;
using System.Collections;

class TestMJ : MonoBehaviour
{
    /**
     * GamePaiHang 比赛模式时游戏排行榜.
     * BiSaiKaBuZuDlg 比赛卡不足.
     * HuaShangDaZhuanPan 海选大转盘.
     */
    public enum TestMaJiangGame
    {
        Null = -100,
        GamePaiHang,
        BiSaiKaBuZuDlg,
        HuaShangDaZhuanPan,
    }
    public TestMaJiangGame MJSt = TestMaJiangGame.Null;
    public int HuaShangZhuanPanIndex = 10;
    int TestIndex = 0;
    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        //激活为当前监听源
        //MusicPlayer.activeMyListener = true;
        //开始音乐播放
        //MusicPlayer.Stop(true);
        //MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Normal;
        //MusicPlayer.Play("YouXiZhong05", true);
    }

    // Update is called once per frame
    void Update ()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            //SoundEffectPlayer.Play("Wan01_M");
            switch (MJSt)
            {
                case TestMaJiangGame.GamePaiHang:
                    {
                        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView != null)
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnGamePaiHangDlg();
                        }
                        break;
                    }
                case TestMaJiangGame.BiSaiKaBuZuDlg:
                    {
                        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null)
                        {
                            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgBiSaiKaBuZu();
                        }
                        break;
                    }
                case TestMaJiangGame.HuaShangDaZhuanPan:
                    {
                        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript != null
                            && MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pHuaShangDaZhuanPan != null)
                        {
                            if (TestIndex % 2 == 0)
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pHuaShangDaZhuanPan.StartYunSuRunPoint();
                            }
                            else
                            {
                                MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pHuaShangDaZhuanPan.InitMovePoint(HuaShangZhuanPanIndex);
                            }
                            TestIndex++;
                        }
                        break;
                    }
                default:
                    {
                        TestIndex++;
                        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-ThemeRaceXiangQing"), MainRoot._gUIModule.pMainCanvas.transform, false);
                        EnsureDlg dlg = obj.GetComponent<EnsureDlg>();
                        dlg.Initial(EnsureDlg.EnsureKind.ThemeRaceXiangQing);
                        dlg.p_ShowTextA.text = MainRoot._gPlayerData.sThemeRace_HuoDongXiangQing;
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiYiKaiShi();
                        //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.DIAMOND_PANEL);
                        //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgHuaShangDaZhuanPan();
                        //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(1, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//匹配中.
                        //MainRoot._pMJGameTable.ShowHaiXuanSaiTabelInfo(1, 1);
                        //if (TestIndex % 2 == 0)
                        //{
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(75, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//比赛准备中.
                        //}
                        //else
                        //{
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(75);//删除比赛匹配中
                        //}

                        //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ClearXiaPaoZhong();
                        //if (TestIndex % 2 == 0)
                        //{
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaPaoUI();//自己下炮
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(9, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(10, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(11, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
                        //}
                        //else
                        //{
                        //    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ClearXiaPaoZhong();
                        //}
                        break;
                    }
            }
        }
        //if (Input.GetKeyUp(KeyCode.I))
        //{
        //    //SoundEffectPlayer.PlayStandalone("Wan01_M");
        //    MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.Wan01_M, true);
        //}
    }
}