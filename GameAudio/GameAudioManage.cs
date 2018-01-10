/// <summary>
/// Assets\ftproject\config\config.unity3d\Audio\MusicAudio.xml -> 配置游戏背景音乐.
/// Assets\ftproject\config\config.unity3d\Audio\SoundEffect.xml -> 配置游戏音效音乐.
/// Assets\ftproject\config\config.unity3d\Resources\ResourceInventoryFile\AudioClipResourcesInventory.xml -> 配置游戏音乐资源路径.
/// 策划案\音频文件分类 -> 描述GmAudioEnum枚举变量的具体含义.
/// </summary>
class GameAudioManage
{
    public enum GmAudioEnum
    {
        Wan01_M,
        Wan02_M,
        Wan03_M,
        Wan04_M,
        Wan05_M,
        Wan06_M,
        Wan07_M,
        Wan08_M,
        Wan09_M,
        Tiao01_M,
        Tiao02_M,
        Tiao03_M,
        Tiao04_M,
        Tiao05_M,
        Tiao06_M,
        Tiao07_M,
        Tiao08_M,
        Tiao09_M,
        Tong01_M,
        Tong02_M,
        Tong03_M,
        Tong04_M,
        Tong05_M,
        Tong06_M,
        Tong07_M,
        Tong08_M,
        Tong09_M,
        Dong_M,
        Nan_M,
        Xi_M,
        Bei_M,
        Zhong_M,
        Fa_M,
        Bai_M,
        Hua_M,
        Wan01_W,
        Wan02_W,
        Wan03_W,
        Wan04_W,
        Wan05_W,
        Wan06_W,
        Wan07_W,
        Wan08_W,
        Wan09_W,
        Tiao01_W,
        Tiao02_W,
        Tiao03_W,
        Tiao04_W,
        Tiao05_W,
        Tiao06_W,
        Tiao07_W,
        Tiao08_W,
        Tiao09_W,
        Tong01_W,
        Tong02_W,
        Tong03_W,
        Tong04_W,
        Tong05_W,
        Tong06_W,
        Tong07_W,
        Tong08_W,
        Tong09_W,
        Dong_W,
        Nan_W,
        Xi_W,
        Bei_W,
        Zhong_W,
        Fa_W,
        Bai_W,
        Hua_W,
        CaoZuoMsg01_M,
        CaoZuoMsg02_M,
        CaoZuoMsg03_M,
        CaoZuoMsg04_M,
        CaoZuoMsg05_M,
        CaoZuoMsg06_M,
        CaoZuoMsg07_M,
        CaoZuoMsg08_M,
        CaoZuoMsg09_M,
        CaoZuoMsg10_M,
        CaoZuoMsg11_M,
        CaoZuoMsg12_M,
        CaoZuoMsg13_M,
        CaoZuoMsg14_M,
        CaoZuoMsg15_M,
        CaoZuoMsg01_W,
        CaoZuoMsg02_W,
        CaoZuoMsg03_W,
        CaoZuoMsg04_W,
        CaoZuoMsg05_W,
        CaoZuoMsg06_W,
        CaoZuoMsg07_W,
        CaoZuoMsg08_W,
        CaoZuoMsg09_W,
        CaoZuoMsg10_W,
        CaoZuoMsg11_W,
        CaoZuoMsg12_W,
        CaoZuoMsg13_W,
        CaoZuoMsg14_W,
        CaoZuoMsg15_W,
        LiaoTianMsg01_M,
        LiaoTianMsg02_M,
        LiaoTianMsg03_M,
        LiaoTianMsg04_M,
        LiaoTianMsg05_M,
        LiaoTianMsg06_M,
        LiaoTianMsg07_M,
        LiaoTianMsg08_M,
        LiaoTianMsg01_W,
        LiaoTianMsg02_W,
        LiaoTianMsg03_W,
        LiaoTianMsg04_W,
        LiaoTianMsg05_W,
        LiaoTianMsg06_W,
        LiaoTianMsg07_W,
        LiaoTianMsg08_W,
        XiTong01,
        XiTong02,
        YouXiZhong01_M,
        YouXiZhong02_M,
        YouXiZhong01_W,
        YouXiZhong02_W,
        YouXiZhong01,
        YouXiZhong02,
        YouXiZhong03,
        YouXiZhong04,
        YouXiZhong05,
        YouXiZhong06,
        YouXiZhong07,
        YouXiZhong08,
        YouXiZhong09,
        YouXiZhong10,
        YouXiZhong11,
        YouXiZhong12,
        YouXiZhong13,
        YouXiZhong14,
        YouXiZhong15,
        YouXiZhong16,
        YouXiZhong17,
        YouXiZhong18,
        YouXiZhong19,
        YouXiZhong20,
    }
    /// <summary>
    /// 初始化GameAudioManage.
    /// </summary>
    public void InitGameAudioManage()
    {
        //激活为当前监听源
        MusicPlayer.activeMyListener = true;
    }
    /// <summary>
    /// 播放游戏声音.
    /// isStandalone == true -> 声音在同一时间只能播放一个.
    /// isStandalone == false -> 声音在同一时间可以播放多个.
    /// </summary>
    public void PlayGameAudio(GmAudioEnum audioEnum, bool isStandalone = false)
    {
        string audioName = audioEnum.ToString();
        try
        {
            switch (audioEnum)
            {
                case GmAudioEnum.YouXiZhong05:
                    {
                        //开始背景音乐播放
                        MusicPlayer.Stop(true);
                        if (!MainRoot._gGameSetData.IsOpenYinYueAudio)
                        {
                            return;
                        }
                        MusicPlayer.workMode = MusicPlayer.MusicPlayerWorkMode.Mode_Normal;
                        MusicPlayer.Play(audioName, true);
                    }
                    break;
                default:
                    {
                        if (!MainRoot._gGameSetData.IsOpenYinXiaoAudio)
                        {
                            return;
                        }

                        //开始游戏音效播放.
                        if (isStandalone)
                        {
                            SoundEffectPlayer.PlayStandalone(audioName);
                        }
                        else
                        {
                            SoundEffectPlayer.Play(audioName);
                        }
                    }
                    break;
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e.ToString());
        }

    }
}