using UnityEngine;
using System.Collections;

public class Aim : MonoBehaviour {
	public GameObject character;
	public GameManager gameManager;
	public GameManagerSingle gameManagerSingle;

	public GameObject gage;
	public GameObject arrow;
	public GameObject circle;
	public Camera camFront;
	private SpriteRenderer ren;
	private bool charge;
	private float chargeAmt = 1f;
	// Use this for initialization
	void Awake () {
		ren = gage.GetComponent<SpriteRenderer> ();
		camFront = Camera.main;
		Reset ();
	}

	void Update () {
		//angle
		Vector3 v2 = new Vector3();
		if (character != null)	v2 = character.transform.position;
		Vector3 v = camFront.ScreenToWorldPoint(Input.mousePosition);  
		float f = Mathf.Atan2(v.x-v2.x, v.y-v2.y);
		f = f * 360f * 0.318f * 0.5f;
		if (-80 < f && f < 45)	f = -80f;
		if (45 <= f && f <= 170)	f = 170f;
		transform.eulerAngles = new Vector3(0,0,-f+180);

		if (Input.GetMouseButton(0)) {
			Ray ray = camFront.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit = new RaycastHit();
			if (Physics.Raycast(ray, out hit)) {
				if (hit.collider.Equals(circle.collider)) {
					charge = true;
					circle.SetActive(false);
					camFront.GetComponent<Camwalk>().walkable = false;
				}
			}
		}
		

		//gage
		if(charge){
			character.GetComponent<Character>().Charge();
			Vector3 v3 = gage.transform.localScale;
			gage.transform.localScale = new Vector2 (1,v3.y+chargeAmt*Time.deltaTime*0.8f);
			if(v3.y > 1f){
				gage.transform.localScale = new Vector2 (1,1f-chargeAmt*Time.deltaTime*0.8f);
				chargeAmt *= -1;
			}else if(v3.y <= 0){				
				chargeAmt *= -1;
				gage.transform.localScale = new Vector2 (1,0+chargeAmt*Time.deltaTime*0.8f);
			}
			ren.material.color = Color.Lerp(Color.green, Color.red, v3.y);
		}
		
	}
	void FixedUpdate(){
		//addforce
		if(charge && !Input.GetMouseButton(0)){
			camFront.GetComponent<Camwalk>().walkable = true;
			Vector2 v3 = ren.gameObject.transform.localScale;
			Vector2 forceDist = new Vector2(arrow.transform.position.x-character.transform.position.x,arrow.transform.position.y-character.transform.position.y);
			Vector2 forcePos = forceDist.normalized * v3.y*1200f+Vector2.up*50f;
			if(gameManager != null)	gameManager.RequestAddForce(forcePos);
			if(gameManagerSingle != null)	gameManagerSingle.WWWRequestAddForce(forcePos);
			charge = false;
			gameObject.SetActive (false);
		}
	}
	
	public void Reset(){
		charge = false;
		gage.transform.localScale = Vector2.zero;
		circle.SetActive(true);
	}

}
