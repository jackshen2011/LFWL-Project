using UnityEngine;
using System.Collections;

public class ChatEmotion : MonoBehaviourIgnoreGui {

    public int mEmotionFlag = 0;
    public Animator ani;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    /// <summary>
    /// 点击表情
    /// </summary>
    public void OnClickChatEmotion()
    {
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            if (MainRoot._gRoomData)
            {
                RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerSendEmotion((int)MainRoot._gRoomData.cCurRoomData.eRoomType, MainRoot._gRoomData.cCurRoomData.nRoomId, mEmotionFlag);
            }
        }
    }
    public void PlayerEmotion(Animator ani)
    {

    }
}
