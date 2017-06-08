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

	public int x;
	public int y;
	public int width;
	public int height;

	public Texture2D texture;



	void Start() {

		texture = new Texture2D(1,1);
		texture.SetPixel(0,0, new Color(255,20,20,1));
		texture.wrapMode = TextureWrapMode.Repeat;
		texture.Apply ();


		////////////////////////////////////////////
		GameObject guide = GameObject.Find ("QRScanGuide");
		RectTransform guideRect = guide.GetComponent<RectTransform> ();



//		screenRect = new Rect(0, 0, Screen.width, Screen.height);
		camTexture = new WebCamTexture();
		camTexture.requestedHeight = Screen.height; 
		camTexture.requestedWidth = Screen.width;

		if (camTexture != null) {
			camTexture.Play();
		}


		Vector2 gmin = guideRect.anchorMin;
		Vector2 gmax = guideRect.anchorMax;
//		Vector2 pmin = guide.

		Debug.Log (gmin);


		Vector2 min = gmin;
		Vector2 max = gmax;
		

		x = (int) (min.x*Screen.width);
		y = (int) (min.y*Screen.height);
		width = (int) guideRect.rect.width;
		height = (int) guideRect.rect.height;

//		x = 0;
//		y = 0;
//		width = Screen.width;
//		height = Screen.height;

//		Debug.Log ("x,y: " + x + " : " + y);
//		Debug.Log ("Width,Height: " + width + " : " + height);




		InvokeRepeating ("QRScan", 0f, 0.5f); 
	}
	void OnGUI()
	{
		int rectWidth = (int)(Screen.width);
		int rectHeight = (int)(Screen.height * 0.4);
		int xStart = (int)((Screen.width * 0.5) - (rectWidth * 0.5));
		int yStart = (int)((Screen.height * 0.5) - (rectHeight * 0.5));

		GUI.Box(new Rect(xStart, yStart, rectWidth, rectHeight), texture);
	}

	void QRScan () {
		// drawing the camera on screen
//		GUI.DrawTexture (screenRect, camTexture, ScaleMode.ScaleToFit);
		// do the reading — you might want to attempt to read less often than you draw on the screen for performance sake
		try {
			Debug.Log ("SCREEN Width,Height: " + Screen.width + " : " + Screen.height);
			Debug.Log ("CAMERA TEXTURE Width,Height: " + camTexture.width + " : " + camTexture.height);

			// decode the current frame
//			Color[] colorPixels = camTexture.GetPixels();
////			Color[] colorPixels = camTexture.GetPixels(x,y,width,height);
//			List<Color32> color32PixelsList = new List<Color32>();
//
//
//			foreach(Color c in colorPixels){
//				color32PixelsList.Add((Color32) c);
//			}
//			Color32[] color32Pixels = color32PixelsList.ToArray();

			int startIndex = camTexture.width*(int)(camTexture.height*0.25);
			int length = camTexture.width*(int)(camTexture.height*0.50);
//			int startIndex = 0;
//			int length = Screen.width*Screen.height;

			Color32[] color32Pixels = new Color32[length];


			Array.Copy(camTexture.GetPixels32(), startIndex, color32Pixels, 0, length);

//			foreach( Color32 c in color32Pixels){
//				Debug.Log(c);
//				break;
//			}
//


			IBarcodeReader barcodeReader = new BarcodeReader ();

			var result = barcodeReader.Decode(color32Pixels, camTexture.width , (int)(camTexture.height*0.5));
//			var result = barcodeReader.Decode(camTexture.GetPixels32(), camTexture.width , camTexture.height);

			if (result != null) {
				QRData = result.Text;
				Debug.Log("DECODED TEXT FROM QR: " + result.Text);
//				CancelInvoke();
			}
			else {
				Debug.Log("NO CODE FOUND AT " + x + " + " + width + ", " + y + " + " + height);
			}
		} catch(Exception ex) { Debug.LogWarning (ex.Message); }
	}
}
