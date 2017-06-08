using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoadMenuOnClick : MonoBehaviour {

	public void LoadByName (string destinationMenu)
	{

		GameObject current = gameObject.transform.parent.gameObject;
		Debug.Log (current.name);
		GameObject parent = current.transform.parent.gameObject;
		Transform[] trs = parent.GetComponentsInChildren<Transform>(true);

		foreach (Transform t in trs) {
			if (t.name == destinationMenu) {
				t.gameObject.SetActive (true);
			}
		}

		Debug.Log (current.name);
		current.SetActive(false);

	}
}
