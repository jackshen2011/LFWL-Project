namespace CommonLibrary
{
    public class PayDefine
    {
        //充值项
        public enum PAYGOOD
        {
            COIN10000 = 1,//20000金币
            COIN42000 = 2,//42000金币
            COIN120000 = 3,//120000金币
            COIN250000 = 4,//250000金币
            COIN1500000 = 5,//1500000金币
            COIN3200000 = 6,//3200000金币
            COIN10000000 = 7,//10000000金币
            COIN18000000 = 8,//18000000金币

            CARD2 = 9,//2张房卡
            CARD5 = 10,//5张房卡
            CARD10 = 11,//10张房卡
            CARD50 = 12,//50张房卡

            FIRSTCHARGE = 13,//首充
            THEMEENTERANCE = 14,//主题赛海选赛入场券

            JEWEL60 = 15,//60钻
            JEWEL190 = 16,//190钻
            JEWEL320 = 17,//320钻
            JEWEL720 = 18,//720钻
            JEWEL1400 = 19,//1400钻
            JEWEL1900 = 20,//1900钻
            JEWEL3400 = 21,//3400钻
            JEWEL7200 = 22,//7200钻
        }

        //充值渠道
        public enum PayChannel
        {
            PayChannel_Android_WeiXin = 0,  //安卓的微信充值
            PayChannel_IOS_Official = 1,    //IOS的官方充值
        }

        //是否是充值金币选项
        public static bool IsPayItem_Coin(int index)
        {
            if (index == (int)PAYGOOD.COIN10000 ||
                    index == (int)PAYGOOD.COIN42000 ||
                    index == (int)PAYGOOD.COIN120000 ||
                    index == (int)PAYGOOD.COIN250000 ||
                    index == (int)PAYGOOD.COIN1500000 ||
                    index == (int)PAYGOOD.COIN3200000 ||
                    index == (int)PAYGOOD.COIN10000000 ||
                    index == (int)PAYGOOD.COIN18000000)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPayItem_Card(int index)
        {
            if (index == (int)PAYGOOD.CARD2 ||
                    index == (int)PAYGOOD.CARD5 ||
                    index == (int)PAYGOOD.CARD10 ||
                    index == (int)PAYGOOD.CARD50 )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPayItem_Jewel(int index)
        {
            if (index == (int)PAYGOOD.JEWEL60 ||
                    index == (int)PAYGOOD.JEWEL190 ||
                    index == (int)PAYGOOD.JEWEL320 ||
                    index == (int)PAYGOOD.JEWEL720 ||
                    index == (int)PAYGOOD.JEWEL1400 ||
                    index == (int)PAYGOOD.JEWEL1900 ||
                    index == (int)PAYGOOD.JEWEL3400 ||
                    index == (int)PAYGOOD.JEWEL7200)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    //商城物品
    public class Product
    {
        public string name = "";
        public int Product_From = 0;
        public int Product_To = 0;

        //IOS 的收费项
        public static readonly string[] IAPGOOD = {"",
            "COIN10000", "COIN42000" , "COIN120000", "COIN250000", "COIN1500000",
            "COIN3200000", "COIN10000000", "COIN18000000",
            "CARD2","CARD5","CARD10","CARD50","FIRSTCHARGE","THEMEENTERANCE",
        "JEWEL60","JEWEL190","JEWEL320","JEWEL720","JEWEL1400","JEWEL1900","JEWEL3400","JEWEL7200"};
        //IOS 当前的消费项
        public string IOSPayItemStr = "";

        public enum Product_Type
        {
            Product_Null = 0,
            Product_Good = 1,
            Product_Card = 2,
            Product_Jewel = 3,
        }
        public Product_Type Producttype = Product_Type.Product_Null;

        //充值商品返回
        public enum Product_RetrunCode
        {
            //充值正常
            RetrunCode_Normal = 0,
            //充值错误码返回 钻石换金币 钻石不够   
            RetrunCode_Error_JewelToGood = 1,
            //充值错误码返回 钻石换房卡 钻石不够   
            RetrunCode_Error_JewelToCard = 2,
            //充值错误码返回 充值钻石异常  
            RetrunCode_Error_Jewel = 3,
            //充值错误码返回 充值钻石异常  
            RetrunCode_Error_Unknown = 4,  
        }

        public Product(PayDefine.PAYGOOD good)
        {

            switch (good)
            {
                case PayDefine.PAYGOOD.COIN10000:
                    name = "秦人麻将充值中心-购买2万金币";
                    Product_To = 20000;
                    Product_From = 10;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN42000:
                    name = "秦人麻将充值中心-购买4万2千金币";
                    Product_To = 42000;
                    Product_From = 20;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN120000:
                    name = "秦人麻将充值中心-购买12万金币";
                    Product_To = 120000;
                    Product_From = 50;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN250000:
                    name = "秦人麻将充值中心-购买25万金币";
                    Product_To = 250000;
                    Product_From = 100;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN1500000:
                    name = "秦人麻将充值中心-购买150万金币";
                    Product_To = 1500000;
                    Product_From = 500;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN3200000:
                    name = "秦人麻将充值中心-购买320万金币";
                    Product_To = 3200000;
                    Product_From = 1000;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN10000000:
                    name = "秦人麻将充值中心-购买1000万金币";
                    Product_To = 10000000;
                    Product_From = 3000;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.COIN18000000:
                    name = "秦人麻将充值中心-购买1800万金币";
                    Product_To = 18000000;
                    Product_From = 5000;
                    Producttype = Product_Type.Product_Good;
                    break;
                case PayDefine.PAYGOOD.CARD2:
                    name = "秦人麻将充值中心-购买2张房卡";
                    Product_To = 2;
                    Product_From = 20;
                    Producttype = Product_Type.Product_Card;
                    break;
                case PayDefine.PAYGOOD.CARD5:
                    name = "秦人麻将充值中心-购买5张房卡";
                    Product_To = 5;
                    Product_From = 50;
                    Producttype = Product_Type.Product_Card;
                    break;
                case PayDefine.PAYGOOD.CARD10:
                    name = "秦人麻将充值中心-购买10张房卡";
                    Product_To = 10;
                    Product_From = 100;
                    Producttype = Product_Type.Product_Card;
                    break;
                case PayDefine.PAYGOOD.CARD50:
                    name = "秦人麻将充值中心-购买50张房卡";
                    Product_To = 50;
                    Product_From = 500;
                    Producttype = Product_Type.Product_Card;
                    break;
                case PayDefine.PAYGOOD.FIRSTCHARGE://首充
                    name = "秦人麻将充值中心-首充活动";
                    Product_To = 0;
                    Product_From = 3500;
                    Producttype = Product_Type.Product_Null;
                    break;
                case PayDefine.PAYGOOD.THEMEENTERANCE://THEMEENTERANCE//主题赛海选赛入场券
                    name = "秦人麻将充值中心-海选入场券";
                    Product_To = 0;
                    Product_From = 200;
                    Producttype = Product_Type.Product_Null;
                    break;
                case PayDefine.PAYGOOD.JEWEL60:
                    name = "秦人麻将充值中心-购买60钻石";
                    Product_To = 60;
                    Product_From = 600;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[0];
                    break;
                case PayDefine.PAYGOOD.JEWEL190:
                    name = "秦人麻将充值中心-购买190钻石";
                    Product_To = 190;
                    Product_From = 1800;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[1];
                    break;
                case PayDefine.PAYGOOD.JEWEL320:
                    name = "秦人麻将充值中心-购买320钻石";
                    Product_To = 320;
                    Product_From = 3000;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[2];
                    break;
                case PayDefine.PAYGOOD.JEWEL720:
                    name = "秦人麻将充值中心-购买720钻石";
                    Product_To = 720;
                    Product_From = 6800;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[3];
                    break;
                case PayDefine.PAYGOOD.JEWEL1400:
                    name = "秦人麻将充值中心-购买1400钻石";
                    Product_To = 1400;
                    Product_From = 12800;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[4];
                    break;
                case PayDefine.PAYGOOD.JEWEL1900:
                    name = "秦人麻将充值中心-购买1900钻石";
                    Product_To = 1900;
                    Product_From = 16800;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[5];
                    break;
                case PayDefine.PAYGOOD.JEWEL3400:
                    name = "秦人麻将充值中心-购买3400钻石";
                    Product_To = 3400;
                    Product_From = 29800;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[6];
                    break;
                case PayDefine.PAYGOOD.JEWEL7200:
                    name = "秦人麻将充值中心-购买7200钻石";
                    Product_To = 7200;
                    Product_From = 61800;
                    Producttype = Product_Type.Product_Jewel;
                    IOSPayItemStr = IAPGOOD[7];
                    break;
                default:
                    name = "充值无效，慎重！";
                    Product_To = 0;
                    Product_From = 0;
                    Producttype = Product_Type.Product_Null;
                    return;
            }
        }
    }
}
