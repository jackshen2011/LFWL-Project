using UnityEngine;
using System.Collections;

/// <summary>
/// 调完通风
/// </summary>
public enum MAJIANG 
{
    NONE=-1,
    MJ01 =1, MJ02, MJ03, MJ04, MJ05, MJ06, MJ07, MJ08, MJ09,
    MJ11=16+1, MJ12, MJ13, MJ14, MJ15, MJ16, MJ17, MJ18, MJ19,
    MJ21=16*2+1, MJ22, MJ23, MJ24, MJ25, MJ26, MJ27, MJ28, MJ29,
    DONG=16*3+1, NAN, XI, BEI, ZHONG, FA, BAI
}
class MaJiang : MonoBehaviourIgnoreGui
{
    public Renderer faceModelRenderer;
    MAJIANG whoAmI;
    MJKIND kind;
    int arrayIndex=-1;//手牌在数组里的索引
    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
    public MAJIANG WhoAmI
    {
        get
        {
            return whoAmI;
        }
    }
    public MJKIND Kind
    {
        get
        {
            return kind;
        }
    }
    public int ArrayIndex
    {
        get
        {
            return arrayIndex;
        }
        set
        {
            arrayIndex = value;
        }
    }
    public void SetStates(MAJIANG mj, MJKIND type)
    {
        whoAmI = mj;
        kind = type;
    }

    /// <summary>
    /// 更换麻将材质求
    /// </summary>
    /// <param name="m">材质</param>
    public void ChangeMJMaterial(Material m)
    {
        if (faceModelRenderer)
        {
            faceModelRenderer.sharedMaterial = m;
        }
    }
}
