using UnityEngine;
using System;
using System.Collections;

public class InAppBilling {
	ShopManager manager;

	public InAppBilling(ShopManager m){
		this.manager = m;		
		using (AndroidJavaObject activity = GetActivity()) {
			activity.Call("init_Helper", new OnIabPurchaseFinishedProxy(this), GetActivity(), "GabHHqhx2AYD8EWkbk+b+EFRuILDpJbEwrYJxrGz/RMW9Cm9qlkeSseJ9/uaiucY2lzVQNA/HqhHGB6Okzp5LW9eHWAqKfJM/5TO5VLSut69I7N4YEqICxesBiwgqFOdVWwC9AupFSCBPsOpMnJxGVF0Ut+5dZTa9E7OrpjEShQ1biQ397D0OOdKxRLTkgRsAaOxGt/T6HVyCD58AcQHKa2owIDAQAB");
		}
	}

	internal AndroidJavaObject GetActivity() {
		using (AndroidJavaClass activity = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
			return activity.GetStatic<AndroidJavaObject>("currentActivity");
		}
	}

	public void InAppBuyItem(string itemId){
		manager.SpinnerOnOff(true);	
		using (AndroidJavaObject activity = GetActivity()) {
			activity.Call("inAppBuyItem", itemId);
		}
	}

	public void GetInventory(){
		AndroidJavaObject list;
		manager.inventoryItemPanel.localInventory = Info.items;
		manager.shopItemPanel.localInventory = Info.items;

		using (AndroidJavaObject activity = GetActivity()) {
			list = activity.Call<AndroidJavaObject>("queryInventory");
		}
		int size = list.Call<int> ("size");

		int length = 0;
		if (manager.inventoryItemPanel.localInventory == null) {

		}else{
			length = manager.inventoryItemPanel.localInventory.Length;
		}
		string[] newArr = new string[length+size];
		for(int i=0; i<length; i++){
			newArr[i] = manager.inventoryItemPanel.localInventory[i];
		}
		for(int i=0; i<size; i++){
			string s = list.Call<string>("get",i);
			if(!s.Equals("helmet_elephant"))	newArr[length+i] = s;
		}
		manager.inventoryItemPanel.localInventory = newArr;
		manager.shopItemPanel.localInventory = newArr;
		manager.inventoryItemPanel.initFinished = true;
		manager.shopItemPanel.initFinished = true;
	}

	void onInitFinished(bool success){
		if(success){
			GetInventory();
		}else{
			manager.Error();
			manager.errorPanel.SetActive(true);
		}
	}

	void onPurchaseFinished(bool success, int response){
		Debug.Log ("onPurchaseFinished : "+success);
		manager.SpinnerOnOff(false);
		if(success){
			manager.Perfect();
			GetInventory();
			manager.PanelsUpdate();
		}else{
			manager.Error();
		}
	}

	private class OnIabPurchaseFinishedProxy : AndroidJavaProxy{
		InAppBilling mOwner;

		internal OnIabPurchaseFinishedProxy(InAppBilling owner)
			: base("studio.realcrack.citykong.UnityListener") {
			mOwner = owner;
		}

		public void onInitFinished(bool success){
			mOwner.onInitFinished(success);
		}

		public void onPurchaseFinished(bool success, int response){
			mOwner.onPurchaseFinished(success, response);
		}

		public string toString(){
			return ToString ();
		}
	}
}
