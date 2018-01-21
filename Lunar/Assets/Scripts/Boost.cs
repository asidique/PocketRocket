using UnityEngine;
using System.Collections;

public class Boost : MonoBehaviour {

	public float boostPower = 25.0f;

	void Start() {
		boostPower = 25.0f;
	}
	void OnTriggerEnter(Collider col) {
		if(col.tag == "Player") {
			col.GetComponent<Player>().Boost(boostPower);
		}
	}
}
