using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Pickup : MonoBehaviour {

	public bool pickedUp = false;
	public BoxCollider bC;
	public HUD hud;
	public int value;

	// Use this for initialization
	void Start () {
		bC = gameObject.GetComponent<BoxCollider>();
		hud = GameObject.FindObjectOfType<HUD>();
	}
	
	// Update is called once per frame
	void Update () {
		Rotate();
	}

	void Rotate(){
		if(!pickedUp) {
			transform.Rotate(new Vector3(0, 3.0f, 0));
		}
	}

	void PickUp() {
		pickedUp = false;
		Destroy(gameObject);
	}

	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/CoinSplash"), transform.position, Quaternion.Euler(0.0f, 0.0f, 0.0f));
			hud.updateCoinText(value);
			PickUp();
		}
	}
}
