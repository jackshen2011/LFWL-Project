using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class IAPInterface : MonoBehaviour
{
    class ProductIdentifier
    {
        public string payID { get; set; }
        public string receipt { get; set; }
        public ProductIdentifier(string payID,string receipt)
        {
            this.payID = payID;
            this.receipt = receipt;
        }
    }
    public List<string> productInfo = new List<string>();
    List<ProductIdentifier> productIdentifier = new List<ProductIdentifier>();
    public static bool lockIAPVerify = false;

    [DllImport("__Internal")]
    private static extern void InitIAPManager();//初始化

    [DllImport("__Internal")]
    private static extern bool IsProductAvailable();//判断是否可以购买

    [DllImport("__Internal")]
    private static extern void RequstProductInfo(string s);//获取商品信息

    [DllImport("__Internal")]
    private static extern void BuyProduct(string s);//购买商品

    [DllImport("__Internal")]
    private static extern int EndTransaction(string s);//结束交易
    private bool bWaitResponse=false;

    void Update()
    {
        if (MainRoot._gPlayerData == null)
        {
            return;
        }
        if (RoomCardNet.RoomCardNetClientModule.netModule == null || RoomCardNet.RoomCardNetClientModule.netModule.isWanMainClientConnect == false)
        {
            return;
        }
        if (RoomCardNet.PlayerLinkClientPlayer.m_IsPlayerLogin == false)
        {
            return;
        }
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer == null)
        {//IAP
            return;
        }
        if (lockIAPVerify)
        {
            return;
        }
        if (productIdentifier.Count>0)
        {
            Debug.Log("发送验证");
            ProductIdentifier p = productIdentifier[0];
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallVerifyIAP(p.payID, p.receipt);
            lockIAPVerify = true;
            //productIdentifier.RemoveAt(0);
        }
    }
    public void DeleteProductIdentifier(string payID)
    {
        foreach (ProductIdentifier item in productIdentifier)
        {
            if (item.payID.Equals(payID))
            {
                productIdentifier.Remove(item);
                break;
            }
        }
    }
    public void Init()
    {
        InitIAPManager();
    }
    public bool isPaying()
    {
        return bWaitResponse;
    }
    //获取product列表
    void ShowProductList(string s)
    {
        productInfo.Add(s);
    }
    /// <summary>
    /// 发起iap购买
    /// </summary>
    /// <param name="payId">购买id</param>
    /// <param name="iStatus">拓展参数</param>
    /// <returns></returns>
    public void Pay(string payId,int iStatus)
    {
        if (bWaitResponse)
        {//请耐心等待
            Debug.Log(payId + " 请耐心等待");
            return;
        }
        if (IsProductAvailable())
        {
            BuyProduct(payId);
            bWaitResponse = true;
            if (MainRoot._gUIModule.pUnModalUIControl != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.SpawnSMShangChengGouMaiDengDai();
            }
        }
        else
        {
            Debug.Log(payId+"不可购买");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMZhangHaoNotZhiFu();
        }       
    }
    void ProvideContent(string s)
    {
        Debug.Log("[MsgFrom ios]proivideContent : " + s);
        bWaitResponse = false;
    }
    /// <summary>
    /// 购买完成ios的回调
    /// </summary>
    /// <param name="s"></param>
    void MsgPayInfo(string s)
    {
        if (MainRoot._gUIModule.pUnModalUIControl != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.DeleteSMShangChengGouMaiDengDai();
        }
        bWaitResponse = false;
        Hashtable list = MiniJsonExtensions.hashtableFromJson(s);
        string status = (string)list["status"];
        if ("1".Equals(status))
        {
            /*if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
            {//IAP
                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallVerifyIAP((string)list["payID"], (string)list["payResult"]);
            }*/
            productIdentifier.Add(new ProductIdentifier((string)list["payID"], (string)list["payResult"]));
            Debug.Log("添加验证成功" + (string)list["payID"]);
        }
        else if("2".Equals(status))
        {//取消
            Debug.Log("取消购买");
        }
        else if ("3".Equals(status))
        {//异常终止
            Debug.Log("异常终止");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMNotLinkiTunesStore();
        }
        else if ("4".Equals(status))
        {//请勿重复购买
            Debug.Log("请勿重复购买");
        }
    }
    public void endTransaction(string sId)
    {
        lockIAPVerify = false;
        DeleteProductIdentifier(sId);
        int error = EndTransaction(sId);
        Debug.Log("EndTransaction----------" + error);
    }
}
