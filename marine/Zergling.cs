using UnityEngine;
using System.Collections;

public class Zergling : Unit {
	public Transform marine;

	void Update(){	
		if(dead || marine == null) return;
		float f = Mathf.Atan2(marine.position.x - transform.position.x, marine.position.y - transform.position.y);
		f = f * 360f * 0.318f * 0.5f;
		Move (f, (new Vector2 (marine.position.x, marine.position.y) - new Vector2 (transform.position.x, transform.position.y)).normalized);
	}

}
