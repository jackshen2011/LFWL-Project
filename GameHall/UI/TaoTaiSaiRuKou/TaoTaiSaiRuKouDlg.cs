using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class TaoTaiSaiRuKouDlg : MonoBehaviour
{
    public class PlayerDt
    {
        public int UserId;
        public string UserName;
        public string HeadUrl;
        public int UserSex;
        public string GroupNum; //淘汰赛面板编号.
    }
    /// <summary>
    /// 存储淘汰赛面板60个单元的玩家Id信息.
    /// </summary>
    List<PlayerDt> PlayerDtIdList = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛32强的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_32 = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛32强A组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_32A = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛32强B组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_32B = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛16强A组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_16A = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛16强B组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_16B = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛8强A组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_8A = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛8强B组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_8B = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛4强A组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_4A = new List<PlayerDt>();
    /// <summary>
    /// 存储淘汰赛4强B组的数据链表.
    /// </summary>
    List<PlayerDt> PlayerDtList_4B = new List<PlayerDt>();
    int mDaoJiShiVal = 0; //倒计时
    float TimeLastVal = 0f;
    /// <summary>
    /// 服务器当前时间
    /// </summary>
    DateTime mServerTime;
    /// <summary>
    /// 提示信息数组.
    /// TipsObjArray[x]: 0 时间未到, 1 比赛准备中, 2 比赛已开始.
    /// </summary>
    public GameObject[] TipsObjArray;
    /// <summary>
    /// 晋级信息Tip1
    /// </summary>
    public TextBase[] TxJinJiTip1;
    /// <summary>
    /// 时间信息Tip1
    /// </summary>
    public TextBase[] TxTimeTip1;
    /// <summary>
    /// 时间信息Tip2
    /// </summary>
    public TextBase TxTimeTip2;
    /// <summary>
    /// 玩家信息列表父级.
    /// </summary>
    public Transform PlayerInfoListTr;
    /// <summary>
    /// 比赛准备文本对象.
    /// </summary>
    //public GameObject BiSaiZhunBeiObj;
    /// <summary>
    /// 活动详情对象.
    /// </summary>
    GameObject XingQingObj;
    /// <summary>
    /// 比赛场次img.
    /// </summary>
    //public ImageBase BiSaiChangCiImg;
    /// <summary>
    /// 比赛场次Sprite.
    /// </summary>
    //public Sprite[] BiSaiChangCiSpAy;
    /// <summary>
    /// 淘汰赛对话框组件.
    /// </summary>
    EnsureDlg TaoTaiSaiDlg;
    /// <summary>
    /// 比赛状态(0 比赛时间未到, 1 海选赛开始, 2 小组赛准备, 3 小组赛开始)
    /// </summary>
    OneRoomData.ThemeRaceState mBiSaiState = OneRoomData.ThemeRaceState.ShiJianWeiDao;
    /// <summary>
    /// 淘汰赛面板编号信息列表.
    /// </summary>
    //TextBase[] TaoTaiSaiPlayerTxAy = new TextBase[60];
    /// <summary>
    /// 淘汰赛面板32强A组编号置灰列表.
    /// </summary>
    [HideInInspector]
    public GameObject[] TaoTaiSaiZhiHuiAy_32A = new GameObject[16];
    /// <summary>
    /// 淘汰赛面板32强B组编号置灰列表.
    /// </summary>
    [HideInInspector]
    public GameObject[] TaoTaiSaiZhiHuiAy_32B = new GameObject[16];
    /// <summary>
    /// 淘汰赛面板16强A组编号置灰列表.
    /// </summary>
    [HideInInspector]
    public GameObject[] TaoTaiSaiZhiHuiAy_16A = new GameObject[8];
    /// <summary>
    /// 淘汰赛面板16强B组编号置灰列表.
    /// </summary>
    [HideInInspector]
    public GameObject[] TaoTaiSaiZhiHuiAy_16B = new GameObject[8];
    /// <summary>
    /// 淘汰赛面板8强A组编号置灰列表.
    /// </summary>
    [HideInInspector]
    public GameObject[] TaoTaiSaiZhiHuiAy_8A = new GameObject[4];
    /// <summary>
    /// 淘汰赛面板8强B组编号置灰列表.
    /// </summary>
    [HideInInspector]
    public GameObject[] TaoTaiSaiZhiHuiAy_8B = new GameObject[4];
    bool IsFillDt = false;
    /// <summary>
    /// 比赛轮数信息.
    /// </summary>
    int mBiSaiChangCi = 0;
    OneRoomData.DataDefine_MultiRaceType eMultiRaceType;
    /// <summary>
    /// 初始化淘汰赛入口界面信息.
    /// </summary>
    public void InitTaoTaiSaiDlg(int biSaiState, int changCi, OneRoomData.DataDefine_MultiRaceType multiRaceType)
    {
        eMultiRaceType = multiRaceType;
        Invoke("OnClickXiangQingBt", 3f);
        mBiSaiChangCi = changCi;
        TaoTaiSaiDlg = GetComponent<EnsureDlg>();
        TaoTaiSaiDlg.btn2.SetActive(false);
        //XingQingObj.SetActive(false);
        //BiSaiZhunBeiObj.SetActive(false);
        mBiSaiState = (OneRoomData.ThemeRaceState)biSaiState;
        switch (mBiSaiState)
        {
            case OneRoomData.ThemeRaceState.ShiJianWeiDao:
            case OneRoomData.ThemeRaceState.HaiXuanStart:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiWeiKaiShi();
                    break;
                }
            case OneRoomData.ThemeRaceState.GroupZhunBei:
                {
                    //BiSaiZhunBeiObj.SetActive(true);
                    //TaoTaiSaiDlg.btn2.SetActive(true);
                    break;
                }
            case OneRoomData.ThemeRaceState.GroupStart:
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiYiKaiShi();
                    break;
                }
        }

        Transform tr = null;
        for (int i = 0; i < PlayerInfoListTr.childCount; i++)
        {
            tr = PlayerInfoListTr.GetChild(i);
            tr.GetChild(1).GetComponent<TextBase>().text = "";
            if (i >= 32)
            {
                tr.GetChild(3).gameObject.SetActive(true);
            }
        }
        //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShouJiHaoYanZhengDlg(); //test
        //if (biSaiChangCi < 0)
        //{
        //    biSaiChangCi = 0;
        //}
        //BiSaiChangCiImg.sprite = BiSaiChangCiSpAy[biSaiChangCi];
        //ShowTaoTaiSaiPlayerInfo(); //test.
    }

    /// <summary>
    /// 显示服务端返回的淘汰赛界面玩家数据信息.
    /// args 淘汰赛玩家信息[x+1, x+60] 玩家id, [x+60+1, x+60+1+(4*32)] 玩家详细信息(id, name, url, sex).
    /// </summary>
    public void ShowTaoTaiSaiPlayerInfo(object[] args = null)
    {
        if (IsFillDt)
        {
            return;
        }
        IsFillDt = true;
        //玩家Id在淘汰赛里可以找到,并且没有被淘汰时,显示开始比赛按键.
        bool isShowStartBt = false;
        int userId = 0;
        //bool isBeiTaoTai = false; //是否被淘汰.
        Transform tr = null;
        //GameObject objZhiHui = null;
        string shouJiHaoVal = MainRoot._gPlayerData.TelInfo; //服务端获取手机号.
        bool isTaoTaiSaiXuanShou = false; //是否是淘汰赛选手.

        int taoTaiSaiChangCi = -1; //-1 比赛未开始, 0 第一场, *** 3 第四场.
        int startIndex = 2; //有效数据起始位置.
        ShowTipInfo(args);
        for (int i = 0; i < PlayerInfoListTr.childCount; i++)
        {
            tr = PlayerInfoListTr.GetChild(i);
            //TaoTaiSaiPlayerTxAy[i] = tr.GetComponentInChildren<TextBase>();

            PlayerDt playerDtVal = new PlayerDt();
            playerDtVal.UserId = (int)args[startIndex + i + 1];
            //test
            //if (i < 32)
            //{
            //    playerDtVal.UserId = i + 1;
            //}
            //else if (i < 48)
            //{
            //    if (i < 34)
            //    {
            //        playerDtVal.UserId = i + 1 - 32;
            //    }
            //    else if (i < 36)
            //    {
            //        playerDtVal.UserId = i + 1 - 32 + 2;
            //    }
            //    else if (i < 38)
            //    {
            //        playerDtVal.UserId = i + 1 - 32 + 2 + 2;
            //    }
            //    else if (i < 40)
            //    {
            //        playerDtVal.UserId = i + 1 - 32 + 2 + 2 + 2;
            //    }
            //}
            //else if (i < 56)
            //{
            //    if (i < 50)
            //    {
            //        playerDtVal.UserId = i + 1 - 48;
            //    }
            //    else if (i < 52)
            //    {
            //        playerDtVal.UserId = i + 1 - 48 + 6;
            //    }
            //}
            //else if (i < 60)
            //{
            //    if (i < 58)
            //    {
            //        playerDtVal.UserId = i + 1 - 56;
            //    }
            //}
            //test
            PlayerDtIdList.Add(playerDtVal);
            if (i < 32)
            {
                if (!isTaoTaiSaiXuanShou)
                {
                    isTaoTaiSaiXuanShou = playerDtVal.UserId == MainRoot._gPlayerData.nUserId ? true : false;
                }

                if (playerDtVal.UserId != 0 && taoTaiSaiChangCi != 0)
                {
                    taoTaiSaiChangCi = 0;
                }

                if (i < 16)
                {
                    PlayerDtList_32A.Add(playerDtVal);
                    TaoTaiSaiZhiHuiAy_32A[i] = tr.GetChild(2).gameObject;
                }
                else
                {
                    PlayerDtList_32B.Add(playerDtVal);
                    TaoTaiSaiZhiHuiAy_32B[i - 16] = tr.GetChild(2).gameObject;
                }
            }
            else if (i < 48)
            {
                if (playerDtVal.UserId != 0 && taoTaiSaiChangCi != 1)
                {
                    taoTaiSaiChangCi = 1;
                }

                if (i < 40)
                {
                    PlayerDtList_16A.Add(playerDtVal);
                    TaoTaiSaiZhiHuiAy_16A[i - 32] = tr.GetChild(2).gameObject;
                }
                else
                {
                    PlayerDtList_16B.Add(playerDtVal);
                    TaoTaiSaiZhiHuiAy_16B[i - 40] = tr.GetChild(2).gameObject;
                }
            }
            else if (i < 56)
            {
                if (playerDtVal.UserId != 0 && taoTaiSaiChangCi != 2)
                {
                    taoTaiSaiChangCi = 2;
                }

                if (i < 52)
                {
                    PlayerDtList_8A.Add(playerDtVal);
                    TaoTaiSaiZhiHuiAy_8A[i - 48] = tr.GetChild(2).gameObject;
                }
                else
                {
                    PlayerDtList_8B.Add(playerDtVal);
                    TaoTaiSaiZhiHuiAy_8B[i - 52] = tr.GetChild(2).gameObject;
                }
            }
            else if (i < 60)
            {
                if (playerDtVal.UserId != 0 && taoTaiSaiChangCi != 3)
                {
                    taoTaiSaiChangCi = 3;
                }

                if (i < 58)
                {
                    PlayerDtList_4A.Add(playerDtVal);
                }
                else
                {
                    PlayerDtList_4B.Add(playerDtVal);
                }
            }
        }

        for (int i = 0; i < 32; i++)
        {
            PlayerDt playerDtVal = new PlayerDt();
            //playerDtVal.UserId = i + 1; //test
            userId = (int)args[startIndex + 61 + (i * 4)];
            playerDtVal.UserId = userId;
            playerDtVal.UserName = (string)args[startIndex + 61 + (i * 4) + 1];
            playerDtVal.HeadUrl = (string)args[startIndex + 61 + (i * 4) + 2];
            playerDtVal.UserSex = (int)args[startIndex + 61 + (i * 4) + 3];
            //playerDtVal.GroupNum = TaoTaiSaiPlayerTxAy[i].text;
            PlayerDtList_32.Add(playerDtVal);
        }

        //userId = 0;
        for (int i = 0; i < PlayerInfoListTr.childCount; i++)
        {
            tr = PlayerInfoListTr.GetChild(i);
            userId = PlayerDtIdList[i].UserId;
            //Debug.Log("Unity: i == " + i); //test
            //if (i == 35)
            //{
            //    Debug.Log("Unity: i == " + i); //test
            //}

            if (i < 32)
            {
                if (userId != 0)
                {
                    PlayerDt pDt = PlayerDtList_32.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId.Equals(userId); });
                    if (pDt != null)
                    {
                        tr.GetChild(1).GetComponent<TextBase>().text = pDt.UserName.Length <= 6 ? pDt.UserName : pDt.UserName.Remove(6);
                        if (pDt.HeadUrl != null)
                        {
                            ImageBase photoicon = tr.GetChild(0).GetComponent<ImageBase>();
                            MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage(pDt.HeadUrl, photoicon);
                        }
                    }
                    else
                    {
                        Debug.LogWarning("Unity: pDt is null! userId == " + userId + ", i == " + i);
                    }
                }
            }
            else
            {
                if (userId != 0)
                {
                    PlayerDt pDt = PlayerDtList_32.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId.Equals(userId); });
                    if (pDt != null)
                    {
                        tr.GetChild(1).GetComponent<TextBase>().text = pDt.UserName.Length <= 6 ? pDt.UserName : pDt.UserName.Remove(6);
                        if (pDt.HeadUrl != null)
                        {
                            ImageBase photoicon = tr.GetChild(0).GetComponent<ImageBase>();
                            MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage(pDt.HeadUrl, photoicon);
                        }
                        tr.GetChild(3).gameObject.SetActive(false);
                        //TaoTaiSaiPlayerTxAy[i].text = pDt.GroupNum; //这里给32个之后的单元填充编号信息.
                    }
                    else
                    {
                        Debug.LogWarning("Unity: pDt is null! userId == " + userId + ", i == " + i);
                    }
                }
                else
                {
                    tr.GetChild(3).gameObject.SetActive(true); //显示胜者.
                    //TaoTaiSaiPlayerTxAy[i].text = "胜者";
                }
            }
        }
        CheckZhiHuiTaoTaiSai(taoTaiSaiChangCi);
        MainRoot._gRoomData.cCacheRoomData.nCurChangCi = taoTaiSaiChangCi;
        MainRoot._gRoomData.cCacheRoomData.nDiFen = 1;
        
        isShowStartBt = CheckIsShowStartBiSaiBt(mBiSaiChangCi);
        if (isShowStartBt != TaoTaiSaiDlg.btn2.activeInHierarchy)
        {
            TaoTaiSaiDlg.btn2.SetActive(isShowStartBt);
        }

        if (shouJiHaoVal == "" && isTaoTaiSaiXuanShou)
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnShouJiHaoYanZhengDlg();
        }
    }

    /// <summary>
    /// 检测是否显示开始比赛按键.
    /// taoTaiSaiChangCi: -1 比赛未开始, 0 第一场, *** 3 第四场.
    /// </summary>
    bool CheckIsShowStartBiSaiBt(int taoTaiSaiChangCi)
    {
        if (MainRoot._gPlayerData.nUserId == 0)
        {
            return false;
        }

        switch (taoTaiSaiChangCi)
        {
            case 0:
                {
                    PlayerDt rvPlayerDt = PlayerDtList_32A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }

                    rvPlayerDt = PlayerDtList_32B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    break;
                }
            case 1:
                {
                    PlayerDt rvPlayerDt = PlayerDtList_16A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (PlayerDtList_32A.Find(delegate (PlayerDt playerDtTmp) {
                    //        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    //    }).UserId == MainRoot._gPlayerData.nUserId)
                    //    {
                    //        return true;
                    //    }
                    //}

                    rvPlayerDt = PlayerDtList_16B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (PlayerDtList_32B.Find(delegate (PlayerDt playerDtTmp) {
                    //        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    //    }).UserId == MainRoot._gPlayerData.nUserId)
                    //    {
                    //        return true;
                    //    }
                    //}
                    break;
                }
            case 2:
                {
                    PlayerDt rvPlayerDt = PlayerDtList_8A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (PlayerDtList_16A.Find(delegate (PlayerDt playerDtTmp) {
                    //        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    //    }).UserId == MainRoot._gPlayerData.nUserId)
                    //    {
                    //        return true;
                    //    }
                    //}

                    rvPlayerDt = PlayerDtList_8B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (PlayerDtList_16B.Find(delegate (PlayerDt playerDtTmp) {
                    //        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    //    }).UserId == MainRoot._gPlayerData.nUserId)
                    //    {
                    //        return true;
                    //    }
                    //}
                    break;
                }
            case 3:
                {
                    PlayerDt rvPlayerDt = PlayerDtList_4A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (PlayerDtList_8A.Find(delegate (PlayerDt playerDtTmp) {
                    //        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    //    }).UserId == MainRoot._gPlayerData.nUserId)
                    //    {
                    //        return true;
                    //    }
                    //}

                    rvPlayerDt = PlayerDtList_4B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == MainRoot._gPlayerData.nUserId)
                    {
                        return true;
                    }
                    //else
                    //{
                    //    if (PlayerDtList_8B.Find(delegate (PlayerDt playerDtTmp) {
                    //        return playerDtTmp.UserId == MainRoot._gPlayerData.nUserId;
                    //    }).UserId == MainRoot._gPlayerData.nUserId)
                    //    {
                    //        return true;
                    //    }
                    //}
                    break;
                }
        }
        return false;
    }

    /// <summary>
    /// 检测是否显示置灰信息.
    /// taoTaiSaiChangCi: -1 比赛未开始, 0 第一场, *** 3 第四场.
    /// </summary>
    void CheckZhiHuiTaoTaiSai(int taoTaiSaiChangCi)
    {
        if (taoTaiSaiChangCi == -1)
        {
            return;
        }

        if (taoTaiSaiChangCi >= 3)
        {
            PlayerDt rvPlayerDtTmp = PlayerDtList_4A.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId != 0; });
            if (rvPlayerDtTmp != null && rvPlayerDtTmp.UserId != 0)
            {
                for (int i = 0; i < PlayerDtList_8A.Count; i++)
                {
                    PlayerDt rvPlayerDt = PlayerDtList_4A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == PlayerDtList_8A[i].UserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == PlayerDtList_8A[i].UserId)
                    {
                        //找到玩家id.
                    }
                    else
                    {
                        //在高一级房间没有找到玩家id.
                        TaoTaiSaiZhiHuiAy_8A[i].SetActive(true);
                    }
                }
            }

            rvPlayerDtTmp = PlayerDtList_4B.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId != 0; });
            if (rvPlayerDtTmp != null && rvPlayerDtTmp.UserId != 0)
            {
                for (int i = 0; i < PlayerDtList_8B.Count; i++)
                {
                    PlayerDt rvPlayerDt = PlayerDtList_4B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == PlayerDtList_8B[i].UserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == PlayerDtList_8B[i].UserId)
                    {
                        //找到玩家id.
                    }
                    else
                    {
                        //在高一级房间没有找到玩家id.
                        TaoTaiSaiZhiHuiAy_8B[i].SetActive(true);
                    }
                }
            }
        }

        if (taoTaiSaiChangCi >= 2)
        {
            PlayerDt rvPlayerDtTmp = PlayerDtList_8A.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId != 0; });
            if (rvPlayerDtTmp != null && rvPlayerDtTmp.UserId != 0)
            {
                for (int i = 0; i < PlayerDtList_16A.Count; i++)
                {
                    PlayerDt rvPlayerDt = PlayerDtList_8A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == PlayerDtList_16A[i].UserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == PlayerDtList_16A[i].UserId)
                    {
                        //找到玩家id.
                    }
                    else
                    {
                        //在高一级房间没有找到玩家id.
                        TaoTaiSaiZhiHuiAy_16A[i].SetActive(true);
                    }
                }
            }

            rvPlayerDtTmp = PlayerDtList_8B.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId != 0; });
            if (rvPlayerDtTmp != null && rvPlayerDtTmp.UserId != 0)
            {
                for (int i = 0; i < PlayerDtList_16B.Count; i++)
                {
                    PlayerDt rvPlayerDt = PlayerDtList_8B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == PlayerDtList_16B[i].UserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == PlayerDtList_16B[i].UserId)
                    {
                        //找到玩家id.
                    }
                    else
                    {
                        //在高一级房间没有找到玩家id.
                        TaoTaiSaiZhiHuiAy_16B[i].SetActive(true);
                    }
                }
            }
        }

        if (taoTaiSaiChangCi >= 1)
        {
            PlayerDt rvPlayerDtTmp = PlayerDtList_16A.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId != 0; });
            if (rvPlayerDtTmp != null && rvPlayerDtTmp.UserId != 0)
            {
                for (int i = 0; i < PlayerDtList_32A.Count; i++)
                {
                    PlayerDt rvPlayerDt = PlayerDtList_16A.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == PlayerDtList_32A[i].UserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == PlayerDtList_32A[i].UserId)
                    {
                        //找到玩家id.
                    }
                    else
                    {
                        //在高一级房间没有找到玩家id.
                        TaoTaiSaiZhiHuiAy_32A[i].SetActive(true);
                    }
                }
            }

            rvPlayerDtTmp = PlayerDtList_16B.Find(delegate (PlayerDt playerDtTmp) { return playerDtTmp.UserId != 0; });
            if (rvPlayerDtTmp != null && rvPlayerDtTmp.UserId != 0)
            {
                for (int i = 0; i < PlayerDtList_32B.Count; i++)
                {
                    //Debug.Log("Unity: i == " + i); //test.
                    PlayerDt rvPlayerDt = PlayerDtList_16B.Find(delegate (PlayerDt playerDtTmp)
                    {
                        return playerDtTmp.UserId == PlayerDtList_32B[i].UserId;
                    });

                    if (rvPlayerDt != null && rvPlayerDt.UserId == PlayerDtList_32B[i].UserId)
                    {
                        //找到玩家id.
                    }
                    else
                    {
                        //在高一级房间没有找到玩家id.
                        TaoTaiSaiZhiHuiAy_32B[i].SetActive(true);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 点击玩家信息按键.
    /// indexVal: 0-31 第一组, 32-47 第二组, 48-55 第三组, 56-59 第四组.
    /// </summary>
    public void OnClickPlayerDtBt(int indexVal)
    {
        //Debug.Log("Unity: OnClickPlayerDtBt -> indexVal " + indexVal);
        bool isUsed = false;
        if (isUsed)
        {
            return;
        }

        if (PlayerDtIdList[indexVal].UserId == 0)
        {
            return;
        }
        Vector2 startPos = Vector2.zero;
        GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/TaoTaiSaiRuKou/UserDetailsInfo-TaoTaiSai"),
                            transform.parent, false);
        UserDetailsInfo userInfoCom = obj.GetComponent<UserDetailsInfo>();
        userInfoCom.InitialTaoTaiSaiUserInfo(PlayerDtList_32.Find(delegate (PlayerDt playerDtTmp) {
            return playerDtTmp.UserId == PlayerDtIdList[indexVal].UserId;
        }));

        RectTransform rectTr = PlayerInfoListTr.GetChild(indexVal).GetComponent<RectTransform>();
        userInfoCom.SetTaoTaiSaiUserInfoPos(rectTr.anchoredPosition);
    }

    /// <summary>
    /// 点击活动详情按键.
    /// </summary>
    public void OnClickXiangQingBt()
    {
        if (XingQingObj != null)
        {
            return;
        }
        XingQingObj = (GameObject)Instantiate(Resources.Load("Prefab/HaiXuanRuKou/ED-ThemeRaceXiangQing"), transform.parent.transform, false);
		EnsureDlg dlg = XingQingObj.GetComponent<EnsureDlg>();
		dlg.Initial(EnsureDlg.EnsureKind.ThemeRaceXiangQing);
		dlg.p_ShowTextA.text = MainRoot._gPlayerData.sThemeRace_HuoDongXiangQing;
	}

	/// <summary>
	/// 点击详情关闭按键.
	/// </summary>
	public void OnClickXiangQingCloseBt()
    {
        //XingQingObj.SetActive(false);
    }

    /// <summary>
    /// 点击开始比赛按键.
    /// </summary>
    public void OnClickStartBiSaiBt()
    {
        Debug.Log("Unity: OnClickStartBiSaiBt");
        if (MainRoot._gPlayerData.relinkUserRoomData != null)
        {//说明玩家有未完成的牌局
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnLoginPaiJuWeiWanChengDlg();//点击请求重新连入牌局
            return;
        }

        //这里添加向服务端发送点击淘汰赛界面开始比赛按键消息.
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallTaoTaiSaiClickStartBiSaiBt(0, eMultiRaceType);
        }
    }

    /// <summary>
    /// 收到服务端开始比赛按键的返回消息.
    /// arg: 0 比赛尚未开始, 1 进入比赛, 2 比赛已开始,系统视为自动退赛.
    /// </summary>
    public void OnReceivedStartBtMsg(int arg)
    {
        OneRoomData.ThemeRace_GroupState state = (OneRoomData.ThemeRace_GroupState)arg;
        switch (state)
        {
            case OneRoomData.ThemeRace_GroupState.Game_NotStart: //比赛尚未开始.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiWeiKaiShi();
                    break;
                }
            case OneRoomData.ThemeRace_GroupState.Game_Ready: //进入比赛.
                {
                    MakePlayerIntoTaoTaiSai();
                    break;
                }
            case OneRoomData.ThemeRace_GroupState.Game_Runing: //比赛已开始,系统视为自动退赛.
                {
                    //MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnSMBiSaiYiKaiShi_TuiSai();
                    MakePlayerIntoTaoTaiSai();
                    break;
                }
        }
    }

    /// <summary>
    /// 使玩家进入淘汰赛游戏场景.
    /// </summary>
    public void MakePlayerIntoTaoTaiSai()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "ShaanxiMahjong")
        {
            MainRoot._gRoomData.cCacheRoomData.eRoomType = OneRoomData.RoomType.RoomType_ThemeRace_Group;
            MainRoot._gRoomData.cCacheRoomData.eMultiRaceType = eMultiRaceType;
            MainRoot._gRoomData.cCacheRoomData.nMaxRound = 8;
            MainRoot._gMainRoot.ChangeScene("ShaanxiMahjong"); //直接创建淘汰赛房间，进入游戏场景.
        }
    }

    /// <summary>
    /// 显示提示信息.
    /// </summary>
    void ShowTipInfo(object[] args)
    {
        int JinJiA = 32;
        int JinJiB = 16;
        DateTime raceTime = (DateTime)args[191];
        MainRoot._gRoomData.cCacheRoomData.TimeBiSaiStart = raceTime;
        mServerTime = (DateTime)args[192];
        foreach (var item in TipsObjArray)
        {
            item.SetActive(false);
        }

        switch (mBiSaiChangCi)
        {
            case 1:
                {
                    JinJiA = 16;
                    JinJiB = 8;
                    break;
                }
            case 2:
                {
                    JinJiA = 8;
                    JinJiB = 4;
                    break;
                }
        }

        switch (mBiSaiState)
        {
            case OneRoomData.ThemeRaceState.GroupStart:
                {
                    //TipsObjArray[2].SetActive(true);
                    break;
                }
            case OneRoomData.ThemeRaceState.GroupZhunBei:
                {
                    mDaoJiShiVal = (((raceTime.Hour - mServerTime.Hour) * 60 + (raceTime.Minute - mServerTime.Minute)) * 60) + (60 - mServerTime.Second);
                    TxTimeTip2.text = mDaoJiShiVal.ToString();
                    TipsObjArray[1].SetActive(true);
                    break;
                }
            case OneRoomData.ThemeRaceState.ShiJianWeiDao:
                {
                    TipsObjArray[0].SetActive(true);
                    TxJinJiTip1[0].text = JinJiA.ToString();
                    TxJinJiTip1[1].text = JinJiB.ToString();
                    TxTimeTip1[0].text = raceTime.Month.ToString();
                    TxTimeTip1[1].text = raceTime.Day.ToString();

                    mDaoJiShiVal = -1;
                    if (raceTime.Day == mServerTime.Day)
                    {
                        mDaoJiShiVal = (((raceTime.Hour - mServerTime.Hour) * 60 + (raceTime.Minute - mServerTime.Minute)) * 60) + (60 - mServerTime.Second);
                    }
                    break;
                }
        }
        TimeLastVal = Time.time;
    }

    void Update()
    {
        UpdateTipState();
        UpdateTipDaoJiShi();
    }

    /// <summary>
    /// 更新提示信息的显示状态.
    /// </summary>
    void UpdateTipState()
    {
        if (!TipsObjArray[0].activeInHierarchy)
        {
            return;
        }

        if (mDaoJiShiVal <= -1)
        {
            return;
        }

        if (Time.time - TimeLastVal < 1f)
        {
            return;
        }
        TimeLastVal = Time.time;
        mDaoJiShiVal--;
        
        if (mDaoJiShiVal <= 600)
        {
            //比赛进入准备阶段.
            TipsObjArray[0].SetActive(false);
            TipsObjArray[1].SetActive(true);

            bool isShowStartBt = CheckIsShowStartBiSaiBt(mBiSaiChangCi);
            if (isShowStartBt != TaoTaiSaiDlg.btn2.activeInHierarchy)
            {
                TaoTaiSaiDlg.btn2.SetActive(isShowStartBt);
            }
        }
    }

    /// <summary>
    /// 更新提示信息的倒计时.
    /// </summary>
    void UpdateTipDaoJiShi()
    {
        if (!TipsObjArray[1].activeInHierarchy)
        {
            return;
        }

        if (Time.time - TimeLastVal < 1f)
        {
            return;
        }
        TimeLastVal = Time.time;

        mDaoJiShiVal--;
        if (mDaoJiShiVal > 86400)
        {
            TxTimeTip2.text = "比赛还有 " + (mDaoJiShiVal / 86400).ToString() + " 天";
        }
        else if (mDaoJiShiVal > 3600)
        {
            TxTimeTip2.text = "比赛还有 " + (mDaoJiShiVal / 3600).ToString() + " 小时";
        }
        else if (mDaoJiShiVal > 600)
        {
            TxTimeTip2.text = "比赛还有 " + (mDaoJiShiVal / 60).ToString() + " 分钟";
        }
        else
        {
            TxTimeTip2.text = "比赛还有 " + mDaoJiShiVal.ToString() + " 秒";
        }

        if (mDaoJiShiVal <= 0)
        {
            //比赛已开始.
            TipsObjArray[1].SetActive(false);
            //TipsObjArray[2].SetActive(true);
        }
    }
}