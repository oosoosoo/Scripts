using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerPanel : MonoBehaviour {
	public Text name;
	public Text level;
	public Text record;
	public Text AIrecord;

	public void SetName(string name){
		this.name.text = name;
	}
	public void SetLevel(int level){
		if(level <= 10)	this.level.text = (11-level)+"급";
		else this.level.text = level-10+"단";
	}
	public void SetRecord(int record){
		this.record.text = record+"승";
	}
	public void SetAIRecord(int AIrecord){
		this.AIrecord.text = AIrecord+"승";
	}
}
