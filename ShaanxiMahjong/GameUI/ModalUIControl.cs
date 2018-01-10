using UnityEngine;
using System.Collections;
using MoleMole;

/// <summary>
/// 模态窗口基类
/// </summary>
class ModalUIBase : MonoBehaviourIgnoreGui
{
    public int nModalUIIndex = -1;
}

/// <summary>
/// 模态窗口控制类
/// </summary>
class ModalUIControl : MonoBehaviour {

    // Use this for initialization

    public ModalUIBase pCurModalUI;

    public void ShowCurModalUI(ModalUIBase pm)
    {
        if (pCurModalUI == null)
        {
            pCurModalUI = pm;
        }
    }
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
