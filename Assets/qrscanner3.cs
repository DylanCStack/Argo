using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using Vuforia;
using System.Runtime.InteropServices;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Util;
using Amazon.CognitoIdentity;
using Amazon;
using APIKeys;


public class qrscanner3 : MonoBehaviour {

	private IScanner BarcodeScanner;
	public RawImage Image;
	private float RestartTime;
	private WebCamTexture camTexture;
	private Rect screenRect;
	public static string _qrid;
	private string videoName;
	private string currentVideoName;
	private Dictionary <string, string> contacts = new Dictionary<string, string>();

	public string contact = null;
	public string recipient = null;
	/////////////////////////////////////////////////////////////////SCANNER METHODS
	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start () {
		_OpenContactPicker ();
//		ContactPicked("Fuck fuck|09345710");
//		ContactPicked("Fuck poop|01234710");
//		ContactPicked("Fuck shit|09345710");
//		ContactPicked("Fuck balls|09345710");
//		ContactPicked("Fuck yuck|09345710");
//		ContactPicked("Fuc dont|09345710");
//		ContactPicked("Fuk @mebro|09345710");
//		ContactPicked("Fck fuck|09345710");
//		ContactPicked("ck poop|01234710");
//		ContactPicked("Fck shit|09345710");
//		ContactPicked("uck balls|09345710");
//		ContactPicked("uck yuck|09345710");
//		ContactPicked("uck dont|09345710");
//		ContactPicked("uck @mebro|09345710");
//		ContactPicked("uck fuck|09345710");
//		ContactPicked("uck poop|01234710");
//		ContactPicked("s shit|09345710");
//		ContactPicked("Fck balls|09345710");
//		ContactPicked("Fck yuck|09345710");
//		ContactPicked("Fck dont|09345710");
//		ContactPicked("Fck @mebro|09345710");
//		ContactPicked("Fuc fuck|09345710");
//		ContactPicked("Fuc poop|01234710");
//		ContactPicked("Fuc shit|09345710");
//		ContactPicked("Fuc balls|09345710");
//		ContactPicked("Fuc yuck|09345710");
//		ContactPicked("Fucdont|09345710");
//		ContactPicked("Fuk@mebro|09345710");
//		ContactPicked("Fckfuck|09345710");
//		ContactPicked("ck oop|01234710");
//		ContactPicked("Fc shit|09345710");
//		ContactPicked("uc balls|09345710");
//		ContactPicked("uc yuck|09345710");
//		ContactPicked("uc dont|09345710");
//		ContactPicked("uc @mebro|09345710");
//		ContactPicked("uc fuck|09345710");
//		ContactPicked("uc poop|01234710");
//		ContactPicked("Fac shit|09345710");
//		ContactPicked("Fc balls|09345710");
//		ContactPicked("Fc yuck|09345710");
//		ContactPicked("Fc dont|09345710");
//		ContactPicked("Fc @mebro|09345710");
//		ContactPicked("Fuckw fuck|09345710");
//		ContactPicked("Fuckw poop|01234710");
//		ContactPicked("Fuckw shit|09345710");
//		ContactPicked("Fuckw balls|09345710");
//		ContactPicked("Fuckw yuck|09345710");
//		ContactPicked("Fuc wdont|09345710");
//		ContactPicked("Fuk w@mebro|09345710");
//		ContactPicked("Fck wfuck|09345710");
//		ContactPicked("ck powop|01234710");
//		ContactPicked("Fck wshit|09345710");
//		ContactPicked("uck wballs|09345710");
//		ContactPicked("uck wyuck|09345710");
//		ContactPicked("uck wdont|09345710");
//		ContactPicked("uck w@mebro|09345710");
//		ContactPicked("uck wfuck|09345710");
//		ContactPicked("uck wpoop|01234710");
//		ContactPicked("s shwit|09345710");
//		ContactPicked("Fck wballs|09345710");
//		ContactPicked("Fck wyuck|09345710");
//		ContactPicked("Fck wdont|09345710");
//		ContactPicked("Fck w@mebro|09345710");
//		ContactPicked("Fuc wfuck|09345710");
//		ContactPicked("Fuc wpoop|01234710");
//		ContactPicked("Fuc wshit|09345710");
//		ContactPicked("Fuc wballs|09345710");
//		ContactPicked("Fuc wyuck|09345710");
//		ContactPicked("Fucdwont|09345710");
//		ContactPicked("Fuk@wmebro|09345710");
//		ContactPicked("Fckfwuck|09345710");
//		ContactPicked("ck owop|01234710");
//		ContactPicked("Fc swhit|09345710");
//		ContactPicked("uc bwalls|09345710");
//		ContactPicked("uc ywuck|09345710");
//		ContactPicked("uc dwont|09345710");
//		ContactPicked("uc @wmebro|09345710");
//		ContactPicked("uc fwuck|09345710");
//		ContactPicked("uc pwoop|01234710");
//		ContactPicked("Fc swashit|09345710");
//		ContactPicked("Fc bwalls|09345710");
//		ContactPicked("Fc ywuck|09345710");
//		ContactPicked("Fc dwont|09345710");
//		ContactPicked("Fc @wmebro|09345710");
	}
		
	void OnEnable() {

		//attach amazon details
		UnityInitializer.AttachToGameObject(this.gameObject);

		//Destroy the old scanner when re-enabling
		BarcodeScanner = null;
		_qrid = null;
		BarcodeScanner = new Scanner ();
		BarcodeScanner.Camera.Play();
		Debug.Log("--------------------STARTED-FROM-UPDATE--------------------");
		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
			// Set Orientation & Texture
			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
			Image.transform.localScale = BarcodeScanner.Camera.GetScale();
			Image.texture = BarcodeScanner.Camera.Texture;

			// Keep Image Aspect Ratio
			var rect = Image.GetComponent<RectTransform>();
			var newWidth = rect.sizeDelta.y * BarcodeScanner.Camera.Width / BarcodeScanner.Camera.Height;
			rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);

			RestartTime = Time.realtimeSinceStartup;
		};

	}
		
		
	void Update()
	{
		if (BarcodeScanner != null)
		{

			BarcodeScanner.Update();
		} else if (BarcodeScanner == null) {//when scanner has been destroyed create new one
			

		}

		// Check if the Scanner need to be started or restarted
		if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup)
		{
			StartScanner();
			RestartTime = 0;

		}
	}






	public IEnumerator checkURL(string barCodeValue) {
		CoroutineWithData cd = new CoroutineWithData(this, CheckArgoDB(barCodeValue));
		yield return cd.coroutine;
		_qrid = barCodeValue;

		#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
		#endif

		if (cd.result.ToString() == "false") {	//no matching qrid in database
			
			_OpenVideoPicker ();

		} else {//video found in database

			videoName = cd.result.ToString ();
			StartVuforia ();

		}
	}


	/////////////////////////////////SCANNER METHODS
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();
			if(_qrid != barCodeValue) {
				_qrid = barCodeValue;
				GameObject.Find("DisplayLog").GetComponent<Text>().text = barCodeValue;
				CoroutineWithData cd = new CoroutineWithData(this, checkURL(barCodeValue));
			} else {

			}
			RestartTime += Time.realtimeSinceStartup + 1f;
		});
	}

/////////////////////////////////iOS PLUGIN
	#if UNITY_IOS
		//import videopicker method from custom iOS plugin
		[DllImport("__Internal")]
		private static extern void OpenVideoPicker (string game_object_name, string function_name);

		[DllImport("__Internal")]
		private static extern void OpenContactPicker (string game_object_name, string function_name);
	#endif

	public void _OpenVideoPicker() {
		Debug.Log("hello from _OpenVideoPicker");
		Debug.Log (_qrid + "from 240");
		PlayerPrefs.SetString ("qrid", _qrid);
		FindObject (GameObject.Find("Canvas"), "LoadingPanel").SetActive (true);
		OpenVideoPicker ("TestObject", "VideoPicked");//sends request to iOS with "TestObject" as return location and "VideoPicked" as callback function
	}

	public void _OpenContactPicker() {
		Debug.Log("hello from _OpenContactPicker");
//		FindObject (GameObject.Find("Canvas"), "LoadingPanel").SetActive (true);
		OpenContactPicker ("TestObject", "ContactPicked");//sends request to iOS with "TestObject" as return location and "ContactPicked" as callback function
	}

	//collect returned information from iOS plugin
	void VideoPicked( string path ){

		//reattatch amazon client
		UnityInitializer.AttachToGameObject(this.gameObject);

		//get image target 
		VideoPlayer videoPreview = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();

		//prepare path name for movie preview
		string newPath = path.Replace ("file:///", "");

		//assign video to imagetarget
		videoPreview.url = newPath;
		videoPreview.Play ();

		//Post video to S3
		PostObject (newPath);

	}

	void ContactPicked( string name ){

		Debug.Log (name);
		String[] contactArray = name.Split ('|');
		contacts.Add (contactArray[0], contactArray [1]);
		Transform list = GameObject.Find ("AddressBookPanel").transform;
		GameObject button = (GameObject)Instantiate(Resources.Load("AddressBookButton"), list);
		button.GetComponentInChildren<Text> ().text = contactArray [0];
		GameObject addressBookPanel = GameObject.Find ("AddressBookPanel");
		addressBookPanel.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100, 50 * contacts.Count);
		button.GetComponent<Button> ().onClick.AddListener (() => {
			sendTo (contactArray[1]);
		});
	}

	void sendTo (string name){
		recipient = contacts[name];
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
		yield return request.text;
		GameObject.Find("DisplayLog").GetComponent<Text>().text = request.text;
	}

	/// post a new message
	public IEnumerator PostToArgoDB(string filename,string privacy,string recipient) {
		WWWForm form = new WWWForm();

		form.AddField ("url", filename);
		form.AddField ("privacy", privacy);
		form.AddField ("recipient", recipient);
		form.AddField ("qrid",_qrid);
		Dictionary<string, string> headers = form.headers;
		headers.Add(
			"authToken", PlayerPrefs.GetString("authToken")
		);
		WWW request = new WWW("https://argo-server.herokuapp.com/message/upload", form.data, headers);
		yield return request;
		Debug.Log(request.text);
		videoName = request.text;
		GameObject.Find ("LoadingPanel").SetActive(false);
		StartVuforia();
	}



	/////////////////////////////////VUFORIA CONFIGURATION AND ACTIVATION
	public void StartVuforia() {
		//enable vuforia camera so it can track objects
		GameObject arCamera = FindRootObject("ARCamera");
		arCamera.SetActive (true);
		arCamera.GetComponent<VuforiaBehaviour>().enabled = true;
		GameObject camera = FindObject (arCamera, "Camera");
		camera.SetActive (true);

		//enable the image target so vuforia knows what to track
		GameObject ARScanner = GameObject.Find("ImageTarget");

		//enable the video player on the image target
		VideoPlayer player = ARScanner.GetComponent<VideoPlayer>();
		ARScanner.GetComponent<ImageTargetPlayAudio>().enabled = true;

		GameObject.Find("RawImage").SetActive(false);

		string bucket = "https://s3-us-west-2.amazonaws.com/eyecueargo/";

		//set video url to value of qr code
		if(player.url != bucket + videoName) {
			player.url = bucket + videoName;
		}
	}



/////////////////////////////////AMAZON CONFIGURATION AND REQUESTS

	//Amazon config
	public string IdentityPoolId = "us-west-2:bdbe6639-8a19-4315-b0ad-294039672635";
	public string CognitoIdentityRegion = RegionEndpoint.USWest2.SystemName;
	private RegionEndpoint _CognitoIdentityRegion
	{
		get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
	}
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
		string videoName = generateVideoName(filePath);

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
		StartCoroutine(SendMessage( request));//move to coroutine below which will wait for contact to be chosen.
	}
	public void SetContact(string chosenContact){
		contact = recipient;
	}

	public IEnumerator SendMessage(PostObjectRequest request){
		GameObject ContactPicker = FindObject(GameObject.Find("Canvas"), "ContactPickerPanel");
		GameObject homePanel = GameObject.Find ("HomeScreenPanel");

		ContactPicker.SetActive (true);
		homePanel.SetActive(false);

		while (contact==null) {
			yield return contact;
		}

		homePanel.SetActive (true);
		ContactPicker.SetActive (false);

		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{//successfully posted
					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));

					//Post record to ArgoDB
					StartCoroutine (
						PostToArgoDB (videoName, "public", recipient)
					);
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
	


