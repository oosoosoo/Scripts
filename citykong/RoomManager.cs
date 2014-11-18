using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using LitJson;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;

public class RoomManager : MonoBehaviour{
	public static string id = "realcrack";
	public GameObject camFront;
	public Transform dummyCenter;
	
	public GameObject[] frames;
	public GameObject[] characters;
	public GameObject character;
	
	public GameObject playerListPanel;
	public GameObject waitPanel;
	public GameObject readyButton;
	public GameObject quitButton;
	public Text progressText;
	public GameObject waitingText;
	public GameObject waitFailText;
	public GameObject gameListPanel;
	public GameObject allLeftPanel;
	
	private int myIndex = -1;
	private int readyCount = 0;
	private int numOfPlayers = 0;
	
	private int quitRoom;	
	private RealTimeListener listener = MainManager.listener;
	
	void Start(){
		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
		soundOffText.SetActive(!Info.soundOn);
		
		Info.playerIdList.Clear ();
		listener.roomManager = this;
		if(!string.IsNullOrEmpty(Info.invitationId)){
			if(!PlayGamesPlatform.Instance.RealTime.IsRoomConnected())	AcceptInvitation(Info.invitationId);
		}
		listener.STATUS = RealTimeListener.ROOM;
	}
	
	public void AcceptInvitation(string invitationId){
		Info.invitationId = null;
		PlayGamesPlatform.Instance.RealTime.AcceptInvitation(invitationId, listener);
		gameListPanel.SetActive (false);
		waitPanel.SetActive (true);
	}
	
	public void RoomSetupProgress (float percent){
		progressText.text = percent+"%";
	}
	
	public void RoomConnected(bool success){
		for(int i=0; i<4; i++){
			characters[i].transform.position = new Vector3(characters[i].transform.position.x, 7, characters[i].transform.position.z);
		}
		if(success){
			Message ();
			SetRoom(); //after SetRoom called, index 0 player makes map and send it to others.
			Info.myTurn = myIndex;
			waitPanel.SetActive(false);
		}else{
			Alert();
			WaitFail ();
		}
	}
	
	public void PeerDisconnected(string participantId){
		characters[Info.playerIdList.IndexOf(participantId)].SetActive(false);
		Info.characters[Info.playerIdList.IndexOf(participantId)] = null;
		frames[Info.playerIdList.IndexOf(participantId)].SetActive(false);
		numOfPlayers--;
		Info.playerIdList[Info.playerIdList.IndexOf(participantId)] = null;
		if(numOfPlayers < 2){
			allLeftPanel.SetActive(true);
		}
	}
	
	public void CreateRandomMatch(){
		Button ();
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame (2, 2, 0, listener);
		gameListPanel.SetActive (false);
		waitPanel.SetActive (true);
	}
	
	public void CreateInvitation(){
		Button ();
		PlayGamesPlatform.Instance.RealTime.CreateWithInvitationScreen (2, 2, 0, listener);
		gameListPanel.SetActive (false);
		waitPanel.SetActive (true);
	}
	
	public void QuitRoom(){
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
		Button ();
		Application.LoadLevel ("Main");
	}
	public void CancelWait(){
		Info.playerIdList.Clear ();
		camFront.transform.position = character.transform.position;
		camFront.GetComponent<Camwalk> ().playerPos = character.transform;
		for(int i=0; i<4; i++){
			frames[i].SetActive(false);
		}
		for(int i=0; i<4; i++){
			characters[i].SetActive(false);
		}
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
		waitingText.SetActive(true);
		progressText.text = "0%";
		waitFailText.SetActive (false);
		waitPanel.SetActive (false);
		allLeftPanel.SetActive (false);
		playerListPanel.SetActive (false);
		gameListPanel.SetActive (true);
		
	}
	public void LeftRoom(){
		allLeftPanel.SetActive(true);
	}
	
	public void RealTimeMessageReceived(string senderId, byte[] data){
		string str = System.Text.Encoding.UTF8.GetString(data);
		ParseResult (str, Info.playerIdList.IndexOf (senderId));
	}
	
	void WaitFail(){
		waitingText.SetActive(false);
		progressText.text = "";
		waitFailText.SetActive(true);
	}
	
	
	void SetRoom(){
		readyCount = 0;
		camFront.transform.position = dummyCenter.position;
		camFront.GetComponent<Camwalk> ().playerPos = dummyCenter;
		playerListPanel.SetActive (true);
		Participant myself = PlayGamesPlatform.Instance.RealTime.GetSelf();
		string myId = myself.ParticipantId;
		
		List<Participant> pList =  PlayGamesPlatform.Instance.RealTime.GetConnectedParticipants();
		int index = 0;
		foreach(Participant p in pList){
			if(p.ParticipantId.Equals (myId)) myIndex = pList.IndexOf(p);
			Info.playerIdList.Add(p.ParticipantId);
			frames [index].SetActive (true);
			frames [index].GetComponent<Frame>().Cancel();
			characters [index].SetActive (true);
			Info.characters [index] = characters [index];
			Info.characters [index].GetComponent<Character> ().SetName(p.DisplayName);
			index++;
		}numOfPlayers = index;
		StartCoroutine( RequestJoin () );
	}
	
	IEnumerator RequestJoin(){
		yield return new WaitForSeconds (1f);
		string str = "";
		if(Info.avatarSet != null){
			foreach(string s in Info.avatarSet){
				str += s+" ";
			}
		}
		MessageSender.Send ("join", new string[]{"name", "avatarSet"}, new string[]{Info.myName, str});
		ResponseJoin (Info.myName, myIndex, str);
	}
	
	public void ResponseJoin(string name, int playerIndex, string avatarSet){	
		if(playerIndex!=myIndex && myIndex==0) RequestBrick();
		characters[playerIndex].GetComponent<Character>().SetName(name);
		if(string.IsNullOrEmpty(avatarSet)) return;
		string[] avatar = avatarSet.Split (' ');
		characters [playerIndex].GetComponent<Character> ().SetAvatar (avatar);
	}
	
	public void RequestReady(){
		Button ();	
		MessageSender.Send ("ready");		
		ResponseReady (myIndex);
	}
	
	public void ResponseReady(int playerIndex){
		Chat ();
		frames [playerIndex].GetComponent<Frame> ().Ready ();
		readyCount++;
		if(readyCount == numOfPlayers){
			readyButton.SetActive(false);
			GameStart();
		}
	}
	
	public void RequestCancel(){
		Button ();
		readyCount--;
		MessageSender.Send ("cancel");
		frames [myIndex].GetComponent<Frame> ().Cancel ();
	}
	
	public void ResponseCancel(int playerIndex){
		frames [playerIndex].GetComponent<Frame> ().Cancel ();
		readyCount--;
	}
	
	public void RequestBrick(){
		string bricks = MapGenerator.GenerateBrick ();
		MessageSender.Send ("brick", "bricks", bricks);		
		ResponseBrick (JsonMapper.ToObject (bricks));
	}
	
	
	public void ResponseBrick(JsonData bricks){
		Info.bricks = new Vector2[10];
		JsonData brick = JsonMapper.ToObject( bricks["brickLoc"].ToJson() );
		for(int i=0; i<brick.Count; i++){
			Info.bricks[i].x = float.Parse(brick[i]["x"].ToJson());
			Info.bricks[i].y = float.Parse(brick[i]["y"].ToJson());
		}
		char[] fireLocArr = (bricks ["fireLoc"]+"").ToCharArray ();
		Info.fireLoc = new int[fireLocArr.Length];
		for(int i=0; i<fireLocArr.Length; i++){
			Info.fireLoc[i] = int.Parse(fireLocArr[i]+"");
		}
		char[] waterLocArr = (bricks ["waterLoc"]+"").ToCharArray ();
		Info.waterLoc = new int[waterLocArr.Length];
		for(int i=0; i<waterLocArr.Length; i++){
			Info.waterLoc[i] = int.Parse(waterLocArr[i]+"");
		}
		char[] windLocArr = (bricks ["windLoc"]+"").ToCharArray ();
		Info.windLoc = new int[windLocArr.Length];
		for(int i=0; i<windLocArr.Length; i++){
			Info.windLoc[i] = int.Parse(windLocArr[i]+"");
		}
		char[] brickIndex = (bricks ["brickIndex"]+"").ToCharArray ();
		Info.brickIndex = new int[10];
		for(int i=0; i<brickIndex.Length; i++){
			Info.brickIndex[i] = int.Parse(brickIndex[i]+"");
		}
		readyButton.SetActive (true);
	}	
	
	public void ParseResult(string text,int playerIndex){
		JsonData task = JsonMapper.ToObject (text);
		string taskType = (string)task["taskType"];
		
		if(taskType.Equals("ready")){
			ResponseReady(playerIndex);
		}
		else if(taskType.Equals("cancel")){
			ResponseCancel(playerIndex);
		}
		else if(taskType.Equals("join")){
			ResponseJoin((string)task["name"], playerIndex, (string)task["avatarSet"]);
		}
		else if(taskType.Equals("brick")){
			ResponseBrick(JsonMapper.ToObject((string)task["bricks"]));
		}
	}
	
	void GameStart(){
		Info.numOfPlayers = numOfPlayers;
		foreach(GameObject o in Info.characters){
			DontDestroyOnLoad(o);
		}Application.LoadLevel ("Multi_play");
	}
	
	
	public AudioClip button;
	public AudioClip alert;
	public AudioClip message;
	public AudioClip perfectSound;
	public AudioClip chat;
	void Chat(){
		AudioSource.PlayClipAtPoint (chat, Vector3.zero);
	}
	void Button(){
		AudioSource.PlayClipAtPoint (button, Vector3.zero);
	}
	void Alert(){
		AudioSource.PlayClipAtPoint (alert, Vector3.zero);
	}
	void Message(){
		AudioSource.PlayClipAtPoint (message, Vector3.zero);
	}
	public GameObject soundOffText;
	public void SoundOnOff(){
		Info.soundOn = !Info.soundOn;
		soundOffText.SetActive(!Info.soundOn);
		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
	}
}