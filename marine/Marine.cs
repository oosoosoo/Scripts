using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Marine : Unit {
	public AudioClip[] stimSound;
	public Button stimBtn;
	public GameObject rangeCircle;
	float range = 1.2f;
	void Start(){
		rangeCircle.transform.localScale = new Vector2 (range, range);
	}
	// Update is called once per frame
	void Update () {
		Collider2D enemy = Physics2D.OverlapCircle(transform.position, range);
		if(enemy != null && enemy.tag.Equals("enemy"))	Attack (enemy.GetComponent<Unit>());
	}

	void OnCollisionEnter2D(Collision2D col){
		Debug.Log ("collision enter");
		Die ();
	}

	public void StimPack(){
		stimBtn.enabled = false;
		stimBtn.GetComponent<Image> ().fillAmount = 0;
		AudioSource.PlayClipAtPoint (stimSound[Random.Range(0, stimSound.Length)], Vector3.zero);
		attDelay *= 0.5f;
		attTime *= 0.5f;
		speed *= 1.5f;
		StartCoroutine (StimPackOff());
		StartCoroutine (StimPackCool());
	}
	IEnumerator StimPackOff(){
		yield return new WaitForSeconds (5f);
		attDelay *= 2f;
		attTime *= 2f;
		speed /= 1.5f;
	}
	IEnumerator StimPackCool(){
		int repeat = 0;
		while(repeat < 10){
			yield return new WaitForSeconds(1f);
			repeat++;
			stimBtn.GetComponent<Image> ().fillAmount = 1/10f * repeat;
		}
		stimBtn.enabled = true;
	}
}
