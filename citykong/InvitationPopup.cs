using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class InvitationPopup : MonoBehaviour {	
	public ArrayList invitationList = new ArrayList ();

	[HideInInspector]
	public string invitationId;
	public Text text;
	public Image image;

	void Start(){
	}

	public void Pop(Invitation invitation){
		invitationList.Add (invitation);
		gameObject.SetActive(true);
		if(invitationList.Count == 1)
		SetInvitorName (invitation.Inviter.Player.DisplayName);
	}

	public void SetInvitorName(string name){
		text.text = name + " invited you";
		GetComponent<RectTransform> ().anchoredPosition = Vector2.zero;
	}

	public void SetInvitorPic(Texture2D texture){ 
		image.sprite = Sprite.Create (texture, new Rect (0, 0, texture.width, texture.height), new Vector2 ());
	}

	public void AcceptInvitation(){
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
		Invitation inv = invitationList[0] as Invitation;
		Info.invitationId = inv.InvitationId;
		invitationList.Remove (inv);
		foreach(Invitation i in invitationList){
			PlayGamesPlatform.Instance.RealTime.DeclineInvitation (i.InvitationId);
		}
		invitationList.Clear ();
		Application.LoadLevel ("Single_play");
	}

	public void DeclineInvitation(){
		Invitation inv = invitationList[0] as Invitation;
		PlayGamesPlatform.Instance.RealTime.DeclineInvitation (inv.InvitationId);
		invitationList.Remove (inv);
		if(invitationList.Count > 0){
			Invitation iv = invitationList[0] as Invitation;
			SetInvitorName (iv.Inviter.Player.DisplayName);
		}else{
			gameObject.SetActive(false);
		}
	}
}
