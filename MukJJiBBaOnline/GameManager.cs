using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public class GameManager : MonoBehaviour {
	public Hand me;
	public Hand opponent;
	public CenterText text;
	public GameObject beginText;
	public GameObject playAgainPanel;
	public Button giveupButton;
	public AudioClip beginSound;
	public AudioClip applaus;
	public Fire fire;

	public PlayerPanel myPanel;
	public PlayerPanel oppPanel;

	public AudioClip ggbSound;

	private int mode = -1;
	private const int NONE = -1;
	private const int GAMBAMBO = 1;
	private const int OFFENCE = 2;
	private const int DEFENCE = 3;

	private int dmg;

	public void Start(){
		myPanel.SetName (Dic.myName);
		myPanel.SetLevel (Dic.myLevel);
		oppPanel.SetName (Dic.oppName);
		oppPanel.SetLevel (Dic.oppLevel);

		//send ready message
		StartCoroutine ("NewBegin");
	}

	IEnumerator NewBegin(){
		AudioSource.PlayClipAtPoint (beginSound, Vector3.zero);
		yield return new WaitForSeconds (3f);
		audio.Play ();
		StartCoroutine ("Begin");
		beginText.SetActive (false);
	}

	IEnumerator Begin(){
		me.StopShake ();
		opponent.StopShake ();
		dmg = 20;
		yield return new WaitForSeconds (1.5f);
		fire.Clear ();
		StartCoroutine( "CollectGBB" );
	}

	public IEnumerator CollectGBB(){
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
			mode = NONE;
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
			mode = NONE;
			TimeOver();
		}
	}
	IEnumerator StartOffence(){
		StopCoroutine( "CollectOffence" );
		me.StartShake ();
		#region AI
		int num = Random.Range(0, 3);
		opponent.gambamboInt = num;
		yield return new WaitForSeconds(0.5f);
		#endregion
		me.StopShake ();
		me.Shoot ();
		opponent.Shoot();
		
		yield return new WaitForSeconds(0.5f);

		if(WhoWin()==null){
			text.SetText("승리 !!");
			text.SetActive(true);
			if(opponent.Damage(dmg)==0) StartCoroutine( Win() );
			else StartCoroutine ("Begin");
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
		StartCoroutine( "CollectDefence" );
	}

	public IEnumerator CollectDefence(){
		me.gambamboInt = -3;
		opponent.gambamboInt = -3;
		#region AI	
		yield return new WaitForSeconds (Random.Range(0.5f, 2f));
		int n = Random.Range(0, 3);
		opponent.gambamboInt = n;
		#endregion
		
		mode = DEFENCE;
		opponent.StartShake();
		yield return new WaitForSeconds (0.5f);
		if(me.gambamboInt == -3){
			mode = NONE;
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
			else StartCoroutine ( "Begin");
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
			#region AI
			int num = Random.Range(0, 3);
			opponent.gambamboInt = num;
			//if GBB received
			StopCoroutine( "CollectGBB" );
			StartGBB ();
			#endregion
			mode = NONE;
			break;
		case OFFENCE:
			me.gambamboInt = i;
			me.StartShake ();
			//send offence
			//if defence received
			StartCoroutine( "StartOffence" );
			mode = NONE;
			break;
		case DEFENCE:
			me.gambamboInt = i;
			//send defence
			StartCoroutine( "ResultDefence" );
			mode = NONE;
			break;
		}
	}

	public void TimeOver(){
		//send timeover message
		text.SetText ("타임오버");
		text.SetActive(true);
		if(me.Damage(dmg)==0) StartCoroutine( Lose() );
		else StartCoroutine ( "Begin");
		//send ready message
	}
	public void OppTimeOver(){
		//send timeover message
		text.SetText ("타임오버");
		text.SetActive(true);
		if(opponent.Damage(dmg)==0) StartCoroutine( Win() );
		else StartCoroutine ( "Begin");
		//send ready message
	}

	public void GiveUp(){
		mode = NONE;
		Sound.Button ();
		StopCoroutine ("NewBegin");
		StopCoroutine ("Begin");
		StopCoroutine ("CollectGBB");
		StopCoroutine ("CompareGBB");
		StopCoroutine ("CollectOffence");
		StopCoroutine ("CollectDefence");
		StopCoroutine ("StartOffence");
		StopCoroutine ("ResultDefence");
		audio.Stop ();
		StartCoroutine( Lose() );
	}


	IEnumerator Lose(){	
		AudioSource.PlayClipAtPoint (applaus, Vector3.zero);	
		yield return new WaitForSeconds(3f);
		#if UNITY_ANDROID && !UNITY_EDITOR
		new StateListener ().Lose_com ();
		#endif
		PlayAgain ();
	}
	IEnumerator Win(){
		AudioSource.PlayClipAtPoint (applaus, Vector3.zero);
		yield return new WaitForSeconds(3f);
		#if UNITY_ANDROID && !UNITY_EDITOR
		new StateListener ().Win_com ();
		#endif
		PlayAgain ();
	}

	void PlayAgain(){
		playAgainPanel.SetActive (true);
	}
	
	public void PlayAgainYES(){
		Sound.Button ();
		Application.LoadLevel ("Single");
	}
	
	public void PlayAgainNO(){
		Sound.Button ();
		Quit ();
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
		StartCoroutine (Quit ());
	}

	IEnumerator Quit(){
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
