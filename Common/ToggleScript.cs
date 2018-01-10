using UnityEngine;
using System.Collections;

class ToggleScript : MonoBehaviour {
	[HideInInspector]
	public int nIndexOfGroup = -1;  //本开关在组内的顺序,预制体内填写数值，从0开始
	public ToggleControl MyControl;
	public InputFieldBase MyInput;
	/// <summary>
	/// 初始化开关在本组的序号，用于在开关List中定位
	/// </summary>
	public void Initial()
	{
		switch (name)
		{
			case "8ju": nIndexOfGroup = 0; break;
			case "16ju": nIndexOfGroup = 1; break;
			case "fangzuobi": nIndexOfGroup = 0; break;
			case "pinghu": nIndexOfGroup = 0; break;
			case "zimo": nIndexOfGroup = 1; break;
			case "7duijiafan": nIndexOfGroup = 0; break;
			case "7duibujiafan": nIndexOfGroup = 1; break;
			case "0": nIndexOfGroup = 0; break;
			case "1": nIndexOfGroup = 1; break;
			case "2": nIndexOfGroup = 2; break;
			case "3": nIndexOfGroup = 3; break;
			case "4": nIndexOfGroup = 4; break;
			case "ziyou": nIndexOfGroup = 5; break;
			case "daifeng": nIndexOfGroup = 0; break;
			case "hongzhonglaizi": nIndexOfGroup = 1; break;
			case "258yingjiang": nIndexOfGroup = 2; break;
			case "hu258jiafan": nIndexOfGroup = 3; break;
			case "jiang258jiafan": nIndexOfGroup = 4; break;
			case "qingyisejiafan": nIndexOfGroup = 5; break;
			case "lunliuzuozhuang": nIndexOfGroup = 0; break;
			case "yingjiazuozhuang": nIndexOfGroup = 1; break;
			case "dianpaoyijiafu": nIndexOfGroup = 0; break;
			case "dianpaosanjiafu": nIndexOfGroup = 1; break;
			case "zhuawanhuangzhuang": nIndexOfGroup = 0; break;
			case "1gangqi2gangba": nIndexOfGroup = 1; break;
			case "zimofanbei": nIndexOfGroup = 0; break;
			case "wuxianzhidengdai": nIndexOfGroup = 0; break;
			case "15miaodengdai": nIndexOfGroup = 1; break;
			case "yunxupangguan": nIndexOfGroup = 0; break;
			case "yunxudiaoyu": nIndexOfGroup = 1; break;
			case "1fen": nIndexOfGroup = 0; break;
			case "10fen": nIndexOfGroup = 1; break;
			case "zidingyifen": nIndexOfGroup = 2; break;

			default:
				break;
		}
	}

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
