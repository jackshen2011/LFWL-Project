using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using System.Text;

/// <summary>
/// 牌桌用户详情界面
/// </summary>
class UserDetailsInfo : UnModalUIBase
{
    /// <summary>
    /// 玩家的头像ui控制对象.
    /// </summary>
    public UserInfoPanel pUserInfoPanel;
    public PlayerBase pMyPlayer;
    /// <summary>
    /// 玩家头像img.
    /// </summary>
    public ImageBase photoicon;
    public ImageBase pSexImg;
    /// <summary>
    /// SexSpriteArray[0] -> 男图标.
    /// SexSpriteArray[1] -> 女图标.
    /// </summary>
    public Sprite[] SexSpriteArray;
    public TextBase userdetail_name;
    public TextBase userdetail_id;
    public TextBase userdetail_ip;
    /// <summary>
    /// 玩家信息坐标控制.
    /// </summary>
    public RectTransform UserInfoRectTr;
    /// <summary>
    /// 点赞/啤酒.
    /// </summary>
    public TextBase userdetail_count1;
    /// <summary>
    /// 牛粪.
    /// </summary>
    public TextBase userdetail_count2;
    /// <summary>
    /// 鲜花.
    /// </summary>
    public TextBase userdetail_count3;
    /// <summary>
    /// 拖鞋.
    /// </summary>
    public TextBase userdetail_count4;
    // Use this for initialization
    void Start ()
    {
        nUnModalUIIndex = MainRoot._gUIModule.pUnModalUIControl.GetOneUnModalUIIndex();
        if (!MainRoot._gUIModule.pUnModalUIControl.AddMyToUnModalUIList(this))
        {
            Debug.LogError("Unity:"+"UserDetailsInfo Init Fault!");
            DestroyImmediate(gameObject);
        }
    }
    public void Initial(PlayerBase pBase, UserInfoPanel userPanel)
    {
        pUserInfoPanel = userPanel;
        pMyPlayer = pBase;
        pSexImg.sprite = SexSpriteArray[(pMyPlayer.nSex==1)?0:1];
        string playerName = pMyPlayer.sUserName;
        userdetail_name.text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        userdetail_id.text = "ID:"+pMyPlayer.nUserId.ToString();
        MainRoot._gPlayerData.pAsyncImageDownload.SetAsyncImage((AsyncImageDownload.PlayerSeats)pMyPlayer.nUIUserSit, pMyPlayer.nUserId, (PlayerData.PlayerSexEnum)pMyPlayer.nSex, pMyPlayer.sHeadImgUrl, photoicon);
        /*userdetail_ip.text = MainRoot._gPlayerData.getIpAddress();
        int[] dtAy = new int[]{pMyPlayer.DianZanNum,
            pMyPlayer.NiuFenNum,
            pMyPlayer.FlowerNum};
        SetPlayerDetailsInfo(dtAy);*/
    }
    public void ShowTablePlayerIp()
    {//设置点开头像显示IP
        if (userdetail_ip != null)
        {
            if (pMyPlayer.sUserIp != null && !"".Equals(pMyPlayer.sUserIp))
            {
                userdetail_ip.text = "IP:"+pMyPlayer.sUserIp;
            }
            else
            {//不显示ip
                userdetail_ip.text = "";
            }
        }
    }
    /// <summary>
    /// 当点击玩家详情的道具信息.
    /// indexVal == 0 -> 点赞/啤酒.
    /// indexVal == 1 -> 牛粪.
    /// indexVal == 2 -> 鲜花.
    /// indexVal == 3 -> 拖鞋.
    /// </summary>
    public void OnItemBtnClick(int n)
    {
        if (pMyPlayer == null)
            return;
        if (pMyPlayer.playertype == PLAYERTYPE.MAIN_USER)
        {
            Debug.Log("Unity: Click self prop!");
            ((OtherPlayer)pMyPlayer).OnUseItem(n); //玩家使用自己的道具.
            return;
        }
        if (pMyPlayer.nCoinNum < 2200)
        {
            Debug.Log("Unity: Lack of gold coins!");
            //产生金币不足的消息.
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(42, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//金币不足买道具.
            return;
        }
        //产生-200金币的msg.
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMSubCoin(n, pUserInfoPanel); //给其它玩家赠送道具.
    }

    public override void DestroyThis()
    {
        MainRoot._gUIModule.pUnModalUIControl.DeletOneInList(nUnModalUIIndex);
    }
    /// <summary>
    /// Sets the player details info.
    /// dtAy[0] -> 点赞/啤酒.
    /// dtAy[1] -> 牛粪.
    /// dtAy[2] -> 鲜花.
    /// dtAy[3] -> 拖鞋.
    /// </summary>
    public void SetPlayerDetailsInfo(int[] dtAy)
    {
        string headTx = "X";
        userdetail_count1.text = headTx + dtAy[0].ToString();
        userdetail_count2.text = headTx + dtAy[1].ToString();
        userdetail_count3.text = headTx + dtAy[2].ToString();
        userdetail_count4.text = headTx + dtAy[3].ToString();
    }

    /// <summary>
    /// 显示淘汰赛玩家数据信息.
    /// </summary>
    public void InitialTaoTaiSaiUserInfo(TaoTaiSaiRuKouDlg.PlayerDt userDt)
    {
        if (userDt == null)
        {
            Debug.LogWarning("Unity: InitialTaoTaiSaiUserInfo -> userDt is null!");
            return;
        }
        pSexImg.sprite = SexSpriteArray[(userDt.UserSex == 1) ? 0 : 1];
        string playerName = userDt.UserName == null ? "" : userDt.UserName;
        userdetail_name.text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "...");
        userdetail_id.text = userDt.UserId.ToString();
        MainRoot._gPlayerData.pAsyncImageDownload.LoadingUrlImage(userDt.HeadUrl == null ? "" : userDt.HeadUrl, photoicon);
    }

    /// <summary>
    /// 设置淘汰赛玩家信息位置.
    /// </summary>
    public void SetTaoTaiSaiUserInfoPos(Vector2 startPos)
    {
        Vector3 pos = Vector2.Lerp(Vector2.zero, startPos, 0.6f);
        pos.z = 0f;
        UserInfoRectTr.anchoredPosition = pos;
    }
}