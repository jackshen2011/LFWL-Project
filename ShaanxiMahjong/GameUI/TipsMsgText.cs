using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

class TipsMsgText : MonoBehaviour
{
    public struct TipsMsg
    {
		public string sFromUserName;	//发布者昵称
        public string tip;      //显示内容
        public float showtime;  //显示时间
        public float lasttime;  //持续时间
        public bool isSys;      //用于系统公告
    };

    public TextBase tipstextA;
    public TextBase tipstextB;
	public GameObject pback;

	public List<TipsMsg> TipsList = new List<TipsMsg>();
	public int nShowIndex = 0; //当前显示第几条

    TipsMsg tempTipMsg;
    void Start ()
    {
        nShowIndex = 0;
        /*
		SetTipsMsgText("年轻人嘛，现在没钱算什么，以后没钱的日子还多着呢。", 5.7f);
        SetTipsMsgText("等忙完这一阵，就可以接着忙下一阵了。", 3.3f);
        SetTipsMsgText("没有钱包的充实，哪来内心的宁静。", 3.1f);
        SetTipsMsgText("你以为有了钱就会像你想象中那样快乐吗？不，你错了。有钱人的快乐，你根本想象不到。", 3.1f);
        SetTipsMsgText("人都有幸福的权利，但少数人有幸福的能力。", 3.1f);
        SetTipsMsgText("系统真诚的说：万事开头难，然后中间难，最后结束难。", 4.5f,true);
		*/
    }
    /// <summary>
    /// 设置系统通知信息,具体内容来源于服务器.
    /// 默认第一条消息为系统开始信息.
    /// args[0 - 2]为1组数据,具体对应TipsMsg里的逐条数据.
    /// args[0] -> tip.
    /// args[1] -> showtime.
    /// args[2] -> isSys.
    /// </summary>
    public void SetTipsMsgTextByServerPort(object[] args)
    {
		if (args.Length==3)//只有三个元素，应该就是要清空客户端内容
		{
			TipsList.Clear();
			nShowIndex = 0;
			return;
		}
		if (pback != null)
		{
			pback.SetActive(true);
		}
		string msg = "";
        float showtime = 0f;
        bool isSys = false;
        byte countInfo = 3;
        for (int i = 3; i < args.Length; i++) {
            if (i % countInfo == 0) {
                msg = (string)args[i];
            }
            if (i % countInfo == 1) {
                showtime = (float)args[i];
            }
            if (i % countInfo == 2) {
                isSys = (bool)args[i];
                SetTipsMsgText("",msg, showtime, isSys);
            }
        }
    }
    public void SetTipsMsgText(string sFromName,string msg, float showtime,bool isSys = false)    //添加一条tips信息，isSys表示是否系统宫傲
    {
        TipsMsg temp;
		temp.sFromUserName = "";
        temp.tip = msg;
        temp.showtime = showtime;
        temp.lasttime = 0.0f;
        temp.isSys = isSys;
        Debug.Log("@@@@@@@msg=" + msg + "@@@@@showtime=" + showtime.ToString() + "@@@@@isSys" + isSys.ToString());
        if (!isSys)
        {
            TipsList.Add(temp);
        }
        else
        {
            TipsList.Insert(0,temp);
            nShowIndex = 0;
        }
    }
    // Update is called once per frame
    void Update ()
    {
        if (TipsList.Count != 0 && nShowIndex < TipsList.Count)
        {
			if (pback.activeSelf == false)
			{
				pback.SetActive(true);
			}
			tempTipMsg = TipsList[nShowIndex];
            tempTipMsg.lasttime += Time.deltaTime;
            if (tempTipMsg.lasttime >= tempTipMsg.showtime)
            {
				if (tempTipMsg.isSys)	//系统的就保留，下一波继续
				{
					tempTipMsg.lasttime = 0;
					TipsList[nShowIndex] = tempTipMsg;
					nShowIndex++;
				}
				else //玩家发的就删除
				{
					TipsList.RemoveAt(nShowIndex);
					if (nShowIndex == TipsList.Count-1)
					{
						nShowIndex = 0; 
					}
				}
            }
            else
            {
				if (tipstextA != null)
				{
					tipstextA.text = tempTipMsg.tip;
				}
				TipsList[nShowIndex] = tempTipMsg;
			}
            
        }
        else
        {
            nShowIndex = 0;
			if (tipstextA != null)
			{
				tipstextA.text = "";
			}
			if (pback!=null && TipsList.Count==0)
			{
				pback.SetActive(false);
			}

		}
	}
}
