using UnityEngine;
using System.Collections;

public class Background_move : MonoBehaviour {
	public Camera cam_back;
	public float range;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (cam_back.transform.position.x > transform.position.x + range*2) {
			transform.position = new Vector3(transform.position.x+ range*4, transform.position.y, 2);
		}else if(cam_back.transform.position.x < transform.position.x - range*2){
			transform.position = new Vector3(transform.position.x- range*4, transform.position.y, 2);
		}
	}
}
