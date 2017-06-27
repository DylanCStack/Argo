//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Video;
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Runtime.InteropServices;
//using System.IO;
//using Amazon.S3;
//using Amazon.S3.Model;
//using Amazon.Runtime;
//using Amazon.S3.Util;
//using Amazon.CognitoIdentity;
//using Amazon;
//
//
//
//public class ObjectiveCInterfaceTestScript : MonoBehaviour {
//
//
//
//	// Check if running on iphone
//	#if UNITY_IOS
//
//		//import videopicker method from custom iOS plugin
//		[DllImport("__Internal")]
//		private static extern void OpenVideoPicker(string game_object_name, string function_name);
//
//	#endif
//
//	//Amazon config
//	private string IdentityPoolId = "us-west-2:bdbe6639-8a19-4315-b0ad-294039672635";
//	private string CognitoIdentityRegion = RegionEndpoint.USWest2.SystemName;
//	private RegionEndpoint _CognitoIdentityRegion
//	{
//		get { return RegionEndpoint.GetBySystemName(CognitoIdentityRegion); }
//	}
//	private string S3Region = RegionEndpoint.USWest2.SystemName;
//	private RegionEndpoint _S3Region
//	{
//		get { return RegionEndpoint.GetBySystemName(S3Region); }
//	}
//	private string S3BucketName = "arn:aws:s3:::eyecueargo";
//	private IAmazonS3 _s3Client;
//	private BasicAWSCredentials Credentials = new BasicAWSCredentials("AKIAJQZXKUPR47AXETNA","GFyHFW/vjbGpV7Ek4Fr6A79UoitF0m0DCZmlCifc");
//
////	//AWSCredential constructor
////	private AWSCredentials Credentials
////	{
////		get
////		{
////			if (_credentials == null)
////				_credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.USWest2);
////			return _credentials;
////		}
////	}
//
//	//Client constructor
//	private IAmazonS3 Client
//	{
//		get
//		{
//			if (_s3Client == null)
//			{
//				_s3Client = new AmazonS3Client(Credentials, _S3Region);
//			}
//			return _s3Client;
//		}
//	}
//
//
//
//
//	void Start () {//initialize AWS and open videopicker
//
//		UnityInitializer.AttachToGameObject(this.gameObject);
//
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "started";
////		OpenVideoPicker ("ComposeButton", "VideoPicked");
////		Debug.Log(SystemInfo.deviceUniqueIdentifier);
////		PostObject("Assets/birthday.webm");
////		GetObject();
//		GetObjects();
//
//
//	}
//
//	public void _OpenVideoPicker() {
//		Debug.Log("hello from _OpenVideoPicker");
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "opened video picker";
//		OpenVideoPicker ("ComposeButton", "VideoPicked");
//	}
//
//	//Post a video to S3
//	public void PostObject(string filePath)
//	{
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "posting object";
//		//Adjust file name to be more readable
//		string fileName = filePath.Replace ("/", "");
//		string fileName2 = fileName.Replace ("tmptrim.", "");
//		string fileName3 = fileName2.Replace (".MOV", ".mov");
//		string fileName4 = fileName3.Replace("privatevarmobileContainersDataApplication", "");
//		string fileName5 = fileName4.Substring (fileName4.Length - 12);
//		string fileName6 = SystemInfo.deviceUniqueIdentifier + "-" + fileName5;
//
//		//prepare file for upload
//		var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
//
//		Debug.Log (S3BucketName);
//		//prepares request to amazon
//		var request = new PostObjectRequest()
//		{
//			Bucket = S3BucketName,
//			Key = fileName6,
//			InputStream = stream,
//			CannedACL = S3CannedACL.Private,
//			Region = _S3Region
//		};
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "request and bucket prepared";
//
////		GameObject.Find ("LoadingPanel").SetActive (true);
//
//
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "loading panel turned on";
//		//Post to s3 using current client
//		Client.PostObjectAsync(request, (responseObj) =>
//			{
//				if (responseObj.Exception == null)
//				{//successfully posted
//					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
//					//Post record to ArgoDB
//					StartCoroutine (
//						PostToArgoDB (fileName6, "public", "noone")
//					);
//					GameObject.Find("DisplayLog").GetComponent<Text>().text = "posted and started co-routine";
//				}
//				else
//				{//did not post
////					GameObject.Find("DisplayLog").GetComponent<Text>().text = "did not post to amazon";
//					Debug.Log("\nException while posting the result object");
//					Debug.Log(responseObj.Exception.Message);
//					Debug.Log(responseObj.Exception.Source);
////					GameObject.Find ("LoadingPanel").SetActive (false);
//
//				}
//			});
//	}
//
//
//
//	//collect returned information from iOS plugin
//	void VideoPicked( string path ){
//		GameObject.Find("DisplayLog").GetComponent<Text>().text = "video picked " + path;
//
//		UnityInitializer.AttachToGameObject(this.gameObject);
//
//		//get image target
//		VideoPlayer videoPreview = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();
//
//		//prepare path name for movie preview
//		string newPath = path.Replace ("file:///", "");
//
//		//assign video to imagetarget
//		videoPreview.url = newPath;
//		videoPreview.Play ();
//
//		//Post video to S3
//		PostObject (newPath);
//
//	}
//
//
//	private void GetObject()
//	{
//		Debug.Log( string.Format("fetching {0} from bucket {1}", "poop2", S3BucketName));
//		Client.GetObjectAsync(S3BucketName, "poop2", (responseObj) =>
//			{
//				string data = null;
//				var response = responseObj.Response;
//				if (response.ResponseStream != null)
//				{
//					using (StreamReader reader = new StreamReader(response.ResponseStream))
//					{
//						data = reader.ReadToEnd();
//					}
//
//					Debug.Log(data);
//				}
//			});
//	}
//
//	public void GetObjects()
//	{
//		Debug.Log( "Fetching all the Objects from " + S3BucketName);
//
//		var request = new ListObjectsRequest()
//		{
//			BucketName = S3BucketName
//		};
//		Debug.Log (S3BucketName);
//		Client.ListObjectsAsync(request, (responseObject) =>
//			{
//				Debug.Log( "\n");
//				if (responseObject.Exception == null)
//				{
//					Debug.Log("Got Response \nPrinting now \n");
//					responseObject.Response.S3Objects.ForEach((o) =>
//						{
//							Debug.Log( string.Format("{0}\n", o.Key));
//						});
//				}
//				else
//				{
//					Debug.Log("Got Exception \n");
//				}
//			});
//	}
//
//	public IEnumerator PostToArgoDB(string filename,string privacy,string recipient) {
//
//		WWWForm testForm = new WWWForm();
//		testForm.AddField ("url", filename);
//		testForm.AddField ("privacy", privacy);
//		testForm.AddField ("recipient", recipient);
//		WWW request = new WWW("https://argo-server.herokuapp.com/message/upload", testForm);
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "waiting for text to return " + filename;
//		yield return request;
//		Debug.Log(request.text);
//		GameObject.Find ("DisplayLog").GetComponent<Text> ().text = request.text;
////		GameObject.Find ("LoadingPanel").SetActive (false);
//
//	}
//
//
//
//}
