using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DropPanel : MonoBehaviour {
	public Text seconds;
	private int sec = 100;

	public void CountDown(){
		if(sec <= 0){
			return;
		}
		sec--;
		seconds.text = sec+"";
	}

	public void SetSec(int s){
		sec = s;
	}
}
