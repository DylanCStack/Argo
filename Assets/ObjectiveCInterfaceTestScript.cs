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
	public Texture2D shareButtonImage; // Use this for initialization
	[DllImport("__Internal")]
	private static extern void OpenVideoPicker(string game_object_name, string function_name);
	void Start () {
		OpenVideoPicker ("TestObject", "VideoPicked");
	}
	// Update is called once per frame
	void Update () {
	}
//	void OnGUI ()
//	{
//		if (GUILayout.Button (shareButtonImage, GUIStyle.none, GUILayout.Width (128), GUILayout.Height (128))) {
//			OpenVideoPicker( "GameObject", "VideoPicked" );
//		}
//	}

	void VideoPicked( string path ){
		Debug.Log ("---->VideoPicked");
		VideoPlayer testVideo = GameObject.Find ("ImageTarget").GetComponent<VideoPlayer> ();
		testVideo.url = path;
		testVideo.Play ();



	}
}