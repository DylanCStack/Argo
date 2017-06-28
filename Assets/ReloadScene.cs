using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Vuforia;
using UnityEngine.UI;
using UnityEngine.Video;

public class ReloadScene : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void reloadScene () {
//		if (GameObject.Find ("ARCamera")) {
////			Destroy(GameObject.Find ("ARCamera"));
//		}
//		if (GameObject.Find ("TestObject")) {
////			Destroy (GameObject.Find ("TestObject"));
//		}
//		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);

		GameObject.Find ("ARCamera").GetComponent<VuforiaBehaviour> ().enabled = false;
		Debug.Log ("---------disable vuforia behaviour----------");
		GameObject.Find ("ARCamera").SetActive (false);
		Debug.Log ("---------set arcamera inactive----------");
		FindObject (GameObject.Find("HomeScreenPanel"),"RawImage").SetActive (true);	
		Debug.Log ("---------set raw image active----------");
		GameObject.Find ("TestObject").GetComponent<qrscanner3> ().enabled = true;
		Debug.Log ("---------enabled qr scanner----------");
		GameObject.Find ("ImageTarget").GetComponent<ImageTargetPlayAudio> ().enabled = false;

	}

	GameObject FindObject(GameObject parent, string name)
	{
		Component[] trs= parent.GetComponentsInChildren(typeof(Transform), true);
		foreach(Component t in trs){
			if(t.name == name){
				return t.gameObject;
			}
		}
		return null;
	}
}
