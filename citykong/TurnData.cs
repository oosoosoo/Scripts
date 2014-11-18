using UnityEngine;
using System;

public class TurnData {
	public int[] charBrickIndex = new int[4];
	public float[] playerPosx = new float[4];
	public float[] playerPosy = new float[4];
	public int[] extraJumpChance = new int[4];

	public TurnData(){}
	public TurnData(string s){
		string[] ss = s.Split ('/');
		for(int i=0; i<4; i++){
			charBrickIndex[i] = int.Parse( ss[i] );
		}
		for(int i=4; i<8; i++){
			playerPosx[i-4] = float.Parse( ss[i] );
		}
		for(int i=8; i<12; i++){
			playerPosy[i-8] = float.Parse( ss[i] );
		}
		for(int i=12; i<16; i++){
			extraJumpChance[i-12] = int.Parse( ss[i] );
		}
	} 
	public override string ToString(){
		string result = "";
		for(int i=0; i<4; i++){
			result += charBrickIndex[i];
			result += "/";
		}
		for(int i=0; i<4; i++){
			result += playerPosx[i];
			result += "/";
		}
		for(int i=0; i<4; i++){
			result += playerPosy[i];
			result += "/";
		}
		for(int i=0; i<4; i++){
			result += extraJumpChance[i];
			result += "/";
		}
		return result;
	}
}
