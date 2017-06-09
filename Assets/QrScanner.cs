using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using Vuforia;
using ZXing;
using ZXing.QrCode;

public class QrScanner : MonoBehaviour {

	// Global Variables
	private WebCamTexture camTexture;
	private String QRData;
	private IBarcodeReader barcodeReader = new BarcodeReader ();
	public String videoURL;
	//variables set in editor to determine the scanner dimensions
	public double ScannerHeight;
	public double ScannerWidth;

	//pixel dimensions determined by scanner size relative to screen and camera
	private int rectWidth;
	private int rectHeight;

	//texture of scanner. If empty will be a dark grey, translucent box
	public Texture2D texture;

	private Rect guide;

	// Use this for initialization
	void Start () {

		if (texture != null) {//set guide texture if empty
			texture = new Texture2D (1, 1);
			texture.SetPixel (0, 0, new Color (255, 20, 20, 1));
			texture.wrapMode = TextureWrapMode.Repeat;
			texture.Apply ();
		}
			
		camTexture = new WebCamTexture ();
		camTexture.requestedHeight = Screen.height;
		camTexture.requestedWidth = Screen.width;
		if (camTexture != null) {
			camTexture.Play ();
		}

		ScannerHeight = 0.5;
		ScannerWidth = 0.5;

		//		int baseW = (Screen.width < camTexture.width) ? Screen.width : camTexture.width;
		//		int baseh = (Screen.height < camTexture.height) ? Screen.height : camTexture.height;

		rectWidth = (int)(Screen.width * ScannerHeight);
		rectHeight = (int)(Screen.height * ScannerWidth);

		int xStart = (int)((Screen.width * 0.5) - (rectWidth * 0.5));
		int yStart = (int)((Screen.height * 0.5) - (rectHeight * 0.5));

		guide = new Rect (xStart, yStart, rectWidth, rectHeight);

		InvokeRepeating ("QRScan", 0f, 0.5f);// repeatedly runs QRScan every 0.5s starting in 0s
	}
	void OnGUI()
	{
		GUI.Box(guide, texture);//draws guide on screen
	}
	
	void QRScan () {
		// do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
		try {
			//capture frame and slice off all but the center 1/2

			Debug.Log("SCREEN WIDTH/HEIGHT: " + Screen.width + " , " + Screen.height);
			Debug.Log("CAMERA WIDTH/HEIGHT: " + camTexture.width + " , " + camTexture.height);


			int scanWidth = rectWidth;
			int scanHeight = rectHeight;

			int length = scanWidth*scanHeight;

			//			int scanWidth = (int)(Screen.width*0.50);
			//			int scanHeight = (int)(Screen.height*0.50);



			int startIndex = (int)((camTexture.width*0.5)-scanWidth*0.5);//how far into the image the scanner will start
			//			int startIndex = camTexture.width*(int)(camTexture.height*0.25) + (int)(Screen.width*0.25);

			//			int startIndex = 0;
			//			int length = Screen.width*Screen.height;

			Color32[] color32Pixels = new Color32[length];
			Color32[] FullColor32Pixels = camTexture.GetPixels32();

			//for every row of pixels to be scanned, copy that row from full pixel list to scan pixel list
			for(int i = 0; i < scanHeight; i++){
				Array.Copy(FullColor32Pixels, startIndex + (i * camTexture.width), color32Pixels, scanWidth*i, scanWidth);
			}

			//check for codes in frame segment
			//			IBarcodeReader barcodeReader = new BarcodeReader ();

			var result = barcodeReader.Decode(color32Pixels, scanWidth , scanHeight);
			//			var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width , camTexture.height);//origional code, scans entire screen at the cost of severely reduced performance

			if (result != null) {
				QRData = result.Text;
				Debug.Log("DECODED TEXT FROM QR: " + result.Text);

				////Display Text on Screen
				GameObject DisplayLog = GameObject.Find("DisplayLog");
				DisplayLog.GetComponent<Text>().text = QRData;

				/////Assign VideoClip to VideoPLayer
				GameObject ARScanner = GameObject.Find("ImageTarget");
				VideoPlayer player = ARScanner.GetComponent<VideoPlayer>();
				ARScanner.GetComponent<ImageTargetPlayAudio>().enabled = true;
				player.url = QRData;
				CancelInvoke();

			}
			else {
				Debug.Log("NO CODE FOUND.");
			}
		} catch(Exception ex) { Debug.LogWarning (ex.Message); }

	}
}

