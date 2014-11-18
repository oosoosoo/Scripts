using UnityEngine;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;

public class RealTimeListener : RealTimeMultiplayerListener {
	public MainManager mainManager;	
	public RoomManager roomManager;
	public GameManager gameManager;
	public ShopManager shopManager;

	public int STATUS = 0;
	public const int LOBBY = 0;
	public const int ROOM = 1;
	public const int PLAYING = 2;
	public const int SHOP = 3;

	public void OnInvited(Invitation invitation){
		switch(STATUS){
		case(LOBBY):
			mainManager.Invited(invitation);
			break;
		case(ROOM):
			PlayGamesPlatform.Instance.RealTime.DeclineInvitation(invitation.InvitationId);
			break;
		case(PLAYING):
			PlayGamesPlatform.Instance.RealTime.DeclineInvitation(invitation.InvitationId);
			gameManager.Invited(invitation.Inviter.DisplayName);
			break;
		case(SHOP):
			shopManager.Invited(invitation);
			break;
		}
	}

	public void OnRoomSetupProgress(float percent){
		roomManager.RoomSetupProgress (percent);
	}

	public void OnRoomConnected(bool success){	
		switch(STATUS){
		case(ROOM):
			roomManager.RoomConnected(success);
			break;
		case(PLAYING):
		case(LOBBY):
			PlayGamesPlatform.Instance.RealTime.LeaveRoom();
			break;
		}
	}

	public void OnLeftRoom(){
		switch(STATUS){
		case(ROOM):
			roomManager.LeftRoom();
			break;
		case(PLAYING):
			gameManager.LeftRoom();
			break;
		}	
	}
	
	public void OnPeersConnected(string[] participantIds){		
	}
	
	public void OnPeersDisconnected(string[] participantIds){}
	public void OnPeerDisconnected(string participantId){
		Debug.Log ("OnPeerDisconnected");
		switch(STATUS){
		case(ROOM):
			roomManager.PeerDisconnected(participantId);
			break;
		case(PLAYING):
			gameManager.PeerDisconnected(participantId);
			break;
		}
	}
	
	public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data){
		switch(STATUS){
		case(ROOM):
			roomManager.RealTimeMessageReceived(senderId, data);
			break;
		case(PLAYING):
			gameManager.RealTimeMessageReceived(senderId, data);
			break;
		}
	}
}
