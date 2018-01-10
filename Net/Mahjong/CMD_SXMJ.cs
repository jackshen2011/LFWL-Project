using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mahjong
{
    public class CMD_SXMJ
    {
        //组件属性
        public const int GAME_PLAYER = 4;
        public const byte MASK_VALUE = 0x0F;								//数值掩码

        //常量定义
        public const int MAX_WEAVE = 4;                                 //最大组合
        public const int MAX_INDEX = 34; 									//最大索引
        public const int MAX_COUNT = 14; 								//最大数目
        public const int MAX_REPERTORY = 108;                           //最大库存  无风 无红中赖子
        public const int MAX_REPERTORY_HZ = 112;                        //最大库存  无风 有红中赖子
        public const int MAX_REPERTORY_FENG = 136;                      //最大库存  有风
        public const int MAX_SURPLUS = 55;                              //最大剩余

        //////////////////////////////////////////////////////////////////////////
        //动作定义

        //动作标志
        public const int WIK_NULL = 0x00;                               //没有类型
        public const int WIK_LEFT = 0x01;                               //左吃类型
        public const int WIK_CENTER = 0x02;                             //中吃类型
        public const int WIK_RIGHT = 0x04;                              //右吃类型
        public const int WIK_PENG = 0x08;                               //碰牌类型
        public const int WIK_GANG = 0x10;                               //杠牌类型
        public const Byte WIK_XIAO_HU = 0x20;//小胡								//吃牌类型
        public const int WIK_CHI_HU = 0x40;                             //吃胡类型
        public const int WIK_ZI_MO = 0x80;								//自摸
        //////////////////////////////////////////////////////////////////////////
        //胡牌定义

        //胡牌
        public const UInt32 CHK_NULL = 0x00;										//非胡类型

        // 大胡
        //public const UInt32 CHR_PENGPENG_HU = 0x00000001;								//碰碰胡
        public const UInt32 CHR_HU_258 = 0x00000002;                                    //胡258
        public const UInt32 CHR_QING_YI_SE = 0x00000004;								//清一色
        public const UInt32 CHR_JIANG_258 = 0x00000008;                                 //将258
        //public const UInt32 CHR_HAI_DI_PAO = 0x00000010;								//海底捞
        public const UInt32 CHR_QI_XIAO_DUI = 0x00000020;								//七小对
        //public const UInt32 CHR_HAOHUA_QI_XIAO_DUI = 0x00000040;						//豪华七小对
        public const UInt32 CHR_GANG_KAI = 0x00000080;									//杠上开花
        public const UInt32 CHR_QIANG_GANG_HU = 0x00000100;								//抢杠胡
        public const UInt32 CHR_GANG_SHANG_PAO = 0x00000200;                            //杠上炮
        public const UInt32 CHR_ZI_MO = 0x01000000;                                     //自摸
        //public const UInt32 CHR_QUAN_QIU_REN = 0x00000400;								//全求人

        //胡258将 加番
        public static bool IsHu258JiaFan(UInt32 value)
        {
            return ((value & CHR_HU_258) == CHR_HU_258) ? true : false;
        }
        //将258将 加番
        public static bool IsJiang258JiaFan(UInt32 value)
        {
            return ((value & CHR_JIANG_258) == CHR_JIANG_258) ? true : false;
        }
        //清一色
        public static bool IsQingYiSe(UInt32 value)
        {
            return ((value & CHR_QING_YI_SE) == CHR_QING_YI_SE) ? true : false;
        }
        //是否七对
        public static bool IsQiDui(UInt32 value)
        {
            return ((value & CHR_QI_XIAO_DUI) == CHR_QI_XIAO_DUI) ? true : false;
        }

        //小胡
        //public const UInt32 CHR_XIAO_DA_SI_XI = 0x00004000;								//大四喜
        //public const UInt32 CHR_XIAO_BAN_BAN_HU = 0x00008000;							//板板胡
        //public const UInt32 CHR_XIAO_QUE_YI_SE = 0x00010000;							//缺一色
        //public const UInt32 CHR_XIAO_LIU_LIU_SHUN = 0x00020000;							//六六顺


        //public const UInt32 CHR_SHU_FAN = 0x02000000;									//素翻
        //////////////////////////////////////////////////////////////////////////

        //参数定义
        public const UInt16 INVALID_CHAIR = 0xFFFF;								//无效椅子
        public const UInt16 INVALID_TABLE = 0xFFFF;                             //无效桌子

        //结束原因
        public const Byte GER_NORMAL = 0x00;                                    //常规结束
        public const Byte GER_DISMISS = 0x01;                                   //游戏解散
        public const Byte GER_USER_LEAVE = 0x02;                                //用户离开
        public const Byte GER_NETWORK_ERROR = 0x03;                             //网络错误

        //离开原因
        public const Byte LER_NORMAL = 0x00;                            //常规离开
        public const Byte LER_SYSTEM = 0x01;                                //系统原因
        public const Byte LER_NETWORK = 0x02;                                   //网络原因
        public const Byte LER_USER_IMPACT = 0x03;                                   //用户冲突
        public const Byte LER_SERVER_FULL = 0x04;                                   //人满为患
        public const Byte LER_SERVER_CONDITIONS = 0x05;                                 //条件限制

        //游戏状态
        public const int GAME_STATUS_FREE = 0;									//空闲状态
        public const int GAME_STATUS_PLAY = 100;									//游戏状态
        public const int GAME_STATUS_WAIT = 200;                                    //等待状态

        //客户端命令结构

        public const int SUB_C_OUT_CARD = 1;                                    //出牌命令
        public const int SUB_C_OPERATE_CARD = 3;                                //操作扑克
        public const int SUB_C_TRUSTEE = 4;                                 //用户托管
        public const int SUB_C_XIAOHU = 5;                                  //小胡

        public const int MASK_CHI_HU_RIGHT = 0x0fffffff;
        public const int MAX_RIGHT_COUNT = 1;                                   //最大权位DWORD个数	


        

    }

    //设置相关
    public class CMD_Set
    {
        //胡牌类型
        public enum HuCardsMode
        {
            CommonHu,   //普通平胡
            OneselfHu,  //自摸胡
        }

        //是否可胡七对
        public enum HuSevenPairs
        {
            NoSevenPairs = -1,   //没有七对
            DoubleSevenPairs = 0,  //七对加翻
            UseSevenPairs = 1    // 七对不加番
        }

        //下炮规则
        public enum StakeRule
        {
            RandomStake,    //任意注
            OneStake,   //一注
            TwoStake,   //两注
            ThreeStake, //三注
            FourStake,  //四注
        }

        //可选功能
        //是否带风
        private const UInt32 m_DaiFeng = 0x00000001;
        //是否有红中赖子
        private const UInt32 m_HongZhong = 0x00000002;
        //是否258将
        private const UInt32 m_258Jiang = 0x00000004;
        //胡258将 加番
        private const UInt32 m_Hu258JiaFan = 0x00000008;
        //将258将 加番
        private const UInt32 m_Jiang258JiaFan = 0x00000008;
        //清一色
        private const UInt32 m_QingYiSe = 0x00000011;

        //是否带风
        public static bool IsDaiFeng(UInt32 value)
        {
            return ((value & m_DaiFeng) == m_DaiFeng) ? true : false;
        }
        //是否红中赖子
        public static bool IsHongZhong(UInt32 value)
        {
            return ((value & m_HongZhong) == m_HongZhong) ? true : false;
        }
        //是否258将
        public static bool Is258Jiang(UInt32 value)
        {
            return ((value & m_258Jiang) == m_258Jiang) ? true : false;
        }
        //胡258将 加番
        public static bool IsHu258JiaFan(UInt32 value)
        {
            return ((value & m_Hu258JiaFan) == m_Hu258JiaFan) ? true : false;
        }
        //将258将 加番
        public static bool IsJiang258JiaFan(UInt32 value)
        {
            return ((value & m_Jiang258JiaFan) == m_Jiang258JiaFan) ? true : false;
        }
        //清一色
        public static bool IsQingYiSe(UInt32 value)
        {
            return ((value & m_QingYiSe) == m_QingYiSe) ? true : false;
        }

        //坐庄规则
        public enum BankerSet
        {
            Everyone,   //人人做庄
            Win,    //赢家做庄
        }
        //点炮付钱规则
        public enum PayHu
        {
            OnePay, //一个人给
            ThreePay, //三个人给
        }

        //流局规则
        public enum FlowBureau
        {
            CardOver,   //所以纸牌抓完
            RemainCard, //一杠七二杠八
        }

        //自摸加番
        public enum OneselfHu
        {
            HuTwice,    //自摸加番
            HuOnce,     //自摸不加番
        }
    }

    //黄庄设置
    public class CMD_FlowBureau
    {
        public const int ZeroGang = 26; //零杠
        public const int OneGang = 15; //一杠
        public const int TwoGang = 16; //二杠
        public const int ThreeGang = 27; //三杠
    }

    //扑克数据
    //protected static byte[] mCardDataArray = new byte[]
    //{
    //        0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
    //     0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
    //     0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
    //     0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
    //     0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子  17
    //     0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子
    //     0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子
    //     0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子
    //     0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,						//同子 33
    //     0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,						//同子
    //     0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,						//同子
    //     0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,                       //同子
    //};

    //类型子项
    public class tagKindItem
    {
        public Byte cbWeaveKind;                       //组合类型
        public Byte cbCenterCard;                      //中心扑克
        public Byte[] cbCardIndex = new Byte[3];       //扑克索引
        public Byte[] cbValidIndex = new Byte[3];      //实际扑克索引
        public tagKindItem()
        {
            cbCardIndex.Initialize();
            cbValidIndex.Initialize();
        }
    };

    //组合子项
    public class tagWeaveItem
    {
        public Byte cbWeaveKind;                       //组合类型
        public Byte cbCenterCard;                      //中心扑克
        public Byte cbPublicCard;                      //公开标志
        public UInt16 wProvideUser;                      //供应用户
    };

    //杠牌结果
    public class tagGangCardResult
    {
        public Byte cbCardCount;                       //扑克数目
        public Byte[] cbCardData = new Byte[4];        //扑克数据
        public tagGangCardResult()
        {
            cbCardData.Initialize();
        }
    };

    //分析子项
    public class tagAnalyseItem
    {
        public Byte cbCardEye;                         //牌眼扑克
        bool bMagicEye;                                //牌眼是否是王霸
        public Byte[] cbWeaveKind = new Byte[4];       //组合类型
        public Byte[] cbCenterCard = new Byte[4];      //中心扑克
        public Byte[][] cbCardData = new Byte[4][];     //实际扑克 4,4
        public tagAnalyseItem()
        {
            cbWeaveKind.Initialize();
            cbCenterCard.Initialize();
            cbCardData.Initialize();
            for (int i=0; i< cbCardData.Length; i++)
            {
                cbCardData[i] = new byte[4];
                cbCardData[i].Initialize();
            }
        }
    };

    //效验类型
    public enum enEstimatKind
    {
        EstimatKind_OutCard,            //出牌效验
        EstimatKind_GangCard,           //杠牌效验
    };

    //杠牌得分
    public class tagGangScore
    {
        public byte cbGangCount;                                                            //杠个数
        public int m_score;

        public int[,] lScore = new int[CMD_SXMJ.MAX_WEAVE,CMD_SXMJ.GAME_PLAYER];			//每个杠得分
        /// <summary>
        /// 取得分的字节值
        /// </summary>
        /// <returns></returns>
        /*public byte[] GetGangScore()
        {
            byte[] score = new byte[CMD_SXMJ.GAME_PLAYER*4];
            score.Initialize();
            for (int i = 0; i < score.Length; i++)
            {
                score[i] = (byte)(lScore[cbGangCount,i / 4]>>((i % 4)*8) & 0xff);
            }
            return score;
        }*/
    };

    //游戏开始
    public class CMD_S_GameStart
    {
        public int lSiceCount;                                                            //骰子点数
        public UInt16 wBankerUser;                                                           //庄家用户
        public UInt16 wCurrentUser;                                                          //当前用户
        public byte cbUserAction;                                                           //用户动作
        public byte[] cbCardData = new byte[CMD_SXMJ.MAX_COUNT * CMD_SXMJ.GAME_PLAYER];     //扑克列表
        public byte cbLeftCardCount;                                                        //
    };

    //出牌命令
    public class CMD_S_OutCard
    {
        public UInt16 wOutCardUser;                      //出牌用户
        public byte cbOutCardData;                     //出牌扑克
    };

    //发送扑克
    public class CMD_S_SendCard
    {
        public byte cbCardData;                          //扑克数据
        public byte cbActionMask;                        //动作掩码
        public UInt16 wCurrentUser;                      //当前用户
        public bool bTail;                               //末尾发牌
    };

    //操作命令
    public class CMD_S_OperateResult
    {
        public UInt16 wOperateUser;                     //操作用户
        public UInt16 wProvideUser;                     //供应用户
        public byte cbOperateCode;                      //操作代码
        public byte cbOperateCard;                      //操作扑克

        //如果是杠
        //public bool cbXiaYu;//是否下雨，即暗杠
        //public int[] lGangScore = new int[CMD_SXMJ.GAME_PLAYER];//杠得分

        /// <summary>
        /// 取得分的字节值
        /// </summary>
        /// <returns></returns>
        /*public byte[] GetGangScore()
        {
            byte[] score = new byte[CMD_SXMJ.GAME_PLAYER*4];
            score.Initialize();
            for (int i = 0; i < score.Length; i++)
            {
                score[i] = (byte)(lGangScore[i / 4]>>(i % 4)*8 & 0xff);
            }
            return score;
        }*/
    };

    //吃胡命令
    public class CMD_S_ChiHu
    {
        public UInt16 wChiHuUser;                            //胡牌用户
        public UInt16 wProviderUser;                     //供应用户
        public byte cbChiHuCard;                       //胡牌扑克
        public byte cbCardCount;                       //扑克数量
        public int lGameScore;                            //得分
        public byte cbWinOrder;                            //
    };

    //操作提示
    public class CMD_S_OperateNotify
    {
        public UInt16 wResumeUser;                       //还原用户
        public byte cbActionMask;                      //动作掩码
        public byte cbActionCard;                      //动作扑克
    };
    //游戏结束
    //游戏结束
    public class CMD_S_GameEnd
    {
        public byte[] cbCardCount = new byte[CMD_SXMJ.GAME_PLAYER];          //用户手牌数
        public byte[][] cbCardData = new byte[CMD_SXMJ.GAME_PLAYER][];   // CMD_SXMJ.MAX_COUNT 14 用户手牌
        //结束信息
        //public UInt16[] wProvideUser = new UInt16[CMD_SXMJ.GAME_PLAYER];         //供应用户
        //public UInt32[] dwChiHuRight = new UInt32[CMD_SXMJ.GAME_PLAYER];            //胡牌类型

        public UInt16 wProvideUser;   //供应用户
        public byte cbChiHuCard;    //吃胡的牌
        public UInt16 wWinUser;   //胡牌用户
        public UInt32 dwChiHuRight;  //胡牌类型

        //积分信息
        //public int[] lGameScore = new int[CMD_SXMJ.GAME_PLAYER];           //游戏积分
        //组合扑克
        public byte[] cbWeaveCount = new byte[CMD_SXMJ.GAME_PLAYER];                 //组合数目
        public tagWeaveItem[][] WeaveItemArray = new tagWeaveItem[CMD_SXMJ.GAME_PLAYER][];		//组合扑克 CMD_SXMJ.MAX_WEAVE 4
        public CMD_S_GameEnd()
        {
            cbCardCount.Initialize();
            for(int i=0; i< cbCardData.Length; i++)
            {
                cbCardData[i] = new byte[CMD_SXMJ.MAX_COUNT];
                cbCardData[i].Initialize();
            }
            //wProvideUser.Initialize();
            //dwChiHuRight.Initialize();
            //lGameScore.Initialize();
            //cbWeaveCount.Initialize();
            //for (int i = 0; i < WeaveItemArray.Length; i++)
            //{
            //    WeaveItemArray[i] = new CMD_WeaveItem[CMD_SXMJ.MAX_WEAVE];
            //    WeaveItemArray[i].Initialize();
            //}
        }
    };
}
