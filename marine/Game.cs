using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	public GameObject marine;
	public GameObject zergling;
	float delay = 3f;
	float[] randomY = new float[2]{-2.311f, 2.379f};
	// Use this for initialization
	void Start () {
		StartCoroutine (Spawn ());
	}
	
	IEnumerator Spawn(){
		while(true){
			GameObject obj = Instantiate(zergling, new Vector2(Random.Range(1.621f, 3.835f),randomY[Random.Range(0, randomY.Length)]), new Quaternion()) as GameObject;
			delay -= 0.05f;
			obj.GetComponent<Zergling> ().marine = marine.transform;
			yield return new WaitForSeconds(delay);
		}
	}
}
