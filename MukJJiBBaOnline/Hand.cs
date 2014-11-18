using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Hand : MonoBehaviour {
	public Sprite[] mukjjibbaSprite;
	public AudioClip count;
	public AudioClip shoot;
	public AudioClip brk;
	public GameObject hpBar;

	private Animator anim;
	[HideInInspector]
	public int gambamboInt = -1;
	[HideInInspector]
	public int hp = 100;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();	
	}

	public void GamBamBo(int index){
		ResetGBB ();
		gambamboInt = index;
		anim.SetTrigger ("count");
		AudioSource.PlayClipAtPoint (count, Vector3.zero);
	}

	public void GamBamBo(){
		GamBamBo (gambamboInt);
	}

	public void Shoot(int index){
		gambamboInt = index;
		anim.SetTrigger ("shoot");
		AudioSource.PlayClipAtPoint (shoot, Vector3.zero);
	}
	
	public void Shoot(){
		Shoot (gambamboInt);
	}

	public void GamBamBoEnd(){
		if(gambamboInt < 0) gambamboInt = 0;
		gameObject.GetComponent<SpriteRenderer>().sprite = mukjjibbaSprite[gambamboInt];
	}

	public void ResetGBB(){
		gameObject.GetComponent<SpriteRenderer>().sprite = mukjjibbaSprite[0];
	}

	public void StartShake(){
		anim.SetBool ("shake", true);
	}
	
	public void StopShake(){
		anim.SetBool ("shake", false);
		GamBamBoEnd ();
	}

	public int Damage(int dmg){
		hp -= dmg;
		if(hp < 0) hp = 0;
		StartCoroutine (Damage2());
		return hp;
	}

	IEnumerator Damage2(){
		yield return new WaitForSeconds (1f);
		hpBar.GetComponent<Animator> ().SetTrigger ("shake");
		hpBar.transform.localScale = new Vector3 (1, hp*0.01f, 1);
		hpBar.GetComponent<Image> ().color = Color.Lerp (Color.red, Color.green, hp*0.01f);
		AudioSource.PlayClipAtPoint (brk, Vector3.zero, 3f);
	}
}
