using UnityEngine;
using System;
using System.Collections;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using LitJson;

public class StateListener :  OnStateLoadedListener {
	public MainManager mainManager;
	bool[] loaded = new bool[4];

	public void OnStateLoaded(bool success, int slot, byte[] data){
		//0:inventory
		//1:avatarSet
		//2:score
		//3:spent
		if(!success){
			mainManager.LoginSuccess(false);
			return;
		}
		switch(slot){
		case 0:
			if(data == null) break;
			string str = (string)JsonFromBytes(data)["items"];
			string[] array = str.Split(' ');
			Info.items = array;
			break;
		case 1:
			if(data == null) break;
			string str2 = (string)JsonFromBytes(data)["avatarSet"];
			string[] array2 = str2.Split(' ');
			Info.avatarSet = array2;
			break;
		case 2:
			if(data == null) break;
			int score = BitConverter.ToInt32(data, 0);
			Info.score = score;
			break;
		case 3:
			if(data == null) break;
			int spent = BitConverter.ToInt32(data, 0);
			Info.spent = spent;
			break;
		}		
		loaded[slot] = true;
		mainManager.LoadProgress();
	}
	public byte[] OnStateConflict(int slot, byte[] localData, byte[] serverData){
		switch(slot){
		case 0:
			if(serverData == null && localData == null) break;
			return CompareSavedDate(localData, serverData);
		case 1:
			if(serverData == null && localData == null) break;
			return CompareSavedDate(localData, serverData);
		case 2:
			if(serverData == null && localData == null) break;
			int lScore = BitConverter.ToInt32(localData, 0);
			int sScore = BitConverter.ToInt32(serverData, 0);
			return lScore >= sScore ? localData : serverData;
		case 3:
			if(serverData == null && localData == null) break;
			int lSpent = BitConverter.ToInt32(localData, 0);
			int sSpent = BitConverter.ToInt32(serverData, 0);
			return lSpent >= sSpent ? localData : serverData;
		}	
		return null;
	}
	public void OnStateSaved(bool success, int slot){
	}

	byte[] CompareSavedDate(byte[] data1, byte[] data2){	
		string lInven = System.Text.Encoding.UTF8.GetString(data1);
		string sInven = System.Text.Encoding.UTF8.GetString(data2);
		JsonData lJson = JsonMapper.ToObject (lInven);
		JsonData sJson = JsonMapper.ToObject (sInven);
		string lSavedDate = (string)lJson["savedDate"];
		string SsavedDate = (string)sJson["savedDate"];
		long lLong = long.Parse(lSavedDate);
		long sLong = long.Parse(SsavedDate);
		return lLong >= sLong? data1 : data2;
	}

	JsonData JsonFromBytes(byte[] data){
		string str = System.Text.Encoding.UTF8.GetString(data);
		return JsonMapper.ToObject (str);
	}

	public void SaveAvatar(string[] avatarSet){		
		JsonData json = new JsonData ();
		string arrayToString="";
		foreach(string s in avatarSet){
			arrayToString+=s+" ";
		}
		json["avatarSet"] = arrayToString;
		json ["savedDate"] = DateTime.Now.Ticks+"";
		byte[] b = System.Text.Encoding.UTF8.GetBytes (json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (1, b, this);
	}

	public bool AddInventoryItem(string str, int point){
		if(Info.score < Info.spent + 100){
			return false;
		}
		string[] newArr = null;
		if (Info.items != null) {
			newArr = new string[Info.items.Length + 1];
		}else{
			newArr= new string[1];
		}
		for(int i=0; i<newArr.Length-1; i++){
			newArr[i] = Info.items[i];
		}
		newArr [newArr.Length - 1] = str;
		Info.items = newArr;

		JsonData json = new JsonData ();
		string arrayToString="";
		foreach(string s in newArr){
			if(!string.IsNullOrEmpty(s)) arrayToString+=s+" ";
		}
		json["items"] = arrayToString;
		json ["savedDate"] = DateTime.Now.Ticks+"";
		byte[] b = System.Text.Encoding.UTF8.GetBytes (json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
		
		Info.spent += 100;
		b = System.BitConverter.GetBytes (Info.spent);		
		PlayGamesPlatform.Instance.UpdateState (3, b, this);

		return true;
	}

	public void ReportScore(int score){
		Info.score += score;
		byte[] b = System.BitConverter.GetBytes (Info.score);	
		PlayGamesPlatform.Instance.UpdateState (2, b, this);
		PlayGamesPlatform.Instance.ReportScore (Info.score, "CgkI3vSFmIoVEAIQAQ", (bool bl) => {});
	}

	public void ResetItems(){
		JsonData json = new JsonData ();
		json["items"] = "";
		json ["savedDate"] = DateTime.Now.Ticks+"";
		byte[] b = System.Text.Encoding.UTF8.GetBytes (json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
	}
	public void ResetAvatarSet(){
		JsonData json = new JsonData ();
		json["avatarSet"] = "";
		json ["savedDate"] = DateTime.Now.Ticks+"";
		byte[] b = System.Text.Encoding.UTF8.GetBytes (json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (1, b, this);
	}
	public void ResetScore(){
		byte[] b = System.BitConverter.GetBytes (0);		
		PlayGamesPlatform.Instance.UpdateState (2, b, this);
	}
	public void ResetSpent(){
		byte[] b = System.BitConverter.GetBytes (0);	
		PlayGamesPlatform.Instance.UpdateState (3, b, this);
	}
}
