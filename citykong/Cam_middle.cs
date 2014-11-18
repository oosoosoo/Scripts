using UnityEngine;
using System.Collections;

public class Cam_middle : MonoBehaviour {
	public Camera cam_front;
	//void OnPreRender() { cam_front.Render(); }
	// Use this for initialization
	void Awake () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3 (cam_front.transform.position.x*0.5f, cam_front.transform.position.y*0.2f, -10);
		//camera.orthographicSize = cam_front.orthographicSize;
	}
}
