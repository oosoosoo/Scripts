using UnityEngine;
using System.Collections;

public class Wind : MonoBehaviour {
	public AudioClip windSound;

	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnTriggerEnter2D(Collider2D col){
		AudioSource.PlayClipAtPoint (windSound, col.transform.position);
		col.gameObject.GetComponent<Character> ().Blow ();
	}
}
