using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {
	public GameObject accuracyObj;

	public GameManager gameManager;
	public GameManagerSingle gameManagerSingle;
	public GameObject magnetCircle;

	public TextMesh id;
	public TextMesh idShadow;

	public AudioClip[] audio;
	public ParticleSystem landingPrtcl;
	public ParticleSystem flyingPrtcl;
	public ParticleSystem rockPrtcl;
	
	public GameObject charac;
	public GameObject sprite;

	public bool onAir;
	public bool landed;
	public bool forceRdy;

	public float wind;
	public bool freeJump;
	public bool exchange;

	public SpriteRenderer[] avatars;
	
	private Animator anim;
	private Vector2 forcePower;

	public int charBrickIndex = -1;

	public string myName;
	void Awake () {
		anim = sprite.GetComponentInChildren<Animator> ();
		Reset ();
	}
	
	// Update is called once per frame
	void Update () {
		if(onAir)
		charac.transform.eulerAngles = new Vector3 (0, 0, rigidbody2D.velocity.y*1.5f);

	}
	
	void OnCollisionEnter2D(Collision2D col){
		if(!onAir) return;
		if(magnetCircle != null)	magnetCircle.SetActive (false);
		flyingPrtcl.Stop ();
		charac.transform.rotation = new Quaternion ();
		col.gameObject.GetComponentInChildren<Animator> ().SetTrigger ("shake");
		AudioSource.PlayClipAtPoint(audio[1], transform.position);
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.angularVelocity = 0;
		transform.rotation = new Quaternion ();
		landingPrtcl.transform.position = transform.position+new Vector3(0,-0.8f,0);
		landingPrtcl.Play ();
		onAir = false;

		Vector3 landPos = gameObject.transform.position;
		float colX = col.gameObject.GetComponent<BoxCollider2D> ().size.x;
		float accuracyF = 100-Mathf.Abs((gameObject.transform.position.x - col.transform.position.x)*100);
		int accuracy = Mathf.Clamp( Mathf.CeilToInt (accuracyF), 0, 100);

		if ( Mathf.Abs(gameObject.transform.position.x - col.transform.position.x) <= colX * 0.5f) {
			if(gameManager != null)	StartCoroutine( gameManager.checkSuccess (gameObject, col.gameObject, accuracy) );
			if(gameManagerSingle != null) StartCoroutine( gameManagerSingle.checkSuccess (gameObject, col.gameObject, accuracy) );
		}else{
			//make sure to fall character
			gameObject.transform.position = new Vector3(landPos.x+0.1f*(landPos.x > col.transform.position.x ? +1:-1), landPos.y, -7);
		}
	}
	public void Charge(){
		anim.SetBool ("charge", true);
	}
	public void Victory(){
		anim.SetBool ("victory", true);
	}
	public void AddForce(Vector2 v){
		forcePower = v;
		flyingPrtcl.Play();
	}

	void FixedUpdate(){
		if (onAir) {
			rigidbody2D.AddForce (new Vector2 (wind, 0) );
		}
		if (forcePower != Vector2.zero) {	
			onAir = true;
			landed = false;	
			AudioSource.PlayClipAtPoint(audio[0], transform.position);
			anim.SetBool ("charge", false);		
			rigidbody2D.AddForce (forcePower);
			forcePower = Vector2.zero;
		}
	}

	
	public void Reset(){
		freeJump = false;
		exchange = false;
		anim.SetBool ("charge", false);
		anim.SetBool ("victory", false);
		magnetCircle = null;
		flyingPrtcl.Stop ();
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.angularVelocity = 0;
		forcePower = Vector2.zero;
		rigidbody2D.gravityScale = 2;
		transform.rotation = new Quaternion ();
		charac.transform.rotation = new Quaternion ();
		foreach (SpriteRenderer ren in sprite.GetComponentsInChildren<SpriteRenderer>()){
			ren.color = Color.white;
		}
		onAir = false;
		landed = true;
	}

	public void Burn(){		
		foreach (SpriteRenderer ren in sprite.GetComponentsInChildren<SpriteRenderer>()){
			ren.color = Color.red;
		}
	}

	public void Wet(){	
		foreach (SpriteRenderer ren in sprite.GetComponentsInChildren<SpriteRenderer>()){
			ren.color = Color.cyan;
		}
		rigidbody2D.velocity = new Vector2 (rigidbody2D.velocity.x*0.6f, rigidbody2D.velocity.y*0.6f);
	}
	
	public void Blow(){	
		anim.SetBool ("charge", false);
		anim.SetTrigger ("wind");
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.gravityScale = 0;
		StartCoroutine ("BlowNext");
	}
	
	public IEnumerator Kicked(){	
		anim.SetBool ("charge", false);
		anim.SetTrigger ("kicked");
		AudioSource.PlayClipAtPoint (audio[2], gameObject.transform.position);
		yield return new WaitForSeconds (0.3f);
		gameObject.transform.position = new Vector3 (gameObject.transform.position.x, gameObject.transform.position.y, -7);
	}
	
	public void Rock(){
		anim.SetTrigger ("rock");
		rockPrtcl.transform.position = transform.position+new Vector3(0,-0.8f,0);
		rockPrtcl.Play ();
		AudioSource.PlayClipAtPoint (audio[3], gameObject.transform.position);
	}

	public void SetName(string name){
		myName = name;
		id.text = name;
		idShadow.text = name;
	}

	IEnumerator BlowNext(){
		yield return new WaitForSeconds (0.5f);
		gameObject.transform.position += new Vector3 (0,20,0);
		int index = 0;
		float randomF = Random.Range (0f, 1f);
		if(randomF < 0.1f) index = -1;
		else if(0.1f <= randomF && randomF < 0.35f) index = 0;
		else if(0.35f <= randomF && randomF <= 1f) index = 1;
		gameManager.MoveCharacter (index);
	}

	public void NewItem(int itemNo){
		if(gameManager != null)	gameManager.AddItem (itemNo);
	}

	public void SetAvatar(string[] s){
		if(s==null) return;
		for(int i = 0; i<s.Length; i++){
			switch(i){
			case 0://helmet
				wearAvatar(0, s[0]);
				break;
			case 1://hair
				wearAvatar(1, s[1]);
				break;
			case 2://face
				wearAvatar(2, s[2]);
				break;
			case 3://top
				wearAvatar(3, s[3]);
				wearAvatar(4, s[3]);
				wearAvatar(5, s[3]);
				break;
			case 4://bottom
				wearAvatar(6, s[4]);
				wearAvatar(7, s[4]);
				wearAvatar(8, s[4]);
				wearAvatar(9, s[4]);
				break;
			case 5://back
				wearAvatar(10, s[5]);
				break;
			case 6://hands
				wearAvatar(11, s[6]);
				wearAvatar(12, s[6]);
				break;
			case 7://foot
				wearAvatar(13, s[7]);
				wearAvatar(14, s[7]);
				break;
			}
		}
	}
	private void wearAvatar(int spriteIndex, string avatarNo){
		Sprite spr = Resources.Load<Sprite>("Sprite/avatar/"+spriteIndex+"/"+avatarNo);
		switch(spriteIndex){
		case 5://top
			spr = Resources.Load<Sprite>("Sprite/avatar/"+4+"/"+avatarNo);
			break;
		case 9://bottom
			spr = Resources.Load<Sprite>("Sprite/avatar/"+8+"/"+avatarNo);
			break;
		case 12://hands
			spr = Resources.Load<Sprite>("Sprite/avatar/"+11+"/"+avatarNo);
			break;
		case 14://foot
			spr = Resources.Load<Sprite>("Sprite/avatar/"+13+"/"+avatarNo);
			break;
		}
		if(spr != null){
			avatars[spriteIndex].sprite = spr;
		}else{
			avatars[spriteIndex].sprite = null;
		}
	}
	public void SetAvatar(int index, string s){
		switch(index){
		case 0://helmet
			wearAvatar(0, s);
			break;
		case 1://hair
			wearAvatar(1, s);
			break;
		case 2://face
			wearAvatar(2, s);
			break;
		case 3://top
			wearAvatar(3, s);
			wearAvatar(4, s);
			wearAvatar(5, s);
			break;
		case 4://bottom
			wearAvatar(6, s);
			wearAvatar(7, s);
			wearAvatar(8, s);
			wearAvatar(9, s);
			break;
		case 5://back
			wearAvatar(10, s);
			break;
		case 6://hands
			wearAvatar(11, s);
			wearAvatar(12, s);
			break;
		case 7://foot
			wearAvatar(13, s);
			wearAvatar(14, s);
			break;
		}

	}
}
