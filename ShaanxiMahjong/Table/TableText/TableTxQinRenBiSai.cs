using UnityEngine;

/// <summary>
/// 秦人比赛场(红包赛).
/// </summary>
class TableTxQinRenBiSai : MJGameTableTextCtrl
{
    /// <summary>
    /// 秦人比赛15元红包赛
    /// </summary>
    public GameObject QinRenBiSaiTx15;
    /// <summary>
    /// 秦人比赛30元红包赛
    /// </summary>
    public GameObject QinRenBiSaiTx30;
    public override void RefreshShowRoomTabelInfo()
    {
        //QinRenBiSaiTx15.SetActive(true);
        //QinRenBiSaiTx30.SetActive(true);
    }
}