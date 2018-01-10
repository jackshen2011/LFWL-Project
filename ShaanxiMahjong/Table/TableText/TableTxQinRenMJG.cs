using UnityEngine;

/// <summary>
/// 秦人麻将馆-金币房.
/// </summary>
class TableTxQinRenMJG : MJGameTableTextCtrl
{
    /// <summary>
    /// 秦人麻将馆房间等级.
    /// </summary>
    public MeshRenderer QinRenRoomLevelMeshRd;
    /// <summary>
    /// 秦人麻将馆房间等级材质.
    /// QinRenRoomLevelMaterial[0] -> 初级.
    /// QinRenRoomLevelMaterial[1] -> 中级.
    /// QinRenRoomLevelMaterial[2] -> 高级.
    /// </summary>
    public Material[] QinRenRoomLevelMaterial;
    public override void RefreshShowRoomTabelInfo()
    {
        QinRenRoomLevelMeshRd.material = QinRenRoomLevelMaterial[MainRoot._gRoomData.cCurRoomData.nGoldRoomLevel];
    }
}