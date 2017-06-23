using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using Amazon.S3.Util;
using Amazon.CognitoIdentity;
using Amazon;



public class ObjectiveCInterfaceTestScript : MonoBehaviour {



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
	public string S3BucketName = "arn:aws:s3:::eyecueargo";
	private IAmazonS3 _s3Client;
	private BasicAWSCredentials Credentials = new BasicAWSCredentials("AKIAJQZXKUPR47AXETNA","GFyHFW/vjbGpV7Ek4Fr6A79UoitF0m0DCZmlCifc");

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




	void Start () {//initialize AWS and open videopicker

		UnityInitializer.AttachToGameObject(this.gameObject);
//		OpenVideoPicker ("TestObject", "VideoPicked");
//		Debug.Log(SystemInfo.deviceUniqueIdentifier);
//		PostObject("Assets/birthday.webm");
//		GetObject();
//		GetObjects();


	}

	//Post a video to S3
	public void PostObject(string filePath)
	{
		
		//Adjust file name to be more readable
		string fileName = filePath.Replace ("/", "");
		string fileName2 = fileName.Replace ("tmptrim.", "");
		string fileName3 = fileName2.Replace (".MOV", ".mov");
		string fileName4 = fileName3.Replace("privatevarmobileContainersDataApplication", "");
		string fileName5 = fileName4.Substring (fileName4.Length - 12);
		string fileName6 = SystemInfo.deviceUniqueIdentifier + "-" + fileName5;

		//prepare file for upload
		var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

		//prepares request to amazon
		var request = new PostObjectRequest()
		{
			Bucket = S3BucketName,
			Key = fileName4,
			InputStream = stream,
			CannedACL = S3CannedACL.Private,
			Region = _S3Region
		};
				

		//Post to s3 using current client
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
					Debug.Log(responseObj.Exception.Source);

				}
			});
	}
		


	//collect returned information from iOS plugin
	void VideoPicked( string path ){

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


	private void GetObject()
	{
		Debug.Log( string.Format("fetching {0} from bucket {1}", "poop2", S3BucketName));
		Client.GetObjectAsync(S3BucketName, "poop2", (responseObj) =>
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
			


}
