using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 标准系统提示界面
/// </summary>
class SystemMsgText : UnModalUIBase
{
	/// <summary>
	/// 弃用
	/// </summary>
    public enum SystemMsgKind
    {
        DEFAULT,
        SHOW_TEXT,
        SHOW_TEXT_ANI,
        SHOW_TEXT_ANI_TIME,
        SHOW_PNG,
        SHOW_PNG_ANI,
        SHOW_PNG_ANI_TIME
    };
	//---------------以下弃用--------------------
	/// <summary>
	/// 提示类型和相关配置
	/// </summary>
	public SystemMsgKind MsgKind;

	public ImageBase gAni;
    public TextBase gmsgtext;
    public TextBase gtimertext;
    public ImageBase gMsgPng;
	//---------------以上弃用--------------------
	/// <summary>
	/// 标准弹出提示的信息存储类
	/// </summary>
	public class SysMsgInfo
	{
		/// <summary>
		/// 标准弹出提示的序号
		/// </summary>
		public int nIndex;
		/// <summary>
		/// 预制体路径
		/// </summary>
		public string sPath;
		/// <summary>
		/// 消失倒计时
		/// </summary>
		public float fTime;
		public SysMsgInfo(int n, string s, float t)
		{
			nIndex = n;
			sPath = s;
			fTime = t;
		}
	}
	/// <summary>
	/// 提示编号
	/// </summary>
    public int nIndex;
    public float showtime;  //显示时间
    public string msgsrc;      //显示的内容

	/// <summary>
	/// 如果显示时间为0，则为事件驱动，当发生特定事件，则执行此对象上的特定委托
	/// </summary>
    public UnModalUIBase pEventCaller;
    public Color ShuFenColor; //输分颜色.
    public Color YingFenColor; //赢分颜色.

	public bool bIsTimer = false;
	public bool bIsEvent = false;
    //-------------------声明委托-----------------
    //public delegate void SysMsgDelegate(int nIndex);
    //-------------------定义委托----------------

	/// <summary>
	/// 如果显示时间不为0，则为倒计时驱动，当时间结束，则执行此委托
	/// </summary>
    public SysMsgDelegate delegate_timeover;//时间结束回调

    // Use this for initialization
    void Start ()
    {
	    
	}
    public void Initial()
    {
		//MsgKind = SystemMsgKind.;
	}


	/*   public void SM_SetSystemMsgTextWithWaitEvent(int n, UnModalUIBase waitsctip)
	   {

	   }
	   public void SM_SetSystemMsgTextWithTimeOver(int n, float t,SysMsgDelegate timeoverfunc)
	   {

	   }
	   */
	/// <summary>
	/// 设定此提示的处理方式为事件驱动并初始化，当事件发生时，外部的waitsctip调用自己挂上去的特定引用进行通知
	/// </summary>
	/// <param name="n"></param>
	/// <param name="waitsctip"></param>
	/// <param name="param"></param>
	public void SM_SetSystemMsgTextWithWaitEvent(int n, UnModalUIBase waitsctip, object param = null)
    {
        try
        {
            if (param != null)
            {
                switch (n)
                {
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 41:
                        if (!gmsgtext.isActiveAndEnabled)
                        {
                            gmsgtext.gameObject.SetActive(true);
                        }
                        ShowPlayerShuYingFen((int)param);
                        break;
                    default:
                        break;
                }
            }

            nIndex = n;
            showtime = 0;
            pEventCaller = waitsctip;
            pEventCaller.delegate_waitevent += SM_OnSysMsgWaitEventHappen;
			//Debug.Log("Unity:" + "The WaitEventCreate:" + n.ToString()+ " "+pEventCaller.name);
			bIsEvent = true;
        }
        catch (Exception)
        {
            Debug.LogError("Unity:"+"Exception In SM_SetSystemMsgTextWithWaitEvent," + n.ToString() + "," + waitsctip.name + "," + param.ToString());
        }

    }
    /// <summary>
    /// 显示玩家的输赢分.
    /// </summary>
    void ShowPlayerShuYingFen(int fenShu)
    {
        string fenShuStr = fenShu >= 0 ? "+" : "-";
        fenShuStr += Mathf.Abs(fenShu).ToString();
        gmsgtext.text = fenShuStr;
        gmsgtext.color = fenShu >= 0 ? YingFenColor : ShuFenColor;
    }
	/// <summary>
	/// 时间驱动的标准提示初始化，当时间结束，调用委托的timeoverfunc对外部通知。
	/// </summary>
	/// <param name="n"></param>
	/// <param name="t"></param>
	/// <param name="timeoverfunc"></param>
	/// <param name="param"></param>
	public void SM_SetSystemMsgTextWithTimeOver(int n, float t, SysMsgDelegate timeoverfunc, object param = null)
    {
        try
        {
            if (param != null)
            {
                switch (n)
                {
                    case 18:
                    case 19:
                    case 20:
                    case 21:
                    case 41:
                        if (!gmsgtext.isActiveAndEnabled)
                        {
                            gmsgtext.gameObject.SetActive(true);
                        }
                        ShowPlayerShuYingFen((int)param);
                        break;
                    default:
                        break;
                }
            }

            nIndex = n;
            showtime = t;
            delegate_timeover += timeoverfunc;
            bIsTimer = true;
        }
        catch (Exception)
        {
            Debug.LogError("Unity:"+"Exception In SM_SetSystemMsgTextWithTimeOver," + n.ToString() + ","+t.ToString()+"," + timeoverfunc.ToString() + "," + param.ToString());
        }
    }
	/// <summary>
	/// 当外部发生特定事件对标准提示进行驱动的时候，所执行的委托函数
	/// </summary>
	/// <param name="n"></param>
    void SM_OnSysMsgWaitEventHappen(int n)
    {
        if (n == nIndex)
        {
            switch (n)
            {
                case 1://等待匹配
                    break;
                case 2: //庄家出牌提示
                    break;
                case 3: //下炮界面
                    break;
                default:
                    break;
            }
			pEventCaller.delegate_waitevent -= SM_OnSysMsgWaitEventHappen;
			if(pEventCaller != null && pEventCaller.nSysMsgArrayLenth > nIndex && pEventCaller.SysMsgArray[nIndex] == this)
			{
				pEventCaller.SysMsgArray[nIndex] = null;
			}
			//Debug.Log("Unity:"+"The WaitEventHapped:" + n.ToString() + " " + pEventCaller.name);
            DestroyThis();
        }

    }
	/// <summary>
	/// 当时间驱动的标准提示，时间结束的时候，执行此函数
	/// </summary>
    public void SM_OnSysMsgTimeOver()
    {
        switch (nIndex)
        {
            case 1://等待匹配
                //showtime = 30;
                //MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnPiPeiShiBaiTuiChuDlg();
                return;
            case 2: //庄家出牌提示
                break;
            case 3: //下炮界面
                ((XiaPaoSelectUI)pEventCaller).OnXiaPaoTimeOverAutoSelect();   //时间结束自动选择
                break;
            case 4: //胡牌，牌局结束
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetJieSuanUIActive(true);   //时间结束自动选择
                break;
            case 22: //流局，牌局结束
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetJieSuanUIActive(true);   //时间结束自动选择
                break;
            case 41:
                {
                    //产生道具UI飞向玩家头像.
                    DaoJuUICtrl daoJuUI = GetComponent<DaoJuUICtrl>();
                    daoJuUI.SpawnDaoJuToPlayer();
                    break;
                }
            /*case 23: //匹配成功牌局开始
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.InitialXiaPaoUI();//自己下炮
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(9, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(10, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(11, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//其他人等待下炮
								break;*/

            case 75:
                {
                    return;
                }
            default:
                break;
        }
        //Debug.Log("Unity:"+"The SM_OnSysMsgTimeOver:" + nIndex.ToString());

        if (bIsEvent && pEventCaller!=null)
        {
            pEventCaller.delegate_waitevent -= SM_OnSysMsgWaitEventHappen;
		}
        if (delegate_timeover!=null)
        {
			delegate_timeover(nIndex);
        }
		if (pEventCaller != null && pEventCaller.nSysMsgArrayLenth>nIndex && pEventCaller.SysMsgArray[nIndex] == this)
		{
			pEventCaller.SysMsgArray[nIndex] = null;
		}
		DestroyThis();

    }
    // Update is called once per frame
    void Update ()
	{

        if (!bIsTimer) return;
        if (showtime>0.0f)
        {
            showtime -= Time.deltaTime;
            gtimertext.text = ((int)showtime).ToString()+"秒";
         }
        if (showtime<=0.0f)
        {
			showtime = 0.0f;
            gtimertext.text = "";
            SM_OnSysMsgTimeOver();
        }
	}
}
