using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
public class ObjectiveCInterfaceTestScript : MonoBehaviour {
	public Texture2D shareButtonImage; // Use this for initialization
	[DllImport("__Internal")]
	private static extern void OpenVideoPicker(string game_object_name, string function_name);
	void Start () {
		OpenVideoPicker ("RawImage", "VideoPicked");
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
		Debug.Log( path );
	}
}