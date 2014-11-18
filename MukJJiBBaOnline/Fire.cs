using UnityEngine;
using System.Collections;

public class Fire : MonoBehaviour {
	public void Move(Hand h){
		transform.parent = h.transform;
		transform.localPosition = new Vector3(0,5.5f,0);
	}

	public void Clear(){
		transform.position = new Vector3 (0,0,0);
	}
}
