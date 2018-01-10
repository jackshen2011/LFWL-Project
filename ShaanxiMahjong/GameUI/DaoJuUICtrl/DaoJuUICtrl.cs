using UnityEngine;
using System.Collections;

class DaoJuUICtrl : MonoBehaviour
{
    /// <summary>
    /// DaoJuIndex 道具类型.
    /// DaoJuIndex == 0 -> 点赞/啤酒.
    /// DaoJuIndex == 1 -> 牛粪.
    /// DaoJuIndex == 2 -> 鲜花.
    /// DaoJuIndex == 3 -> 拖鞋.
    /// </summary>
    int DaoJuIndex = 0;
    /// <summary>
    /// 获得道具的玩家panel.
    /// </summary>
    UserInfoPanel UserInfoPanelCom;
    /// <summary>
    /// 初始化DaoJuUI.
    /// indexVal 道具类型.
    /// indexVal == 0 -> 点赞/啤酒.
    /// indexVal == 1 -> 牛粪.
    /// indexVal == 2 -> 鲜花.
    /// indexVal == 3 -> 拖鞋.
    /// userPanel -> 获赠玩家的UserInfoPanel信息.
    /// </summary>
    public void InitDaoJuUI(int indexVal, UserInfoPanel userPanel)
    {
        DaoJuIndex = indexVal;
        UserInfoPanelCom = userPanel;
    }
    /// <summary>
    /// 产生道具飞向玩家头像位置.
    /// </summary>
    public void SpawnDaoJuToPlayer()
    {
        string daoJuPrefab = "";
        switch (DaoJuIndex)
        {
            case 0:
                {
                    daoJuPrefab = "Prefab/GameDaoJu/DaoJu01-PiJiu";
                    break;
                }
            case 1:
                {
                    daoJuPrefab = "Prefab/GameDaoJu/DaoJu02-NiuFen";
                    break;
                }
            case 2:
                {
                    daoJuPrefab = "Prefab/GameDaoJu/DaoJu03-XianHua";
                    break;
                }
            case 3:
                {
                    daoJuPrefab = "Prefab/GameDaoJu/DaoJu04-TuoXie";
                    break;
                }
        }
        GameObject temp = (GameObject)GameObject.Instantiate(Resources.Load(daoJuPrefab), MainRoot._gUIModule.pUnModalUIControl.pGameUIView.gameObject.transform, false);
        UGuiTweenPos tweenPos = temp.GetComponent<UGuiTweenPos>();
        PlayerBase mainPlayerBase = MainRoot._gGameRoomCenter.gGameRoom.GetUserByServerSit(0);
        tweenPos.From = mainPlayerBase.pInfoPanel.transform.localPosition;
        tweenPos.To = UserInfoPanelCom.transform.localPosition;
        tweenPos.InitTransformAnimation();
        tweenPos.InitMsgSendInfo("OnEndDaoJuFly", new int[] {DaoJuIndex, UserInfoPanelCom.pMyPlayer.nUIUserSit, UserInfoPanelCom.pMyPlayer.nUserId});
    }
}