/// <summary>
/// 商城管理.
/// </summary>
class ShopManageCtrl
{
    /// <summary>
    /// 购买房卡的最少钻石数量
    /// </summary>
    int MinDiamondToCard = 50;

    /// <summary>
    /// 获取购买金币的最少钻石数量.
    /// </summary>
    public int GetMinDiamondToCoin(ShopCoinDlgCtrl.ShopCoinEnum eCoinEnum)
    {
        int diamond = 0;
        switch (eCoinEnum)
        {
            case ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin01:
                {
                    diamond = 50;
                    break;
                }
            case ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin02:
                {
                    diamond = 50;
                    break;
                }
            case ShopCoinDlgCtrl.ShopCoinEnum.RoomCoin03:
                {
                    diamond = 100;
                    break;
                }
        }
        return diamond;
    }

    /// <summary>
    /// 获取购买房卡的最少钻石数量.
    /// </summary>
    public int GetMinDiamondToCard()
    {
        return MinDiamondToCard;
    }
}