using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {
	public Vector3 speed;
	public bool movingRight;
	public float multiplier;
	public Vector3 target;
	public int coinAttack;
	public int fuelAttack;
	//TO DO: Add dying mechanisms
	// Use this for initialization
	void Start () {
		target = GameObject.FindGameObjectWithTag("Player").transform.position;
		float randomness = 1.007f;
		randomness = Mathf.Pow(randomness, GameObject.FindObjectOfType<Player>().platformsLanded);

		if(transform.position.x > target.x) {
			speed.x = multiplier * (Random.Range(-randomness,-randomness-3.0f));
		} else {
			speed.x = multiplier * (Random.Range(randomness,randomness+3.0f));
		}
	}

	// Update is called once per frame
	void Update () {
		Movement();
		Border();
	}



	void Movement() {
		transform.position += speed * Time.deltaTime;
	}

	void Border() {
		if(transform.position.x > target.x + 25.0f) {
			Destroy(gameObject);
		} else if(transform.position.x < target.x -25.0f) {
			Destroy(gameObject);
		}
	}
	void OnTriggerEnter(Collider other) {
		if(other.tag == "Player") {
			Debug.Log("Hit");
			other.GetComponent<Player>().PlayerHit(coinAttack, fuelAttack);
			Destroy(gameObject);
		} else if(other.tag == "Land") {
			Destroy(gameObject);
		}
	}
}
