using UnityEngine;
using System.Collections;



class UserInfoPanel : UnModalUIBase
{
    // Use this for initialization
    public PlayerBase pMyPlayer;
    public UserDetailsInfo pDetail;
    public GameObject pZhuangjiaImg;
    public ImageBase pXiaPaoImg;
    public Animator emotionShow;//表情显示
    public TextBase textEmotion;//表情文字
    private GuiPlaneAnimationPlayer emotionEvent;//表情播放事件控制
    private GuiPlaneAnimationPlayer textEmotionEvent;//文字播放事件控制
    public RuntimeAnimatorController[] allEmotions;//所有的表情动画

    public string[] TEXT = { "大家好，很高兴见到各位！",
                            "快点吧，我等的花儿都谢了！",
                            "不要走，决战到天亮！",
                            "你是帅哥还是美女啊！",
                            "君子报仇，十盘不算晚！",
                            "快放炮啊，我都等得不耐烦了！",
                            "真不好意思，又胡啦！呵呵！",
                            "打错了！呜呜……"};
    /// <summary>
    /// 用户头像上的出牌提示特效
    /// </summary>
    public GameObject pPlayerHeadEff;

	/// <summary>
	/// 玩家微信头像.
	/// </summary>
	public ImageBase pHeadImg;
    /**
     * 金币数量.
     */
    public TextBase CoinNumTx;
    public void Initial(PlayerBase p)
    {
        pMyPlayer = (PlayerBase)p;
        pMyPlayer.CheckPlayerDataFromPlayerBase();
        int coin= pMyPlayer.nCardCoinNum; 
        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_Gold)
        {
            coin = pMyPlayer.nCoinNum;
        }
        ShowPlayerCoinNumInfo(coin);
		Debug.Log("Unity:" + pMyPlayer.nUserId.ToString() + "/" + pMyPlayer.nSex.ToString());
		MainRoot._gPlayerData.pAsyncImageDownload.SetAsyncImage((AsyncImageDownload.PlayerSeats)p.nUIUserSit,
            p.nUserId,
            (PlayerData.PlayerSexEnum)p.nSex,
            p.sHeadImgUrl,
            pHeadImg);
		SetZhuangjia(false);
	}
    public void SetZhuangjia(bool isZhuang)
    {
        pZhuangjiaImg.SetActive(isZhuang);
    }
    public void SetXiaPaoImg(int ntype)
    {
		if (pXiaPaoImg == null)
		{
			return;
		}
		if( ntype>4 || ntype<0 )
        {
			pXiaPaoImg.gameObject.SetActive(false);
			return;
		}
        if (!pXiaPaoImg.gameObject.activeSelf)
        {
            pXiaPaoImg.gameObject.SetActive(true);
        }
        pXiaPaoImg.overrideSprite = Resources.Load("Image/UserInfoPanel/PS-"+ntype.ToString(),typeof(Sprite) ) as Sprite;
    }
    public void OnUserFaceClick()
    {
        if (pMyPlayer == null)
            return;
        if (pDetail == null)
        {
            GameObject temp;
            float a = -1.1f, b = -1.1f;
            float xpos = ((RectTransform)transform).anchoredPosition.x;
            float ypos = ((RectTransform)transform).anchoredPosition.y;
            float width = ((RectTransform)transform).sizeDelta.x;
            float height = ((RectTransform)transform).sizeDelta.y;

            temp = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/UserDetailsInfo"), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
            pDetail = temp.GetComponent<UserDetailsInfo>();
            pDetail.Initial(pMyPlayer, this);
            pDetail.ShowTablePlayerIp();

            if (xpos < 0.0f)
            {
                a = 1.1f;
            }
            if (ypos < 0.0f)
            {
                b = 1.1f;
            }
            ((RectTransform)pDetail.transform).anchoredPosition = new Vector2(xpos + width * a, ypos + height * b);
        }
        else
        {
            pDetail.DestroyThis();
            pDetail = null;
        }
    }
    public static string GetPlayerCoinValInfo(int val)
    {
        //val = 112010089; //test.
        float coinF = 0f;
        string coinStr = "";
        if (val < 10000) {
            coinStr = val.ToString();
        }
        if (val >= 10000 && val < 100000000) {
            coinF = (float)val / 10000;
            coinStr = coinF.ToString() + "万";
        }
        if (val >= 100000000) {
            if (val > PlayerData.PlayerCoinMax) {
                val = PlayerData.PlayerCoinMax;
            }
            val /= 10000; //消掉最后四位的数据.
            coinF = (float)val / 10000;
            coinStr = coinF.ToString() + "亿";
        }
        return coinStr;
    }
    public void ShowPlayerCoinNumInfo(int val)
    {
        CoinNumTx.text = GetPlayerCoinValInfo(val);
    }
    //显示表情
    public void ShowEmotion(int index)
    {
        if (index<100)
        {//图像
            if (emotionShow == null)
            {
                return;
            }
            if (emotionEvent == null)
            {
                emotionEvent = emotionShow.GetComponent<GuiPlaneAnimationPlayer>();
            }
            if (emotionEvent == null)
            {
                return;
            }
            if (emotionEvent.DelegateOnPlayEndEvent == null)
            {
                emotionEvent.DelegateOnPlayEndEvent = HideEmotion;
            }
            emotionEvent.Play();
            emotionShow.gameObject.SetActive(true);
            if (!emotionShow.gameObject.activeSelf)
            {
                emotionShow.gameObject.SetActive(true);
            }
            if (index>= allEmotions.Length)
            {
                return;
            }
            emotionShow.runtimeAnimatorController = allEmotions[index];
        }
        else
        {//文字
            string say = TEXT[index-100];
            textEmotion.text = say;
            if (textEmotionEvent == null)
            {
                textEmotionEvent = textEmotion.GetComponent<GuiPlaneAnimationPlayer>();
            }
            if (textEmotionEvent == null)
            {
                return;
            }
            if (textEmotionEvent.DelegateOnPlayEndEvent == null)
            {
                textEmotionEvent.DelegateOnPlayEndEvent = HideSpeaking;
            }
            switch (pMyPlayer.nUIUserSit)
            {
                case 0:
                    textEmotion.gameObject.transform.localPosition = new Vector3(112,102,0);
                    break;
                case 1:
                    textEmotion.gameObject.transform.localPosition = new Vector3(112, 102, 0);
                    break;
                case 2:
                    textEmotion.gameObject.transform.localPosition = new Vector3(105, -113, 0);
                    break;
                case 3:
                    textEmotion.gameObject.transform.localPosition = new Vector3(-220, 24, 0);
                    break;
                default:
                    break;
            }
            textEmotionEvent.Play();
            textEmotion.gameObject.SetActive(true);

            //播放声音
            if (pMyPlayer.nSex == 1)
            {//男
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.LiaoTianMsg01_M + index - 100);
            }
            else
            {//女
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.LiaoTianMsg01_W + index - 100);
            }
        }

    }
    //隐藏表情
    void HideEmotion()
    {
        emotionShow.gameObject.SetActive(false);
        emotionEvent.Stop();
    }
    void HideSpeaking()
    {
        if (textEmotion)
        {
            textEmotion.gameObject.SetActive(false);
            textEmotionEvent.Stop();
        }
    }
}
