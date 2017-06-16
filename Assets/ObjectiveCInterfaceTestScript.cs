using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;

public class ObjectiveCInterfaceTestScript : MonoBehaviour {


	#if UNITY_IOS     
	[DllImport ("__Internal")]
	private static extern string framework_hello();

	[DllImport ("__Internal")]
	private static extern void framework_ChooseExisting();

	[DllImport ("__Internal")]
	private static extern void _chooseExisting();

	#endif      
	public static string hello() {         
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {             
			return framework_hello();
		}
		#endif  

		return "nothin";
	}     

	public void chooseExisting() {
		#if UNITY_IOS
		if (Application.platform == RuntimePlatform.IPhonePlayer) {             
//			framework_ChooseExisting();
			_chooseExisting();
		}
		#endif  
	}

	public void recieveImagePath(string message) {
		GameObject.Find("DisplayLog").GetComponent<Text>().text = message;
	}
	 

	void Start () {

	}



}
