using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SocialPlatforms;
using GooglePlayGames;
using GooglePlayGames.BasicApi.Multiplayer;

public class ShopManager : MonoBehaviour {
	public static string id = "citykong";
	public GameObject spinner;
	public GameObject errorPanel;

	public GameObject character;
	public GameObject invitationPopup;
	public GameObject soundOffText;
	public GameObject inventoryBtn;
	public GameObject shopBtn;
	public GameObject inventoryPanel;
	public GameObject shopPanel;
	public GameObject shopBtnBack;
	public GameObject shopPanelBack;
	public GameObject buyBox;

	public Button[] inventoryPageButtons;
	public Button[] shopPageButtons;

	public Button[] inventoryButtons;
	public Panel inventoryItemPanel;
	public Panel shopItemPanel;
	public Button[] shopButtons;

	public ColorBlock cBlock;
	private ColorBlock sblock;
	private ColorBlock iblock;

	public Animator characterAnim;

	private int inventoryIndex;
	private int inventorySubIndex;
	private int shopIndex;
	private int shopSubIndex;

	private RealTimeListener listener = MainManager.listener;

	public GameObject particleB;

	public Text pointsText;

	private StateListener sListener = new StateListener();

	InAppBilling inapp;
	void Start(){
	#if UNITY_ANDROID
		inapp = new InAppBilling (this);
	#endif
		StartCoroutine (AnimChange());
		sblock = shopButtons [0].colors;
		iblock = inventoryButtons [0].colors;
		shopButtons [0].colors = cBlock;
		inventoryButtons [0].colors = cBlock;
		shopPageButtons [0].colors = cBlock;
		inventoryPageButtons [0].colors = cBlock;
		
		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
		soundOffText.SetActive(!Info.soundOn);

		listener.shopManager = this;
		listener.STATUS = RealTimeListener.SHOP;

		float x = Screen.width * 0.90f;
		float y = Screen.height * 0.3f;
		particleB.transform.position = Camera.main.ScreenToWorldPoint(new Vector2(x, y)) - new Vector3(0, 0, -45);

		SetPoints ();
		if(Info.avatarSet != null) character.GetComponent<Character>().SetAvatar(Info.avatarSet);
	}
	public void SpinnerOnOff(bool b){
		spinner.SetActive (b);
	}

	void SetPoints(){
		pointsText.text = (Info.score - Info.spent) + "";
	}

	public void ShopBtnsClick(int index){
		Button ();
		shopIndex = index;
		for(int i=0; i<shopButtons.Length; i++){
			shopButtons[i].colors = sblock;
		}shopButtons[index].colors = cBlock;

		
		for(int i=0; i<shopPageButtons.Length; i++){
			shopPageButtons[i].colors = sblock;
		}shopPageButtons[0].colors = cBlock;


		shopItemPanel.setItems (index, 0);
	}
	public void InventoryBtnsClick(int index){
		Button ();
		inventoryIndex = index;
		for(int i=0; i<inventoryButtons.Length; i++){
			inventoryButtons[i].colors = iblock;
		}inventoryButtons[index].colors = cBlock;
		
		for(int i=0; i<inventoryPageButtons.Length; i++){
			inventoryPageButtons[i].colors = iblock;
		}inventoryPageButtons[0].colors = cBlock;

		inventoryItemPanel.showItems (index, 0);
	}
	public void shopPageBtnsClick(int index){
		Button ();
		shopSubIndex = index;
		for(int i=0; i<shopPageButtons.Length; i++){
			shopPageButtons[i].colors = sblock;
		}shopPageButtons[index].colors = cBlock;
		shopItemPanel.setItems (shopIndex, index);
	}
	public void inventoryPageBtnsClick(int index){
		Button ();
		inventorySubIndex = index;
		for(int i=0; i<inventoryPageButtons.Length; i++){
			inventoryPageButtons[i].colors = iblock;
		}inventoryPageButtons[index].colors = cBlock;
		inventoryItemPanel.showItems (inventoryIndex, index);
	}

	public void Inventory(){
		Button ();
		shopPanel.SetActive (false);
		shopBtn.SetActive (false);
		character.GetComponent<Character> ().SetAvatar (Info.avatarSet);
		inventoryItemPanel.GetComponent<Panel> ().showItems (0,0);
		inventoryItemPanel.showItems (inventoryIndex, inventorySubIndex);
	}
	public void Shop(){
		Button ();
		shopPanel.SetActive (true);
		shopBtn.SetActive (true);
	}
	public void BuyItem(int avatarType, string avatarChar, string name, string itemId, int itemPrice, bool isPoint){
		buyBox.GetComponent<Item> ().avatarType= avatarType;
		buyBox.GetComponent<Item> ().avatarChar = avatarChar;
		//buyBox.GetComponent<Item> ().SetName (name);
		buyBox.GetComponent<Item> ().itemId = itemId;
		buyBox.GetComponent<Item> ().SetImage ();
		buyBox.GetComponent<Item> ().SetPrice (itemPrice, isPoint);
		buyBox.SetActive (true);
	}

	IEnumerator AnimChange(){
		string[] boolNameArray = new string[]{"charge", "victory", "rock"};
		int index = 0;
		while(true){
			yield return new WaitForSeconds(2f);
			characterAnim.SetBool(boolNameArray[index], false);
			index++;
			if(index >= boolNameArray.Length) index = 0;
			characterAnim.SetBool(boolNameArray[index], true);
		}
	}	
	public void Invited(Invitation invitation){
		Message ();
		invitationPopup.GetComponent<InvitationPopup> ().Pop (invitation);
	}
	public void LoadLevel(string levelName){
		Button ();
		if(Info.avatarSet != null)	sListener.SaveAvatar (Info.avatarSet);
		Application.LoadLevel(levelName);
	}

	public void BuyPoint(){
		Button ();		
		if(Info.score < Info.spent + buyBox.GetComponent<Item> ().itemPrice){
			Error();
			return;
		}else{
			buyBox.SetActive (false);
			if(sListener.AddInventoryItem(buyBox.GetComponent<Item> ().itemId, buyBox.GetComponent<Item> ().itemPrice)){
				string[] newArr = new string[inventoryItemPanel.localInventory.Length+1];
				for(int i=0; i<inventoryItemPanel.localInventory.Length; i++){
					newArr[i] = inventoryItemPanel.localInventory[i];
				}newArr[inventoryItemPanel.localInventory.Length] = buyBox.GetComponent<Item> ().itemId;

				inventoryItemPanel.localInventory = newArr;
				shopItemPanel.localInventory = newArr;
				PanelsUpdate();
			}else{

			}
			SetPoints ();
		}
	}
	public void BuyUSD(){
		Button ();
		buyBox.SetActive (false);
		Inapp (buyBox.GetComponent<Item> ().itemId);
	}
	public void BuyCancel(){
		Button ();
		buyBox.SetActive (false);
	}
	public void PanelsUpdate(){
		shopItemPanel.GetComponent<Panel> ().showItems (inventoryIndex, inventorySubIndex);
		shopItemPanel.GetComponent<Panel> ().setItems (shopIndex, shopSubIndex);
	}

	public void ResetAvatar(){
		Button ();
		string[] avatarSet = new string[8];
		for(int i=0; i<avatarSet.Length; i++){
			avatarSet[i] = "";
		}
		Info.avatarSet = avatarSet;
		character.GetComponent<Character> ().SetAvatar (Info.avatarSet);
	}
	public AudioClip button;
	public AudioClip alert;
	public AudioClip message;
	public AudioClip error;
	public AudioClip perfect;
	public void Perfect(){
		Debug.Log ("PERFECT");
		AudioSource.PlayClipAtPoint (perfect, Vector3.zero);
	}
	public void Error(){
		AudioSource.PlayClipAtPoint (error, Vector3.zero);
	}
	public void Alert(){
		AudioSource.PlayClipAtPoint (alert, Vector3.zero);
	}
	public void Message(){
		AudioSource.PlayClipAtPoint (message, Vector3.zero);
	}
	public void Button(){
		AudioSource.PlayClipAtPoint (button, Vector3.zero);
	}
	public void SoundOnOff(){
		Info.soundOn = !Info.soundOn;
		soundOffText.SetActive(!Info.soundOn);
		AudioListener.pause = !Info.soundOn;
		AudioListener.volume = Info.soundOn ? 1f : 0f;
	}

	public void Inapp(string id){
		inapp.InAppBuyItem (id);
	}
}
