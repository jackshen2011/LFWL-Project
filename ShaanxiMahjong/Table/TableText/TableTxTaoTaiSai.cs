using UnityEngine;

/// <summary>
/// 淘汰赛.
/// 华商淘汰赛.
/// 社区淘汰赛.
/// </summary>
class TableTxTaoTaiSai : MJGameTableTextCtrl
{
    /// <summary>
    /// 淘汰赛轮数.
    /// </summary>
    public MeshRenderer TaoTaiLunShuMesh;
    /// <summary>
    /// 淘汰赛底分.
    /// </summary>
    public MeshRenderer[] TaoTaiDiFenMesh;
    public override void RefreshShowRoomTabelInfo()
    {
        ShowTaoTaiSaiTabelInfo(MainRoot._gRoomData.cCurRoomData.nCurChangCi + 1, MainRoot._gRoomData.cCurRoomData.nDiFen);
    }

    /// <summary>
    /// 显示淘汰赛麻将桌信息.
    /// </summary>
    public void ShowTaoTaiSaiTabelInfo(int lunShu, int diFen)
    {
        float offsetPx = 0f;
        offsetPx = lunShu / 10f;
        TaoTaiLunShuMesh.materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        if (diFen <= 0)
        {
            diFen = 1;
        }

        for (int i = 0; i < TaoTaiDiFenMesh.Length; i++)
        {
            offsetPx = (diFen % 10) / 10f;
            diFen = diFen / 10;
            if (i == TaoTaiDiFenMesh.Length - 1 && offsetPx == 0f)
            {
                TaoTaiDiFenMesh[i].gameObject.SetActive(false);
                break;
            }
            TaoTaiDiFenMesh[i].materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        }
    }
}