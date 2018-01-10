using UnityEngine;
using System.Collections;

class HaiXuanPaiHangUI : ListUiDtCtrl
{
    /// <summary>
    /// 玩家自身战绩文本颜色.
    /// </summary>
    public Color PlayerTextColor = Color.red;
    /// <summary>
    /// 自己的排行编号.
    /// </summary>
    public Sprite[] MyPaiHangNum;
    /// <summary>
    /// 其他人的排行编号.
    /// </summary>
    public Sprite[] OtherPaiHangNum;

    /// <summary>
    /// args[x]: 0 排行榜编号, 1 玩家昵称, 2 累计分值, 3 是否为自己.
    /// UiText[x]: 0 玩家昵称, 1 累计分值.
    /// UiImg[x]: 0 排行榜编号.
    /// </summary>
    public override void ShowListUiDt(object[] args)
    {
        int indexPaiMing = (int)args[0] - 1;
        int leiJiScroe = (int)args[2];
        bool isMyScore = (int)args[3] == 0 ? false : true;
        string playerName = (string)args[1];
        string scoreHead = leiJiScroe >= 0 ? "+" : "-";
        if (leiJiScroe == 0)
        {
            scoreHead = "";
        }

        if (indexPaiMing < 8)
        {
            if (isMyScore)
            {
                UiImg[0].sprite = MyPaiHangNum[indexPaiMing];
            }
            else
            {
                UiImg[0].sprite = OtherPaiHangNum[indexPaiMing];
            }
            UiText[2].gameObject.SetActive(false);
        }
        else
        {
            UiImg[0].gameObject.SetActive(false);
            UiText[2].text = (indexPaiMing + 1).ToString();
        }
        UiText[0].text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        UiText[1].text = scoreHead + Mathf.Abs(leiJiScroe).ToString();
    }

    /// <summary>
    /// 改变玩家自身战绩文本格式.
    /// UiText[x]: 0 玩家昵称, 1 累计分值.
    /// </summary>
    public void ChangeZhanJiTextType()
    {
        for (int i = 0; i < 2; i++)
        {
            UiText[i].color = PlayerTextColor;
            UiText[i].fontStyle = FontStyle.Bold;
        }
    }
}