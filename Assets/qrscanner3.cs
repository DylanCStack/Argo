using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;
using System.Runtime.InteropServices;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Util;
using Amazon.CognitoIdentity;
using Amazon;

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

public class qrscanner3 : MonoBehaviour {

	private IScanner BarcodeScanner;
	public RawImage Image;
	private float RestartTime;
	private WebCamTexture camTexture;
	private Rect screenRect;
	private bool foundQR = false;
	private string _qrid;
	private string videoURL;
	private string currentVideoName;
	/////////////////////////////////////////////////////////////////iOS PLUGIN AND AMAZON COMMUNICATIONS
	// Check if running on iphone
	#if UNITY_IOS

	//import videopicker method from custom iOS plugin
	[DllImport("__Internal")]
	private static extern void OpenVideoPicker(string game_object_name, string function_name);

	#endif

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
	private BasicAWSCredentials Credentials = new BasicAWSCredentials("AKIAIWRQUPSW4SCCLD4Q","lDX+bLTeY9xdofU0kEytq66GbpjDUCRiYF58ObNG");

	//	//AWSCredential constructor
	//	private AWSCredentials Credentials
	//	{
	//		get
	//		{
	//			if (_credentials == null)
	//				_credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.USWest2);
	//			return _credentials;
	//		}
	//	}

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


	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start () {
		
		//attach amazon stuff to this object
		UnityInitializer.AttachToGameObject(this.gameObject);
//		PostObject ("Assets/birthday.webm");
//		GetObjects();
//		GetBucketList();
		GameObject.Find("DisplayLog").GetComponent<Text>().text = "started";

		// Create a basic scanner
		BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();
		Debug.Log(BarcodeScanner.Camera);

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
		
	void OnEnable() {

		GameObject.Find("DisplayLog").GetComponent<Text>().text = "started";
//		_qrid = "";
//		videoURL = "";
//		currentVideoName = "";
//
//		BarcodeScanner.Stop ();
		BarcodeScanner.Camera.Play();
		Debug.Log(BarcodeScanner.Camera);
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "";
//		FindObject(GameObject.Find ("HomeScreenPanel"), "RawImage").SetActive(true);
//		GameObject.Find ("RawImage").GetComponent<RawImage> ().enabled = false;
//		Image = GameObject.Find ("RawImage").GetComponent<RawImage> ();
//
//		// Display the camera texture through a RawImage
//		BarcodeScanner.OnReady += (sender, arg) => {
//			// Set Orientation & Texture
//			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
//			Image.transform.localScale = BarcodeScanner.Camera.GetScale();
//			Image.texture = BarcodeScanner.Camera.Texture;
//
//			// Keep Image Aspect Ratio
//			var rect = Image.GetComponent<RectTransform>();
//			var newWidth = rect.sizeDelta.y * BarcodeScanner.Camera.Width / BarcodeScanner.Camera.Height;
//			rect.sizeDelta = new Vector2(newWidth, rect.sizeDelta.y);
//
//			RestartTime = 0;
//
//		};
//		StartScanner ();

	}
	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();
			PlayerPrefs.SetString("qrid",barCodeValue);
			if(_qrid != barCodeValue) {
				_qrid = barCodeValue;
				Debug.Log("hello from barcode found");
				GameObject.Find("DisplayLog").GetComponent<Text>().text = barCodeValue;
				CoroutineWithData cd = new CoroutineWithData(this, checkURL(barCodeValue));
			} else {

			}
			RestartTime += Time.realtimeSinceStartup + 1f;
		});
	}

	/// <summary>
	/// The Update method from unity need to be propagated
	/// </summary>
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


	public IEnumerator CheckArgoDB(string qrid) {
		WWWForm form = new WWWForm();
		form.AddField ("qrid", qrid);
		WWW request = new WWW ("https://argo-server.herokuapp.com/message/isRegistered", form);
		yield return request;
		yield return request.text;
	}

	public IEnumerator checkURL(string barCodeValue) {
		CoroutineWithData cd = new CoroutineWithData(this, CheckArgoDB(barCodeValue));
		yield return cd.coroutine;

		Debug.Log("hello from checkURL)");
		Debug.Log (cd.result);
		_qrid = barCodeValue;

		Debug.Log (_qrid + "from 130");

		if (cd.result.ToString() == "false") {	//go to image picker
			Debug.Log(cd.result);
			_OpenVideoPicker ();

		} else {//set video to returned url

			//turn off raw image displaying camera texture
			if(Image != null){
				Image.enabled = false;
			}

			videoURL = cd.result.ToString();
			StartVuforia ();
			// Feedback

			#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
			#endif


		}
	}


//	void Start () {//initialize AWS and open videopicker
//
//
//		//		OpenVideoPicker ("ComposeButton", "VideoPicked");
//		//		Debug.Log(SystemInfo.deviceUniqueIdentifier);
//		//		PostObject("Assets/birthday.webm");
//		//		GetObject();
//		//		GetObjects();
//
//
//	}

	public void _OpenVideoPicker() {
		Debug.Log("hello from _OpenVideoPicker");
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "opened video picker";
		Debug.Log (_qrid + "from 240");
		PlayerPrefs.SetString ("qrid", _qrid);
		OpenVideoPicker ("ARCamera", "VideoPicked");
	}

	//Post a video to S3
	public void PostObject(string filePath)
	{
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "posting object";
		//Adjust file name to be more readable
		string fileName = filePath.Replace ("/", "");
		string fileName2 = fileName.Replace ("tmptrim.", "");
		string fileName3 = fileName2.Replace (".MOV", ".mov");
		string fileName4 = fileName3.Replace("privatevarmobileContainersDataApplication", "");
		string fileName5 = fileName4.Substring (fileName4.Length - 12);
		string fileName6 = SystemInfo.deviceUniqueIdentifier + "-" + fileName5;

		videoURL = fileName6;
		//prepare file for upload
		var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

		//prepares request to amazon
		var request = new PostObjectRequest()
		{
			Bucket = S3BucketName,
			Key = fileName6,
			InputStream = stream,
			CannedACL = S3CannedACL.Private,
			Region = _S3Region
		};
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "request and bucket prepared";

		//		GameObject.Find ("LoadingPanel").SetActive (true);


//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "loading panel turned on";
		Debug.Log (_qrid + "from 317");
		//Post to s3 using current client
		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{//successfully posted
					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
					Debug.Log (_qrid + "from 281");
					//Post record to ArgoDB
					StartCoroutine (
						PostToArgoDB (fileName6, "public", "noone")
					);
//					GameObject.Find("DisplayLog").GetComponent<Text>().text = "posted and started co-routine";
				}
				else
				{//did not post
					//					GameObject.Find("DisplayLog").GetComponent<Text>().text = "did not post to amazon";
					Debug.Log("\nException while posting the result object");
					Debug.Log(responseObj.Exception.Message);
					Debug.Log(responseObj.Exception.Source);
					//					GameObject.Find ("LoadingPanel").SetActive (false);

				}
			});
	}



	//collect returned information from iOS plugin
	void VideoPicked( string path ){
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "video picked " + path;

		UnityInitializer.AttachToGameObject(this.gameObject);

		//get image target 
		VideoPlayer videoPreview = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();

		//prepare path name for movie preview
		string newPath = path.Replace ("file:///", "");

		//assign video to imagetarget
		videoPreview.url = newPath;
		videoPreview.Play ();
		Debug.Log (_qrid + "from 317");
		//Post video to S3
		PostObject (newPath);

	}


	private void GetObject()
	{
		Debug.Log( string.Format("fetching {0} from bucket {1}", "poop2", S3BucketName));
		Client.GetObjectAsync(S3BucketName, "giveYouUp.mp4", (responseObj) =>
			{
				string data = null;
				var response = responseObj.Response;
				if (response.ResponseStream != null)
				{
					using (StreamReader reader = new StreamReader(response.ResponseStream))
					{
						data = reader.ReadToEnd();
					}

					Debug.Log(data);
				}
			});
	}

	public void GetObjects()
	{
		Debug.Log( "Fetching all the Objects from " + S3BucketName);

		var request = new ListObjectsRequest()
		{
			BucketName = S3BucketName
		};

		Client.ListObjectsAsync(request, (responseObject) =>
			{
				Debug.Log( "\n");
				if (responseObject.Exception == null)
				{
					Debug.Log("Got Response \nPrinting now \n");
					responseObject.Response.S3Objects.ForEach((o) =>
						{
							Debug.Log( string.Format("{0}\n", o.Key));
						});
				}
				else
				{
					Debug.Log("Got Exception \n");
				}
			});
	}

	public IEnumerator PostToArgoDB(string filename,string privacy,string recipient) {
		Debug.Log (_qrid);
		Debug.Log (_qrid);
		WWWForm testForm = new WWWForm();
		testForm.AddField ("url", filename);
		testForm.AddField ("privacy", privacy);
		testForm.AddField ("recipient", recipient);
		testForm.AddField ("qrid",GameObject.Find("DisplayLog").GetComponent<Text>().text);
		WWW request = new WWW("https://argo-server.herokuapp.com/message/upload", testForm);
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "waiting for text to return " + filename;
		yield return request;
		Debug.Log(request.text);
		StartVuforia();

//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = request.text;
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = PlayerPrefs.GetString ("qrid");
		//		GameObject.Find ("LoadingPanel").SetActive (false);

	}


	public void GetBucketList()
	{
		Debug.Log( "Fetching all the Buckets");
		Client.ListBucketsAsync(new ListBucketsRequest(), (responseObject) =>
			{
				if (responseObject.Exception == null)
				{
					Debug.Log("Got Response \nPrinting now \n");
					responseObject.Response.Buckets.ForEach((s3b) =>
						{
							Debug.Log(string.Format("bucket = {0}, created date = {1} \n", s3b.BucketName, s3b.CreationDate));
						});
				}
				else
				{
					Debug.Log( "Got Exception \n");
				}
			});
	}

	public void StartVuforia() {
		//enable vuforia camera so it can track objects
		FindObject(this.collider.transform.root.gameObject,"ARCamera");
		GameObject arCamera = GameObject.Find("ARCamera");
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
		if(player.url != bucket + videoURL) {
			player.url = bucket + videoURL;
		}
		foundQR = true;
		Update ();
	}

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
		
}

