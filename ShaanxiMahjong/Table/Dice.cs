using UnityEngine;
using System.Collections;

class Dice : MonoBehaviourIgnoreGui
{
    int nStage;
    bool bIsRolling=false;
    float fRollTime=0f;
    private float m_CurrentPlayTime = 0.0f;
    private Vector3 m_DestinationPoint;
    public delegate void eventOver();
    public eventOver diceOver;
    public float currentPlayTime
    {
        get { return m_CurrentPlayTime; }
        set
        {
            m_CurrentPlayTime = value;
        }
    }
    //初始化一个骰子
    public void InitDice () {
        int n=1;
        n = Mathf.CeilToInt(Random.Range(0f,3f));
        if (Random.Range(0f, 2f) < 1)
        {
            transform.localEulerAngles = new Vector3(0, Random.value * 360.0f, n*90);
        }
        else {
            transform.localEulerAngles = new Vector3(n * 90, Random.value * 360.0f, 0);
        }        
    }
	
	// Update is called once per frame
	void Update () {
        if (1 <= nStage)
        {
            currentPlayTime += Time.deltaTime;
            transform.localEulerAngles = new Vector3(Random.value * 360.0f, Random.value * 360.0f, Random.value * 360.0f);
            if (!bIsRolling)
            {
                fRollTime = Random.Range(0.7f, 1.5f);
            }
            bIsRolling = true;
            if (currentPlayTime> fRollTime)
            {
                bIsRolling = false;
                currentPlayTime = 0;
                nStage = 0;
                transform.localEulerAngles = m_DestinationPoint;
                if (diceOver != null)
                {
                    diceOver();
                }
            }
        }
    }
    /// <summary>
    /// 掷出指定点数
    /// </summary>
    /// <param name="n"></param>
    public void CircleDiceTypicalNum(int n)
    {
        if (nStage == 0)
        {
            nStage = 1;
        }
        else
        {
            return;
        }
        switch (n)
        {
            case 1:
                m_DestinationPoint = new Vector3(0, Random.value * 360.0f, 0);
                break;
            case 2:
                m_DestinationPoint = new Vector3(0, Random.value * 360.0f, 90);
                break;
            case 3:
                m_DestinationPoint = new Vector3(270, Random.value * 360.0f, 0);
                break;
            case 4:
                m_DestinationPoint = new Vector3(90, Random.value * 360.0f, 0);
                break;
            case 5:
                m_DestinationPoint = new Vector3(0, Random.value * 360.0f, 270);
                break;
            case 6:       
                m_DestinationPoint = new Vector3(180, Random.value * 360.0f, 0);
                break;
        }
        GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-5f,5f), 0, Random.Range(-5f, 5f)));
        GetComponent<Rigidbody>().AddTorque(new Vector3(Random.value*360f, 0, Random.value * 360f));
    }
}
