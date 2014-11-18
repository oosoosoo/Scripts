using UnityEngine;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;

public class Main : MonoBehaviour {
	public Hand hand1;
	public Hand hand2;
	public GameObject spinner;
	public GameObject loginFailPanel;
	public GameObject[] tutorialPanels;
	// Use this for initialization

	void Start () {
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		StartCoroutine ("Begin");
		#if UNITY_ANDROID && !UNITY_EDITOR
			Authenticate ();
		#endif
		#if UNITY_EDITOR
			spinner.SetActive(false);
		#endif

	}

	IEnumerator Begin(){
		while(true){
			yield return new WaitForSeconds(1f);
			hand1.Shoot (Random.Range(0,3));
			hand2.Shoot (Random.Range(0,3));
		}
	}

	void Authenticate(){
		Debug.Log ("Authenticate");
		if(!string.IsNullOrEmpty(Dic.myName)) {
			LoginSuccess();
			return;
		}

		PlayGamesPlatform.DebugLogEnabled = false;
		PlayGamesPlatform.Activate ();
		Social.localUser.Authenticate ( (bool success) =>{
			if(success){
				Dic.myName = PlayGamesPlatform.Instance.GetUserDisplayName();
				StateListener sListener = new StateListener();
				sListener.main = this;
				PlayGamesPlatform.Instance.LoadState(0, sListener);
			}else{
				LoginFail();
			}
		});
	}
	public void LoginSuccess(){
		spinner.SetActive(false);
	}

	public void LoginFail(){
		audio.Stop ();
		StopCoroutine ("Begin");
		loginFailPanel.SetActive (true);
		Sound.Error();
	}

	public void Load(){
		Sound.Button ();
		if(Dic.isFirst){
			StopCoroutine("Begin");
			audio.Stop();
			Tutorial(0);
		}else{
			Application.LoadLevel ("Waitroom");
		}
	}

	public void Tutorial(int i){
		Sound.Button ();
		if(i >= tutorialPanels.Length){
			Dic.isFirst = false;
			Application.LoadLevel ("Waitroom");
			return;
		}
		tutorialPanels [i].SetActive (true);
		if(i > 0)	tutorialPanels [i-1].SetActive (false);
	}

	public void Quit(){
		Sound.Button ();		
		#if UNITY_ANDROID && !UNITY_EDITOR
		using (AndroidJavaObject activity = GetActivity()) {
			activity.Call("closePopUp");
		}
		#endif
	}
	
	internal AndroidJavaObject GetActivity() {
		using (AndroidJavaClass activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			return activity.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}
}
