using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class RotateZ : MonoBehaviour {
	public float rotationRate;
	public bool isUIElement;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(isUIElement) {
			GetComponent<Image>().rectTransform.Rotate(0.0f, 0.0f, rotationRate*Time.deltaTime);
		} else {
			transform.Rotate(0.0f, 0.0f, rotationRate*Time.deltaTime);
		}
	}
}
