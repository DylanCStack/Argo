using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class ObjectiveCInterfaceTestScript : MonoBehaviour {


	#if UNITY_IPHONE
	[DllImport("__Internal")]
	private static extern string _testMethod();
	#endif

	public string displayText = "false";

	void Start () {
		displayText = TestMethod ();

			GameObject displayLog = GameObject.Find ("DisplayLog");
			displayLog.GetComponent<Text> ().text = displayText;

	}


	public static string TestMethod() {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			return _testMethod();
		}
		#endif
		return "did not work";
	}
}
