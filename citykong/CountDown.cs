using UnityEngine;
using System.Collections;

public class CountDown : MonoBehaviour {
	public AudioClip countSound;
	int count = 5;
	void Count(){
		count--;
		if(count <= 0){
			gameObject.SetActive(false);
			return;
		}
		this.guiText.color = Color.Lerp (Color.red, Color.white, count*0.2f-0.2f);
		this.guiText.text = count + "";
		AudioSource.PlayClipAtPoint (countSound, transform.position, 1-count*0.2f);
	}
	public void Reset(){
		count = 5;
		this.guiText.text = count + "";
	}
}
