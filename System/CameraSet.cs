using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CameraSet : MonoBehaviourIgnoreGui
{
    public enum CameraType
    {
        CameraSet_Main,
        CameraSet_HandSelf,
        CameraSet_Canvas,
    }

    public CameraType Type = CameraType.CameraSet_Main;
    public Camera SelfCamera;
    public CanvasScaler SelfCanvasScaler;

    public GameObject GameStartLoadingUI;

    // Use this for initialization
    void Start ()
    {
        if(SystemSetManage.ResolutionIndex == -1)
        {
            SystemSetManage.delegateDelegateResolutionSetFn = ResolutionSet;
            return;
        }
        if (Type == CameraType.CameraSet_HandSelf && SelfCamera != null)
        {
            if (SystemSetManage.ResolutionIndex != -1)
                SelfCamera.orthographicSize = SystemSetManage.initializeSystemSetManage.HandCardCameraValueList[SystemSetManage.ResolutionIndex];
        }
        else if(Type == CameraType.CameraSet_Main && SelfCamera != null)
        {
            if (SystemSetManage.ResolutionIndex != -1)
                SelfCamera.fieldOfView = SystemSetManage.initializeSystemSetManage.MainCameraValueList[SystemSetManage.ResolutionIndex];
        }
        else if(Type == CameraType.CameraSet_Canvas && SelfCanvasScaler != null)
        {
            if (SystemSetManage.ResolutionIndex != -1)
                SelfCanvasScaler.referenceResolution = new Vector2(Screen.width, SystemSetManage.initializeSystemSetManage.CanvasValueList[SystemSetManage.ResolutionIndex]);
        }
    }

    public void ResolutionSet()
    {
        if (Type == CameraType.CameraSet_HandSelf && SelfCamera != null)
        {
            if (SystemSetManage.ResolutionIndex != -1)
                SelfCamera.orthographicSize = SystemSetManage.initializeSystemSetManage.HandCardCameraValueList[SystemSetManage.ResolutionIndex];
        }
        else if (Type == CameraType.CameraSet_Main && SelfCamera != null)
        {
            if (SystemSetManage.ResolutionIndex != -1)
                SelfCamera.fieldOfView = SystemSetManage.initializeSystemSetManage.MainCameraValueList[SystemSetManage.ResolutionIndex];
        }
        else if (Type == CameraType.CameraSet_Canvas && SelfCanvasScaler != null)
        {
            if (SystemSetManage.ResolutionIndex != -1)
                SelfCanvasScaler.referenceResolution = new Vector2(Screen.width, SystemSetManage.initializeSystemSetManage.CanvasValueList[SystemSetManage.ResolutionIndex]);
            if(GameStartLoadingUI != null && GameStartLoadingUI.activeSelf == false)
            {
                GameStartLoadingUI.SetActive(true);
            }
        }
    }
}
