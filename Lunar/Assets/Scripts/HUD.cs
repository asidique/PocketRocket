using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class HUD : MonoBehaviour {
	public Text distanceText;
	public Text coinText;
	public Text plusCoin;
	public RectTransform fuel;
	public Player player;
	public RotateTowards rTRef;
	public Button pauseButton;
	public Text fuelText;
	Color plusCoinColor = new Color(1.0f, 1.0f, 0.0f);
	public Image[] hudImages;
	public Button[] hudButtons;
	public Text[] hudTexts;
	float timer = 0.0f;
	float maxTimer = 1.5f;
	bool textShine;
	int textShineAmount = 0;
	public bool gamePaused = false;
	int[] categoryMax = new int[4];
	public GameObject store;

	//4 is full, 150 is empty for fuel tank.
	void Start () {
		rTRef = GetComponentInChildren<RotateTowards>();
		hudImages = FindObjectsOfType<Image>();
		hudButtons = FindObjectsOfType<Button>();
		hudTexts = FindObjectsOfType<Text>();
		SetCoinText(player.currentCoins);
	}
	
	// Update is called once per frame
	void Update () {
		distanceText.text = ((int)rTRef.distance).ToString();
		updateFuelTank((int)(150.0f-(146.0f*(player.currentFuel/100.0f))), 100);
		pumpCoinText(textShine);

	}

	public void ToggleMenu() {
		gamePaused = !gamePaused;
		int n = 0;
		Sprite tmp;
		
		if(!gamePaused) {
			n = 1;
			Time.timeScale = 1.0f;
			tmp = (Sprite)Resources.Load<Sprite>("MenuItems/PauseIcon");
		} else {
			tmp = (Sprite)Resources.Load<Sprite>("MenuItems/PlayIcon");
			Time.timeScale = 0;
		}
		for(int i = 0; i < hudTexts.Length; i++) {
			if(hudTexts[i].tag=="Pause1") {
				hudTexts[i].enabled = gamePaused;
			} else if(hudTexts[i].tag == "Pause0"){
				hudTexts[i].enabled = !gamePaused;
			}
		}

		for(int i = 0; i < hudButtons.Length; i++) {
			if(hudButtons[i].tag == "Pause1") {
				hudButtons[i].enabled = gamePaused;
			} else if(hudButtons[i].tag == "Pause0"){
				hudButtons[i].enabled = !gamePaused;
			} else if(hudButtons[i].name == "Pause") {
				hudButtons[i].image.sprite = tmp;
			}
		}

		for(int i = 0; i < hudImages.Length; i++) {
			if(hudImages[i].tag=="Pause1") {
				hudImages[i].enabled = gamePaused;
			} else if(hudImages[i].tag == "Pause0"){
				hudImages[i].enabled = !gamePaused;
			}
		}

			
		//Make pause button turn to play button.
		//Make pause title appear
		//Have settings to turn off the music.
		//Have settings to change mechanics:

		//Input type: accelerometer, touch.
		//Sensitivity of accelerometer.

		//fade the other UI elements except for the coin icons.
	}
	public void ToggleStore() {
		if(player.landed) {
			if(store.active == false) {
				store.SetActive(true);
			} else {
				store.SetActive(false);
			}
		} else {
			store.SetActive(false);
		}
	}


	public void UpdateHUDValues() {
		SetCoinText(player.currentCoins);
	}

	void pumpCoinText(bool shine) {
		if(shine) {
			if(timer < maxTimer) {
				timer += Time.deltaTime;
				if(textShineAmount > 0) {
					plusCoin.text = "+"+textShineAmount.ToString();
				} else {
					plusCoin.text = textShineAmount.ToString();
				}
				plusCoinColor.a = 1.0f - timer/maxTimer;
				plusCoin.color = plusCoinColor;
			} else {
				timer = 0.0f;
				textShineAmount = 0;
				textShine = false;
			}
		}
	}

	void updateFuelTank(int currentFuel, int maxFuel) {
		fuel.offsetMax = new Vector2(fuel.offsetMax.x, -currentFuel);
		if(player.currentFuel < 0) {
			SetFuelTextColor(Color.red);
		} else if(player.currentFuel >= 99) {
			SetFuelTextColor(Color.green);
		} else {
			SetFuelTextColor(Color.white);
		}
	}

	void SetFuelTextColor(Color a) {
		if(fuelText.color != a) {
			fuelText.color = a;
		}
	}



	public void updateCoinText(int c) {
		timer = 0.0f;
		textShine = true;
		textShineAmount += c;
		player.currentCoins += c;
		SetCoinText(player.currentCoins);
		if(textShineAmount > 0) {
			plusCoinColor = Color.yellow;
		} else {
			plusCoinColor = Color.red;
		}
	}
		
	public void changeScene(int s) {
		SceneManager.LoadScene(s);
	}

	void SetCoinText(int amount) {
		if(amount >= 1000 && amount <= 1000000) {
			coinText.text = (Math.Round((amount/1000.0f), 1)+"K").ToString();
		} else if(amount >= 1000000) {
			coinText.text = (Math.Round((amount/1000000.0f), 1)+"M").ToString();
		} else {
			coinText.text = amount.ToString();
		}
	}


	//ITEMNAME GUIDE
	/*
	 * "C0L0M100"
	 * C0 - Category 0
	 * L0 - Level 0
	 * M100 - Cost 100
	 * 
	 * "C10L10M1000
	 * Category 10 (What to upgrade)
	 * Level 10 (How much to upgrade by)
	 * Cost 1000 (Cost of operation)
	 * 
	 * int categoryStart = itemName.IndexOf('C');
		int levelStart = itemName.IndexOf('L');
		int moneyStart = itemName.IndexOf('M');
		int cost = int.Parse(itemName.Substring(moneyStart+1));
		int category = int.Parse(itemName.Substring(categoryStart+1, levelStart-categoryStart-1));
		int level = int.Parse(itemName.Substring(levelStart+1, moneyStart-levelStart-1));
	 * */

	public void MakePurchase(int category) {
		Text button = GameObject.Find("Upgrade"+category.ToString()).GetComponentInChildren<Text>();
		int cost = int.Parse(button.text);
		bool successful = false;

		if(categoryMax[category] >= 7) {
			button.text = "MAX";
			button.color = Color.green;
		} 


		if(player.currentCoins >= cost) {
			if(categoryMax[category] < 7) {
				categoryMax[category]++;
			}
			Debug.Log(categoryMax[category]);
			switch(category) {
			case 0:
				successful = player.UpgradeTopSpeed();
				break;
			case 1:
				successful = player.UpdateAcceleration();
				break;
			case 2:
				successful = player.UpdateMaxTurn();
				break;
			case 3:
				successful = player.UpdateTurnRate();
				break;
			default:
				break;
			}
			if(successful) {
				updateCoinText((int)(-cost));
				if(categoryMax[category] <= 7) {
					cost *= (int)2.75f;
					button.text = ((int)Mathf.Ceil(cost)).ToString();
					if(categoryMax[category] == 7) {
						button.text = "MAX";
						button.color = Color.green;
						successful = false;
					}
				}
			}
		} else {
			//display message that player is broke AF
		}
	}
}
