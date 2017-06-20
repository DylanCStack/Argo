using BarcodeScanner;
using BarcodeScanner.Scanner;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using Vuforia;

public class qrscanner3 : MonoBehaviour {

	private IScanner BarcodeScanner;
	public RawImage Image;
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
	}
		
	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner()
	{
		BarcodeScanner.Scan((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop();

			RestartTime += Time.realtimeSinceStartup + 1f;

			//turn off raw image displaying camera texture
			if(Image != null){
				Image.enabled = false;
			}

			//enable vuforia camera so it can track objects
			GameObject arCamera = GameObject.Find("ARCamera");
			arCamera.GetComponent<VuforiaBehaviour>().enabled = true;

			//enable the image target so vuforia knows what to track
			GameObject ARScanner = GameObject.Find("ImageTarget");

			//enable the video player on the image target
			VideoPlayer player = ARScanner.GetComponent<VideoPlayer>();
			ARScanner.GetComponent<ImageTargetPlayAudio>().enabled = true;

			//set video url to value of qr code
			if(player.url != barCodeValue) {
				player.url = barCodeValue;
			}
			foundQR = true;

			// Feedback

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
		
}