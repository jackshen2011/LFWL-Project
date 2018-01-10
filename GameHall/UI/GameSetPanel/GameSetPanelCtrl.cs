using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class GameSetPanelCtrl : MonoBehaviour
{
    public Slider YinYueSlider;
    public GameObject[] YinYueBtObj;
    public Slider YinXiaoSlider;
    public GameObject[] YinXiaoBtObj;
    EnsureDlg mEnsureDlg;
    public void InitGameSetPanel()
    {
        mEnsureDlg = GetComponent<EnsureDlg>();
        mEnsureDlg.Initial(EnsureDlg.EnsureKind.GameSetPanelDlg);
        YinYueSlider.value = MainRoot._gGameSetData.IsOpenYinYueAudio == true ? 0 : 1;
        YinXiaoSlider.value = MainRoot._gGameSetData.IsOpenYinXiaoAudio == true ? 0 : 1;
        OnClickYinYueSlider();
        OnClickYinXiaoSlider();
    }
    /// <summary>
    /// 当音乐slider被点击.
    /// </summary>
    public void OnClickYinYueSlider()
    {
        bool isOpen = YinYueSlider.value == 0 ? true : false;
        MainRoot._gGameSetData.SetIsOpenYinYueAudio(isOpen);
        for (int i = 0; i < YinYueBtObj.Length; i++)
        {
            YinYueBtObj[i].SetActive(i == 0 ? isOpen : !isOpen);
        }
    }
    /// <summary>
    /// 当音效slider被点击.
    /// </summary>
    public void OnClickYinXiaoSlider()
    {
        bool isOpen = YinXiaoSlider.value == 0 ? true : false;
        MainRoot._gGameSetData.SetIsOpenYinXiaoAudio(isOpen);
        for (int i = 0; i < YinYueBtObj.Length; i++)
        {
            YinXiaoBtObj[i].SetActive(i == 0 ? isOpen : !isOpen);
        }
    }

    public void OnDestoryDlg()
    {
        mEnsureDlg.DestroyThis();
    }
}