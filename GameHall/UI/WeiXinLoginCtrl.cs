using UnityEngine;
using System.Collections;
/// <summary>
/// 微信登陆游戏对话框.
/// </summary>
class WeiXinLoginCtrl : UnModalUIBase
{
    public GameObject DuiGouObj;
    public GameObject LoginBtObj;
    void Start()
    {
        //LoginBtObj.SetActive(false);
#if UNITY_EDITOR
        EnsureDlg dlg = GetComponent<EnsureDlg>();
        dlg.btn3.SetActive(true); //在编辑模式下显示返回按键. 
#endif

        DuiGouObj.SetActive(true);
    }
	public void InitialTips()
	{
		MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnDengLuLianJieZhong();
	}
	public void ShowDuiGou()
    {
        DuiGouObj.SetActive(!DuiGouObj.activeInHierarchy);
    }
}