using UnityEngine;
using System.Collections;
using System.Collections.Generic;


class InfoPanel
{
    public Vector3 panelVec;

}
/// <summary>
/// 牌桌上的其他玩家对象
/// </summary>
class OtherPlayer : PlayerBase
{
    public override void LinkMyDelegate()  //必须实现将自己的函数挂到GameCenter委托上
    {
        MainRoot._gGameRoomCenter.mytest += OtherPlayerTestMsg;
    }

    void OtherPlayerTestMsg(MsgInfo s)
    {
        Debug.Log("Unity:"+name + ":" + s.message);
        isUse = true;
    }
    public void Initial()
    {
        playertype = PLAYERTYPE.OTHER_USER;
        LinkMyDelegate();
    }
    //使用道具
    public override void OnUseItem(int ntype) 
    {
        Debug.Log("Unity:"+nUserId+":OnUseItem:"+ntype.ToString());
    }
}