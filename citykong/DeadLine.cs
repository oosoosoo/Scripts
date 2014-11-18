using UnityEngine;
using System.Collections;

public class DeadLine : MonoBehaviour {
	public Transform character;
	public GameManager gameManager;
	public GameManagerSingle gameManagerSingle;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(character != null)
		transform.Translate (new Vector3(character.position.x, -15, 0) - transform.position);
	}

	void OnTriggerEnter2D(Collider2D c){
		if (c.gameObject.tag == "Player") {
			if(gameManager != null)	gameManager.Fail(c.gameObject);
			if(gameManagerSingle != null)	gameManagerSingle.Turn();
		}
	}
}
