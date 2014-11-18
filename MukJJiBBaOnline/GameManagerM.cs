using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public class GameManagerM : MonoBehaviour {
	public Hand me;
	public Hand opponent;
	public CenterText text;
	public GameObject beginText;
	public GameObject playAgainPanel;
	public AlertPanel alertPanel;
	public Button giveupButton;
	public AudioClip beginSound;
	public AudioClip applaus;
	public Fire fire;

	public Malpoong myChat;
	public Malpoong oppChat;
	public InputField chatInput;

	public PlayerPanel myPanel;
	public PlayerPanel oppPanel;
	public GameObject networkPanel;

	public AudioClip ggbSound;

	private int mode = -1;
	private const int NONE = -1;
	private const int GAMBAMBO = 1;
	private const int OFFENCE = 2;
	private const int DEFENCE = 3;

	private int dmg;
	private bool isPlaying;
	private bool[] again = new bool[2];
	private bool oppDisc;

	private string beforeMsg;

	static RealTimeListener listener = WaitRoom.listener;

	public void Start(){
		listener.multi = this;
		myPanel.SetName (Dic.myName);
		myPanel.SetLevel (Dic.myLevel);
		oppPanel.SetName (Dic.oppName);
		oppPanel.SetLevel (Dic.oppLevel);

		StartCoroutine ("SendNewBegin");
	}

	IEnumerator SendNewBegin(){
		yield return new WaitForSeconds (2f);
		MessageSender.Send ("NewBegin");
	}

	public void StartCrt(string methodName){
		StartCoroutine (methodName);
	}

	public void OppNumReceived(string mode, int num){
		opponent.gambamboInt = num;
		if(mode.Equals("GBB")){			
			if(me.gambamboInt >= 0 && opponent.gambamboInt >= 0) StartGBB ();
		}
		if(mode.Equals("OFFENCE")){			
			StartCoroutine( "CollectDefence" );
		}
		if(mode.Equals("DEFENCE")){			
			StartCoroutine( "StartOffence" );
		}
	}

	public void SendChat(){
		if(beforeMsg != null && beforeMsg.Equals(chatInput.value) && !string.IsNullOrEmpty(chatInput.value)){
			return;
		}
		beforeMsg = chatInput.value;
		MessageSender.Send ("chat", "msg", chatInput.value);
		myChat.Say(chatInput.value);
	}

	public void ChatReceived(string msg){
		oppChat.Say(msg);
	}

	public IEnumerator NewBegin(){
		isPlaying = true;
		AudioSource.PlayClipAtPoint (beginSound, Vector3.zero);
		yield return new WaitForSeconds (3f);
		audio.Play ();
		StartCoroutine ("Begin");
		beginText.SetActive (false);
	}

	IEnumerator Begin(){
		me.StopShake ();
		opponent.StopShake ();
		yield return new WaitForSeconds (1.5f);
		fire.Clear ();
		StartCoroutine( "CollectGBB" );
	}

	public IEnumerator CollectGBB(){
		dmg = 20;
		me.gambamboInt = -1;
		opponent.gambamboInt = -1;
		mode = GAMBAMBO;
		text.SetActive (true);
		text.SetText ("가위");
		AudioSource.PlayClipAtPoint (ggbSound, Vector3.zero);
		yield return new WaitForSeconds (1f);
		text.SetText ("가위\n바위");
		AudioSource.PlayClipAtPoint (ggbSound, Vector3.zero);
		yield return new WaitForSeconds (1f);
		text.SetText ("가위\n바위\n보");
		AudioSource.PlayClipAtPoint (ggbSound, Vector3.zero);
		yield return new WaitForSeconds (1f);
		if(me.gambamboInt == -1){
			TimeOver();
		}
	}

	void StartGBB(){
		text.SetActive (false);
		opponent.GamBamBo ();
		me.GamBamBo();
		StartCoroutine("CompareGBB");
	}


	public IEnumerator CollectOffence(){
		me.gambamboInt = -2;
		opponent.gambamboInt = -2;
		mode = OFFENCE;
		//text.SetText ("공격");
		//text.SetActive (true);
		fire.Move(me);
		yield return new WaitForSeconds (3f);
		if(me.gambamboInt == -2){
			TimeOver();
		}
	}
	IEnumerator StartOffence(){

		me.StopShake ();
		me.Shoot ();
		opponent.Shoot();
		
		yield return new WaitForSeconds(0.5f);

		if(WhoWin()==null){
			text.SetText("승리 !!");
			text.SetActive(true);
			if(opponent.Damage(dmg)==0) StartCoroutine( Win() );
			else MessageSender.Send ("Begin");
		}else if(WhoWin().Equals(me)){
			dmg++;
			StartCoroutine( "CollectOffence" );
		}else{
			dmg++;
			WaitDefence();
		}
	}

	public void WaitDefence(){
		//text.SetText ("방어");
		//text.SetActive (true);
		fire.Move(opponent);

		//if offence received
		//StartCoroutine( "CollectDefence" );
	}

	public IEnumerator CollectDefence(){
		me.gambamboInt = -3;
		
		mode = DEFENCE;
		opponent.StartShake();
		yield return new WaitForSeconds (0.5f);
		if(me.gambamboInt == -3){
			TimeOver();
		}
	}

	public IEnumerator ResultDefence(){
		StopCoroutine( "CollectDefence" );
		opponent.StopShake();		
		me.Shoot ();
		opponent.Shoot();

		yield return new WaitForSeconds(0.5f);

		if(WhoWin()==null){
			text.SetText("패배 !!");
			text.SetActive(true);
			if(me.Damage(dmg)==0) StartCoroutine( Lose() );
			else MessageSender.Send ("Begin");
		}else if(WhoWin().Equals(me)){
			dmg++;
			StartCoroutine( "CollectOffence" );
		}else{
			dmg++;				
			WaitDefence();
		}
	}

	public void MJBBtnClick(int i){
		switch(mode){
		case GAMBAMBO:
			me.gambamboInt = i;
			MessageSender.Send("GBB", "num", i+"");
			StopCoroutine( "CollectGBB" );
			if(me.gambamboInt >= 0 && opponent.gambamboInt >= 0) StartGBB ();
			mode = NONE;
			break;
		case OFFENCE:
			me.gambamboInt = i;
			MessageSender.Send("OFFENCE", "num", i+"");
			StopCoroutine( "CollectOffence" );
			me.StartShake ();
			mode = NONE;
			break;
		case DEFENCE:
			me.gambamboInt = i;
			MessageSender.Send("DEFENCE", "num", i+"");
			StartCoroutine( "ResultDefence" );
			mode = NONE;
			break;
		}
	}

	public void TimeOver(){
		StopAllCrt ();
		mode = NONE;
		MessageSender.Send ("timeover");
		text.SetText ("타임오버");
		text.SetActive(true);
		if(me.Damage(dmg)==0) StartCoroutine( Lose() );
		else MessageSender.Send ("Begin");
	}
	public void OppTimeOver(){
		StopAllCrt ();
		mode = NONE;
		text.SetText ("상대\n타임오버");
		text.SetActive(true);
		if(opponent.Damage(dmg)==0) StartCoroutine( Win() );
		else MessageSender.Send ("Begin");
	}

	public void GiveUp(){
		mode = NONE;
		isPlaying = false;
		Sound.Button ();
		StopAllCrt ();
		audio.Stop ();
		StartCoroutine( Lose() );
		MessageSender.Send ("giveup");
	}
	public void GiveUpReceived(){
		mode = NONE;
		beginText.SetActive (false);
		isPlaying = false;
		StopAllCrt ();		
		text.SetActive (true);
		text.SetText ("상대방\n기권.");
		audio.Stop ();
		StartCoroutine( Win() );
	}
	void StopAllCrt(){
		StopCoroutine ("NewBegin");
		StopCoroutine ("Begin");
		StopCoroutine ("CollectGBB");
		StopCoroutine ("CompareGBB");
		StopCoroutine ("CollectOffence");
		StopCoroutine ("CollectDefence");
		StopCoroutine ("StartOffence");
		StopCoroutine ("ResultDefence");
	}

	IEnumerator Lose(){	
		StopAllCrt ();
		isPlaying = false;
		AudioSource.PlayClipAtPoint (applaus, Vector3.zero);	
		yield return new WaitForSeconds(3f);
		#if UNITY_ANDROID && !UNITY_EDITOR
		new StateListener ().Lose ();
		#endif
		PlayAgain ();
	}
	IEnumerator Win(){
		StopAllCrt ();
		isPlaying = false;
		AudioSource.PlayClipAtPoint (applaus, Vector3.zero);
		yield return new WaitForSeconds(3f);
		#if UNITY_ANDROID && !UNITY_EDITOR
		new StateListener ().Win ();
		#endif
		PlayAgain ();
	}

	void PlayAgain(){
		playAgainPanel.SetActive (true);
	}
	
	public void PlayAgainYES(){
		playAgainPanel.SetActive (false);
		giveupButton.enabled = false;
		Sound.Button ();
		MessageSender.Send ("again");
		again [0] = true;
		if(again[0] && again[1]){
			Application.LoadLevel("Multi");
		}
	}
	
	public void PlayAgainNO(){
		Sound.Button ();
		Quit ();
	}

	public void AgainReceived(){
		again [1] = true;
		if(again[0] && again[1]){
			Application.LoadLevel("Multi");
		}
	}

	IEnumerator CompareGBB(){
		yield return new WaitForSeconds (2f);
		if(WhoWin() == null){
			dmg++;
			StartCoroutine( "CollectGBB" );
		}else if(WhoWin().Equals(me)){
			StartCoroutine( "CollectOffence" );
		}else{			
			#region AI
			WaitDefence();
			#endregion
		}
	}


	public Hand WhoWin(){
		if(me.gambamboInt == opponent.gambamboInt) return null;
		if(me.gambamboInt == 0){
			if(opponent.gambamboInt == 1) return me;
			if(opponent.gambamboInt == 2) return opponent;
		}
		if(me.gambamboInt == 1){
			if(opponent.gambamboInt == 0) return opponent;
			if(opponent.gambamboInt == 2) return me;
		}
		if(me.gambamboInt == 2){
			if(opponent.gambamboInt == 0) return me;
			if(opponent.gambamboInt == 1) return opponent;
		}
		return null;
	}

	public void QuitGame(){
		Sound.Button ();
		StartCoroutine (Quit ());
	}

	public void OppDisc(){
		StopAllCrt ();
		Debug.Log ("OppDisc");
		oppDisc = true;
		Sound.Error();
		alertPanel.Alert ("상대방이\n 퇴장했습니다.");
		if(!isPlaying) return;
		#if UNITY_ANDROID && !UNITY_EDITOR
		new StateListener ().Win ();
		#endif
	}
	
	IEnumerator MeDisc(){
		if(!isPlaying) StopCoroutine("MeDisc");
		StopAllCrt ();
		yield return new WaitForSeconds (3f);
		if(oppDisc){
		}else{
			Sound.Error();
			alertPanel.Alert ("접속이\n 끊겼습니다.");
			#if UNITY_ANDROID && !UNITY_EDITOR
			new StateListener ().Lose ();
			#endif
		}
	}

	IEnumerator StartNetworkCheck(){
		yield return new WaitForSeconds (10f);
		networkPanel.SetActive (true);
		WWW www = new WWW ("http://google.com");
		yield return new WaitForSeconds (3f);
		giveupButton.enabled = false;
		networkPanel.SetActive (false);
		if(!www.isDone || !string.IsNullOrEmpty(www.error)) {
			Sound.Error();
			alertPanel.Alert ("접속이\n 끊겼습니다.");
			if(!isPlaying) StopCoroutine("StartNetworkCheck");
			#if UNITY_ANDROID && !UNITY_EDITOR
			new StateListener ().Lose ();
			#endif
		}else {
			OppDisc();
		}
	}
	public void StopNetworkCheck(){
		StopCoroutine ("StartNetworkCheck");
		networkPanel.SetActive (false);
	}

	IEnumerator Quit(){
		alertPanel.gameObject.SetActive (false);
		playAgainPanel.SetActive (false);
		giveupButton.enabled = false;
		#if UNITY_ANDROID && !UNITY_EDITOR
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
		using (AndroidJavaObject activity = GetActivity()) {
			activity.Call<bool>("LoadInterstitial");
		}
		#endif
		yield return new WaitForSeconds (5f);
		Application.LoadLevel ("Main");
	}
	internal AndroidJavaObject GetActivity() {
		using (AndroidJavaClass activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			return activity.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}
}
