using UnityEngine;

/// <summary>
/// 朋友麻将房-房卡房.
/// </summary>
class TableTxPengYouMJG : MJGameTableTextCtrl
{
    /// <summary>
    /// 朋友麻将房id.
    /// </summary>
    public MeshRenderer[] PengYouMJGIdMaterial;
    public override void RefreshShowRoomTabelInfo()
    {
        ShowPengYouRoomId(MainRoot._gRoomData.cCurRoomData.nRoomId);
    }

    /// <summary>
    /// 显示朋友房的id.
    /// </summary>
    void ShowPengYouRoomId(int roomId)
    {
        float offsetPx = 0f;
        for (int i = 0; i < PengYouMJGIdMaterial.Length; i++)
        {
            offsetPx = (float)(roomId % 10) / 10f;
            roomId = roomId / 10;
            PengYouMJGIdMaterial[i].materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        }
    }
}