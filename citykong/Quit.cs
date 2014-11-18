using UnityEngine;
using System.Collections;

public class Quit : MonoBehaviour {

	public RoomManager roomManager;


	public void QuitRoom(){
		roomManager.QuitRoom ();
		Application.LoadLevel("Main");
	}
}
