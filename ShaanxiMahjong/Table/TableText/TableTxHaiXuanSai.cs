using UnityEngine;

/// <summary>
/// 海选赛.
/// 华商海选赛.
/// 社区海选赛.
/// </summary>
class TableTxHaiXuanSai : MJGameTableTextCtrl
{
    /// <summary>
    /// 海选赛轮数.
    /// </summary>
    public MeshRenderer HaiXuanLunShuMesh;
    /// <summary>
    /// 海选赛底分.
    /// </summary>
    public MeshRenderer[] HaiXuanDiFenMesh;
    public override void RefreshShowRoomTabelInfo()
    {
        ShowHaiXuanSaiTabelInfo(MainRoot._gRoomData.cCurRoomData.nCurChangCi + 1, MainRoot._gRoomData.cCurRoomData.nDiFen);
    }

    /// <summary>
    /// 显示海选赛麻将桌信息.
    /// </summary>
    public void ShowHaiXuanSaiTabelInfo(int lunShu, int diFen)
    {
        float offsetPx = 0f;
        offsetPx = lunShu / 10f;
        HaiXuanLunShuMesh.materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        if (diFen <= 0)
        {
            diFen = 1;
        }

        for (int i = 0; i < HaiXuanDiFenMesh.Length; i++)
        {
            offsetPx = (diFen % 10) / 10f;
            diFen = diFen / 10;
            if (i == HaiXuanDiFenMesh.Length - 1 && offsetPx == 0f)
            {
                HaiXuanDiFenMesh[i].gameObject.SetActive(false);
                break;
            }
            HaiXuanDiFenMesh[i].materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        }
    }
}