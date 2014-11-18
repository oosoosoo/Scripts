using UnityEngine;
using System.Collections;

public class ItemAni : MonoBehaviour {
	public AudioClip itemUseSound;
	public Sprite[] itemSprite;
	public SpriteRenderer spriteChild;
	// Use this for initialization
	public void ItemUse(){
		//GetComponent<Animator> ().SetTrigger ("itemuse");
		AudioSource.PlayClipAtPoint (itemUseSound, gameObject.transform.position);
	}
}
