using UnityEngine;
using System.Collections;
using RoomCardNet;
using Mahjong;
using System.Collections.Generic;

class ChiPengHuTips : UnModalUIBase {

    // Use this for initialization
    public GameObject pPanel;
    public CommonButton pGuo;
    public CommonButton pHu;
    public CommonButton pGang;
    public CommonButton pPeng;
    public CommonButton pChi;

    public float nWidth = 647.0f;  //最大宽度
    public float nHeight = 179.0f; //最大高度
    public float nxpos = 680.0f;  //默认位置
    public float nypos = 304.0f; //默认位置
    public float OneBtnWidth = 100.0f;
    public float OneBtn_Ypos = -90.0f;
    public float OneBtn_JianJu = 20.0f;
    public float FisrtBtn_Xpos = 75.0f;
    void Start() {

    }
    public void Initial()
    {
        nxpos = ((RectTransform)transform).anchoredPosition.x;
        nypos = ((RectTransform)transform).anchoredPosition.y;
        //((RectTransform)transform).anchoredPosition = new Vector2(nxpos, nypos);
        //((RectTransform)transform).sizeDelta = new Vector2(nWidth, nHeight);
        HideChiPengHuTips();

    }
    /// <summary>
    /// 隐藏吃碰胡界面
    /// </summary>
    public void HideChiPengHuTips()
    {

        if (pPanel != null)
        {
            pPanel.SetActive(false);
        }

        if (pGuo != null)
        {
            pGuo.gameObject.SetActive(false);
        }

        if (pHu != null)
        {
            pHu.gameObject.SetActive(false);
        }

        if (pGang != null)
        {
            pGang.gameObject.SetActive(false);
        }

        if (pPeng != null)
        {
            pPeng.gameObject.SetActive(false);
        }

        if (pChi != null)
        {
            pChi.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
    /// <summary>
    /// 显示吃碰胡界面
    /// </summary>
    /// <param name="isHu">是否可胡</param>
    /// <param name="isGang">是否可杠</param>
    /// <param name="isPeng">是否可碰</param>
    /// <param name="isChi">是否可吃</param>
    public void ShowChiPengHuTips(bool isHu,bool isGang,bool isPeng,bool isChi)
    {
        int n=0;
        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong15, true);
        if (pPanel != null)
        {
            pPanel.SetActive(true);
        }
        if (isHu && pHu != null)
        {
            ((RectTransform)pHu.transform).anchoredPosition = new Vector2(FisrtBtn_Xpos + OneBtnWidth * n + OneBtn_JianJu * n, OneBtn_Ypos);
            pHu.gameObject.SetActive(true);
            n++;
        }

        if (isGang && pGang != null)
        {
            ((RectTransform)pGang.transform).anchoredPosition = new Vector2(FisrtBtn_Xpos + OneBtnWidth * n + OneBtn_JianJu * n, OneBtn_Ypos);
            pGang.gameObject.SetActive(true);
             n++;
        }

        if (isPeng && pPeng != null)
        {
            ((RectTransform)pPeng.transform).anchoredPosition = new Vector2(FisrtBtn_Xpos + OneBtnWidth * n + OneBtn_JianJu*n, OneBtn_Ypos);
            pPeng.gameObject.SetActive(true);
             n++;
        }

        if (isChi && pChi != null)
        {
            ((RectTransform)pChi.transform).anchoredPosition = new Vector2(FisrtBtn_Xpos + OneBtnWidth * n + OneBtn_JianJu * n, OneBtn_Ypos);
            pChi.gameObject.SetActive(true);
            n++;
        }
        if (pGuo != null)
        {

            ((RectTransform)pGuo.transform).anchoredPosition = new Vector2(FisrtBtn_Xpos + OneBtnWidth * n + OneBtn_JianJu * n, OneBtn_Ypos);
            pGuo.gameObject.SetActive(true);
            n++;
            if (n < 2)  //至少有两个按钮才显示
            {
                HideChiPengHuTips();
                return;
            }
        }
        ((RectTransform)transform).anchoredPosition = new Vector2(nxpos - (OneBtnWidth * n + OneBtn_JianJu * (n-1))/2.0f, nypos);
        ((RectTransform)transform).sizeDelta = new Vector2(FisrtBtn_Xpos + OneBtnWidth * n + OneBtn_JianJu * (n-1), nHeight);
        gameObject.SetActive(true);
    }
	// Update is called once per frame
	void Update () {
	
	}
    public void ClickOneButton(int nKind)
    {
        GameObject temp;
        switch (nKind)
        {
            case 0:
                Debug.Log("Unity:"+"ClickOneButton Guo "+nKind.ToString());
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //过
                {
					if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect != null)
					{
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.DestroyThis();
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = null;
					}
					RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_NULL, (byte)MainRoot._pMJGameTable.GetCheckMJ);
                    HideChiPengHuTips();
                }
            break;
            case 1:
                Debug.Log("Unity:"+"ClickOneButton Hu" + nKind.ToString());
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //胡
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard((byte)CMD_SXMJ.WIK_CHI_HU, (byte)MainRoot._pMJGameTable.GetCheckMJ);
                    HideChiPengHuTips();
                }
                break;
            case 2:
                Debug.Log("Unity:"+"ClickOneButton Gang" + nKind.ToString());
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //杠
                {
                    List<MAJIANG> templist = MainRoot._pMJGameTable.GetPossibleGangList();
                    if (templist.Count == 0)
                    {
                        Debug.LogError("Unity:"+"Server 可以杠，Client 不能杠！");
                        HideChiPengHuTips();
                    }
                    if (templist.Count > 1)
                    {
                        if (pHu) { pHu.gameObject.SetActive(false); }
                        if (pPeng) { pPeng.gameObject.SetActive(false); }
                        if (pChi) { pChi.gameObject.SetActive(false); }
                        temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/SelectGangUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.transform, false);
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect = temp.GetComponent<SelectGangUI>();
						MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pSelect.Initial(templist);
                    }
                    else
                    {
                        RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_GANG, (byte)(int)templist[0]);
                        HideChiPengHuTips();
                    }
                }
                //MainRoot._pMJGameTable.UserDoGang();
                break;
            case 3:
                Debug.Log("Unity:"+"ClickOneButton Peng" + nKind.ToString());
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //碰
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_PENG, (byte)MainRoot._pMJGameTable.GetCheckMJ);
                    HideChiPengHuTips();
                }
                //MainRoot._pMJGameTable.UserDoPeng();
                break;
            case 4:
                if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //吃
                {
                    RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOperateCard(CMD_SXMJ.WIK_PENG, (byte)MainRoot._pMJGameTable.GetCheckMJ);
                    HideChiPengHuTips();
                }
                Debug.Log("Unity:"+"ClickOneButton Chi" + nKind.ToString());
                break;
            default:
                HideChiPengHuTips();
                break;
        }
    }
}
