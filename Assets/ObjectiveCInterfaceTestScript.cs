using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class ObjectiveCInterfaceTestScript : MonoBehaviour {


	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern bool _getValue();
	#endif

	public string displayText = "FAILURE";

	void Start () {
		displayText = TestMethod ();

			GameObject displayLog = GameObject.Find ("DisplayLog");
			displayLog.GetComponent<Text> ().text = displayText;

	}


	public static string TestMethod() {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
		bool result = _getValue();
		if (result)
			return "SUCCESS";
		}
		#endif
		return "did not work";
	}
}
