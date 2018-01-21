using UnityEngine;
using System.Collections;

public class DestroyInSeconds : MonoBehaviour {
	public float lifeTime;
	float currentTime;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
		if(currentTime > lifeTime) {
			GameObject.Destroy(this.gameObject);
		}
	}
}
