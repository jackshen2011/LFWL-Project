using UnityEngine;
using System.Collections;

class ShopCoinDlgCtrl : MonoBehaviour
{
    public enum ShopCoinEnum
    {
        RoomCoin01,
        RoomCoin02,
        RoomCoin03,
    }
    ShopCoinEnum ShopCoinState;
    /// <summary>
    /// 购买金币的背景图
    /// </summary>
    public ImageBase ShopCoinImg;
    /// <summary>
    /// ShopCoinSprite[0] -> 初级房购买.
    /// ShopCoinSprite[1] -> 中级房购买.
    /// ShopCoinSprite[2] -> 高级房购买.
    /// </summary>
    public Sprite[] ShopCoinSprite;
    /// <summary>
    /// 设置购买金币的背景图.
    /// </summary>
    public void SetShopCoinImg(ShopCoinEnum eShopCoin)
    {
        byte indexVal = (byte)eShopCoin;
        ShopCoinState = eShopCoin;
        ShopCoinImg.sprite = ShopCoinSprite[indexVal];
    }

    /// <summary>
    /// 获取购买道具的索引.
    /// </summary>
    public int GetPayGoodIndex()
    {
        CommonLibrary.PayDefine.PAYGOOD payGood = CommonLibrary.PayDefine.PAYGOOD.COIN120000;
        switch (ShopCoinState)
        {
            case ShopCoinEnum.RoomCoin01:
                {
                    payGood = CommonLibrary.PayDefine.PAYGOOD.COIN120000;
                    break;
                }
            case ShopCoinEnum.RoomCoin02:
                {
                    payGood = CommonLibrary.PayDefine.PAYGOOD.COIN120000;
                    break;
                }
            case ShopCoinEnum.RoomCoin03:
                {
                    payGood = CommonLibrary.PayDefine.PAYGOOD.COIN120000;
                    break;
                }
        }
        return (int)payGood;
    }
}