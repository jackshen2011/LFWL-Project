using UnityEngine;
using System.Collections;

class ZongJueSaiPlayerInfo : MonoBehaviour
{
    /// <summary>
    /// 玩家昵称文本信息.
    /// </summary>
    public TextBase PlayerNameTx;
    /// <summary>
    /// 玩家ID文本信息.
    /// </summary>
    public TextBase PlayerIDTx;
    /// <summary>
    /// 玩家IP文本信息.
    /// </summary>
    public TextBase PlayerIPTx;
    /// <summary>
    /// 玩家头像图片.
    /// </summary>
    public ImageBase PlayerHeadImg;
    public ImageBase PlayerSexImg;
    public Sprite[] PlayerSexSprite;
    /// <summary>
    /// 总决赛名次.
    /// </summary>
    public ImageBase ZongJueSaiMingCi;
    /// <summary>
    /// 总决赛名次图集.
    /// ZongJueSaiMingCiSprite[0] 第一名.
    /// ZongJueSaiMingCiSprite[1] 第二名.
    /// ZongJueSaiMingCiSprite[2] 第三名.
    /// ZongJueSaiMingCiSprite[3] 第四名.
    /// </summary>
    public Sprite[] ZongJueSaiMingCiSprite;
    /// <summary>
    /// 显示总决赛赛玩家信息.
    /// args[0] id
    /// args[1] 昵称
    /// args[2] 微信头像url
    /// args[3] 性别
    /// args[4] 总决赛名次.
    /// </summary>
    public void ShowPlayerInfo(object[] args)
    {
        string playerName = (string)args[1];
        PlayerNameTx.text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        PlayerIDTx.text = "ID：" + ((int)args[0]).ToString();
        //PlayerIPTx.text = "IP：" + MainRoot._gPlayerData.getIpAddress();
        PlayerSexImg.sprite = PlayerSexSprite[((int)args[3] == 1 ? 0 : 1)];
        MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage((string)args[2], PlayerHeadImg);
        int mingCiVal = (int)args[4];
        if (mingCiVal <= -1)
        {
            ZongJueSaiMingCi.gameObject.SetActive(false);
        }
        else
        {
            ZongJueSaiMingCi.sprite = ZongJueSaiMingCiSprite[mingCiVal];
            ZongJueSaiMingCi.gameObject.SetActive(true);
        }
    }
}