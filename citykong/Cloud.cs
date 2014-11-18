using UnityEngine;
using System.Collections;

public class Cloud : MonoBehaviour {
	private float speed;
	// Use this for initialization
	void Start () {
		speed = Random.Range (0.1f, 0.4f);
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = transform.position;
		float x = v.x + speed * Time.deltaTime;
		transform.position = new Vector3 (x, v.y, v.z);
	}
}
