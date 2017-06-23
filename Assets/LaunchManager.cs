using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LaunchManager : MonoBehaviour {

	public Text log;
	public string url;

	public void OnOpenWithUrl(string args){
		log.text = args;
		url = args;
		for (int i = 0; i <= 10; i++) {
			Debug.Log ("ARGS: " + args);
		}

		StartCoroutine (_OnOpenWithUrl ());
	}
	public IEnumerator _OnOpenWithUrl(){
		WWWForm login = new WWWForm ();
		login.AddField ("phone", PlayerPrefs.GetString("phone");
		login.AddField ("code", url.Substring (url.Length - 5));

		WWW loginRequest = new WWW ("http://argo-server.herokuapp.com/user/login", login);
		yield return loginRequest;

		log.text = loginRequest.text;
	}
	// Use this for initialization/
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
