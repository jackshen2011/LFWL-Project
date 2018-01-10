using UnityEngine;
using System.Collections;

public class ScrollTextCtrl : MonoBehaviour
{
	// Use this for initialization
	void Start()
    {
        UpdateTextRect();
    }
    /// <summary>
    /// 更新文本框父级的大小和坐标.
    /// </summary>
    void UpdateTextRect()
    {
        RectTransform rectTr = (RectTransform)transform.parent;
        TextBase textBaseCom = GetComponent<TextBase>();
        float fontSize = textBaseCom.fontSize;
        int textLen = textBaseCom.text.Length;
        string[] splitText = textBaseCom.text.Split('\n');
        Vector2 rectSize = rectTr.sizeDelta;
        int countW = (int)(rectSize.x / fontSize);
        int countH = (splitText.Length - 1) + (textLen / countW);
        rectSize.y = 1.1f * countH * fontSize;
        rectTr.sizeDelta = rectSize;
        Vector3 locPos = rectTr.localPosition;
        locPos.y = -0.5f * rectSize.y;
        rectTr.localPosition = locPos;
    }
}