using UnityEngine;
using System.Collections;

class TaoTaiSaiPlayerInfo : MonoBehaviour
{
    public TextBase NameText;
    /// <summary>
    /// 显示淘汰赛玩家信息.
    /// </summary>
    public void ShowPlayerInfo(object[] args)
    {
        string playerName = (string)args[0];
        NameText.text = playerName.Length <= 8 ? playerName : (playerName.Remove(8) + "..."); ;
    }
}