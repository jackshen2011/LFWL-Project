using UnityEngine;
using System.Collections;
/// <summary>
/// 开始麻将按钮，为了区别发送是否第一局的消息
/// </summary>
class StartButton : ButtonBase
{
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    public bool IfFirstStart{get; set;}//是否第一局准备按钮
}
