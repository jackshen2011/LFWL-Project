using UnityEngine;
using System.Collections;

class DaoJuAddUICtrl : MonoBehaviour
{
    /// <summary>
    /// DaoJuSprite[0] -> 点赞/啤酒.
    /// DaoJuSprite[1] -> 牛粪.
    /// DaoJuSprite[2] -> 鲜花.
    /// DaoJuSprite[3] -> 拖鞋.
    /// </summary>
    public Sprite[] DaoJuSprite;
    /// <summary>
    /// DaoJuIndex 道具类型.
    /// DaoJuIndex == 0 -> 点赞/啤酒.
    /// DaoJuIndex == 1 -> 牛粪.
    /// DaoJuIndex == 2 -> 鲜花.
    /// DaoJuIndex == 3 -> 拖鞋.
    /// </summary>
    int DaoJuIndex = 0;
    PlayerBase mPlayerBase;
    /// <summary>
    /// 初始化添加道具的信息.
    /// </summary>
    public void InitDaoJuAddUI(int indexVal, PlayerBase playerBase)
    {
        Debug.Log("InitDaoJuAddUI -> indexVal " + indexVal);
        Debug.Log("Send msg to server port add daoJu to player!");
        DaoJuIndex = indexVal;
        mPlayerBase = playerBase;
        SystemMsgText sysMsg = GetComponent<SystemMsgText>();
        sysMsg.gMsgPng.sprite = DaoJuSprite[indexVal];
    }
}