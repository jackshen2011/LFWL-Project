using UnityEngine;
using System.Collections;

class PlayerDtInfoCtrl : UnModalUIBase
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
    public void SetPlayerDtInfo(Sprite playerHeadSprite)
    {
        if (playerHeadSprite != null) {
            PlayerHeadImg.sprite = playerHeadSprite;
        }

        if (MainRoot._gPlayerData != null) {
            string playerName = MainRoot._gPlayerData.sUserName;
            PlayerNameTx.text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
            PlayerIDTx.text = "ID：" + MainRoot._gPlayerData.nUserId.ToString();
            if (MainRoot._gPlayerData.PlayerIP.Equals(""))
            {
                PlayerIPTx.text = "";
            }
            else
            {
                PlayerIPTx.text = "IP：" + MainRoot._gPlayerData.PlayerIP;
            }
            PlayerSexImg.sprite = PlayerSexSprite[(MainRoot._gPlayerData.nSex==1?0:1)];
        }
    }
    public void OnClickPanel()
    {
        DestroyThis();
    }
}