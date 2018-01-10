using UnityEngine;
using System.Collections;
using MoleMole;
using CommonLibrary;

/// <summary>
/// 游戏商城大面板.
/// </summary>
class ShopPanelCtrl : MonoBehaviour
{
    /// <summary>
    /// 钻石数量文本.
    /// </summary>
    public TextBase DiamondNumTx;
    /// <summary>
    /// 金币数量文本.
    /// </summary>
    public TextBase CoinNumTx;
    /// <summary>
    /// 房卡数量文本.
    /// </summary>
    public TextBase CardNumTx;
    /// <summary>
    /// 钻石购买面板.
    /// </summary>
    public GameObject DiamondPanel;
    /// <summary>
    /// 金币购买面板.
    /// </summary>
    public GameObject CoinPanel;
    /// <summary>
    /// 房卡购买面板.
    /// </summary>
    public GameObject CardPanel;
    public ImageBase BtnDiamondImg;
    public ImageBase BtnCoinImg;
    public ImageBase BtnCardImg;
    /// <summary>
    /// COIN_PANEL -> 金币购买面板.
    /// CARD_PANEL -> 房卡购买面板.
    /// DIAMOND_PANEL -> 钻石购买面板.
    /// </summary>
    public enum ShopPanelEnum
    {
        COIN_PANEL,
        CARD_PANEL,
        DIAMOND_PANEL,
    }
    ShopPanelEnum SelectPanelSt = ShopPanelEnum.COIN_PANEL;
    /// <summary>
    /// 设置游戏商城金币,房卡数量信息.
    /// </summary>
    public void SetShopPanelInfo()
    {
        if (MainRoot._gPlayerData == null)
        {
            return;
        }
        CoinNumTx.text = UserInfoPanel.GetPlayerCoinValInfo(MainRoot._gPlayerData.nCoinNum);
        CardNumTx.text = MainRoot._gPlayerData.nFangKaCount.ToString();
        DiamondNumTx.text = MainRoot._gPlayerData.GemCount.ToString();
    }

    /// <summary>
    /// 显示商城对应面板.
    /// </summary>
    public void ShowShopPanel(ShopPanelEnum panelSt)
    {
        if (MainRoot._gUIModule.pUnModalUIControl.ShopDiamondState == EnsureDlg.EnsureKind.ShopDiamondCoinDlg
            || MainRoot._gUIModule.pUnModalUIControl.ShopDiamondState == EnsureDlg.EnsureKind.ShopDiamondCardDlg)
        {
        }
        else
        {
            MainRoot._gUIModule.pUnModalUIControl.ShopDiamondState = EnsureDlg.EnsureKind.ShopPanelDlg;
        }
        CoinPanel.SetActive(panelSt == ShopPanelEnum.COIN_PANEL ? true : false);
        CardPanel.SetActive(panelSt == ShopPanelEnum.CARD_PANEL ? true : false);
        DiamondPanel.SetActive(panelSt == ShopPanelEnum.DIAMOND_PANEL ? true : false);
        SelectPanelSt = panelSt;

        BtnCoinImg.color = new Color(1f, 1f, 1f, panelSt == ShopPanelEnum.COIN_PANEL ? 1f : 0f);
        BtnCardImg.color = new Color(1f, 1f, 1f, panelSt == ShopPanelEnum.CARD_PANEL ? 1f : 0f);
        BtnDiamondImg.color = new Color(1f, 1f, 1f, panelSt == ShopPanelEnum.DIAMOND_PANEL ? 1f : 0f);
    }

    /// <summary>
    /// 玩家点击购买商店道具(金币或房卡)按键事件.
    /// </summary>
    public void OnClickBuyPropBt(int indexProp)
    {
        int baseIndex = 0;
        //int payIndex = 0;
        Product proVal = null;
        switch (SelectPanelSt)
        {
            case ShopPanelEnum.COIN_PANEL:
                {
                    break;
                }
            case ShopPanelEnum.CARD_PANEL:
                {
                    baseIndex = (int)PayDefine.PAYGOOD.COIN18000000;
                    break;
                }
            case ShopPanelEnum.DIAMOND_PANEL:
                {
                    baseIndex = (int)PayDefine.PAYGOOD.THEMEENTERANCE;
                    break;
                }
        }
        mPayIndexVal = indexProp + baseIndex;
        //mPayIndexVal = payIndex;
        Debug.Log("Unity:" + "OnClickBuyPropBt -> SelectPanelSt " + SelectPanelSt + ", payIndex " + (PayDefine.PAYGOOD)mPayIndexVal);
        proVal = new Product((PayDefine.PAYGOOD)mPayIndexVal);

        //购买金币或房卡时判断玩家钻石数量是否足够.
        switch (SelectPanelSt)
        {
            case ShopPanelEnum.COIN_PANEL:
                {
                    if (proVal.Product_From > MainRoot._gPlayerData.GemCount)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgZuanShiBuZuMaiCoin();
                        return;
                    }
                    else
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDiamondToCoinPanel(proVal.Product_From, proVal.Product_To);
                    }
                    break;
                }
            case ShopPanelEnum.CARD_PANEL:
                {
                    if (proVal.Product_From > MainRoot._gPlayerData.GemCount)
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDlgZuanShiBuZuMaiCard();
                        return;
                    }
                    else
                    {
                        MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDiamondToCardPanel(proVal.Product_From, proVal.Product_To);
                    }
                    break;
                }
            case ShopPanelEnum.DIAMOND_PANEL:
                {
                    ShopSendMsgToServer();
                    break;
                }
        }
    }

    /// <summary>
    /// 支付道具的索引.
    /// </summary>
    int mPayIndexVal;
    /// <summary>
    /// 商城购买道具发送网络消息到服务端.
    /// </summary>
    public void ShopSendMsgToServer()
    {
        MainRoot._gUIModule.pUnModalUIControl.ShopSendMsgToServer(mPayIndexVal);
    }
}