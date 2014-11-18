using UnityEngine;
using System.Collections;

public class StartingLogo : MonoBehaviour {
	public GameObject logo1;
	public GameObject logo2;
	// Use this for initialization
	void Awake () {
		StartCoroutine (Logo ());
	}

	IEnumerator Logo(){
		yield return new WaitForSeconds(3f);
		Application.LoadLevel ("Main");

	}
}
