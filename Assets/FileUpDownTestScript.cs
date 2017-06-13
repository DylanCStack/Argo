using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileUpDownTestScript : MonoBehaviour {

	public string url;

	public WWW Media;

//	void Awake(){
//		Media = new WWW (url);
//	}

//	IEnumerator GetMedia(){
//
//
//
//
//	}

	void WWWStatus(){
		Debug.Log ("Download Status\n" + "isDone: " + Media.isDone + "\n Progress: " + Media.progress + "\n bytesDownloaded: " + Media.bytesDownloaded);
		if (Media.isDone) {
			CancelInvoke ();
		}
	}

	// Use this for initialization
	IEnumerator Start () {
		Debug.Log ("Media loading...");

		Media = new WWW (url);
//		InvokeRepeating ("WWWStatus", 0.5f, 0.5f);//monitor status of download of media object

		yield return Media;
		Debug.Log ("Media loaded");

			
//		var info = new DirectoryInfo(Application.temporaryCachePath);
//		var fileInfo = info.GetFiles();
//		foreach ( var file in fileInfo)  Debug.Log(file);
//
//
//		Debug.Log (url);

		//		File.WriteAllBytes(Application.temporaryCachePath + "/Message.mp4", Media.bytes);
	}
	
	// Update is called once per frame
	void Update () {
//		if (Media != null) {
//			Debug.Log ("Download Status\n" + "isDone: " + Media.isDone + "\n Progress: " + Media.progress + "\n bytesDownloaded: " + Media.bytesDownloaded);
//
//		}
	}
}
