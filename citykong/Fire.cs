using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	public AudioClip fireSound;
	// Update is called once per frame
	void Start(){
		//StartCoroutine ("ValChange");
	}

	void Update () {
	}

	void OnTriggerEnter2D(Collider2D col){
		AudioSource.PlayClipAtPoint (fireSound, col.transform.position);
		Vector3 vel = col.gameObject.rigidbody2D.velocity;
		col.gameObject.rigidbody2D.velocity = new Vector3 (0, vel.y, vel.z);
		col.gameObject.GetComponent<Character> ().Burn ();
	}
}
