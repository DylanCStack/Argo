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
		GameObject.Find ("ARCamera").SetActive(false);
		SceneManager.LoadScene (SceneManager.GetActiveScene().buildIndex);

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
