using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 游戏状态，表示游戏进行到哪一步
/// 每一步只能进行自己的操作
/// </summary>
enum GAMESTATE
{
    Free,//空闲,未开始
    Deal,//发牌中
    Start,//开始游戏
    Over//打完了
}
/// <summary>
/// 方位，东南西北
/// </summary>
public enum DIRECTION
{
    EAST,
    SOUTH,
    WEST,
    NORTH
}
/// <summary>
/// 麻将姿势
/// </summary>
/// <remarks>
/// 初始模型 带影子的初始模型 明牌 立起来自己的牌
/// </remarks>
public enum MJKIND
{
    BLANK,
    SBLANK,
    LAY,
    STAND,
    HAND
}
/// <summary>
/// 麻将桌
/// 玩家自己总是面朝Z轴 右侧是X轴 桌子正面是Y轴
/// 方位索引 0下 1右 2上 3左<see cref="currentIndex"/>
/// 外部网络调用接口函数<see cref="PlayerNetTableOperation"/>
/// </summary>
/// <remarks>
/// 挂在麻将桌模型上，管理打麻将的相关操作
/// </remarks>
class MJGameTable : MonoBehaviourIgnoreGui
{
    /// <summary>
    /// 服务器发到桌面的消息类型
    /// </summary>
    public enum SMSG_TABLE
    {
        READY,//匹配成功
        START,//开始
        TAKE,//接牌
        PUTOUT,//出牌
        PENG,//碰牌
        GANG,//杠牌
        HU,//胡牌
        LIUJU,//流局
        RELINK//断线重连
    }
    class TTransform
    {
        Vector3 pos;
        Quaternion roto;
        public Vector3 position { get { return pos; } set { pos = value; } }
        public Quaternion rotation { get { return roto; } set { roto = value; } }

        public TTransform()
        {
            pos = new Vector3();
            roto = new Quaternion();
        }

    }
    //一个麻将模型的长宽高
    public const float width = 0.033f;//一个正面幺鸡宽度
    public const float height = 0.044f;//一个正面幺鸡高度
    public const float thickness = 0.022f;//一个正面幺鸡厚度
    public const float grooveLength = 0.580f;//凹槽的长度，用来摆牌墙的位置
    public const float clearance = 0.0004f;//麻将牌之间的间隔
    public const float readyToPutOutWidth = 0.05f;//接的牌与手牌之间的间隔
    public const float pengGangWidth = 0.01f;//碰杠吃之间的间隔
    public int putOutCountRow = 6;//打出去一行显示几张牌
    public Transform[] putOutPosition;//打出的牌的初始位置顺序下右上左
    public Transform[] blockPosition;//牌墙的初始位置
    public Transform[] sidePosition;//碰杠牌的初始位置
    public Transform[] huPosition;//胡牌位置

    public Transform[] handParent;//手牌的父对象
    TTransform[] handPosition = new TTransform[4];//手牌的初始位置
    Vector3[] putOutPositionLarge = { new Vector3(-0.269f,0.011f,-0.1413f),new Vector3(),new Vector3(0.265f,0.011f,0.129f),new Vector3()};//二人麻将打出的牌的初始位置 顺序下右上左

    public Transform groove;//凹槽物体
    public Renderer cover;//盖子
    GuiPlaneAnimationPlayer playGroove;//凹槽物体动画组件

    public Material[] directionShow;//东南西北显示切换的材质
    public Renderer[] directionMeshModel;//东南西北四个模型
    public GuiPlaneAnimationPlayer[] directionMeshPlayer;//东南西北材质明暗控制
    public Material[] materialMJ;//麻将的材质

    public Renderer tableClothModel;//桌布
    public Material partyTableClothMaterail;//车展活动桌布材质
    /// <summary>
    /// 桌面文本信息控制.
    /// </summary>
    public MJGameTableTextCtrl TableTextCom;
    /// <summary>
    /// 文本信息父级.
    /// </summary>
    public Transform TableTextTr;
    /// <summary>
    /// 牌桌风向遮挡对象.
    /// </summary>
    [HideInInspector]
    public GameObject TableFengXiangZheDang;
    /// <summary>
    /// 倒计时Sprite.
    /// </summary>
    public SpriteRenderer CountDownSprite;
    /// <summary>
    /// 倒计时Sprite列表.
    /// </summary>
    public Sprite[] CountDownSpArray;
    /// <summary>
    /// 是否超过60秒还没有出牌.
    /// </summary>
    bool IsChaoShiChuPai;
    [HideInInspector]
    public MaJiang[] mJPrefab = new MaJiang[4];//牌模型预置

    //两个骰子
    public Dice[] dice;


    GAMESTATE gameState = GAMESTATE.Free;
    DIRECTION userDirection = DIRECTION.EAST;//玩家的东南西北

    /// <summary>
    /// 当前出牌的方位 索引 0下 1右 2上 3左
    /// </summary>
    int currentIndex = -1;
    int banker = 1;//庄家位置 索引 0下 1右 2上 3左
    int bankerPlayerID;//庄家玩家id
    int[] playerIDSequence;//玩家打牌顺序数组

    List<List<MaJiang>> userHandMaJiang = new List<List<MaJiang>>();//玩家手牌
    List<List<List<MaJiang>>> userSideMaJiang = new List<List<List<MaJiang>>>();//玩家碰杠吃的牌
    List<List<MaJiang>> userPutOutMaJiang = new List<List<MaJiang>>();//玩家打出的牌
    List<List<MaJiang>> userBlockMaJiang = new List<List<MaJiang>>();//玩家面前的牌墙
    MaJiang[] userChoiceMaJiang = new MaJiang[4];//玩家接的牌
    MaJiang[] userHuMaJiang = new MaJiang[4];//玩家胡牌
    /// <summary>
    /// 桌面上有可能放胡的牌，打出和续杠 
    /// </summary>
    [HideInInspector]
    public MaJiang possibleHu;
    Transform pose;//定位器

    /// <summary>
    /// 玩家自己的手牌片操作管理
    /// </summary>
    public PlayerHandMaJiang playerHandMaJiang;

    int[] startPos;//摇完骰子开始抓牌的位置
    int takePos = 0;//下一次接牌的位置
    int gangPos = 0;//下一次杠牌的位置

    /**判断打出哪一张
    出牌前将手牌索引记录起来，然后向服务端传牌型
    服务端返回是否出牌成功以及出的牌
    */
    bool isRecent = true;//是否打出刚接的牌
    /// <summary>
    /// 将要打出的手牌索引
    /// 设置完成先发服务端
    /// </summary>
    private int putOutArrayIndex;

    int method;//打法
    NormalPlayMethod playMethod;//自己的牌数据 用于判断碰杠胡
    public Mahjong.TableFrameSink m_TableFrameSink;


    float lastCountdownTime = 0;//上次倒计时记录的时间数
    float startTime = 0;//准备接牌开始的时间数用以播放动画
    int remainBlockMJ;//剩余未接的麻将数

    bool sending = false;//正在发送网络消息

    bool isLastOperateGang = false;//接牌前的操作是否是杠，用于判断是否在后边接牌

    /// <summary>
    /// 服务端传过来开始的一堆数据暂存，因为要显示动画
    /// </summary>
    struct PlayTempInfo
    {
        public int dir, pos;//抓牌位置
        public int step;//抓牌步骤从庄家开始一人抓四张算一步
        public bool drawing;//抓牌中
        public int[] mj;//自己手牌
        public int firstMJ;//由于要动画，第一张接的牌要延迟接
        public byte showTips;//玩家提示延迟显示
        public int c, d;//第二次摇到的点数
    }
    PlayTempInfo grooveTempInfo;
    public void InitGameTable(int[]method)
    {
        if (MainRoot._gMainRoot.IsDateInCarPromotionDate())
        {
            if (tableClothModel && partyTableClothMaterail)
            {
                tableClothModel.sharedMaterial = partyTableClothMaterail;
            }
        }
        //物体动画脚本
        playGroove = groove.GetComponent<GuiPlaneAnimationPlayer>();
        //打法
        if (playMethod == null)
        {
            playMethod = new NormalPlayMethod();
        }
        playMethod.SetPlayMethod(method);
        m_TableFrameSink = new Mahjong.TableFrameSink();
        if (method!=null && method.Length>5)
        {
            m_TableFrameSink.huSevenPairs = (Mahjong.CMD_Set.HuSevenPairs)method[3];  //是否可胡七对
            m_TableFrameSink.HuCardsOptions = (System.UInt32)method[5];  //是否可胡七对
        }
        //牌池
        for (int i = 0; i < 4; i++)
        {
            userPutOutMaJiang.Add(new List<MaJiang>());
            userSideMaJiang.Add(new List<List<MaJiang>>());
            userHandMaJiang.Add(new List<MaJiang>());
            userBlockMaJiang.Add(new List<MaJiang>());
        }
        for (int i = 0; i < mJPrefab.Length; i++)
        {
            mJPrefab[i] = InitOneMJModel(MAJIANG.MJ01, (MJKIND)i, new Vector3(10, 10, -10), Quaternion.identity);
        }

        TableTextCom.SpawnMJTableText(TableTextTr);
        RefreshRoomTabelLevelShow();
        playerHandMaJiang = ((GameObject)Instantiate(Resources.Load("Prefabs/PlayerHandMaJiang"), transform.parent, false)).GetComponent<PlayerHandMaJiang>();
        //根据分辨率设置手牌第一张牌初始坐标
        if (SystemSetManage.ResolutionIndex != -1 && SystemSetManage.initializeSystemSetManage.HandCardPos_X != 0f)
        {
            playerHandMaJiang.ResetFirstHandPos(SystemSetManage.initializeSystemSetManage.HandCardPos_X, SystemSetManage.initializeSystemSetManage.HandCardPos_Y);
        }
        for (int i = 0; i < handPosition.Length; i++)
        {
            handPosition[i] = new TTransform();
        }
        if (!pose)
        {
            pose = InitTableEffect(1, Vector3.zero);
        }
        if (pose)
        {
            pose.gameObject.SetActive(false);
        }
        if (directionMeshPlayer.Length==0)
        {//东南西北初始化播放闪烁
            directionMeshPlayer = new GuiPlaneAnimationPlayer[directionMeshModel.Length];
            for (int i = 0; i < directionMeshModel.Length; i++)
            {
                directionMeshModel[i].sharedMaterial.color = new Color(1, 1, 1);
                
                directionMeshPlayer[i] = directionMeshModel[i].GetComponent<GuiPlaneAnimationPlayer>();
                directionMeshPlayer[i].Stop();
            }
        }
        handPosition[0].position = new Vector3(-0.255f, 0.022f, -0.404f);
        handPosition[0].rotation = Quaternion.Euler(90, 0, 0);
        handPosition[1].position = new Vector3(0.4085f, 0.022f, -0.2366f);
        handPosition[1].rotation = Quaternion.Euler(90, -90, 0);
        handPosition[2].position = new Vector3(0.229f, 0.022f, 0.3827f);
        handPosition[2].rotation = Quaternion.Euler(90, 180, 0);
        handPosition[3].position = new Vector3(-0.4157f, 0.022f, 0.2376f);
        handPosition[3].rotation = Quaternion.Euler(90, 90, 0);
    }
    /// <summary>
    /// 刷新麻将桌上的底分显示
    /// </summary>
    public void RefreshRoomTabelLevelShow()
    {
        if (MainRoot._gRoomData.cCurRoomData != null)
        {
            TableTextCom.RefreshShowRoomTabelInfo();
        }
    }
    /// <summary>
    /// 设置牌桌风向遮挡对象是否可见.
    /// </summary>
    public void SetActiveTableFengXiangZheDang(bool isActive)
    {
        TableFengXiangZheDang.SetActive(isActive);
    }
    /// <summary>
    /// 凹槽下去
    /// </summary>
    public void GrooveDown()
    {
        playGroove.Play();
        playGroove.DelegateOnPlayEndEvent = GrooveDownPlayEnd;
    }
    /// <summary>
    /// 凹槽升起
    /// </summary>
    public void GrooveUp()
    {
        playGroove.Play();
        playGroove.DelegateOnPlayEndEvent = GroovePlayEnd;
    }
    void GrooveDownPlayEnd()
    {
        playGroove.playProgress = 0f;
        //上牌
        InitBlockMJ(1);//摆牌墙

        playGroove.Overturn = !playGroove.Overturn;

        GrooveUp();
    }
    void GroovePlayEnd()
    {
        playGroove.playProgress = 0f;
        playGroove.Overturn = !playGroove.Overturn;
        //StartTakeMaJiang(1, grooveTempInfo.dir, grooveTempInfo.pos, grooveTempInfo.mj);//直接抓牌到手显示
        DiceRollPoint(grooveTempInfo.c, grooveTempInfo.d);//摇骰子
        dice[0].diceOver = DiceRollOver;
    }
    void DiceRollOver()
    {
        grooveTempInfo.drawing = true;//准备开始接牌，时间到了就开始接
    }
    /// <summary>
    /// 大家接完牌
    /// </summary>
    void OnDrawMaJiangEnd()
    {
        startPos = new int[] { grooveTempInfo.dir, grooveTempInfo.pos };//记录从这里抓牌
        if (IsTwoHumanRoom())
        {
            takePos = 13 * 2;//一人13张，接下来该摸牌的位置
        }
        else
        {
            takePos = 13 * 4;//一人13张，接下来该摸牌的位置
        }
        
        gangPos = playMethod.shortBlockNum * 4 + playMethod.longBlockNum * 4 - 2;//接下来该接杠牌的位置
        grooveTempInfo.drawing = false;

        gameState = GAMESTATE.Start;//开始打牌

        //停牌提示
        playerHandMaJiang.CheckTingPaiBtnShow();

        if (grooveTempInfo.firstMJ != -1)
        {
            UserTakeOneMaJiang(Banker, GetMaJiangByInt(grooveTempInfo.firstMJ));
        }
        if (grooveTempInfo.showTips != 0)
        {//延迟显示吃碰胡
            SetChiPengHuTipsDelay(grooveTempInfo.showTips);
            grooveTempInfo.showTips = 0;
        }
        if (bankerPlayerID == MainRoot._gPlayerData.nUserId) {
            if (MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pJieSuanUI != null
                && MainRoot._gUIModule.pUnModalUIControl.pGameUIView.pGaoJiJieSuanCtrl != null)
            {
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(2, MainRoot._gUIModule.pUnModalUIControl.pGameUIView); //庄家请出牌.
            }
        }
    }
    /// <summary>
    /// 根据服务端4个玩家数字id和庄家确认顺序
    /// </summary>
    /// <param name="playerID">{0,1,2,3}</param>
    /// <param name="banker">1</param>
    public void SetBankerByPlayerID(int[] playerID, int banker)
    {
        if (gameState != GAMESTATE.Free)
        {
            Debug.Log("Unity:"+"已经开始了,不能乱设！");
            return;
        }
        playerIDSequence = playerID;
        if (playerIDSequence.Length < 4)
        {
            Debug.Log("Unity:"+"请先设置玩家顺序数组");
            return;
        }
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer == null)
        {
            Debug.Log("Unity:"+"未连接");
            return;
        }
        //将南和北调换
        int temp;
        temp = playerIDSequence[1];
        playerIDSequence[1] = playerIDSequence[3];
        playerIDSequence[3] = temp;

        bankerPlayerID = banker;//庄家玩家id

        int mine = RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.m_PlayerID;
        int pSelf = -1, pBanker = -1;
        for (int i = 0; i < playerIDSequence.Length; i++)
        {
            if (mine == playerIDSequence[i])
            {
                pSelf = i;//找到自己
            }
            if (bankerPlayerID == playerIDSequence[i])
            {
                pBanker = i;//找到庄家
            }
        }
        if (pSelf == -1 || pBanker == -1)
        {
            Debug.Log("Unity:"+"未找到这种玩家");
            return;
        }

        SetUserDirection(pSelf);//设置自己的方位
        Banker = GetIndexByDirection((DIRECTION)pBanker);

        if (IsTwoHumanRoom())
        {//设置下二人麻将的位置
            putOutCountRow = 14;
            for (int i = 0; i < putOutPosition.Length; i++)
            {
                putOutPosition[i].position = putOutPositionLarge[i];
            }
        }
    }
    public int GetPlayerIndexByID(int id)
    {
        if (playerIDSequence.Length < 4)
        {
            return -1;
        }
        int player = -1;
        for (int i = 0; i < playerIDSequence.Length; i++)
        {
            if (id == playerIDSequence[i])
            {
                player = i;//找到自己
                break;
            }
        }
        if (player<0)
        {
            return -1;
        }
        return GetIndexByDirection((DIRECTION)player);
    }
    /// <summary>
    /// 是否是二人麻将
    /// </summary>
    /// <returns></returns>
    bool IsTwoHumanRoom()
    {
        if (MainRoot._gRoomData.cCurRoomData.IsErRenCardRoom())
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 开始游戏了
    /// </summary>
    /// <param name="nDir">东南西北</param>
    /// <param name="banker">庄家索引</param>
    /// <param name="a">骰子1</param>
    /// <param name="b">骰子2</param>
    /// <param name="c">骰子3</param>
    /// <param name="d">骰子4</param>
    /// <param name="x">抓牌索引</param>
    /// <param name="y">抓牌位置</param>
    /// <param name="selfMJ">自己的手牌</param>
    [System.Obsolete("This method is just for client test only due to the first two params!", false)]
    public void StartGame(int nDir,
        int banker,
        int a, int b, int c, int d,
        int x, int y,
        int[] selfMJ)
    {
        if (gameState != GAMESTATE.Free)
        {
            Debug.Log("Unity:"+"已经开始了！");
            return;
        }
        startTime = Time.time;
        gameState = GAMESTATE.Deal;

        SetUserDirection(nDir);//初始化方位
        Banker = banker;//设置庄家
        DiceRollPoint(a, b);//摇骰子
        //InitBlockMJ(1);//摆牌墙
        GrooveDown();//牌墙落下升起
        //StartTakeMaJiang(1, x, y, selfMJ);//初始化自己手牌
        grooveTempInfo = new PlayTempInfo();
        grooveTempInfo.dir = x;
        grooveTempInfo.pos = y;
        grooveTempInfo.step = -1;
        grooveTempInfo.drawing = false;
        grooveTempInfo.mj = selfMJ;
        grooveTempInfo.firstMJ = -1;
        grooveTempInfo.showTips = 0;

        //自己的手牌数据直接加载
        if (playMethod == null)
        {//报错
            Debug.Log("Unity:"+"玩法数据未初始化！请检查。");
        }
        playMethod.InitNormalPlayMethod(selfMJ);

        handParent[0].gameObject.SetActive(false);
    }
    public void StartGame(
    int a, int b, int c, int d,
    int x, int y,
    int[] selfMJ)
    {
        if (gameState != GAMESTATE.Free)
        {
            Debug.Log("Unity:"+"已经开始了！");
            return;
        }
        startTime = Time.time;
        gameState = GAMESTATE.Deal;

        DiceRollPoint(a, b);//摇骰子
        //InitBlockMJ(1);//摆牌墙
        GrooveDown();//牌墙落下升起
        //StartTakeMaJiang(1, x, y, selfMJ);//初始化自己手牌
        grooveTempInfo = new PlayTempInfo();
        grooveTempInfo.dir = x;
        grooveTempInfo.pos = y;
        grooveTempInfo.step = -1;
        grooveTempInfo.drawing = false;
        grooveTempInfo.mj = selfMJ;
        grooveTempInfo.firstMJ = -1;
        grooveTempInfo.showTips = 0;
        grooveTempInfo.c = c;
        grooveTempInfo.d = d;

        //自己的手牌数据直接加载
        if (playMethod == null)
        {//报错
            Debug.Log("Unity:"+"玩法数据未初始化！请检查。");
        }
        playMethod.InitNormalPlayMethod(selfMJ);

        handParent[0].gameObject.SetActive(false);
    }
    /// <summary>
    /// 设置吃碰胡提示延迟显示
    /// </summary>
    /// <param name="tipsFlag"></param>
    public void SetChiPengHuTipsDelay(byte tipsFlag)
    {
        if (gameState == GAMESTATE.Start)
        {//如果开始了就直接显示
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.GetChiPengHuTiShi().ShowChiPengHuTips((tipsFlag & Mahjong.CMD_SXMJ.WIK_CHI_HU) == Mahjong.CMD_SXMJ.WIK_CHI_HU, (tipsFlag & Mahjong.CMD_SXMJ.WIK_GANG) == Mahjong.CMD_SXMJ.WIK_GANG, (tipsFlag & Mahjong.CMD_SXMJ.WIK_PENG) == Mahjong.CMD_SXMJ.WIK_PENG, false);
        }
        else if (gameState == GAMESTATE.Deal)
        {
            if (grooveTempInfo.showTips==0)
            {
                grooveTempInfo.showTips = tipsFlag;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (grooveTempInfo.drawing)
        {//接牌过程
            float start = 3.5f;
            int maxStep;
            if (Time.time - startTime > start)
            {
                if (grooveTempInfo.step != Mathf.FloorToInt((Time.time - startTime - start) / 0.2f))
                {
                    if (IsTwoHumanRoom())
                    {
                        maxStep = 7;
                    }
                    else
                    {
                        maxStep = 15;
                    }
                    if (grooveTempInfo.step < maxStep)
                    {
                        grooveTempInfo.step = Mathf.FloorToInt((Time.time - startTime - start) / 0.2f);
                        StartTakeMaJiangOneByOne();
                    }
                    else
                    {
                        OnDrawMaJiangEnd();
                    }
                }
            }
        }
        if (currentIndex >= 0)
        {
            CountDown();
        }
        //左键执行测试代码
        /*if (Input.GetButtonUp("Fire1"))
        {
            PlayerNetTableOperation(SMSG_TABLE.RELINK,new object[] { 1});
        }*/
    }

    /// <summary>
    /// 扔出两个点数
    /// </summary>
    /// <param name="nPoint1">骰子1的点数，从1开始</param>
    /// <param name="nPoint2">骰子2的点数，从1开始</param>
    void DiceRollPoint(int nPoint1, int nPoint2)
    {
        if (dice.Length >= 2)
        {
            //播放警告声音
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong13);
            dice[0].CircleDiceTypicalNum(nPoint1);
            dice[1].CircleDiceTypicalNum(nPoint2);
        }
    }
    void SetUserDirection(DIRECTION dir)
    {
        SetUserDirection((int)dir);
    }
    /// <summary>
    /// 设置玩家方位，初始方位
    /// </summary>
    /// <param name="nDir">东南西北，不是索引</param>
    void SetUserDirection(int nDir)
    {
        if (!System.Enum.IsDefined(typeof(DIRECTION), nDir))
        {
            return;
        }
        userDirection = (DIRECTION)nDir;
        if (directionShow.Length < 8)
        {
            return;
        }
        if (directionMeshModel.Length < 4)
        {
            return;
        }

        for (int i = 0; i < directionMeshModel.Length; i++)
        {
            directionMeshModel[i].sharedMaterial = directionShow[(int)userDirection * 2];
            directionMeshModel[i].sharedMaterial.color = new Color(1,1,1);
        }
    }
    public GAMESTATE GameState
    {
        get { return gameState; }
    }
    ///<value>庄家</value>
    public int Banker
    {
        get
        {
            return banker;
        }
        set
        {
            banker = value;
        }
    }
    int RemainBlockMJ
    {
        get
        {
            return remainBlockMJ;
        }
        set
        {
            remainBlockMJ = value;
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SetpRemainCount(remainBlockMJ);
        }
    }
    /// <summary>
    /// 取当前碰杠信息
    /// </summary>
    /// <returns>牌 碰杠类型...</returns>
    public uint[] GetSelfPengGang()
    {
        List<uint> result = new List<uint>();
        for (int i = 0; i < playMethod.AllPengGangChi[0].Count; i++)
        {
            result.Add((uint)playMethod.AllPengGangChi[0][i].mj[0]);
            result.Add((uint)playMethod.AllPengGangChi[0][i].kind);
        }
        return result.ToArray();
    }
    /// <summary>
    /// 设置玩家当前出牌方位 游戏已经开始
    /// </summary>
    /// <param name="nIndex">索引 0代表下 1右 2上 3左</param>
    void SetUserCurrentDirection(int nIndex)
    {
        SetUserCurrentDirection(GetDirectionByIndex(nIndex));
    }
    /// <summary>
    /// 设置玩家当前出牌方位
    /// </summary>
    /// <param name="nDir">东南西北</param>
    void SetUserCurrentDirection(DIRECTION nDir)
    {
        if (currentIndex >= 0)
        {
            directionMeshModel[currentIndex].sharedMaterial =
                directionShow[(int)userDirection * 2];
            directionMeshPlayer[currentIndex].Stop();
        }

        currentIndex = GetIndexByDirection(nDir);
        lastCountdownTime = Time.time;
        directionMeshModel[GetIndexByDirection(nDir)].sharedMaterial =
            directionShow[(int)userDirection * 2 + 1];
        directionMeshPlayer[currentIndex].Play();
    }
    /// <summary>
    /// 从方向获取索引
    /// </summary>
    /// <param name="nDir">方向</param>
    /// <returns>索引 下右上左</returns>
    private int GetIndexByDirection(DIRECTION nDir)
    {
        int nIndex = (int)nDir;
        return (nIndex - (int)userDirection + 4) % 4;
    }
    /// <summary>
    /// 获取索引对应的方向<seealso cref="currentIndex"/>
    /// </summary>
    /// <param name="nIndex"></param>
    /// <returns>方位</returns>
    private DIRECTION GetDirectionByIndex(int nIndex)
    {
        DIRECTION nDir;
        nDir = (DIRECTION)((nIndex + (int)userDirection) % 4);
        return nDir;
    }
    /// <summary>
    /// 初始化牌墙
    /// </summary>
    /// <param name="method">打法</param>
    void InitBlockMJ(int method)
    {
        if (banker < 0)
        {
            Debug.Log("Unity:"+"请先设置庄家");
            return;
        }
        //按规则摆牌
        #region
        MaJiang tempObj;//麻将牌
        Vector3 pos;//世界位置
        Quaternion roto;//旋转
        float blank;//摆牌前面的空白长度
        MJKIND tempKind;//牌型
        int playerLen = playMethod.shortBlockNum;//闲家牌墙数
        int bankerLen = playMethod.longBlockNum;//庄家牌墙数
        float ax = 0, ay = 0, az = 0;//坐标偏移

        blank = (grooveLength - width * playerLen) / 2 + width / 2;
        RemainBlockMJ = playerLen * 4 + bankerLen * 4;//牌总数

        for (int i = 0; i < playerLen * 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (banker % 2 != j % 2)
                {
                    ay = ((i + 1) % 2) * thickness;
                    if (j == 0)
                    {
                        ax = -(i / 2) * width - blank;
                        az = 0;
                    }
                    else if (j == 1)
                    {
                        ax = 0;
                        az = (i / 2) * width + blank;
                    }
                    else if (j == 2)
                    {
                        ax = (i / 2) * width + blank;
                        az = 0;
                    }
                    else if (j == 3)
                    {
                        ax = 0;
                        az = -(i / 2) * width - blank;
                    }
                    pos = blockPosition[j].position;
                    roto = blockPosition[j].rotation;
                    pos = new Vector3(pos.x + ax, pos.y + ay, pos.z + az);
                    if (i % 2 == 1)
                    {
                        tempKind = MJKIND.SBLANK;
                    }
                    else
                    {
                        tempKind = MJKIND.BLANK;
                    }

                    tempObj = InitOneMJModel(MAJIANG.MJ01, tempKind, pos, roto);
                    tempObj.transform.SetParent(groove);
                    userBlockMaJiang[j].Add(tempObj);
                }
            }
        }

        blank = (grooveLength - width * bankerLen) / 2 + width / 2;

        for (int i = 0; i < bankerLen * 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (banker % 2 == j % 2)
                {
                    ay = ((i + 1) % 2) * thickness;
                    if (j == 0)
                    {
                        ax = -(i / 2) * width - blank;
                        az = 0;
                    }
                    else if (j == 1)
                    {
                        ax = 0;
                        az = (i / 2) * width + blank;
                    }
                    else if (j == 2)
                    {
                        ax = (i / 2) * width + blank;
                        az = 0;
                    }
                    else if (j == 3)
                    {
                        ax = 0;
                        az = -(i / 2) * width - blank;
                    }
                    pos = blockPosition[j].position;
                    roto = blockPosition[j].rotation;
                    pos = new Vector3(pos.x + ax, pos.y + ay, pos.z + az);
                    if (i % 2 == 1)
                    {
                        tempKind = MJKIND.SBLANK;
                    }
                    else
                    {
                        tempKind = MJKIND.BLANK;
                    }

                    tempObj = InitOneMJModel(MAJIANG.MJ01, tempKind, pos, roto);
                    tempObj.transform.SetParent(groove);
                    userBlockMaJiang[j].Add(tempObj);
                }
            }
        }
        #endregion
    }
    /// <summary>
    /// 根据信息摆牌墙
    /// </summary>
    /// <param name="lDice">骰子信息</param>
    /// <param name="gangCount">杠了几次</param>
    /// <param name="remainCount">剩余牌数</param>
    void SetBlockMJ(int lDice,int gangCount,int remainCount)
    {
        int[] tmp = GetFourDiceAndTakePosition(Banker, lDice);
        int[] gangIndexs=new int[gangCount];
        startPos = new int[] { tmp[4], tmp[5] };//记录从这里抓牌

        gangPos = playMethod.shortBlockNum * 4 + playMethod.longBlockNum * 4 - 2;//接下来该接杠牌的位置
        for (int i = 0; i < gangCount; i++)
        {
            gangIndexs[i] = gangPos;
            NextGang();
        }
        takePos = playMethod.shortBlockNum * 4 + playMethod.longBlockNum * 4 - remainCount - gangCount;
        //按规则摆牌
        #region
        MaJiang tempObj;//麻将牌
        Vector3 pos;//世界位置
        Quaternion roto;//旋转
        float blank;//摆牌前面的空白长度
        MJKIND tempKind;//牌型
        int playerLen = playMethod.shortBlockNum;//闲家牌墙数
        int bankerLen = playMethod.longBlockNum;//庄家牌墙数
        float ax = 0, ay = 0, az = 0;//坐标偏移

        RemainBlockMJ = remainCount;//牌数
        for (int i = 0; i < playerLen * 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (banker % 2 != j % 2)
                {
                    userBlockMaJiang[j].Add(null);
                }
            }
        }
        for (int i = 0; i < bankerLen * 2; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                if (banker % 2 == j % 2)
                {
                    userBlockMaJiang[j].Add(null);
                }
            }
        }
        for (int i = (playerLen+ bankerLen)*4-1; i > (playerLen + bankerLen) * 4 - 1 - remainCount-gangCount; i--)
        {
            int[] indexs = GetBlockMaJiangIndex(i);
            if (indexs.Length<2)
            {
                continue;
            }
            if (System.Array.IndexOf(gangIndexs,i) >= 0)
            {
                continue;
            }
            if (banker % 2 == indexs[0] % 2)
            {
                blank = (grooveLength - width * bankerLen) / 2 + width / 2;
            }
            else
            {
                blank = (grooveLength - width * playerLen) / 2 + width / 2;
            }
            ay = ((i + 1) % 2) * thickness;
            if (indexs[0] == 0)
            {
                ax = -(indexs[1] / 2) * width - blank;
                az = 0;
            }
            else if (indexs[0] == 1)
            {
                ax = 0;
                az = (indexs[1] / 2) * width + blank;
            }
            else if (indexs[0] == 2)
            {
                ax = (indexs[1] / 2) * width + blank;
                az = 0;
            }
            else if (indexs[0] == 3)
            {
                ax = 0;
                az = -(indexs[1] / 2) * width - blank;
            }
            pos = blockPosition[indexs[0]].position;
            roto = blockPosition[indexs[0]].rotation;
            pos = new Vector3(pos.x + ax, pos.y + ay, pos.z + az);
            if (i % 2 == 1)
            {
                tempKind = MJKIND.SBLANK;
            }
            else
            {
                tempKind = MJKIND.BLANK;
            }

            tempObj = InitOneMJModel(MAJIANG.MJ01, tempKind, pos, roto);
            tempObj.transform.SetParent(groove);
            userBlockMaJiang[indexs[0]][indexs[1]] = tempObj;
        }
        #endregion
    }
    /// <summary>
    /// 初始化打出去的麻将牌
    /// </summary>
    /// <param name="mjs">所有玩家的打出的牌</param>
    void SetPutOutMJ(int [][] mjs)
    {
        for (int nIndex = 0; nIndex < mjs.Length; nIndex++)
        {
            Vector3 pos;
            Quaternion roto;
            MaJiang obj;
            
            roto = putOutPosition[nIndex].transform.rotation;

            for (int i = 0; mjs[nIndex]!=null && i < mjs[nIndex].Length; i++)
            {
                float temp;
                float ax = 0, ay = 0, az = 0;

                pos = putOutPosition[nIndex].transform.position;

                MAJIANG mj = GetMaJiangByInt(mjs[nIndex][i]);
                if (userPutOutMaJiang[nIndex].Count >= 3 * putOutCountRow)
                {
                    temp = (userPutOutMaJiang[nIndex].Count - 2 * putOutCountRow);
                }
                else
                {
                    temp = userPutOutMaJiang[nIndex].Count % putOutCountRow;
                }
                if (nIndex == 0)
                {
                    ax = temp * width * 1.1f;
                    az = -Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
                }
                else if (nIndex == 1)
                {
                    ax = Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
                    az = temp * width * 1.1f;
                }
                else if (nIndex == 2)
                {
                    ax = -temp * width * 1.1f;
                    az = Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
                }
                else if (nIndex == 3)
                {
                    ax = -Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
                    az = -temp * width * 1.1f;
                }
                pos = new Vector3(pos.x + ax, pos.y, pos.z + az);
                obj = InitOneMJModel(mj, MJKIND.LAY, pos, roto);
                obj.transform.localScale = new Vector3(1.1f, 1, 1.1f);
                obj.transform.SetParent(putOutPosition[nIndex].parent);

                userPutOutMaJiang[nIndex].Add(obj);
            }
        }
    }
    /// <summary>
    /// 初始化所有碰杠牌
    /// </summary>
    /// <param name="info">4个玩家的uoyou碰杠信息</param>
    void SetPengGangMJ(int[][][] info)
    {
        for (int index = 0; index < info.Length; index++)
        {
            if (info[index] == null)
            {
                continue;
            }
            for (int i = 0; i < info[index].Length; i++)
            {
                if (info[index][i] == null)
                {
                    continue;
                }
                //info[index][i]-0操作玩家id，1-提供者id，2-操作类型，3-牌面 4-碰 杠 吃类型
                MAJIANG mj = GetMaJiangByInt(info[index][i][3]);
                int providerIndex = GetPlayerIndexByID(info[index][i][1]);
                int kind= info[index][i][4];
                //if ((info[index][i][2] & Mahjong.CMD_SXMJ.WIK_GANG) == Mahjong.CMD_SXMJ.WIK_GANG)
                //{

                //    if (info[index][i][0] == info[index][i][1])
                //    {
                //        kind = 3;
                //    }
                //    else
                //    {
                //        kind = 1;
                //    }
                //}
                switch (kind)
                {
                    case 0://碰
                        {
                            CreateAGroupPeng(index, (providerIndex - index + 3) % 4, mj);
                            playMethod.CreatePengGangChi(index, info[index][i][3], 0, (providerIndex - index + 3) % 4);
                            break;
                        }
                    case 1://直杠
                        {
                            CreateAGroupZhiGang(index, (providerIndex - index + 3) % 4, mj);
                            playMethod.CreatePengGangChi(index, info[index][i][3], 1, (providerIndex - index + 3) % 4);
                            break;
                        }
                    case 2://续杠
                        {
                            CreateAGroupXuGang(index, (providerIndex - index + 3) % 4, mj);
                            playMethod.CreatePengGangChi(index, info[index][i][3], 2, (providerIndex - index + 3) % 4);
                            break;
                        }
                    case 3://暗杠
                        {
                            CreateAGroupAnGang(index, mj);
                            playMethod.CreatePengGangChi(index, info[index][i][3], 3, (providerIndex - index + 3) % 4);
                            break;
                        }
                    default:
                        break;
                }
            }
        }
    }
    void SetHandMJ(int [] count,int[] self,int[] choice)
    {
        //发手牌
        MaJiang mj;
        Vector3 tempPos;
        Quaternion tempRoto;
        List<int> temp;
        float ax = 0, ay = 0, az = 0;//坐标偏移
        for (int i = 0; i < choice.Length; i++)
        {
            if (choice[i] == 255)
            {
                choice[i] = -1;
            }
        }
        if (choice[0]>0)
        {
            count[0]--;
            temp = new List<int>(self);
            temp.Remove(choice[0]);
            self = temp.ToArray();
        }
        for (int i = 1; i < choice.Length; i++)
        {
            if (choice[i] >= 0)
            {
                count[i]--;
            }
        }
        for (int i = 0; i < count.Length; i++)
        {
            for (int j = 0; j < count[i]; j++)
            {
                tempPos = handPosition[i].position;
                tempRoto = handPosition[i].rotation;
                if (i == 0)
                {
                    ax = j * width;
                    az = 0;
                }
                else if (i == 1)
                {
                    ax = 0;
                    az = j * width;
                }
                else if (i == 2)
                {
                    ax = -j * width;
                    az = 0;
                }
                else if (i == 3)
                {
                    ax = 0;
                    az = -j * width;
                }
                tempPos = new Vector3(tempPos.x + ax, tempPos.y + ay, tempPos.z + az);
                mj = InitOneMJModel(i == 0 ? GetMaJiangByInt(self[j]) : MAJIANG.MJ01, MJKIND.STAND, tempPos, tempRoto);
                mj.ArrayIndex = j;
                mj.transform.SetParent(handParent[i]);
                userHandMaJiang[i].Add(mj);
                if (i == 0)
                {
                    playerHandMaJiang.CreateOneHandMaJiangAtIndex(j, self[j], true);
                }
            }
            if (i == 0)
            {
                playMethod.InitNormalPlayMethod(self);
            }
            if (choice[i]>=0)
            {
                if (i == 0)
                {
                    SelfTakeOneMaJiang(GetIntByMaJiang(GetMaJiangByInt(choice[i])));
                }
                else
                {
                    OtherTakeOneMaJiang(i, GetIntByMaJiang(GetMaJiangByInt(choice[i])));
                }
            }
        }

    }
    /// <summary>
    /// 开始接牌 只能看见自己的手牌
    /// </summary>
    /// <param name="method">打法</param>
    /// <param name="nDir">从东南西北哪里开始接牌</param>
    /// <param name="pos">第几堆牌墙01234..开始接</param>
    /// <param name="handMaJiang">玩家自己手牌</param>
    [System.Obsolete("Don't use OldMethod, use StartTakeMaJiangOneByOne method instead for playanimation")]
    void StartTakeMaJiang(int method, int nDir, int pos, int[] handMaJiang)
    {
        if (!System.Enum.IsDefined(typeof(DIRECTION), nDir))
        {
            return;
        }
        /**
        这里直接接牌到手，之后得有接牌动画过程
        */
        //对应删除牌堆
        int nIndex;
        int n = 0, len = 0, p = pos;
        int[] sequence = { 0, 3, 2, 1 };//抓牌顺序
        nIndex = GetIndexByDirection((DIRECTION)nDir);

        startPos = new int[] { nIndex, pos };//记录从这里抓牌
        takePos = playMethod.shortBlockNum * 4;//一人13张，接下来该摸牌的位置
        gangPos = playMethod.shortBlockNum * 4 + playMethod.longBlockNum * 4 - 2;//接下来该接杠牌的位置

        for (int i = 0; i < takePos; i++)
        {
            len = userBlockMaJiang[(nIndex + n) % sequence.Length].Count / 2;
            if (len - p > 0)
            {
                Destroy(userBlockMaJiang[(nIndex + n) % sequence.Length][p * 2 + i % 2].gameObject);
                RemainBlockMJ--;

                p = i % 2 == 1 ? p + 1 : p;

                if (len - p <= 0)
                {
                    n++;
                    p = 0;
                }
            }
        }

        //发手牌
        MaJiang mj;
        Vector3 tempPos;
        Quaternion tempRoto;
        float ax = 0, ay = 0, az = 0;//坐标偏移
        for (int i = 0; i < 4; i++)
        {
            userHandMaJiang.Add(new List<MaJiang>());
            for (int j = 0; j < handMaJiang.Length; j++)
            {
                tempPos = handPosition[i].position;
                tempRoto = handPosition[i].rotation;
                if (i == 0)
                {
                    ax = j * width;
                    az = 0;
                }
                else if (i == 1)
                {
                    ax = 0;
                    az = j * width;
                }
                else if (i == 2)
                {
                    ax = -j * width;
                    az = 0;
                }
                else if (i == 3)
                {
                    ax = 0;
                    az = -j * width;
                }
                tempPos = new Vector3(tempPos.x + ax, tempPos.y + ay, tempPos.z + az);
                mj = InitOneMJModel(i == 0 ? GetMaJiangByInt(handMaJiang[j]) : MAJIANG.MJ01, MJKIND.STAND, tempPos, tempRoto);
                mj.ArrayIndex = j;
                mj.transform.SetParent(handParent[i]);
                userHandMaJiang[i].Add(mj);


            }
        }
        playerHandMaJiang.InitAGroupHandMaJiang(handMaJiang, -1);
        //自己的手牌数据
        if (playMethod == null)
        {//报错
            Debug.Log("Unity:"+"玩法数据未初始化！请检查。");
        }
        playMethod.InitNormalPlayMethod(handMaJiang);

    }
    void StartTakeMaJiangOneByOne()
    {
        //对应删除牌堆
        int nIndex;
        int n = 0, len = 0, p = grooveTempInfo.pos;
        int[] sequence = { 0, 3, 2, 1 };//抓牌顺序
        nIndex = grooveTempInfo.dir;
        nIndex = sequence[nIndex];//逆向
        int count;
        if (IsTwoHumanRoom())
        {
            count = grooveTempInfo.step > 5 ? 1 : 4;
            takePos = grooveTempInfo.step > 5 ? 12 * 2 + (grooveTempInfo.step - 6) : grooveTempInfo.step * count;
        }
        else
        {
            count = grooveTempInfo.step > 11 ? 1 : 4;
            takePos = grooveTempInfo.step > 11 ? 12 * 4 + (grooveTempInfo.step - 12) : grooveTempInfo.step * count;
        }
        for (int j = 0; j < takePos + count; j++)
        {
            len = userBlockMaJiang[(nIndex + n) % sequence.Length].Count / 2;
            if (len - p > 0)
            {
                if (j >= takePos)
                {
                    Destroy(userBlockMaJiang[(nIndex + n) % sequence.Length][p * 2 + j % 2].gameObject);
                    RemainBlockMJ--;
                }

                p = j % 2 == 1 ? p + 1 : p;

                if (len - p <= 0)
                {
                    n++;
                    p = 0;
                }
            }
        }

        //发手牌
        MaJiang mj;
        Vector3 tempPos;
        Quaternion tempRoto;
        float ax = 0, ay = 0, az = 0;//坐标偏移
        int i;
        int initIndex;

        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong03, true);

        if (IsTwoHumanRoom())
        {
            i = grooveTempInfo.step*2 % 4;
            initIndex = grooveTempInfo.step / 2 * 4;
        }
        else
        {
            i = grooveTempInfo.step % 4;
            initIndex = grooveTempInfo.step / 4 * 4;
        }
        for (int j = initIndex; j < initIndex + count; j++)
        {
            tempPos = handPosition[i].position;
            tempRoto = handPosition[i].rotation;
            if (i == 0)
            {
                ax = j * width;
                az = 0;
            }
            else if (i == 1)
            {
                ax = 0;
                az = j * width;
            }
            else if (i == 2)
            {
                ax = -j * width;
                az = 0;
            }
            else if (i == 3)
            {
                ax = 0;
                az = -j * width;
            }
            tempPos = new Vector3(tempPos.x + ax, tempPos.y + ay, tempPos.z + az);
            mj = InitOneMJModel(i == 0 ? GetMaJiangByInt(grooveTempInfo.mj[j]) : MAJIANG.MJ01, MJKIND.STAND, tempPos, tempRoto);
            mj.ArrayIndex = j;
            mj.transform.SetParent(handParent[i]);
            userHandMaJiang[i].Add(mj);
            if (i == 0)
            {
                playerHandMaJiang.CreateOneHandMaJiangAtIndex(j, grooveTempInfo.mj[j], j == 12 ? true : false);
            }
        }

        //playerHandMaJiang.InitAGroupHandMaJiang(handMaJiang, -1);
    }
    /// <summary>
    /// 获取牌墙的牌对象
    /// </summary>
    /// <param name="nCount">第几张牌，和抓牌起始位置有关</param>
    /// <returns></returns>
    MaJiang GetBlockMaJiang(int nCount)
    {
        int nIndex = startPos[0];
        int p = startPos[1];
        int n = 0;
        int[] sequence = { 0, 3, 2, 1 };//抓牌顺序
        int tempInt = 0;
        nIndex = sequence[nIndex];
        for (int i = 0; i < 5; i++)
        {
            tempInt += userBlockMaJiang[(nIndex + n) % sequence.Length].Count - p * 2;
            if (tempInt - nCount > 0)
            {
                return userBlockMaJiang[(nIndex + n) % sequence.Length][userBlockMaJiang[(nIndex + n) % sequence.Length].Count - tempInt + nCount];
            }
            n++;
            p = 0;
        }
        return null;
    }
    /// <summary>
    /// 获取牌墙的牌对象位置
    /// </summary>
    /// <param name="nCount">第几张牌，和抓牌起始位置有关</param>
    /// <returns></returns>
    int[] GetBlockMaJiangIndex(int nCount)
    {
        int[] pos=new int[2] { -1,-1};
        int nIndex = startPos[0];
        int p = startPos[1];
        int n = 0;
        int[] sequence = { 0, 3, 2, 1 };//抓牌顺序
        int tempInt = 0;
        nIndex = sequence[nIndex];//逆向
        for (int i = 0; i < 5; i++)
        {
            tempInt += userBlockMaJiang[(nIndex + n) % sequence.Length].Count - p * 2;
            if (tempInt - nCount > 0)
            {
                pos[0] = (nIndex + n) % sequence.Length;
                pos[1] = userBlockMaJiang[(nIndex + n) % sequence.Length].Count - tempInt + nCount;
                return pos;
            }
            n++;
            p = 0;
        }
        return null;
    }
    /// <summary>
    /// 杠一下游标变动
    /// </summary>
    /// <returns>返回旧的游标</returns>
    int NextGang()
    {
        int oldGangPos = gangPos;
        if (gangPos % 2 == 0)
        {
            gangPos++;
        }
        else {
            gangPos -= 3;
        }
        return oldGangPos;
    }
    /// <summary>
    /// 接一张牌游标变动
    /// </summary>
    /// <returns>返回旧的游标</returns>
    int NextTake()
    {
        return takePos++;
    }
    /// <summary>
    /// 玩家接牌，正常接
    /// </summary>
    /// <param name="nIndex">索引</param>
    /// <param name="maJiang">牌面</param>
    public void UserTakeOneMaJiang(int nIndex, MAJIANG maJiang)
    {
        UserTakeOneMaJiang(GetDirectionByIndex(nIndex), maJiang);
    }
    /// <summary>
    /// 获取最后操作的牌的牌面
    /// </summary>
    public MAJIANG GetCheckMJ
    {
        get
        {
            if (possibleHu)
            {
                return possibleHu.WhoAmI;
            }
            return MAJIANG.FA;
        }
    }
    /// <summary>
    /// 获取可能刚的牌型
    /// </summary>
    /// <returns>可能的列表</returns>
    public List<MAJIANG> GetPossibleGangList()
    {
        List<MAJIANG> list = new List<MAJIANG>();
        if (currentIndex == 0)
        {
            list = playMethod.PossibleSelfGang();
        }
        else
        {
            list.Add(possibleHu.WhoAmI);
        }
        return list;
    }
    /// <summary>
    /// 服务端返回调用
    /// </summary>
    /// <param name="msg">消息类型</param>
    /// <param name="args">消息附带参数</param>
    public void PlayerNetTableOperation(SMSG_TABLE msg, params object[] args)
    {
        byte[] CardData;
        int[] CardInt;
        int temp;
        switch (msg)
        {
            case SMSG_TABLE.READY:
				if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard)
				{
					SetBankerByPlayerID(new int[] { (int)(args[3]), (int)(args[6]), (int)(args[9]), (int)(args[12]) }, (int)(args[14]));
				}
                else if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_ThemeRace_Group)
                {
                    //SetBankerByPlayerID(new int[] { (int)(args[3]), (int)(args[6]), (int)(args[9]), (int)(args[12]) }, (int)(args[14]));
                    SetBankerByPlayerID(new int[] { (int)(args[9]), (int)(args[15]), (int)(args[21]), (int)(args[27]) }, (int)(args[32]));
                }
                else if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_MyRoom)
                {
                    //SetBankerByPlayerID(new int[] { (int)(args[3]), (int)(args[6]), (int)(args[9]), (int)(args[12]) }, (int)(args[14]));
                    //SetBankerByPlayerID(new int[] { (int)(args[9]), (int)(args[15]), (int)(args[21]), (int)(args[27]) }, (int)(args[32]));
                }
				else
				{
					SetBankerByPlayerID(new int[] { (int)(args[9]), (int)(args[15]), (int)(args[21]), (int)(args[27]) }, (int)(args[32]));
				}
                
                break;
            case SMSG_TABLE.START:
                CardData = ((System.IO.MemoryStream)(args[6])).ToArray();
                CardInt = new int[13];
                string str="";
                for (int i = 0; i < 13; i++)
                {
                    CardInt[i] = CardData[i];
                    str = string.Format("{0}{1:X}:", str, CardInt[i]);
                }
                Debug.Log(str);
                int[] tmp = GetFourDiceAndTakePosition(Banker, (int)(args[2]));
                StartGame(tmp[0], tmp[1], tmp[2], tmp[3], tmp[4], tmp[5], CardInt);
                break;
            case SMSG_TABLE.TAKE:
                temp = (byte)(args[3]);
                if (temp <= 0)
                {//别人接牌
                    UserTakeOneMaJiang(GetPlayerIndexByID((int)(args[2])), MAJIANG.MJ01);
                }
                else
                {
                    UserTakeOneMaJiang(GetPlayerIndexByID((int)(args[2])), GetMaJiangByInt(temp));
                }
                break;
            case SMSG_TABLE.PUTOUT:
                if ((int)args[2] == MainRoot._gPlayerData.nUserId) {
                    MainRoot._gUIModule.pUnModalUIControl.pGameUIView.DestroySMDengDaiChuPai();
                }
                CardInt = null;
                if (args.Length>4)
                {
                    CardData = ((System.IO.MemoryStream)(args[4])).ToArray();
                    CardInt = new int[CardData.Length];
                    for (int i = 0; i < CardInt.Length; i++)
                    {
                        CardInt[i] = CardData[i];
                    }
                }

                PutOutMJ(GetPlayerIndexByID((int)(args[2])), GetMaJiangByInt((byte)(args[3])), CardInt);//某个玩家打牌
                break;
            case SMSG_TABLE.PENG:
                PlayerDoPeng(GetPlayerIndexByID((int)(args[2])), GetMaJiangByInt((byte)(args[5])));//某个玩家碰牌
                break;
            case SMSG_TABLE.GANG:
                if ((int)(args[2]) == (int)(args[3]))
                {//自己杠
                    if (playMethod.isOtherCanXuGang(GetPlayerIndexByID((int)(args[2])), (byte)(args[5])))
                    {//续杠
                        PlayerDoXuGang(GetPlayerIndexByID((int)(args[2])), GetMaJiangByInt((byte)(args[5])), GetMaJiangByInt(-1));//某个玩家杠牌
                    }
                    else
                    {//暗杠
                        PlayerDoAnGang(GetPlayerIndexByID((int)(args[2])), GetMaJiangByInt((byte)(args[5])), GetMaJiangByInt(-1));//某个玩家杠牌
                    }
                }
                else
                {
                    PlayerDoZhiGang(GetPlayerIndexByID((int)(args[2])), GetMaJiangByInt((byte)(args[5])), GetMaJiangByInt(-1));//某个玩家杠牌
                }
                break;
            case SMSG_TABLE.HU:
                int[][] allMJ = new int[4][];
                int huMJ = (byte)(args[29]);
                int huIndex = GetPlayerIndexByID((int)(args[28]));
                List<int> tempList;
                for (int i = 0; i < 4; i++)
                {
                    CardData = ((System.IO.MemoryStream)(args[4 + i * 6])).ToArray();
                    int[] cardInt = new int[(byte)(args[3 + i * 6])];
                    int currentIndex;
                    for (int j = 0; j < cardInt.Length; j++)
                    {
                        cardInt[j] = CardData[j];
                    }
                    currentIndex = GetPlayerIndexByID((int)(args[2 + i * 6]));
                    if (huIndex == currentIndex)
                    {
                        tempList = new List<int>(cardInt);
                        tempList.Remove(huMJ);
                        allMJ[currentIndex] = tempList.ToArray();
                    }
                    else
                    {
                        allMJ[currentIndex] = cardInt;
                    }
                }
                PlayerDoHu(huIndex, huMJ, allMJ);//某个玩家打牌
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong20, true);
                break;
            case SMSG_TABLE.LIUJU:
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong09, true);
                OnGameOver();
                break;
            case SMSG_TABLE.RELINK:
                {
                    ClearGame();//先清场
                    int[] dimension;
                    int[][] dimension2;
                    int[][][] dimension3;
                    string sInfo;
                    //设置顺序以及庄家
                    SetBankerByPlayerID(new int[] { (int)args[8], (int)args[10], (int)args[12], (int)args[14] }, (int)args[2]);
                    //设置锅底长城摆法
                    //设置玩家碰杠吃
                    int gangCount=0;
                    dimension3 = new int[4][][];
                    for (int i = 0; i < dimension3.Length; i++)
                    {
                        sInfo = (string)args[27 + i * 2];

                        if (sInfo.Equals(""))
                        {
                            continue;
                        }

                        string[] sAll = sInfo.Split('#');
                        string[] one;

                        dimension2 = new int[sAll.Length][];

                        for (int j = 0; j < sAll.Length; j++)
                        {
                            if (sAll[j].Equals(""))
                            {
                                continue;
                            }
                            one = sAll[j].Split(':');
                            int t= System.Convert.ToInt32(one[2]);
                            if ((t & Mahjong.CMD_SXMJ.WIK_GANG) == Mahjong.CMD_SXMJ.WIK_GANG)
                            {
                                gangCount++;
                            }
                        }
                    }
                    SetBlockMJ((int)args[3], gangCount, (int)args[4]);
                    //设置四个玩家打出去的牌
                    dimension2 = new int[4][];
                    for (int i = 0; i < dimension2.Length; i++)
                    {
                        int userIndex= GetPlayerIndexByID((int)args[18+i*2]);
                        if ((int)args[18 + i * 2] == 0)
                        {
                            continue;
                        }
                        CardData = ((System.IO.MemoryStream)(args[19 + i * 2])).ToArray();
                        CardInt = new int[CardData.Length];
                        for (int j = 0; j < CardInt.Length; j++)
                        {
                            CardInt[j] = CardData[j];
                        }
                        dimension2[userIndex] = CardInt;
                    }
                    SetPutOutMJ(dimension2);
                    //设置玩家碰杠吃
                    dimension3 = new int[4][][];
                    for (int i = 0; i < dimension3.Length; i++)
                    {
                        int userIndex = GetPlayerIndexByID((int)args[26 + i * 2]);
                        if ((int)args[26 + i * 2] == 0)
                        {
                            continue;
                        }
                        sInfo = (string)args[27 + i * 2];

                        if (sInfo.Equals(""))
                        {
                            continue;
                        }

                        string[] sAll = sInfo.Split('#');
                        string[] one;

                        dimension2 = new int[sAll.Length][];

                        for (int j = 0; j < sAll.Length; j++)
                        {
                            if (sAll[j].Equals(""))
                            {
                                continue;
                            }
                            one = sAll[j].Split(':');
                            dimension = new int[one.Length];

                            for (int k = 0; k < one.Length; k++)
                            {
                                dimension[k] = System.Convert.ToInt32(one[k]);
                            }

                            dimension2[j] = dimension;
                        }

                        dimension3[userIndex] = dimension2;
                    }
                    SetPengGangMJ(dimension3);
                    //设置玩家手牌
                    int[] count=new int[4];
                    int[] choice=new int[4] {-1,-1,-1,-1};
                    for (int i = 0; i < count.Length; i++)
                    {
                        int userIndex = GetPlayerIndexByID((int)args[8 + i * 2]);
                        if ((int)args[8 + i * 2] == 0)
                        {
                            continue;
                        }
                        count[userIndex] = (int)args[9+i*2];
                    }
                    CardData = ((System.IO.MemoryStream)(args[17])).ToArray();
                    CardInt = new int[CardData.Length];//自己手牌
                    for (int j = 0; j < CardInt.Length; j++)
                    {
                        CardInt[j] = CardData[j];
                    }
                    int currentIndex = GetPlayerIndexByID((int)args[5]);
                    choice[currentIndex] = (int)(byte)args[6];
                    SetHandMJ(count, CardInt, choice);
                    //设置该谁
                    SetUserCurrentDirection(currentIndex);
                    gameState = GAMESTATE.Start;
                    if (userPutOutMaJiang[currentIndex].Count>0)
                    {//可能胡的牌赋值
                        possibleHu = userPutOutMaJiang[currentIndex][userPutOutMaJiang[currentIndex].Count - 1];
                    }
                    if (choice[0]<=0)
                    {//停牌提示
                        playerHandMaJiang.CheckTingPaiBtnShow();
                    }
                    //隐藏桌面自己手牌
                    handParent[0].gameObject.SetActive(false);
                    /*#region 测试复盘代码
                    //设置顺序以及庄家
                    SetBankerByPlayerID(new int[] { 10001, 10002, 10004, 10003 }, 1);
                    //设置锅底长城摆法
                    System.Random randomObj = new System.Random();
                    int lSiceCount = 0;
                    lSiceCount = (randomObj.Next(0, 6));
                    lSiceCount += (randomObj.Next(0, 6)) << 4;
                    lSiceCount += (randomObj.Next(0, 6)) << 8;
                    lSiceCount += (randomObj.Next(0, 6)) << 12;
                    SetBlockMJ(lSiceCount, 3, 20);
                    //设置四个玩家打出去的牌
                    SetPutOutMJ(GetAGroupHu());
                    //设置玩家碰杠吃
                    SetPengGangMJ(new int[][][] { new int[][] { new int[] { 1, 1, 2, 18 } }, new int[][] { new int[] { 1, 2, 3, 1 } }, new int[][] { new int[] { 1, 1, 1, 4 } }, new int[][] { new int[] { 1, 1, 0, 3 } } });
                    //设置玩家手牌
                    SetHandMJ(new int[] { 13, 4, 13, 13 }, new int[13] { 33, 33, 33, 34, 34, 34, 35, 35, 35, 36, 36, 36, 37 }, new int[] {0,0,3,0 });
                    //设置该谁
                    SetUserCurrentDirection(2);
                    #endregion*/
                    break;
                }
            default:
                break;
        }
    }
    /// <summary>
    /// 玩家接牌,正常接
    /// </summary>
    /// <param name="nDir">方位</param>
    /// <param name="maJiang">牌型</param>
    public void UserTakeOneMaJiang(DIRECTION nDir, MAJIANG maJiang)
    {
        if (gameState == GAMESTATE.Deal)
        {//正在发牌的时候延迟接牌
            grooveTempInfo.firstMJ = GetIntByMaJiang(maJiang);
            return;
        }
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (RemainBlockMJ <= 0)
        {
            Debug.Log("Unity:"+"没有牌可以接了");
            return;
        }
        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong06, true);
        if (isLastOperateGang)
        {
            MaJiang majang = GetBlockMaJiang(NextGang());
            if (majang == null)
            {//杠是反的接，可能接牌的时候这张牌已经被杠走，那么就接下一张（理论上只有可能是走后一张牌）
                majang = GetBlockMaJiang(NextGang());
            }
            if (majang != null)
                Destroy(majang.gameObject);//接一张牌销毁牌墙的一张
        }
        else
        {
            MaJiang majang = GetBlockMaJiang(NextTake());
            if (majang == null)
            {//杠是反的接，可能接牌的时候这张牌已经被杠走，那么就接下一张（理论上只有可能是走后一张牌）
                majang = GetBlockMaJiang(NextTake());
            }
            if (majang != null)
                Destroy(majang.gameObject);//接一张牌销毁牌墙的一张
        }
        isLastOperateGang = false;//接完牌重置
        int nIndex = GetIndexByDirection(nDir);
        if (nIndex == 0)
        {
            SelfTakeOneMaJiang(GetIntByMaJiang(maJiang));
        }
        else
        {
            OtherTakeOneMaJiang(nIndex, GetIntByMaJiang(maJiang));
        }
        SetUserCurrentDirection(nIndex);
        RemainBlockMJ--;
    }
    /// <summary>
    /// 自己接牌
    /// </summary>
    /// <param name="maJiang"></param>
    private void SelfTakeOneMaJiang(int maJiang)
    {
        //接一张牌
        MaJiang mj;
        Vector3 pos;
        Quaternion roto;
        pos = handPosition[0].position;
        roto = handPosition[0].rotation;
        pos = new Vector3(pos.x + (userHandMaJiang[0].Count - 1) * width + readyToPutOutWidth, pos.y, pos.z);
        mj = InitOneMJModel(GetMaJiangByInt(maJiang), MJKIND.STAND, pos, roto);
        mj.transform.SetParent(handParent[0]);

        userChoiceMaJiang[0] = mj;
        possibleHu = mj;//检测自摸

        playMethod.UserCurrentTakeMaJiang = GetIntByMaJiang(mj.WhoAmI);

        //片手牌
        playerHandMaJiang.TakeOneHandMaJiang(GetMaJiangByInt(maJiang));
    }
    /// <summary>
    /// 其他玩家接牌
    /// </summary>
    /// <param name="nIndex">索引，不是方位</param>
    /// <param name="maJiang">不给显示</param>
    private void OtherTakeOneMaJiang(int nIndex, int maJiang)
    {
        //接一张牌
        MaJiang mj;
        Vector3 pos;
        Quaternion roto;
        float ax = 0, ay = 0, az = 0;//坐标偏移

        pos = handPosition[nIndex].position;
        roto = handPosition[nIndex].rotation;
        if (nIndex == 1)
        {
            ax = 0;
            az = userHandMaJiang[nIndex].Count * width + readyToPutOutWidth;
        }
        else if (nIndex == 2)
        {
            ax = -userHandMaJiang[nIndex].Count * width - readyToPutOutWidth;
            az = 0;
        }
        else if (nIndex == 3)
        {
            ax = 0;
            az = -userHandMaJiang[nIndex].Count * width - readyToPutOutWidth;
        }
        pos = new Vector3(pos.x + ax, pos.y, pos.z + az);
        mj = InitOneMJModel(MAJIANG.MJ01, MJKIND.STAND, pos, roto);
        mj.transform.SetParent(handParent[nIndex]);
        userChoiceMaJiang[nIndex] = mj;

        possibleHu = mj;//检测自摸
    }
    /// <summary>
    /// 将最后一张牌作为刚接的牌，用于碰后
    /// </summary>
    /// <param name="nIndex">玩家</param>
    void TakeLastMJ(int nIndex)
    {
        userChoiceMaJiang[nIndex] = userHandMaJiang[nIndex][userHandMaJiang[nIndex].Count - 1];
        if (nIndex == 0)
        {
            userChoiceMaJiang[0].ArrayIndex = -1;
        }
        userHandMaJiang[nIndex].RemoveAt(userHandMaJiang[nIndex].Count - 1);
        RefreshHandMaJiang(nIndex);
    }
    /// <summary>
    /// 根据数据直接重置桌子的自己的牌
    /// </summary>
    /// <param name="mj">手牌列表</param>
    /// <param name="choiceMj">接的牌</param>
    void ResetSelfHandMaJiang(int [] mj,int choiceMj)
    {
        if (mj == null)
        {
            return;
        }
        int zeroIndex = mj.Length;
        for (int i = 0; i < mj.Length; i++)
        {
            if (mj[i] == 0)
            {
                zeroIndex = i;
                break;
            }
        }
        for (int i = userHandMaJiang[0].Count-1; i >= zeroIndex; i--)
        {//删除多余的牌
            if (userHandMaJiang[0][i] != null)
            {
                Destroy(userHandMaJiang[0][i].gameObject);
                userHandMaJiang[0].RemoveAt(i);
            }
        }
        for (int i = 0; i < zeroIndex - userHandMaJiang[0].Count; i++)
        {
            userHandMaJiang[0].Add(null);
        }
        for (int i = 0; i < zeroIndex; i++)
        {
            if (userHandMaJiang[0][i] != null)
            {
                ChangeMaJiangFace(userHandMaJiang[0][i],GetMaJiangByInt(mj[i]));
            }
            else
            {
                MaJiang oneMj = InitOneMJModel(GetMaJiangByInt(mj[i]), MJKIND.STAND, Vector3.zero, Quaternion.identity);
                oneMj.ArrayIndex = i;
                oneMj.transform.SetParent(handParent[0]);
                userHandMaJiang[0][i] = oneMj;
            }
        }
        if (choiceMj>0)
        {
            if (userChoiceMaJiang[0] != null)
            {
                ChangeMaJiangFace(userChoiceMaJiang[0], GetMaJiangByInt(choiceMj));
            }
            else
            {
                MaJiang oneMj = InitOneMJModel(GetMaJiangByInt(choiceMj), MJKIND.STAND, Vector3.zero, Quaternion.identity);
                oneMj.transform.SetParent(handParent[0]);
                userChoiceMaJiang[0] = oneMj;
            }
        }
        else
        {
            if (userChoiceMaJiang[0] != null)
            {
                Destroy(userChoiceMaJiang[0].gameObject);
                userChoiceMaJiang[0] = null;
            }
        }
        RefreshHandMaJiang(0);
    }
    /// <summary>
    /// 整理手牌坐标
    /// </summary>
    /// <param name="nIndex"></param>
    private void RefreshHandMaJiang(int nIndex)
    {
        Vector3 pos;
        float ax = 0, ay = 0, az = 0;

        pos = handPosition[nIndex].position;

        for (int i = 0; i < userHandMaJiang[nIndex].Count; i++)
        {
            Vector3 tempPos = pos;
            if (nIndex == 0)
            {
                ax = i * width;
                az = 0;
            }
            else if (nIndex == 1)
            {
                ax = 0;
                az = i * width;
            }
            else if (nIndex == 2)
            {
                ax = -i * width;
                az = 0;
            }
            else if (nIndex == 3)
            {
                ax = 0;
                az = -i * width;
            }
            tempPos = new Vector3(tempPos.x + ax, tempPos.y, tempPos.z + az);
            userHandMaJiang[nIndex][i].transform.position = tempPos;
            userHandMaJiang[nIndex][i].ArrayIndex = i;
        }
        if (userChoiceMaJiang[nIndex] != null)
        {
            if (nIndex == 0)
            {
                ax = (userHandMaJiang[nIndex].Count - 1) * width + readyToPutOutWidth;
                az = 0;
            }
            else if (nIndex == 1)
            {
                ax = 0;
                az = (userHandMaJiang[nIndex].Count - 1) * width + readyToPutOutWidth;
            }
            else if (nIndex == 2)
            {
                ax = -(userHandMaJiang[nIndex].Count - 1) * width - readyToPutOutWidth;
                az = 0;
            }
            else if (nIndex == 3)
            {
                ax = 0;
                az = -(userHandMaJiang[nIndex].Count - 1) * width - readyToPutOutWidth;
            }
            userChoiceMaJiang[nIndex].transform.position = new Vector3(pos.x + ax, pos.y, pos.z + az);
            userChoiceMaJiang[nIndex].ArrayIndex = -1;
        }
    }
    /// <summary>
    /// 打牌期间强行刷新玩家自己手牌
    /// </summary>
    void ForceRefreshSelfMaJiangDuringPlaying(int [] allMJ,int choice)
    {
        //刷桌子自己的牌
        ResetSelfHandMaJiang(allMJ, choice);
        //刷手牌信息
        playMethod.InitNormalPlayMethod(allMJ);
        playMethod.UserCurrentTakeMaJiang = choice;
        //刷手牌
        playerHandMaJiang.ResetHandMJ(allMJ, choice);

        if (choice<=0)
        {
            playerHandMaJiang.CheckTingPaiBtnShow();
        }
    }
    /// <summary>
    /// 打出一张牌，从服务端返回才调用
    /// 之前必须先设置putOutArrayIndex<see cref="putOutArrayIndex"/>的指
    /// </summary>
    /// <param name="playerIndex">玩家标记</param>
    /// <param name="mj">牌型</param>
    public void PutOutMJ(int playerIndex, MAJIANG mj,int[] allMJ=null)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        int nIndex = playerIndex;//暂且认为是索引
        int temp = 0;
        Vector3 pos;
        Quaternion roto;
        MaJiang obj;
        float ax = 0, ay = 0, az = 0;
        bool isError=false;//true表示需要强刷手牌
        pos = putOutPosition[nIndex].transform.position;
        roto = putOutPosition[nIndex].transform.rotation;
        sending = false;

        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong02, true);

        if (userChoiceMaJiang[nIndex] == null)
        {//报错，要打的牌不在
            Debug.Log("Unity:"+"如果让你成功，你就小相公了！");
            return;
        }
        if (userPutOutMaJiang[nIndex].Count >= 3 * putOutCountRow)
        {
            temp = (userPutOutMaJiang[nIndex].Count - 2 * putOutCountRow);
        }
        else
        {
            temp = userPutOutMaJiang[nIndex].Count % putOutCountRow;
        }
        if (nIndex == 0)
        {
            if (MainRoot._gPlayerData.m_bTrustee)
            {
                isRecent = true;
            }
            if (isRecent)
            {
                if (playMethod.UserCurrentTakeMaJiang != GetIntByMaJiang(mj))
                //if (true)
                {
                    Debug.Log("Unity:"+"打出的刚接的牌和服务端不一致"+ playMethod.UserCurrentTakeMaJiang+":"+ GetIntByMaJiang(mj) + ":" + playMethod.UserAllMaJiang.ToString());
                    //刷一下
                    ForceRefreshSelfMaJiangDuringPlaying(allMJ,-1);
                    isError =true;
                }
            }
            else
            {
                if (playMethod.UserAllMaJiang[putOutArrayIndex] != GetIntByMaJiang(mj))
                //if (true)
                {
                    Debug.Log("Unity:"+"打出的牌和服务端不一致" + playMethod.UserAllMaJiang[putOutArrayIndex] + ":" + GetIntByMaJiang(mj)+":"+ playMethod.UserAllMaJiang.ToString());
                    //刷一下
                    ForceRefreshSelfMaJiangDuringPlaying(allMJ, -1);
                    isError =true;
                }
            }
            ax = temp * width * 1.1f;
            az = -Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
        }
        else if (nIndex == 1)
        {
            ax = Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
            az = temp * width * 1.1f;
        }
        else if (nIndex == 2)
        {
            ax = -temp * width * 1.1f;
            az = Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
        }
        else if (nIndex == 3)
        {
            ax = -Mathf.Min(userPutOutMaJiang[nIndex].Count / putOutCountRow, 2) * height * 1.1f;
            az = -temp * width * 1.1f;
        }
        pos = new Vector3(pos.x + ax, pos.y, pos.z + az);
        obj = InitOneMJModel(mj, MJKIND.LAY, pos, roto);
        obj.transform.localScale = new Vector3(1.1f, 1, 1.1f);
        obj.transform.SetParent(putOutPosition[nIndex].parent);

        //播放声音
        PlayerBase tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerIDSequence[(playerIndex-GetIndexByDirection(DIRECTION.EAST)+4)%4]);
        GameAudioManage.GmAudioEnum sound = (GameAudioManage.GmAudioEnum)GetMaJiangMaterialIndex(mj);
        if (tempplay.nSex == 1)
        {//男
        }
        else
        {//女
            sound = (GameAudioManage.GmAudioEnum)(GetMaJiangMaterialIndex(mj)+35);
        }
        MainRoot._gGameAudioManage.PlayGameAudio(sound);

        possibleHu = obj;//记录下，有可能放胡

        userPutOutMaJiang[nIndex].Add(obj);
        if (pose)
        {
            pose.position = new Vector3(obj.transform.position.x, 0.04f, obj.transform.position.z);
            pose.gameObject.SetActive(true);
        }
        if (!isError)
        {
            //删除手牌
            InsertHandMaJiang(nIndex);
        }

        if (bankerPlayerID == MainRoot._gPlayerData.nUserId) {
            MainRoot._gUIModule.pUnModalUIControl.pGameUIView.OnSysMsgWaitEventHappen(2); //销毁庄家出牌提示.
        }

        isRecent = true;//默认打接的牌以便托管
    }
    /// <summary>
    /// 将接的牌插入自己手牌
    /// <see cref="isRecent"/>
    /// <see cref="putOutArrayIndex"/>
    /// </summary>
    /// <param name="nIndex">位置</param>
    void InsertHandMaJiang(int nIndex)
    {
        if (userChoiceMaJiang[nIndex] == null)
        {//报错，要打的牌不在
            Debug.Log("Unity:"+"报错，要打的刚接的牌不在");
            return;
        }
        if (nIndex == 0)
        {//自己插牌
            if (isRecent)
            {
                Destroy(userChoiceMaJiang[0].gameObject);
            }
            else
            {
                if (putOutArrayIndex >= userHandMaJiang[0].Count)
                {//报错，没这个牌
                    Debug.Log("Unity:"+"报错，没这么多牌");
                    return;
                }
                Destroy(userHandMaJiang[0][putOutArrayIndex].gameObject);
                userHandMaJiang[0].RemoveAt(putOutArrayIndex);

                for (int i = 0; i < playMethod.UserAllMaJiang.Count; i++)
                {
                    if (playMethod.UserAllMaJiang[i] > playMethod.UserCurrentTakeMaJiang)
                    {
                        if (putOutArrayIndex < i)
                        {
                            i--;
                        }
                        userHandMaJiang[0].Insert(i, userChoiceMaJiang[0]);
                        userChoiceMaJiang[0] = null;
                        break;
                    }
                }
                if (userChoiceMaJiang[0] != null)
                {
                    userHandMaJiang[0].Add(userChoiceMaJiang[0]);
                    userChoiceMaJiang[0] = null;
                }
                //暂时直接赋值，这里需要动画
                Vector3 pos;
                for (int i = 0; i < userHandMaJiang[0].Count; i++)
                {
                    pos = handPosition[0].position;
                    userHandMaJiang[0][i].transform.position = new Vector3(pos.x + i * width, pos.y, pos.z);
                    userHandMaJiang[0][i].ArrayIndex = i;
                }
            }
            playMethod.PutOutMaJiang(isRecent, putOutArrayIndex);
            playerHandMaJiang.PutOutOneHandMaJiang(isRecent ? -1 : putOutArrayIndex);
        }
        else
        {//别人插牌
            Destroy(userChoiceMaJiang[nIndex].gameObject);
        }
    }
    /// <summary>
    /// 服务端返回碰操作
    /// </summary>
    /// <param name="playerIndex">玩家索引</param>
    /// <param name="mj">碰的牌</param>
    public void PlayerDoPeng(int playerIndex, MAJIANG mj,int[]allMJ=null)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        int nIndex = playerIndex;//暂时玩家id就是索引
        int p = 0;
        bool isError = false;//true表示需要强刷手牌
        sending = false;
        if (nIndex == 0)
        {//是自己的碰
            if (!playMethod.isCanPeng(GetIntByMaJiang(mj)))
            {
                return;
            }
            //isError = true;
            p = playMethod.UserAllMaJiang.IndexOf(GetIntByMaJiang(mj));
        }
        else
        {
            p = Random.Range(0, userHandMaJiang[nIndex].Count - 1);
        }

        //删除牌池
        Destroy(userPutOutMaJiang[currentIndex][userPutOutMaJiang[currentIndex].Count - 1].gameObject);
        userPutOutMaJiang[currentIndex].RemoveAt(userPutOutMaJiang[currentIndex].Count - 1);
        if (pose)
        {
            pose.gameObject.SetActive(false);
        }
        if (!isError)
        {
            //删除手牌
            Destroy(userHandMaJiang[nIndex][p].gameObject);
            Destroy(userHandMaJiang[nIndex][p + 1].gameObject);
            userHandMaJiang[nIndex].RemoveRange(p, 2);

            TakeLastMJ(nIndex);
        }

        //创建碰牌
        CreateAGroupPeng(nIndex, (currentIndex - nIndex + 3) % 4, mj);

        if (nIndex == 0)
        {
            if (!isError)
            {
                //如果是自己还得处理数据
                playMethod.SelfPeng(GetIntByMaJiang(mj), (3 + currentIndex) % 4);
                playerHandMaJiang.HandPeng(p);
            }
            else
            {
                for (int i = allMJ.Length-1; i >=0; i--)
                {
                    if (allMJ[i] != 0)
                    {
                        allMJ[i] = 0;
                        break;
                    }
                }
                ForceRefreshSelfMaJiangDuringPlaying(allMJ, Mathf.Max(allMJ));
            }
        }
        else
        {
            playMethod.AllPengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { GetIntByMaJiang(mj), GetIntByMaJiang(mj), GetIntByMaJiang(mj) }), 0, (currentIndex - nIndex + 3) % 4));
        }

        SetUserCurrentDirection(nIndex);

        //播放声音
        PlayerBase tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerIDSequence[(playerIndex - GetIndexByDirection(DIRECTION.EAST) + 4) % 4]);
		//Debug.Log("Unity:" + tempplay.nUserId.ToString() + "/" + tempplay.nSex.ToString());
		if (tempplay.nSex == 1)
        {//男
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg10_M+Random.Range(0,2));
        }
        else
        {//女
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg10_W + Random.Range(0, 2));
        }
    }

    /// <summary>
    /// 服务端返回调用直杠
    /// </summary>
    /// <param name="playerIndex">玩家索引</param>
    /// <param name="mj">杠的牌</param>
    /// <param name="desti">杠后接的牌</param>
    public void PlayerDoZhiGang(int playerIndex, MAJIANG mj, MAJIANG desti)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (RemainBlockMJ <= 0)
        {
            Debug.Log("Unity:"+"没有牌可以接了，所以杠不了");
            return;
        }
        isLastOperateGang = true;//标记杠操作
        bool isError = false;//true表示需要强刷手牌
        MaJiang obj;
        Vector3 pos;
        Quaternion roto;
        int nIndex = playerIndex;//暂时玩家id就是索引
        int p = 0;
        sending = false;
        if (nIndex == 0)
        {//是自己的
            if (!playMethod.isCanZhiGang(GetIntByMaJiang(mj)))
            {
                return;
            }
            p = playMethod.UserAllMaJiang.IndexOf(GetIntByMaJiang(mj));
        }
        else
        {
            p = Random.Range(0, userHandMaJiang[nIndex].Count - 2);
        }

        //删除牌池
        Destroy(userPutOutMaJiang[currentIndex][userPutOutMaJiang[currentIndex].Count - 1].gameObject);
        userPutOutMaJiang[currentIndex].RemoveAt(userPutOutMaJiang[currentIndex].Count - 1);
        if (pose)
        {
            pose.gameObject.SetActive(false);
        }
        if (!isError)
        {
            //删除手牌
            Destroy(userHandMaJiang[nIndex][p].gameObject);
            Destroy(userHandMaJiang[nIndex][p + 1].gameObject);
            Destroy(userHandMaJiang[nIndex][p + 2].gameObject);
            userHandMaJiang[nIndex].RemoveRange(p, 3);
        }

        //创建牌
        CreateAGroupZhiGang(nIndex, (currentIndex - nIndex + 4) % 4 - 1, mj);

        if (desti > 0)
        {
            //删除牌墙的杠
            Destroy(GetBlockMaJiang(NextGang()).gameObject);//杠一张牌销毁牌墙的一张
            RemainBlockMJ--;
            roto = handPosition[nIndex].rotation;
            obj = InitOneMJModel(desti, MJKIND.STAND, new Vector3(0, 0, 0), roto);
            userChoiceMaJiang[nIndex] = obj;
        }

        RefreshHandMaJiang(nIndex);
        if (nIndex == 0)
        {
            if (!isError)
            {
                //如果是自己还得处理数据
                playMethod.SelfZhiGang(GetIntByMaJiang(mj), GetIntByMaJiang(desti), (4 - currentIndex) % 4 - 1);
                playerHandMaJiang.HandZhiGang(p, desti);
            }
        }
        else
        {
            playMethod.AllPengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { GetIntByMaJiang(mj), GetIntByMaJiang(mj), GetIntByMaJiang(mj), GetIntByMaJiang(mj) }), 1, (currentIndex - nIndex + 3) % 4));
        }
        SetUserCurrentDirection(nIndex);

        //播放声音
        PlayerBase tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerIDSequence[(playerIndex - GetIndexByDirection(DIRECTION.EAST) + 4) % 4]);
		Debug.Log("Unity:" + tempplay.nUserId.ToString() + "/" + tempplay.nSex.ToString());
		if (tempplay.nSex == 1)
        {//男
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg05_M);
        }
        else
        {//女
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg05_W);
        }
    }
    /// <summary>
    /// 暗杠
    /// </summary>
    /// <param name="playerIndex"></param>
    /// <param name="mj"></param>
    /// <param name="desti"></param>
    public void PlayerDoAnGang(int playerIndex, MAJIANG mj, MAJIANG desti)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (RemainBlockMJ <= 0)
        {
            Debug.Log("Unity:"+"没有牌可以接了，所以杠不了");
            return;
        }
        isLastOperateGang = true;//标记杠操作
        MaJiang obj;
        Vector3 pos;
        Quaternion roto;
        int nIndex = playerIndex;//暂时玩家id就是索引
        int nAnGangHandNum = 0;
        int p = 0;
        sending = false;
        if (nIndex == 0)
        {//是自己的
            nAnGangHandNum = playMethod.isCanAnGang(GetIntByMaJiang(mj));
            if (nAnGangHandNum <= 0)
            {
                return;
            }
            p = playMethod.UserAllMaJiang.IndexOf(GetIntByMaJiang(mj));
        }
        else
        {
            p = Random.Range(0, userHandMaJiang[nIndex].Count - 4);
        }

        //创建碰牌
        CreateAGroupAnGang(nIndex, mj);

        if (nIndex == 0)
        {
            //删除手牌
            for (int i = 0; i < nAnGangHandNum; i++)
            {
                Destroy(userHandMaJiang[nIndex][p + i].gameObject);
            }
            userHandMaJiang[nIndex].RemoveRange(p, nAnGangHandNum);
            if (nAnGangHandNum < 4)
            {//刚接的牌杠
                Destroy(userChoiceMaJiang[nIndex].gameObject);
            }
            else
            {
                if (userChoiceMaJiang[nIndex].WhoAmI > userHandMaJiang[nIndex][userHandMaJiang[nIndex].Count - 1].WhoAmI)
                {
                    userHandMaJiang[nIndex].Add(userChoiceMaJiang[nIndex]);
                    userChoiceMaJiang[nIndex] = null;
                }
                else
                {
                    for (int i = 0; i < userHandMaJiang[nIndex].Count; i++)
                    {
                        if (userHandMaJiang[nIndex][i].WhoAmI >= userChoiceMaJiang[nIndex].WhoAmI)
                        {
                            userHandMaJiang[nIndex].Insert(i, userChoiceMaJiang[nIndex]);
                            userChoiceMaJiang[nIndex] = null;
                            break;
                        }
                    }
                }
            }
            //如果是自己还得处理数据
            playMethod.SelfAnGang(GetIntByMaJiang(mj), GetIntByMaJiang(desti));
            playerHandMaJiang.HandAnGang(p, desti, nAnGangHandNum < 4 ? true : false);
        }
        else
        {
            //删除手牌
            Destroy(userHandMaJiang[nIndex][p].gameObject);
            Destroy(userHandMaJiang[nIndex][p + 1].gameObject);
            Destroy(userHandMaJiang[nIndex][p + 2].gameObject);
            Destroy(userChoiceMaJiang[nIndex].gameObject);
            userHandMaJiang[nIndex].RemoveRange(p, 3);

            playMethod.AllPengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { GetIntByMaJiang(mj), GetIntByMaJiang(mj), GetIntByMaJiang(mj), GetIntByMaJiang(mj) }), 3, -1));
        }
        if (desti > 0)
        {
            //删除牌墙的杠
            Destroy(GetBlockMaJiang(NextGang()).gameObject);//杠一张牌销毁牌墙的一张
            RemainBlockMJ--;
            roto = handPosition[nIndex].rotation;
            obj = InitOneMJModel(desti, MJKIND.STAND, new Vector3(0, 0, 0), roto);
            userChoiceMaJiang[nIndex] = obj;
        }
        RefreshHandMaJiang(nIndex);
        //播放声音
        PlayerBase tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerIDSequence[(playerIndex - GetIndexByDirection(DIRECTION.EAST) + 4) % 4]);
		Debug.Log("Unity:" + tempplay.nUserId.ToString() + "/" + tempplay.nSex.ToString());
		if (tempplay.nSex == 1)
        {//男
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg06_M);
        }
        else
        {//女
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg06_W);
        }
    }
    public void PlayerDoXuGang(int playerIndex, MAJIANG mj, MAJIANG desti)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (RemainBlockMJ <= 0)
        {
            Debug.Log("Unity:"+"没有牌可以接了，所以杠不了");
            return;
        }
        isLastOperateGang = true;//标记杠操作
        MaJiang obj;
        Vector3 pos;
        Quaternion roto;
        int gangIndex = -1;
        int nIndex = playerIndex;//暂时玩家id就是索引
        int p = 0;
        sending = false;
        if (nIndex == 0)
        {//是自己的
            gangIndex = playMethod.isCanXuGang(GetIntByMaJiang(mj));
            if (gangIndex < 0)
            {
                return;
            }
        }
        else
        {
            bool canGang = false;
            for (int index = 0; index < playMethod.AllPengGangChi[nIndex].Count; index++)
            {
                PengGangChi peng = playMethod.AllPengGangChi[nIndex][index];
                int kind = peng.checkGang(GetIntByMaJiang(mj));
                if (kind != -1)
                {
                    canGang = true;
                    break;
                }
            }
            if (!canGang)
            {
                return;
            }
        }
        //删除手牌
        if (gangIndex < 0 || gangIndex > userHandMaJiang[nIndex].Count)
        {//别人或自己接的牌
            Destroy(userChoiceMaJiang[nIndex].gameObject);
            userChoiceMaJiang[nIndex] = null;
        }
        else
        {
            Destroy(userHandMaJiang[nIndex][gangIndex].gameObject);
            userHandMaJiang[nIndex].RemoveAt(gangIndex);
            for (int i = 0; i < userHandMaJiang[nIndex].Count; i++)
            {
                if (userHandMaJiang[nIndex][i].WhoAmI >= userChoiceMaJiang[0].WhoAmI)
                {
                    userHandMaJiang[0].Insert(i, userChoiceMaJiang[0]);
                    userChoiceMaJiang[0] = null;
                    break;
                }
            }
            if (userChoiceMaJiang[0] != null)
            {
                userHandMaJiang[0].Add(userChoiceMaJiang[0]);
                userChoiceMaJiang[0] = null;
            }
            //暂时直接赋值，这里需要动画
            for (int i = 0; i < userHandMaJiang[0].Count; i++)
            {
                pos = handPosition[0].position;
                userHandMaJiang[0][i].transform.position = new Vector3(pos.x + i * width, pos.y, pos.z);
                userHandMaJiang[0][i].ArrayIndex = i;
            }
        }

        //创建牌
        CreateAXuGang(nIndex, mj);

        if (desti > 0)
        {//不需要等待
            //删除牌墙的杠
            Destroy(GetBlockMaJiang(NextGang()).gameObject);//杠一张牌销毁牌墙的一张
            RemainBlockMJ--;

            roto = handPosition[nIndex].rotation;
            obj = InitOneMJModel(desti, MJKIND.STAND, new Vector3(0, 0, 0), roto);
            userChoiceMaJiang[nIndex] = obj;
        }
        RefreshHandMaJiang(nIndex);

        if (nIndex == 0)
        {
            playMethod.SelfXuGang(GetIntByMaJiang(mj), GetIntByMaJiang(desti));
            playerHandMaJiang.HandXuGang(gangIndex, desti, gangIndex == 200 ? true : false);
        }
        else
        {
            playMethod.OtherXuGang(nIndex, GetIntByMaJiang(mj));
        }

        //播放声音
        PlayerBase tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerIDSequence[(playerIndex - GetIndexByDirection(DIRECTION.EAST) + 4) % 4]);
		Debug.Log("Unity:" + tempplay.nUserId.ToString() + "/" + tempplay.nSex.ToString());
		if (tempplay.nSex == 1)
        {//男
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg04_M);
        }
        else
        {//女
            MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg04_W);
        }
    }
    /// <summary>
    /// 杠之后并等待能胡的玩家不胡后杠接牌
    /// </summary>
    /// <param name="playerIndex">玩家索引</param>
    /// <param name="desti">接的牌</param>
    public void PlayerDoXuGangAfter(int playerIndex, MAJIANG desti)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (RemainBlockMJ <= 0)
        {
            Debug.Log("Unity:"+"没有牌可以接了，所以杠不了");
            return;
        }
        int nIndex = playerIndex;
        MaJiang obj;
        Quaternion roto;
        Destroy(GetBlockMaJiang(NextGang()).gameObject);//杠一张牌销毁牌墙的一张
        RemainBlockMJ--;

        roto = handPosition[nIndex].rotation;
        obj = InitOneMJModel(desti, MJKIND.STAND, new Vector3(0, 0, 0), roto);
        userChoiceMaJiang[nIndex] = obj;
    }
    /// <summary>
    /// 创建一组碰牌并加入碰杠数组<see cref="userSideMaJiang"/>
    /// </summary>
    /// <param name="nIndex">索引</param>
    /// <param name="kind">碰的第几个玩家索引</param>
    /// <param name="mj">碰的啥</param>
    private void CreateAGroupPeng(int nIndex, int kind, MAJIANG mj)
    {
        MaJiang obj;
        List<MaJiang> listObj = new List<MaJiang>();//碰列表
        float add = 0f, temp = 0f;
        Vector3 pos;
        Quaternion roto;
        float ax = 0, ay = 0, az = 0;
        pos = sidePosition[nIndex].transform.position;
        roto = sidePosition[nIndex].transform.rotation;

        for (int i = 0; i < playMethod.AllPengGangChi[nIndex].Count; i++)
        {
            if (playMethod.AllPengGangChi[nIndex][i].kind == 0 || playMethod.AllPengGangChi[nIndex][i].kind == 2)
            {
                add += width * 2 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 1)
            {
                add += width * 3 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 3)
            {
                add += width * 4;
            }
        }
        add += pengGangWidth * playMethod.AllPengGangChi[nIndex].Count;
        for (int i = 0; i < 3; i++)
        {
            if (kind < i)
            {
                temp = Mathf.Abs(height - width);
            }
            else if (kind == i)
            {
                temp = Mathf.Abs(height - width) / 2;
            }
            else {
                temp = 0;
            }
            if (nIndex == 0)
            {
                ax = -width * i - temp - add;
                az = kind == i ? -Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 1)
            {
                az = -width * i - temp - add;
                ax = kind == i ? Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 2)
            {
                ax = width * i + temp + add;
                az = kind == i ? Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 3)
            {
                az = width * i + temp + add;
                ax = kind == i ? -Mathf.Abs(height - width) / 2 : 0;
            }
            //roto.eulerAngles = new Vector3(0,kind == i?-90:0, 0);
            Vector3 posTemp = new Vector3(pos.x + ax, pos.y, pos.z + az);
            Quaternion rotoTemp = roto;
            rotoTemp.eulerAngles = new Vector3(roto.eulerAngles.x, roto.eulerAngles.y + (kind == i ? 90 : 0), roto.eulerAngles.z);
            obj = InitOneMJModel(mj, MJKIND.LAY, posTemp, rotoTemp);
            obj.transform.SetParent(sidePosition[nIndex].transform.parent);
            listObj.Add(obj);
        }
        userSideMaJiang[nIndex].Add(listObj);
    }
    /// <summary>
    /// 创建一组直杠牌并加入数组<see cref="userSideMaJiang"/>
    /// <seealso cref="CreateAGroupPeng">几乎一样</seealso>
    /// </summary>
    /// <param name="nIndex">索引</param>
    /// <param name="kind">杠的第几个玩家索引</param>
    /// <param name="mj">杠的啥</param>
    private void CreateAGroupZhiGang(int nIndex, int kind, MAJIANG mj)
    {
        MaJiang obj;
        List<MaJiang> listObj = new List<MaJiang>();//碰列表
        float add = 0f, temp = 0f;
        Vector3 pos;
        Quaternion roto;
        float ax = 0, ay = 0, az = 0;
        pos = sidePosition[nIndex].transform.position;
        roto = sidePosition[nIndex].transform.rotation;

        for (int i = 0; i < playMethod.AllPengGangChi[nIndex].Count; i++)
        {
            if (playMethod.AllPengGangChi[nIndex][i].kind == 0 || playMethod.AllPengGangChi[nIndex][i].kind == 2)
            {
                add += width * 2 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 1)
            {
                add += width * 3 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 3)
            {
                add += width * 4;
            }
        }
        add += pengGangWidth * playMethod.AllPengGangChi[nIndex].Count;
        kind = kind == 2 ? 3 : kind;//倒数第二张牌忽略
        for (int i = 0; i < 4; i++)
        {
            if (kind < i)
            {
                temp = Mathf.Abs(height - width);
            }
            else if (kind == i)
            {
                temp = Mathf.Abs(height - width) / 2;
            }
            else {
                temp = 0;
            }
            if (nIndex == 0)
            {
                ax = -width * i - temp - add;
                az = kind == i ? -Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 1)
            {
                az = -width * i - temp - add;
                ax = kind == i ? Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 2)
            {
                ax = width * i + temp + add;
                az = kind == i ? Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 3)
            {
                az = width * i + temp + add;
                ax = kind == i ? -Mathf.Abs(height - width) / 2 : 0;
            }
            //roto.eulerAngles = new Vector3(0,kind == i?-90:0, 0);
            Vector3 posTemp = new Vector3(pos.x + ax, pos.y, pos.z + az);
            Quaternion rotoTemp = roto;
            rotoTemp.eulerAngles = new Vector3(roto.eulerAngles.x, roto.eulerAngles.y + (kind == i ? 90 : 0), roto.eulerAngles.z);
            obj = InitOneMJModel(mj, MJKIND.LAY, posTemp, rotoTemp);
            obj.transform.SetParent(sidePosition[nIndex].transform.parent);
            listObj.Add(obj);
        }
        userSideMaJiang[nIndex].Add(listObj);
    }
    /// <summary>
    /// 创建一张自己明杠的牌，仅显示
    /// </summary>
    /// <param name="nIndex">哪个人的索引</param>
    /// <param name="mj">杠的啥</param>
    private void CreateAXuGang(int nIndex, MAJIANG mj)
    {
        MaJiang obj;
        List<MaJiang> listObj;//碰列表
        float add = 0f, temp = 0f;
        int kind = -1;
        int index;
        Vector3 pos;
        Quaternion roto;
        float ax = 0, ay = 0, az = 0;
        pos = sidePosition[nIndex].transform.position;
        roto = sidePosition[nIndex].transform.rotation;

        for (index = 0; index < playMethod.AllPengGangChi[nIndex].Count; index++)
        {
            PengGangChi peng = playMethod.AllPengGangChi[nIndex][index];
            kind = peng.checkGang(GetIntByMaJiang(mj));
            if (kind != -1)
            {
                break;
            }
            if (playMethod.AllPengGangChi[nIndex][index].kind == 0 || playMethod.AllPengGangChi[nIndex][index].kind == 2)
            {
                add += width * 2 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][index].kind == 1)
            {
                add += width * 3 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][index].kind == 3)
            {
                add += width * 4;
            }
        }
        add += pengGangWidth * (index);
        if (kind == -1)
        {//不了
            return;
        }
        temp = Mathf.Abs(height - width) / 2;
        if (nIndex == 0)
        {
            ax = -width * (kind) - temp - add;
            az = -Mathf.Abs(height - width) / 2 + width;
        }
        else if (nIndex == 1)
        {
            az = -width * (kind) - temp - add;
            ax = Mathf.Abs(height - width) / 2 - width;
        }
        else if (nIndex == 2)
        {
            ax = width * (kind) + temp + add;
            az = Mathf.Abs(height - width) / 2 - width;
        }
        else if (nIndex == 3)
        {
            az = width * (kind) + temp + add;
            ax = -Mathf.Abs(height - width) / 2 + width;
        }
        Vector3 posTemp = new Vector3(pos.x + ax, pos.y, pos.z + az);
        Quaternion rotoTemp = roto;
        rotoTemp.eulerAngles = new Vector3(roto.eulerAngles.x, roto.eulerAngles.y + 90, roto.eulerAngles.z);
        obj = InitOneMJModel(mj, MJKIND.LAY, posTemp, rotoTemp);
        obj.transform.SetParent(sidePosition[nIndex].transform.parent);

        possibleHu = obj;//续杠的牌记录下，有可能放胡

        listObj = userSideMaJiang[nIndex][index];
        listObj.Add(obj);
        userSideMaJiang[nIndex][index] = listObj;
    }
    /// <summary>
    /// 直接创建一组续杠
    /// </summary>
    /// <param name="nIndex">索引</param>
    /// <param name="kind">杠的第几个玩家索引</param>
    /// <param name="mj">杠的啥</param>
    private void CreateAGroupXuGang(int nIndex, int kind, MAJIANG mj)
    {
        MaJiang obj;
        List<MaJiang> listObj = new List<MaJiang>();//碰列表
        float add = 0f, temp = 0f;
        Vector3 pos;
        Quaternion roto;
        float ax = 0, ay = 0, az = 0;
        pos = sidePosition[nIndex].transform.position;
        roto = sidePosition[nIndex].transform.rotation;

        for (int i = 0; i < playMethod.AllPengGangChi[nIndex].Count; i++)
        {
            if (playMethod.AllPengGangChi[nIndex][i].kind == 0 || playMethod.AllPengGangChi[nIndex][i].kind == 2)
            {
                add += width * 2 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 1)
            {
                add += width * 3 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 3)
            {
                add += width * 4;
            }
        }
        add += pengGangWidth * playMethod.AllPengGangChi[nIndex].Count;
        for (int i = 0; i < 3; i++)
        {
            if (kind < i)
            {
                temp = Mathf.Abs(height - width);
            }
            else if (kind == i)
            {
                temp = Mathf.Abs(height - width) / 2;
            }
            else {
                temp = 0;
            }
            if (nIndex == 0)
            {
                ax = -width * i - temp - add;
                az = kind == i ? -Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 1)
            {
                az = -width * i - temp - add;
                ax = kind == i ? Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 2)
            {
                ax = width * i + temp + add;
                az = kind == i ? Mathf.Abs(height - width) / 2 : 0;
            }
            else if (nIndex == 3)
            {
                az = width * i + temp + add;
                ax = kind == i ? -Mathf.Abs(height - width) / 2 : 0;
            }
            //roto.eulerAngles = new Vector3(0,kind == i?-90:0, 0);
            Vector3 posTemp = new Vector3(pos.x + ax, pos.y, pos.z + az);
            Quaternion rotoTemp = roto;
            rotoTemp.eulerAngles = new Vector3(roto.eulerAngles.x, roto.eulerAngles.y + (kind == i ? 90 : 0), roto.eulerAngles.z);
            obj = InitOneMJModel(mj, MJKIND.LAY, posTemp, rotoTemp);
            obj.transform.SetParent(sidePosition[nIndex].transform.parent);
            listObj.Add(obj);

            if (kind == i)
            {
                ax = 0; ay = 0; az = 0;
                if (nIndex == 0)
                {
                    az = width;
                }
                else if (nIndex == 1)
                {
                    ax = -width;
                }
                else if (nIndex == 2)
                {
                    az = -width;
                }
                else if (nIndex == 3)
                {
                    ax = width;
                }
                posTemp = new Vector3(posTemp.x + ax, posTemp.y, posTemp.z + az);

                obj = InitOneMJModel(mj, MJKIND.LAY, posTemp, rotoTemp);
                obj.transform.SetParent(sidePosition[nIndex].transform.parent);
                listObj.Add(obj);
            }
        }
        userSideMaJiang[nIndex].Add(listObj);
    }
    private void CreateAGroupAnGang(int nIndex, MAJIANG mj)
    {
        MaJiang obj;
        List<MaJiang> listObj = new List<MaJiang>();//碰列表
        float add = 0f, temp = 0f;
        Vector3 pos;
        Quaternion roto;
        float ax = 0, ay = 0, az = 0;
        pos = sidePosition[nIndex].transform.position;
        roto = sidePosition[nIndex].transform.rotation;

        for (int i = 0; i < playMethod.AllPengGangChi[nIndex].Count; i++)
        {
            if (playMethod.AllPengGangChi[nIndex][i].kind == 0 || playMethod.AllPengGangChi[nIndex][i].kind == 2)
            {
                add += width * 2 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 1)
            {
                add += width * 3 + height;
            }
            else if (playMethod.AllPengGangChi[nIndex][i].kind == 3)
            {
                add += width * 4;
            }
        }
        add += pengGangWidth * playMethod.AllPengGangChi[nIndex].Count;
        for (int i = 0; i < 4; i++)
        {
            if (nIndex == 0)
            {
                ax = -width * i - add;
            }
            else if (nIndex == 1)
            {
                az = -width * i - add;
            }
            else if (nIndex == 2)
            {
                ax = width * i + add;
            }
            else if (nIndex == 3)
            {
                az = width * i + add;
            }
            //roto.eulerAngles = new Vector3(0,kind == i?-90:0, 0);
            Vector3 posTemp = new Vector3(pos.x + ax, pos.y, pos.z + az);
            Quaternion rotoTemp = roto;
            MJKIND mjKind = MJKIND.LAY;
            if (i > 0)
            {
                mjKind = MJKIND.SBLANK;
                rotoTemp.eulerAngles = new Vector3(roto.eulerAngles.x + 180, roto.eulerAngles.y, roto.eulerAngles.z);
            }
            obj = InitOneMJModel(mj, mjKind, posTemp, rotoTemp);
            obj.transform.SetParent(sidePosition[nIndex].transform.parent);
            listObj.Add(obj);
        }
        userSideMaJiang[nIndex].Add(listObj);
    }
    /// <summary>
    /// 创建一张胡牌
    /// </summary>
    /// <param name="nIndex">玩家</param>
    /// <param name="mj">胡的牌</param>
    private void CreateAHu(int nIndex, int mj)
    {
        MaJiang obj;
        Vector3 pos;
        Quaternion roto;
        pos = huPosition[nIndex].transform.position;
        roto = huPosition[nIndex].transform.rotation;
        obj = InitOneMJModel(GetMaJiangByInt(mj), MJKIND.LAY, pos, roto);
        obj.transform.SetParent(huPosition[nIndex].transform.parent);

        InitTableEffect(2, new Vector3(pos.x, pos.y + thickness / 2, pos.z));

        userHuMaJiang[nIndex] = obj;
    }
    /// <summary>
    /// 玩家胡牌返回
    /// </summary>
    /// <param name="nIndex">胡牌的玩家</param>
    /// <param name="mj">胡的牌</param>
    public void PlayerDoHu(int nIndex, int mj, int[][] allUserMJ)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        Vector3 pos;
        if (nIndex >= 0)
        {
            if (possibleHu)
            {
                Destroy(possibleHu.gameObject);
            }
            CreateAHu(nIndex, mj);
            PlayerShowHu(nIndex, allUserMJ);
            //播放声音
            PlayerBase tempplay = MainRoot._gGameRoomCenter.gGameRoom.GetPlayerByUserId(playerIDSequence[(nIndex - GetIndexByDirection(DIRECTION.EAST) + 4) % 4]);
			Debug.Log("Unity:" + tempplay.nUserId.ToString() + "/" + tempplay.nSex.ToString());
			if (tempplay.nSex == 1)
            {//男
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg07_M + Random.Range(0, 2));
            }
            else
            {//女
                MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.CaoZuoMsg07_W + Random.Range(0, 2));
            }
        }
        else
        {//流局
            for (int i = 0; i < allUserMJ.Length; i++)
            {
                if (userChoiceMaJiang[i])
                {//如果有手牌就删掉
                    Destroy(userChoiceMaJiang[i]);
                }
            }
            OnGameOver();
        }
        //OnGameOver();//只胡不推牌，等服务端返回再推
    }
    /// <summary>
    /// 玩家推倒返回
    /// </summary>
    /// <param name="nIndex">胡牌的玩家</param>
    /// <param name="allUserMJ">玩家的牌型</param>
    public void PlayerShowHu(int nIndex, int[][] allUserMJ)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        int[] oneUserMJ;
        Vector3 pos;
        if (true)
        {//只推倒胡的玩家
            if (nIndex == 0)
            {//自己直接推倒
                for (int j = 0; j < userHandMaJiang[0].Count; j++)
                {
                    userHandMaJiang[0][j].transform.Rotate(new Vector3(90, 0, 0));
                    pos = userHandMaJiang[0][j].transform.position;
                    pos = new Vector3(pos.x, pos.y - (height - thickness) / 2, pos.z);
                    userHandMaJiang[0][j].transform.position = pos;
                }
            }
            else
            {
                oneUserMJ = allUserMJ[nIndex];
                for (int j = 0; j < oneUserMJ.Length; j++)
                {
                    if (userHandMaJiang[nIndex].Count <= j)
                    {
                        break;
                    }
                    ChangeMaJiangFace(userHandMaJiang[nIndex][j], GetMaJiangByInt(oneUserMJ[j]));
                    userHandMaJiang[nIndex][j].transform.Rotate(new Vector3(90, 0, 0));
                    pos = userHandMaJiang[nIndex][j].transform.position;
                    pos = new Vector3(pos.x, pos.y - (height - thickness) / 2, pos.z);
                    userHandMaJiang[nIndex][j].transform.position = pos;
                }
                if (userChoiceMaJiang[nIndex])
                {//如果有手牌就删掉
                    Destroy(userChoiceMaJiang[nIndex]);
                }
            }
        }
        else
        {//全部推倒胡
            for (int j = 0; j < userHandMaJiang[0].Count; j++)
            {
                userHandMaJiang[0][j].transform.Rotate(new Vector3(90, 0, 0));
                pos = userHandMaJiang[0][j].transform.position;
                pos = new Vector3(pos.x, pos.y - (height - thickness) / 2, pos.z);
                userHandMaJiang[0][j].transform.position = pos;
            }
            if (userChoiceMaJiang[0])
            {//如果有手牌就删掉
                Destroy(userChoiceMaJiang[0]);
            }
            for (int i = 1; i < allUserMJ.Length; i++)
            {
                oneUserMJ = allUserMJ[i];
                for (int j = 0; j < userHandMaJiang[i].Count; j++)
                {
                    ChangeMaJiangFace(userHandMaJiang[i][j], GetMaJiangByInt(oneUserMJ[j]));
                    userHandMaJiang[i][j].transform.Rotate(new Vector3(90, 0, 0));
                    pos = userHandMaJiang[i][j].transform.position;
                    pos = new Vector3(pos.x, pos.y - (height - thickness) / 2, pos.z);
                    userHandMaJiang[i][j].transform.position = pos;
                }
                if (userChoiceMaJiang[i])
                {//如果有手牌就删掉
                    Destroy(userChoiceMaJiang[i]);
                }
            }
        }

        OnGameOver();
    }
    /// <summary>
    /// 整型转牌型
    /// </summary>
    /// <param name="n"></param>
    /// <returns></returns>
    public MAJIANG GetMaJiangByInt(int n)
    {
        if (n <= 0)
        {
            return MAJIANG.NONE;
        }
        return (MAJIANG)n;
    }
    /// <summary>
    /// 牌型转整型
    /// </summary>
    /// <param name="mj"></param>
    /// <returns></returns>
    public int GetIntByMaJiang(MAJIANG mj)
    {
        return (int)mj;
    }
    /// <summary>
    /// 生成一个麻将牌
    /// </summary>
    /// <param name="mj">具体的牌，比如九万</param>
    /// <param name="kind">牌的姿势</param>
    /// <returns>牌对象</returns>
    private MaJiang InitOneMJModel(MAJIANG mj, MJKIND kind, Vector3 pos, Quaternion roto)
    {
        GameObject obj = null;
        MaJiang scriptMJ = null;
        switch (kind)
        {
            case MJKIND.BLANK:
                if (mJPrefab[(int)kind] != null)
                {
                    obj = Instantiate(mJPrefab[(int)kind].gameObject);
                }
                else {
                    obj = (GameObject)Instantiate(Resources.Load("Prefabs/MaJingNone"), transform, false);
                }
                scriptMJ = obj.GetComponent<MaJiang>();
                break;
            case MJKIND.SBLANK:
                if (mJPrefab[(int)kind] != null)
                {
                    obj = Instantiate(mJPrefab[(int)kind].gameObject);
                }
                else {
                    obj = (GameObject)Instantiate(Resources.Load("Prefabs/MaJingNoneShadow"), transform, false);
                }
                scriptMJ = obj.GetComponent<MaJiang>();
                break;
            case MJKIND.LAY:
                if (mJPrefab[(int)kind] != null)
                {
                    obj = Instantiate(mJPrefab[(int)kind].gameObject);
                }
                else {
                    obj = (GameObject)Instantiate(Resources.Load("Prefabs/MaJingLay"), transform, false);
                }
                scriptMJ = obj.GetComponent<MaJiang>();
                scriptMJ.SetStates(mj, kind);

                scriptMJ.ChangeMJMaterial(GetTypicalMaJiangMaterial(mj));
                break;
            case MJKIND.STAND:
                if (mJPrefab[(int)kind] != null)
                {
                    obj = Instantiate(mJPrefab[(int)kind].gameObject);
                }
                else {
                    obj = (GameObject)Instantiate(Resources.Load("Prefabs/MaJingStand"), transform, false);
                }
                scriptMJ = obj.GetComponent<MaJiang>();
                scriptMJ.SetStates(mj, kind);

                scriptMJ.ChangeMJMaterial(GetTypicalMaJiangMaterial(mj));
                break;
        }
        if (obj != null)
        {
            obj.transform.position = pos;
            obj.transform.rotation = roto;
        }
        return scriptMJ;
    }
    /// <summary>
    /// 创建特效
    /// </summary>
    /// <param name="nKind"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    private Transform InitTableEffect(int nKind, Vector3 pos)
    {
        GameObject obj = null;
        switch (nKind)
        {
            case 1://定位器
                obj = (GameObject)Instantiate(Resources.Load("Prefabs/MaJiangPointEff"), transform, false);
                break;
            case 2://胡
                obj = (GameObject)Instantiate(Resources.Load("Prefabs/MaJiangHuEff"), transform, false);
                break;
            default:
                break;
        }
        if (obj)
        {
            obj.transform.position = pos;
            return obj.transform;
        }
        return null;
    }
    /// <summary>
    /// 获取牌面对应材质的索引，用以显示牌型
    /// </summary>
    /// <param name="mj"></param>
    /// <returns></returns>
    private int GetMaJiangMaterialIndex(MAJIANG mj)
    {
        if (mj <= 0)
        {
            mj = MAJIANG.BAI;
        }
        int n = (int)mj;
        return n / 16 * 9 + n % 16 - 1;
    }
    /// <summary>
    /// 更换麻将牌面
    /// </summary>
    /// <param name="mjObj">麻将物体</param>
    /// <param name="mj">牌面</param>
    private void ChangeMaJiangFace(MaJiang mjObj, MAJIANG mj)
    {
        mjObj.ChangeMJMaterial(GetTypicalMaJiangMaterial(mj));
        mjObj.SetStates(mj, mjObj.Kind);
    }
    /// <summary>
    /// 获取麻将材质
    /// </summary>
    /// <param name="mj">牌面</param>
    /// <returns></returns>
    public Material GetTypicalMaJiangMaterial(MAJIANG mj)
    {
        Material temp = null;
        try
        {
            temp = materialMJ[GetMaJiangMaterialIndex(mj)];
        }
        catch (System.Exception)
        {
            Debug.LogError("Unity:"+"GetTypicalMaJiangMaterial Error:" + mj.ToString());
        }
        return temp;
    }
    /// <summary>
    /// 获取点数集抓牌位置
    /// </summary>
    /// <param name="bankerIndex"></param>
    /// <param name="lSiceCount"></param>
    /// <returns></returns>
    public int[] GetFourDiceAndTakePosition(int bankerIndex, int lSiceCount)
    {
        int[] info = { 0, 0, 0, 0, 0, 0 };
        for (int i = 0; i < 4; i++)
        {//获取4骰子点数
            info[i] = (lSiceCount >> (4 * i) & 0xf) + 1;
        }
        info[4] = ((info[0] + info[1] + 3) % 4 + bankerIndex) % 4;//下一个摇骰子人的索引
        info[5] = info[2] + info[3];
        return info;
    }
    /// <summary>
    /// 获取指定麻将理论剩余数
    /// </summary>
    /// <param name="mj"></param>
    /// <returns></returns>
    public int GetRemainHuMaJiangCount(MAJIANG mj)
    {
        int used=0;
        try
        {
            for (int i = 0; i < userHandMaJiang[0].Count; i++)
            {//检测自己手牌
                if (userHandMaJiang[0][i].WhoAmI.Equals(mj))
                {
                    used++;
                }
            }
            if (userChoiceMaJiang[0] && userChoiceMaJiang[0].WhoAmI.Equals(mj))
            {//接的牌
                used++;
            }
            for (int i = 0; i < userPutOutMaJiang.Count; i++)
            {//检测打出去的牌
                for (int j = 0; j < userPutOutMaJiang[i].Count; j++)
                {
                    if (userPutOutMaJiang[i][j].WhoAmI.Equals(mj))
                    {
                        used++;
                    }
                }
            }
            for (int i = 0; i < userSideMaJiang.Count; i++)
            {//检测碰杠吃
                for (int j = 0; j < userSideMaJiang[i].Count; j++)
                {
                    if (userSideMaJiang[i][j][0].WhoAmI.Equals(mj))
                    {
                        used += userSideMaJiang[i][j].Count;
                    }
                }
            }
        }
        catch (System.Exception)
        {
            return 0;
        }

        return 4 - used;
    }
    /// <summary>
    /// 刷新倒计时
    /// </summary>
    private void CountDown()
    {
        if (cover)
        {//盖子
            float count = 0f;
            int time =  0;
            if (!cover.gameObject.activeSelf)
            {
                cover.gameObject.SetActive(true);
            }
            count = Time.time - lastCountdownTime;
            if (count >= 60 && !IsChaoShiChuPai) {
                IsChaoShiChuPai = true;
                MainRoot._gUIModule.pUnModalUIControl.pGameUIView.SpawnSMDengDaiChuPai();
            }

            count = count > 15 ? 15 : count;
            time = Mathf.CeilToInt(15 - count);
            if (time == 15) {
                IsChaoShiChuPai = false;
            }
            if (CountDownSprite != null) {
                if (!CountDownSprite.sprite.Equals(CountDownSpArray[time]))
                {//播放警告声音
                    CountDownSprite.sprite = CountDownSpArray[time]; //倒计时.
                    if (time <= 5 && gameState == GAMESTATE.Start)
                    {
                        MainRoot._gGameAudioManage.PlayGameAudio(GameAudioManage.GmAudioEnum.YouXiZhong19);
                    }
                }
            }
        }
    }
    /// <summary>
    /// 一局结束
    /// </summary>
    void OnGameOver()
    {
        gameState = GAMESTATE.Over;
        playerHandMaJiang.ClearAllHandMaJiang();
        handParent[0].gameObject.SetActive(true);
        if (pose)
        {//隐藏出牌定位器
            pose.gameObject.SetActive(false);
        }
        //ClearGame();
        MainRoot._gUIModule.pUnModalUIControl.pGameUIView.ShowOneSysMsgText(4, MainRoot._gUIModule.pUnModalUIControl.pGameUIView);//胡牌，牌局结束
    }
    /// <summary>
    /// 清除桌面元素
    /// </summary>
    public void ClearGame()
    {
        List<MaJiang> temp;
        //玩家手牌
        for (int i = 0; i < userHandMaJiang.Count; i++)
        {
            temp = userHandMaJiang[i];
            for (int j = 0; j < temp.Count; j++)
            {
                if (temp[j])
                {
                    Destroy(temp[j].gameObject);
                }
            }
            userHandMaJiang[i].Clear();
        }
        //玩家打出的牌
        for (int i = 0; i < userPutOutMaJiang.Count; i++)
        {
            temp = userPutOutMaJiang[i];
            for (int j = 0; j < temp.Count; j++)
            {
                if (temp[j])
                {
                    Destroy(temp[j].gameObject);
                }
            }
            userPutOutMaJiang[i].Clear();
        }
        //玩家面前的牌墙
        for (int i = 0; i < userBlockMaJiang.Count; i++)
        {
            temp = userBlockMaJiang[i];
            for (int j = 0; j < temp.Count; j++)
            {
                if (temp[j])
                {
                    Destroy(temp[j].gameObject);
                }
            }
            userBlockMaJiang[i].Clear();
        }
        //玩家碰杠吃的牌
        for (int i = 0; i < userSideMaJiang.Count; i++)
        {

            for (int j = 0; j < userSideMaJiang[i].Count; j++)
            {
                temp = userSideMaJiang[i][j];
                for (int k = 0; k < temp.Count; k++)
                {
                    if (temp[k])
                    {
                        Destroy(temp[k].gameObject);
                    }
                }
            }
            userSideMaJiang[i].Clear();
        }
        //玩家接的牌
        for (int i = 0; i < userChoiceMaJiang.Length; i++)
        {
            if (userChoiceMaJiang[i])
            {
                Destroy(userChoiceMaJiang[i].gameObject);
            }
        }
        //玩家胡牌
        for (int i = 0; i < userHuMaJiang.Length; i++)
        {
            if (userHuMaJiang[i])
            {
                Destroy(userHuMaJiang[i].gameObject);
            }
        }
        for (int i = 0; i < directionMeshPlayer.Length; i++)
        {
            directionMeshModel[i].sharedMaterial.color = new Color(1, 1, 1);
            directionMeshPlayer[i].Stop();
        }
        currentIndex = -1;
        if (cover.gameObject.activeSelf)
        {
            cover.gameObject.SetActive(false);
        }
        userChoiceMaJiang = new MaJiang[4];//玩家接的牌
        userHuMaJiang = new MaJiang[4];//玩家胡牌
        playMethod.Clear();
        sending = false;
        gameState = GAMESTATE.Free;
    }
    #region 玩家的可能操作放在这里
    /// <summary>
    /// 玩家自己请求打牌
    /// </summary>
    /// <param name="isRecent">是否打的刚接的牌</param>
    /// <param name="nIndex">打的手牌索引</param>
    public void UserPutOutMJ(bool isRecent, int nIndex)
    {
        MAJIANG mj;
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (sending)
        {
            Debug.Log("Unity:"+"不能无限发送消息");
            return;
        }
        if (currentIndex != 0)
        {
            Debug.Log("Unity:"+"不该自己打牌就不能瞎打！");
            return;
        }
        sending = true;
        this.isRecent = isRecent;
        if (!isRecent)
        {
            mj = userHandMaJiang[0][nIndex].WhoAmI;
            putOutArrayIndex = nIndex;
        }
        else
        {
            mj = userChoiceMaJiang[0].WhoAmI;
        }

        //测试 直接打出成功
        //PutOutMJ(0, mj);
        if (RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNet.RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallUserOutCard((byte)mj);
        }
    }
    /// <summary>
    /// 请求续杠
    /// </summary>
    public void UserDoXuGang(MAJIANG mj)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (sending)
        {
            Debug.Log("Unity:"+"不能无限发送消息");
            return;
        }
        int gangIndex = -1;
        if (currentIndex != 0)
        {
            Debug.Log("Unity:"+"不该自己打牌就不能续杠！");
            return;
        }
        gangIndex = playMethod.isCanXuGang(GetIntByMaJiang(mj));
        if (gangIndex < 0)
        {
            return;
        }
        sending = true;

        //测试 直接成功 规则不严谨 要改
        PlayerDoXuGang(0, mj, MAJIANG.MJ09);
    }
    /// <summary>
    /// 请求暗杠
    /// </summary>
    public void UserDoAnGang(MAJIANG mj)
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (sending)
        {
            Debug.Log("Unity:"+"不能无限发送消息");
            return;
        }
        int gangIndex = -1;
        if (currentIndex != 0)
        {
            Debug.Log("Unity:"+"不该自己打牌就不能续杠！");
            return;
        }
        gangIndex = playMethod.isCanAnGang(GetIntByMaJiang(mj));
        if (gangIndex == 0)
        {
            return;
        }
        sending = true;

        //测试 直接成功 规则不严谨 要改
        PlayerDoAnGang(0, mj, MAJIANG.MJ04);

    }
    /// <summary>
    /// 请求碰
    /// </summary>
    public void UserDoPeng()
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (sending)
        {
            Debug.Log("Unity:"+"不能无限发送消息");
            return;
        }
        MaJiang obj;
        if (currentIndex == 0)
        {
            Debug.Log("Unity:"+"自己打的牌不能碰的！");
            return;
        }
        obj = userPutOutMaJiang[currentIndex][userPutOutMaJiang[currentIndex].Count - 1];
        if (!playMethod.isCanPeng(GetIntByMaJiang(obj.WhoAmI)))
        {
            return;
        }
        sending = true;

        //测试 直接成功
        PlayerDoPeng(0, obj.WhoAmI);
    }
    /// <summary>
    /// 请求直杠
    /// </summary>
    public void UserDoZhiGang()
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (sending)
        {
            Debug.Log("Unity:"+"不能无限发送消息");
            return;
        }
        MaJiang obj;
        if (currentIndex == 0)
        {
            Debug.Log("Unity:"+"不能刚自己打的牌！");
            return;
        }
        obj = userPutOutMaJiang[currentIndex][userPutOutMaJiang[currentIndex].Count - 1];
        if (!playMethod.isCanZhiGang(GetIntByMaJiang(obj.WhoAmI)))
        {
            return;
        }
        sending = true;

        //测试 直接成功
        PlayerDoZhiGang(0, obj.WhoAmI, MAJIANG.MJ04);

    }
    /// <summary>
    /// 请求胡
    /// </summary>
    public void UserDoHu()
    {
        if (gameState != GAMESTATE.Start)
        {
            Debug.Log("Unity:"+"未开始");
            return;
        }
        if (sending)
        {
            Debug.Log("Unity:"+"不能无限发送消息");
            return;
        }
        MaJiang obj;
        sending = true;
        if (currentIndex == 0)
        {//自摸
            if (obj = userChoiceMaJiang[0])
            {
                //PlayerDoHu(0, GetIntByMaJiang(obj.WhoAmI), new int[][] { });
            }
        }
        else
        {
            obj = userPutOutMaJiang[currentIndex][userPutOutMaJiang[currentIndex].Count - 1];
            //PlayerDoHu(0, GetIntByMaJiang(obj.WhoAmI),new int[][] { });
        }
    }
    #endregion//玩家的可能操作放在这里

    #region 测试函数 
    //测试用，随机取个牌
    public MAJIANG GetRandomMaJiang()
    {
        MAJIANG[] mj = System.Enum.GetValues(typeof(MAJIANG)) as MAJIANG[];
        return mj[Random.Range(0, mj.Length)];
    }
    //测试用，取准备打的牌型
    public MAJIANG GetRecentPutOut()
    {
        return GetMaJiangByInt(playMethod.UserAllMaJiang[putOutArrayIndex]);
    }
    //场上最后打的一张牌
    public MAJIANG GetLastPutOut()
    {
        return possibleHu.WhoAmI;
    }
    //获取自己接的牌
    public MAJIANG GetSelfChoice()
    {
        return userChoiceMaJiang[0].WhoAmI;
    }
    /// <summary>
    /// 获取一组胡牌数据
    /// </summary>
    /// <returns></returns>
    public int[][] GetAGroupHu()
    {
        int[][] result = new int[4][];
        result[0] = new int[13] { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5 };
        result[1] = new int[13] { 1, 1, 1, 2, 2, 2, 3, 3, 3, 4, 4, 4, 5 };
        result[2] = new int[13] { 17, 17, 17, 18, 18, 18, 19, 19, 19, 20, 20, 20, 21 };
        result[3] = new int[13] { 33, 33, 33, 34, 34, 34, 35, 35, 35, 36, 36, 36, 37 };
        return result;
    }
    #endregion
}
