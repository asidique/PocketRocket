using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class HUDMainMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Time.timeScale = 1;
		if(GetComponent<Animator>() != null) {
			GetComponent<Animator>().Play(0);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void changeScene(int s) {
		SceneManager.LoadScene(s);
	}
}
