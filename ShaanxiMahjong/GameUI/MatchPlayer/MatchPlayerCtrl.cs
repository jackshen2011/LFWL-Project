using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 游戏开始时玩家匹配动画控制脚本.
/// </summary>
class MatchPlayerCtrl : MonoBehaviour
{
    /**
     * 关闭匹配动画的延迟时间.
     */
    public float TimeVal = 2f;
    /**
     * 四个玩家的名称列表.
     */
    public TextBase[] PlayerNameTxAy;
    /**
     * 玩家匹配界面人物.
     */
    public Image[] PlayerImgAy;
    /**
     * 匹配界面男玩家图片资源.
     */
    public Sprite[] PlayerBoyTxAy;
    /**
     * 匹配界面女玩家图片资源.
     */
    public Sprite[] PlayerGirlTxAy;
    public Image BenJinImg;
    /**
     * 匹配界面本金资源.
     */
    public Sprite[] BenJinSp;
    float TimeLast;
    /**
     * 显示匹配动画.
     */
    public void ShowMatching()
    {
        if (gameObject.activeSelf) {
            return;
        }
        TimeLast = 0f;
        ShowPlayerInfo();
        gameObject.SetActive(true);
    }
    void Update()
    {
        TimeLast += Time.deltaTime;
        if (TimeLast >= TimeVal) {
            HiddenMatchAni();
        }
    }
    void HiddenMatchAni()
    {
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(29);//删除自己已准备
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(29+1);//删除已准备
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(29+2);//删除已准备
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(29+3);//删除已准备

		gameObject.SetActive(false);
		//接下来调出另一界面(玩家下炮界面).
		MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaPaoUI();//自己下炮
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(9, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(10, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(11, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮

        Destroy(gameObject);
    }

    void ShowPlayerInfo()
    {
        for (int i = 0; i < PlayerNameTxAy.Length; i++) {
            //获取玩家姓名.
            PlayerNameTxAy[i].text = PlayerNameAy[i];

            //获取玩家性别.
            if (PlayerSex[i] == PlayerData.PlayerSexEnum.BOY) {
                PlayerImgAy[i].sprite = PlayerBoyTxAy[PlayerIdArray[i] % PlayerBoyTxAy.Length];
            }
            else {
                PlayerImgAy[i].sprite = PlayerGirlTxAy[PlayerIdArray[i] % PlayerGirlTxAy.Length];
            }

            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan
                || MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group
                || MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_MyRoom
                || MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_OfficialRoom)
            {
                BenJinImg.gameObject.SetActive(false);
            }
            else
            {
                //设置匹配界面本金.
                BenJinImg.sprite = BenJinSp[BenJinState];
            }
        }
    }

    string[] PlayerNameAy;
    PlayerData.PlayerSexEnum[] PlayerSex;
    int[] PlayerIdArray;
    public void InitMatchPlayerInfo(object[] args = null)
    {
        gameObject.SetActive(false);
        PlayerNameAy = new string[4];
        PlayerSex = new PlayerData.PlayerSexEnum[4];
        PlayerIdArray = new int[4];

        PlayerBase playerBaseDt = null;
        int indexVal = 0;
        for (int i = 8; i < 32; i += 6)
        {
            playerBaseDt = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)(args[i + 1]));
            if (playerBaseDt != null)
            {
                PlayerNameAy[indexVal] = playerBaseDt.sUserName;
                PlayerSex[indexVal] = (PlayerData.PlayerSexEnum)playerBaseDt.nSex;
                PlayerIdArray[indexVal] = playerBaseDt.nUserId;
            }
            indexVal++;
        }
        BenJinState = (byte)MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel;

        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.LookPlayerListBtObj.SetActive(false);
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PaiMingInfoObj.SetActive(true);
                    break;
                }
            case OneRoomData.RoomType.RoomType_OfficialRoom:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.PaiMingInfoObj.SetActive(true);
                    break;
                }
        }
    }

    byte BenJinState;
    /**
     * 设置匹配界面本金信息.
     * val本金信息.
     * val == 0 -> 500金.
     * val == 1 -> 1500金.
     * val == 2 -> 3000金.
     */
    public void SetMatchPanelBenJinInfo(byte val)
    {
        if (val < 0 || val > 2) {
            Debug.LogError("Unity:"+"SetMatchPanelBenJinInfo -> val was wrong! val " + val);
            return;
        }
        BenJinState = val;
    }
}