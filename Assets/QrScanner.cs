using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Vuforia;
using ZXing;
using ZXing.QrCode;

public class QrScanner : MonoBehaviour {

	// Global Variables
	private WebCamTexture camTexture;
	private Rect screenRect;
	private String QRData;
	private IBarcodeReader barcodeReader = new BarcodeReader ();
	public String videoURL;

	// Use this for initialization
	void Start () {
		screenRect = new Rect (0, 0, Screen.width, Screen.height);
		camTexture = new WebCamTexture ();
		camTexture.requestedHeight = Screen.height;
		camTexture.requestedWidth = Screen.width;
		if (camTexture != null) {
			camTexture.Play ();
		}

		InvokeRepeating ("QRScan", 0f, 0.5f);
	}
	
	void QRScan () {

		try {

			var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width, camTexture.height);
			if (result != null) {
				QRData = result.Text;
				Debug.Log("DECODED TEXT FROM QR: " + result.Text);
			

				/////Assign VideoClip to VideoPLayer
				GameObject ARScanner = GameObject.Find("ImageTarget");
				VideoPlayer player = ARScanner.GetComponent<VideoPlayer>();
				ARScanner.GetComponent<ImageTargetPlayAudio>().enabled = true;
				player.url = result.Text;
				CancelInvoke();

			}

		} catch(Exception ex) { Debug.LogWarning (ex.Message); }
			

	}
}
