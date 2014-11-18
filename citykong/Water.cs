using UnityEngine;
using System.Collections;

public class Water : MonoBehaviour {
	public AudioClip waterSound;
	public ParticleSystem waterSplash;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter2D(Collider2D col){
		Object o = Instantiate (waterSplash, col.transform.position, new Quaternion ());
		waterSplash.transform.position = col.transform.position;
		waterSplash.Play ();
		Destroy (o, 1f);
		AudioSource.PlayClipAtPoint (waterSound, col.transform.position);
		col.gameObject.GetComponent<Character> ().Wet ();
	}
}
