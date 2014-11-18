using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;
using System.Security.Cryptography;
using System;
using System.Text;


public class MainManager : MonoBehaviour {
	public GameObject loadingPanel;
	public Image progressBar;
	private int loadStep;

	public static string id = "studio";
	public GameObject character;

	public static RealTimeListener listener = new RealTimeListener();
	
	[HideInInspector]
	public GameObject mainCanvas;
	public GameObject quitPanel;
	public GameObject playPanel;
	public GameObject waitFailPanel;

	public GameObject invitationPopup;

	void Start () {
		StartCoroutine (Jump ());

		FindObjectOfType<AdMobScript> ().Interstitial();
		FindObjectOfType<AdMobScript> ().Request ();
		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
		soundOffText.SetActive(!Info.soundOn);


		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		listener.mainManager = this;
		listener.STATUS = RealTimeListener.LOBBY;

		if(!string.IsNullOrEmpty(Info.myName)){ 			
			loadingPanel.SetActive(false);
			character.GetComponent<Character>().SetName(Info.myName);
			if(Info.avatarSet != null)	character.GetComponent<Character>().SetAvatar(Info.avatarSet);
			playPanel.SetActive(true);
			return;
		}

		PlayGamesPlatform.DebugLogEnabled = false;
		PlayGamesPlatform.Activate ();
		Social.localUser.Authenticate ( (bool success) =>{
			if(success){
				LoadProgress();
				StateListener sListener = new StateListener();
				sListener.mainManager = this;
				Info.myName = PlayGamesPlatform.Instance.GetUserDisplayName();
				PlayGamesPlatform.Instance.LoadState(0, sListener);
				PlayGamesPlatform.Instance.LoadState(1, sListener);
				PlayGamesPlatform.Instance.LoadState(2, sListener);
				PlayGamesPlatform.Instance.LoadState(3, sListener);
				character.GetComponent<Character>().SetName(Info.myName);
				if(Info.avatarSet != null)	character.GetComponent<Character>().SetAvatar(Info.avatarSet);
			}else{
				LoginSuccess(false);
			}
		});

		if( PlayGamesPlatform.Instance.IsAuthenticated() ){
			playPanel.SetActive(true);
		}

	}

	public void LoadProgress(){
		loadStep++;
		progressBar.rectTransform.localScale = new Vector3 (1-loadStep*0.2f, 1, 1);
		if(loadStep == 5){
			LoginSuccess(true);
		}
	}

	public void LoginSuccess(bool b){
		loadingPanel.SetActive(false);
		if(b){
			if(Info.avatarSet != null)	character.GetComponent<Character> ().SetAvatar (Info.avatarSet);
			playPanel.SetActive(true);
		}else{
			Alert ();
			waitFailPanel.SetActive(true);
		}
	}

	public void ShowLeaderBoardRank(){
		Button ();
		PlayGamesPlatform.Instance.ShowLeaderboardUI ("CgkI3vSFmIoVEAIQAQ");
	}

	public void Invited(Invitation invitation){
		Message ();
		invitationPopup.GetComponent<InvitationPopup> ().Pop (invitation);
	}

	void Update () {		
		if(Input.GetKey(KeyCode.Escape)){ 
			QuitActive(true);
		}
		if(character.transform.position.x > 11){				
			character.transform.position = new Vector3(-7f,1, -10);
			character.transform.rigidbody2D.velocity = new Vector2(7,-5);
		}
	}

	public void QuitActive(bool b){
		Button ();
		quitPanel.SetActive(b);
	}

	public void Quit(){
		Button ();
		Application.Quit ();
	}

	public void LoadLevel(string levelName){
		Button ();
		Application.LoadLevel(levelName);
	}
	
	public AudioClip button;
	public AudioClip alert;
	public AudioClip message;
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

	IEnumerator Jump(){
		while (true) {
			character.GetComponent<Character>().AddForce (transform.position + new Vector3(0.5f, 1.5f) * 440f);
			yield return new WaitForSeconds (2f);
		}
	}

}
