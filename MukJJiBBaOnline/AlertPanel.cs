using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AlertPanel : MonoBehaviour {
	public Text text;

	public void Alert(string msg){
		text.text = msg;
		gameObject.SetActive (true);
	}
}
