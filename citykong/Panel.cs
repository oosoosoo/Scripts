using UnityEngine;
using System.Collections;

public class Panel : MonoBehaviour {
	public Item[] items;
	public bool initFinished;
	public GameObject spinner;
	int[] itemsLength = new int[]{11, 9, 5, 10, 7, 3, 0, 0};
	int[,] itemsPrice = new int[8,30]{
		//0:helmet		0
		{100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100},
		//1:hair		1
		{100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100},
		//2:face		2
		{100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
				100,100,100,100,100,
			100,100,100,100,100},
		//3:top		3-5
		{100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100},
		//4:bottom	6-9
		{100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
				100,100,100,100,100,
			100,100,100,100,100},
		//5:back		10
		{100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
				100,100,100,100,100,
			100,100,100,100,100},
		//6:hands		11-12
		{100,100,100,100,100,
		100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
				100,100,100,100,100,
			100,100,100,100,100},
		//7:foot		13-14
		{100,100,100,100,100,
		100,100,100,100,100,
			100,100,100,100,100,
			100,100,100,100,100,
				100,100,100,100,100,
				100,100,100,100,100}
	};
	int[,] itemsPoint = new int[8,30]{
		//helmet
		{1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1},
		//hair
		{1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1},
		//face
		{1,1,1,1,1,
		1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
				1,1,1,1,1,
				1,1,1,1,1},
		//top
		{1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1},
		//bottom
		{1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1},
		//back
		{1,1,1,1,1,
		1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
				1,1,1,1,1,
				1,1,1,1,1},
		{1,1,1,1,1,
		1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
				1,1,1,1,1,
				1,1,1,1,1},
		{1,1,1,1,1,
		1,1,1,1,1,
			1,1,1,1,1,
			1,1,1,1,1,
				1,1,1,1,1,
				1,1,1,1,1}
	};

	ArrayList[] inventoryList = new ArrayList[8];
	public string[] localInventory;
	void Start(){
		//showItems (0, 0);
		//setItems (0, 0);
		StartCoroutine (Init ());
	}	
	IEnumerator Init(){
		while(true){
			yield return new WaitForSeconds(1f);
			if(initFinished){
				showItems (0, 0);
				setItems (0, 0);
				spinner.SetActive(false);
				break;
			}
		}
	}

	public void setItems(int itemType, int page){
		for(int i=0; i<items.Length; i++){
			items[i].gameObject.SetActive(false);
		}
		int length = Mathf.Clamp (itemsLength[itemType]-(page * 8), 0, 8);
		int begin = page * 8 ;
		for(int i=0; i<length; i++){
			items[i].gameObject.SetActive(true);
			items[i].avatarType = itemType;
			items[i].avatarChar = (begin+i) < 10 ? "0"+(begin+i) : ""+(begin+i);
			items[i].SetImage();
			items[i].SetName((begin+i) < 10 ? "0"+(begin+i) : ""+(begin+i));
			items[i].itemId = itemType+""+((begin+i) < 10 ? "0"+(begin+i) : ""+(begin+i));
			items[i].SetPrice(itemsPrice[itemType,begin+i], itemsPoint[itemType,begin+i] == 1? true : false);
			if(localInventory != null && System.Array.IndexOf(localInventory, items[i].itemId) > -1){
				//if(inventoryList[itemType].IndexOf(begin+i) > -1){
				items[i].buyBtn.SetActive(false);
			}else if(items[i].buyBtn != null){
				items[i].buyBtn.SetActive(true);
			}
		}
	}

	public void showItems(int itemType, int page){
		Debug.Log ("showItems length : " + localInventory.Length);
		for(int i=0; i<items.Length; i++){
			items[i].gameObject.SetActive(false);
		}		
		
		for(int i = 0; i < inventoryList.Length; i++){
			inventoryList[i] = new ArrayList();
		}

		if(localInventory != null){
			foreach(string s in localInventory){
				if(string.IsNullOrEmpty(s)) continue;
				int i = int.Parse(s);
				int itemT = Mathf.FloorToInt( i * 0.01f );
				int itemN = i % 100;
				inventoryList[itemT].Add(itemN);
			}
		}

		int length = Mathf.Clamp (inventoryList[itemType].Count-(page * 8), 0, 8);
		int begin = page * 8 ;
		for(int i=0; i<length; i++){
			int thisI = (int)inventoryList[itemType][i+begin];
			items[i].gameObject.SetActive(true);
			items[i].avatarType = itemType;
			items[i].avatarChar = (thisI) < 10 ? "0"+(thisI) : ""+(thisI);
			items[i].itemId = itemType+""+(thisI);
			items[i].SetImage();
			items[i].SetName((thisI) < 10 ? "0"+(thisI) : ""+(thisI));
			items[i].SetPrice(itemsPrice[itemType,thisI], itemsPoint[itemType,thisI] == 1? true : false);
		}
	}
}
