using UnityEngine;
using System.Collections;

class UGuiTweenScale : MonoBehaviour
{
    public AnimationCurve AniCurve = new AnimationCurve();
    public Vector3 From;
    public Vector3 To;
    public float Duration = 1f;
    float TimeVal;
    bool IsInitTrAni;
    void Start()
    {
        transform.localScale = From;
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
        transform.localScale = From;
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
            transform.localScale = To;
            ResetTransformAnimation();
            return;
        }

        if (AniCurve.length != 0) {
            vecVal.x = From.x + (To.x - From.x) * durationVal;
            vecVal.y = From.y + (To.y - From.y) * durationVal;
            vecVal.z = From.z + (To.z - From.z) * durationVal;
        }
        transform.localScale = vecVal;
    }
}