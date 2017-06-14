using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using Vuforia;

public class FileUpDownTestScript : MonoBehaviour {

	public string url;
	public GameObject ImageTarget;
	public GameObject ARCamera;

	private WWW query;
	private WWW Media;

//	void Awake(){
//		Media = new WWW (url);
//	}

	private void GetTempPath(){
		Debug.Log ("IS IT DOING ANYTHING?");


		DirectoryInfo info = new DirectoryInfo(Application.temporaryCachePath);
		Debug.Log (info);

		if (info != null) {
			File.WriteAllBytes(Application.temporaryCachePath + "/Message.mp4", Media.bytes);

			var fileInfo = info.GetFiles ();
			foreach (var file in fileInfo) {
				Debug.Log (file);
				Debug.Log ("YES, ITS DOING SOMETHING");
			}
			Debug.Log ("File written");
		} else {
			Debug.Log ("INCORRECT PATH");
		}

	}

	void WWWStatus(){
		Debug.Log ("Download Status\n" + "isDone: " + Media.isDone + "\n Progress: " + Media.progress + "\n bytesDownloaded: " + Media.bytesDownloaded);
		if (Media.isDone) {
			CancelInvoke ();
		}
	}

	IEnumerator storeData(){
		File.WriteAllBytes(Application.temporaryCachePath + "/Message.mp4", Media.bytes);
		yield return null;
	}

	// Use this for initialization
	IEnumerator Start () {
		Debug.Log ("Making query...");

		query = new WWW (url);

		yield return query;

		Debug.Log ("Loading Media...");
		Debug.Log (query.text);
		Media = new WWW (query.text);
//		Media = new WWW ("http://localhost:3000/media/1");
		InvokeRepeating ("WWWStatus", 0.5f, 0.5f);//monitor status of download of media object
//		GetTempPath ();
		yield return Media;

		yield return StartCoroutine(storeData ());
//		yield return new WaitUntil (()=> );
//		yield return new WaitForSeconds(2f);

		if (File.Exists (Application.temporaryCachePath + "/Message.mp4")) {

			VideoPlayer player = ImageTarget.GetComponent<VideoPlayer> ();
//		player.source = VideoClip;
			player.url = Application.temporaryCachePath + "/Message.mp4";

			ARCamera.GetComponent<VuforiaBehaviour> ().enabled = true;

			ImageTarget.GetComponent<ImageTargetPlayAudio> ().enabled = true;
			//https://s3-us-west-2.amazonaws.com/eyecueargo/giveYouUp.mp4
			this.enabled = false;
		}


//
//		Debug.Log (url);

	}
	
	// Update is called once per frame
	void Update () {
//		if (Media != null) {
//			Debug.Log ("Download Status\n" + "isDone: " + Media.isDone + "\n Progress: " + Media.progress + "\n bytesDownloaded: " + Media.bytesDownloaded);
//
//		}
	}
}
