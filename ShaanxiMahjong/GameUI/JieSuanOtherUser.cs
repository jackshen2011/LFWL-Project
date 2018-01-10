using UnityEngine;
using System.Collections;

class JieSuanOtherUser : MonoBehaviour
{
    // Use this for initialization
    public GameObject pPoChan;
    [HideInInspector]
    public TextBase tname;
    public TextBase tfen;
    /// <summary>
    /// 吃胡信息.
    /// </summary>
    public ImageBase ChiHuImg;
    /// <summary>
    /// 吃胡资源.
    /// ChiHuSp[0] -> 吃胡.
    /// ChiHuSp[1] -> 点炮.
    /// ChiHuSp[2] -> 平胡.
    /// ChiHuSp[3] -> 自摸.
    /// </summary>
    public Sprite[] ChiHuSp;
    /**
     * 玩家头像.
     */
    public ImageBase PlayerTX;
    /**
     * 男孩头像.
     */
    public Sprite[] BoyTx;
    /**
     * 女孩头像.
     */
    public Sprite[] GirlTx;
    /// <summary>
    /// 上家,对家,本家,下家的控制.
    /// </summary>
    public ImageBase PlayerBiaoJiImg;
    /// <summary>
    /// 胜利玩家的图片资源.
    /// WinPlayerSp[0] -> 上家.
    /// WinPlayerSp[1] -> 对家.
    /// WinPlayerSp[2] -> 本家.
    /// WinPlayerSp[3] -> 下家.
    /// </summary>
    public Sprite[] WinPlayerSp;
    /// <summary>
    /// 失败玩家的图片资源.
    /// LosePlayerSp[0] -> 上家.
    /// LosePlayerSp[1] -> 对家.
    /// LosePlayerSp[2] -> 本家.
    /// LosePlayerSp[3] -> 下家.
    /// </summary>
    public Sprite[] LosePlayerSp;
    /**
     * 玩家信息背景.
     */
    public ImageBase PlayerInfoBJ;
    /**
     * 结算界面赢家信息背景资源.
     */
    public Sprite YingJiaBJ;
    /**
     * 结算界面动画控制器.
     */
    public Animator JiSuanPlayerAni;
    public RuntimeAnimatorController[] AniList;
    /**
     * nsex性别, nsex == 0 -> 男孩, nsex == 1 -> 女孩.
     */
    public void Initial(int indexVal, int nsex, string sname, int nfen, int playerId)
    {
        //nfen = -100; //test.
        if (tname)
        {
            tname.text = sname;
        }

        if (tfen)
        {
            if (nfen>0)
            {
                tfen.text = "+" + nfen.ToString();
            }
            if (nfen < 0)
            {
                tfen.text = "-" + Mathf.Abs(nfen).ToString();
                //pPoChan.SetActive(true); //暂时不用显示"破产".
            }
            if (nfen == 0) {
                tfen.text = "不输不赢";
            }
        }

        if (PlayerTX != null) {
            PlayerData.PlayerSexEnum sexSt = (PlayerData.PlayerSexEnum)nsex;
            switch (sexSt) {
            case PlayerData.PlayerSexEnum.BOY:
                PlayerTX.sprite = BoyTx[playerId % BoyTx.Length];
                break;
            case PlayerData.PlayerSexEnum.GIRL:
                PlayerTX.sprite = GirlTx[playerId % GirlTx.Length];
                break;
            default:
				PlayerTX.sprite = GirlTx[playerId % GirlTx.Length];
				Debug.LogWarning("Unity:"+"Initial -> nsex is wrong! sexSt " + sexSt);
                break;
            }
        }
    }
    /// <summary>
    /// 设置玩家的顺序文字信息.
    /// indexVal == 0 -> 上家胜.
    /// indexVal == 1 -> 对家胜.
    /// indexVal == 2 -> 本家胜.
    /// indexVal == 3 -> 下家胜.
    /// </summary>
    public void SetPlayerBiaoJi(int indexVal, bool isYingJia)
    {
        if (isYingJia) {
            PlayerBiaoJiImg.sprite = WinPlayerSp[indexVal];
            PlayerInfoBJ.sprite = YingJiaBJ;
        }
        else {
            PlayerBiaoJiImg.sprite = LosePlayerSp[indexVal];
        }

        if (JiSuanPlayerAni != null) {
            JiSuanPlayerAni.runtimeAnimatorController = AniList[indexVal];
        }
    }
    /// <summary>
    /// indexVal == -1 -> 隐藏UI.
    /// indexVal == 0 -> 吃胡.
    /// indexVal == 1 -> 点炮.
    /// indexVal == 2 -> 平胡.
    /// indexVal == 3 -> 自摸.
    /// </summary>
    public void SetPlayerChiHuInfo(int indexVal)
    {
        if (indexVal < 0) {
            ChiHuImg.gameObject.SetActive(false);
        }
        else {
            ChiHuImg.sprite = ChiHuSp[indexVal];
        }
    }
}