using UnityEngine;
using System.Collections;

public class GameManagerSingle : MonoBehaviour {
	public GameObject accuracyObj;

	public AudioClip bgm;
	public AudioClip applaus;
	public AudioClip victory;
	private int tryCount = 16;

	public GameObject aim;
	public GameObject characters;
	public Vector3 playerPos;
	private int charBrickIndex;
	
	private GameObject camFront;
	public GameObject deadLine;
	
	public GameObject[] brick;
	private GameObject[] bricks;
	
	public GameObject fire;
	public GameObject water;
	
	private float randomX = 12f;
	
	private static GameManager instance = null;
	
	public static GameManager Instance {
		get { return instance; }
	}
	void Start(){
		characters.transform.position = Vector3.zero;
		camFront = Camera.main.gameObject;
		camFront.GetComponent<Camwalk> ().playerPos = characters.transform;
		AudioSource.PlayClipAtPoint (bgm, Vector3.zero, 0.5f);

		//player setting
		characters.GetComponent<Character>().gameManagerSingle = this;
		characters.GetComponent<Character> ().SetName (Info.myName);
		if(Info.avatarSet != null)	characters.GetComponent<Character> ().SetAvatar (Info.avatarSet);
		
		playerPos = new Vector3 (0,0,-9);
		charBrickIndex = -1;
		
		//brick setting
		bricks = new GameObject[10];
		float x = Random.Range(6f,randomX);
		float y = Random.Range ( -8f, -2f);
		Vector3 v = new Vector3 (x,y,0);
		for(int i=0; i<10; i++){
			bricks[i] = Instantiate(brick[Random.Range(0,brick.Length)]) as GameObject;
			bricks[i].transform.position = v;
			x += Random.Range(6f,randomX);
			y = Random.Range ( -8f, -2f);
			v.x = x;
			v.y = y;
		}
		
		//fire, water
		int fireNum = Random.Range (1,4);
		int[] fireLoc = new int[fireNum];
		int waterNum = Random.Range (3,5) - fireNum;
		int[] waterLoc = new int[waterNum];
		
		//1, 2, 2
		for(int i=0; i<fireNum; i++){
			bool loop = true;
			while(loop){
				loop = false;
				int randomIndex = Random.Range(1,10);
				fireLoc[i] = randomIndex;
				for(int j=0; j<i; j++){
					if(fireLoc[j] == randomIndex){
						loop = true;
					}
				}
			}
		}		
		for(int i=0; i<fireNum; i++){
			int randomIndex = fireLoc[i];
			float ranX1 = bricks[randomIndex].transform.position.x;
			float ranX2 = bricks[randomIndex-1].transform.position.x;
			float ranX = ranX1 - (ranX1 - ranX2)*0.5f;
			Instantiate(fire,new Vector3(ranX, -8), new Quaternion());
		}
		
		
		for(int i=0; i<waterNum; i++){
			bool loop = true;
			while(loop){
				loop = false;
				int randomIndex = Random.Range(1,10);
				waterLoc[i] = randomIndex;
				for(int j=0; j<i; j++){
					if(waterLoc[j] == randomIndex){
						loop = true;
					}
				}
			}
		}		
		for(int i=0; i<waterNum; i++){
			int randomIndex = waterLoc[i];
			float ranX1 = bricks[randomIndex].transform.position.x;
			float ranX2 = bricks[randomIndex-1].transform.position.x;
			float ranX = ranX1 - (ranX1 - ranX2)*0.5f;
			Instantiate(water,new Vector3(ranX, 8), new Quaternion());
		}
		Turn();
	}
	
	public IEnumerator checkSuccess(GameObject chr, GameObject brick, int accuracy){
		if(brick.Equals (bricks[9])){			
			accuracyObj.transform.position = new Vector3(characters.transform.position.x, characters.transform.position.y-1, -30);
			accuracyObj.GetComponent<TextMesh> ().text = accuracy +"";
			if(accuracy >= 90) {
				Perfect();
				accuracyObj.GetComponent<Animator>().SetTrigger("perfect");
			}
			else accuracyObj.GetComponent<Animator>().SetTrigger("normal");

			characters.GetComponent<Character>().Victory();
			AudioSource.PlayClipAtPoint(applaus, new Vector3(), 0.5f);
			AudioSource.PlayClipAtPoint(victory, new Vector3(), 0.5f);
			playerPos = new Vector3(0, 0, -16);
			charBrickIndex = -1;
			aim.SetActive(false);
			yield return new WaitForSeconds(3f);
		}else if(brick.Equals (bricks[charBrickIndex+1])){			
			accuracyObj.transform.position = new Vector3(characters.transform.position.x, characters.transform.position.y-1, -30);
			accuracyObj.GetComponent<TextMesh> ().text = Mathf.CeilToInt(accuracy) +"";
			if(accuracy >= 90f) {
				Perfect();
				accuracyObj.GetComponent<Animator>().SetTrigger("perfect");
			}else{
				Success();
				accuracyObj.GetComponent<Animator>().SetTrigger("normal");
			}

			playerPos = characters.transform.position;
			charBrickIndex++;
		}
		Turn();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Escape)){ 
			//Application.LoadLevel(1);
		}
	}
	
	public void Turn(){
		characters.GetComponent<Character> ().Reset ();
		tryCount--;
		characters.transform.position = playerPos;
		aim.GetComponent<Aim> ().character = characters;
		aim.transform.position = playerPos;
		aim.GetComponent<Aim> ().Reset ();
		aim.SetActive (true);
	}
	
	public void WWWRequestAddForce(Vector2 forcePos){
		characters.GetComponent<Character> ().AddForce (forcePos);
	}
	public AudioClip perfect;
	public AudioClip success;
	void Success(){
		AudioSource.PlayClipAtPoint (success, Vector3.zero);
	}
	void Perfect(){
		AudioSource.PlayClipAtPoint (perfect, Vector3.zero);
	}
}
