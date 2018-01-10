using UnityEngine;
using System.Collections;
using RoomCardNet;

class ShouJiHaoYanZhengDlg : MonoBehaviour
{
    /// <summary>
    /// 倒计时文本.
    /// </summary>
    public TextBase DaoJiShiText;
    /// <summary>
    /// 获取验证码按键图集.
    /// HuoQuYZMBtSp[x]: 0 灰色, 1 激活.
    /// </summary>
    public Sprite[] HuoQuYZMBtSp;
    ImageBase HuoQuYZMBtImg;
    EnsureDlg ShouJiHaoDlg;
    float TimeDaoJiShiLast; //获取验证码倒计时.
    int DaoJiShiCount;
    float TimeYanZhengMaVal; //点击获取验证码的事件.
    void Update()
    {
        if (DaoJiShiText.gameObject.activeInHierarchy && Time.time - TimeDaoJiShiLast >= 1f)
        {
            TimeDaoJiShiLast = Time.time;
            DaoJiShiCount--;
            DaoJiShiText.text = DaoJiShiCount.ToString();
            if (DaoJiShiCount <= 0)
            {
                DaoJiShiText.gameObject.SetActive(false);
                //ShouJiHaoDlg.btn1.SetActive(true);
                HuoQuYZMBtImg.sprite = HuoQuYZMBtSp[1];
                HuoQuYZMBtImg.raycastTarget = true;
            }
        }
    }

    public void Init()
    {
        ShouJiHaoDlg = GetComponent<EnsureDlg>();
        HuoQuYZMBtImg = ShouJiHaoDlg.btn1.GetComponent<ImageBase>();
        HuoQuYZMBtImg.sprite = HuoQuYZMBtSp[1];
        HuoQuYZMBtImg.raycastTarget = true;
        DaoJiShiText.gameObject.SetActive(false);
    }

    /// <summary>
    /// 点击获取验证码按键.
    /// </summary>
    public void OnClickGetYanZhengMaBt()
    {
        string shouJiHaoVal = ShouJiHaoDlg.p_input1.text;
        //Debug.Log("Unity: OnClickGetYanZhengMaBt -> input shouJiHao is " + shouJiHaoVal);
        bool isReinputShouJiHao = false; //是否重新输入手机号.
        long telNum = 0;
        if (shouJiHaoVal == "" || shouJiHaoVal.Length != 11 || !IsNumberic(shouJiHaoVal, out telNum))
        {
            isReinputShouJiHao = true;
        }

        if (isReinputShouJiHao)
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnReinputShouJiHao();
            return;
        }
        //此处向服务端发送手机号信息.
        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerSendPhoneMessage(shouJiHaoVal);
        }
        //ShouJiHaoDlg.btn1.SetActive(false);
        HuoQuYZMBtImg.sprite = HuoQuYZMBtSp[0];
        HuoQuYZMBtImg.raycastTarget = false;

        DaoJiShiText.gameObject.SetActive(true);
        TimeDaoJiShiLast = Time.time;
        TimeYanZhengMaVal = Time.time;
        DaoJiShiCount = 60;
        DaoJiShiText.text = DaoJiShiCount.ToString();
    }

    /// <summary>
    /// 判断是否为整数字符串
    /// </summary>
    bool IsNumberic(string message, out long result)
    {
        //是的话则将其转换为数字并将其设为out类型的输出值、返回true, 否则为false
        result = -1;   //result 定义为out 用来输出值
        try
        {
            //当数字字符串的为是少于4时，以下三种都可以转换，任选一种
            //如果位数超过4的话，请选用Convert.ToInt32() 和int.Parse()

            //result = int.Parse(message);
            //result = Convert.ToInt16(message);
            result = System.Convert.ToInt64(message);
            return true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// 点击确认按键.
    /// </summary>
    public void OnClickQueRenBt()
    {
        if (Time.time - TimeYanZhengMaVal >= 300)
        {
            //Debug.Log("Unity: OnClickQueRenBt -> time over! please click Get yanZhengMaBt!");
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnReclickGetYanZhengMa();
            return;
        }

        string yanZhengMaVal = ShouJiHaoDlg.p_input2.text;
        if (yanZhengMaVal == "")
        {
            MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnYanZhengMaCuoWuMsg();
            return;
        }
        //Debug.Log("Unity: OnClickQueRenBt -> send yanZhengMa to server port! yanZhengMa is " + yanZhengMaVal);
        //此处向服务端发送验证码信息.
        if (RoomCardNetClientModule.netModule.wanMainClientPlayer != null)
        {
            RoomCardNetClientModule.netModule.wanMainClientPlayer.Net_CallPlayerCheckCode(yanZhengMaVal);
        }
    }

    /// <summary>
    /// 收到服务端确认按键的返回消息.
    /// </summary>
    public void OnReceivedQueRenBtMsg(int arg)
    {
        //arg: 1-无码 2-过期 3-不正确 0-成功
        switch (arg)
        {
            case 1: //无验证码.
            case 3: //验证码错误.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnYanZhengMaCuoWuMsg();
                    break;
                }
            case 2: //验证码过期.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnReclickGetYanZhengMa();
                    break;
                }
            case 0: //验证码正确.
                {
                    MainRoot._gUIModule.pUnModalUIControl.pMainUIScript.SpawnYanZhengMaZhengQueMsg();
                    ShouJiHaoDlg.DestroyThis();
                    break;
                }
            default:
                {
                    Debug.Log("Unity: OnReceivedQueRenBtMsg -> arg was wrong! arg == " + arg);
                    break;
                }
        }
    }
}