using UnityEngine;
using System;
using System.Collections;
using System.Threading;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System;

class VersionUpdate_Android : MonoBehaviourIgnoreGui
{
    public class RequestState
    {
        public const int BUFFER_SIZE = 1024;
        public byte[] BufferRead;
        public HttpWebRequest request;
        public HttpWebResponse response;
        public Stream responseStream;
    }

    public enum DownloadState
    {
        DOWNLOADING,
        FINISHED,
        FAILED
    }

    public delegate void ProgressCallback(long curr, long length, float rate, DownloadState state);
    public ProgressCallback progressCallback;

    string url = "";
    string installPath = "";
    string apkName = "";
    string errorMsg = "";

    private FileStream fileStream = null;
    private long length = 1;
    private long curr = 0;
    private long last = 0;
    private const float UpdateTime = 0.5f;
    private float rate = 0;
    private DownloadState downState = DownloadState.DOWNLOADING;

    public GuiPlaneAnimationProgressBar progressBar;

    public void DownloadApkAsync(string url)
    {
        //string md5, string path, string name;
        string path = Application.persistentDataPath; //"/storage/emulated/0";//
        string name = "RoomCard.apk";
        this.url = url;
        this.installPath = path;
        this.apkName = name;
        this.errorMsg = "";
        downState = DownloadState.DOWNLOADING;
        progressCallback = ProgressCallback_SetProgressBar;

        //File.Delete(installPath + "//" + apkName);
        Debug.Log("Application.dataPath = " + Application.dataPath);
        Debug.Log("Application.persistentDataPath = " + Application.persistentDataPath);
        Debug.Log("Application.temporaryCachePath = " + Application.temporaryCachePath);

        DownloadApkAsync();
    }

    private void ProgressCallback_SetProgressBar(long curr, long length, float rate, DownloadState state)
    {
        float value = (float)curr / length;
        progressBar.SetProgressBar(value, true);
    }

    private void DownloadApkAsync()
    {
        if (string.IsNullOrEmpty(url)) return;
        if (string.IsNullOrEmpty(installPath)) return;
        if (string.IsNullOrEmpty(apkName)) return;

        string fullpath = installPath + "/" + apkName;

        //try
        //{
        //    FileInfo fi = new FileInfo(Application.persistentDataPath + "/Test15324.txt");

        //    //判断文件是否存在
        //    byte[] bytes = new byte[10];
        //    for(int i=0; i<10; i++)
        //    {
        //        bytes[i] = 48;
        //    }
        //    if (!fi.Exists)
        //    {
        //        FileStream fs = fi.OpenWrite();

        //        fs.Write(bytes, 0, bytes.Length);

        //        fs.Flush();

        //        fs.Close();

        //        fs.Dispose();

        //        Debug.Log("CopyTxt Success!" + "\n" + "Path: ======> " + Application.persistentDataPath + "/Test15324.txt");

        //    }
        //    Debug.Log("$$$$$$$$$$$$$$$$");
        //}
        //catch (Exception e)
        //{
        //    Debug.Log("@@@@@@@@@@@@@@#####################" + e.Message);
        //}

        IAsyncResult result = null;
        try
        {
            FileInfo fi = new FileInfo(fullpath);
            if(fi.Exists)
            {
                File.Delete(fullpath);
            }

            fileStream = fi.OpenWrite();

            //fileStream = new FileStream(fullpath, FileMode.CreateNew, FileAccess.Write);

            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            request.Method = "GET";

            RequestState requestState = new RequestState();
            requestState.BufferRead = new byte[RequestState.BUFFER_SIZE];
            requestState.request = request;

            curr = 0;
            length = 1;
            rate = 0.0f;
            downState = DownloadState.DOWNLOADING;
            result = (IAsyncResult)request.BeginGetResponse(new AsyncCallback(ResponeCallback), requestState);
        }
        catch (Exception e)
        {
            errorMsg = "Begin Create Exception!";
            errorMsg += string.Format("Message:{0}", e.Message);
            StopDownload(result);
            downState = DownloadState.FAILED;
        }

        StartCoroutine(updateProgress());
    }

    IEnumerator updateProgress()
    {
        while (curr <= length)
        {
            yield return new WaitForSeconds(UpdateTime);

            rate = (curr - last) / UpdateTime;
            last = curr;

            if (downState == DownloadState.FAILED)
            {
                Debug.LogError(errorMsg);
                if (fileStream != null)
                    fileStream.Close();
                if (progressCallback != null)
                    progressCallback(curr, length, rate, DownloadState.FAILED);
                break;
            }

            if (progressCallback != null)
                progressCallback(curr, length, rate, DownloadState.DOWNLOADING);

            if (downState == DownloadState.FINISHED)
            {
                if (progressCallback != null)
                    progressCallback(curr, length, rate, DownloadState.FINISHED);
                break;
            }
        }
    }

    void StopDownload(IAsyncResult result)
    {
        if (result == null) return;
        RequestState requestState = (RequestState)result.AsyncState;
        requestState.request.Abort();
    }

    void ResponeCallback(IAsyncResult result)
    {
        try
        {
            if (downState != DownloadState.FAILED)
            {
                RequestState requestState = (RequestState)result.AsyncState;
                HttpWebRequest request = requestState.request;
                requestState.response = (HttpWebResponse)request.EndGetResponse(result);

                Stream responseStream = requestState.response.GetResponseStream();
                requestState.responseStream = responseStream;

                length = requestState.response.ContentLength;

                IAsyncResult readResult = responseStream.BeginRead(requestState.BufferRead, 0, RequestState.BUFFER_SIZE, new AsyncCallback(ReadCallback), requestState);
                return;
            }
        }
        catch (Exception e)
        {
            string msg = "ResponseCallback exception!\n";
            msg += string.Format("Message:{0}", e.Message);
            StopDownload(result);
            errorMsg = msg;
            downState = DownloadState.FAILED;
        }
    }

    void ReadCallback(IAsyncResult result)
    {
        try
        {
            if (downState != DownloadState.FAILED)
            {
                RequestState requestState = (RequestState)result.AsyncState;
                Stream responseStream = requestState.responseStream;
                int read = responseStream.EndRead(result);
                if (read > 0)
                {
                    fileStream.Write(requestState.BufferRead, 0, read);
                    fileStream.Flush();
                    curr += read;

                    IAsyncResult readResult = responseStream.BeginRead(requestState.BufferRead, 0, RequestState.BUFFER_SIZE, new AsyncCallback(ReadCallback), requestState);
                    return;
                }
                else
                {
                    Debug.Log("download end");
                    responseStream.Close();
                    fileStream.Close();
                    fileStream.Dispose();

                    downState = DownloadState.FINISHED;

                    this.Invoke("InstallApk", 1.0f);
                    //InstallApk();
                }
            }
        }
        catch (Exception e)
        {
            string msg = "ReadCallBack exception!";
            msg += string.Format("Message:{0}", e.Message);
            Debug.Log(msg);
            StopDownload(result);
            errorMsg = msg;
            downState = DownloadState.FAILED;
        }
    }


    public void InstallApk()
    {
        Debug.Log("download end @@@@@@@@@@@@@@@@");
        //string str = installPath.Replace("/", "$^$");
        //str = str.Replace("\" ,  "%^%");
#if UNITY_ANDROID && !UNITY_EDITOR

        using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                jo.Call("InstallApk", installPath, apkName);
            }

        }
#endif
    }
}
