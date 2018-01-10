using UnityEngine;
using System.Collections;

/// <summary>
/// UI上的显示用单张麻将牌
/// </summary>
class mj_ui : UnModalUIBase
{
    public ImageBase pHuaSeImg;
    public GameObject gMask;
    public int nMaji = 0;
    /**
     * 标记胡牌.
     */
    public ImageBase TagImg;
    /// <summary>
    /// UI上的显示用单张麻将牌初始化
    /// </summary>
    /// <param name="nindex">牌号</param>
    /// <param name="isMask">是否置灰</param>
    public void Initial(int nindex, bool isMask = false)
    {
        nMaji = nindex;
        pHuaSeImg.material = MainRoot._pMJGameTable.GetTypicalMaJiangMaterial((MAJIANG)nindex);
        gMask.SetActive(isMask);
    }
    /// <summary>
    /// 用于设定在选杠界面中的位置
    /// </summary>
    /// <param name="n"></param>
    public void SetMjPos(int n)
    {
        ((RectTransform)transform).anchoredPosition = new Vector2(((RectTransform)transform).sizeDelta.x * (n-2)+24 + n*5 +5,0);
    }

    public void SetMaJiangHuaSeSize(Vector2 sizeVal)
    {
        RectTransform rectTr = pHuaSeImg.GetComponent<RectTransform>();
        rectTr.sizeDelta = sizeVal;
    }

    /**
     * 设置标记胡.
     */
    public void SetTagImg()
    {
        TagImg.gameObject.SetActive(true);
    }
}
