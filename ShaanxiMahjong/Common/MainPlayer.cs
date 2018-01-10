using UnityEngine;
using System.Collections;

/// <summary>
/// 牌桌上的自己
/// </summary>
class MainPlayer : PlayerBase
{
    //GameObject pInfoPanel;

    
    public override void LinkMyDelegate()  //必须实现将自己的函数挂到GameCenter委托上
    {
        //OtherPlayer::LinkMyDelegate();
        MainRoot._gGameRoomCenter.mytest += MainPlayerTestMsg;
    }

    void MainPlayerTestMsg(MsgInfo s)
    {
        Debug.Log("Unity:"+name+":"+s.message);
        isUse = true;
    }
    public void Initial()
    {
        playertype = PLAYERTYPE.MAIN_USER;
        //Debug.Log("Unity:"+playertype);
        LinkMyDelegate();
    }

    // Use this for initialization
    void Start ()
    {

    }
    void Awake()
    {
        
    }
	// Update is called once per frame
	void Update () {
	
	}
}
