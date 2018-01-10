using UnityEngine;
using System.Collections;

/// <summary>
/// 好友房卡选择界面的数字按键控制.
/// </summary>
class RoomCardNumCtrl : MonoBehaviour
{
    /**
     * 房卡数字数组.
     * NumTextAy[0] -> 个位.
     * NumTextAy[1] -> 十位.
     * NumTextAy[2] -> 百位.
     */
    public TextBase[] NumTextAy;
    /**
     * 房卡信息.
     */
    int RoomCardNumVal = 0;
    int IndexCardNum = 0;
    RoomCardListCtrl RoomCardList;
    public void ResetRoomCardNum()
    {
        for (int i = 0; i < NumTextAy.Length; i++) {
            NumTextAy[i].text = "";
        }
        IndexCardNum = 5;
        RoomCardNumVal = 0;
    }
    public void SetRoomCardNumInfo(byte val)
    {
        int index = IndexCardNum;
        if (val < 0 || val > 9) {
            //Debug.LogWarning("Unity:"+"SetRoomCardNumInfo -> val was error! val " + val);
            return;
        }
        if (index < 0 || index > 5) {
            //Debug.LogWarning("Unity:"+"SetRoomCardNumInfo -> index was error! index " + index);
            return;
        }
        RoomCardNumVal += (int)Mathf.Pow(10, index) * val;
        NumTextAy[index].text = val.ToString();
        IndexCardNum--;

        if (index == 0) {
            //房间号已经输入完成.
            //Debug.Log("Unity:"+"Room card num input over!");
            if (RoomCardList == null)
            {
                RoomCardList = GetComponent<RoomCardListCtrl>();
            }
            RoomCardList.CheckPlayerRoomCardState(RoomCardNumVal);
        }
    }
    public void DeleteRoomCardNumInfo()
    {
        int index = IndexCardNum;
        if (index >= 5) {
            //Debug.LogWarning("Unity:"+"DeleteRoomCardNumInfo -> index was wrong! index " + index);
            return;
        }

        IndexCardNum++;
        index = IndexCardNum;
        byte deleteNum = System.Convert.ToByte(NumTextAy[index].text);
        RoomCardNumVal -= (int)Mathf.Pow(10, index) * deleteNum;
        NumTextAy[index].text = "";
    }
}