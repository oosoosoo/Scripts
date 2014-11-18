using UnityEngine;
using System.Collections;

public class Weather : MonoBehaviour {
	public GameObject rain;
	public GameObject sky;
	public AudioClip lightening;
	public GameObject[] clouds;

	private SpriteRenderer ren;

	// Use this for initialization
	void Awake () {
		ren = sky.GetComponent<SpriteRenderer> ();
		int i = Random.Range (0, 2);
		
		//sunny
		if(i==0){
			ren.color = Color.white;
			foreach(GameObject o in clouds){
				o.SetActive(true);
			}
		}
		
		//rain
		if(i==1){
			ren.color = Color.gray;
			rain.SetActive(true);
			StartCoroutine("Lightening");
		}
	}

	IEnumerator Lightening(){
		while(true){
			yield return new WaitForSeconds(Random.Range(5,10));
			AudioSource.PlayClipAtPoint (lightening, transform.position, 0.2f);
			ren.color = Color.white;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.gray;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.white;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.gray;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.white;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.gray;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.white;
			yield return new WaitForSeconds(Random.Range (0.05f, 0.1f));
			ren.color = Color.gray;
		}
	}
}
