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

		Debug.Log ("-----------------Look for ARCamera---------------");
		FindRootObject ("ARCamera").GetComponent<VuforiaBehaviour> ().enabled = false;
		Debug.Log ("-----------------Look for ARCamera2---------------");
		FindRootObject ("ARCamera").SetActive (false);
		Debug.Log ("-----------------Look for HomeScreenPanel---------------");
		FindObject (GameObject.Find("HomeScreenPanel"),"RawImage").SetActive (true);	
		Debug.Log ("-----------------Look for TestObject---------------");
		GameObject testObject = GameObject.Find ("TestObject");
		Debug.Log ("-----------------Look for qrscanner3---------------");
		testObject.GetComponent<qrscanner3> ().enabled = true;
		Debug.Log ("-----------------Look for ImageTarget---------------");
		FindRootObject ("ImageTarget").GetComponent<ImageTargetPlayAudio> ().enabled = false;

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

	GameObject FindRootObject(string name)
	{
		List<GameObject> rootObjects = new List<GameObject>();
		Scene scene = SceneManager.GetActiveScene();
		scene.GetRootGameObjects( rootObjects );
		foreach(GameObject t in rootObjects){
			if(t.name == name){
				return t.gameObject;
			}
		}
		return null;
	}
}
