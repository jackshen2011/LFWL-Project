using UnityEngine;
class GameSetData
{
    /// <summary>
    /// 游戏音乐开关.
    /// </summary>
    public bool IsOpenYinYueAudio = true;

    /// <summary>
    /// 游戏音效开关.
    /// </summary>
    public bool IsOpenYinXiaoAudio = true;
    /// <summary>
    /// 初始化游戏设置信息.
    /// </summary>
    public void InitGameSetData()
    {
        IsOpenYinYueAudio = PlayerPrefs.GetInt("OpenYinYueAudio") == 0 ? true : false;
        IsOpenYinXiaoAudio = PlayerPrefs.GetInt("OpenYinXiaoAudio") == 0 ? true : false;
    }
    public void SetIsOpenYinYueAudio(bool isOpen)
    {
        IsOpenYinYueAudio = isOpen;
        PlayerPrefs.SetInt("OpenYinYueAudio", isOpen == true ? 0 : 1);
    }
    public void SetIsOpenYinXiaoAudio(bool isOpen)
    {
        IsOpenYinXiaoAudio = isOpen;
        PlayerPrefs.SetInt("OpenYinXiaoAudio", isOpen == true ? 0 : 1);
    }
}