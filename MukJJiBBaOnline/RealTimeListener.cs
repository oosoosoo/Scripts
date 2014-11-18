using UnityEngine;
using LitJson;
using System.Collections;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;

public class RealTimeListener : RealTimeMultiplayerListener {

	public int STATUS = 2;
	public const int LOBBY = 0;
	public const int ROOM = 1;
	public const int PLAYING = 2;
	public const int SHOP = 3;

	public WaitRoom waitroom;
	public GameManagerM multi;

	public void OnInvited(Invitation invitation){

	}

	public void OnRoomSetupProgress(float percent){
		waitroom.RoomProgress (percent);
	}

	public void OnRoomConnected(bool success){
		waitroom.RoomCreated (success);
	}

	public void OnLeftRoom(){
		if(waitroom != null) waitroom.RoomCreated(false);
		if(multi != null) multi.StartCrt("MeDisc");
	}
	
	public void OnPeersConnected(string[] participantIds){		
	}
	
	public void OnPeersDisconnected(string[] participantIds){}
	public void OnPeerDisconnected(){
		if(waitroom != null) waitroom.RoomCreated(false);
		if(multi != null) multi.OppDisc();
	}
	
	public void OnRealTimeMessageReceived(bool isReliable, string senderId, byte[] data){
		if(multi != null)	multi.StopNetworkCheck ();
		string str = System.Text.Encoding.UTF8.GetString(data);
		JsonData task = JsonMapper.ToObject (str);
		string taskType = (string)task["taskType"];
		Debug.Log (str);
		if(taskType.Equals("info")){
			waitroom.OppInfo((string)task["name"], int.Parse((string)task["win"]), int.Parse((string)task["win_com"]));
		}
		else if(taskType.Equals("NewBegin")){
			multi.StartCrt("NewBegin");
		}
		else if(taskType.Equals("Begin")){
			multi.StartCrt("Begin");
		}
		else if(taskType.Equals("GBB")){
			multi.OppNumReceived("GBB", int.Parse((string)task["num"]));
		}
		else if(taskType.Equals("OFFENCE")){
			multi.OppNumReceived("OFFENCE", int.Parse((string)task["num"]));
		}
		else if(taskType.Equals("DEFENCE")){
			multi.OppNumReceived("DEFENCE", int.Parse((string)task["num"]));
		}
		else if(taskType.Equals("timeover")){
			multi.OppTimeOver();
		}
		else if(taskType.Equals("again")){
			multi.AgainReceived();
		}
		else if(taskType.Equals("chat")){
			multi.ChatReceived((string)task["msg"]);
		}
		else if(taskType.Equals("giveup")){
			multi.GiveUpReceived();
		}
		if(multi != null)	multi.StartCrt ("StartNetworkCheck");
	}
}
