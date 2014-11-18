using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour {
	public AudioClip[] audio;
	public ParticleSystem prtcl;
	// Use this for initialization

	public GameObject sprite;
	private Animator anim;
	void Awake () {
		anim = sprite.GetComponentInChildren<Animator> ();
		StartCoroutine ("Jump");
	}
	
	// Update is called once per frame
	void Update () {
		sprite.transform.eulerAngles = new Vector3 (0, 0, rigidbody2D.velocity.y*1.5f);
		if(transform.position.x > 11){				
			transform.position = new Vector2(-7f,1);
			transform.rigidbody2D.velocity = new Vector2(7,-5);
		}
	}

	IEnumerator Jump(){
		while (true) {
			AudioSource.PlayClipAtPoint(audio[0], transform.position);
			rigidbody2D.AddForce (transform.position + new Vector3(0.5f, 1.5f) * 440f);
			anim.SetBool ("charge",false);
			yield return new WaitForSeconds (2f);
		}
	}
	
	void OnCollisionEnter2D(Collision2D col){
		AudioSource.PlayClipAtPoint(audio[1], transform.position);
		anim.SetBool ("charge",true);
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.angularVelocity = 0;
		transform.rotation = new Quaternion ();
		col.gameObject.GetComponentInChildren<Animator> ().SetTrigger ("shake");
		prtcl.transform.position = transform.position+new Vector3(0,-0.8f,0);
		prtcl.Play ();
	}
}
