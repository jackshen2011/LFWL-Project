using UnityEngine;
using System.Collections;

/// <summary>
/// 好友房卡选择的总控制脚本.
/// </summary>
class RoomCardCtrl : MonoBehaviour
{
    RoomCardNumCtrl RoomCardNumScript;
    public RoomCardListCtrl RoomCardListScript;
    /**
     * 是否激活消息对话框.
     * 如果有激活消息对话框,不在响应该面板的按键消息.
     */
    public bool IsActiveDlg;
    /**
     * btSt -> 确定玩家点击的是那个按键.
     * Num0 -> 数字0.
     * Num1 -> 数字1.
     * Num2 -> 数字2.
     * Num3 -> 数字3.
     * Num4 -> 数字4.
     * Num5 -> 数字5.
     * Num6 -> 数字6.
     * Num7 -> 数字7.
     * Num8 -> 数字8.
     * Num9 -> 数字9.
     * Reinput -> 重新输入.
     * Delete  -> 删除.
     * CloseBt -> 关闭房卡界面.
     */
    public void OnClickBt(string btSt)
    {
        if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.IsActiveDlg) {
            return;
        }
        string numStr = "";
        switch (btSt) {
        case "Num0":
        case "Num1":
        case "Num2":
        case "Num3":
        case "Num4":
        case "Num5":
        case "Num6":
        case "Num7":
        case "Num8":
        case "Num9":
                {
                    if (RoomCardNumScript != null)
                    {
                        numStr = btSt.Substring(3);
                        RoomCardNumScript.SetRoomCardNumInfo(System.Convert.ToByte(numStr));
                    }
                    break;
                }
        case "Reinput":
                {
                    if (RoomCardNumScript != null)
                    {
                        RoomCardNumScript.ResetRoomCardNum();
                    }
                    break;
                }
        case "Delete":
                {
                    if (RoomCardNumScript != null)
                    {
                        RoomCardNumScript.DeleteRoomCardNumInfo();
                    }
                    break;
                }
        case "CloseBt":
                {
                    OnClickCloseBt();
                    break;
                }
        }
    }
    void OnClickCloseBt()
    {
        Destroy(gameObject);
    }
    /// <summary>
    /// 初始化玩家好友房卡信息.
    /// </summary>
    public void InitRoomCardInfo(object[] args)
    {
        if (RoomCardListScript == null) {
            RoomCardListScript = GetComponent<RoomCardListCtrl>();
        }
        if (RoomCardNumScript == null) {
            RoomCardNumScript = GetComponent<RoomCardNumCtrl>();
        }
        RoomCardNumScript.ResetRoomCardNum();
        RoomCardListScript.InitRoomCardInfo(args);
    }

    /// <summary>
    /// 牌局不存在消息对话框.
    /// </summary>
    public void SpawnPaiJuBuZaiDlg(Transform tr = null)
    {
        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.IsActiveDlg = true;
        Transform trParent = tr != null ? tr : transform;
		GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/RoomCardUI/ED-PaiJuBuZaiDlg"), MainRoot._gUIModule.pMainCanvas.transform, false);
		EnsureDlg temp = test.GetComponent<EnsureDlg>();
		temp.Initial(EnsureDlg.EnsureKind.PaiJuBuZaiDlg);
	}
    /// <summary>
    /// 牌局正在对战消息对话框.
    /// </summary>
    public void SpawnPaiJuPlayingDlg(Transform tr = null)
    {
        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pRoomCardCtrl.IsActiveDlg = true;
        Transform trParent = tr != null ? tr : transform;
        Instantiate(Resources.Load("Prefab/RoomCardUI/ED-PaiJuPlayingDlg"), trParent, false);
    }
}