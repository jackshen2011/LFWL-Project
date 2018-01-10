using UnityEngine;
using System.Collections;

class CardZhanJiListUICtrl : MonoBehaviour
{
    /// <summary>
    /// 总对局.
    /// </summary>
    public TextBase ZongDuiJuTx;
    /// <summary>
    /// 最高连胜/积分.
    /// </summary>
    public TextBase LianShengTx;
    /// <summary>
    /// 胜率.
    /// </summary>
    public TextBase ShengLvTx;
    /// <summary>
    /// 暂无比赛.
    /// </summary>
    public GameObject WuBiSanObj;
    /**
     * 玩家战绩列表.
     * ZhanJiCardListPtr的父级的高度必须是元素间距的整数倍.
     */
    Transform[] ZhanJiTrAy;
    /**
     * 战绩列表父级.
     */
    public Transform ZhanJiCardListPtr;
    /// <summary>
    /// 战绩列表最大数量.
    /// </summary>
    int ZhanJiMax = 100;
    /// <summary>
    /// 玩家战绩的当前數量.
    /// </summary>
    int ZhanJiCount = 50;
    /// <summary>
    /// 战绩在可见框里显示的个数.
    /// </summary>
    public int CountShowZhanJi = 4;
    /**
     * 两个战绩元素的间隔距离.
     */
    public float OffsetPosY = 128f;
    /**
     * 创建战绩名片.
     */
    public void CreateZhanJiList(bool isTestZhanJi)
    {
        object[] args = MainRoot._gUIModule.pUnModalUIControl.CardRoomZhanJiArgs;
        bool isHaveBiSai = isTestZhanJi == true ? true : ((int)args[5] > 0 ? true : false);
        WuBiSanObj.SetActive(!isHaveBiSai);
        ZongDuiJuTx.gameObject.SetActive(isHaveBiSai);
        LianShengTx.gameObject.SetActive(isHaveBiSai);
        if (!isHaveBiSai)
        {
            return;
        }

        int zongDuiJuVal = isTestZhanJi == true ? 50 : (int)args[5];
        int lianShengVal = isTestZhanJi == true ? 10 : (int)args[4];
        //float shengLvVal = 0f;
        Vector3 offsetPos = Vector3.zero;
        float cardHeight = 0f;
        ZhanJiCount = isTestZhanJi == true ? 50 : (int)args[7];
        if (ZhanJiCount > ZhanJiMax)
        {
            ZhanJiCount = ZhanJiMax;
        }
        int roomCardLength = ZhanJiCount > CountShowZhanJi ? ZhanJiCount : CountShowZhanJi;
        for (int i = 0; i < roomCardLength; i++)
        {
            cardHeight += OffsetPosY;
        }
        Vector2 zhanJiListPtrSize = ((RectTransform)ZhanJiCardListPtr).sizeDelta;
        zhanJiListPtrSize.y = cardHeight;
        ((RectTransform)ZhanJiCardListPtr).sizeDelta = zhanJiListPtrSize;
        ZhanJiCardListPtr.localPosition = new Vector3(0f, -(cardHeight * 0.5f), 0f);

        ZongDuiJuTx.text = zongDuiJuVal.ToString();
        LianShengTx.text = lianShengVal.ToString();
        //ShengLvTx.text = shengLvVal.ToString() + "%";
        //ShengLvTx.gameObject.SetActive(false);
        ZhanJiTrAy = new Transform[ZhanJiCount];
        for (int i = 0; i < ZhanJiCount; i++)
        {
            GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/PlayerZhanJiPanel/PlayerCardZhanJiUI"),
                                                                ZhanJiCardListPtr, false);
            ZhanJiTrAy[i] = obj.transform;
            obj.name = "PlayerCardZhanJiUI" + i;
        }

        Vector3 startPos = new Vector3(0f, (cardHeight * 0.5f) - (((RectTransform)ZhanJiTrAy[0]).sizeDelta.y * 0.5f), 0f);
        CardRoomZhanJiUI zhanJiUI = null;
        for (int i = 0; i < ZhanJiCount; i++)
        {
            offsetPos.y = -i * OffsetPosY;
            ZhanJiTrAy[i].localPosition = startPos + offsetPos;

            zhanJiUI = ZhanJiTrAy[i].GetComponent<CardRoomZhanJiUI>();
            zhanJiUI.SetZhanJiIndexVal(i);
            if (!isTestZhanJi)
            {
                zhanJiUI.ShowZhanJiInfo(args[i + 8]); //显示玩家战绩信息.
            }
        }
    }
    /// <summary>
    /// 显示玩家分时战绩信息.
    /// </summary>
    public void ShowPlayerTimeZhanJi(object[] args = null)
    {
        bool isHaveBiSai = (int)args[5] > 0 ? true : false;
        WuBiSanObj.SetActive(!isHaveBiSai);
        ZongDuiJuTx.gameObject.SetActive(isHaveBiSai);
        LianShengTx.gameObject.SetActive(isHaveBiSai);
        if (!isHaveBiSai)
        {
            return;
        }
        int zongDuiJuVal = (int)args[5];
        int lianShengVal = (int)args[4];
        ZongDuiJuTx.text = zongDuiJuVal.ToString();
        LianShengTx.text = lianShengVal.ToString();
    }
}