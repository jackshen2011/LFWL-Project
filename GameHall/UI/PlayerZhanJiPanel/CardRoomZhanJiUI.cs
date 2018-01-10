using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class CardRoomZhanJiUI : MonoBehaviour
{
    /// <summary>
    /// 场次.
    /// </summary>
    public TextBase ChangCiText;
    /// <summary>
    /// 盈利.
    /// </summary>
    public TextBase YingLiText;
    /// <summary>
    /// 时间.
    /// </summary>
    public TextBase TimeText;
    /// <summary>
    /// 索引.
    /// </summary>
    int IndexVal = 0;
    /// <summary>
    /// 战绩详细信息.
    /// </summary>
    object ZhanJiDt;
    /// <summary>
    /// 点击回顾按键.
    /// </summary>
    public void OnClickHuiGuBt()
    {
        Debug.Log("Unity: Card.OnClickHuiGuBt -> IndexVal: " + IndexVal);
    }
    /// <summary>
    /// 点击战绩列表单元按键.
    /// </summary>
    public void OnClickZhanJiUIBt()
    {
        Debug.Log("Unity: Card.OnClickZhanJiUIBt -> IndexVal: " + IndexVal);
    }
    /// <summary>
    /// 显示战绩数据.
    /// </summary>
    public void ShowZhanJiInfo(object zjDt)
    {
        ZhanJiDt = zjDt;
        string changCiVal = "好友房";
        string yingLiVal = "";
        string timeVal = "";
        int scoreVal = 0;
        string stemp = ((string)zjDt).Replace("#", ":");
        List<string> dataList = new List<string>(stemp.Split(":".ToCharArray()));
        dataList[3] += ":" + dataList[4] + ":" + dataList[5]; //时间数据拼接.
        dataList.RemoveAt(5);
        dataList.RemoveAt(4);
        for (int i = 0; i < dataList.Count; i++)
        {
            if (i == 3)
            {
                timeVal = MainRoot._gUIModule.pUnModalUIControl.pPlayerZhanJiPanelCtrl.DateDiff(System.Convert.ToDateTime(dataList[i]));
            }

            if (i > 3)
            {
                if ((i - 4) % 3 == 0)
                {
                    if (MainRoot._gPlayerData.nUserId == System.Convert.ToInt32(dataList[i]))
                    {
                        scoreVal = System.Convert.ToInt32(dataList[i + 2]);
                        yingLiVal = scoreVal >= 0 ? "+" : "-";
                        yingLiVal += Mathf.Abs(scoreVal).ToString();
                    }
                }
            }
        }

        ChangCiText.text = changCiVal;
        YingLiText.text = yingLiVal;
        TimeText.text = timeVal;
    }
    /// <summary>
    /// 设置战绩索引.
    /// </summary>
    public void SetZhanJiIndexVal(int val)
    {
        IndexVal = val;
    }
}