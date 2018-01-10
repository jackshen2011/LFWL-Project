using UnityEngine;
using System.Collections;

class PlayerPaiHangUI : ListUiDtCtrl
{
    /// <summary>
    /// 排名横杠的图集资源.
    /// </summary>
    public Sprite[] PaiMingHGSp;
    /// <summary>
    /// 赢家数字编号.
    /// </summary>
    public Sprite[] YingJiaNumSp;
    /// <summary>
    /// 输家数字编号.
    /// </summary>
    public Sprite[] ShuJiaNumSp;

    /// <summary>
    /// args[x]: 0 排行榜编号, 1 玩家姓名, 2 微信头像的url, 3 累计分值.
    /// UiText[x]: 0 排名数字, 1 玩家昵称, 2 累计分数.
    /// UiImg[x]: 0 玩家头像, 1 排名数字, 2 排名横杠.
    /// </summary>
    public override void ShowListUiDt(object[] args)
    {
        return;
        int indexPaiMing = (int)args[0];
        int leiJiScroe = (int)args[3];
        string playerName = (string)args[1];
        string scoreHead = leiJiScroe >= 0 ? "+" : "-";
        if (leiJiScroe == 0)
        {
            scoreHead = "";
        }
        MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage((string)args[2], UiImg[0]);

        if (indexPaiMing <= 4)
        {
            UiText[0].gameObject.SetActive(false);
            UiImg[1].sprite = leiJiScroe >= 0 ? YingJiaNumSp[indexPaiMing - 1] : ShuJiaNumSp[indexPaiMing - 1];
        }
        else
        {
            UiImg[1].gameObject.SetActive(false);
            UiText[0].text = indexPaiMing.ToString();
        }
        UiImg[2].sprite = leiJiScroe >= 0 ? PaiMingHGSp[0] : PaiMingHGSp[1];
        UiText[1].text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        UiText[2].text = scoreHead + Mathf.Abs(leiJiScroe).ToString();
    }
}