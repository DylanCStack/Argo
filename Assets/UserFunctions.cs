using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using simpleJSON;

public class UserFunctions : MonoBehaviour {

	public InputField phone;
	public Text alerts;
	public Text log;
	public GameObject Panel;

	private bool waiting = false;

	public string url;

	public void OnOpenWithUrl(string args){
		log.text = args;
		url = args;

		StartCoroutine (LogIn ());
	}

	public IEnumerator LogIn(){
		WWWForm login = new WWWForm ();
		login.AddField ("phone", PlayerPrefs.GetString("phone"));
		login.AddField ("code", url.Substring (url.Length - 5));

		WWW loginRequest = new WWW ("http://argo-server.herokuapp.com/user/login", login);
		yield return loginRequest;

		var response = JSON.Parse (loginRequest.text);

		if (response ["authToken"] != "false") {
			PlayerPrefs.SetString ("authToken", response ["authToken"]);
			log.text = "Successful login";
			waiting = false;
		} else if (response ["error"].AsBool) {
			//error
			log.text = "There was an error logging in.";
		} else {
			log.text = "Incorrect code or phone number. Please verify again.";
		}

		log.text = loginRequest.text;
	}


	IEnumerator _verifyPhone(){

		PlayerPrefs.SetString ("phone", phone.text);

		WWWForm form = new WWWForm ();
		form.AddField("phone", phone.text);
		WWW verifyPhone = new WWW("http://argo-server.herokuapp.com/user/verify", form);
		waiting = true;
		yield return verifyPhone;//wait for json response.

		var response = JSON.Parse(verifyPhone.text);
		Debug.Log (verifyPhone.text);
		Debug.Log (response);
		if(response["error"].AsBool){
			alerts.text = "There was an error verifying your account.";
		} else if (response["invalid number"].AsBool){
			log.text = "Invalid phone number";
		} else {
			log.text = "A login code has been sent to " + phone.text;
		}

	}
	public void verifyPhone(){
		Debug.Log (phone.text);
		Debug.Log ("Verify button clicked.");
		if (phone.text != "") {
			StartCoroutine (_verifyPhone ());
		} else {
			alerts.text = "Please enter a valid phone number.";
		}
	}



//	public WWW submit(string url){//make form and submission as a repeatable funciton
//
//	}

//	IEnumerable login(){
//
//	}

//	IEnumerable logout(){//could be void as it would just locally delete the cookie keeping the user logged in
//
//	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (waiting) {
			Panel.SetActive (true);
		} else {
			Panel.SetActive (false);
		}
	}
}
