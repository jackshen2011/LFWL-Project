using UnityEngine;
using System.Collections;

class ShareZhanJiCtrl : MonoBehaviour
{
    public enum ShareZhanJiEnum
    {
        WeChatFriend, //微信好友.
        WeChatFriendMoments, //微信朋友圈.
    }
    public Transform StartTr;
    public Transform EndTr;
    /// <summary>
    /// 分享给微信朋友.
    /// </summary>
    public void ShareToWeiXinFriend()
    {
        Debug.Log("Unity:"+"ShareToWeiXinFriend...");
        if (MainRoot._gUIModule != null) {
            Vector3 startPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(StartTr.position);
            Vector3 endPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(EndTr.position);
            MainRoot._gUIModule.CaptureScreenshotZhanJiToWeChat(startPos, endPos, ShareZhanJiEnum.WeChatFriend);
        }
    }
    /// <summary>
    /// 分享到微信朋友圈.
    /// </summary>
    public void ShareToWeiXinFriendsQuan()
    {
        Debug.Log("Unity:"+"ShareToWeiXinFriendsQuan...");
        if (MainRoot._gUIModule != null) {
            Vector3 startPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(StartTr.position);
            Vector3 endPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(EndTr.position);
            MainRoot._gUIModule.CaptureScreenshotZhanJiToWeChat(startPos, endPos, ShareZhanJiEnum.WeChatFriendMoments);
        }
    }

    #if UNITY_EDITOR
    void OnGUI()
    {
        Vector3 startPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(StartTr.position);
        Vector3 endPos = MainRoot._gUIModule.pUICamera.WorldToScreenPoint(EndTr.position);
        GUI.Box(new Rect(startPos.x, startPos.y, 10f, 10f), "1");
        GUI.Box(new Rect(endPos.x, endPos.y, 10f, 10f), "2");
    }
    #endif
}