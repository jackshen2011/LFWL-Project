using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class ToggleControl : UnModalUIBase
{

	public Dictionary<int, ToggleBase> m_TogglesDic = new Dictionary<int, ToggleBase>();
	/// <summary>
	/// 
	/// </summary>
	/// <returns>返回0表示未选择任何开关，返回其他非0正值则是表示选择的开关的按位或</returns>
	public int GetToggleGroupStat()
	{
		int n = -1;
		ToggleBase tog;
		for (int i = 0; i < m_TogglesDic.Count; i++)
		{
			tog = m_TogglesDic[i];
			if (tog.isOn)	//多选，按位记录
			{
				if (n == -1 && i<31)
				{
					n = 0;
				}
				n = n | 1 << i;
			}
		}
		return n;
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
