using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Mahjong
{
    //麻将算法
    public class CGameLogic
    {
        public const byte MASK_COLOR = 0xF0;								//花色掩码
        public const byte MASK_VALUE = 0x0F;								//数值掩码

        /*******************************************胡法分析**********************************************/
        //网狐的 徐州麻将算法
        //扑克转换
        public byte SwitchToCardData(byte cbCardIndex)
        {
            Debug.Assert(cbCardIndex < 34);
            return (byte)(((cbCardIndex / 9) << 4) | (cbCardIndex % 9 + 1));
        }
        //扑克转换
        public byte SwitchToCardData(byte[] cbCardIndex, ref byte[] cbCardData)  //MAX_INDEX //MAX_COUNT
        {
            //转换扑克
            byte cbPosition = 0;

            for (byte i = 0; i < CMD_SXMJ.MAX_INDEX; i++)
            {
                if (cbCardIndex[i] != 0)
                {
                    for (byte j = 0; j < cbCardIndex[i]; j++)
                    {

                        Debug.Assert(cbPosition < CMD_SXMJ.MAX_COUNT);
                        cbCardData[cbPosition++] = SwitchToCardData(i);
                    }
                }
            }

            return cbPosition;
        }

        public bool IsValidCard(byte cbCardData)
        {
            byte cbValue = (byte)(cbCardData & MASK_VALUE);
            byte cbColor = (byte)((cbCardData & MASK_COLOR) >> 4);
            return (((cbValue >= 1) && (cbValue <= 9) && (cbColor <= 2)) || ((cbValue >= 1) && (cbValue <= 7) && (cbColor == 3)));
        }

        //扑克转换
        public byte SwitchToCardIndex(byte cbCardData)
        {
            Debug.Assert(IsValidCard(cbCardData));
            return (byte)(((cbCardData & MASK_COLOR) >> 4) * 9 + (cbCardData & MASK_VALUE) - 1);
        }
        //扑克转换
        public byte SwitchToCardIndex(byte[] cbCardData, int beginPosIndex, byte cbCardCount, out byte[] cbCardIndex)
        {
            //设置变量
            //ZeroMemory(cbCardIndex,sizeof(byte)*MAX_INDEX);
            cbCardIndex = new byte[CMD_SXMJ.MAX_INDEX];
            cbCardIndex.Initialize();
            //转换扑克
            for (byte i = 0; i < cbCardCount; i++)
            {

                Debug.Assert(IsValidCard(cbCardData[i + beginPosIndex]));
                cbCardIndex[SwitchToCardIndex(cbCardData[i + beginPosIndex])]++;
            }

            return cbCardCount;
        }


        public bool AnalyseCard(byte[] cbCardIndex, tagWeaveItem[] WeaveItem, byte cbWeaveCount, ref List<tagAnalyseItem>  AnalyseItemArray)
        {
            //计算数目
            byte cbCardCount = 0;
            for (byte i = 0; i < CMD_SXMJ.MAX_INDEX; i++)
                cbCardCount += cbCardIndex[i];

            //效验数目
            Debug.Assert((cbCardCount >= 2) && (cbCardCount <= CMD_SXMJ.MAX_COUNT) && ((cbCardCount - 2) % 3 == 0));
            if ((cbCardCount < 2) || (cbCardCount > CMD_SXMJ.MAX_COUNT) || ((cbCardCount - 2) % 3 != 0))
                return false;

            //变量定义
            byte cbKindItemCount = 0;

            tagKindItem[] KindItem = new tagKindItem[CMD_SXMJ.MAX_COUNT - 2]; // Enumerable.Repeat(new tagKindItem(), CMD_SXMJ.MAX_COUNT - 2).ToArray();
            for(int i= 0; i< KindItem.Length; i++)
            {
                KindItem[i] = new tagKindItem();
            }
            //tagKindItem[] KindItem = Enumerable.Repeat(new tagKindItem(), CMD_SXMJ.MAX_COUNT - 2).ToArray();
            //需求判断
            byte cbLessKindItem = (byte)((cbCardCount - 2) / 3);
            Debug.Assert((cbLessKindItem + cbWeaveCount) == 4);

            //单吊判断
            if (cbLessKindItem == 0)
            {
                //效验参数
                Debug.Assert((cbCardCount == 2) && (cbWeaveCount == 4));

                //牌眼判断
                for (byte i = 0; i < CMD_SXMJ.MAX_INDEX; i++)
                {
                    if (cbCardIndex[i] == 2)
                    {
                        //变量定义
                        tagAnalyseItem AnalyseItem = new tagAnalyseItem();

                        //设置结果
                        for (byte j = 0; j < cbWeaveCount; j++)
                        {
                            AnalyseItem.cbWeaveKind[j] = WeaveItem[j].cbWeaveKind;
                            AnalyseItem.cbCenterCard[j] = WeaveItem[j].cbCenterCard;
                        }
                        AnalyseItem.cbCardEye = SwitchToCardData(i);

                        //插入结果
                        AnalyseItemArray.Add(AnalyseItem);

                        return true;
                    }
                }

                return false;
            }

            //拆分分析
            if (cbCardCount >= 3)
            {
                for (byte i = 0; i < CMD_SXMJ.MAX_INDEX; i++)
                {
                    //同牌判断
                    if (cbCardIndex[i] >= 3)
                    {
                        KindItem[cbKindItemCount].cbCardIndex[0] = i;
                        KindItem[cbKindItemCount].cbCardIndex[1] = i;
                        KindItem[cbKindItemCount].cbCardIndex[2] = i;
                        KindItem[cbKindItemCount].cbWeaveKind = CMD_SXMJ.WIK_PENG;
                        KindItem[cbKindItemCount++].cbCenterCard = SwitchToCardData(i);
                    }
                        //连牌判断

                    if ((i < (CMD_SXMJ.MAX_INDEX - 9)) && (cbCardIndex[i] > 0) && ((i % 9) < 7))
                    {
                        for (byte j = 1; j <= cbCardIndex[i]; j++)
                        {
                            if ((cbCardIndex[i + 1] >= j) && (cbCardIndex[i + 2] >= j))
                            {
                                KindItem[cbKindItemCount].cbCardIndex[0] = i;
                                KindItem[cbKindItemCount].cbCardIndex[1] = (byte)(i + 1);
                                KindItem[cbKindItemCount].cbCardIndex[2] = (byte)(i + 2);
                                KindItem[cbKindItemCount].cbWeaveKind = CMD_SXMJ.WIK_LEFT;
                                KindItem[cbKindItemCount++].cbCenterCard = SwitchToCardData(i);
                            }
                        }
                    }
                }
            }

            //组合分析
            if (cbKindItemCount >= cbLessKindItem)
            {
                //变量定义
                byte[] cbCardIndexTemp = new byte[CMD_SXMJ.MAX_INDEX];
                cbCardIndexTemp.Initialize();

                //变量定义
                byte[] cbIndex = { 0, 1, 2, 3 };
                tagKindItem[] pKindItem = new tagKindItem[4];
                for(int i=0; i< 4; i++)
                {
                    pKindItem[i] = new tagKindItem();
                }

                //开始组合
                do
                {
                    //设置变量
                    Array.Copy(cbCardIndex, cbCardIndexTemp, cbCardIndexTemp.Length);
                    for (byte i = 0; i < cbLessKindItem; i++)
                        pKindItem[i] = KindItem[cbIndex[i]];

                    //数量判断
                    bool bEnoughCard = true;
                    for (byte i = 0; i < cbLessKindItem * 3; i++)
                    {
                        //存在判断
                        byte Index = (byte)(pKindItem[i / 3].cbCardIndex[i % 3]);
                        if (cbCardIndexTemp[Index] == 0)
                        {
                            bEnoughCard = false;
                            break;
                        }
                        else
                            cbCardIndexTemp[Index]--;
                    }

                    //胡牌判断
                    if (bEnoughCard == true)
                    {
                        //牌眼判断
                        byte cbCardEye = 0;
                        for (byte i = 0; i < CMD_SXMJ.MAX_INDEX; i++)
                        {
                            if (cbCardIndexTemp[i] == 2)
                            {
                                cbCardEye = SwitchToCardData(i);
                                break;
                            }
                        }

                        //组合类型
                        if (cbCardEye != 0)
                        {
                            //变量定义
                            tagAnalyseItem AnalyseItem = new tagAnalyseItem();

                            //设置组合
                            for (byte i = 0; i < cbWeaveCount; i++)
                            {
                                AnalyseItem.cbWeaveKind[i] = WeaveItem[i].cbWeaveKind;
                                AnalyseItem.cbCenterCard[i] = WeaveItem[i].cbCenterCard;
                            }

                            //设置牌型
                            for (byte i = 0; i < cbLessKindItem; i++)
                            {
                                AnalyseItem.cbWeaveKind[i + cbWeaveCount] = pKindItem[i].cbWeaveKind;
                                AnalyseItem.cbCenterCard[i + cbWeaveCount] = pKindItem[i].cbCenterCard;
                            }

                            //设置牌眼
                            AnalyseItem.cbCardEye = cbCardEye;

                            //插入结果
                            AnalyseItemArray.Add(AnalyseItem);
                        }
                    }

                    //设置索引
                    if (cbIndex[cbLessKindItem - 1] == (cbKindItemCount - 1))
                    {
                        byte i = (byte)(cbLessKindItem - 1);
                        for (; i > 0; i--)
                        {
                            if ((cbIndex[i - 1] + 1) != cbIndex[i])
                            {
                                byte cbNewIndex = cbIndex[i - 1];
                                for (byte j = (byte)(i - 1); j < cbLessKindItem; j++)
                                    cbIndex[j] = (byte)(cbNewIndex + j - i + 2);
                                break;
                            }
                        }
                        if (i == 0)
                            break;
                    }
                    else
                        cbIndex[cbLessKindItem - 1]++;

                } while (true);

            }

            return (AnalyseItemArray.Count > 0);
        }


        //七小对牌
        public bool IsQiXiaoDui(byte[] cbCardIndex, tagWeaveItem[] WeaveItem, byte cbWeaveCount, byte cbCurrentCard,int nGenCount)
        {
	        //组合判断
	        if (cbWeaveCount!=0) return false;

            //单牌数目
            byte cbReplaceCount = 0;
                nGenCount = 0;

            //临时数据
            byte[] cbCardIndexTemp = new byte[CMD_SXMJ.MAX_INDEX];
            Array.Copy(cbCardIndex, cbCardIndexTemp, cbCardIndex.Length);

            //插入数据
            byte cbCurrentIndex = SwitchToCardIndex(cbCurrentCard);
                cbCardIndexTemp[cbCurrentIndex]++;

	        //计算单牌
	        for (byte i = 0; i< CMD_SXMJ.MAX_INDEX;i++)
	        {
                byte cbCardCount = cbCardIndexTemp[i];

		        //单牌统计
		        if( cbCardCount == 1 || cbCardCount == 3 ) 	cbReplaceCount++;

		        if (cbCardCount == 4 )
		        {
			        nGenCount++;
		        }
            }
	
	        //有单牌的时候
	        if(cbReplaceCount > 0 )
		        return false;

	        return true;

        }

        //清一色牌
        public bool IsQingYiSe(byte[] cbCardIndex, tagWeaveItem[] WeaveItem, byte cbItemCount, byte cbCurrentCard)
        {
            //胡牌判断
            byte cbCardColor = 0xFF;

	        for (byte i = 0; i<CMD_SXMJ.MAX_INDEX;i++)
	        {
		        if (cbCardIndex[i]!=0)
		        {
			        //花色判断
			        if (cbCardColor!=0xFF)
				        return false;

			        //设置花色
			        cbCardColor=(byte)(SwitchToCardData(i)& MASK_COLOR);

			        //设置索引
			        i=(byte)((i / 9 + 1) * 9 - 1);
		        }
            }


	        if((cbCurrentCard&MASK_COLOR)!=cbCardColor) return false;

	        //组合判断
	        for (byte i = 0; i<cbItemCount;i++)
	        {
                byte cbCenterCard = WeaveItem[i].cbCenterCard;
		        if ((cbCenterCard&MASK_COLOR)!=cbCardColor)	return false;
	        }

	        return true;
        }
    }
}
