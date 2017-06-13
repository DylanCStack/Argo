using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class qrscanner3 : MonoBehaviour {

	private IScanner BarcodeScanner;
	public Text TextHeader;
	public RawImage Image;
	public AudioSource Audio;
	private float RestartTime;
	private WebCamTexture camTexture;
	private Rect screenRect;
	private bool foundQR = false;

	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	void Start () {

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

//		screenRect = new Rect(0, 0, Screen.width, Screen.height);
//		camTexture = new WebCamTexture();
//		camTexture.requestedHeight = Screen.height; 
//		camTexture.requestedWidth = Screen.width;
//		if (camTexture != null) {
//			camTexture.Play();
//		}
	}

	void OnGUI () {
		// drawing the camera on screen
//		if (!foundQR) {
//			
//			GUI.DrawTexture (screenRect, camTexture, ScaleMode.ScaleAndCrop);
//		}
	}


	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();
			if (TextHeader.text.Length > 250)
			{
				TextHeader.text = "";
			}
			TextHeader.text += "Found: " + barCodeType + " / " + barCodeValue + "\n";
			RestartTime += Time.realtimeSinceStartup + 1f;

			GameObject rawImage = GameObject.Find("RawImage");
			if(rawImage != null){
				rawImage.SetActive(false);
			}
			GameObject arCamera = GameObject.Find("ARCamera");
			arCamera.GetComponent<VuforiaBehaviour>().enabled = true;

			GameObject ARScanner = GameObject.Find("ImageTarget");
			VideoPlayer player = ARScanner.GetComponent<VideoPlayer>();
			ARScanner.GetComponent<ImageTargetPlayAudio>().enabled = true;
			if(player.url != barCodeValue) {
				player.url = barCodeValue;
			}
					foundQR = true;

			// Feedback
			Audio.Play();

			#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
			#endif
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

	#region UI Buttons

	public void ClickBack()
	{
		// Try to stop the camera before loading another scene
		StartCoroutine(StopCamera(() => {
			SceneManager.LoadScene("Boot");
		}));
	}

	/// <summary>
	/// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
	/// Trying to stop the camera in OnDestroy provoke random crash on Android
	/// </summary>
	/// <param name="callback"></param>
	/// <returns></returns>
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

	#endregion
}