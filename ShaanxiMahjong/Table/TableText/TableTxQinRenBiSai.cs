using UnityEngine;

/// <summary>
/// 秦人比赛场(红包赛).
/// </summary>
class TableTxQinRenBiSai : MJGameTableTextCtrl
{
    /// <summary>
    /// 秦人比赛场(红包赛)场次.
    /// </summary>
    public MeshRenderer[] ChangCiMaterial;
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
        ShowBiSaiChangCi();
    }

    /// <summary>
    /// 显示秦人比赛场(红包赛)场次.
    /// </summary>
    void ShowBiSaiChangCi()
    {
        int changCiVal = MainRoot._gRoomData.cCurRoomData.nOfficialRoomChangCi;
        float offsetPx = 0f;
        for (int i = 0; i < ChangCiMaterial.Length; i++)
        {
            offsetPx = (changCiVal % 10) / 10f;
            changCiVal = changCiVal / 10;
            ChangCiMaterial[i].materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        }
    }
}