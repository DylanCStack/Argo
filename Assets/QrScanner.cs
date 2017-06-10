using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Vuforia;
using BarcodeScanner;
using BarcodeScanner.Scanner;

public class QrScanner : MonoBehaviour {

	// Global Variables
	private String QRData;
	public String videoURL;
	private int repeatCount = 0;
	private IScanner BarcodeScanner;
	public Text TextHeader;
	public RawImage Image;
	public AudioSource Audio;
	private float RestartTime;

	// Disable Screen Rotation on that screen
	void Awake()
	{
		Screen.autorotateToPortrait = false;
		Screen.autorotateToPortraitUpsideDown = false;
	}

	// Use this for initialization
	void Start () {

		// Create a basic scanner
		BarcodeScanner = new Scanner();
		BarcodeScanner.Camera.Play();

		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
			// Set Orientation & Texture
			Image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles();
			Image.transform.localScale = BarcodeScanner.Camera.GetScale();
			Image.texture = BarcodeScanner.Camera.Texture;

			// Keep Image Aspect Ratio
			var rect = Image.GetComponent<RectTransform>();
			var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
			rect.sizeDelta = new Vector2(rect.sizeDelta.x, newHeight);

			RestartTime = Time.realtimeSinceStartup;
		};
	}
	
//	void QRScan () {
//
//
//
//
//		try {
//
//			IBarcodeReader barcodeReader = new BarcodeReader ();
//			var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
//			/////Display Text on Iphone
//			GameObject DisplayLog = GameObject.Find("DisplayLog");
////			DisplayLog.GetComponent<Text>().text = result.Text;
//
//			if (result != null) {
//				QRData = result.Text;
//				Debug.Log("DECODED TEXT FROM QR: " + result.Text);
//
//				////Display Text on Screen
//				DisplayLog.GetComponent<Text>().text = QRData;
//
//				/////Assign VideoClip to VideoPLayer
//				GameObject ARScanner = GameObject.Find("ImageTarget");
//				VideoPlayer player = ARScanner.GetComponent<VideoPlayer>();
//				ARScanner.GetComponent<ImageTargetPlayAudio>().enabled = true;
//				player.url = QRData;
//				CancelInvoke();
//
//			}
//
//		} catch(Exception ex) { 
//			Debug.LogWarning (ex.Message); 
//			/////Display Text on Iphone
//			GameObject DisplayLog = GameObject.Find("DisplayLog");
//			DisplayLog.GetComponent<Text>().text = "Error message"+ repeatCount++ + ex.Message;
////			CancelInvoke ();
//		
//		}
			

//	}

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

			GameObject DisplayLog = GameObject.Find("DisplayLog");
			DisplayLog.GetComponent<Text>().text = TextHeader.text;

			// Feedback
			Audio.Play();

			#if UNITY_ANDROID || UNITY_IOS
			Handheld.Vibrate();
			#endif
		});
	}
}
