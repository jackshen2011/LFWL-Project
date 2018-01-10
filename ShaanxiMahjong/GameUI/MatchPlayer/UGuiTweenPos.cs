using UnityEngine;
using System.Collections;

class UGuiTweenPos : MonoBehaviour
{
    public AnimationCurve AniCurve = new AnimationCurve();
    public Vector3 From;
    public Vector3 To;
    public float Duration = 1f;
    /// <summary>
    /// 回调函数->当运动事件结束后执行回调.
    /// </summary>
    public string MsgSend;
    /// <summary>
    /// 回调函数的参数列表.
    /// </summary>
    public int[] MsgArgs = new int[] {-1, -1, -1, -1, -1};
    float TimeVal;
    bool IsInitTrAni;
    void Start()
    {
        transform.localPosition = From;
    }
    void Update()
    {
        TransformAnimation();
    }
    public void InitTransformAnimation()
    {
        if (IsInitTrAni) {
            return;
        }
        IsInitTrAni = true;
        TimeVal = 0f;
        transform.localPosition = From;
    }
    void ResetTransformAnimation()
    {
        IsInitTrAni = false;
    }
    void TransformAnimation()
    {
        if (!IsInitTrAni) {
            return;
        }

        Vector3 vecVal = Vector3.zero;
        TimeVal += Time.deltaTime;
        float durationVal = TimeVal / Duration;
        durationVal = durationVal > 1f ? 1f : durationVal;
        if (durationVal >= 1f) {
            transform.localPosition = To;
            ResetTransformAnimation();
            if (MsgSend != null && MsgSend != "")
            {
                gameObject.SendMessage(MsgSend, MsgArgs);
            }
            return;
        }

        if (AniCurve.length != 0) {
            vecVal.x = From.x + (To.x - From.x) * durationVal;
            vecVal.y = From.y + (To.y - From.y) * durationVal;
            vecVal.z = From.z + (To.z - From.z) * durationVal;
        }
        transform.localPosition = vecVal;
    }
    /// <summary>
    /// 初始化消息发送信息.
    /// </summary>
    public void InitMsgSendInfo(string msgSend, int[] args)
    {
        MsgSend = msgSend;
        MsgArgs = args;
    }
}