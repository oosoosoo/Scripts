using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using LitJson;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;

public class GameManager : MonoBehaviour {
	public GameObject resultPanel;
	public Text[] resultText;

	private float[] playerScore = new float[4];
	public GameObject[] playerScoreImage;
	public Text[] playerScoreText;
	public Text[] playerScoreNumber;

	public GameObject accuracyObj;

	private int turnCount;
	private int[] dropVote = new int[4];

	public AudioClip applaus;
	public AudioClip victory;
	public AudioClip wrong;
	public AudioSource bgm;

	public GameObject aim;
	public GameObject magnetCircle;
	public ParticleSystem ptcl;
	public ParticleSystem rockPrtcl;

	public GameObject yourTurnText;
	public AudioClip yourTurnAudio;
	public GameObject countdown;

	public Texture2D[] itemImages;
	public GameObject baloon;
	public GameObject itemAni;
	private ArrayList baloons = new ArrayList();
	[HideInInspector]
	public ArrayList items = new ArrayList();
	
	[HideInInspector]
	public GameObject[] characters;
	private GameObject myCharacter;
	[HideInInspector]
	public Vector2[] brickPos;
	[HideInInspector]
	public GameObject[] brickObj = new GameObject[10];
	private int[] brickType;
	private int[] charBrickIndex = new int[4];
	private Vector2[] playerPos = new Vector2[4];
	private int[] fireLoc;
	private int[] waterLoc;
	private int[] windLoc;

	public int thisTurn = -1;
	private int myTurn = -1;
	private int turnForItem = -1; //item generating turn
	private int formerTurn = -1;
	private int[] extraJumpChance = new int[4];

	private GameObject camFront;
	public GameObject deadLine;	
	public GameObject[] brickPref;
	public GameObject fire;
	public GameObject water;
	public GameObject wind;
	public GUIStyle style;
	public GUIStyle labelStyle;

	private ArrayList chats = new ArrayList();
	private int numOfPlayers;
	private int readyCount;

	public GameObject quitPanel;
	public GameObject allLeftPanel;
	public Text chatText;
	public InputField chatInputField;

	private bool isPlaying = true;
	private RealTimeListener listener = MainManager.listener;


	public GameObject dropPanel;

	private bool quit;
	void Start(){
		FindObjectOfType<AdMobScript> ().Destroy();

		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
		soundOffText.SetActive(!Info.soundOn);

		listener.gameManager = this;
		listener.STATUS = RealTimeListener.PLAYING;
		numOfPlayers = Info.numOfPlayers;
		characters = Info.characters;
		brickPos = Info.bricks;
		brickType = Info.brickIndex;
		fireLoc = Info.fireLoc;
		waterLoc = Info.waterLoc;
		windLoc = Info.windLoc;
		myTurn = Info.myTurn;
		bgm.Play ();
		myCharacter = characters [myTurn];
		if(!Application.loadedLevelName.Equals ("Multi_play")) return;
		camFront = Camera.main.gameObject;

		//player setting
		for(int i=0; i<characters.Length; i++){
			GameObject obj = characters[i];
			if(obj != null){
				obj.GetComponent<Character>().gameManager = this;
				obj.GetComponent<Character>().landingPrtcl = ptcl;
				obj.GetComponent<Character>().rockPrtcl = rockPrtcl;
				obj.transform.position = new Vector3(0,0,-7);
			}
		}

		for(int i=0; i<charBrickIndex.Length; i++){
			charBrickIndex[i] = -1;
			GameObject obj = characters[i];
			if(obj != null){
				obj.GetComponent<Character>().charBrickIndex = -1;
			}
		}

		for(int i=0; i<playerPos.Length; i++){
			playerPos[i] = new Vector2();
		}

		for(int i=0; i<10; i++){
			brickObj[i] = Instantiate(brickPref[ brickType[i] ], new Vector3(brickPos[i].x,brickPos[i].y,10), new Quaternion()) as GameObject;
		}

		for(int i=0; i<fireLoc.Length; i++){
			float x = brickPos[ fireLoc[i] ].x - (brickPos[ fireLoc[i] ].x - brickPos[fireLoc[i]-1].x)*0.5f ;
			Instantiate(fire,new Vector3(x, -8), new Quaternion());
		}

		for(int i=0; i<waterLoc.Length; i++){
			float x = brickPos[ waterLoc[i] ].x - (brickPos[ waterLoc[i] ].x - brickPos[waterLoc[i]-1].x)*0.5f ;
			Instantiate(water,new Vector3(x, 8), new Quaternion());
		}

		for(int i=0; i<windLoc.Length; i++){
			float x = brickPos[ windLoc[i] ].x - (brickPos[ windLoc[i] ].x - brickPos[windLoc[i]-1].x)*0.5f ;
			Instantiate(wind,new Vector3(x, 4), new Quaternion());
		}

		for(int i=0; i<playerScoreText.Length; i++){
			if(characters[i]!=null){
				playerScoreText[i].gameObject.SetActive(true);
				playerScoreNumber[i].gameObject.SetActive(true);
				playerScoreText[i].text = characters[i].GetComponent<Character>().myName;
			}
		}

		RequestReady ();
	}
	public IEnumerator checkSuccess(GameObject chr, GameObject brick, int accuracy){
		int moveBrickIndex = charBrickIndex[thisTurn];
		int succBrickIndex = System.Array.IndexOf(brickObj, brick);
		
		if(brick.Equals (brickObj[ charBrickIndex[thisTurn]+1 ])){
			accuracyObj.transform.position = new Vector3(characters[thisTurn].transform.position.x, characters[thisTurn].transform.position.y-1, -30);
			accuracyObj.GetComponent<TextMesh> ().text = accuracy +"";
			if(accuracy >= 90f) {
				Perfect();
				accuracyObj.GetComponent<Animator>().SetTrigger("perfect");
				playerScore[thisTurn] += 2;
				playerScoreNumber[thisTurn].text = ""+playerScore[thisTurn];
			}else{
				Success();
				accuracyObj.GetComponent<Animator>().SetTrigger("normal");
				playerScore[thisTurn] += accuracy*0.01f;
				playerScoreNumber[thisTurn].text = ""+playerScore[thisTurn];
			}
		}

		if(chr.Equals(characters[thisTurn]) && chr.GetComponent<Character>().exchange){
			if(brick.Equals (brickObj[ charBrickIndex[thisTurn]+1 ]) || chr.GetComponent<Character>().freeJump ){
				chr.GetComponent<Character>().Rock();
				brick.GetComponentInChildren<Animator>().SetTrigger("rock");
			}
			for(int i = 0; i<4; i++){
				GameObject o = characters[i];
				bool succ = false;
				if(o != null && !o.Equals(chr) && charBrickIndex[i] == succBrickIndex){
					succ = true;
					StartCoroutine( o.GetComponent<Character>().Kicked() ); 
					Vector3 moveP = new Vector3(0, 20, -7);
					if(moveBrickIndex != -1)
						moveP = new Vector3(brickObj[moveBrickIndex].transform.position.x, brickObj[moveBrickIndex].transform.position.y+20, -7);
					StartCoroutine( ResponseMove(i, moveP, moveBrickIndex) );
					charBrickIndex[i] = moveBrickIndex;
					characters[i].GetComponent<Character>().charBrickIndex = moveBrickIndex;
				}				
				if(succ)	{
					playerPos[thisTurn] = chr.transform.position;
					charBrickIndex[thisTurn] = succBrickIndex;
					characters[thisTurn].GetComponent<Character>().charBrickIndex = succBrickIndex;
				}
			}
		}
		if(thisTurn == myTurn){
			if(chr.Equals(myCharacter) && chr.GetComponent<Character>().freeJump){
				bool succ = false;
				for(int i=0; i<4; i++){
					if(charBrickIndex[i] == succBrickIndex) succ = true;
				}
				if(succ)	{
					playerPos[myTurn] = chr.transform.position;
					charBrickIndex[myTurn] = succBrickIndex;
					characters[myTurn].GetComponent<Character>().charBrickIndex = succBrickIndex;
				}
			}
			if(chr.Equals(myCharacter) && brick.Equals (brickObj[ charBrickIndex[thisTurn]+1 ])){//matches next brick
				yield return new WaitForSeconds(0.5f);
				playerPos[myTurn] = chr.transform.position;
				charBrickIndex[myTurn]++;
				characters[myTurn].GetComponent<Character>().charBrickIndex++;
			}else{
				yield return new WaitForSeconds(0.5f);
			}
			RequestTurn();
		}
	}

	// Update is called once per frame
	void Update () {		
		if(Input.GetKey(KeyCode.Escape)){ 
			QuitActive(true);
		}
	}

	public void Turn(){	
		turnCount++;
		dropPanel.SetActive (false);
		//remove all items on the map
		foreach (GameObject o in baloons) {
			Destroy(o);
		}baloons.Clear ();
		foreach (GameObject o in characters) {
			if(o != null)	o.GetComponent<Character> ().Reset ();
		}
		magnetCircle.GetComponent<MagnetCircle> ().Reset ();

		for(int i=0; i<4; i++){
			if(charBrickIndex[i]==9){
				isPlaying = false;
				characters[i].GetComponent<Character>().Victory();
				bgm.Stop();
				if(thisTurn == myTurn) StartCoroutine( Win() );
				else StartCoroutine( Lose() );
				return;
			}
		}

		if(thisTurn == -1) thisTurn++;
		if(extraJumpChance[thisTurn] > 0){
			extraJumpChance[thisTurn]--;
		}else{
			thisTurn++;
			if(thisTurn >= 4){ 
				thisTurn = 0;
				turnForItem++;
			}
			while(characters[thisTurn]==null){
				thisTurn++;
				if(thisTurn >= 4){ 
					thisTurn = 0;
					turnForItem++;
				}
			}
		}
		
		for(int i=0; i<playerScoreImage.Length; i++){
			playerScoreImage[i].SetActive(false);
		}playerScoreImage[thisTurn].SetActive(true);



		aim.GetComponent<Aim> ().Reset ();
		aim.SetActive (false);
		aim.GetComponent<Aim> ().character = characters[thisTurn];
		
		characters[thisTurn].transform.position = new Vector3(characters[thisTurn].transform.position.x,characters[thisTurn].transform.position.y,-9);
		aim.transform.position = characters[thisTurn].transform.position;
		camFront.GetComponent<Camwalk> ().playerPos = characters[thisTurn].transform;
		deadLine.GetComponent<DeadLine> ().character = characters[thisTurn].transform;
		
		if(turnForItem%3 == 0 && formerTurn != thisTurn){ //generate item
			baloons.Add( Instantiate(baloon, characters[thisTurn].transform.position+new Vector3(1.5f, 3.5f,0), new Quaternion()) );
		}formerTurn = thisTurn;

		StartCoroutine (StartCountDrop(turnCount));
		if(thisTurn == myTurn){
			StartCoroutine("StartCountDown");
			aim.SetActive (true);
			yourTurnText.SetActive(true);
			yourTurnText.GetComponent<Animator>().SetTrigger("Turn");
			AudioSource.PlayClipAtPoint(yourTurnAudio, Vector3.one);
			StartCoroutine(RemoveObj(yourTurnText,1.5f));
		}else{
			characters[thisTurn].GetComponentInChildren<Animator> ().SetTrigger ("charge");
		}
	}

	public void RequestAddForce(Vector2 forcePos){
		StopCoroutine ("StartCountDown");
		countdown.SetActive (false);
		MessageSender.Send ("addforce", new string[]{"forcePosx", "forcePosy"}, new string[]{forcePos.x+"", forcePos.y+""});
		ResponseAddForce (myTurn, forcePos);
	}
	
	public void ResponseAddForce(int playerIndex, Vector2 forcePos){
		characters[playerIndex].GetComponent<Character> ().AddForce (forcePos);
	}

	public void Fail(GameObject o){
		if(o.Equals (myCharacter))	RequestTurn ();
	}
	
	public void MoveCharacter(int index){
		if(thisTurn != myTurn) return;
		MoveCharacter (thisTurn, index);
	}

	public void MoveCharacter(int playerIndex, int index){
		if(thisTurn != myTurn) return;
		index = Mathf.Clamp (charBrickIndex[thisTurn]+index, 0, 9); // current brick index = charBrickIndex[thisTurn], index will ++ at CheckSuccess()
		float randomRange = Random.Range (-0.15f, 0.15f);
		randomRange += (randomRange >= 0 ? 0.2f : -0.2f);
		Vector2 movePos = new Vector2 (brickObj[index].transform.position.x+randomRange, brickObj[index].transform.position.y + 20);
		RequestMove (movePos, index);
	}
	
	void RequestMove(Vector2 movePos, int index){
		MessageSender.Send ("move", new string[]{"movePosx", "movePosy", "moveIndex"}, new string[]{movePos.x+"", movePos.y+"", index+""});
		StartCoroutine( ResponseMove (myTurn, movePos, index) );
	}

	public IEnumerator ResponseMove(int playerIndex, Vector2 movePos, int moveIndex){
		StartCoroutine( characters[playerIndex].GetComponent<Character>().Kicked() );
		yield return new WaitForSeconds(0.5f);

		if (playerIndex == thisTurn) {
			charBrickIndex [playerIndex] = moveIndex - 1;
			characters[playerIndex].GetComponent<Character>().charBrickIndex = moveIndex - 1;
		}

		characters[playerIndex].rigidbody2D.gravityScale = 2;
		characters[playerIndex].transform.position = movePos;
		characters[playerIndex].GetComponent<Character>().charBrickIndex = moveIndex;
		float y = 0f;
		if(moveIndex != -1){
			y = brickObj [moveIndex].transform.position.y + 4f;
		}
		playerPos [playerIndex] = new Vector2 (movePos.x, y);
	}

	public void RequestChat(){
		Button ();
		string chatMsg = chatInputField.text.text;
		if(string.IsNullOrEmpty(chatMsg)) return;
		MessageSender.Send ("chat", new string[]{"playerName", "chatMsg"}, new string[]{Info.myName, chatMsg});
		ResponseChat (Info.myName, chatMsg);
		chatInputField.text.text = "";
		chatInputField.value = "";
	}

	public void ResponseChat(string playerName, string chat){
		Chat ();
		chats.Add (playerName + " : " + chat);
		if(chats.Count > 5) chats.RemoveAt(0);
		chatText.text = "";
		foreach(string s in chats){
			chatText.text += s+"\n";
		}
	}
	
	public void QuitActive(bool b){
		Button ();
		quitPanel.SetActive (b);
	}

	public void QuitGame(){
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
		foreach(GameObject o in Info.characters){
			Destroy(o);
		}
		Application.LoadLevel("Main");
	}

	IEnumerator StartCountDown(){
		yield return new WaitForSeconds (5f);
		countdown.GetComponent<CountDown> ().Reset();
		countdown.SetActive (true);
		yield return new WaitForSeconds (5f);
		RequestTurn ();
	}
	IEnumerator StartCountDrop(int count){
		yield return new WaitForSeconds (13f);
		dropPanel.GetComponent<DropPanel> ().SetSec (10);
		if(count >= turnCount && isPlaying){
			dropPanel.SetActive (true);
			StartCoroutine( RequestDrop(count) );
		}
	}
	IEnumerator RequestDrop(int count){
		yield return new WaitForSeconds (3f);
		if(count >= turnCount && isPlaying){
			if(numOfPlayers <= 2){
				StartCoroutine( SelfDropCheck(thisTurn) );
				yield return null;
			}else{
				MessageSender.Send("drop", "dropIndex", ""+thisTurn);
				ResponseDrop(thisTurn);
			}
		}
		yield return new WaitForSeconds (3f);
		if(count >= turnCount && isPlaying){
			if(numOfPlayers >= 2){
				for(int i=0; i<4; i++){
					if(dropVote [i] >= 2) yield return null;
				}
				StartCoroutine( LeftRoomOK() );
			}
		}
	}
	void ResponseDrop(int dropIndex){
		dropVote [dropIndex]++;
		for(int i=0; i<4; i++){
			if(numOfPlayers > 2 && dropVote[i] >= numOfPlayers-1){
				PeerDisconnected(Info.playerIdList[i] as string);
			}
		}
	}

	IEnumerator SelfDropCheck(int dropIndex){
		WWW www = new WWW ("http://www.google.com");
		yield return new WaitForSeconds (3f);
		if(!www.isDone || !string.IsNullOrEmpty(www.error)) {
			allLeftPanel.SetActive (true);
			StopCoroutine("StartCountDown");
		}else {
			PeerDisconnected(Info.playerIdList[dropIndex] as string);
		}
	}

	IEnumerator RemoveObj(GameObject obj, float f){
		yield return new WaitForSeconds (f);
		obj.SetActive (false);
	}

	void OnApplicationQuit(){
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
	}

	public void AddItem(int itemNo){
		if(thisTurn == myTurn){
			if(items.Count >= 5) items.RemoveAt (0);
			items.Add (itemNo);
		}
	}

	IEnumerator Win(){
		isPlaying = false;
		AudioSource.PlayClipAtPoint(applaus, new Vector3(), 0.5f);
		AudioSource.PlayClipAtPoint(victory, new Vector3(), 0.5f);
		StopCoroutine ("StartCountDown");
		StopCoroutine ("StartCountDrop");
		dropPanel.SetActive (false);
		aim.SetActive (false);
		int score = Mathf.CeilToInt (playerScore [myTurn]) + Mathf.CeilToInt ((charBrickIndex [myTurn]+1)*0.5f);
		new StateListener ().ReportScore (score);
		yield return new WaitForSeconds (5f);
		resultPanel.SetActive (true);
		resultText [0].text = "You win.\n you've got "+Mathf.CeilToInt (playerScore [myTurn])+"+"+Mathf.CeilToInt ((charBrickIndex [myTurn]+1)*0.5f)+" points.";
		resultText [1].text = "You win.\n you've got "+Mathf.CeilToInt (playerScore [myTurn])+"+"+Mathf.CeilToInt ((charBrickIndex [myTurn]+1)*0.5f)+" points.";

	}
	IEnumerator Lose(){
		isPlaying = false;
		AudioSource.PlayClipAtPoint(applaus, new Vector3(), 0.5f);
		AudioSource.PlayClipAtPoint(victory, new Vector3(), 0.5f);
		StopCoroutine ("StartCountDown");
		StopCoroutine ("StartCountDrop");
		dropPanel.SetActive (false);
		aim.SetActive (false);
		int score = Mathf.CeilToInt (playerScore [myTurn]);
		new StateListener ().ReportScore (score);
		yield return new WaitForSeconds (5f);
		resultPanel.SetActive (true);
		resultText [0].text = "You lose.\n you've got "+Mathf.CeilToInt (playerScore [myTurn])+" points.";
		resultText [1].text = "You lose.\n you've got "+Mathf.CeilToInt (playerScore [myTurn])+" points.";
	}

	public void WinLoseClose(){		
		resultPanel.SetActive (false);
		quit = true;
		FindObjectOfType<AdMobScript> ().ShowInterstitial();
	}

	void OnGUI(){
		//item
		for(int i=0; i<items.Count; i++){
			if(GUI.Button (new Rect(Screen.width-Screen.height*0.125f, Screen.height*0.375f+Screen.height*0.125f*i, Screen.height*0.125f, Screen.height*0.125f), itemImages[(int)items[i]])){
				if((int)items[i]==1 && charBrickIndex[myTurn]==8){
					AudioSource.PlayClipAtPoint(wrong, characters[thisTurn].transform.position);
					return;
				}
				if((int)items[i]==4 && charBrickIndex[thisTurn]==-1){
					AudioSource.PlayClipAtPoint(wrong, characters[thisTurn].transform.position);
					return;
				}
				if(thisTurn==myTurn && !characters[thisTurn].GetComponent<Character>().onAir && characters[thisTurn].GetComponent<Character>().landed){
					RequestItem((int)items[i]);
					items.RemoveAt(i);
				}else{
					AudioSource.PlayClipAtPoint(wrong, characters[thisTurn].transform.position);
				}
			}
		}
	}

	public void ResponseItem(int playerIndex, int itemNo){
		itemAni.GetComponent<ItemAni>().spriteChild.sprite = itemAni.GetComponent<ItemAni>().itemSprite [itemNo];
		Object ob = Instantiate(itemAni, characters[playerIndex].transform.position+new Vector3(0,2,0), new Quaternion());
		itemAni.GetComponent<ItemAni>().ItemUse();
		Destroy (ob, 0.5f);
		switch(itemNo){
		case 0://exchange
			characters[playerIndex].GetComponent<Character>().exchange = true;
			//characters[playerIndex].GetComponent<Character>().freeJump = true;
			break;
		case 1:	//go1
			characters[playerIndex].GetComponent<Character>().onAir = true;
			if(playerIndex == myTurn){
				aim.SetActive(false);
				MoveCharacter(+1);
			}
			break;
		case 2: //+1
			extraJumpChance[playerIndex]++;
			break;
		case 3: //magnet
			magnetCircle.GetComponent<MagnetCircle>().character = characters[playerIndex];
			characters[playerIndex].GetComponent<Character>().magnetCircle = magnetCircle;
			characters[playerIndex].GetComponent<Character>().freeJump = true;
			break;
		case 4://kick
			characters[playerIndex].GetComponent<Character>().Rock();
			//yield return new WaitForSeconds(0.2f);
			brickObj[ charBrickIndex[playerIndex] ] .GetComponentInChildren<Animator>().SetTrigger("rock");
			for(int i = 0; i < 4; i++){
				GameObject o = characters[i];
				if(o != null && !o.Equals (characters[playerIndex])){
					if(charBrickIndex[i] == charBrickIndex[playerIndex]){
						int moveBrickIndex = charBrickIndex[i]-1;
						Vector3 moveP = new Vector3(0, 20, -7);
						if(moveBrickIndex != -1) {
							moveP = new Vector3(brickObj[moveBrickIndex].transform.position.x, brickObj[moveBrickIndex].transform.position.y+20, -7);
						}
						StartCoroutine( ResponseMove(i, moveP, moveBrickIndex) );
						charBrickIndex[i] = moveBrickIndex;
						characters[i].GetComponent<Character>().charBrickIndex = moveBrickIndex;
					}
				}
			}
			break;
		case 5:
			break;
		}
	}

	void RequestReady(){
		MessageSender.Send ("ready");
		ResponseReady (myTurn);
	}

	void ResponseReady(int playerIndex){
		readyCount++;
		if(readyCount == numOfPlayers) Turn();
	}

	void RequestTurn(){
		TurnData data = new TurnData ();
		data.charBrickIndex = charBrickIndex;
		float[] posx = new float[4];
		float[] posy = new float[4];
		for(int i=0; i<4; i++){
			posx[i] = playerPos[i].x;
			posy[i] = playerPos[i].y;
		}
		data.playerPosx = posx;
		data.playerPosy = posy;
		data.extraJumpChance = extraJumpChance;

		MessageSender.Send ("turn", "turnData", data.ToString ());
		ResponseTurn (data, myTurn);
	}

	void ResponseTurn(TurnData data, int playerIndex){
		if(thisTurn != playerIndex) return;
		for(int i=0; i<4; i++){
			playerPos[i].x = data.playerPosx[i];
			playerPos[i].y = data.playerPosy[i];
		}
		charBrickIndex = data.charBrickIndex;
		for(int i=0; i<charBrickIndex.Length; i++){
			if(characters[i] != null)	characters[i].GetComponent<Character>().charBrickIndex = charBrickIndex[i];
		}
		extraJumpChance = data.extraJumpChance;
		for(int i=0; i<4; i++){
			if(characters[i] != null){
				characters[i].transform.position = playerPos[i];
			}
		}
		Turn ();
	}

	void RequestItem(int itemNo){
		if(itemNo == 1) StopCoroutine ("StartCountDown");
		MessageSender.Send ("item", "itemNo", itemNo+"");
		ResponseItem(myTurn, itemNo);
	}

	public void LeftRoom(){
		StartCoroutine (LeftRoomOK ());
	}

	IEnumerator LeftRoomOK(){
		yield return new WaitForSeconds (3f);
		if(isPlaying){
			if(numOfPlayers >= 2 ){
				allLeftPanel.SetActive (true);
				StopCoroutine("StartCountDown");
			}
		}else if(quit){
			QuitGame();
		}
	}

	public void PeerDisconnected(string participantId){
		int playerIndex = Info.playerIdList.IndexOf(participantId);
		if(playerIndex == myTurn) LeftRoomOK();
		ResponseChat("SYSTEM" , characters[playerIndex].GetComponent<Character>().myName+" has left the game.");
		if(characters[playerIndex] != null){
			Destroy(Info.characters[playerIndex]);
			characters[playerIndex] = null;
			playerScoreNumber[playerIndex].gameObject.SetActive(false);
			playerScoreText[playerIndex].gameObject.SetActive(false);
			numOfPlayers--;
		}
		if(numOfPlayers <= 1 && isPlaying){
			StartCoroutine( Win() );
			return;
		}
		if(playerIndex == thisTurn && isPlaying) Turn ();
	}
	public void RealTimeMessageReceived(string senderId, byte[] data){
		string str = System.Text.Encoding.UTF8.GetString(data);
		ParseResult (str, Info.playerIdList.IndexOf (senderId));
	}

	public void ParseResult(string text,int playerIndex){	
		JsonData task = JsonMapper.ToObject (text);
		string taskType = (string)task["taskType"];
		
		if(taskType.Equals("ready")){
			ResponseReady(playerIndex);
		}
		else if(taskType.Equals("addforce")){
			Vector2 v = new Vector2();
			v.x = float.Parse((string)task["forcePosx"]);
			v.y = float.Parse((string)task["forcePosy"]);
			ResponseAddForce(playerIndex, v);
		}
		else if(taskType.Equals("turn")){
			string s = (string)task["turnData"];
			TurnData data = new TurnData(s);
			ResponseTurn(data, playerIndex);
		}
		else if(taskType.Equals("item")){
			int itemNo = int.Parse((string)task["itemNo"]);
			ResponseItem(playerIndex, itemNo);
		}
		else if(taskType.Equals("move")){
			float posx = float.Parse((string)task["movePosx"]);
			float posy = float.Parse((string)task["movePosy"]);
			int moveIndex = int.Parse((string)task["moveIndex"]);
			Vector2 movePos = new Vector2(posx, posy);
			StartCoroutine( ResponseMove(playerIndex, movePos, moveIndex) );
		}
		else if(taskType.Equals("chat")){
			string playerName = (string)task["playerName"];
			string chatMsg = (string)task["chatMsg"];
			ResponseChat(playerName, chatMsg);
		}
		else if(taskType.Equals("drop")){
			int dropIndex = int.Parse((string)task["dropIndex"]);
			ResponseDrop(dropIndex);
		}
	}
	public AudioClip button;
	public AudioClip alert;
	public AudioClip chatSound;
	public AudioClip perfect;
	public AudioClip success;
	void Success(){
		AudioSource.PlayClipAtPoint (success, Vector3.zero);
	}
	void Perfect(){
		AudioSource.PlayClipAtPoint (perfect, Vector3.zero);
	}
	void Chat(){
		AudioSource.PlayClipAtPoint (chatSound, Vector3.zero);
	}
	void Button(){
		AudioSource.PlayClipAtPoint (button, Vector3.zero);
	}
	void Alert(){
		AudioSource.PlayClipAtPoint (alert, Vector3.zero);
	}

	public void Invited(string invitorName){
		ResponseChat ("SYSTEM", invitorName + " invited you. (canceled)");
	}
	public GameObject soundOffText;
	public void SoundOnOff(){
		Info.soundOn = !Info.soundOn;
		soundOffText.SetActive(!Info.soundOn);
		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
	}
}
