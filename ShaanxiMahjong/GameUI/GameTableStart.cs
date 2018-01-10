using UnityEngine;
using System.Collections;

class GameTableStart : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
    public void OnStartBtn_Click()
    {
        MainRoot._gMainRoot.Tos("MainRoot Start ,need login tos!");
    }
	// Update is called once per frame
	void Update ()
    {
	
	}
}
