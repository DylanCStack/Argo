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
//namespace iPhoneArgoAPI
//{
//
//	public class ArgoAPI
//	{
////		public static GameObject currentGameObject = GameObject.Find("ComposeButton");
//		private static BasicAWSCredentials Credentials = new BasicAWSCredentials("AKIAJQZXKUPR47AXETNA","GFyHFW/vjbGpV7Ek4Fr6A79UoitF0m0DCZmlCifc");
//		private static IAmazonS3 _s3Client;
//		private static string S3BucketName = "arn:aws:s3:::eyecueargo";
//
//		public static string S3Region = RegionEndpoint.USWest2.SystemName;
//		private static RegionEndpoint _S3Region
//		{
//			get { return RegionEndpoint.GetBySystemName(S3Region); }
//		}
//
//		//Client constructor
//		private static IAmazonS3 Client
//		{
//			get
//			{
//				if (_s3Client == null)
//				{
//					_s3Client = new AmazonS3Client(Credentials, _S3Region);
//				}
//				return _s3Client;
//			}
//		}
//
//		//Post a video to S3
//		public static void PostObject(string filePath)
//		{
////			GameObject.Find("DisplayLog").GetComponent<Text>().text = "posting object";
//			//Adjust file name to be more readable
//			string fileName = filePath.Replace ("/", "");
//			string fileName2 = fileName.Replace ("tmptrim.", "");
//			string fileName3 = fileName2.Replace (".MOV", ".mov");
//			string fileName4 = fileName3.Replace("privatevarmobileContainersDataApplication", "");
//			string fileName5 = fileName4.Substring (fileName4.Length - 12);
//			string fileName6 = SystemInfo.deviceUniqueIdentifier + "-" + fileName5;
//
//			//prepare file for upload
//			var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
//
//			//prepares request to amazon
//			var request = new PostObjectRequest()
//			{
//				Bucket = S3BucketName,
//				Key = fileName6,
//				InputStream = stream,
//				CannedACL = S3CannedACL.Private,
//				Region = _S3Region
//			};
////			GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "request and bucket prepared";
//
//			//		GameObject.Find ("LoadingPanel").SetActive (true);
//
//
////			GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "loading panel turned on";
//			//Post to s3 using current client
//			Client.PostObjectAsync(request, (responseObj) =>
//				{
//					if (responseObj.Exception == null)
//					{//successfully posted
//						Debug.Log(string.Format("\nobject {0} posted to bucket {1}", responseObj.Request.Key, responseObj.Request.Bucket));
//						//Post record to ArgoDB
//						PostVideoToArgoDB (fileName6, "public", "noone");
////						GameObject.Find("DisplayLog").GetComponent<Text>().text = "posted and started co-routine";
//					}
//					else
//					{//did not post
////						GameObject.Find("DisplayLog").GetComponent<Text>().text = "did not post to amazon";
//						Debug.Log("\nException while posting the result object");
//						Debug.Log(responseObj.Exception.Message);
//						Debug.Log(responseObj.Exception.Source);
//						//					GameObject.Find ("LoadingPanel").SetActive (false);
//
//					}
//				});
//		}
//
//
//
//		private static void GetObject()
//		{
//			Debug.Log( string.Format("fetching {0} from bucket {1}", "poop2", S3BucketName));
//			Client.GetObjectAsync(S3BucketName, "poop2", (responseObj) =>
//				{
//					string data = null;
//					var response = responseObj.Response;
//					if (response.ResponseStream != null)
//					{
//						using (StreamReader reader = new StreamReader(response.ResponseStream))
//						{
//							data = reader.ReadToEnd();
//						}
//
//						Debug.Log(data);
//					}
//				});
//		}
//
//		private static void GetObjects()
//		{
//			Debug.Log( "Fetching all the Objects from " + S3BucketName);
//
//			var request = new ListObjectsRequest()
//			{
//				BucketName = S3BucketName
//			};
//
//			Client.ListObjectsAsync(request, (responseObject) =>
//				{
//					Debug.Log( "\n");
//					if (responseObject.Exception == null)
//					{
//						Debug.Log("Got Response \nPrinting now \n");
//						responseObject.Response.S3Objects.ForEach((o) =>
//							{
//								Debug.Log( string.Format("{0}\n", o.Key));
//							});
//					}
//					else
//					{
//						Debug.Log("Got Exception \n");
//					}
//				});
//		}
//
//		private static IEnumerator PostVideoToArgoDB(string filename,string privacy,string recipient) {
//
//			WWWForm testForm = new WWWForm();
//			testForm.AddField ("url", filename);
//			testForm.AddField ("privacy", privacy);
//			testForm.AddField ("recipient", recipient);
//			WWW request = new WWW("http://argo-server.herokuapp.com/message/upload", testForm);
////			GameObject.Find ("DisplayLog").GetComponent<Text> ().text = "waiting for text to return " + filename;
//			yield return request;
//			Debug.Log(request.text);
////			GameObject.Find ("DisplayLog").GetComponent<Text> ().text = request.text;
//			//		GameObject.Find ("LoadingPanel").SetActive (false);
//
//		}
//
//
//
//	}
//		
//
//
//
//
//}
