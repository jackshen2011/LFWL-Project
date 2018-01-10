using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomCardNet;
using Mahjong;

class SelectGangUI : UnModalUIBase {


    public List<GameObject> pListGang;
    public List<mj_ui> pListMj;
    // Use this for initialization
    /// <summary>
    /// 初始化杠牌选择界面
    /// </summary>
    /// <param name="plist">可杠的牌型</param>
    public void Initial(List<MAJIANG> plist)
    {
        MAJIANG item;
        GameObject test;
        mj_ui pMjHuaSe;
		float nwidth = 18.0f; 
        for (int i = 0; i <Mathf.Min(plist.Count,pListGang.Count); i++)
        {
            pListGang[i].SetActive(true);
            item = plist[i];
            test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/mj_ui"), pListGang[i].transform, false);
            pMjHuaSe = test.GetComponent<mj_ui>();
            pListMj.Add(pMjHuaSe);
            pMjHuaSe.Initial((int)item);
			pMjHuaSe.SetMjPos(0);

			test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/mj_ui"), pListGang[i].transform, false);
            pMjHuaSe = test.GetComponent<mj_ui>();
            pMjHuaSe.Initial((int)item);
            pMjHuaSe.SetMjPos(1);

            test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/mj_ui"), pListGang[i].transform, false);
            pMjHuaSe = test.GetComponent<mj_ui>();
            pMjHuaSe.Initial((int)item);
            pMjHuaSe.SetMjPos(2);

            test = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/mj_ui"), pListGang[i].transform, false);
            pMjHuaSe = test.GetComponent<mj_ui>();
            pMjHuaSe.Initial((int)item);
            pMjHuaSe.SetMjPos(3);

			nwidth += (76*4+20);

		}
		((RectTransform)transform).sizeDelta = new Vector2(nwidth, ((RectTransform)transform).sizeDelta.y);
	}
	/// <summary>
	/// 多选杠牌
	/// </summary>
	/// <param name="n"></param>
	public void OnGangBtnClick(int n)
    {
        switch (n)
        {
            case 1:
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_GANG, (byte)pListMj[n-1].nMaji);
                break;
            case 2:
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_GANG, (byte)pListMj[n-1].nMaji);
                break;
            case 3:
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_GANG, (byte)pListMj[n-1].nMaji);
                break;
            default:
                break;
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips.HideChiPengHuTips();
        }
        DestroyThis();
    }
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
