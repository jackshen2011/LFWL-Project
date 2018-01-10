using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

class TingPaiTishiUI : UnModalUIBase
{

    public Transform grid_tishi_mj;
    GridLayoutGroup p_tishi_Grid;
    public List<tishi_mj> tishi_mj_list = new List<tishi_mj>();

    Vector3[] currentTips;

    public float nWidth = 765.0f;  //最大宽度
    public float nHeight = 480.0f; //最大高度
    public float nxpos = 345.0f;  //默认位置
    public float nypos = -189.0f; //默认位置

    public float DefaultHeight = 170.0f;
    public float OneBtnWidth = 200.0f;
    public float OneBtn_Height = 115.0f;
    public float OneBtn_JianJu_X = 15.0f;
    public float OneBtn_JianJu_Y = 15.0f;
    public float FisrtBtn_Xpos = 110.0f;

	bool isSave =false; //是否已经记录过初始状态
    // Use this for initialization
    void Start ()
    {

    }
    public void Initial()
    {
		nxpos = ((RectTransform)transform).anchoredPosition.x;
		nypos = ((RectTransform)transform).anchoredPosition.y;
		nWidth = ((RectTransform)transform).sizeDelta.x+1;
		nHeight = ((RectTransform)transform).sizeDelta.y+1;
		HideTingPaiTishi();
    }
    public void ShowTingPaiTiShi()
    {
        if (currentTips != null)
        {
            ShowTingPaiTishi(currentTips);
        }
	}
    public void HideTingPaiTishi()
    {
        for (int i = 0; i < tishi_mj_list.Count; i++)
        {
            tishi_mj_list[i].gameObject.SetActive(false);
        }
        ((RectTransform)transform).anchoredPosition = new Vector2(nxpos, nypos);
        ((RectTransform)transform).sizeDelta = new Vector2(nWidth, nHeight);
        gameObject.SetActive(false);
	}
	/// <summary>
	/// 无参数版本即为清理当前缓存数据
	/// </summary>
	/// <param name="tishi_info"></param>
    public void ShowTingPaiTishi(Vector3[] tishi_info,bool isRest = false)
    {
        HideTingPaiTishi();
        if (isRest)
		{
			currentTips = null;
			return;
		}

		int nlen = 9;
        float xpos = nxpos;
        float ypos = nypos;
        float width = nWidth;
        float height = nHeight;

        if (tishi_info == null)
        {
            return;
        }
        currentTips = tishi_info;
        //高度还原
        height = DefaultHeight;

        if (tishi_info.Length< nlen)
        {
            nlen = tishi_info.Length;
        }
        //宽度计算
        width = nWidth + OneBtnWidth * (nlen<=3? nlen-1 :2) + OneBtn_JianJu_X * (nlen <= 3 ? nlen - 1 : 2);
		height = nHeight;

		for (int i = 0; i < tishi_info.Length; i++)
        {
            tishi_mj_list[i].gameObject.SetActive(true);
            tishi_mj_list[i].Initial((int)tishi_info[i].x, (int)tishi_info[i].y, (int)tishi_info[i].z);
            if (i>0 && i%3==0) //增加高度
            {
                height += (OneBtn_Height+ OneBtn_JianJu_Y);
            }
        }
        ((RectTransform)transform).anchoredPosition = new Vector2(xpos, ypos);
        ((RectTransform)transform).sizeDelta = new Vector2(width, height);

        if (p_tishi_Grid == null)
        {
            p_tishi_Grid = grid_tishi_mj.GetComponent<GridLayoutGroup>();
        }
		((RectTransform)p_tishi_Grid.transform).sizeDelta = new Vector2(width - FisrtBtn_Xpos, height - 2.0f * OneBtn_JianJu_Y);
		p_tishi_Grid.cellSize = new Vector2(OneBtnWidth, OneBtn_Height);
		p_tishi_Grid.spacing = new Vector2(OneBtn_JianJu_X, OneBtn_JianJu_Y);

		grid_tishi_mj.gameObject.SetActive(true);
        gameObject.SetActive(true);
	}
	// Update is called once per frame
	void Update () {
	
	}
}
