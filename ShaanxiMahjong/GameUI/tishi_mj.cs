using UnityEngine;
using System.Collections;

/// <summary>
/// 界面上的停牌提示每张牌的元件
/// </summary>
class tishi_mj : UnModalUIBase
{

    public TextBase tMulti;
    public TextBase tSheet;
    public mj_ui pMjHuaSe;
	// Use this for initialization
	void Start () {
	
	}
    /// <summary>
    /// 初始化胡牌提示中的每个单元
    /// </summary>
    /// <param name="nIndex">牌号</param>
    /// <param name="nMulti">倍数</param>
    /// <param name="nSheet">剩余张数</param>
    /// <param name="isMask">是否置灰</param>
    public void Initial(int nIndex, int nMulti, int nSheet,bool isMask = false)
    {
        if (pMjHuaSe==null)
        {
            GameObject test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/mj_ui"),transform,false);
            pMjHuaSe = test.GetComponent<mj_ui>();
        }
        pMjHuaSe.Initial(nIndex, nSheet<=0);
        if (tMulti != null)
        {
            tMulti.text = nMulti.ToString();
        }
        if (tSheet != null)
        {
            tSheet.text = nSheet.ToString();
        }

    }
	// Update is called once per frame
	void Update () {
	
	}
}
