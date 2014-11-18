using UnityEngine;
using System.Collections;

public class Camwalk : MonoBehaviour {
	[HideInInspector]
	public Transform playerPos;
	private float camMinSize = 5f;
	private float touchDist = 100f;
	private bool zooming;

	private Vector3 beforeTouch;
	public bool walkable = true;
	// Use this for initialization
	void Start () {
	}
	
	void Update(){	
		//zoom
		if (Input.touchCount == 2) { 
			walkable = false;
			zooming = true;
			Vector2 v = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
			Vector2 v2 = camera.ScreenToWorldPoint(Input.GetTouch(1).position);
			if(Vector2.Distance(v, v2) < touchDist){	
				camMinSize += 1f;
				if(camMinSize <= 5f) camMinSize = 5f;
			}else if(Vector2.Distance(v, v2) > touchDist){	
				camMinSize -= 1f;		
				if(camMinSize >= 8f) camMinSize = 8f;
			}			
			touchDist = Vector2.Distance(v, v2);
		}camMinSize = Mathf.Clamp(camMinSize, 5, 8);

		if(zooming && Input.touchCount == 0){
			walkable = true;
			zooming = false;
		}

		//cameramove
		if (Input.touchCount == 1 && walkable) {
			if(Input.GetTouch(0).phase == TouchPhase.Began){
				beforeTouch = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
			}
			if(Input.GetTouch(0).phase == TouchPhase.Moved){
				Vector3 currentTouch = camera.ScreenToWorldPoint(Input.GetTouch(0).position);
				if(playerPos.rigidbody2D.velocity == Vector2.zero){
					transform.position = new Vector3(transform.position.x+(beforeTouch.x-currentTouch.x), transform.position.y, transform.position.z);
				}
			}
		}

	}
	
	void FixedUpdate(){
		if(playerPos == null) return;
		float gap = camMinSize - 3;
		float velX = playerPos.rigidbody2D.velocity.x;
		float velY = playerPos.position.y+5f;
		Vector3 v = new Vector3 (playerPos.position.x + gap + 2, gap - 3, -50);
		gameObject.camera.orthographicSize = Mathf.Lerp (gameObject.camera.orthographicSize, camMinSize + (Mathf.Max(velX,velY) * 0.2f), 0.05f);
		if (Input.touchCount == 0 && walkable) {
			transform.position = Vector3.Lerp (transform.position, v, 0.05f);
		}
	}
}
