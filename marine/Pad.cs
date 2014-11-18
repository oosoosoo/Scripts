using UnityEngine;
using System.Collections;

public class Pad : MonoBehaviour {
	public GameObject ball;
	public Unit marine;
	float f;
	bool isMoving;
	// Update is called once per frame
	void Update () {
		if(marine == null) return;
		if (Input.GetMouseButton (0)) {
			isMoving = true;
			Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			if(Vector2.Distance(mousePos, transform.position) < 1f){
				f = Mathf.Atan2(mousePos.x - transform.position.x, mousePos.y - transform.position.y);
				f = f * 360f * 0.318f * 0.5f;
				ball.transform.position = new Vector3(mousePos.x, mousePos.y, -1);
				marine.Move (f, (mousePos-new Vector2(transform.position.x, transform.position.y)).normalized);
			}
		}else if(isMoving && !Input.GetMouseButton (0)){
			isMoving = false;
			ball.transform.localPosition = new Vector3(0,0,-1);
			marine.Stop (f);
		}
	}
}
