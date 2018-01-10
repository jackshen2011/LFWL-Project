using UnityEngine;
using System.Collections;

public class RoomCardNeDevice : MonoBehaviour {

    // Use this for initialization

    //标记异常玩家
    public static bool IsExceptionPlayer = false;

    void Start()
    {
        IsExceptionPlayer = false;
        GameObject.DontDestroyOnLoad(gameObject);
        this.Invoke("InvokeCreateRoomCardNetClientModule", 1.5f);
    }

    void InvokeCreateRoomCardNetClientModule()
    {
        RoomCardNet.RoomCardNetClientModule.CreateRoomCardNetClientModule();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (RoomCardNet.RoomCardNetClientModule.netModule != null && IsExceptionPlayer == false)
            RoomCardNet.RoomCardNetClientModule.netModule.NetworkUpdate();

    }
}
