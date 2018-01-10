using UnityEngine;

/// <summary>
/// 总决赛.
/// 华商总决赛.
/// 社区总决赛.
/// </summary>
class TableTxZongJueSai : MJGameTableTextCtrl
{
    /// <summary>
    /// 总决赛底分.
    /// </summary>
    public MeshRenderer[] ZongJueSaiDiFenMesh;
    public override void RefreshShowRoomTabelInfo()
    {
        ShowZongJueSaiTabelInfo(MainRoot._gRoomData.cCurRoomData.nDiFen);
    }

    /// <summary>
    /// 显示总决赛麻将桌信息.
    /// </summary>
    public void ShowZongJueSaiTabelInfo(int diFen)
    {
        float offsetPx = 0f;
        if (diFen <= 0)
        {
            diFen = 1;
        }

        for (int i = 0; i < ZongJueSaiDiFenMesh.Length; i++)
        {
            offsetPx = (diFen % 10) / 10f;
            diFen = diFen / 10;
            if (i == ZongJueSaiDiFenMesh.Length - 1 && offsetPx == 0f)
            {
                ZongJueSaiDiFenMesh[i].gameObject.SetActive(false);
                break;
            }
            ZongJueSaiDiFenMesh[i].materials[0].SetTextureOffset("_MainTex", new Vector2(offsetPx, 0f));
        }
    }
}