using UnityEngine;
using System.Collections;

class ListUiDtCtrl : MonoBehaviour
{
    /// <summary>
    /// 文本组件列表.
    /// </summary>
    public TextBase[] UiText;
    /// <summary>
    /// Img组件列表.
    /// </summary>
    public ImageBase[] UiImg;
    /// <summary>
    /// 用来显示列表单元的数据信息.
    /// </summary>
    public virtual void ShowListUiDt(object[] args)
    {
    }
}