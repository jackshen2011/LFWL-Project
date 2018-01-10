using UnityEngine;
using System.Collections;
using System;

class clocktimer : MonoBehaviour {

    public TextBase ttimer;
    DateTime dt;
    // Use this for initialization
    void Start ()
    {
        dt = DateTime.Now;
    }

    // Update is called once per frame
    void Update ()
    {
        if (ttimer)
        {
            dt = DateTime.Now;
            ttimer.text = dt.ToString("HH:mm");
        }
	}
}
