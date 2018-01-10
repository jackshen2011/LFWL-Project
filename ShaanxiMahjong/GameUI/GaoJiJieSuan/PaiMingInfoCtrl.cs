using UnityEngine;
using System.Collections;

class PaiMingInfoCtrl : MonoBehaviour
{
    /// <summary>
    /// 淘汰赛排行榜晋级文本对象.
    /// </summary>
    public GameObject TaoTaiSaiJinJiTxObj;
    /// <summary>
    /// 排名横杠的图集资源.
    /// </summary>
    public Sprite[] PaiMingHGSp;
    /// <summary>
    /// 排名横杠的图片.
    /// </summary>
    public ImageBase PaiMingHGImg;
    /// <summary>
    /// 赢家数字编号.
    /// </summary>
    public Sprite[] YingJiaNumSp;
    /// <summary>
    /// 输家数字编号.
    /// </summary>
    public Sprite[] ShuJiaNumSp;
    /// <summary>
    /// 排名数字图片,直接放上第一名的图片.
    /// </summary>
    public ImageBase PaiMingNumImg;
    /// <summary>
    /// 玩家头像.
    /// </summary>
    public ImageBase HeadImg;
    /// <summary>
    /// "房主"图片.
    /// </summary>
    public GameObject FangZhuImg;
    /// <summary>
    /// 玩家昵称.
    /// </summary>
    public TextBase PlayerNameText;
    /// <summary>
    /// 对局数文本.
    /// </summary>
    public TextBase DuiJuNumText;
    /// <summary>
    /// 胜局数文本.
    /// </summary>
    public TextBase ShengJuNumText;
    /// <summary>
    /// 累计分数文本.
    /// </summary>
    public TextBase LeiJiScoreText;
	/// <summary>
	/// 结算界面显示的数字单位图片/分/金币
	/// </summary>
	public ImageBase JieSuanCoinImg;
	/// <summary>
	/// 设置玩家排名信息.
	/// indexPaiMing[1-4] -> 排名编号.
	/// playerName -> 玩家昵称.
	/// playerHeadSprite -> 头像图片资源.
	/// duiJuNum -> 对局数.
	/// shengJuNum -> 胜局数.
	/// leiJiScroe -> 累计分数.
	/// </summary>
	public void SetPaiMingInfo(int indexPaiMing,
        string playerName,
        Sprite playerHeadSprite,
        int duiJuNum,
        int shengJuNum,
        int leiJiScroe,
        bool isFangZhu)
    {
        string scoreHead = leiJiScroe >= 0 ? "+" : "-";
        if (leiJiScroe == 0)
        {
            scoreHead = "";
        }

        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
        {
            if (!MainRoot._gRoomData.cCacheRoomData.IsZongJueSaiRoom)
            {
                if (indexPaiMing > 2)
                {
                    TaoTaiSaiJinJiTxObj.SetActive(false);
                }
                else
                {
                    TaoTaiSaiJinJiTxObj.SetActive(true);
                }
            }
        }
        else
        {
            TaoTaiSaiJinJiTxObj.SetActive(false);
        }
        HeadImg.sprite = playerHeadSprite;
        PaiMingNumImg.sprite = leiJiScroe >= 0 ? YingJiaNumSp[indexPaiMing - 1] : ShuJiaNumSp[indexPaiMing - 1];
        PaiMingHGImg.sprite = leiJiScroe >= 0 ? PaiMingHGSp[0] : PaiMingHGSp[1];
        PlayerNameText.text = playerName;
        DuiJuNumText.text = duiJuNum.ToString();
        ShengJuNumText.text = shengJuNum.ToString();
        LeiJiScoreText.text = scoreHead + Mathf.Abs(leiJiScroe).ToString();
        if (FangZhuImg != null) {
            FangZhuImg.SetActive(isFangZhu);
        }
		if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
		{
			JieSuanCoinImg.sprite = Resources.Load("Image/GaoJiJieSuanUI/JS-Coin", typeof(Sprite)) as Sprite;
		}
    }
}