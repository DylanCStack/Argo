using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ZXing;
using ZXing.QrCode;

public class QrScanner : MonoBehaviour {



	private WebCamTexture camTexture;
	private Rect screenRect;
	public String QRData;
	public IBarcodeReader barcodeReader = new BarcodeReader ();

	void Start() {
		screenRect = new Rect(0, 0, Screen.width, Screen.height);
		camTexture = new WebCamTexture();
		camTexture.requestedHeight = Screen.height; 
		camTexture.requestedWidth = Screen.width;
		if (camTexture != null) {
			camTexture.Play();
		}

		InvokeRepeating ("QRScan", 0f, 0.5f); 
	}

	void QRScan () {
		// drawing the camera on screen
//		GUI.DrawTexture (screenRect, camTexture, ScaleMode.ScaleToFit);
		// do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
		try {
			
			// decode the current frame
			var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width , camTexture.height);
			if (result != null) {
				QRData = result.Text;
				Debug.Log("DECODED TEXT FROM QR: " + result.Text);
				CancelInvoke();

			}
		} catch(Exception ex) { Debug.LogWarning (ex.Message); }
	}
}
