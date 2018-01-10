using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

class HuPaiTishiUI : UnModalUIBase
{

    public Transform grid_tishi_mj;
    GridLayoutGroup p_tishi_Grid;
    public List<tishi_mj> tishi_mj_list = new List<tishi_mj>();

    public float nMaxWidth = 765.0f;  //最大宽度
    public float nMaxHeight = 480.0f; //最大高度
    public float nxpos = 345.0f;  //默认位置
    public float nypos = -189.0f; //默认位置

    public float DefaultHeight = 170.0f;
    public float OneBtnWidth = 200.0f;
    public float OneBtn_Height = 115.0f;
    public float OneBtn_JianJu_X = 15.0f;
    public float OneBtn_JianJu_Y = 15.0f;
    public float FisrtBtn_Xpos = 110.0f;
    // Use this for initialization
    void Start ()
    {

    }
    public void Initial()
    {
        nxpos = ((RectTransform)transform).anchoredPosition.x;
        nypos = ((RectTransform)transform).anchoredPosition.y;
        ((RectTransform)transform).anchoredPosition = new Vector2(nxpos, nypos);
        ((RectTransform)transform).sizeDelta = new Vector2(nMaxWidth, nMaxHeight);
        HideHuPaiTishi();

    }
    public void HideHuPaiTishi()
    {

        for (int i = 0; i < tishi_mj_list.Count; i++)
        {
            tishi_mj_list[i].gameObject.SetActive(false);
        }
        ((RectTransform)transform).anchoredPosition = new Vector2(nxpos, nypos);
        ((RectTransform)transform).sizeDelta = new Vector2(nMaxWidth, nMaxHeight);
        gameObject.SetActive(false);
    }
    public void ShowHuPaiTishi(Vector3[] tishi_info)
    {
        int nlen = 9;
        float xpos = ((RectTransform)transform).anchoredPosition.x;
        float ypos = ((RectTransform)transform).anchoredPosition.y;
        float width = ((RectTransform)transform).sizeDelta.x;
        float height = ((RectTransform)transform).sizeDelta.y;

        if (grid_tishi_mj==null)
        {
            return;
        }
        //高度还原
        height = DefaultHeight;

        if (tishi_info.Length< nlen)
        {
            nlen = tishi_info.Length;
        }
        if (nlen <= 3) //宽度计算
        {
            width = FisrtBtn_Xpos + OneBtnWidth * nlen + OneBtn_JianJu_X * nlen;
            xpos -= (OneBtnWidth * nlen + OneBtn_JianJu_X * (nlen-1))/2.0f;
        }
        else
        {
            width = FisrtBtn_Xpos + OneBtnWidth * 3 + OneBtn_JianJu_X * 3;
            xpos -= (OneBtnWidth * 2 + OneBtn_JianJu_X * 1) / 2.0f;
        }

        for (int i = 0; i < tishi_info.Length; i++)
        {
            tishi_mj_list[i].gameObject.SetActive(true);
            tishi_mj_list[i].Initial((int)tishi_info[i].x, (int)tishi_info[i].y, (int)tishi_info[i].z);
            if (i>0 && i%3==0) //增加高度
            {
                height += (OneBtn_Height+ OneBtn_JianJu_Y);
                ypos += (OneBtn_Height+ OneBtn_JianJu_Y) /2.0f;
            }
        }
        ((RectTransform)transform).anchoredPosition = new Vector2(xpos, ypos);
        ((RectTransform)transform).sizeDelta = new Vector2(width, height);

        if (p_tishi_Grid == null)
        {
            p_tishi_Grid = grid_tishi_mj.GetComponent<GridLayoutGroup>();
            ((RectTransform)p_tishi_Grid.transform).sizeDelta = new Vector2(width - FisrtBtn_Xpos, height-2.0f * OneBtn_JianJu_Y);
            ((RectTransform)p_tishi_Grid.transform).anchoredPosition = new Vector2(FisrtBtn_Xpos / 2, -1.0f * OneBtn_JianJu_Y);

            p_tishi_Grid.cellSize = new Vector2(OneBtnWidth, OneBtn_Height);
            p_tishi_Grid.spacing = new Vector2(OneBtn_JianJu_X, OneBtn_JianJu_Y);
        }

        grid_tishi_mj.gameObject.SetActive(true);
        gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update () {
	
	}
}
