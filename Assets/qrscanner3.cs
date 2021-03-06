﻿//this script holds most of the functionality of Argo - the qrscanner, the iOS plugins, the amazon connectivity, 
// and the connection to our own database. much of the code is borrowed from the stack overflow and unity forums, as 
// well as documentation. 

using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
//using UnityEditor;
using Vuforia;
using System.Runtime.InteropServices;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Util;
using Amazon;
using APIKeys; //custom class to hold api keys - amazon keys have to be added in this class
using simpleJSON;


public class qrscanner3 : MonoBehaviour {

	//Public References to GameObjects

	public GameObject ARCamera;
	public GameObject childCamera;
	public GameObject ImageTarget;
	public GameObject childPlane;


	public GameObject ImageObj;
	public RawImage Image;

	public Text DisplayLog;
	public GameObject HomeScreenPanel;
	public GameObject AddressBookPanel;
	public GameObject LoadingPanel;
	public GameObject VerifyPanel;
	public GameObject ContactPickerPanel;

//	ImageTarget
//		Plane
//	AddressBookPanel
//	LoadigPanel
//	VerifyPanel
//	HomeScreenPanel
//	ContactPickerPanel
//	ARCamera
//		Camera
	//

	private IScanner BarcodeScanner;
	private float RestartTime;
	private WebCamTexture camTexture;
	private Rect screenRect;
	public static string _qrid;
	private string videoName;
	private string currentVideoName;
	private static string aspectRatio;
	private float currentAspectRatio;
	private static float orientationAngle;
	private Dictionary <string, string> contacts = new Dictionary<string, string>();

	public string contact = null;
	public string recipient = null;

	//plurals to test sending to an arbitrary number of contacts
	public Dictionary<string, string> contactsDict = new Dictionary<string,string>();
	public Dictionary<string,string> recipientsDict = new Dictionary<string,string>();
	//


	/////////////////////////////////////////////////////////////////SCANNER METHODS
	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start () {
		_OpenContactPicker ();
	}

	void OnEnable() {

		//attach amazon details
		UnityInitializer.AttachToGameObject(this.gameObject);

		ConfigureScanner ();


	}

	void OnDisable() {

	}


	void Update()
	{
		if (BarcodeScanner != null)
		{

			BarcodeScanner.Update();

		}

		// Check if the Scanner need to be started or restarted
		if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
		{

			StartScanner();
			RestartTime = 0;

		}
	}





	// check if a scanned url exists in our database
	public IEnumerator checkURL(string barCodeValue) {
		//helper method for coroutines with data - returns response from ARGO servers
		CoroutineWithData cd = new CoroutineWithData(this, CheckArgoDB(barCodeValue));
		yield return cd.coroutine;
		_qrid = barCodeValue;

		//parse response string in to json
		var responseObject = JSON.Parse (cd.result.ToString());

		//give feeback to signal that a barcode has been scanned
		#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
		#endif

		if (responseObject["url"] == "false") {	//no matching qrid in database

			_OpenVideoPicker ();

		} else {//video found in database

			//response is not an error - start vuforia
			if (cd.result.ToString() != "error") {

				videoName = responseObject["url"];
				currentAspectRatio = responseObject ["aspect_ratio"];
				StartCoroutine(
					StartVuforia ()
				);
			}
		}
	}


	/////////////////////////////////SCANNER METHODS
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();
			//make sure barcode is both new and begins with ARGO
			if(_qrid != barCodeValue && barCodeValue.Substring(0,4) == "ARGO") {
				_qrid = barCodeValue;
				//helper method for coroutines with data - check to see if the qrid exists in our database
				CoroutineWithData cd = new CoroutineWithData(this, checkURL(barCodeValue));
			} else {

			}
			RestartTime += 1f;
		});
	}

	private void ConfigureScanner()
	{
		//clears existing scanners and starts a new one
		BarcodeScanner = null;
		_qrid = null;
		BarcodeScanner = new Scanner ();
		BarcodeScanner.Camera.Play();

		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
			// Set Orientation & Texture
			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
			Image.transform.localScale = BarcodeScanner.Camera.GetScale();
			Image.texture = BarcodeScanner.Camera.Texture;

			Vector2 screenSize = new Vector2(Screen.width, Screen.height);
			Image.GetComponent<RectTransform>().sizeDelta = screenSize;
			// Keep Image Aspect Ratio
			var rect = Image.GetComponent<RectTransform>();
			var newWidth = rect.sizeDelta.y * BarcodeScanner.Camera.Width / BarcodeScanner.Camera.Height;
			rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);

			RestartTime += Time.realtimeSinceStartup + 1f;
		};
	}

/////////////////////////////////iOS PLUGIN
	#if UNITY_IOS
		//import videopicker method from custom iOS plugin
		[DllImport("__Internal")]
		private static extern void OpenVideoPicker (string game_object_name, string function_name);

		//import contact collector
		[DllImport("__Internal")]
		private static extern void OpenContactPicker (string game_object_name, string function_name);

		//wrapper for imported method
		public void _OpenVideoPicker() {
			OpenVideoPicker ("TestObject", "VideoPicked");//sends request to iOS with "TestObject" as return location and "VideoPicked" as callback function
		}

		//wrapper for imported method
		public void _OpenContactPicker() {
			Debug.Log("hello from _OpenContactPicker");
			OpenContactPicker ("TestObject", "ContactPicked");//sends request to iOS with "TestObject" as return location and "ContactPicked" as callback function
		}


	#else//empty functions to appease the unity editor

		public void _OpenContactPicker(){

		}
		public void _OpenVideoPicker(){

		}


	#endif

	//collect returned information from iOS plugin
	void VideoPicked( string path ){

		//break the string in to the video path ([0]), aspect ratio ([1]), and orientation in degrees([2])
		String[] videoInfoArray = path.Split ('|');

//		//reattatch amazon client
//		UnityInitializer.AttachToGameObject(this.gameObject);

		//get image target
		VideoPlayer player = ImageTarget.GetComponent<VideoPlayer> ();

		//prepare path name for movie preview
		string newPath = videoInfoArray[0].Replace ("file:///", "");

		//save aspect ratio
		string aspectRatioString = videoInfoArray [1];
		currentAspectRatio = float.Parse (aspectRatioString);
		aspectRatio = aspectRatioString;

		//get orientation angle
//		if (videoInfoArray [2] == "portrait") {
//
//		} else {
//
//		}

		//assign video to imagetarget
		player.url = newPath;
		player.Play ();

		//Post video to S3
		PostObject (newPath);

	}

	//reciever method for a single contact
	void ContactPicked( string name ){

		//split contact in to name and phone number
		String[] contactArray = name.Split ('|');

		//add this contact to the dictionary
		contacts.Add (contactArray[0], contactArray [1]);

		//add a button to the contact list to represent the new contact
		GameObject list = AddressBookPanel;
		GameObject button = (GameObject)Instantiate(Resources.Load("AddressBookButton"), list.transform);
		button.GetComponentInChildren<Text> ().text = contactArray [0];

		//expand the list to accomadate the new button
		list.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100, 104 * contacts.Count);

		//add a click listener that sets the recipient phone number
		button.GetComponent<Button> ().onClick.AddListener (() => {
			sendTo (contactArray[1]);
			sendToDict(contactArray[0],contactArray[1]);

		});
	}

	//in case the video picker gets cancelled
	public void PickerDidCancel(string cancel) {
		ConfigureScanner ();
	}

	//click function attached to contact buttons
	void sendTo (string number){
		recipient = number;
	}
	void sendToDict(string name, string number){
		if (recipientsDict.ContainsKey (name)) {
			recipientsDict.Remove (name);
		} else {
			recipientsDict.Add (name, number);
		}
	}

	//sets contact after confirmation button is clicked
	public void SetContact(){
		contact = recipient;
		contactsDict = recipientsDict;
	}

	//sets public contact after confirmation
	public void SetPublicContact() {
		contact = "public";
	}

	public void ContactPickerDidCancel() {
		ConfigureScanner ();
	}

	/////////////////////////////////ARGO SERVER METHODS

	/// check if qrid exists
	public IEnumerator CheckArgoDB(string qrid) {
		WWWForm form = new WWWForm();
		form.AddField ("qrid", qrid);
		Dictionary<string, string> headers = form.headers;
		headers.Add(
			"authToken", PlayerPrefs.GetString("authToken")
		);
		WWW request = new WWW ("https://argo-server.herokuapp.com/message/checkCode", form.data, headers);
		yield return request;
		var ArgoResult = JSON.Parse(request.text);

		if (ArgoResult ["error"].AsBool) {
			//handle errors
			Debug.Log (ArgoResult ["error"]);
			DisplayLog.text = ArgoResult["response"];
			yield return @"error";
		}

		//return the name of the video if exists
		yield return ArgoResult ["response"];

		//set the aspect ratio
		currentAspectRatio = ArgoResult ["response"]["aspect_ratio"];
		DisplayLog.text = ArgoResult["response"];
	}

	/// post a new message
	public IEnumerator PostToArgoDB(string filename,string privacy,string recipient) {
		WWWForm form = new WWWForm();

		form.AddField ("url", filename);
		form.AddField ("privacy", privacy);
		form.AddField ("recipient", recipient);
		form.AddField ("qrid",_qrid);
		form.AddField ("does_loop", "1");
		form.AddField ("permanent", "1");
		form.AddField ("aspect_ratio", aspectRatio);
		Dictionary<string, string> headers = form.headers;
		headers.Add(
			"authToken", PlayerPrefs.GetString("authToken")
		);
		WWW request = new WWW("https://argo-server.herokuapp.com/message/upload", form.data, headers);
		yield return request;
		var ArgoResult = JSON.Parse (request.text);
		LoadingPanel.SetActive(false);
		yield return StartCoroutine (
			StartVuforia ()
			// StartDigitalPreview ()
		);
		yield return StartCoroutine (
			SaveThumbnailToS3()
		);

	}

	public IEnumerator StartDigitalPreview(){
		FindRootObject ("PreviewHolder").SetActive (true);


		StartCoroutine (StopCamera ());
		yield return null;
	}

	/////////////////////////////////VUFORIA CONFIGURATION AND ACTIVATION
	public IEnumerator StartVuforia() {
		//enable vuforia camera so it can track objects
		ARCamera.SetActive (true);
		ARCamera.GetComponent<VuforiaBehaviour>().enabled = true;
		childCamera.SetActive (true);


		//enable the video player on the image target
		VideoPlayer player = ImageTarget.GetComponent<VideoPlayer>();
		ImageTarget.GetComponent<ImageTargetPlayAudio>().enabled = true;
		Vector3 aspectRatioVector = new Vector3 (0.16F, (0.16F * currentAspectRatio), (0.16F * currentAspectRatio));
		childPlane.transform.localScale = aspectRatioVector;

		ImageObj.SetActive(false);

		StartCoroutine(
			StopCamera()
		);


		string bucket = "https://s3-us-west-2.amazonaws.com/eyecueargo/";
		string videoName2 = videoName.Replace ("\"", "");
		DisplayLog.text = bucket + videoName2;
		//set video url to value of qr code
		if(player.url != bucket + videoName2) {
			player.url = bucket + videoName2;
		}

		yield return null;

	}



/////////////////////////////////AMAZON CONFIGURATION AND REQUESTS
	//Make sure you place your valid basic amazon credential apikeys in the AWSKeys class!
	//Amazon config
	public string S3Region = RegionEndpoint.USWest2.SystemName;
	private RegionEndpoint _S3Region
	{
		get { return RegionEndpoint.GetBySystemName(S3Region); }
	}
	public string S3BucketName = "eyecueargo";
	private IAmazonS3 _s3Client;
	private BasicAWSCredentials Credentials = new BasicAWSCredentials(AWSKeys.visible, AWSKeys.secret);


	//Client constructor
	private IAmazonS3 Client
	{
		get
		{
			if (_s3Client == null)
			{
				_s3Client = new AmazonS3Client(Credentials, _S3Region);
			}
			return _s3Client;
		}
	}

	//Post a video to S3
	public void PostObject(string filePath)
	{

		//Adjust file name to be more readable
		videoName = generateVideoName(filePath);

		//prepare file for upload
		var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);


		//prepares request to amazon
		var request = new PostObjectRequest()
		{
			Bucket = S3BucketName,
			Key = videoName,
			InputStream = stream,
			CannedACL = S3CannedACL.Private,
			Region = _S3Region
		};

		//post request to amazon
		StartCoroutine(RequireLogin(SendMessage(request)));//move to coroutine below which will wait for contact to be chosen.
	}



	public IEnumerator SendMessage(PostObjectRequest request){

		if (PlayerPrefs.GetString ("authToken").Length < 1) {
			VerifyPanel.SetActive (true);
			HomeScreenPanel.SetActive (false);
		}

		//verification panel will stay open until user confirms their phone number or chooses not to verify.
		//if user chooses not to verify they will be reset at the home screen by in a separate script.
		while (PlayerPrefs.GetString ("authToken").Length < 1) {
			yield return null;
		}

		VerifyPanel.SetActive (false);
		HomeScreenPanel.SetActive (true);
		//////above code from RequireLogin

		//Contact panel will stay active until the user confirms their selection. Multiple contacts may be toggled on click/tap. This will not show visually.
		ContactPickerPanel.SetActive (true);
		HomeScreenPanel.SetActive(false);

		contact = null;
		contactsDict = null;

		while (contactsDict == null) {
//		while (String.IsNullOrEmpty(contact)) {//old code from when only one contact could be chosen. Revert if multiple selections not working
			yield return null;
		}

		HomeScreenPanel.SetActive (true);
		ContactPickerPanel.SetActive (false);

		LoadingPanel.SetActive (true);
		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{//successfully posted
					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));

					//Post record to ArgoDB
					foreach(KeyValuePair<string,string> person in contactsDict){
						StartCoroutine (
							PostToArgoDB (videoName, "private", person.Value)
						);
					}
					//old code from when only one contact could be chosen. Revert if multiple selections not working
					// StartCoroutine(
					// 	PostToArgoDB(videoName, "private", contact)
					// )
				}
				else
				{//did not post
					Debug.Log("\nException while posting the result object");
					Debug.Log(responseObj.Exception.Message);
				}
			});
	}

	public IEnumerator RequireLogin(IEnumerator next){
//if the user is not logged in this will require it before the next coroutine can be run
		if (PlayerPrefs.GetString ("authToken").Length < 1) {
			VerifyPanel.SetActive (true);
			HomeScreenPanel.SetActive (false);
		}

		//verification panel will stay open until user confirms their phone number or chooses not to verify.
		//if user chooses not to verify they will be reset at the home screen by in a separate script.
		while (PlayerPrefs.GetString ("authToken").Length < 1) {
			yield return null;
		}

		VerifyPanel.SetActive (false);
		HomeScreenPanel.SetActive (true);

		yield return StartCoroutine (next);

	}

	public IEnumerator SaveThumbnailToS3() {

		yield return new WaitForSeconds(1);
		VideoPlayer player = ImageTarget.GetComponent<VideoPlayer> ();
		player.Play ();
		yield return new WaitForSeconds(1);
		while (player.texture == null) {
			yield return null;
		}

		Texture texture = player.texture;
		RenderTexture Rtexture = texture as RenderTexture;
		RenderTexture.active = Rtexture;
		Texture2D myTexture2d = new Texture2D (texture.width, texture.height);
		myTexture2d.ReadPixels (new Rect (0, 0, Rtexture.width, Rtexture.height), 0, 0);
		myTexture2d.Apply ();
		RenderTexture.active = null;

		byte[] thumbnail = myTexture2d.EncodeToPNG();
		player.Stop ();

		MemoryStream stream = new MemoryStream(thumbnail);

		string videoName2 = videoName.Replace ("\"", "");
		string videoName3 = videoName2.Replace (".mov", ".png");
		//prepares request to amazon
		var request = new PostObjectRequest()
		{
			Bucket = S3BucketName,
			Key = videoName3,
			InputStream = stream,
			CannedACL = S3CannedACL.Private,
			Region = _S3Region
		};
		Debug.Log (Client);


		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{//successfully posted
					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));

				}
				else
				{//did not post
					Debug.Log("\nException while posting the result object");
					Debug.Log(responseObj.Exception.Message);

				}
			});

	}

/// ///////////////////////////////FILE NAME CREATION
	string generateVideoName(string filePath) {
		string fileName = filePath.Replace ("/", "");
		string fileName2 = fileName.Replace ("tmptrim.", "");
		string fileName3 = fileName2.Replace (".MOV", ".mov");
		string fileName4 = fileName3.Replace("privatevarmobileContainersDataApplication", "");
		string fileName5 = fileName4.Substring (fileName4.Length - 12);
		string fileName6 = SystemInfo.deviceUniqueIdentifier + "-" + fileName5;

		videoName = fileName6;
		return fileName6;
	}


/// ///////////////////////////////FIND GAME OBJECT HELPER METHODS


	GameObject FindObject(GameObject parent, string name)
	{
		Component[] trs= parent.GetComponentsInChildren(typeof(Transform), true);
		foreach(Component t in trs){
			if(t.name == name){
				return t.gameObject;
			}
		}
		return null;
	}

	GameObject FindRootObject(string name)
	{
		List<GameObject> rootObjects = new List<GameObject>();
		Scene scene = SceneManager.GetActiveScene();
		scene.GetRootGameObjects( rootObjects );
		foreach(GameObject t in rootObjects){
			if(t.name == name){
				return t.gameObject;
			}
		}
		return null;
	}



	public IEnumerator StopCamera()
	{
		// Stop Scanning
		Image = null;
		BarcodeScanner.Destroy();
		BarcodeScanner = null;

		// Wait a bit
		yield return new WaitForSeconds(0.1f);
	}
}

/// ///////////////////////////////RUN COROUTINES OUTSIDE OF FRAMES HELPER CLASS

public class CoroutineWithData {
	public Coroutine coroutine { get; private set; }
	public object result;
	private IEnumerator target;
	public CoroutineWithData(MonoBehaviour owner, IEnumerator target) {
		this.target = target;
		this.coroutine = owner.StartCoroutine(Run());
	}

	private IEnumerator Run() {
		while(target.MoveNext()) {
			result = target.Current;
			yield return result;
		}
	}

	public void stopCoroutine() {
		this.coroutine = null;
	}
}
