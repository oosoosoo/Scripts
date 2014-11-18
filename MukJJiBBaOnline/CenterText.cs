using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CenterText : MonoBehaviour {
	public Text t1;
	public Text t2;

	public void SetActive(bool b){
		gameObject.SetActive (b);
	}

	public void SetText(string s){
		t1.text = s;
		t2.text = s;
	}
}
