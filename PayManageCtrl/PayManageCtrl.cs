using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//用于充值管理
public class PayManageCtrl
{
    public static void PlayerPayInterface(int goodsKind, int channel)
    {
#if !UNITY_EDITOR && UNITY_ANDROID
		PlayerPayInterface_Android(goodsKind, channel);
#elif UNITY_IPHONE
        PlayerPayInterface_IOS(goodsKind, channel);
#endif //UNITY_ANDROID
    }

    public static void PlayerPayInterface_Android(int goodsKind, int channel)
    {

    }

    public static void PlayerPayInterface_IOS(int goodsKind, int channel)
    {

    }
}
