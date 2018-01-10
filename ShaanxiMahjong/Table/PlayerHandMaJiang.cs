using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 玩家自己可操作的手牌片管理
/// </summary>
class PlayerHandMaJiang : MonoBehaviourIgnoreGui
{
    public float firstHandXpos = -0.461f;
    public float firstHandYpos = -1.31f;
    public float clearance=0.04f;
    public HandMaJiang handMaJiangPrefab;//手牌预置
    List<HandMaJiang> myHandMaJiang=null;//手牌
    HandMaJiang choiceMaJiang;//接的牌

    public HandMaJiang dragMaJiang;//拖拽的虚影

    // Use this for initialization
    void InitPlayerHandMaJiang () {
        handMaJiangPrefab = InitOneHandMJModel(MAJIANG.MJ01,new Vector3(10, 10, -10));
    }
    public void ResetFirstHandPos(float x,float y)
    {
        firstHandXpos = x;
        firstHandYpos = y;
    }
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 初始化一副手牌
    /// </summary>
    /// <param name="mjs">有序牌面列表</param>
    /// <param name="choiceMj">如果有接的牌</param>
    public void InitAGroupHandMaJiang(int[]mjs,int choiceMj)
    {
        if (myHandMaJiang == null)
            myHandMaJiang = new List<HandMaJiang>();
        HandMaJiang mjScript;
        for (int i = 0; i < mjs.Length; i++)
        {
            mjScript = InitOneHandMJModel((MAJIANG)mjs[i],Vector3.zero);
            mjScript.ArrayIndex = i;
            myHandMaJiang.Add(mjScript);
        }
        if (choiceMj>0)
        {
            mjScript = InitOneHandMJModel((MAJIANG)choiceMj, Vector3.zero);
            choiceMaJiang = mjScript;
        }
        RefreshHandMaJiangPosition();
    }
    public void CreateOneHandMaJiangAtIndex(int nIndex,int mj,bool over)
    {
        HandMaJiang mjScript;
        if (myHandMaJiang == null)
        {
            myHandMaJiang = new List<HandMaJiang>();
        }
        if (myHandMaJiang.Count<=nIndex)
        {
            mjScript = InitOneHandMJModel((MAJIANG)mj, Vector3.zero);
            mjScript.ArrayIndex = nIndex;
            myHandMaJiang.Add(mjScript);
        }
        //if (over)
        //{
        RefreshHandMaJiangPosition();
       // }
    }
    /// <summary>
    /// 接一张牌
    /// </summary>
    /// <param name="mj"></param>
    public void TakeOneHandMaJiang(MAJIANG mj)
    {
        HandMaJiang obj;
        Vector3 pos;
        if (choiceMaJiang)
        {
            Debug.Log("Unity:"+"已有牌，不能再接");
            return;
        }
        pos = new Vector3(firstHandXpos+ clearance + myHandMaJiang.Count * HandMaJiang.handWidth, firstHandYpos, 0);
        obj = InitOneHandMJModel(mj, pos);
        choiceMaJiang = obj;
    }
    /// <summary>
    /// 出一张牌并洗牌
    /// </summary>
    /// <param name="nIndex"></param>
    public void PutOutOneHandMaJiang(int nIndex)
    {
        if (nIndex < 0)
        {
            Destroy(choiceMaJiang.gameObject);
            choiceMaJiang = null;
        }
        else
        {
            if (nIndex >= myHandMaJiang.Count)
            {
                return;
            }
            Destroy(myHandMaJiang[nIndex].gameObject);
            myHandMaJiang.RemoveAt(nIndex);
            for (int i = 0; i < myHandMaJiang.Count; i++)
            {
                if (myHandMaJiang[i].WhoAmI>=choiceMaJiang.WhoAmI)
                {
                    myHandMaJiang.Insert(i,choiceMaJiang);
                    choiceMaJiang = null;
                    break;
                }
            }
            if (choiceMaJiang)
            {
                myHandMaJiang.Add(choiceMaJiang);
                choiceMaJiang = null;
            }
            RefreshHandMaJiangPosition();
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.HideTingPaiTishi();
        }
        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips.HideChiPengHuTips();
        }

        CheckTingPaiBtnShow();
    }
    public void HandPeng(int nIndex)
    {
        if (nIndex<0 || nIndex>myHandMaJiang.Count)
        {
            return;
        }
        DeleteHandMaJiang(nIndex,2,false);
        LastMJ();
    }
    public void HandZhiGang(int nIndex,MAJIANG mj)
    {
        if (nIndex < 0 || nIndex > myHandMaJiang.Count)
        {
            return;
        }
        DeleteHandMaJiang(nIndex, 3, false);
        if (mj>0)
        {
            TakeOneHandMaJiang(mj);
        }
        RefreshHandMaJiangPosition();
        //LastMJ();
    }
    public void HandAnGang(int nIndex,MAJIANG mj,bool isChoice)
    {
        if (isChoice)
        {
            DeleteHandMaJiang(-1, 0, false);
            DeleteHandMaJiang(nIndex, 3);
        }
        else
        {
            DeleteHandMaJiang(nIndex, 4,false);
            if (choiceMaJiang.WhoAmI > myHandMaJiang[myHandMaJiang.Count - 1].WhoAmI)
            {
                myHandMaJiang.Add(choiceMaJiang);
                choiceMaJiang = null;
            }
            else
            {
                for (int i = 0; i < myHandMaJiang.Count; i++)
                {
                    if (myHandMaJiang[i].WhoAmI >= choiceMaJiang.WhoAmI)
                    {
                        myHandMaJiang.Insert(i, choiceMaJiang);
                        choiceMaJiang = null;
                        break;
                    }
                }
            }
            RefreshHandMaJiangPosition();
        }
        if (mj > 0)
        {
            TakeOneHandMaJiang(mj);
        }
    }
    public void HandXuGang(int nIndex,MAJIANG mj,bool isChoice)
    {
        if (isChoice)
        {
            DeleteHandMaJiang(-1, 0, false);
        }
        else
        {
            DeleteHandMaJiang(nIndex, 1,false);
            if (choiceMaJiang.WhoAmI > myHandMaJiang[myHandMaJiang.Count - 1].WhoAmI)
            {
                myHandMaJiang.Add(choiceMaJiang);
                choiceMaJiang = null;
            }
            else
            {
                for (int i = 0; i < myHandMaJiang.Count; i++)
                {
                    if (myHandMaJiang[i].WhoAmI >= choiceMaJiang.WhoAmI)
                    {
                        myHandMaJiang.Insert(i, choiceMaJiang);
                        choiceMaJiang = null;
                        break;
                    }
                }
            }
            RefreshHandMaJiangPosition();
        }
        if (mj > 0)
        {
            TakeOneHandMaJiang(mj);
        }
    }
    /// <summary>
    /// 删除牌
    /// </summary>
    /// <param name="nIndex">起始索引，-1表示刚接的牌</param>
    /// <param name="count">删除数量</param>
    /// <param name="isRefresh">是否需要刷新坐标</param>
    private void DeleteHandMaJiang(int nIndex,int count,bool isRefresh=true)
    {
        if (nIndex + count > myHandMaJiang.Count)
        {
            return;
        }
        if (nIndex < 0)
        {
            Destroy(choiceMaJiang.gameObject);
            choiceMaJiang = null;
        }
        else
        {
            for (int i = nIndex; i < nIndex+count; i++)
            {
                Destroy(myHandMaJiang[i].gameObject);
            }
            
            myHandMaJiang.RemoveRange(nIndex, count);
            if (isRefresh)
            {
                RefreshHandMaJiangPosition();
            }
        }
    }
    /// <summary>
    /// 把最后一张牌作为刚接的牌
    /// </summary>
    /// <param name="isRefresh">是否要刷新坐标</param>
    private void LastMJ(bool isRefresh=true)
    {
        if (choiceMaJiang)
        {
            return;
        }
        choiceMaJiang = myHandMaJiang[myHandMaJiang.Count - 1];
        myHandMaJiang.RemoveAt(myHandMaJiang.Count - 1);
        if (isRefresh)
        {
            RefreshHandMaJiangPosition();
        }
    }
    /// <summary>
    /// 刷新位置
    /// </summary>
    public void RefreshHandMaJiangPosition()
    {
        Vector3 pos;
        int i;
        for (i = 0; i < myHandMaJiang.Count; i++)
        {
            pos = new Vector3(firstHandXpos + i*HandMaJiang.handWidth, firstHandYpos, 0);
            myHandMaJiang[i].transform.position = pos;
            myHandMaJiang[i].ArrayIndex = i;
        }
        if(choiceMaJiang)
        {
            pos = new Vector3(firstHandXpos + i* HandMaJiang.handWidth+ clearance, firstHandYpos, 0);
            choiceMaJiang.transform.position = pos;
            choiceMaJiang.ArrayIndex = -1;
        }
    }
    /// <summary>
    /// 删除所有牌
    /// </summary>
    public void ClearAllHandMaJiang()
    {
        for (int i = 0; i < myHandMaJiang.Count; i++)
        {
            Destroy(myHandMaJiang[i].gameObject);
        }
        if (choiceMaJiang)
        {
            Destroy(choiceMaJiang.gameObject);
        }
        myHandMaJiang.Clear();
    }
    /// <summary>
    /// 创建麻将手牌
    /// </summary>
    /// <param name="mj">牌面</param>
    /// <param name="pos">位置</param>
    /// <returns></returns>
    HandMaJiang InitOneHandMJModel(MAJIANG mj,Vector3 pos)
    {
        HandMaJiang scriptMJ;
        GameObject obj;
        if (handMaJiangPrefab)
        {
            obj = Instantiate(handMaJiangPrefab.gameObject);
        }
        else
        {
            obj = (GameObject)Instantiate(Resources.Load("Prefabs/HandMaJiang"), transform, false);
        }
        scriptMJ = obj.GetComponent<HandMaJiang>();
        scriptMJ.SetParent(this);
        scriptMJ.SetStates(mj, MJKIND.HAND);
        scriptMJ.ChangeMJMaterial(MainRoot._pMJGameTable.GetTypicalMaJiangMaterial(mj));
        if (obj != null)
        {
            obj.transform.position = pos;
        }
        return scriptMJ;
    }
    /// <summary>
    /// 强制刷新手牌
    /// </summary>
    /// <param name="mj"></param>
    /// <param name="choiceMj"></param>
    public void ResetHandMJ(int[]mj,int choiceMj)
    {
        if (mj == null)
        {
            return;
        }
        int zeroIndex = mj.Length;
        for (int i = 0; i < mj.Length; i++)
        {
            if (mj[i] == 0)
            {
                zeroIndex = i;
                break;
            }
        }
        for (int i = myHandMaJiang.Count - 1; i >= zeroIndex; i--)
        {//删除多余的牌
            if (myHandMaJiang[i] != null)
            {
                Destroy(myHandMaJiang[i].gameObject);
                myHandMaJiang.RemoveAt(i);
            }
        }
        for (int i = 0; i < zeroIndex - myHandMaJiang.Count; i++)
        {
            myHandMaJiang.Add(null);
        }
        for (int i = 0; i < zeroIndex; i++)
        {
            if (myHandMaJiang[i] != null)
            {
                myHandMaJiang[i].ChangeMJMaterial(MainRoot._pMJGameTable.GetTypicalMaJiangMaterial(MainRoot._pMJGameTable.GetMaJiangByInt(mj[i])));
                myHandMaJiang[i].SetStates(MainRoot._pMJGameTable.GetMaJiangByInt(mj[i]), MJKIND.STAND);
            }
            else
            {
                HandMaJiang oneMj = InitOneHandMJModel(MainRoot._pMJGameTable.GetMaJiangByInt(mj[i]),Vector3.zero);
                oneMj.ArrayIndex = i;
                myHandMaJiang[i] = oneMj;
            }
        }
        if (choiceMj > 0)
        {
            if (choiceMaJiang != null)
            {
                choiceMaJiang.ChangeMJMaterial(MainRoot._pMJGameTable.GetTypicalMaJiangMaterial(MainRoot._pMJGameTable.GetMaJiangByInt(choiceMj)));
                choiceMaJiang.SetStates(MainRoot._pMJGameTable.GetMaJiangByInt(choiceMj),MJKIND.STAND);
            }
            else
            {
                HandMaJiang oneMj = InitOneHandMJModel(MainRoot._pMJGameTable.GetMaJiangByInt(choiceMj), Vector3.zero);
                oneMj.ArrayIndex = -1;
                choiceMaJiang = oneMj;
            }
        }
        else
        {
            if (choiceMaJiang != null)
            {
                Destroy(choiceMaJiang.gameObject);
                choiceMaJiang = null;
            }
        }
        RefreshHandMaJiangPosition();
    }
    /// <summary>
    /// 指定麻将升起选中
    /// </summary>
    /// <param name="mj"></param>
    public void OnHandMaJiangPointUp(HandMaJiang mj)
    {
        if (mj.Selected)
        {
            return; 
        }
        AllHandMaJiangDown();
        mj.Selected = true;

        List<byte> tempCards = new List<byte>();
        for (int i = 0; i < myHandMaJiang.Count; i++)
        {
            if (!myHandMaJiang[i].Selected)
            {
                tempCards.Add((byte)myHandMaJiang[i].WhoAmI);
            }
        }
        if (choiceMaJiang)
        {
            if (!choiceMaJiang.Selected)
            {
                tempCards.Add((byte)choiceMaJiang.WhoAmI);
            }
        }
        else
        {
            return;
        }
        System.UInt32[] showTingPai = GetTingPaiIfPutOut(tempCards.ToArray());

        if (showTingPai.Length>0)
        {
            if (MainRoot._gGameRoomCenter.gGameRoom != null)
            {
                if (!MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
                {
                    GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/TingPaiTishiUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi = temp.GetComponent<TingPaiTishiUI>();
					MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.Initial();
                }
                Vector3[] tparams = new Vector3[showTingPai.Length / 2];
                for (int i = 0; i < tparams.Length; i++)
                {
                    tparams[i] = new Vector3(showTingPai[i * 2], showTingPai[i * 2+1], MainRoot._pMJGameTable.GetRemainHuMaJiangCount((MAJIANG)showTingPai[i * 2]));
                }
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTishi(tparams);
            }
        }
        else
        {
            if (MainRoot._gGameRoomCenter.gGameRoom != null)
            {
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTishi(new Vector3[1] { Vector3.zero },true);
                }
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.activeSelf)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.SetActive(false);
                }
            }
        }
    }
    /// <summary>
    /// 取停牌数据
    /// </summary>
    /// <param name="card"></param>
    /// <returns></returns>
    private System.UInt32[] GetTingPaiIfPutOut(byte[] card)
    {
        byte[] bt = new byte[34];
        int index;
        for (int i = 0; i < card.Length; i++)
        {
            index = MainRoot._pMJGameTable.m_TableFrameSink.m_GameLogic.SwitchToCardIndex(card[i]);
            bt[index]++;
        }
        uint[] info = MainRoot._pMJGameTable.GetSelfPengGang();
        Mahjong.tagWeaveItem[] tagInfo = new Mahjong.tagWeaveItem[info.Length / 2];
        for (int i = 0; i < tagInfo.Length; i += 1)
        {
            tagInfo[i] = new Mahjong.tagWeaveItem();
            tagInfo[i].cbCenterCard = (byte)info[i*2];
            tagInfo[i].cbWeaveKind = (byte)(info[i*2 + 1] == 0 ? Mahjong.CMD_SXMJ.WIK_PENG : Mahjong.CMD_SXMJ.WIK_GANG);
        }
        System.UInt32[] showTingPai = MainRoot._pMJGameTable.m_TableFrameSink.GetSuspensionData(bt, tagInfo, (byte)tagInfo.Length);
        return showTingPai;
    }
    /// <summary>
    /// 检测停牌按钮，必须是牌打出去才检测
    /// </summary>
    public void CheckTingPaiBtnShow()
    {
        List<byte> tempCards = new List<byte>();
        for (int i = 0; i < myHandMaJiang.Count; i++)
        {
            tempCards.Add((byte)myHandMaJiang[i].WhoAmI);
        }
        if (choiceMaJiang)
        {
            tempCards.Add((byte)choiceMaJiang.WhoAmI);
        }
        System.UInt32[] showTingPai = GetTingPaiIfPutOut(tempCards.ToArray());
        if (showTingPai.Length > 0)
        {
            if (MainRoot._gGameRoomCenter.gGameRoom != null)
            {
                if (!MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.activeSelf)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.SetActive(true);
                }
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi == null)
                {
                    GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/TingPaiTishiUI"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi = temp.GetComponent<TingPaiTishiUI>();
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.Initial();
                    Vector3[] tparams = new Vector3[showTingPai.Length / 2];
                    for (int i = 0; i < tparams.Length; i++)
                    {
                        tparams[i] = new Vector3(showTingPai[i * 2], showTingPai[i * 2+1], MainRoot._pMJGameTable.GetRemainHuMaJiangCount((MAJIANG)showTingPai[i * 2]));
                    }
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTishi(tparams);
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.HideTingPaiTishi();
                }
                else
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.Initial();
                    Vector3[] tparams = new Vector3[showTingPai.Length / 2];
                    for (int i = 0; i < tparams.Length; i++)
                    {
                        tparams[i] = new Vector3(showTingPai[i * 2], showTingPai[i * 2 + 1], MainRoot._pMJGameTable.GetRemainHuMaJiangCount((MAJIANG)showTingPai[i * 2]));
                    }
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTishi(tparams);
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.HideTingPaiTishi();
                }
            }
        }
        else
        {
            if (MainRoot._gGameRoomCenter.gGameRoom != null)
            {
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.ShowTingPaiTishi(new Vector3[1] { Vector3.zero }, true);
                }
                if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.activeSelf)
                {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gHuPaiBtn.SetActive(false);
                }
            }
        }
    }
    /// <summary>
    /// 所有手牌取消选中
    /// </summary>
    public void AllHandMaJiangDown()
    {
        for (int i = 0; i < myHandMaJiang.Count; i++)
        {
            if (myHandMaJiang[i].Selected)
            {
                myHandMaJiang[i].Selected = false;
            }
        }
        if (choiceMaJiang)
        {
            choiceMaJiang.Selected = false;
        }
    }
}
