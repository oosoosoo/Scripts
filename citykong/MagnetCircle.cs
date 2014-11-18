using UnityEngine;
using System.Collections;

public class MagnetCircle : MonoBehaviour {
	public GameObject character;
	public GameObject targetCharacter;
	public AudioClip magnet;

	void Update(){
		if(character == null) return;
		gameObject.transform.position = character.transform.position;
	}

	void OnTriggerEnter2D(Collider2D c){
		if(c.gameObject != character){
			if(c.gameObject.GetComponent<Character>().charBrickIndex == character.gameObject.GetComponent<Character>().charBrickIndex)
				return;
			targetCharacter = c.gameObject;
			AudioSource.PlayClipAtPoint(magnet, gameObject.transform.position);
			gameObject.collider2D.enabled = false;
			Vector2 vel = character.rigidbody2D.velocity;
			Vector2 pos = character.transform.position;
			Vector2 tPos = targetCharacter.transform.position;
			character.rigidbody2D.velocity = Vector2.zero;
			character.rigidbody2D.velocity = new Vector2((tPos.x-pos.x)*10, vel.y*0.5f);
		}
	}

	public void Reset(){
		gameObject.SetActive (true);
		character = null;
		targetCharacter = null;
		gameObject.collider2D.enabled = true;
		gameObject.transform.position = new Vector3 (-100, 0, 0);
	}
}
