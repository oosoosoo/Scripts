using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Malpoong : MonoBehaviour {
	public Text text;
	Animator anim;

	// Use this for initialization
	void Awake () {

	}

	public void Say(string s){
		StopCoroutine ("Close");
		text.text = s;
		gameObject.SetActive (true);
		Sound.Chat ();
		StartCoroutine ("Close");
	}

	IEnumerator Close(){
		yield return new WaitForSeconds (5f);
		gameObject.SetActive (false);
	}
}
