using UnityEngine;
using System.Collections;

class ToggleBase : UnityEngine.UI.Toggle {

	ToggleGroupBase MyGroup;

	public ToggleScript ts;

	/// <summary>
	/// 创建时加载附加脚本
	/// </summary>
	protected override void Awake()
	{
		/*ToggleScript[] templ = gameObject.GetComponents<ToggleScript>();
		foreach (ToggleScript item in templ)
		{
			GameObject.DestroyImmediate(item);
		}*/
		ts = gameObject.GetComponent<ToggleScript>();
		if (ts  == null )
		{
			Debug.Log("Unity:"+ts.name);
			ts = gameObject.AddComponent<ToggleScript>();
		}
		ts.Initial();
	}
	/// <summary>
	/// 将自己加入开关组的list中，方便进行管理
	/// </summary>
	private void GetToogleInfo()
	{
		if (isActiveAndEnabled )
		{
			if (group != null)
			{
				MyGroup = group.GetComponent<ToggleGroupBase>();
			}
			if (MyGroup != null)	//按组管理的，所以是单选
			{
				if (ts.nIndexOfGroup != -1)
				{
					MyGroup.m_TogglesDic.Add(ts.nIndexOfGroup, this);
				}
				//Debug.Log("Unity:"+name+":"+ ts.nIndexOfGroup.ToString());
			}
			else        //不按组管理的，所以是独立开关，或者是多选开关组
			{

				//ToggleControl
				if (ts.MyControl !=null)	//多选开关组
				{
					if (ts.nIndexOfGroup != -1)
					{
						ts.MyControl.m_TogglesDic.Add(ts.nIndexOfGroup, this);
					}
				}
			}
		}
	}

	protected override void Start ()
	{
		GetToogleInfo();
	}

	// Update is called once per frame
	void Update () {
	
	}
}
