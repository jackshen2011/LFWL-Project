using UnityEngine;
using System.Collections;

class MyRoomPlayerDtUI : ListUiDtCtrl
{
    /// <summary>
    /// 踢出按键置灰对象.
    /// </summary>
    public GameObject ZhiHuiBtObj;
    /// <summary>
    /// args[x]: 0 玩家微信头像url, 1 玩家id, 2 玩家昵称, 3 踢出按键是否置灰, 4 是否隐藏踢出按键.
    /// UiText[x]: 0 玩家id, 1 玩家昵称.
    /// UiImg[x]: 0 微信头像.
    /// </summary>
    public override void ShowListUiDt(object[] args)
    {
        bool isHiddenTiChuBt = false;
        //isHiddenTiChuBt = (int)args[4] == 0 ? false : true;
        Transform tiChuBtTr = ZhiHuiBtObj.transform.parent;
        tiChuBtTr.gameObject.SetActive(!isHiddenTiChuBt);
        ZhiHuiBtObj.SetActive(false);
        if (!isHiddenTiChuBt)
        {
            ZhiHuiBtObj.transform.SetParent(transform);
        }
        return;

        int id = (int)args[1];
        string playerName = (string)args[2];
        MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage((string)args[0], UiImg[0]);
        UiText[0].text = id.ToString();
        UiText[1].text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        ZhiHuiBtObj.SetActive((int)args[3] == 0 ? false : true);
    }

    /// <summary>
    /// 点击踢出按键.
    /// </summary>
    public void OnClickTiChuBt()
    {
        Debug.Log("Unity: OnClickTiChuBt");
        //这里向服务端发送踢出某玩家的信息.

        MainRoot._gUIModule.pUnModalUIControl.pMyRoomPlayerPanelDlg.pVerListUI.RemoveObjTrFromPObjTrList(transform);
    }
}