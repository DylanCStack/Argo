using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;
using UnityEngine.UI;

public class ReloadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void reloadScene () {
		GameObject.Find ("ARCamera").GetComponent<VuforiaBehaviour> ().enabled = false;
		GameObject.Find ("ARCamera").GetComponent<qrscanner3> ().enabled = false;
		GameObject.Find ("ARCamera").GetComponent<qrscanner3> ().enabled = true;

	}


}
