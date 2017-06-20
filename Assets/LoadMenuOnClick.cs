using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadMenuOnClick : MonoBehaviour {

	public void LoadByName (string destinationMenu)
	{
		//get parent panel game object
		GameObject current = gameObject.transform.parent.gameObject;

		//get canvas
		GameObject parent = current.transform.parent.gameObject;

		//get all components in the parent by transform (to find disabled components)
		Transform[] trs = parent.GetComponentsInChildren<Transform>(true);


		foreach (Transform t in trs) {//check if component name is the one we are looking for
			if (t.name == destinationMenu) {//turn on destination component
				t.gameObject.SetActive (true);
				current.SetActive(false); //turn off current panel
			}
		}
	}
}
