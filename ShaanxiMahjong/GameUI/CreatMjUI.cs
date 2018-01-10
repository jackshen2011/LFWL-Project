using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RoomCardNet;

class CreatMjUI : UnModalUIBase
{
	/// <summary>
	/// 开关组的类型
	/// </summary>
	public enum ToggleType
	{
		JUSHU,
		FANGZUOBI,
		WANFA,
		PAIXING,
		XIAPAO,
		KEXUAN,
		ZHUANGJIA,
		SUANFEN,
		HUANGZHUANG,
		ZIMO,
		SHIXIAN,
		TESE,
		DIFEN
	}

	public List<ToggleGroupBase> ToggleGroupList = new List<ToggleGroupBase>();

	private string sFangZhuName;
	public TextBase gRoomTitleText;
	public TextBase gFangKaCountText;
	private object[] nOptionParam;
	public ScrollRectBase pShowOpSR;
	public GameObject pOptionShow_ShannXi;
	public GameObject pOptionShow_ErRen;
	public GameObject pOptionShow_Cur;
	public float smoth, targetPos;
	public ButtonBase pAdvBtn;
	public ButtonBase pDefBtn;

	public GameObject gErRenBtn;	//二人麻将按钮
	/// <summary>
	/// 1:陕西麻将 ，2:二人麻将
	/// </summary>
	public int nMJType = 1;
	/// <summary>
	/// 初始化创建麻将馆界面
	/// </summary>
	public void Initial(string sName)
	{
		sFangZhuName = sName;
		gRoomTitleText.text = sName + "的麻将馆";
		gFangKaCountText.text = string.Format("已有房卡X{0}", MainRoot._gPlayerData.nFangKaCount.ToString());
		pOptionShow_Cur = pOptionShow_ShannXi;

		if (SystemSetManage.AuditVersion.IsCopyRightAudit || SystemSetManage.AuditVersion.IsIOSAudit || SystemSetManage.AuditVersion.IsMyAppAudit)
		{
			if (gErRenBtn!=null)
			{
				gErRenBtn.SetActive(false);
			}
		}
	}
	/// <summary>
	/// 点击了创建房间按钮
	/// </summary>
	public void ClickCreatRoomButton()
	{
		if (nMJType == 1)   //陕西麻将
		{
			nOptionParam = new object[ToggleGroupList.Count + 2];
			for (int i = 0; i < ToggleGroupList.Count; i++)
			{
				if (!ToggleGroupList[i].IsActive())
				{
					nOptionParam[i] = (object)0;
					continue;
				}
				nOptionParam[i] = (object)ToggleGroupList[i].GetToggleGroupStat();
				if ((int)nOptionParam[i] == -1
					&&
					(
					 i == (int)ToggleType.KEXUAN
					))
				{
					nOptionParam[i] = 0;
				}
			}
			nOptionParam[ToggleGroupList.Count] = (object)MainRoot._gPlayerData.sUserName;
			nOptionParam[ToggleGroupList.Count + 1] = (object)MainRoot._gPlayerData.sUserName + "的麻将馆";
			if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //创建房卡麻将房间
			{
				RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCreateCardRoom(nOptionParam);
				if (MainRoot._gRoomData != null)
				{
					MainRoot._gRoomData.cCurRoomData = new OneRoomData();
					MainRoot._gRoomData.cCurRoomData.sRoomOwnerName = MainRoot._gPlayerData.sUserName;
					MainRoot._gRoomData.cCurRoomData.eRoomType = OneRoomData.RoomType.RoomType_RoomCard;
					MainRoot._gRoomData.cCurRoomData.nCurRound = 1;
					MainRoot._gRoomData.cCurRoomData.nMaxRound = (int)nOptionParam[0];
				}
				DestroyThis();
			}
		}
		else if (nMJType == 2)  //二人麻将
		{
			nOptionParam = new object[1];
			nOptionParam[0] = 1;
			if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)  //创建二人麻将房间
			{
				MainRoot._gRoomData.cCurRoomData = new OneRoomData();
				MainRoot._gRoomData.cCurRoomData.sRoomOwnerName = MainRoot._gPlayerData.sUserName;
				MainRoot._gRoomData.cCurRoomData.eRoomType = OneRoomData.RoomType.RoomType_TwoHumanRoom;
				MainRoot._gRoomData.cCurRoomData.nCurRound = 1;
				MainRoot._gRoomData.cCurRoomData.nMaxRound = 1;
				RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCreateErRenRoom(nOptionParam);
				DestroyThis();
			}
		}
	}
	/// <summary>
	/// 点击高级设置按钮
	/// </summary>
	public void OnDefaultOptionBtnClick()
	{
		pAdvBtn.gameObject.SetActive(true);
		pDefBtn.gameObject.SetActive(false);
		((RectTransform)pOptionShow_Cur.transform).anchoredPosition = new Vector2(((RectTransform)pOptionShow_Cur.transform).anchoredPosition.x, 0.0f);
		//targetPos = 0.5f;
		//pShowOpSR.verticalNormalizedPosition = targetPos;// Mathf.Lerp(pShowOpSR.horizontalNormalizedPosition, targetPos, Time.deltaTime * smoth);

	}
	/// <summary>
	/// 点击收起设置按钮
	/// </summary>
	public void OnAdvanceOptionBtnClick()
	{
		pDefBtn.gameObject.SetActive(true);
		pAdvBtn.gameObject.SetActive(false);
		((RectTransform)pOptionShow_Cur.transform).anchoredPosition = new Vector2(((RectTransform)pOptionShow_Cur.transform).anchoredPosition.x, 761.0f);

		//targetPos = 780.0f;
		//pShowOpSR.verticalNormalizedPosition = targetPos;// Mathf.Lerp(pShowOpSR.horizontalNormalizedPosition, targetPos, Time.deltaTime * smoth);

	}
	public void OnShannXiMjTypeSelect(ToggleBase pTog)
	{
		pOptionShow_ShannXi.SetActive(pTog.isOn);
		if (pTog.isOn)
		{
			nMJType = 1;
			pShowOpSR.content = (RectTransform)pOptionShow_ShannXi.transform;
			pOptionShow_Cur = pOptionShow_ShannXi;
		}
		Debug.Log("Unity:" + "OnShannXiMjTypeSelect:" + pTog.isOn.ToString());
	}
	public void OnErRenMJTypeSelect(ToggleBase pTog)
	{
		pOptionShow_ErRen.SetActive(pTog.isOn);
		if (pTog.isOn)
		{
			nMJType = 2;
			pShowOpSR.content = (RectTransform)pOptionShow_ErRen.transform;
			pOptionShow_Cur = pOptionShow_ErRen;
		}
		Debug.Log("Unity:" + "OnErRenMJTypeSelect:" + pTog.isOn.ToString());
	}
	public void OnCloseBtnClick()
	{
		DestroyThis();
	}
	// Use this for initialization
	void Start ()
	{
	
	}
	
	// Update is called once per frame
	void Update ()
	{
	}
}
