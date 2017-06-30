using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LoadMenuOnClick : MonoBehaviour {

	public void LoadByName (string destinationMenu)
	{
		//get parent panel game object
//		GameObject currentPanel = gameObject.transform.parent.gameObject;
		GameObject currentPanel = GetCurrentPanel(gameObject);

		//get canvas
		GameObject parentPanel = currentPanel.transform.parent.gameObject;

		//get all components in the parent by transform (to find disabled components)
		Transform[] trs = parentPanel.GetComponentsInChildren<Transform>(true);


		foreach (Transform t in trs) {//check if component name is the one we are looking for
			if (t.name == destinationMenu) {//turn on destination component
				t.gameObject.SetActive (true);
				currentPanel.SetActive(false); //turn off current panel
			}
		}
	}

	public GameObject GetCurrentPanel(GameObject child){
		if (child.transform.parent.gameObject.name == "Canvas") {
			return child;
		} else {
			 return GetCurrentPanel (child.transform.parent.gameObject);
		}
	}
}
