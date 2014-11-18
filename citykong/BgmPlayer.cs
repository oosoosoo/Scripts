using UnityEngine;
using System.Collections;

public class BgmPlayer : MonoBehaviour {
	public string[] destroyLevelName;
	private static BgmPlayer instance = null;
	public static BgmPlayer Instance {
		get { return instance; }
	}
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		} else {
			instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}

	void OnLevelWasLoaded(int level){
		for(int i=0; i<destroyLevelName.Length; i++){
			string levelName = destroyLevelName[i];
			if(Application.loadedLevelName.Equals(levelName)){
				Destroy(this.gameObject);
			}
		}
	}
}
