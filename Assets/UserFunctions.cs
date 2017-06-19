using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using simpleJSON;

public class UserFunctions : MonoBehaviour {

	public InputField phone;
	public InputField email;
	public InputField password;
	public Text alerts;

	IEnumerator _register(){
		WWWForm form = new WWWForm ();
		form.AddField("phone", phone.text);
		form.AddField ("email", email.text);
		form.AddField("password", password.text);
		WWW register = new WWW("localhost:3000/user/register", form);
		yield return register;//wait for json response.

		var response = JSON.Parse(register.text);

		Debug.Log (response);
		if(response["error"].AsBool){
			alerts.text = "There was an error registering your account.";
		} else if (!response["register"].AsBool){
			alerts.text = "The phone number or email address is already in use.";
		} else if(response["register"].AsBool){
			alerts.text = "Successfully registered. Welcome to Argo.";
		}

	}
	public void register(){
		if (phone.text != "" && password.text != "") {
			StartCoroutine (_register ());
		} else {
			alerts.text = "Please enter a valid phone number and password";
		}
	}
	IEnumerator _login(){
		WWWForm form = new WWWForm ();
		form.AddField("phone", phone.text);
		form.AddField ("email", email.text);
		form.AddField("password", password.text);

		Dictionary<string, string> headers = form.headers;
		headers ["cookie"] = PlayerPrefs.GetString ("cookie");

		WWW loginRequest = new WWW("localhost:3000/user/login", form.data, headers);
		yield return loginRequest;//wait for json response.

		var response = JSON.Parse(loginRequest.text);



		Debug.Log (response);
		if(response["error"].AsBool){
			alerts.text = "There was an error registering your account.";
		} else if (!response["login"].AsBool){
			alerts.text = "Invalid phone number or password.";
		} else if(response["login"].AsBool){
			Debug.Log (loginRequest.text);


			if (loginRequest.responseHeaders.ContainsKey("SET-COOKIE")) {
				Debug.Log ("SETTING NEW KEY");
				string cookie = loginRequest.responseHeaders ["SET-COOKIE"];//.Substring (8);//gets cookie data to be set
//			string cookie = loginRequest.responseHeaders ["SET-COOKIE"] || loginRequest.responseHeaders[""];//gets cookie data to be set

				PlayerPrefs.SetString ("cookie", cookie);
			}
			Debug.Log (PlayerPrefs.GetString("cookie"));

			alerts.text = "Successfully logged in. Welcome Back.";
		}

	}

	public void login(){
		if (phone.text != "" && password.text != "") {
			StartCoroutine (_login ());
		} else {
			alerts.text = "Please enter a valid phone number and password";
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
		
	}
}
