using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// 问题:
/// 1.点击分享按键后是否关闭高级结算界面.
/// 2.点击退出房间按键后是否游戏返回到第一个选择房间或秦人麻将的游戏场景.
/// </summary>
class GaoJiJieSuanCtrl : UnModalUIBase
{
    /// <summary>
    /// 排名信息数据.
    /// </summary>
    class PaiMingDt
    {
        public int playerId;
        public int playerScore;
        public DateTime TimeTaoTai; //淘汰赛玩家得到积分的时间.
    };
    public Transform GridGroupTr;
    /// <summary>
    /// 淘汰赛高级结算界面名称文本对象.
    /// </summary>
    public GameObject TaoTaiSaiTitleTxObj;
    /// <summary>
    /// TrArray[0] -> 起点.
    /// TrArray[1] -> 终点.
    /// </summary>
    public Transform[] TrArray;
    List<PaiMingInfoCtrl> PaiMingInfoList = new List<PaiMingInfoCtrl>();
    /// <summary>
    /// 分享战绩的材质.
    /// </summary>
    public Material ShareZhanJiMat;
    /// <summary>
    /// 截取战绩图片.
    /// </summary>
    public void CaptureUIScreenshotByPoint()
    {
        Vector3 startPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(TrArray[0].position);
        Vector3 endPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(TrArray[1].position);
        MainRoot._gUIModule.CaptureScreenshotZhanJi(startPos, endPos, ShareZhanJiMat);
    }
    /// <summary>
    /// 初始化高级结算的界面信息.
    /// </summary>
    public void InitGaoJiJieSuanInfo(object[] args = null)
    {
        if (MainRoot._gPlayerData.relinkUserRoomData != null)
        {
            MainRoot._gPlayerData.relinkUserRoomData = null; //清除当前牌局信息.
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pChiPengHuTips.DestroyThis();
        }

        if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi != null)
        {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.tptishi.DestroyThis();
        }

        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
        {
            TaoTaiSaiTitleTxObj.SetActive(true);
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetActiveStartGameBt(false);
        }
        else
        {
            TaoTaiSaiTitleTxObj.SetActive(false);
        }
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.DestroySMDengDaiChuPai();
        MainRoot._gRoomData.cCurRoomData.eRoomState = OneRoomData.RoomStat.COLSE;
        PaiMingInfoList.Clear();
        for (int i = 0; i < 4; i++)
		{
			if (((int)(args[7 + i * 4])) != 0)
			{
				SpawnPaiMingInfoObj(GridGroupTr);
			}
        }
        SetGaoJiJieSuanPlayerInfo(args);
    }
    int CompareByScore(PaiMingDt x, PaiMingDt y)//从大到小排序器.
    {
        if (x == null)
        {
            if (y == null)
            {
                return 0;
            }
            return 1;
        }
        if (y == null)
        {
            return -1;
        }
        int retval = y.playerScore.CompareTo(x.playerScore);

        if (retval == 0 && MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
        {
            //淘汰赛时玩家积分相同则按照获得积分的时间排序.
            retval = x.TimeTaoTai.CompareTo(y.TimeTaoTai);
        }
        return retval;
    }
    /// <summary>
    /// 设置高级结算界面的排名信息.
    /// </summary>
    public void SetGaoJiJieSuanPlayerInfo(object[] args = null)
    {
        bool isFangZhu = false;
        Sprite headImg = null;
        PaiMingDt paiMingDtTmp = null;
        List<PaiMingDt> paiMingDtList = new List<PaiMingDt>();
        for (int i = 0; i < 4; i++)
        {
			if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
			{
				if (((int)(args[7 + i * 4])) != 0)
				{
					paiMingDtTmp = new PaiMingDt();
					paiMingDtTmp.playerId = (int)(args[7 + i * 4]);
					paiMingDtTmp.playerScore = (int)(args[10 + i * 4]);
					paiMingDtList.Add(paiMingDtTmp);
				}
			}
			else
			{ 
				paiMingDtTmp = new PaiMingDt();
				paiMingDtTmp.playerId = (int)(args[7 + i * 4]);
				paiMingDtTmp.playerScore = (int)(args[10 + i * 4]);
                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                {
                    //paiMingDtTmp.TimeTaoTai = (DateTime)(args[10 + i * 4]); //淘汰赛得积分时间.
                }
				paiMingDtList.Add(paiMingDtTmp);
			}
        }
        paiMingDtList.Sort(CompareByScore); //按照分数从大到小排序.

        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < 4; i++)
            {

				if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
				{
					if ((int)(args[7 + i * 4])!=0 && j< paiMingDtList.Count)
					{
						if (paiMingDtList[j].playerId == (int)(args[7 + i * 4]))
						{
							isFangZhu = (int)args[2] == (int)(args[7 + i * 4]) ? true : false;
							headImg = MainRoot._gPlayerData.pAsyncImageDownload.GetPlayerWXHeadImg((int)(args[7 + i * 4]));
							PaiMingInfoList[j].SetPaiMingInfo(j + 1, (string)(args[8 + i * 4]), headImg, (int)args[6], (int)(args[9 + i * 4]), (int)(args[10 + i * 4]), isFangZhu);
							break;
						}
					}
				}
				else
				{
					if (paiMingDtList[j].playerId == (int)(args[7 + i * 4]))
					{
						isFangZhu = (int)args[2] == (int)(args[7 + i * 4]) ? true : false;
						headImg = MainRoot._gPlayerData.pAsyncImageDownload.GetPlayerWXHeadImg((int)(args[7 + i * 4]));
						PaiMingInfoList[j].SetPaiMingInfo(j + 1, (string)(args[8 + i * 4]), headImg, (int)args[6], (int)(args[9 + i * 4]), (int)(args[10 + i * 4]), isFangZhu);

                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                        {
                            if (MainRoot._gPlayerData.nUserId == (int)(args[7 + i * 4]))
                            {
                                if (MainRoot._gRoomData.cCurRoomData.IsZongJueSaiRoom)
                                {
                                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(j + 81, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //恭喜您已经获得了本次大赛的冠军/亚军/季军/殿军.
                                }
                                else
                                {
                                    if (j < 2)
                                    {
                                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(79, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //已晋级.
                                    }
                                    else
                                    {
                                        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(80, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //未晋级.
                                    }
                                }
                            }
                        }
                        break;
					}
				}
            }
        }
    }

    /// <summary>
    /// 产生排名信息对象.
    /// </summary>
    void SpawnPaiMingInfoObj(Transform tr = null)
    {
        Transform parentTr = tr != null ? tr : MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform;
        GameObject test = (GameObject)Instantiate(Resources.Load("Prefab/GaoJiJieSuanUI/PaiMingInfo"), parentTr, false);
        PaiMingInfoList.Add(test.GetComponent<PaiMingInfoCtrl>());
    }

    #if UNITY_EDITOR
    void OnGUI()
    {
        Vector3 startPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(TrArray[0].position);
        Vector3 endPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(TrArray[1].position);
        GUI.Box(new Rect(startPos.x, startPos.y, 10f, 10f), "1");
        GUI.Box(new Rect(endPos.x, endPos.y, 10f, 10f), "2");
    }
    #endif
}