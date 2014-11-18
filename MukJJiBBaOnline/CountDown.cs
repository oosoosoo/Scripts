using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountDown : MonoBehaviour {
	public Text countText;
	public AudioClip tickSound;
	public GameManager manager;

	private int number;
	private bool counting;
	private Animator anim;

	public void Start(){
		anim = GetComponent<Animator> ();
	}

	public void Reset(int i){
		//countText.text = ""+i;
		number = i;
	}

	IEnumerator Count(){
		while(true){
			AudioSource.PlayClipAtPoint(tickSound, Vector3.zero, 0.5f);
			yield return new WaitForSeconds(1f);
			number--;
			if(number <= -1){
				StopCount();
				yield break;
			}
			//countText.text = ""+number;
		}
	}

	public void StartCount(int i){
		gameObject.SetActive (true);
		Reset (i);
		StartCoroutine (Count ());
		//anim.SetBool ("counting", true);
	}
	public void StopCount(){
		gameObject.SetActive(false);
		StopCoroutine (Count ());
		//anim.SetBool ("counting", false);
	}
}
