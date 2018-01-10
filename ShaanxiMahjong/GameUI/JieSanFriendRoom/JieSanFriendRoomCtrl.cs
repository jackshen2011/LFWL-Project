using UnityEngine;
using System.Collections;

class JieSanFriendRoomCtrl : UnModalUIBase
{
    /// <summary>
    /// 发出解散好友房间的信息.
    /// </summary>
    public TextBase FaQiJieSanInfo;
    /// <summary>
    /// 4个玩家的头像IMG.
    /// </summary>
    public ImageBase[] PlayerHeadImg;
    /// <summary>
    /// 4个玩家的选择状态IMG.
    /// </summary>
    public ImageBase[] PlayerSelectSt;
    /// <summary>
    /// SelectSprite[0] -> 选择中.
    /// SelectSprite[1] -> 同意.
    /// SelectSprite[2] -> 拒绝.
    /// </summary>
    public Sprite[] SelectSprite;
    /// <summary>
    /// 玩家选择之后需要隐藏的按键.
    /// </summary>
    public GameObject[] HiddenBt;
    /// <summary>
    /// 界面上的两个倒计时文本.
    /// </summary>
    public TextBase[] TimeDaoJiShi;
    /// <summary>
    /// "已同意"文本对象.
    /// </summary>
    public GameObject AgreeTextObj;
    string FaQiJSStr01 = "【";
    string FaQiJSStr02 = "】";
    float TimeLastVal;
    int DaoJiShiCount = 120;
    /// <summary>
    /// 发起解散的玩家昵称.
    /// </summary>
    public string FaQiJieSanPlayerName = "";
    /// <summary>
    /// SelectJieDuan -> 选择同意或拒绝阶段.
    /// WaitJieDuan -> 选择同意或拒绝之后的等待阶段.
    /// </summary>
    enum DaoJiShiEnum
    {
        SelectJieDuan,
        WaitJieDuan,
    }
    DaoJiShiEnum DaoJiShiSt = DaoJiShiEnum.SelectJieDuan;
    public enum BtEnum
    {
        Selecting,
        AgreeBt,
        RefuseBt,
    }
    public void InitJieSanRoom(object[] args)
    {
        int playerIdFaQi = (int)args[2]; //发起解散房间的玩家id.
        PlayerBase pBase = null;
        DaoJiShiCount = 120;
        AgreeTextObj.SetActive(false);
        SetTimeDaoJiShiInfo();
        if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
        {//若是二人麻将
            // 选择
            ImageBase temp = PlayerSelectSt[0];
            PlayerSelectSt[0] = PlayerSelectSt[1];
            PlayerSelectSt[1] = temp;

            PlayerSelectSt[1].gameObject.SetActive(false);
            PlayerSelectSt[3].gameObject.SetActive(false);

            //头像
            temp = PlayerHeadImg[0];
            PlayerHeadImg[0] = PlayerHeadImg[1];
            PlayerHeadImg[1] = temp;

            PlayerHeadImg[1].gameObject.SetActive(false);
            PlayerHeadImg[3].gameObject.SetActive(false);
        }
        else
        {

        }
        for (int i = 0; i < PlayerSelectSt.Length; i++)
        {
            pBase = MainRoot._gGameRoomCenter.gGameRoom.GetUserByServerSit(i);
            if (pBase != null)
            {
                if (pBase.nUserId != playerIdFaQi)
                {
                    SetPlayerSelectState(i, BtEnum.Selecting);
                }
                else
                {
                    SetPlayerSelectState(i, BtEnum.AgreeBt);
                }
                PlayerHeadImg[i].sprite = MainRoot._gPlayerData.pAsyncImageDownload.GetPlayerWXHeadImg(pBase.nUserId);
            }
        }
        FaQiJieSanInfo.text = FaQiJSStr01 + FaQiJieSanPlayerName + FaQiJSStr02;

        if (playerIdFaQi == MainRoot._gPlayerData.nUserId)
        {
            HiddenDlgBt(BtEnum.AgreeBt);
        }
    }
    public void UpdateJieSanFriendRoom(object[] args = null)
    {
        bool isBackGameHall = true;
        bool isDestoryPanel = false;
        PlayerBase pBase = null;
        for (int i = 0; i < 4; i++)
        {
            pBase = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId((int)(args[1 + i * 2]));
            if (pBase == null)
            {
                continue;
            }

            if ((int)(args[2 + i * 2]) != 1)
            {
                isBackGameHall = false;
            }
            if ((int)(args[2 + i * 2]) == 2)
            {
                isDestoryPanel = true;
            }
            PlayerSelectSt[i].sprite = SelectSprite[(int)(args[2 + i * 2])];
            PlayerHeadImg[i].sprite = MainRoot._gPlayerData.pAsyncImageDownload.GetPlayerWXHeadImg((int)(args[1 + i * 2]));
            //Debug.Log("Unity:"+pBase.sUserName + ": selectState " + (int)(args[2 + i * 2]) + ", pId " + (int)(args[1 + i * 2]));
        }

        if (isBackGameHall)
        {
            MainRoot._gRoomData.cCurRoomData = new OneRoomData();
            MainRoot._gMainRoot.ChangeScene("GameHall");
        }
        if (isDestoryPanel)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMJieSanShiBai();
            DestroyThis();
        }
    }
    void Update()
    {
        if (Time.time - TimeLastVal < 1f)
        {
            return;
        }
        if (DaoJiShiCount == 0) {
            return;
        }
        DaoJiShiCount--;
        SetTimeDaoJiShiInfo();

        //Debug.Log("DaoJiShiCount -> " + DaoJiShiCount);
        if (DaoJiShiCount == 0)
        {
            if (!AgreeTextObj.activeInHierarchy)
            {
                //倒计时结束默认为拒绝.
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMJieSanShiBai();
                if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                {
                    RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerAnsDissolutionCardRoom(MainRoot._gRoomData.cCurRoomData.nRoomId, false);
                }
            }
            Debug.LogWarning("Unity: TouPiaoJieSan time over!");
            DestroyThis();
        }
    }
    void SetTimeDaoJiShiInfo()
    {
        TimeLastVal = Time.time;
        switch (DaoJiShiSt)
        {
            case DaoJiShiEnum.SelectJieDuan:
                TimeDaoJiShi[0].text = DaoJiShiCount.ToString();
                break;
            case DaoJiShiEnum.WaitJieDuan:
                TimeDaoJiShi[1].text = DaoJiShiCount.ToString();
                break;
        }
    }
    /// <summary>
    /// 隐藏解散面板上的按键.
    /// </summary>

    public void HiddenDlgBt(BtEnum btSt, int indexVal = 0)
    {
        //根据玩家的id信息获取indexVal.
        SetPlayerSelectState(indexVal, btSt);
        if (DaoJiShiSt == DaoJiShiEnum.SelectJieDuan) {
            DaoJiShiSt = DaoJiShiEnum.WaitJieDuan;
        }
        for (int i = 0; i < HiddenBt.Length; i++) {
            HiddenBt[i].SetActive(false);
        }
        AgreeTextObj.SetActive(true);
    }
    /// <summary>
    /// 设置玩家选择解散房间的状态.
    /// </summary>
    public void SetPlayerSelectState(int indexVal, BtEnum btSt)
    {
        PlayerSelectSt[indexVal].sprite = SelectSprite[(byte)btSt];
    }
}