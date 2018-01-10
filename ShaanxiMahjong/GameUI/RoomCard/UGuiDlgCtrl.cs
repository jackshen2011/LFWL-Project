using UnityEngine;
using System.Collections;

class UGuiDlgCtrl : MonoBehaviour
{
    /**
     * PaiJuBuZai    -> 房间号的牌局不存在. 
     * PaiJuPlaying  -> 该房间正在对局中.
     */
    public enum DlgEnum
    {
        Null,
        PaiJuBuZai,
        PaiJuPlaying,
    }
    public DlgEnum DlgSt = DlgEnum.Null;
    /**
     * 玩家点击按键消息入口.
     * indexBt -> 按键编号.
     */
    public void OnClickBt(int indexBt)
    {
        switch (DlgSt) {
            case DlgEnum.PaiJuBuZai:
            case DlgEnum.PaiJuPlaying:
            //后续添加牌局对局中旁观者是否已满的逻辑!
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.IsActiveDlg = false;
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.OnClickBt("Reinput");
            OnDestoryDlg();
            break;
        }
    }
    void OnDestoryDlg()
    {
        Destroy(gameObject);
    }
}