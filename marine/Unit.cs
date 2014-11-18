using UnityEngine;
using System.Collections;

public class Unit : MonoBehaviour {
	protected int hp = 100;
	protected int damage = 20;
	protected float attDelay = 0.5f;
	protected float attTime = 0.2f;
	float preAttTime;
	float preVoiceTime = -2f;
	public float speed = 1;
	int index = -1 ;
	bool isMoving;
	protected bool dead;
	public AudioClip[] attSound;
	public AudioClip[] voiceSound;
	public AudioClip[] dieSound;
	// Use this for initialization
	void Awake () {
		GetComponent<Animator> ().speed = speed;
	}

	public void Move(float angle, Vector2 v){
		if(Time.time - preAttTime <= attTime || dead) return;
		if(voiceSound.Length > 0 && Time.time-preVoiceTime >= 2f){
			AudioSource.PlayClipAtPoint (voiceSound[Random.Range(0, voiceSound.Length)], Vector3.zero);
			preVoiceTime = Time.time;
		}
		isMoving = true;
		GetComponent<Animator> ().SetInteger ("attackIndex", -1);
		GetComponent<Animator> ().SetInteger ("stopIndex", -1);

		if(angle>-10 && angle<180 ){
			index = (int)(angle/20);
			transform.localScale = new Vector3(1, 1, 1);
		}else{
			index = (int)(angle/-20);
			transform.localScale = new Vector3(-1, 1, 1);
		}
		GetComponent<Animator> ().SetInteger ("moveIndex", index);
		transform.Translate (v*Time.deltaTime*speed);
	}

	public void Stop(float angle){
		isMoving = false;
		GetComponent<Animator> ().SetInteger ("attackIndex", -1);
		GetComponent<Animator> ().SetInteger ("moveIndex", -1);
		
		if(angle>-10 && angle<180 ){
			index = (int)(angle/20);
			transform.localScale = new Vector3(1, 1, 1);
		}else{
			index = (int)(angle/-20);
			transform.localScale = new Vector3(-1, 1, 1);
		}
		GetComponent<Animator> ().SetInteger ("stopIndex", index);
	}
	public void Stop(){
		isMoving = false;
		GetComponent<Animator> ().SetInteger ("attackIndex", -1);
		GetComponent<Animator> ().SetInteger ("moveIndex", -1);
		GetComponent<Animator> ().SetInteger ("stopIndex", index);
	}

	public void Attack(Unit u){
		if(Time.time-preAttTime < attDelay || isMoving || dead){
			return;
		}
		AudioSource.PlayClipAtPoint (attSound[Random.Range(0, attSound.Length)], Vector3.zero);
		preAttTime = Time.time;
		GetComponent<Animator> ().SetInteger ("stopIndex", -1);
		GetComponent<Animator> ().SetInteger ("moveIndex", -1);
		Transform t = u.transform;
		float f = Mathf.Atan2(t.position.x - transform.position.x, t.position.y - transform.position.y);
		f = f * 360f * 0.318f * 0.5f;
		if(f>-10 && f<180 ){
			index = (int)(f/20);
			transform.localScale = new Vector3(1, 1, 1);
		}else{
			index = (int)(f/-20);
			transform.localScale = new Vector3(-1, 1, 1);
		}
		GetComponent<Animator> ().SetInteger ("attackIndex", index);
		u.hp -= damage;
		if(u.hp <= 0) u.Die();
	}

	public void Die(){	
		dead = true;
		collider2D.enabled = false;
		tag = "Player";
		AudioSource.PlayClipAtPoint (dieSound[Random.Range(0, dieSound.Length)], Vector3.zero);
		GetComponent<Animator> ().SetInteger ("attackIndex", -1);
		GetComponent<Animator> ().SetInteger ("moveIndex", -1);
		GetComponent<Animator> ().SetInteger ("stopIndex", -1);
		GetComponent<Animator> ().SetTrigger ("die");	
	}
	public void DieAfter(){
		GetComponent<Animator> ().SetInteger ("stopIndex", 10);
		DestroyObject (gameObject);
	}
}
