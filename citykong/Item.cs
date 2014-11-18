using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Item : MonoBehaviour {
	public Character character;
	public int avatarType = 5;
	public string avatarChar = "00";
	public Image[] itemImage;
	public Text itemNameText;
	public Text itemPriceText;
	public int itemPrice;
	public bool isPoint;
	public string itemId;

	public GameObject buyBtn;


	public ShopManager shopManager;
	private Texture2D[] itemTexture = new Texture2D[4];
	void Start(){
		SetImage ();
	}

	public void SetAvatar(bool inventory){
		shopManager.Button ();
		character.SetAvatar (avatarType, avatarChar);
		if(inventory){
			if(Info.avatarSet != null){
				if(Info.avatarSet.Length < 8){
					string[] newArr = new string[8];
					for(int i=0; i<8; i++){
						newArr[i] = "";
					}
					for(int i=0; i<Info.avatarSet.Length; i++){
						newArr[i] = Info.avatarSet[i];
					}
					Info.avatarSet = newArr;
				}
				Info.avatarSet[avatarType] = avatarChar;
			}else{
				Info.avatarSet = new string[8];
				for(int i=0; i<Info.avatarSet.Length; i++){
					Info.avatarSet[i] = "";
					if(i==avatarType) Info.avatarSet[i] = avatarChar;
				}
			}
		}
	}

	public void SetImage(){
		itemImage [0].gameObject.SetActive (false);
		itemImage [1].gameObject.SetActive (false);
		itemImage [2].gameObject.SetActive (false);
		itemImage [3].gameObject.SetActive (false);
		switch(avatarType){
		case 0://helmet
		case 1://hair
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+avatarType+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(39, 97, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(39, 97, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			break;
		case 2://face
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+avatarType+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(58, 96, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(58, 96, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			break;
		case 3://top
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+(avatarType+0)+"/"+avatarChar);
			itemTexture[1] = Resources.Load<Texture2D>("Sprite/avatar/"+(avatarType+1)+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(30, 32, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(30, 32, 128, 128), new Vector2(64,64));
			itemImage[1].sprite = Sprite.Create(itemTexture[1], new Rect(30, 32, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			if(itemTexture[1] != null)
				itemImage[1].gameObject.SetActive(true);
			break;
		case 4://bottom
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+(6+0)+"/"+avatarChar);
			itemTexture[1] = Resources.Load<Texture2D>("Sprite/avatar/"+(6+1)+"/"+avatarChar);
			itemTexture[2] = Resources.Load<Texture2D>("Sprite/avatar/"+(6+2)+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(0, 12, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(0, 12, 128, 128), new Vector2(64,64));
			itemImage[1].sprite = Sprite.Create(itemTexture[1], new Rect(0, 12, 128, 128), new Vector2(64,64));
			itemImage[2].sprite = Sprite.Create(itemTexture[2], new Rect(0, 12, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			if(itemTexture[1] != null)
				itemImage[1].gameObject.SetActive(true);
			if(itemTexture[2] != null)
				itemImage[2].gameObject.SetActive(true);
			break;
		case 5://back
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+10+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(29, 57, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(29, 57, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			break;
		case 6://hands
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+11+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(39, 97, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(39, 97, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			break;
		case 7://foot
			itemTexture[3] = Resources.Load<Texture2D>("Sprite/decoMain");
			itemTexture[0] = Resources.Load<Texture2D>("Sprite/avatar/"+13+"/"+avatarChar);
			itemImage[3].sprite = Sprite.Create(itemTexture[3], new Rect(39, 97, 128, 128), new Vector2(64,64));
			itemImage[0].sprite = Sprite.Create(itemTexture[0], new Rect(39, 97, 128, 128), new Vector2(64,64));
			itemImage[3].gameObject.SetActive(true);
			itemImage[0].gameObject.SetActive(true);
			break;
		}
	}

	public void SetName(string name){
		itemNameText.text = name;
	}
	public void SetPrice(int price, bool point){
		this.isPoint = point;
		itemPrice = price;
		itemPriceText.text = price + (point? " PTS" : " USD");
	}
	public void Buy(){
		shopManager.Button ();
		shopManager.BuyItem (avatarType, avatarChar, name, itemId, itemPrice, isPoint);
	}
}
