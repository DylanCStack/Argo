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
using simpleJSON;


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
	}
		
	void OnEnable() {

		Image = GameObject.Find ("RawImage").GetComponent<RawImage>();
		Debug.Log (Image);
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
			
			if (cd.result.ToString() != "error") {

				videoName = cd.result.ToString();
				Debug.Log ("hello video name is" + videoName);
				StartVuforia ();
			}
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
	#else//empty functions to appease the unity editor

		public void _OpenContactPicker(){

		}
		public void _OpenVideoPicker(){

		}

	#endif
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
		
		String[] contactArray = name.Split ('|');
		contacts.Add (contactArray[0], contactArray [1]);
		GameObject list = FindObject(GameObject.Find("Canvas"),"AddressBookPanel");
		GameObject button = (GameObject)Instantiate(Resources.Load("AddressBookButton"), list.transform);
		button.GetComponentInChildren<Text> ().text = contactArray [0];
		list.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100, 50 * contacts.Count);
		button.GetComponent<Button> ().onClick.AddListener (() => {
			sendTo (contactArray[1]);
		});
	}

	void sendTo (string number){
		recipient = number;
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
			Debug.Log (ArgoResult ["error"]);
			yield return @"error";
		}
		yield return ArgoResult["response"];	
		GameObject.Find("DisplayLog").GetComponent<Text>().text = ArgoResult["response"];
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
		var ArgoResult = JSON.Parse (request.text);
		yield return ArgoResult;
		videoName = ArgoResult["response"];
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
		string videoName2 = videoName.Replace ("\"", "");
		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = bucket + videoName2;
		//set video url to value of qr code
		if(player.url != bucket + videoName2) {
			player.url = bucket + videoName2;
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
		StartCoroutine(RequireLogin(SendMessage( request)));//move to coroutine below which will wait for contact to be chosen.
	}

	public void SetContact(){
		contact = recipient;
		Debug.Log ("hello from set contact");
		Debug.Log (recipient);
	}

	public IEnumerator SendMessage(PostObjectRequest request){
		GameObject ContactPicker = FindObject(GameObject.Find("Canvas"), "ContactPickerPanel");
		GameObject homePanel = GameObject.Find ("HomeScreenPanel");

		ContactPicker.SetActive (true);
		homePanel.SetActive(false);
		Debug.Log ("hello from 418");

		contact = null;

		while (String.IsNullOrEmpty(contact)) {
			yield return null;
		}

		Debug.Log ("hello from 425");
		homePanel.SetActive (true);
		ContactPicker.SetActive (false);
		Debug.Log ("hello from 428");

		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{//successfully posted
					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));

					//Post record to ArgoDB
					StartCoroutine (
						PostToArgoDB (videoName, "private", contact)
					);
				}
				else
				{//did not post
					Debug.Log("\nException while posting the result object");
					Debug.Log(responseObj.Exception.Message);
				}
			});
	}

	public IEnumerator RequireLogin(IEnumerator next){
		GameObject VerifyPanel = FindObject(GameObject.Find("Canvas"), "VerifyPanel");
		GameObject HomePanel = GameObject.Find ("HomeScreenPanel");

		if (PlayerPrefs.GetString ("authToken").Length < 1) {
			VerifyPanel.SetActive (true);
			HomePanel.SetActive (false);
		}

		//verification panel will stay open until user confirms their phone number or chooses not to verify.
		//if user chooses not to verify they will be reset at the home screen by in a separate script. 
		while (PlayerPrefs.GetString ("authToken").Length < 1) {
			yield return null;
		}

		VerifyPanel.SetActive (false);
		HomePanel.SetActive (true);
		StartCoroutine (next);
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