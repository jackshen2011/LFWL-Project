using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// 普通结算界面麻将牌面控制逻辑.
/// </summary>
class SJiSuanGameCtrl : MonoBehaviour
{
    /**
     * ShouPai -> 手牌.
     * PengPai -> 碰牌.
     * GangPai -> 杠牌.
     */
    enum PaiMianEnum
    {
        ShouPai,
        PengPai = 8,
        GangPai = 16,
    }
    //public Sprite[] MaJiangSp;
    public List<mj_ui> MaJiangList = new List<mj_ui>();
    /**
     * 赢家牌的起始位置.
     */
    //public Vector2 MJStartPos;
    /**
     * 赢家每张牌的水平间隔距离.
     */
    public float OffsetPX = 20f;
    /**
     * 赢家碰或杠的水平间隔距离.
     */
    public float OffsetPGX = 20f;
    /**
     * 麻将的宽度.
     */
    public float MaJiangWidth = 134f;
    public float MaJiangHeight = 192f;
    /// <summary>
    /// 麻将真实原大小.
    /// </summary>
    Vector2 MaJiangSizeReal = Vector2.zero;
    /**
     * 赢家牌的数量.
     */
    public int MaxCountPM = 13;
    /// <summary>
    /// 结算界面麻将的父级.
    /// </summary>
    //public Transform MaJiangPtr;
//    void Awake()
//    {
//        InitJieSuanGameInfo(new object[1]); //test.
//    }

    void SpawnYingJiaMaJiang()
    {
        MaJiangList.Clear();
        GameObject obj = null;
        //Vector2 pos = MJStartPos;
        //float px = 0f;
        RectTransform rectTr = null;
        //Vector2 sizeVal = new Vector2(MaJiangWidth, MaJiangHeight);
        for (int i = 0; i < MaxCountPM; i++) {
            obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/mj_ui"), transform, false);
            MaJiangList.Add(obj.GetComponent<mj_ui>());
            //px = MJStartPos.x + (MaJiangWidth + OffsetPX) * i;
            //pos.x = px;
            //rectTr = obj.GetComponent<RectTransform>();
            //rectTr.sizeDelta = sizeVal;
            //rectTr.anchoredPosition = pos;
            //MaJiangList[i].SetMaJiangHuaSeSize(sizeVal);
        }

        if(obj != null)
        {
            rectTr = obj.GetComponent<RectTransform>();
            MaJiangSizeReal = rectTr.sizeDelta;
        }
    }

    /**
     * 赢家牌面数据列表.
     */
    List<byte> YingJiaPaiList = new List<byte>();
    /**
     * 设置赢家牌面数据.
     */
    public void SetYingJiaPaiListDt(byte val)
    {
        YingJiaPaiList.Add(val);
    }
    /**
     * 赢家手牌数量.
     */
    byte YingJiaShouPaiNum = 10;
    /**
     * 赢家碰牌数量.
     */
    byte YingJiaPengPaiNum = 3;
    /**
     * 赢家杠牌数量.
     */
    byte YingJiaGangPaiNum = 0;
    /// <summary>
    /// 牌面状态,确定牌是手牌,碰牌,杠牌.
    /// </summary>
    public byte[] PaiMianStAy;
    ///<summary>
    /// 初始化结算界面的信息.
    /// param -> 服务器返回的数据.
    /// 0-8 -> 玩家的分数.
    ///</summary>
    public void InitJieSuanGameInfo(object[] args, bool isLiuJu = false)
    {
        if (isLiuJu)
        {
            return;
        }
        List<byte> cardData = new List<byte>();
        byte[] cardDataTmp = new byte[18];
        byte[] pengGangDataTmp = new byte[18];
        int pengGangNum = 0;
        //PaiMianStAy = new byte[10];
        //PaiMianStAy[0] = (byte)PaiMianEnum.ShouPai;
        //PaiMianStAy[1] = (byte)PaiMianEnum.PengPai;

        YingJiaPaiList.Clear();
        //赢家的牌面信息,信息参照PlayerLinkClientPlayer的结算.
        for (int index = 0; index < 4; index++) {
            if ((int)args[28] == (int)(args[2 + index * 6])) {
                cardDataTmp = ((MemoryStream)(args[4 + index * 6])).ToArray();
                pengGangNum = (int)(args[5 + index * 6]);
                pengGangDataTmp = ((MemoryStream)(args[6 + index * 6])).ToArray();
            }
        }

        for (int i = 0; i < cardDataTmp.Length; i++)
        {
            if (cardDataTmp[i] == 0)
            {
                break;
            }
            cardData.Add(cardDataTmp[i]);
        }
        YingJiaShouPaiNum = (byte)cardData.Count;
        PaiMianStAy = new byte[pengGangNum + 1];
        PaiMianStAy[0] = (byte)PaiMianEnum.ShouPai;

        for (int i = 1; i < PaiMianStAy.Length; i++)
        {
            switch ((PaiMianEnum)pengGangDataTmp[2*(i-1) + 1])
            {
                case PaiMianEnum.PengPai:
                    for (int j = 0; j < 3; j++)
                    {
                        cardData.Add(pengGangDataTmp[2 * (i - 1)]);
                    }
                    PaiMianStAy[i] = (byte)PaiMianEnum.PengPai;
                    break;
                case PaiMianEnum.GangPai:
                    for (int j = 0; j < 4; j++)
                    {
                        cardData.Add(pengGangDataTmp[2 * (i - 1)]);
                    }
                    PaiMianStAy[i] = (byte)PaiMianEnum.GangPai;
                    break;
            }
        }

        bool isSetHuPaiVal = false;
        for (int i = 0; i < cardData.Count; i++) {
            if (!isSetHuPaiVal && cardData[i] == (byte)args[29]) {
                isSetHuPaiVal = true;
                YingJiaHuPaiVal = (byte)args[29];
            }
            SetYingJiaPaiListDt(cardData[i]);
        }
        MaxCountPM = cardData.Count;

        SpawnYingJiaMaJiang();
        SetYingJiaPaiMianInfo();
    }
    /**
     * 牌面里手牌,碰牌,杠牌状态的索引.
     */
    int IndexPaiMianSt = 0;
    /**
     * 获取牌面手牌,碰牌,杠牌状态.
     */
    PaiMianEnum GetPaiMianState()
    {
        if (PaiMianStAy == null)
            return PaiMianEnum.ShouPai;
        if(IndexPaiMianSt >= PaiMianStAy.Length)
            return PaiMianEnum.ShouPai;
        PaiMianEnum paiMianSt = (PaiMianEnum)PaiMianStAy[IndexPaiMianSt];
        IndexPaiMianSt++;
        return paiMianSt;
    }
    /**
     * 碰牌的单元数量.
     */
    const byte PengPaiNum = 3;
    /**
     * 杠牌的单元数量.
     */
    const byte GangPaiNum = 4;
    /**
     * 赢家胡牌信息.
     */
    public byte YingJiaHuPaiVal = 1;
    /**
     * 设置赢家牌面信息.
     */
    void SetYingJiaPaiMianInfo()
    {
        PaiMianEnum paiMianSt = PaiMianEnum.ShouPai;
        byte indexPNum = 0; //碰牌索引.
        byte indexGNum = 0; //杠牌索引.
        bool isShowHuPai = false;
        Transform maJiangPtr = null;
        Debug.Log("Unity:"+"SetYingJiaPaiMianInfo...");

        maJiangPtr = SpawnMaJiangGrid();
        GetPaiMianState();
        for (int i = 0; i < MaxCountPM; i++)
        {
            MaJiangList[i].Initial(YingJiaPaiList[i]);
            if (i < YingJiaShouPaiNum)
            {
                if (!isShowHuPai && YingJiaHuPaiVal == YingJiaPaiList[i])
                {
                    isShowHuPai = true;
                    MaJiangList[i].SetTagImg();
                }
            }
            else
            {
                if (indexPNum % PengPaiNum == 0 && paiMianSt == PaiMianEnum.PengPai)
                {
                    maJiangPtr = SpawnMaJiangGrid();
                }
                if (indexGNum % 4 == 0 && paiMianSt == PaiMianEnum.GangPai)
                {
                    maJiangPtr = SpawnMaJiangGrid();
                }

                if (paiMianSt == PaiMianEnum.PengPai)
                {
                    if (indexPNum % PengPaiNum == (PengPaiNum - 1))
                    {
                        paiMianSt = GetPaiMianState();
                    }
                    indexPNum++;
                }
                if (paiMianSt == PaiMianEnum.GangPai)
                {
                    if (indexPNum % GangPaiNum == (GangPaiNum - 1))
                    {
                        paiMianSt = GetPaiMianState();
                    }
                    indexGNum++;
                }
            }
            MaJiangList[i].transform.SetParent(maJiangPtr);
        }

        Vector2 posGrid = Vector2.zero;
        Vector2 sizeGrid = new Vector2(0f, MaJiangSizeReal.y);
        GridLayoutGroup GridLayout = null;
        int maJiangCount = 0;
        if (MaJiangGridPtr != null)
        {
            ((RectTransform)MaJiangGridPtr).sizeDelta = sizeGrid;
        }

        for (int i = 0; i < MaJiangGridList.Count; i++)
        {
            if (MaJiangGridPtr != null)
            {
                MaJiangGridList[i].SetParent(MaJiangGridPtr);
            }
            maJiangCount = MaJiangGridList[i].childCount;
            GridLayout = MaJiangGridList[i].GetComponent<GridLayoutGroup>();
            GridLayout.spacing = new Vector2(OffsetPX, 0f);
            sizeGrid.x = (MaJiangSizeReal.x * maJiangCount) + ((maJiangCount - 1) * GridLayout.spacing.x);
            MaJiangGridList[i].sizeDelta = sizeGrid;
            MaJiangGridList[i].localScale = Vector3.one;
            if (i == 0)
            {
                posGrid.x = MaJiangGridList[i].sizeDelta.x * 0.5f;
            }
            else
            {
                posGrid.x = OffsetPGX + MaJiangGridList[i-1].anchoredPosition.x
                    + (MaJiangGridList[i-1].sizeDelta.x * 0.5f)
                    + (MaJiangGridList[i].sizeDelta.x * 0.5f);
            }
            MaJiangGridList[i].anchoredPosition = posGrid;

            if (MaJiangGridPtr != null)
            {
                ((RectTransform)MaJiangGridPtr).sizeDelta += sizeGrid;
                if (i > 0)
                {
                    ((RectTransform)MaJiangGridPtr).sizeDelta += new Vector2(OffsetPGX, 0f);    
                }
            }
        }
        MaJiangGridPtr.localScale = new Vector3(MaJiangWidth / MaJiangSizeReal.x, MaJiangHeight / MaJiangSizeReal.y, 0f);
    }

    /// <summary>
    /// 麻将Grid父级,该对象必须设置为中心对齐.
    /// </summary>
    public Transform MaJiangGridPtr;
    List<RectTransform> MaJiangGridList = new List<RectTransform>();
    Transform SpawnMaJiangGrid()
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/PTJieSuanUI/PTJiSuanMaJiangGrid"), transform, false);
        MaJiangGridList.Add(obj.GetComponent<RectTransform>());
        return obj.transform;
    }
}