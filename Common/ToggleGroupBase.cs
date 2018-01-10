using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

class ToggleGroupBase : UnityEngine.UI.ToggleGroup
{
	/// <summary>
	/// 用于存储本组所有开关对象
	/// </summary>
	public Dictionary<int, ToggleBase> m_TogglesDic = new Dictionary<int, ToggleBase>();
	/// <summary>
	/// 获取本组所有开关的状态
	/// </summary>
	/// <returns>返回-1表示未选择，其他值表示选择了对应顺序的开关，参见ToggleScript( 返回0表示选择的第一个，依次类推)</returns>
	public int GetToggleGroupStat()
	{
		int n = -1;
		ToggleBase tog;
		if (!isActiveAndEnabled)
		{
			return n;
		}

		if (m_TogglesDic.Count!=0)	//如果是通过组进行管理的
		{
			for (int i = 0;i< m_TogglesDic.Count;i++)
			{
				tog = m_TogglesDic[i];
				if (tog.isOn)	//单选，多选在ToogleControl里处理
				{
					if (tog.ts!=null && name== "Toggle_DiFen")	//专为底分输入特殊处理
					{
						if (i==0)
						{
							n = 1;
							break;
						}
						if (i==1)
						{
							n = 10;
							break;
						}
						if (i==2 && tog.ts.MyInput != null && tog.ts.MyInput.name == "input_fen")
						{
							if (tog.ts.MyInput.text.Length == 0)
							{
								n = 100;
							}
							else
							{
								n = Convert.ToInt32(tog.ts.MyInput.text);
							}
							if (n <= 0)
							{
								n = 1;
							}
							if (n > 100)
							{
								n = 100;
							}
							break;
						}
						break;
					}
					n = i;
					break;
				}
			}
		}
		else //如果不是通过组进行管理，则可能是通过ToggleControl管理的
		{
			ToggleControl MyControl;
			MyControl = GetComponent<ToggleControl>();
			if (MyControl != null)  //多选开关组,则取ToggleControl的值
			{
				n = MyControl.GetToggleGroupStat();
			}
		}

		return n;
	}
	protected override void Start()
	{
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
