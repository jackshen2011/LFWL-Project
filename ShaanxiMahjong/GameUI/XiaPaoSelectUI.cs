using UnityEngine;
using System.Collections;
using RoomCardNet;

/// <summary>
/// 下炮选择界面
/// </summary>
class XiaPaoSelectUI : UnModalUIBase
{

    public void OnXiaPaoSelectBtnClick(int n)
    {
        OnSysMsgWaitEventHappen(3);
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(12, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//自己下炮
        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallGameStartReadyOK((byte)n);
        }
        Debug.Log("Unity:"+"OnXiaPaoSelectBtnClick:"+n.ToString());
    }
	public void OnXiaPaoTimeOverAutoSelect()
	{
		OnSysMsgWaitEventHappen(3);
		if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
		{
			RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallGameStartReadyOK((byte)0);
		}
		Debug.Log("Unity:" + "OnXiaPaoTimeOverAutoSelect:0");
	}
	public void Initial()
    {
        nUnModalUIIndex = MainRoot._gUIModule.pUnModalUIControl.GetOneUnModalUIIndex();
        MainRoot._gUIModule.pUnModalUIControl.JustShowMeOnly(this);
    }
	public override void OnSysMsgWaitEventHappen(int n)
	{
		base.OnSysMsgWaitEventHappen(n);
		DestroyThis();
	}

	// Use this for initialization
	void Start ()
    {

	}

    // Update is called once per frame
    void Update () {
	
	}
}
