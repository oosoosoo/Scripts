using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GooglePlayGames;

public class WaitRoom : MonoBehaviour {
	public Text tip;
	public PlayerPanel me;
	public PlayerPanel opp;
	public GameObject spinner;
	public GameObject oppFoundPanel;
	public GameObject alertPanel;

	public static RealTimeListener listener = new RealTimeListener();

	// Use this for initialization
	void Start () {
		listener.waitroom = this;

		me.SetName (Dic.myName);
		int totalwin = Dic.win + Dic.win_com;
		Dic.myLevel = totalwin / (10 + totalwin / 10) + 1;
		me.SetLevel (Dic.myLevel);
		me.SetRecord (Dic.win);
		me.SetAIRecord (Dic.win_com);

		tip.text = Dic.tips [Random.Range (0, Dic.tips.Length)];
		StartCoroutine ("Begin");
		
		#if UNITY_ANDROID && !UNITY_EDITOR
		PlayGamesPlatform.Instance.RealTime.CreateQuickGame (1, 1, 0, listener);
		#endif
	}

	public void RoomProgress(float f){
		if(f > 20){
			spinner.SetActive(false);
			oppFoundPanel.SetActive(true);
			StopCoroutine("Begin");
		}
	}

	public void RoomCreated(bool success){
		if(success){
			StartCoroutine( SendInfo() );
		}else{			
			#if UNITY_ANDROID && !UNITY_EDITOR
			PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
			#endif
			Sound.Error();
			StopCoroutine("Multi");
			alertPanel.SetActive(true);
		}
	}

	IEnumerator SendInfo(){
		yield return new WaitForSeconds(1f);
		MessageSender.Send("info",new string[]{"name","win", "win_com"}, new string[]{Dic.myName, Dic.win+"", Dic.win_com+""});
	}

	public void OppInfo(string name, int win, int win_com){
		int totalwin = win + win_com;

		Dic.oppName = name;
		Dic.oppLevel = totalwin / (10 + totalwin / 10) + 1;
		oppFoundPanel.SetActive (false);
		audio.Stop ();
		Sound.Perfect ();

		opp.SetName (name);
		opp.SetLevel (Dic.oppLevel);
		opp.SetRecord (win);
		opp.SetAIRecord (win_com);
		opp.gameObject.SetActive (true);

		StartCoroutine ("Multi");
	}
	IEnumerator Multi(){
		yield return new WaitForSeconds(5f);
		Application.LoadLevel ("Multi");
	}

	IEnumerator Begin(){
		yield return new WaitForSeconds(20f);
		#if UNITY_ANDROID && !UNITY_EDITOR
		PlayGamesPlatform.Instance.RealTime.LeaveRoom ();
		#endif
		spinner.SetActive(false);
		oppFoundPanel.SetActive(true);
		OppInfo ("컴퓨터", 0, 0);
		yield return new WaitForSeconds(5f);
		Application.LoadLevel ("Single");
	}

	public void Quit(){
		Application.LoadLevel ("Main");
	}
}
