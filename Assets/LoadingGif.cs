using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingGif : MonoBehaviour {

	public Texture2D[] mSprites;
	private Texture2D currentSprite;
	private int counter;
	public float switchTime = 0.5f;

	public Rect r_Sprite;

	void Start(){
		counter = 0;
		StartCoroutine("SwitchSprite");
	}

	void OnGUI(){
		GUI.DrawTexture(r_Sprite,currentSprite);
	}

	private IEnumerator SwitchSprite(){
		currentSprite = mSprites[counter];

		if(counter < mSprites.Length-1){
			counter++;
		}else{
			counter = 0;
		}

		yield return new WaitForSeconds(switchTime);
		StartCoroutine("SwitchSprite");
	}
}
