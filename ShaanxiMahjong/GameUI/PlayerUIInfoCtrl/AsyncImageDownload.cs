using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;

/// <summary>
/// 动态下载玩家微信头像的控制脚本.
/// </summary>
class AsyncImageDownload : MonoBehaviour  
{
    /// <summary>
    /// 玩家座次枚举
    /// </summary>
    public enum PlayerSeats
    {
        SELF_PLAYER,
        UP_PLAYER,
        OPPOSITE_PLAYER,
        DOWN_PLAYER,
    }
    /// <summary>
    /// 男玩家的默认微信头像资源.
    /// </summary>
    public Sprite[] PlayerBoySp;
    /// <summary>
    /// 女玩家的默认微信头像资源.
    /// </summary>
    public Sprite[] PlayerGirlSp;
    public struct PlayerWXDt
    {
        /// <summary>
        /// 游戏中4个玩家的微信头像资源.
        /// </summary>
        public Sprite PlayerWXHead;
        /// <summary>
        /// 玩家ID信息.
        /// </summary>
        public int UserID;
        /// <summary>
        /// 玩家微信头像的Url.
        /// </summary>
        public string Url;
    }
    /// <summary>
    /// PlayerWXDtAy[0] -> 玩家自己的信息.
    /// PlayerWXDtAy[1] -> 玩家上家的信息.
    /// PlayerWXDtAy[2] -> 玩家对家的信息.
    /// PlayerWXDtAy[3] -> 玩家下家的信息.
    /// </summary>
    public PlayerWXDt[] PlayerWXDtAy = new PlayerWXDt[4];
    /// <summary>
    /// 获取玩家微信头像.
    /// </summary>
    public Sprite GetPlayerWXHeadImg(int playerId)
    {
        Sprite headImg = null;
        for (int i = 0; i < PlayerWXDtAy.Length; i++)
        {
            if (playerId == PlayerWXDtAy[i].UserID)
            {
                headImg = PlayerWXDtAy[i].PlayerWXHead;
                break;
            }
        }

        if (headImg == null)
        {
            headImg = PlayerBoySp[0];
        }
        return headImg;
    }
    /// <summary>
    /// 设置游戏中4个玩家的微信头像信息.
    /// </summary>
    public void SetAsyncImage(PlayerSeats playerSeat, int userID, PlayerData.PlayerSexEnum playerSex, string url, ImageBase image)
    {
        //开始下载图片前，将UITexture的主图片设置为占位图.
        int indexVal = (int)playerSeat;
        if (PlayerWXDtAy[indexVal].PlayerWXHead != null
            && PlayerWXDtAy[indexVal].Url == url
            && PlayerWXDtAy[indexVal].UserID == userID) {
            if (image.sprite != PlayerWXDtAy[indexVal].PlayerWXHead) {
                image.sprite = PlayerWXDtAy[indexVal].PlayerWXHead;
            }
            return;
        }

        if (url == "") {
            switch (playerSex) {
                case PlayerData.PlayerSexEnum.BOY:
                    if (PlayerBoySp.Length != 0) {
                        image.sprite = PlayerBoySp[Random.Range(0, 100) % PlayerBoySp.Length];
                    }
                    break;
                case PlayerData.PlayerSexEnum.GIRL:
                    if (PlayerGirlSp.Length != 0) {
                        image.sprite = PlayerGirlSp[Random.Range(0, 100) % PlayerGirlSp.Length];
                    }
                    break;
				case PlayerData.PlayerSexEnum.SEXNULL:
					if (PlayerBoySp.Length != 0)
					{
						image.sprite = PlayerGirlSp[Random.Range(0, 100) % PlayerGirlSp.Length];
					}
					break;
			}
        }
        else {
            StartCoroutine(DownloadImage(indexVal, userID, url, image));
        }
    }

    /// <summary>
    /// 根据url信息下载图片信息.
    /// </summary>
    public void LoadingUrlImage(string url, ImageBase image)
    {
        if (url == "")
        {
            PlayerData.PlayerSexEnum playerSex = (PlayerData.PlayerSexEnum)(Random.Range(0, 100) % 3);
            switch (playerSex)
            {
                case PlayerData.PlayerSexEnum.BOY:
                    if (PlayerBoySp.Length != 0)
                    {
                        image.sprite = PlayerBoySp[Random.Range(0, 100) % PlayerBoySp.Length];
                    }
                    break;
                case PlayerData.PlayerSexEnum.GIRL:
                    if (PlayerGirlSp.Length != 0)
                    {
                        image.sprite = PlayerGirlSp[Random.Range(0, 100) % PlayerGirlSp.Length];
                    }
                    break;
                case PlayerData.PlayerSexEnum.SEXNULL:
                    if (PlayerBoySp.Length != 0)
                    {
                        image.sprite = PlayerGirlSp[Random.Range(0, 100) % PlayerGirlSp.Length];
                    }
                    break;
            }
        }
        else
        {
            StartCoroutine(DownloadImage(-1, -1, url, image));
        }
    }

    /// <summary>
    /// (indexVal 小于 0 或 indexVal 大于 3)时,不去对PlayerWXDtAy进行操作.
    /// indexVal = [0, 3].
    /// </summary>
    IEnumerator DownloadImage(int indexVal, int userID, string url, ImageBase image)
    {
		Texture2D tex2d = null;
		//Debug.Log("Unity:"+"downloading new image:" + url.GetHashCode());//url转换HD5作为名字.
		WWW www = null;
		try
		{
			www = new WWW(url);
		}
		catch (System.Exception)
		{
		}
		yield return www;
		try
		{
			tex2d = www.texture;
		}
		catch (System.Exception)
		{
		}
		if (tex2d==null)
		{
			tex2d = (Texture2D)Resources.Load("Image/UserDetailsInfo/Photo");
		}

		Sprite m_sprite = Sprite.Create(tex2d, new Rect(0, 0, tex2d.width, tex2d.height), new Vector2(0, 0));
        if (image != null)
        {
            image.sprite = m_sprite;
        }

        if (indexVal > -1 && indexVal < 4)
        {
            PlayerWXDtAy[indexVal].PlayerWXHead = m_sprite;
            PlayerWXDtAy[indexVal].UserID = userID;
            PlayerWXDtAy[indexVal].Url = url;
        }
    }
}