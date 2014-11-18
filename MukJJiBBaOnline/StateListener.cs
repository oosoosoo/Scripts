using UnityEngine;
using System;
using System.Collections;
using GooglePlayGames.BasicApi;
using GooglePlayGames;
using LitJson;

public class StateListener :  OnStateLoadedListener {
	public Main main;
	bool[] loaded = new bool[4];

	public void OnStateLoaded(bool success, int slot, byte[] data){
		if(!success){
			main.LoginFail();
			return;
		}
		switch(slot){
		case 0:
			if(data == null){
				Dic.json = JsonMapper.ToObject("{\"win\":0, \"lose\":0,\"win_com\":0,\"lose_com\":0,\"win_frd\":0,\"lose_frd\":0,\"savedDate\":0 }");
				Dic.isFirst = true;
			}else{
				Dic.json = JsonFromBytes(data);
			}
			Dic.win = (int)Dic.json["win"];
			Dic.lose = (int)Dic.json["lose"];
			Dic.win_com = (int)Dic.json["win_com"];
			Dic.lose_com = (int)Dic.json["lose_com"];
			Dic.win_frd = (int)Dic.json["win_frd"];
			Dic.lose_frd = (int)Dic.json["lose_frd"];
			break; 
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		}		
		loaded[slot] = true;
		main.LoginSuccess();
	}
	public byte[] OnStateConflict(int slot, byte[] localData, byte[] serverData){
		switch(slot){
		case 0:
			if(serverData == null && localData == null) break;
			return CompareSavedDate(localData, serverData);
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
		long lLong = (long)lJson["savedDate"];
		long sLong = (long)sJson["savedDate"];
		return lLong >= sLong? data1 : data2;
	}

	JsonData JsonFromBytes(byte[] data){
		string str = System.Text.Encoding.UTF8.GetString(data);
		return JsonMapper.ToObject (str);
	}

	public void Win(){
		Dic.json ["win"] = Dic.win += 1;
		Dic.json ["savedDate"] = DateTime.Now.Ticks;
		byte[] b = System.Text.Encoding.UTF8.GetBytes (Dic.json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
		PlayGamesPlatform.Instance.ReportScore (Dic.win+1, "CgkIkvaA75sdEAIQAQ", (bool bl) => {});
	}	
	public void Win_com(){
		Dic.json ["win_com"] = Dic.win_com += 1;
		Dic.json ["savedDate"] = DateTime.Now.Ticks;
		byte[] b = System.Text.Encoding.UTF8.GetBytes (Dic.json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
		PlayGamesPlatform.Instance.ReportScore (Dic.win_com+1, "CgkIkvaA75sdEAIQAg", (bool bl) => {});
	}	
	public void Win_frd(){
		Dic.json ["win_frd"] = Dic.win_frd += 1;
		Dic.json ["savedDate"] = DateTime.Now.Ticks;
		byte[] b = System.Text.Encoding.UTF8.GetBytes (Dic.json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
		PlayGamesPlatform.Instance.ReportScore (Dic.win_frd+1, "CgkIkvaA75sdEAIQAw", (bool bl) => {});
	}	
	public void Lose(){
		Dic.json ["lose"] = Dic.lose += 1;
		Dic.json ["savedDate"] = DateTime.Now.Ticks;
		byte[] b = System.Text.Encoding.UTF8.GetBytes (Dic.json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
	}
	public void Lose_com(){
		Dic.json ["lose_com"] = Dic.lose_com += 1;
		Dic.json ["savedDate"] = DateTime.Now.Ticks;
		byte[] b = System.Text.Encoding.UTF8.GetBytes (Dic.json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
	}
	public void Lose_frd(){
		Dic.json ["lose_frd"] = Dic.lose_frd += 1;
		Dic.json ["savedDate"] = DateTime.Now.Ticks;
		byte[] b = System.Text.Encoding.UTF8.GetBytes (Dic.json.ToJson ());
		PlayGamesPlatform.Instance.UpdateState (0, b, this);
	}
}
