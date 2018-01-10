using UnityEngine;

class HongBaoSaiWaitPanel : MonoBehaviour
{
    /// <summary>
    /// 标题.
    /// </summary>
    public TextBase TitleTx;
    /// <summary>
    /// 倒计时(需要服务端告知).
    /// </summary>
    public TextBase DaoJiShiTx;
    public void Init()
    {
        TitleTx.text = MainRoot._gRoomData.cCurRoomData.OfficialRoomName;
    }

    int DaoJiShiVal;
    /// <summary>
    /// 显示倒计时.
    /// </summary>
    void ShowBiSaiDaoJiShi(int val)
    {
        int rv = DaoJiShiVal - val;
        if (rv >= 60)
        {
            DaoJiShiTx.text = (rv / 60).ToString("d2") + ":" + (rv % 60).ToString("d2");
        }
        else if (rv >= 0)
        {
            DaoJiShiTx.text = "00:" + (rv % 60).ToString("d2");
        }
        else
        {
            DaoJiShiTx.text = "00:00";
        }
    }
}