using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 创建竖向UI列表面板通用控制方法.
/// </summary>
class VerticalListUICtrl : MonoBehaviour
{
    /// <summary>
    /// 列表元素的配置信息.
    /// ObjPrefab 列表元素的单元预制.
    /// ObjName 列表元素的名称.
    /// CountObj 列表元素数量.
    /// </summary>
    public struct ObjListConfigDt
    {
        public string ObjPrefab; //列表元素的单元预制.
        public string ObjName; //列表元素的名称.
        public int CountObj; //列表元素数量.
    }
    /// <summary>
    /// 列表元素的父级.
    /// ObjListPtr的高度必须是元素间距的整数倍.
    /// </summary>
    public Transform ObjListPtr;
    /// <summary>
    /// 列表元素的最大数量.
    /// </summary>
    public int ObjMax = 64;
    /// <summary>
    /// 列表元素的当前數量.
    /// </summary>
    int ObjCount = 50;
    /// <summary>
    /// 列表元素在可见框里显示的最小个数.
    /// </summary>
    public int CountShowObj = 4;
    /// <summary>
    /// 两个列表元素的纵向间隔距离.
    /// </summary>
    public float OffsetPosY = 128f;
    List<Transform> ObjTrList = new List<Transform>();

    /// <summary>
    /// 创建纵向元素列表.
    /// testNum != -1 -> 测试模式.
    /// testNum == -1 -> 正式游戏模式.
    /// </summary>
    public void CreateObjList(ObjListConfigDt configDt)
    {
        ObjCount = configDt.CountObj;
        if (ObjCount <= 0)
        {
            Debug.Log("Unity: MingPianCount was wrong!");
            return;
        }

        if (ObjCount > ObjMax)
        {
            ObjCount = ObjMax;
        }

        Transform[] objTrAy = new Transform[ObjCount];
        GameObject obj = null;
        for (int i = 0; i < ObjCount; i++)
        {
            //创建列表元素.
            obj = (GameObject)Instantiate(Resources.Load(configDt.ObjPrefab), ObjListPtr, false);
            ObjTrList.Add(obj.transform);
            objTrAy[i] = obj.transform;
            obj.name = configDt.ObjName + i;
        }
        SetObjListPosInfo(objTrAy);
        
        DisplayListObjUiDt(objTrAy); //必须在本函数中设置列表元素的数据信息,用来显示!!!
    }

    /// <summary>
    /// 设置竖向列表元素的位置.
    /// </summary>
    void SetObjListPosInfo(Transform[] objTrArray)
    {
        float objListHeight = 0f;
        int objListLength = ObjCount > CountShowObj ? ObjCount : CountShowObj;
        objListHeight = OffsetPosY * objListLength;
        Vector2 objListPtrSize = ((RectTransform)ObjListPtr).sizeDelta;
        objListPtrSize.y = objListHeight;
        ((RectTransform)ObjListPtr).sizeDelta = objListPtrSize;

        //objListPtrSize = ((RectTransform)ObjListPtr.parent).sizeDelta;
        //objListPtrSize.y = OffsetPosY * CountShowObj;
        //((RectTransform)ObjListPtr.parent).sizeDelta = objListPtrSize;
        ((RectTransform)ObjListPtr).anchoredPosition = new Vector3(0f, -(objListHeight * 0.5f), 0f);

        Vector3 offsetPos = Vector3.zero;
        Vector3 startPos = new Vector3(0f, (objListHeight * 0.5f) - (((RectTransform)objTrArray[0]).sizeDelta.y * 0.5f), 0f);
        for (int i = 0; i < ObjCount; i++)
        {
            offsetPos.y = -i * OffsetPosY;
            objTrArray[i].localPosition = startPos + offsetPos;
        }
    }

    /// <summary>
    /// 删除竖向列表的某个元素.
    /// </summary>
    public void RemoveObjTrFromPObjTrList(Transform tr)
    {
        if (!ObjTrList.Contains(tr))
        {
            return;
        }
        ObjTrList.Remove(tr);
        Destroy(tr.gameObject);

        Transform[] objTrArray = ObjTrList.ToArray();
        ObjCount = objTrArray.Length;
        if (ObjCount <= 0)
        {
            return;
        }
        float objListHeight = 0f;
        int objListLength = objTrArray.Length > CountShowObj ? ObjCount : CountShowObj;
        objListHeight = OffsetPosY * objListLength;
        Vector2 objListPtrSize = ((RectTransform)ObjListPtr).sizeDelta;
        objListPtrSize.y = objListHeight;
        ((RectTransform)ObjListPtr).sizeDelta = objListPtrSize;

        //objListPtrSize = ((RectTransform)ObjListPtr.parent).sizeDelta;
        //objListPtrSize.y = OffsetPosY * CountShowObj;
        //((RectTransform)ObjListPtr.parent).sizeDelta = objListPtrSize;
        ObjListPtr.localPosition = new Vector3(0f, -(objListHeight * 0.5f), 0f);

        Vector3 offsetPos = Vector3.zero;
        Vector3 startPos = new Vector3(0f, (objListHeight * 0.5f) - (((RectTransform)objTrArray[0]).sizeDelta.y * 0.5f), 0f);
        for (int i = 0; i < ObjCount; i++)
        {
            offsetPos.y = -i * OffsetPosY;
            objTrArray[i].localPosition = startPos + offsetPos;
        }
    }

    /// <summary>
    /// 显示列表中元素的Ui数据.
    /// </summary>
    void DisplayListObjUiDt(Transform[] objTrAy)
    {
        ShowListUiDtCtrl showListUiDtCom = GetComponent<ShowListUiDtCtrl>();
        showListUiDtCom.ShowListUiDt(objTrAy);
    }
}