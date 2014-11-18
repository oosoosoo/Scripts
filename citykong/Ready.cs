using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Ready : MonoBehaviour {
	public GameObject quitButton;
	public RoomManager roomManager;
	private bool ready = false;
	private bool click;
	private Text text;
	// Use this for initialization
	void Start () {
		text = GetComponentInChildren<Text> ();
	}

	public void Click(){
		if(ready) Cancel();
		else Ready_();
	}

	void Ready_(){
		ready = true;
		text.text = "CANCEL";
		quitButton.SetActive (false);
		roomManager.RequestReady();

	}
	public void Cancel(){
		ready = false;
		text.text = "READY";
		quitButton.SetActive (true);
		roomManager.RequestCancel();
	}
}
