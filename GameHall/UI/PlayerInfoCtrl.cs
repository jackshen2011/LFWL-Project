using UnityEngine;
using System.Collections;

/// <summary>
/// GameHall场景里玩家的头像,昵称等信息管理.
/// </summary>
class PlayerInfoCtrl : MonoBehaviour
{
    enum BtEnum
    {
        CoinBt,
        CardBt,
        PlayerHeadBt,
    }
    public TextBase PlayerNameTx;
    public TextBase CoinNumTx;
    /// <summary>
    /// 钻石/房卡数量文本.
    /// </summary>
    public TextBase CardNumTx;
    public ImageBase PlayerHeadImg;
    public Transform ParentTr;

    void Start()
    {
        if (MainRoot._gPlayerData != null && MainRoot._gPlayerData.pAsyncImageDownload != null) {
            SetPlayerInfo(MainRoot._gPlayerData);
        }
    }

    public void OnClickBt(int indexBt)
    {
        BtEnum btSt = (BtEnum)indexBt;
        switch (btSt) {
            case BtEnum.CoinBt:
                Debug.Log("Unity:"+"Show coin shop!");
                //if (SystemSetManage.AuditVersion.IsIOSAudit != true)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.COIN_PANEL, ParentTr);
                }
                break;
            case BtEnum.CardBt:
                Debug.Log("Unity:"+"Show card room shop!");
                //if (SystemSetManage.AuditVersion.IsIOSAudit != true)
                {
                    //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.CARD_PANEL, ParentTr);
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShopPanelObj(ShopPanelCtrl.ShopPanelEnum.DIAMOND_PANEL, ParentTr);
                }
                break;
            case BtEnum.PlayerHeadBt:
                Debug.Log("Unity:"+"Show player id and ip!");
                SpawnPlayerDtInfoPanel();
                break;
        }
    }
    /// <summary>
    /// 设置GameHall面板的玩家信息.
    /// </summary>
    public void SetPlayerInfo(PlayerData pData)
    {
        //Debug.Log("Unity:"+"SetPlayerInfo...");
        string playerName = pData.sUserName;
        PlayerNameTx.text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        CoinNumTx.text = UserInfoPanel.GetPlayerCoinValInfo(pData.nCoinNum);
        CardNumTx.text = pData.GemCount.ToString();
        MainRoot._gPlayerData.pAsyncImageDownload.SetAsyncImage(AsyncImageDownload.PlayerSeats.SELF_PLAYER,
            pData.nUserId,
            (PlayerData.PlayerSexEnum)pData.nSex,
            pData.sHeadImgUrl,
            PlayerHeadImg);
    }
    /// <summary>
    /// 产生玩家详细信息有ID,IP.
    /// </summary>
    void SpawnPlayerDtInfoPanel()
    {
        Transform parentTr = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.gameObject.transform.parent;
        //if (MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView != null
        //    && !MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.b_isClick) {
        //    parentTr = MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.pQinRenSelectUIView.gameObject.transform;
        //}
        GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/PlayerDtInfoCtrl"), parentTr, false);
        PlayerDtInfoCtrl temp = test.GetComponent<PlayerDtInfoCtrl>();
        temp.SetPlayerDtInfo(PlayerHeadImg.sprite);
    }

    /// <summary>
    /// 设置商城产生时的父级.
    /// </summary>
    public void SetPlayerInfoParent(Transform tr)
    {
        ParentTr = tr;
    }
}