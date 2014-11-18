using UnityEngine;
using System.Collections;

public class ItemBalloon : MonoBehaviour {
	public AudioClip boxCrash;

	void OnTriggerEnter2D(Collider2D c){
		AudioSource.PlayClipAtPoint (boxCrash, c.transform.position);
		GetComponent<Animator> ().SetTrigger ("hit");
		GetComponent<BoxCollider2D> ().enabled = false;
		//c.gameObject.GetComponent<Character>().NewItem(0);
		c.gameObject.GetComponent<Character>().NewItem(Random.Range (0, 5));
		/*
		 * 0: exchange
		 * 1: go1
		 * 2: jump2
		 * 3: jump3
		 * 4: kick
		 * 5:magnet
		 * */
	}

	void DestroyThis(){
		gameObject.SetActive(false);
	}
}
