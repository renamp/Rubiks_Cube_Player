using UnityEngine;
using System.Collections;

public class test : MonoBehaviour {

	string text = "";
	string addr = "";

	// Use this for initialization
	void Start () {
	
	}

	void OnGUI(){

		addr = GUI.TextField(new Rect(0,0,300,30), addr);

		if( GUI.Button(new Rect(320,0,60,30), "COUNT") ){
			addr += "#" + (addr.Length+4);
		}

		if( GUI.Button(new Rect(0,40,100,100), "connect") ){
			WWW www = new WWW(addr);

			addr += "#" + addr.Length;

			while( !www.isDone );
			text = www.text;
		}

		GUI.TextArea(new Rect(0,160,300,300), text);
	}

	// Update is called once per frame
	void Update () {
	
	}
}
