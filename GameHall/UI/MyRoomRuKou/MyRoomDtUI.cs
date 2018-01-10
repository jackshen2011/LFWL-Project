using UnityEngine;
using System.Collections;

class MyRoomDtUI : ListUiDtCtrl
{
    /// <summary>
    /// 开始按键置灰.
    /// </summary>
    public GameObject StartGameBtZhiHui;
    /// <summary>
    /// 开始按键图集.
    /// StartGameBtSprite[0] 立即开始.
    /// StartGameBtSprite[1] 已开始.
    /// </summary>
    public Sprite[] StartGameBtSprite;
    /// <summary>
    /// args[x] 0 房间编号, 1 最大局数, 2 最多人数, 3 当前人数, 4 房间状态, 5 比赛是否已开始.
    /// UiText[x]: 0 房间编号, 1 房间信息, 2 房间人数信息, 3 房间状态.
    /// UiImg[x]: 0 开始按键img.
    /// </summary>
    public override void ShowListUiDt(object[] args)
    {
        Transform tiChuBtTr = StartGameBtZhiHui.transform.parent;
        //StartGameBtZhiHui.SetActive(false);
        StartGameBtZhiHui.transform.SetParent(transform);
        return;
        int roomId = (int)args[0];
        int maxJuShu = (int)args[1];
        int maxPlayer = (int)args[2];
        int curPlayer = (int)args[3];
        //roomState: 0 等待中, 1 比赛中.
        int roomState = (int)args[4];
        string roomTypeStr = maxPlayer + "人赛/" + maxJuShu + "局制";
        string roomStateStr = "";
        switch (roomState)
        {
            case 0:
                {
                    roomStateStr = "等待中";
                    break;
                }
            case 1:
                {
                    roomStateStr = "比赛中";
                    break;
                }
        }

        bool isYiKaiShiBiSai = (int)args[5] == 0 ? false : true;
        if (curPlayer >= maxPlayer && !isYiKaiShiBiSai)
        {
            StartGameBtZhiHui.SetActive(false);
        }
        UiImg[0].sprite = StartGameBtSprite[isYiKaiShiBiSai == false ? 0 : 1];
        UiText[0].text = roomId.ToString();
        UiText[1].text = roomTypeStr;
        UiText[2].text = curPlayer + "/" + maxPlayer;
        UiText[3].text = roomStateStr;
    }

    /// <summary>
    /// 点击查看按键.
    /// </summary>
    public void OnClickLookRoom()
    {
        Debug.Log("Unity: OnClickLookRoom");
        MainRoot._gUIModule.pUnModalUIControl.SpawnMyRoomPlayerPanelDlg();
    }

    /// <summary>
    /// 点击立即开始按键.
    /// </summary>
    public void OnClickStartGame()
    {
        Debug.Log("Unity: OnClickStartGame");
        //这里添玩家进入自建比赛房的服务端网络消息.

        MakePlayerIntoMyRoom();
    }
    
    /// <summary>
    /// 使玩家进入自建比赛游戏场景.
    /// </summary>
    public void MakePlayerIntoMyRoom()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShaanxiMahjong")
        {
            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_MyRoom;
            MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong"); //直接创建自建赛房间，进入游戏场景.
        }
    }
}