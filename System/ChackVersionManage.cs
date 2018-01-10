using UnityEngine;
using System.Collections;



public class ChackVersionManage : MonoBehaviourIgnoreGui
{
    public static void DownloadApk(string sTempVer)
    {
#if !UNITY_EDITOR && UNITY_ANDROID

        GameObject obj = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/VersionUpdate_Android"));
        if(obj != null)
        {
            VersionUpdate_Android dlg = obj.GetComponent<VersionUpdate_Android>();
            dlg.DownloadApkAsync(sTempVer);
        }
				
#elif UNITY_IPHONE
        Application.OpenURL(sTempVer);
#endif //UNITY_ANDROID
    }
}
