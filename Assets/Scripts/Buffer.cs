using UnityEngine;
using System.Collections;

public class Buffer : MonoBehaviour {

	private string buffer = "";

	// Use this for initialization
	void Start () {
		buffer = "";
	}

	void Awake(){
		DontDestroyOnLoad(transform.gameObject);
	}

	public void storeBuffer(string value){
		buffer = value;
	}

	public bool isEmpty(){
		if( buffer == "" ) return true;
		return false;
	}

	public string getBuffer(){
		return buffer;
	}

	public void freeBuffer(){
		buffer = "";
	}
}
