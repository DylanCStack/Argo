using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Runtime.InteropServices;
using System.IO;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.Runtime;
using System.IO;
using System;
using Amazon.S3.Util;
using System.Collections.Generic;
using Amazon.CognitoIdentity;
using Amazon;
using UnityEngine.Video;


public class ObjectiveCInterfaceTestScript : MonoBehaviour {



	// Image Picker Handler
	public Texture2D shareButtonImage; // Use this for initialization
	[DllImport("__Internal")]
	private static extern void OpenVideoPicker(string game_object_name, string function_name);

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


	void Start () {
//		//Logging configuration for amazon
//		var loggingConfig = AWSConfigs.LoggingConfig;
//		loggingConfig.LogTo = LoggingOptions.UnityLogger;
//		loggingConfig.LogMetrics = true;
//		loggingConfig.LogResponses = ResponseLoggingOption.Always;
//		loggingConfig.LogResponsesSizeLimit = 4096;
//		loggingConfig.LogMetricsFormat = LogMetricsFormatOption.JSON;


		UnityInitializer.AttachToGameObject(this.gameObject);
		OpenVideoPicker ("TestObject", "VideoPicked");
//		PostObject("Assets/birthday.webm");
//		GetObjects();

	}

	private IAmazonS3 _s3Client;
	private AWSCredentials _credentials;

	private AWSCredentials Credentials
	{
		get
		{
			if (_credentials == null)
				_credentials = new CognitoAWSCredentials(IdentityPoolId, RegionEndpoint.USWest2);
			return _credentials;
		}
	}

	private IAmazonS3 Client
	{
		get
		{
			if (_s3Client == null)
			{
				_s3Client = new AmazonS3Client(Credentials, _S3Region);
			}
			//test comment
			return _s3Client;
		}
	}

	public void PostObject(string filePath)
	{

		string fileName = filePath.Replace ("/", "");
		string fileName2 = fileName.Replace ("tmptrim.", "");
		string fileName3 = fileName2.Replace (".MOV", ".mov");
		string fileName4 = fileName3.Replace("privatevarmobileContainersDataApplication", "");

		var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);

		var request = new PostObjectRequest()
		{
			Bucket = S3BucketName,
			Key = fileName4,
			InputStream = stream,
			CannedACL = S3CannedACL.PublicRead,
			Region = _S3Region
		};



		Client.PostObjectAsync(request, (responseObj) =>
			{
				if (responseObj.Exception == null)
				{
					Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
				}
				else
				{
					Debug.Log("\nException while posting the result object");
					Debug.Log(responseObj.Exception);
				}
			});
	}

	public void GetObjects()
	{
		Debug.Log (CognitoIdentityRegion);
		Debug.Log (_CognitoIdentityRegion);
		Debug.Log(Client.Config.RegionEndpoint);
		Debug.Log("Fetching all the Objects from " + S3BucketName);

		var request = new ListObjectsRequest()
		{
			BucketName = S3BucketName
		};

		Client.ListObjectsAsync(request, (responseObject) =>
			{

				if (responseObject.Exception == null)
				{
					Debug.Log("Got Response \nPrinting now \n");
					responseObject.Response.S3Objects.ForEach((o) =>
						{
							Debug.Log(string.Format("{0}\n", o.Key));
						});
				}
				else
				{
					Debug.Log(responseObject.Exception);
				}
			});
	}




	void VideoPicked( string path ){
		Debug.Log ("---->VideoPicked");
		VideoPlayer testVideo = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();
		string newPath = path.Replace ("file:///", "");
		testVideo.url = newPath;
		testVideo.Play ();
		PostObject (newPath);



	}


	//amazon helper methods
			private string GetFileHelper()
			{
				var fileName = "nadda";

				if (!File.Exists(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName))
				{
					var streamReader = File.CreateText(Application.persistentDataPath + Path.DirectorySeparatorChar + fileName);
					streamReader.WriteLine("This is a sample s3 file uploaded from unity s3 sample");
					streamReader.Close();
				}
				return fileName;
			}

			private string GetPostPolicy(string bucketName, string key, string contentType)
			{
				bucketName = bucketName.Trim();

				key = key.Trim();
				// uploadFileName cannot start with /
				if (!string.IsNullOrEmpty(key) && key[0] == '/')
				{
					throw new ArgumentException("uploadFileName cannot start with / ");
				}

				contentType = contentType.Trim();

				if (string.IsNullOrEmpty(bucketName))
				{
					throw new ArgumentException("bucketName cannot be null or empty. It's required to build post policy");
				}
				if (string.IsNullOrEmpty(key))
				{
					throw new ArgumentException("uploadFileName cannot be null or empty. It's required to build post policy");
				}
				if (string.IsNullOrEmpty(contentType))
				{
					throw new ArgumentException("contentType cannot be null or empty. It's required to build post policy");
				}

				string policyString = null;
				int position = key.LastIndexOf('/');
				if (position == -1)
				{
					policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
						bucketName + "\"},[\"starts-with\", \"$key\", \"" + "\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
				}
				else
				{
					policyString = "{\"expiration\": \"" + DateTime.UtcNow.AddHours(24).ToString("yyyy-MM-ddTHH:mm:ssZ") + "\",\"conditions\": [{\"bucket\": \"" +
						bucketName + "\"},[\"starts-with\", \"$key\", \"" + key.Substring(0, position) + "/\"],{\"acl\": \"private\"},[\"eq\", \"$Content-Type\", " + "\"" + contentType + "\"" + "]]}";
				}

				return policyString;
			}


}
