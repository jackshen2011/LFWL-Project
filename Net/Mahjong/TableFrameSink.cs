using FTLibrary.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace Mahjong
{
    //麻将的桌子类
    public class TableFrameSink
    {
        //无风 的 扑克数据
        protected static byte[] m_cbCardDataArray = new byte[]
        {
            0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
	        0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子
	        0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,						//同子  27 
        };

        //无风 的 扑克数据
        protected static byte[] m_cbCardDataArray_HZ = new byte[]
        {
            0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
	        0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子
	        0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,						//同子  27 
        };

        //有风的 扑克数据
        protected static byte[] m_cbCardDataArray_Feng = new byte[]
        {
            0x01,0x02,0x03,0x04,0x05,0x06,0x07,0x08,0x09,						//万子
	        0x11,0x12,0x13,0x14,0x15,0x16,0x17,0x18,0x19,						//索子
	        0x21,0x22,0x23,0x24,0x25,0x26,0x27,0x28,0x29,						//同子
            //东 南 西 北 红中 发财 白板
	        0x31,0x32,0x33,0x34,0x35,0x36,0x37,						//字 27 + 7 = 34
        };
        public UInt32 HuCardsOptions = 0x00000000;  //胡牌可选项
        public CMD_Set.HuSevenPairs huSevenPairs = CMD_Set.HuSevenPairs.NoSevenPairs;  //是否可胡七对 (少七对结算 加番 算分)
        public CGameLogic m_GameLogic = new CGameLogic();
        //吃胡牌型判断
        byte AnalyseChiHuCard(byte[] cbCardIndex, tagWeaveItem[] WeaveItem, byte cbWeaveCount, byte cbCurrentCard, ref UInt32 ChiHuRight)
        {
            //变量定义
            byte cbChiHuKind = CMD_SXMJ.WIK_NULL;
            List<tagAnalyseItem> AnalyseItemArray = new List<tagAnalyseItem>();
            //设置变量
            ChiHuRight = 0x0;

            //构造扑克
            byte[] cbCardIndexTemp = new byte[CMD_SXMJ.MAX_INDEX];
            Array.Copy(cbCardIndex, cbCardIndexTemp, CMD_SXMJ.MAX_INDEX);

            //cbCurrentCard一定不为0			!!!!!!!!!
            Debug.Assert(cbCurrentCard != 0);
            if (cbCurrentCard == 0) return CMD_SXMJ.WIK_NULL;

            //插入扑克
            if (cbCurrentCard != 0)
                cbCardIndexTemp[m_GameLogic.SwitchToCardIndex(cbCurrentCard)]++;

            //分析扑克
            bool bValue = m_GameLogic.AnalyseCard(cbCardIndexTemp, WeaveItem, cbWeaveCount, ref AnalyseItemArray);
            if (!bValue)
            {
                //如果没有胡牌 则分析是否 七小对牌
                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                   huSevenPairs != CMD_Set.HuSevenPairs.NoSevenPairs)
                {
                    int nGenCount = 0;
                    if (m_GameLogic.IsQiXiaoDui(cbCardIndex, WeaveItem, cbWeaveCount, cbCurrentCard, nGenCount))
                    {
                        ChiHuRight |= CMD_SXMJ.CHR_QI_XIAO_DUI;

                        //如果是 258 硬将
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                            CMD_Set.Is258Jiang(HuCardsOptions) == true)
                        {
                            for (int i = 0; i < cbCardIndex.Length; i++)
                            {
                                if (cbCardIndex[i] > 0)
                                {
                                    byte cbCardData_1 = m_GameLogic.SwitchToCardData((byte)i);
                                    byte cbCardValue_1 = (byte)(cbCardData_1 & CMD_SXMJ.MASK_VALUE);
                                    if (cbCardValue_1 < 0x29 && (cbCardValue_1 == 2 || cbCardValue_1 == 5 || cbCardValue_1 == 8))
                                    {
                                        ChiHuRight |= CMD_SXMJ.CHR_JIANG_258;
                                        cbChiHuKind = CMD_SXMJ.WIK_CHI_HU;
                                        break;
                                    }
                                }


                            }

                            //有2 5 8 则胡， 没有2 5 8 则不胡 并且返回
                            if (cbChiHuKind != CMD_SXMJ.WIK_CHI_HU)
                            {
                                ChiHuRight = 0;
                                return CMD_SXMJ.WIK_NULL;
                            }
                        }
                        cbChiHuKind = CMD_SXMJ.WIK_CHI_HU;

                        //清一色
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                               CMD_Set.IsQingYiSe(HuCardsOptions) == true)
                        {
                            if (cbChiHuKind == CMD_SXMJ.WIK_CHI_HU)
                            {
                                //清一色牌
                                if (m_GameLogic.IsQingYiSe(cbCardIndex, WeaveItem, cbWeaveCount, cbCurrentCard))
                                    ChiHuRight |= CMD_SXMJ.CHR_QING_YI_SE;
                            }
                        }

                        //胡258
                        if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                               CMD_Set.IsHu258JiaFan(HuCardsOptions) == true)
                        {
                            if (cbChiHuKind == CMD_SXMJ.WIK_CHI_HU)
                            {
                                //胡258
                                if (cbCurrentCard < 0x29 && (cbCurrentCard == 2 || cbCurrentCard == 5 || cbCurrentCard == 8))
                                {
                                    ChiHuRight |= CMD_SXMJ.CHR_HU_258;
                                }
                            }
                        }
                    }
                    else
                    {
                        ChiHuRight = 0;
                        return CMD_SXMJ.WIK_NULL;
                    }

                    return cbChiHuKind;
                }
                else
                {
                    ChiHuRight = 0;
                    return CMD_SXMJ.WIK_NULL;
                }
            }

            bool isYingJiang258 = false;
            /*if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.CARDROOM)
            {
                int[] roomSetting = MainRoot._gRoomData.cCurRoomData.vRoomSetting;
                isYingJiang258 = ((roomSetting[5] > -1) && (roomSetting[5] & 0x04) == 0x04) ? true : false; //258硬将.
            }*/
               

            ////胡牌分析
            ////牌型分析
            for (int i = 0; i < AnalyseItemArray.Count; i++)
            {
                tagAnalyseItem pAnalyseItem = AnalyseItemArray[i];
                byte cbCardValue = (byte)(pAnalyseItem.cbCardEye & CMD_SXMJ.MASK_VALUE);

                //如果是 258 硬将
                if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                    CMD_Set.Is258Jiang(HuCardsOptions) == true)
                {

                    if (pAnalyseItem.cbCardEye < 0x29 && cbCardValue != 2 && cbCardValue != 5 && cbCardValue != 8)
                    {
                        continue;
                    }
                    ChiHuRight |= CMD_SXMJ.CHR_JIANG_258;
                    cbChiHuKind = CMD_SXMJ.WIK_CHI_HU;
                }
                else
                {
                    //先胡
                    cbChiHuKind = CMD_SXMJ.WIK_CHI_HU;
                    if (pAnalyseItem.cbCardEye < 0x29 && cbCardValue != 2 && cbCardValue != 5 && cbCardValue != 8)
                    {
                        continue;
                    }
                    ChiHuRight |= CMD_SXMJ.CHR_JIANG_258;
                }

            }

            //清一色
            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                   CMD_Set.IsQingYiSe(HuCardsOptions) == true)
            {
                if (cbChiHuKind == CMD_SXMJ.WIK_CHI_HU)
                {
                    //清一色牌
                    if (m_GameLogic.IsQingYiSe(cbCardIndex, WeaveItem, cbWeaveCount, cbCurrentCard))
                        ChiHuRight |= CMD_SXMJ.CHR_QING_YI_SE;
                }
            }

            //胡258
            if (MainRoot._gRoomData.cCurRoomData.eRoomType == OneRoomData.RoomType.RoomType_RoomCard &&
                   CMD_Set.IsHu258JiaFan(HuCardsOptions) == true)
            {
                if (cbChiHuKind == CMD_SXMJ.WIK_CHI_HU)
                {
                    //胡258
                    if (cbCurrentCard < 0x29 && (cbCurrentCard == 2 || cbCurrentCard == 5 || cbCurrentCard == 8))
                    {
                        ChiHuRight |= CMD_SXMJ.CHR_HU_258;
                    }
                }
            }


            return cbChiHuKind;
        }

        /// <summary>
        /// 获取停牌提示 （i牌，i+1翻型）能胡的牌以及翻数
        /// </summary>
        /// <param name="cbCardIndex">牌序列</param>
        /// <param name="WeaveItem">碰杠吃序列</param>
        /// <param name="cbWeaveCount">碰杠吃数目</param>
        /// <returns>获取能胡的牌以及翻数</returns>
        public UInt32[] GetSuspensionData(byte[] cbCardIndex, tagWeaveItem[] WeaveItem, byte cbWeaveCount)
        {
            List<UInt32> result=new List<uint>();
            byte[] checkCardList = null;
            if (CMD_Set.IsDaiFeng(HuCardsOptions))
            {
                checkCardList = m_cbCardDataArray_Feng;
            }
            else if(CMD_Set.IsHongZhong(HuCardsOptions))
            {
                checkCardList = m_cbCardDataArray_HZ;
            }
            else
            {
                checkCardList = m_cbCardDataArray;
            }

            UInt32 ChiHuRight = 0;
            for (int i = 0; i< checkCardList.Length; i++)
            {
                if(CMD_SXMJ.WIK_CHI_HU == AnalyseChiHuCard(cbCardIndex, WeaveItem, cbWeaveCount, checkCardList[i], ref ChiHuRight))
                {
                    //记录牌型 以及ChiHuRight
                    result.Add(checkCardList[i]);
                    //计算倍数
                    UInt32 multiple = 1;
                    //胡258
                    if (CMD_Set.IsHu258JiaFan(HuCardsOptions) == true &&
                        CMD_SXMJ.IsHu258JiaFan(ChiHuRight) == true)
                    {
                        multiple = multiple * 2;
                    }
                    //将258
                    if (CMD_Set.IsJiang258JiaFan(HuCardsOptions) == true &&
                        CMD_SXMJ.IsJiang258JiaFan(ChiHuRight) == true)
                    {
                        multiple = multiple * 2;
                    }
                    //清一色
                    if (CMD_Set.IsQingYiSe(HuCardsOptions) == true &&
                        CMD_SXMJ.IsQingYiSe(ChiHuRight) == true)
                    {
                        multiple = multiple * 2;
                    }
                    //七对加番
                    if (huSevenPairs == CMD_Set.HuSevenPairs.DoubleSevenPairs &&
                        CMD_SXMJ.IsQiDui(ChiHuRight) == true)
                    {
                        multiple = multiple * 2;
                    }
                    result.Add(multiple);
                }
            }

            return result.ToArray();
        }
    }
}
