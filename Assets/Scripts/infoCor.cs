using UnityEngine;
using System.Collections;
using System.IO;

public class infoCor : MonoBehaviour {
	
	public MoveCubo script;
	public Transform cubo;
	
	public Material[] mat;
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		for( int i=0,k=0; i<6; i++)
			for( int j=0; j<9; j++,k++){
				try{
					string temp = cubo.GetChild(k).name;
					int a = (int)(temp[0]-97), b = (int)(temp[2]-0x30);
					cubo.GetChild(k).GetComponent<Renderer>().material = mat[ script.m[a][b] ];
					
				}
				catch{
				}
			}
	}
}
