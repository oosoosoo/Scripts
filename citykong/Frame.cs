using UnityEngine;
using System.Collections;

public class Frame : MonoBehaviour {
	public GameObject ready;
	public GameObject character;
	// Use this for initialization
	void Start () {
		ready.SetActive (false);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Ready(){
		ready.SetActive (true);
	}
	
	public void Cancel(){
		ready.SetActive (false);
	}
}
