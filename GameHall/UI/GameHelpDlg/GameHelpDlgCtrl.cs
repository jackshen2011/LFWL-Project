using UnityEngine;
using System.Collections;

public class GameHelpDlgCtrl : MonoBehaviour
{
    /// <summary>
    /// 帮助01的内容资源.
    /// </summary>
    public GameObject[] HelpNeiRong01;
    void Start()
    {
        for (int i = 0; i < HelpNeiRong01.Length; i++)
        {
            HelpNeiRong01[i].SetActive(i == 0 ? true : false);
        }
    }
    public void OnClickHelpToggle(int indexVal)
    {
        for (int i = 0; i < HelpNeiRong01.Length; i++)
        {
            HelpNeiRong01[i].SetActive(i == indexVal ? true : false);
        }
    }
}