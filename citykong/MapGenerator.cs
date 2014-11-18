using UnityEngine;
using System.Collections;
using LitJson;

public class MapGenerator {

	public static string GenerateBrick(){
		float randomX = 12f;
		
		//brick setting 
		Vector2[] bricksPos = new Vector2[10];
		int[] brickIndex = new int[10];
		
		float x = Random.Range(8f,randomX);
		float y = Random.Range ( -8f, -2f);
		Vector3 v = new Vector3 (x, y, 0);
		for(int i=0; i<9; i++){
			brickIndex[i] = Random.Range(0,5);
			bricksPos[i] = v; 
			x += Random.Range(8f,randomX);
			y = Random.Range ( -8f, -2f);
			v.x = x;
			v.y = y;
		}
		x += Random.Range (10, 13);
		y = Random.Range (-8f, -2f);
		v = new Vector3 (x, y, 0);
		brickIndex[9] = Random.Range (2, 4);
		bricksPos[9] = v; 
		
		//fire, water, wind
		int waterNum = Random.Range (1,3);
		int[] waterLoc = new int[waterNum];
		int windNum = 1;
		int[] windLoc = new int[windNum];
		int fireNum = Random.Range (2,4);
		int[] fireLoc = new int[fireNum];
		
		//not duplicated
		for(int i=0; i<fireNum; i++){
			bool loop = true;
			while(loop){
				loop = false;
				int randomIndex = Random.Range(1,9);
				fireLoc[i] = randomIndex;
				for(int j=0; j<i; j++){
					if(fireLoc[j] == randomIndex){
						loop = true;
					}
				}
			}
		}			
		
		for(int i=0; i<waterNum; i++){
			bool loop = true;
			while(loop){
				loop = false;
				int randomIndex = Random.Range(1,9);
				waterLoc[i] = randomIndex;
				for(int j=0; j<i; j++){
					if(waterLoc[j] == randomIndex){
						loop = true;
					}
				}
			}
		}		
		
		for(int i=0; i<windNum; i++){
			bool loop = true;
			while(loop){
				loop = false;
				int randomIndex = Random.Range(1,8);
				windLoc[i] = randomIndex;
				for(int j=0; j<waterNum; j++){
					if(waterLoc[j] == randomIndex){
						loop = true;
					}
				}
				for(int j=0; j<fireNum; j++){
					if(fireLoc[j] == randomIndex){
						loop = true;
					}
				}
			}
		}
		string temp ="{\"brickLoc\" : [";
		foreach(Vector2 vv in bricksPos){
			temp+= "{\"x\":"+vv.x+", \"y\" :"+vv.y+"},";
		}		
		temp = temp.Substring (0, temp.Length-1);
		temp += "],";
		
		temp += "\"fireLoc\" : ";
		foreach(int i in fireLoc){
			temp += i;
		}
		temp += ",";
		
		temp += "\"waterLoc\" : ";
		foreach(int i in waterLoc){
			temp += i;
		}	
		temp += ",";
		
		temp += "\"windLoc\" : ";
		foreach(int i in windLoc){
			temp += i;
		}	
		temp += ",";
		
		temp += "\"brickIndex\" : \"";
		foreach(int i in brickIndex){
			temp += i;
		}	
		temp += "\"}";

		return temp;
	}

}
