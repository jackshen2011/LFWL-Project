using UnityEngine;

class MJGameTableTextCtrl : MonoBehaviour
{
    GameObject mTableTextObj;
    /// <summary>
    /// 产生麻将桌面文本预制信息.
    /// </summary>
    public void SpawnMJTableText(Transform trParent)
    {
        if (mTableTextObj != null)
        {
            return;
        }

        string tableTextPrefab = "";
        switch (MainRoot._gRoomData.cCurRoomData.eRoomType)
        {
            case OneRoomData.RoomType.RoomType_Gold:
                {
                    tableTextPrefab = "Prefabs/TableText/QinRenMJGCtrl";
                    break;
                }
            case OneRoomData.RoomType.RoomType_RoomCard:
                {
                    tableTextPrefab = "Prefabs/TableText/PengYouMJGCtrl";
                    break;
                }
            case OneRoomData.RoomType.RoomType_ThemeRace_HaiXuan:
                {
                    tableTextPrefab = GetHaiXuanSaiPrefab();
                    break;
                }
            case OneRoomData.RoomType.RoomType_ThemeRace_Group:
                {
                    tableTextPrefab = GetZongJueSaiPrefab();
                    break;
                }
            case OneRoomData.RoomType.RoomType_MyRoom:
                {
                    tableTextPrefab = "Prefabs/TableText/ZiJianBiSaiMJZTxObj";
                    break;
                }
            case OneRoomData.RoomType.RoomType_OfficialRoom:
                {
                    tableTextPrefab = "Prefabs/TableText/QinRenBiSaiMJZTxObj";
                    break;
                }
        }

        if (tableTextPrefab == "")
        {
            Debug.LogWarning("Unity: SpawnMJTableText -> tableTextPrefab is wrong!");
            return;
        }
        mTableTextObj = (GameObject)Instantiate(Resources.Load(tableTextPrefab), trParent, false);
    }

    string GetHaiXuanSaiPrefab()
    {
        string tableTextPrefab = "";
        switch (MainRoot._gRoomData.cCurRoomData.eMultiRaceType)
        {
            case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                {
                    tableTextPrefab = "Prefabs/TableText/HaiXuanSaiMJZTxObj";
                    break;
                }
            case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                {
                    tableTextPrefab = "Prefabs/TableText/HaiXuanSaiMJZTxObj-SheQuSai";
                    break;
                }
        }
        return tableTextPrefab;
    }

    string GetZongJueSaiPrefab()
    {
        bool isZongJueSai = MainRoot._gRoomData.cCurRoomData.IsZongJueSaiRoom;
        string tableTextPrefab = "";
        switch (MainRoot._gRoomData.cCurRoomData.eMultiRaceType)
        {
            case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_ThemeRace:
                {
                    if (isZongJueSai)
                    {
                        tableTextPrefab = "Prefabs/TableText/ZongJueSaiMJZTxObj";
                    }
                    else
                    {
                        tableTextPrefab = "Prefabs/TableText/TaoTaiSaiMJZTxObj";
                    }
                    break;
                }
            case OneRoomData.DataDefine_MultiRaceType.MultiRaceType_Room1:
                {
                    if (isZongJueSai)
                    {
                        tableTextPrefab = "Prefabs/TableText/ZongJueSaiMJZTxObj-SheQuSai";
                    }
                    else
                    {
                        tableTextPrefab = "Prefabs/TableText/TaoTaiSaiMJZTxObj-SheQuSai";
                    }
                    break;
                }
        }
        return tableTextPrefab;
    }

    /// <summary>
    /// 刷新麻将桌面的文本.
    /// </summary>
    public virtual void RefreshShowRoomTabelInfo()
    {
        if (mTableTextObj != null)
        {
            mTableTextObj.SendMessage("RefreshShowRoomTabelInfo");
        }
    }
}