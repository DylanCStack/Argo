using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchManager : MonoBehaviour {

	public Text log;

	public void OnOpenWithUrl(string args){
		log.text = args;
		for (int i = 0; i <= 10; i++) {
			Debug.Log ("ARGS: " + args);
		}
	}

	// Use this for initialization/
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
