using System.Collections.Generic;
public struct PengGangChi
{
    /// <summary>
    /// 类型 0碰 1直杠 2续杠 3暗杠
    /// </summary>
    public int kind;
    public int index;//碰的是其他哪一家的如果有
    public List<int> mj;
    /// <summary>
    /// 构造一组碰杠牌
    /// </summary>
    /// <param name="mj">牌型</param>
    /// <param name="kind">碰杠种类</param>
    /// <param name="index">碰杠的谁</param>
    public PengGangChi(List<int> mj,int kind,int index)
    {
        this.mj = mj;
        this.kind = kind;
        this.index=index;
    }
    /// <summary>
    /// 检测的牌是否可以被杠
    /// </summary>
    /// <param name="testMJ"></param>
    /// <returns>杠的位置 -1表示不可被杠</returns>
    public int checkGang(int testMJ)
    {
        int first=mj[0];
        if (kind != 0)
        {
            return -1;
        }
        if (first != testMJ)
        {
            return -1;
        }
        return index;
    }
}
/// <summary>
/// 陕西麻将玩法之普通玩法
/// 玩家自己的手牌数据
/// </summary>
public class NormalPlayMethod
{
    List<int> userAllMaJiang;
    List<List<PengGangChi>> pengGangChi = new List<List<PengGangChi>>();
    int userCurrentTakeMaJiang=-1;/// <remark>当前接的牌</remark>

    public int longBlockNum;//打法中牌墙摆的长度，长的
    public int shortBlockNum;//打法中牌墙摆的长度，短的
    public NormalPlayMethod()
    {
        for (int i = 0; i < 4; i++)
        {
            pengGangChi.Add(new List<PengGangChi>());
        }
    }
    /// <summary>
    /// 设置打法选项 如是否七对可胡
    /// </summary>
    /// <param name="flag"></param>
    public void SetPlayMethod(int[]flag)
    {
        longBlockNum = 14;
        shortBlockNum = 13;
        if (flag != null)
        {
            if (flag.Length>=14)
            {
                bool isFeng = false;
                bool isLaiZi = false;
                if ((flag[5] & 1) > 0)
                {
                    isFeng = true;
                }
                if ((flag[5] & (1<<1)) > 0)
                {
                    isLaiZi = true;
                }
                if (isFeng)
                {
                    longBlockNum = 17;
                    shortBlockNum = 17;
                }
                else
                {
                    if (isLaiZi)
                    {
                        longBlockNum = 14;
                        shortBlockNum = 14;
                    }
                }
            }
        }
    } 
    /// <summary>
    /// 初始化牌型
    /// </summary>
    /// <param name="maJiangs"></param>
    public void InitNormalPlayMethod(int[] maJiangs)
    {
        if (maJiangs == null)
        {
            return;
        }
        int endFlag= maJiangs.Length;
        for (int i = 0; i < maJiangs.Length; i++)
        {
            if (maJiangs[i] == 0)
            {
                endFlag = i;
                break;
            }
        }
        int[] mj=new int[endFlag];
        System.Array.Copy(maJiangs, mj,mj.Length);
        userAllMaJiang = new List<int>(mj);
    }
    public List<int> UserAllMaJiang
    {
        get
        {
            return userAllMaJiang;
        }
    }
    /// <summary>
    /// 所有的碰杠吃
    /// </summary>
    public List<List<PengGangChi>> AllPengGangChi
    {
        get
        {
            return pengGangChi;
        }
    }
    public int UserCurrentTakeMaJiang
    {
        get
        {
            return userCurrentTakeMaJiang;
        }
        set
        {
            userCurrentTakeMaJiang = value;
        }
    }
    /// <summary>
    /// 是否有自己杠
    /// </summary>
    /// <returns></returns>
    public List<MAJIANG> PossibleSelfGang()
    {
        List<MAJIANG> mj=new List<MAJIANG>();
        int n=-1,add=0;
        List<int> temp = new List<int>(userAllMaJiang);
        if (userCurrentTakeMaJiang != -1)
        {
            temp.Add(userCurrentTakeMaJiang);
            temp.Sort();

            for (int i = 0; i < temp.Count; i++)
            {
                if (n != temp[i])
                {
                    add = 1;
                    n = temp[i];
                }
                else
                {
                    add++;
                    if (add>=4)
                    {
                        mj.Add((MAJIANG)n);
                    }
                }
            }
            for (int i = 0; i < AllPengGangChi[0].Count; i++)
            {
                PengGangChi pgc = AllPengGangChi[0][i];

                if (pgc.kind == 0)
                {
                    for (int j = 0; j < temp.Count; j++)
                    {
                        if (pgc.mj[0] == temp[j])
                        {
                            mj.Add((MAJIANG)temp[j]);
                        }
                    }
                }
            }
        }
        return mj;
    }
    /// <summary>
    /// 打牌
    /// </summary>
    /// <param name="isRecent">是否打的刚接的牌</param>
    /// <param name="nIndex">打出的牌</param>
    public void PutOutMaJiang(bool isRecent,int nIndex)
    {
        if (!isRecent)
        {
            userAllMaJiang.RemoveAt(nIndex);
            userAllMaJiang.Add(UserCurrentTakeMaJiang);
            userAllMaJiang.Sort();
        }
        UserCurrentTakeMaJiang = -1;
    }
    /// <summary>
    /// 玩家自己碰牌
    /// </summary>
    /// <param name="maJiang">牌型</param>
    /// <param name="who">碰的谁012</param>
    /// <returns>是否成功</returns>
    public bool SelfPeng(int maJiang,int who)
    {
        int count = 0;
        int index=-1;
        for (int i = 0; i < userAllMaJiang.Count; i++)
        {
            if (maJiang == userAllMaJiang[i])
            {
                if (index == -1)
                {
                    index = i;
                }
                if (++count > 2)
                {
                    break;
                }
            }
        }
        if (count<2)
        {//碰不了
            return false;
        }
        userAllMaJiang.RemoveRange(index, 2);
        pengGangChi[0].Add(new PengGangChi(new List<int>(new int[] { maJiang,maJiang,maJiang}),0,who));

        UserCurrentTakeMaJiang = userAllMaJiang[userAllMaJiang.Count - 1];
        userAllMaJiang.RemoveAt(userAllMaJiang.Count - 1);
        return true;
    }
    /// <summary>
    /// 续杠
    /// </summary>
    /// <param name="mj">杠的牌</param>
    /// <param name="desti">杠后接的牌</param>
    /// <returns></returns>
    public bool SelfXuGang(int mj,int desti)
    {
        PengGangChi gang;
        if (userCurrentTakeMaJiang <= 0)
        {//手里没牌怎么杠
            return false;
        }
        for (int i = 0; i < pengGangChi[0].Count; i++)
        {
            gang = pengGangChi[0][i];
            if (gang.kind == 0 && gang.mj[0] == mj)
            {
                if (userCurrentTakeMaJiang == mj)
                {
                    gang.mj.Add(mj);//杠
                    if (desti > 0)
                    {
                        userCurrentTakeMaJiang = desti;//换手牌
                    }
                    else
                    {
                        userCurrentTakeMaJiang = -1;
                    }
                    return true;
                }
                else
                {
                    for (int j = 0; j < userAllMaJiang.Count; j++)
                    {
                        if (userAllMaJiang[j] == mj)
                        {
                            gang.mj.Add(mj);
                            userAllMaJiang.RemoveAt(j);
                            userAllMaJiang.Add(userCurrentTakeMaJiang);//杠
                            if (desti > 0)
                            {
                                userCurrentTakeMaJiang = desti;//换手牌
                            }
                            else
                            {
                                userCurrentTakeMaJiang = -1;
                            }
                            userAllMaJiang.Sort();
                            return true;
                        }
                    }
                }
            }
        }        

        return false;
    }
    public bool OtherXuGang(int nIndex,int mj)
    {
        for (int i = 0; i < pengGangChi[nIndex].Count; i++)
        {
            if (pengGangChi[nIndex][i].kind == 0 && pengGangChi[nIndex][i].mj[0] == mj)
            {
                pengGangChi[nIndex][i].mj.Add(mj);
                return true;
            }
        }
        return false;
        
    }
    /// <summary>
    /// 暗杠
    /// </summary>
    /// <param name="maJiang">杠的牌型</param>
    /// <param name="desti">杠后接的牌</param>
    /// <returns></returns>
    public bool SelfAnGang(int maJiang,int desti)
    {
        if (userCurrentTakeMaJiang <= 0)
        {//手里没牌怎么杠
            return false;
        }
        int count = 0;
        int index = -1;
        for (int i = 0; i < userAllMaJiang.Count; i++)
        {
            if (maJiang == userAllMaJiang[i])
            {
                if (index == -1)
                {
                    index = i;
                }
                if (++count > 4)
                {
                    break;
                }
            }
        }
        if (count < 3)
        {//杠不了
            return false;
        }
        userAllMaJiang.RemoveRange(index, count);
        pengGangChi[0].Add(new PengGangChi(new List<int>(new int[] { maJiang, maJiang, maJiang ,maJiang}), 3, -1));

        userAllMaJiang.Add(userCurrentTakeMaJiang);//杠
        userAllMaJiang.Sort();

        if (desti > 0)
        {
            userCurrentTakeMaJiang = desti;//换手牌
        }
        else
        {
            userCurrentTakeMaJiang = -1;
        }
        return true;

    }
    /// <summary>
    /// 玩家自己直杠别人牌
    /// </summary>
    /// <param name="maJiang">牌型</param>
    /// <param name="who">杠的谁012</param>
    /// <returns>是否成功</returns>
    public bool SelfZhiGang(int maJiang, int desti,int who)
    {
        int count = 0;
        int index = -1;
        for (int i = 0; i < userAllMaJiang.Count; i++)
        {
            if (maJiang == userAllMaJiang[i])
            {
                if (index == -1)
                {
                    index = i;
                }
                if (++count > 3)
                {
                    break;
                }
            }
        }
        if (count < 3)
        {//杠不了
            return false;
        }
        userAllMaJiang.RemoveRange(index, 3);
        pengGangChi[0].Add(new PengGangChi(new List<int>(new int[] { maJiang, maJiang, maJiang,maJiang }), 1, who));
        if (desti>0)
        {
            userCurrentTakeMaJiang = desti;
        }
        return true;
    }
    /// <summary>
    /// 直接创建一组碰杠信息
    /// </summary>
    /// <param name="nIndex">位置索引</param>
    /// <param name="maJiang">牌</param>
    /// <param name="kind">碰杠类型</param>
    /// <param name="who">碰的哪一个</param>
    public void CreatePengGangChi(int nIndex,int maJiang,int kind,int who)
    {
        switch (kind)
        {
            case 0://碰
                {
                    pengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { maJiang, maJiang, maJiang }), 0, who));
                    break;
                }
            case 1://直杠
                {
                    pengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { maJiang, maJiang, maJiang,maJiang }), 1, who));
                    break;
                }
            case 2://续杠
                {
                    pengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { maJiang, maJiang, maJiang, maJiang }), 2, who));
                    break;
                }
            case 3://暗杠
                {
                    pengGangChi[nIndex].Add(new PengGangChi(new List<int>(new int[] { maJiang, maJiang, maJiang, maJiang }), 3, -1));
                    break;
                }
            default:
                break;
        }
    }
    /// <summary>
    /// 是否可碰牌
    /// </summary>
    /// <param name="maJiang">要碰的牌</param>
    /// <returns>可碰</returns>
    public virtual bool isCanPeng(int maJiang)
    {
        int count=0;
        for (int i = 0; i < userAllMaJiang.Count; i++)
        {
            if (maJiang == userAllMaJiang[i])
            {
                count++;
            }
        }
        if (count>=2)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 是否可直杠
    /// </summary>
    /// <param name="maJiang">要杠的牌</param>
    /// <returns></returns>
    public virtual bool isCanZhiGang(int maJiang)
    {
        int count = 0;
        for (int i = 0; i < userAllMaJiang.Count; i++)
        {
            if (maJiang == userAllMaJiang[i])
            {
                count++;
            }
        }
        if (count >= 3)
        {
            return true;
        }
        return false;
    }
    /// <summary>
    /// 检测暗杠，返回暗杠手牌数
    /// </summary>
    /// <returns>0-不可，3刚接的牌暗杠，4手牌暗杠</returns>
    public virtual int isCanAnGang(int mj)
    {
        bool b;
        int count=0;
        List<int> allMj= new List<int>(userAllMaJiang);
        if (userCurrentTakeMaJiang<=0)
        {
            return 0;
        }
        allMj.Add(userCurrentTakeMaJiang);
        allMj.Sort();
        for (int i = 0; i < allMj.Count; i++)
        {
            if (mj == allMj[i])
            {
                count++;
            }
        }
        if (count>=4)
        {
            if (userCurrentTakeMaJiang == mj)
            {
                return 3;
            }
            else
            {
                return 4;
            }
        }
        return 0;
    }
    /// <summary>
    /// 检测续杠
    /// </summary>
    /// <returns>-1不可续杠 可杠手牌索引 200刚接的牌</returns>
    public int isCanXuGang(int gangMJ)
    {
        PengGangChi gang;
        for (int i = 0; i < pengGangChi[0].Count; i++)
        {
            gang = pengGangChi[0][i];
            if (gang.kind == 0)
            {
                if (userCurrentTakeMaJiang == gang.mj[0] && gangMJ == gang.mj[0])
                {
                    return 200;
                }
                for (int j = 0; j < userAllMaJiang.Count; j++)
                {
                    if (userAllMaJiang[j] == gang.mj[0] && gangMJ == gang.mj[0])
                    {
                        return j;
                    }
                }
            }
        }
        return -1;
    }
    /// <summary>
    /// 其他玩家是否可以续杠
    /// </summary>
    /// <param name="nIndex"></param>
    /// <param name="mj"></param>
    /// <returns></returns>
    public bool isOtherCanXuGang(int nIndex,int mj)
    {
        for (int i = 0; i < pengGangChi[nIndex].Count; i++)
        {
            if (pengGangChi[nIndex][i].kind == 0 && pengGangChi[nIndex][i].mj[0] == mj)
            {
                return true;
            }
        }
        return false;
    }
    /// <summary>
    /// 是否可胡牌
    /// </summary>
    /// <param name="maJiang">要胡的牌</param>
    /// <returns></returns>
    public virtual bool isCanHu(int maJiang)
    {
        return true;
    }

    public virtual void Clear()
    {
        userAllMaJiang = new List<int>();
        userCurrentTakeMaJiang = -1;/// <remark>当前接的牌</remark>
        pengGangChi.Clear();
        for (int i = 0; i < 4; i++)
        {
            pengGangChi.Add(new List<PengGangChi>());
        }
    }
}
