using UnityEngine;

/// <summary>
/// 红包赛游戏等待界面.
/// </summary>
class HongBaoSaiWaitPanel : MonoBehaviour
{
    /// <summary>
    /// 标题.
    /// </summary>
    public TextBase TitleTx;
    /// <summary>
    /// 倒计时管理组.
    /// </summary>
    public GameObject DaoJiShiGroup;
    /// <summary>
    /// 倒计时(需要服务端告知).
    /// </summary>
    public TextBase DaoJiShiTx;
    /// <summary>
    /// 倒计时(秒)
    /// </summary>
    int DaoJiShiVal = -1;
    float TimeLastDaoJiShi;
    public void Init()
    {
        TitleTx.text = MainRoot._gRoomData.cCurRoomData.OfficialRoomName;
        DaoJiShiGroup.SetActive(false);
    }

    void Update()
    {
        if (DaoJiShiVal <= 0)
        {
            return;
        }

        if (!DaoJiShiGroup.activeInHierarchy)
        {
            return;
        }

        if (Time.time - TimeLastDaoJiShi < 1f)
        {
            return;
        }
        TimeLastDaoJiShi = Time.time;
        DaoJiShiVal--;
        ShowBiSaiDaoJiShi();
    }

    public void SetHongBaoSaiWaitPanelDt(int daoJiShi)
    {
        TimeLastDaoJiShi = Time.time;
        DaoJiShiVal = daoJiShi;
        ShowBiSaiDaoJiShi();
        DaoJiShiGroup.SetActive(true);
    }

    public void RemoveThis()
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// 显示倒计时.
    /// </summary>
    void ShowBiSaiDaoJiShi()
    {
        int rv = DaoJiShiVal;
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