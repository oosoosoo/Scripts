using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using GooglePlayGames.BasicApi.Multiplayer;
using GooglePlayGames;
using LitJson;

public class MessageSender {
	public static void Send(string taskType){
		JsonData json = new JsonData ();
		json["taskType"] = taskType;
		string str = json.ToJson ();
		byte[] b = System.Text.Encoding.UTF8.GetBytes (str);
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, b);
	}
	public static void Send(string taskType, string[] paramNames, string[] paramValues){
		JsonData json = new JsonData ();
		json["taskType"] = taskType;
		for(int i=0; i<paramNames.Length; i++){
			json[paramNames[i]] = paramValues[i];
		}		
		string str = json.ToJson ();
		byte[] b = System.Text.Encoding.UTF8.GetBytes (str);
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, b);
	}
	public static void Send(string taskType, string paramName, string paramValue){
		JsonData json = new JsonData ();
		json["taskType"] = taskType;
		json[paramName] = paramValue;
		string str = json.ToJson ();
		byte[] b = System.Text.Encoding.UTF8.GetBytes (str);
		PlayGamesPlatform.Instance.RealTime.SendMessageToAll(true, b);
	}

}
