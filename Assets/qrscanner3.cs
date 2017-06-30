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
	private static string aspectRatio;
	private float currentAspectRatio;
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

			RestartTime += Time.realtimeSinceStartup + 1f;
		};
			

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






	public IEnumerator checkURL(string barCodeValue) {
		CoroutineWithData cd = new CoroutineWithData(this, CheckArgoDB(barCodeValue));
		yield return cd.coroutine;
		_qrid = barCodeValue;

		var responseObject = JSON.Parse (cd.result.ToString());

		#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
		#endif

		if (responseObject["url"] == "false") {	//no matching qrid in database
			
			_OpenVideoPicker ();

		} else {//video found in database
			
			if (cd.result.ToString() != "error") {

				videoName = responseObject["url"];
				currentAspectRatio = responseObject ["aspect_ratio"];
				Debug.Log ("hello video name is" + videoName);
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
			if(_qrid != barCodeValue && barCodeValue.Substring(0,4) == "ARGO") {
				_qrid = barCodeValue;
				GameObject.Find("DisplayLog").GetComponent<Text>().text = barCodeValue;
				CoroutineWithData cd = new CoroutineWithData(this, checkURL(barCodeValue));
			} else {
				
			}
			RestartTime += 1f;
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

		String[] videoInfoArray = path.Split ('|');
		//reattatch amazon client
		UnityInitializer.AttachToGameObject(this.gameObject);

		//get image target 
		VideoPlayer videoPreview = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();

		//prepare path name for movie preview
		string newPath = videoInfoArray[0].Replace ("file:///", "");


		Debug.Log (videoInfoArray [1]);
		string aspectRatioString = videoInfoArray [1];
		currentAspectRatio = float.Parse (aspectRatioString);
		aspectRatio = aspectRatioString;

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
		list.GetComponent<RectTransform> ().sizeDelta = new Vector2 (100, 104 * contacts.Count);
		button.GetComponent<Button> ().onClick.AddListener (() => {
			sendTo (contactArray[1]);
		});
	}

	public void PickerDidCancel(string cancel) {
		GameObject.Find ("LoadingPanel").SetActive (false);
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

			RestartTime += Time.realtimeSinceStartup + 1f;
		};
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
			GameObject.Find("DisplayLog").GetComponent<Text>().text = ArgoResult["response"];
			yield return @"error";
		}
		yield return ArgoResult["response"];
		currentAspectRatio = ArgoResult ["response"]["aspect_ratio"];
		GameObject.Find("DisplayLog").GetComponent<Text>().text = ArgoResult["response"];
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
		yield return ArgoResult;
		GameObject.Find ("LoadingPanel").SetActive(false);
		yield return StartCoroutine (
			StartVuforia ()
		);
		StartCoroutine (
			SaveThumbnailToS3()
		);

	}



	/////////////////////////////////VUFORIA CONFIGURATION AND ACTIVATION
	public IEnumerator StartVuforia() {
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
		GameObject videoPlane = GameObject.Find ("Plane");
		Vector3 aspectRatioVector = new Vector3 (0.16F, (0.16F * currentAspectRatio), (0.16F * currentAspectRatio));
		videoPlane.transform.localScale = aspectRatioVector;

		GameObject.Find("RawImage").SetActive(false);

		StartCoroutine(StopCamera(() => {
			
		}));


		string bucket = "https://s3-us-west-2.amazonaws.com/eyecueargo/";
		string videoName2 = videoName.Replace ("\"", "");
		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = bucket + videoName2;
		//set video url to value of qr code
		if(player.url != bucket + videoName2) {
			player.url = bucket + videoName2;
		}

		yield return null;

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

	public void SetContact(){
		contact = recipient;
		Debug.Log ("hello from set contact");
		Debug.Log (recipient);
	}

	public void SetPublicContact() {
		contact = "public";
	}

	public IEnumerator SendMessage(PostObjectRequest request){
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
		//////above code from RequireLogin
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

		yield return StartCoroutine (next);

	}

	public IEnumerator SaveThumbnailToS3() {
		
		Debug.Log ("--------------url set---------------");
		yield return new WaitForSeconds(1);
		VideoPlayer player = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();
		player.Play ();
		Debug.Log ("--------------player played---------------");
		yield return new WaitForSeconds(1);
		while (player.texture == null) {
			yield return null;
		}

		Texture texture = player.texture;
		Debug.Log ("--------------texture got1---------------");
		RenderTexture Rtexture = texture as RenderTexture;
		Debug.Log ("--------------texture got2---------------");
		RenderTexture.active = Rtexture;
		Debug.Log ("--------------texture got3---------------");
		Texture2D myTexture2d = new Texture2D (texture.width, texture.height);
		Debug.Log ("--------------texture got4---------------");
		myTexture2d.ReadPixels (new Rect (0, 0, Rtexture.width, Rtexture.height), 0, 0);
		Debug.Log ("--------------texture got5---------------");
		myTexture2d.Apply ();
		Debug.Log ("--------------texture got6---------------");
		RenderTexture.active = null;
		Debug.Log ("--------------texture got7---------------");


		IntPtr pointer = texture.GetNativeTexturePtr ();
		Debug.Log ("--------------pointer got8---------------");

		//
		////		Color[] pixels = texture2.GetPixels(0, 0, texture.width, texture.height, 0);
		////		Texture2D preThumbnail = new Texture2D (texture.width, texture.height);
		////		preThumbnail.SetPixels (pixels);
		//		TextureImporter importer = new TextureImporter();
		//		importer.isReadable = true;
		//		Texture2D texture2 = Texture2D.CreateExternalTexture (texture.width, texture.height, TextureFormat.Alpha8, false, false, pointer);


		//		byte[] thumbnail = texture2.EncodeToPNG();
		byte[] thumbnail = myTexture2d.EncodeToPNG();
		Debug.Log ("-------------thumbnail got---------------");
		player.Stop ();
		Debug.Log ("--------------player stop---------------");
		MemoryStream stream = new MemoryStream(thumbnail);

		Debug.Log ("--------------stream streamed---------------");
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
		Debug.Log ("--------------request configured---------------");

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
					Debug.Log ("--------------did not post---------------");
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



	public IEnumerator StopCamera(Action callback)
	{
		// Stop Scanning
		Image = null;
		BarcodeScanner.Destroy();
		BarcodeScanner = null;

		// Wait a bit
		yield return new WaitForSeconds(0.1f);

		callback.Invoke();
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