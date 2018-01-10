using UnityEngine;
using System.Collections;
using cn.sharesdk.unity3d;

/// <summary>
/// 微信SDK模块
/// </summary>
class WeChat_Module : UnModalUIBase {

	/// <summary>
	/// shareSDK 对象
	/// </summary>
	public ShareSDK ssdk;
	public bool isReady;

#if CHANNELS_HUASHANG
    public const string ShareUrl = "http://www.qinrenmajiang.com/downloadHS/";
#elif UNITY_IPHONE
    public const string ShareUrl = "http://www.qinrenmajiang.com/download/";
#else
	public const string ShareUrl = "http://www.qinrenmajiang.com/download/";
#endif

	/// <summary>
	/// 初始化shareSDk
	/// </summary>
	public void InitialWeChatSDK()
	{
		try
		{
			ssdk = gameObject.GetComponent<ShareSDK>();
			if (ssdk == null)
			{
				Debug.LogError("Unity:"+" WeChat Error InitialWeChatSDK Error! Get ShareSDK Faild!");
				return;
			}
			ssdk.shareHandler = OnWeChatShareResultHandler;
			ssdk.authHandler = OnWeChatAuthResultHandler;
			ssdk.showUserHandler = OnWeChatGetUserInfoResultHandler;
			isReady = true;
		}
		catch (System.Exception ex)
		{
			Debug.LogError("Unity:"+" WeChat Error InitialWeChatSDK Error! :"+ex.ToString());
		}

	}

	/// <summary>
	/// 在iOS系统下，用户主动操作微信相关功能前，检查是否已经安装了微信
	/// </summary>
	/// <returns></returns>
	public bool IsWeChatLoginValid_iOS()
	{
		bool iFlag = false;
		if ((!isReady) || (ssdk == null))
		{
			Debug.LogError("Unity:" + "iOS WeChat Error WeChatSDK Not Initial!");
			return iFlag;
		}
		iFlag = ssdk.IsClientValid(PlatformType.WechatPlatform);
#if UNITY_IPHONE
		Debug.LogError("Unity:" + "IsWeChatLoginValid_iOS -iFlag:"+iFlag.ToString());
		if (!iFlag)
		{
			Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
		}
#endif
		return iFlag;
	}
	/// <summary>
	/// 微信登录SDK是否可用
	/// </summary>
	/// <returns></returns>
	public bool IsWeChatLoginValid()
	{
		if ((!isReady) || (ssdk == null))
		{
			Debug.LogError("Unity:"+" WeChat Error WeChatSDK Not Initial!");
			return false;
		}
		return ssdk.IsClientValid(PlatformType.WechatPlatform);
	}
	/// <summary>
	/// 当前手机登陆的微信用户是否已经给游戏授权过
	/// </summary>
	/// <returns></returns>
	public bool IsWeChatAuthorized()
	{
		if (!IsWeChatLoginValid())
		{
			Debug.LogError("Unity:"+" WeChat Error WeChatSDK Not Initial!");
			return false;
		}
		return ssdk.IsAuthorized(PlatformType.WechatPlatform);

	}
	/// <summary>
	/// 微信分享到好友功能SDK是否可用
	/// </summary>
	/// <returns></returns>
	public bool IsWeChatShareSDKValid()
	{
		if ((!isReady) || (ssdk == null))
		{
			Debug.LogError("Unity:"+" WeChat Error WeChatSDK Not Initial!");
			return false;
		}
		return ssdk.IsClientValid(PlatformType.WeChat);

	}
	/// <summary>
	/// 微信分享到朋友圈功能SDK是否可用
	/// </summary>
	/// <returns></returns>
	public bool IsWeChatMomentsSDKValid()
	{
		if ((!isReady) || (ssdk == null))
		{
			Debug.LogError("Unity:"+" WeChat Error WeChatSDK Not Initial!");
			return false;
		}
		return ssdk.IsClientValid(PlatformType.WeChatMoments);

	}
	/// <summary>
	/// 分享到微信好友对话
	/// </summary>
	public void ShareInfoToWeChat()
	{
		if (IsWeChatShareSDKValid())
		{
			ShareContent content = new ShareContent();
			content.SetText("秦人麻将！咱老陕自己的麻将！快来加入吧！");
			content.SetTitle("秦人麻将");
			content.SetImageUrl("http://www.qinrenmajiang.com/images/gameicon.png");
			content.SetUrl(ShareUrl);
			content.SetShareType(ContentType.Webpage);
			ssdk.ShareContent(PlatformType.WeChat, content);
		}
		else
		{
			Debug.LogError("Unity:"+" WeChat Error ShareInfoToWeChat Error,IsWeChatShareSDKValid false! ");
#if UNITY_IPHONE
			Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
			return;
#endif
		}
	}
	public void ShareInfoToWeChat(string scontent,string stitle="秦人麻将")
	{
		bool n = IsWeChatShareSDKValid();
		Debug.Log("Unity:IsWeChatShareSDKValid" + n.ToString());
		if (n)
		{
			ShareContent content = new ShareContent();
			content.SetText(scontent);
			content.SetTitle(stitle);
			content.SetImageUrl("http://www.qinrenmajiang.com/images/gameicon.png");
			content.SetUrl(ShareUrl);
			content.SetShareType(ContentType.Webpage);
			ssdk.ShareContent(PlatformType.WeChat, content);
			Debug.Log("Unity:ShareInfoToWeChat1");
		}
		else
		{
			Debug.LogError("Unity:"+" WeChat Error ShareInfoToWeChat Error,IsWeChatShareSDKValid false! ");
#if UNITY_IPHONE
			Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
			return;
#endif
		}
	}
	/// <summary>
	/// 分享到微信朋友圈
	/// </summary>
	public void ShareInfoToWeChatMoments()
	{
		bool n = IsWeChatMomentsSDKValid();
		Debug.Log("Unity:IsWeChatMomentsSDKValid" + n.ToString());
		if (n)
		{
			ShareContent content = new ShareContent();
			content.SetTitle("游戏邀请");
			content.SetText("秦人麻将！咱老陕自己的麻将！快来加入吧！");
			content.SetUrl("http://www.qinrenmajiang.com/download/");
			content.SetImageUrl(ShareUrl);
			content.SetShareType(ContentType.Webpage);
			ssdk.ShareContent(PlatformType.WeChatMoments, content);
			Debug.Log("Unity:ShareInfoToWeChatMoments");
		}
		else
		{
			Debug.LogError("Unity:"+" WeChat Error ShareInfoToWeChatMoments Error,IsWeChatMomentsSDKValid false! ");
#if UNITY_IPHONE
			Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
			return;
#endif
		}
	}
	/// <summary>
	/// 获取微信授权
	/// </summary>
	public void GetWeChatAuthorize()
	{
		if (!IsWeChatAuthorized())
		{
			ssdk.Authorize(PlatformType.WechatPlatform);
		}
		else
		{
			Debug.LogError("Unity:"+" WeChat Error GetWeChatAuthorize Error,IsWeChatAuthorized true! ");
		}
	}
	/// <summary>
	/// 清除微信授权缓存
	/// </summary>
	public void RemoveWeChatAuthorize()
	{
		if (IsWeChatAuthorized())
		{
			ssdk.CancelAuthorize(PlatformType.WechatPlatform);
		}
		else
		{
			Debug.LogError("Unity:"+" WeChat Error RemoveWeChatAuthorize Error,IsWeChatAuthorized false! ");
		}
		MainRoot._gPlayerData.PlayerAuthorizeFaild("");
	}
	/// <summary>
	/// 获取微信用户信息，如未授权，将自动拉起授权
	/// </summary>
	public void GetWeChatUserInfo()
	{
		if (IsWeChatLoginValid())
		{
			ssdk.GetUserInfo(PlatformType.WechatPlatform);
		}
		else
		{
			Debug.LogError("Unity:"+" WeChat Error GetWeChatUserInfo Error,IsWeChatLoginValid false! ");
		}
	}
	/// <summary>
	/// 获取微信授权回调函数
	/// </summary>
	/// <param name="reqID"></param>
	/// <param name="state"></param>
	/// <param name="type"></param>
	/// <param name="result"></param>
	void OnWeChatAuthResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		/*if (showt != null)
		{
			if (state == ResponseState.Success)
			{
				showt.text += ("authorize success !" + "Platform :" + type);
			}
			else if (state == ResponseState.Fail)
			{
#if UNITY_ANDROID
				showt.text += ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
#elif UNITY_IPHONE
			print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
			}
			else if (state == ResponseState.Cancel)
			{
				showt.text += ("cancel !");
			}
		}*/
	}
	/// <summary>
	/// 获取微信用户信息回调函数
	/// </summary>
	/// <param name="reqID"></param>
	/// <param name="state"></param>
	/// <param name="type"></param>
	/// <param name="result"></param>
	void OnWeChatGetUserInfoResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		Debug.Log("Unity:"+"WeChat OnWeChatGetUserInfoResultHandler!");

		if (MainRoot._gPlayerData != null)
		{
			if (state == ResponseState.Success)
			{
				Debug.Log("Unity:" + "WeChat ResponseState.Success!");
				MainRoot._gPlayerData.PlayerAuthorizeSuccess(result);
			}
			else if (state == ResponseState.Fail)
			{
#if UNITY_ANDROID
				//showt.text += ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
				MainRoot._gPlayerData.PlayerAuthorizeFaild((string)result["msg"]);
#elif UNITY_IPHONE
				print ("fail! error code = " + result["error_code"] + "; error msg = " + result["error_msg"]);
#endif
			}
			else if (state == ResponseState.Cancel)
			{
				MainRoot._gPlayerData.PlayerAuthorizeCancel();
			}
		}
	}
	/// <summary>
	/// 微信分享的回调函数
	/// </summary>
	/// <param name="reqID"></param>
	/// <param name="state"></param>
	/// <param name="type"></param>
	/// <param name="result"></param>
	void OnWeChatShareResultHandler(int reqID, ResponseState state, PlatformType type, Hashtable result)
	{
		Debug.Log("Unity:"+"WeChat OnWeChatShareResultHandler!");

		if (MainRoot._gPlayerData != null)
		{
			if (state == ResponseState.Success)
			{
				MainRoot._gPlayerData.PlayerShareToWeChatSucess(result);
				//showt.text += "share result :";
				//showt.text += MiniJSON.jsonEncode(result);
			}
			else if (state == ResponseState.Fail)
			{
				MainRoot._gPlayerData.PlayerShareToWeChatFaild((string)result["msg"]);
				//showt.text += ("fail! throwable stack = " + result["stack"] + "; error msg = " + result["msg"]);
			}
			else if (state == ResponseState.Cancel)
			{
				MainRoot._gPlayerData.PlayerShareToWeChatCancel();
			}
		}
	}

    /// <summary>
    /// 分享高级结算战绩到微信好友.
    /// </summary>
    public void ShareGaoJiJieSuanZhanJiToWeChat(string filepath)
    {
		if (IsWeChatShareSDKValid())
		{
			ShareContent content = new ShareContent();
			content.SetText("秦人麻将！咱老陕自己的麻将！快来加入吧！");
			content.SetTitle("秦人麻将");
			content.SetImagePath(filepath);
			//content.SetUrl(ShareUrl);
			content.SetShareType(ContentType.Image);
			ssdk.ShareContent(PlatformType.WeChat, content);
		}
		else
		{
			Debug.LogError("Unity:" + " WeChat Error ShareGaoJiJieSuanZhanJiToWeChat Error,IsWeChatShareSDKValid false! ");
#if UNITY_IPHONE
			Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
			return;
#endif
		}
	}
	/// <summary>
	/// 分享高级结算战绩到微信朋友圈.
	/// </summary>
	public void ShareGaoJiJieSuanZhanJiToWeChatMoments(string filepath)
    {
		bool n = false;
		n = IsWeChatMomentsSDKValid();

		Debug.Log("Unity:IsWeChatMomentsSDKValid" + n.ToString());
		if (n)
		{
			ShareContent content = new ShareContent();
			content.SetTitle("游戏邀请");
			content.SetText("秦人麻将！咱老陕自己的麻将！快来加入吧！");
			//content.SetUrl("http://www.qinrenmajiang.com/");
			//content.SetImageUrl("http://www.qinrenmajiang.com/images/gameicon.png");
			//content.SetUrl("http://www.qinrenmajiang.com/");
			content.SetImagePath(filepath);
			content.SetShareType(ContentType.Image);
			ssdk.ShareContent(PlatformType.WeChatMoments, content);
			Debug.Log("Unity:ShareInfoToWeChatMoments");
		}
		else
		{
			Debug.LogError("Unity:" + " WeChat Error ShareGaoJiJieSuanZhanJiToWeChatMoments Error,IsWeChatMomentsSDKValid false! ");
#if UNITY_IPHONE
			Application.OpenURL("https://itunes.apple.com/cn/app/%E5%BE%AE%E4%BF%A1/id414478124?mt=8");
			return;
#endif
		}
	}
}