using UnityEngine;
using System.Collections;

class RoomCardDt : MonoBehaviour {
    /// <summary>
    /// RoomOwnerName 玩家名称.
    /// RoomNum 房号.
    /// PaiJuState 牌局状态.
    /// JuShu 局数信息.
    /// DiZhu 底注信息.
    /// </summary>

    public OneRoomData pOneRoomData;
    public TextBase PaiJuTx;
    public TextBase RoomNumTx;
    public TextBase RoomPlayerTx;
    public TextBase PaiJuStTx;
    public TextBase JuShuTx;
    public TextBase DiZhuTx;
    byte IndexRoomCard;
    /// <summary>
    /// 设置房卡索引.
    /// </summary>
    public void SetIndexRoomCard(byte val)
    {
        IndexRoomCard = val;
    }
    /// <summary>
    /// 设置玩家房卡数据信息.
    /// </summary>
    public void SetPlayerRoomCardData(int nroomid, OneRoomData.RoomStat stat,int ncurround,int nmaxround,int nroomownid,string sroomownname,int ndifen, OneRoomData.RoomType nroomtype,int ngoldroomtype)
    {
        pOneRoomData = new OneRoomData();
        pOneRoomData.SetRoomInfo(nroomid, stat, ncurround, nmaxround, nroomownid, sroomownname, ndifen, nroomtype, ngoldroomtype);
    }
    /**
     * 显示玩家饭卡信息.
     */
    public void ShowRoomCardDtInfo()
    {
        string paiJuSt = "";
        switch (pOneRoomData.eRoomState) {
            case OneRoomData.RoomStat.NULL:
				paiJuSt = "等待中";
				break;
            case OneRoomData.RoomStat.WAIT_JOIN:
				paiJuSt = "对局中";
				break;
			default:
				paiJuSt = "其他";
				break;
        }

        PaiJuTx.text = pOneRoomData.sRoomOwnerName + "的麻将馆";
        RoomNumTx.text = "房号: " + pOneRoomData.nRoomId;
        RoomPlayerTx.text = "房主: " + pOneRoomData.sRoomOwnerName;
        PaiJuStTx.text = paiJuSt;
        JuShuTx.text = "局数: " + pOneRoomData.nMaxRound;
        DiZhuTx.text = "底注: " + pOneRoomData.nDiFen;
        switch (paiJuSt) {
        case "等待中":
            PaiJuStTx.color = Color.green;
            break;
        default:
            PaiJuStTx.color = Color.red;
            break;
        }
    }
    public void OnClickRoomCardBt()
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.IsActiveDlg) {
            return;
        }
        Debug.Log("Unity:"+"IndexRoomCard " + IndexRoomCard);
        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.RoomCardListScript.OnClickBt(IndexRoomCard);
    }
}