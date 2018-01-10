using UnityEngine;
using System.Collections;
using System;

class RoomCardInfoCtrl : MonoBehaviour
{
    /// <summary>
    /// 结束时间信息.
    /// </summary>
    public TextBase TimeText;
    /// <summary>
    /// 房主信息.
    /// </summary>
    public TextBase FangZhuText;
    /// <summary>
    /// 玩法信息.
    /// </summary>
    public TextBase WanFaText;
    /// <summary>
    /// 特色信息.
    /// </summary>
    public TextBase TeSeText;
    public void InitRoomCardInfo()
    {
        string fangZhuName = MainRoot._gRoomData.cCurRoomData.sRoomOwnerName;
        string wanFaInfo = "";
        string teSeInfo = "";
        int[] roomSetting = MainRoot._gRoomData.cCurRoomData.vRoomSetting;
        //玩法start.
        if (roomSetting == null)
        {
            Debug.Log("InitRoomCardInfo -> roomSetting is null");
            return;
        }

        bool isPuTongHu = roomSetting[2] == 0x00 ? true : false; //普通平胡.
        bool isZiMoHu = roomSetting[2] == 0x01 ? true : false; //自摸胡.
        bool isHuQiDuiJiaFan = roomSetting[3] == 0x00 ? true : false; //胡七对加番.
        bool isHuQiDuiBuJiaFan = roomSetting[3] == 0x01 ? true : false; //胡七对不加番.
        byte paoShuState = (byte)roomSetting[4]; //炮数类型.
        bool isDaiFengPai = ((roomSetting[5] > -1) && (roomSetting[5] & 0x01) == 0x01) ? true : false; //带风牌.
        bool isHongZhongLaiZi = ((roomSetting[5] > -1) && (roomSetting[5] & 0x02) == 0x02) ? true : false; //红中赖子.
        bool isYingJiang258 = ((roomSetting[5] > -1) && (roomSetting[5] & 0x04) == 0x04) ? true : false; //258硬将.
        bool isHuJiaFan258 = ((roomSetting[5] > -1) && (roomSetting[5] & 0x08) == 0x08) ? true : false; //胡258（加番）.
        bool isJiangJiaFan258 = ((roomSetting[5] > -1) && (roomSetting[5] & 0x10) == 0x10) ? true : false; //将258（加番）.
        bool isQingYiSeFan = ((roomSetting[5] > -1) && (roomSetting[5] & 0x20) == 0x20) ? true : false; //清一色（加番）.
        wanFaInfo += isPuTongHu == true ? "普通平胡、" : "";
        wanFaInfo += isZiMoHu == true ? "自摸胡、" : "";
        wanFaInfo += isHuQiDuiJiaFan == true ? "可胡七对（加番）、" : "";
        wanFaInfo += isHuQiDuiBuJiaFan == true ? "可胡七对（不加番）、" : "";
        switch(paoShuState)
        {
            case 0:
                {
                    wanFaInfo += "不可下炮、";
                    break;
                }
            case 1:
            case 2:
            case 3:
            case 4:
                {
                    wanFaInfo += ("固定"+ paoShuState + "炮、");
                    break;
                }
            case 5:
                {
                    wanFaInfo += "自由炮、";
                    break;
                }
        }
        wanFaInfo += isDaiFengPai == true ? "带风牌、" : "";
        wanFaInfo += isHongZhongLaiZi == true ? "红中赖子、" : "";
        wanFaInfo += isYingJiang258 == true ? "258硬将、" : "";
        wanFaInfo += isHuJiaFan258 == true ? "胡258（加番）、" : "";
        wanFaInfo += isJiangJiaFan258 == true ? "将258（加番）、" : "";
        wanFaInfo += isQingYiSeFan == true ? "清一色（加番）、" : "";
        wanFaInfo = wanFaInfo.Remove(wanFaInfo.Length - 1);
        //玩法end.
        //特色start.
        bool isFangZuoBi = false; //防作弊.
        bool isLunLiuZuoZhuang = roomSetting[6] == 0x00 ? true : false; //轮流坐庄.
        bool isYingJiaZuoZhuang = roomSetting[6] == 0x01 ? true : false; //赢家坐庄.
        bool isDianPaoYiJiaFu = roomSetting[7] == 0x00 ? true : false; //点炮一家付.
        bool isDianPaoSanJiaFu = roomSetting[7] == 0x01 ? true : false; //点炮三家付.
        bool isZhuaWanHuangZhuang = roomSetting[8] == 0x00 ? true : false; //抓完荒庄.
        bool isGang17_28 = roomSetting[8] == 0x01 ? true : false; //1杠七2杠八.
        bool isZiMoFanBei = roomSetting[9] == 0x00 ? true : false; //自摸翻倍.
        bool isWuXianZhiDeng = roomSetting[10] == 0x00 ? true : false; //无限制等待.
        bool isDengDai15 = roomSetting[10] == 0x01 ? true : false; //15秒等待.
        bool isPangGuan = false; //旁观.
        bool isDiaoYu = false; //钓鱼.
        int beiShuVal = roomSetting[12]; //底分.
        teSeInfo += isFangZuoBi == true ? "开启防作弊、" : "";
        teSeInfo += isLunLiuZuoZhuang == true ? "轮流坐庄、" : "";
        teSeInfo += isYingJiaZuoZhuang == true ? "赢家坐庄、" : "";
        teSeInfo += isDianPaoYiJiaFu == true ? "点炮一家付、" : "";
        teSeInfo += isDianPaoSanJiaFu == true ? "点炮三家付、" : "";
        teSeInfo += isZhuaWanHuangZhuang == true ? "抓完荒庄、" : "";
        teSeInfo += isGang17_28 == true ? "1杠七2杠八、" : "";
        teSeInfo += isZiMoFanBei == true ? "自摸翻倍、" : "";
        teSeInfo += isWuXianZhiDeng == true ? "无限制等待、" : "";
        teSeInfo += isDengDai15 == true ? "15秒等待、" : "";
        teSeInfo += isPangGuan == true ? "可旁观、" : "";
        teSeInfo += isDiaoYu == true ? "可钓鱼、" : "";
        teSeInfo += beiShuVal + "分、";
        teSeInfo = teSeInfo.Remove(teSeInfo.Length - 1);
        //特色start.
        FangZhuText.text = fangZhuName;
        WanFaText.text = wanFaInfo;
        TeSeText.text = teSeInfo;

        DateTime roomCreatTime = DateTime.Now; //房间创建时间.
        DateTime roomEndTime = roomCreatTime.AddHours(12); //房间结束时间.
        string timeStrVal = string.Format("{0:u}", roomEndTime);
        timeStrVal = timeStrVal.Replace('-', '/');
        timeStrVal = timeStrVal.Remove(timeStrVal.Length - 4);
        timeStrVal += " 结束";
        TimeText.text = timeStrVal;
    }
}
