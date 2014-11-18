using UnityEngine;
using System.Collections;

public class Cam_front : MonoBehaviour {
	public GameObject character;
	
	// Update is called once per frame
	void Update () {
		Vector3 v = new Vector3 (0.1f, -0.5f, -50);
		transform.position = Vector3.Lerp (transform.position, v, Time.deltaTime);
	}
}
