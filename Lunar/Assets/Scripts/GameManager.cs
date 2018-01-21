using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour {

	public EnemyRow rowOne;
	public EnemyRow rowTwo;
	public EnemyRow rowThree;
	public PickUpColumn columnOne;
	public PickUpColumn columnTwo;
	public PickUpColumn columnThree;
	public PickUpColumn columnFour;
	public PickUpColumn columnFive;
	public Transform SideSpawnPosition;
	public Transform TopSpawnPosition;
	public Vector3[] SideSpawns = new Vector3[6];
	public Vector3[] TopSpawns = new Vector3[5];
	public float FPS;
    public Text fpsCounter, accelCounter;
	public HUD hud;

	public float enemyRateOfSpawn;
	public float coinRateOfSpawn;
	public float difficultyMultiplier = 0.99f;

	public GameObject[] platforms = new GameObject[3];
	Vector3 nextPlatformPosition = new Vector3(20.0f, 100.0f, 0.0f);
	public Player player;

	// Use this for initialization
	void Start () {
		SetUpSpawns();

		player = GameObject.FindObjectOfType<Player>();
		hud = GameObject.FindObjectOfType<HUD>();
		GameObject.FindObjectOfType<RotateTowards>().t = platforms[1].transform;

		LoadPlayerStats();
		UpdateSpawnPositions();
	}



	void SetUpSpawns() {

		if(platforms[0] == null) {
			platforms[0] = GameObject.Find("Land");
		}
		if(platforms[1] == null) {
			platforms[1] = (GameObject)Instantiate(Resources.Load("Prefabs/Land"),platforms[0].transform.position+nextPlatformPosition, Quaternion.Euler(-90.0f,0.0f,0.0f));
		}
		rowOne = new EnemyRow(enemyRateOfSpawn);
		rowTwo = new EnemyRow(enemyRateOfSpawn+Random.Range(0, 2));
		rowThree = new EnemyRow(enemyRateOfSpawn+Random.Range(0, 4));
		columnOne = new PickUpColumn(Random.Range(3, 5));
		columnTwo = new PickUpColumn(Random.Range(2, 6));
		columnThree = new PickUpColumn(Random.Range(2, 5)); 
		columnFour = new PickUpColumn(Random.Range(4, 4));
		columnFive = new PickUpColumn(Random.Range(5, 7));
	}

	public void LandedOnPlatform(GameObject plat) {
		if(plat == platforms[1]) {
			hud.updateCoinText(100);
			GameObject.Destroy(platforms[0].gameObject);
			platforms[0] = platforms[1];
			platforms[1] = (GameObject)Instantiate(Resources.Load("Prefabs/Land"),platforms[0].transform.position+nextPlatformPosition, Quaternion.Euler(-90.0f,0.0f,0.0f));
			player.platformsLanded++;
			GameObject.FindObjectOfType<RotateTowards>().t = platforms[1].transform;
			coinRateOfSpawn = 3 * Mathf.Pow(difficultyMultiplier, player.platformsLanded);
			enemyRateOfSpawn = 5 * Mathf.Pow(difficultyMultiplier, player.platformsLanded);
			nextPlatformPosition = new Vector3((Random.Range(-50, 50)), Random.Range(100, 150)+player.platformsLanded, 0.0f);
			StreamWriter sW = new StreamWriter(Application.persistentDataPath + "/stats.txt");
			sW.WriteLine("PLAYER COINS:");
			sW.WriteLine(player.currentCoins.ToString());
			sW.WriteLine("PLAYER FUEL:");
			sW.WriteLine(player.currentFuel.ToString());
			sW.WriteLine("PLATFORM LEVEL:");
			sW.WriteLine(player.platformsLanded.ToString());
			sW.WriteLine("BOOST RATE: ");
			sW.WriteLine(player.boostRate);
			sW.WriteLine("TURN RATE: ");
			sW.WriteLine(player.turnRate);
			sW.WriteLine("Max Speed: ");
			sW.WriteLine(player.MAX_FORCE);
			sW.WriteLine("Max Torque: ");
			sW.WriteLine(player.MAX_TORQUE);
			sW.Close();
		}

	}

	void LoadPlayerStats() {
		StreamReader sR = new StreamReader(Application.persistentDataPath +"/stats.txt");
		while(!sR.EndOfStream) {
			string a = sR.ReadLine();
			if(a.Contains("COINS")) {
				player.currentCoins=int.Parse(sR.ReadLine());
			}
			if(a.Contains("FUEL")) {
				player.currentFuel=float.Parse(sR.ReadLine());
			}
			if(a.Contains("LEVEL")) {
				player.platformsLanded=int.Parse(sR.ReadLine());
			}
			if(a.Contains("BOOST")) {
				player.boostRate=float.Parse(sR.ReadLine());
			}
			if(a.Contains("TURN")) {
				player.turnRate=float.Parse(sR.ReadLine());
			}
			if(a.Contains("Speed")) {
				player.MAX_FORCE=float.Parse(sR.ReadLine());
			}
			if(a.Contains("Torque")) {
				player.MAX_TORQUE=float.Parse(sR.ReadLine());
			}
			hud.UpdateHUDValues();
		}
		sR.Close();
	}



	public void PlayerHit(int attack) {
		hud.updateCoinText(-attack);
	}

	public void InitiateSpawn(bool x) {
		rowOne.startSpawn = x;
		rowTwo.startSpawn = x;
		rowThree.startSpawn = x;
		columnOne.startSpawn = x;
		columnTwo.startSpawn = x;
		columnThree.startSpawn = x;
		columnFour.startSpawn = x;
		columnFive.startSpawn = x;
	}
	// Update is called once per frame
	void Update () {
		UpdateSpawnPositions();
		FPS = 1/Time.deltaTime;
		fpsCounter.text = "FPS: " + FPS;
		InitiateSpawn(true);
		rowOne.Update();
		rowTwo.Update();
		rowThree.Update();
		columnOne.Update();
		columnTwo.Update();
		columnThree.Update();
		columnFour.Update();
		columnFive.Update();
	}

	void UpdateSpawnPositions() {
		for(int i = 0; i < 3; i++) {
			SideSpawns[i] = new Vector3(SideSpawnPosition.position.x+12.0f, SideSpawnPosition.position.y+i*5.0f, 7.0f);
			SideSpawns[i+3] = new Vector3(SideSpawnPosition.position.x-12.0f, SideSpawnPosition.position.y+i*5.0f, 7.0f);
		}
		for(int i = 0; i < 5; i++) {
			TopSpawns[i] = new Vector3(TopSpawnPosition.position.x+(i*2.5f), TopSpawnPosition.position.y, 7.0f);
		}
		rowOne.UpdateRowPositions(SideSpawns[0], SideSpawns[3]);
		rowTwo.UpdateRowPositions(SideSpawns[1], SideSpawns[4]);
		rowThree.UpdateRowPositions(SideSpawns[2], SideSpawns[5]);
		columnOne.UpdateRowPositions(TopSpawns[0]);
		columnTwo.UpdateRowPositions(TopSpawns[1]);
		columnThree.UpdateRowPositions(TopSpawns[2]);
		columnFour.UpdateRowPositions(TopSpawns[3]);
		columnFive.UpdateRowPositions(TopSpawns[4]);

	}






	#region TABLEDEFINITION
	public class TableAttribute {
		public GameObject currentSpawnObject;
		public float rateOfSpawn = 2.0f; //Seconds per spawn
		public float currentRate = 0.0f;
		public bool startSpawn = false;
	}

	public class Row : TableAttribute {
		public Vector3[] spawnPosition = new Vector3[2];
		public string[] objectTypes = new string[3];

		public void UpdateRowPositions(Vector3 spawnPositionOne, Vector3 spawnPositionTwo) {
			spawnPosition[0] = spawnPositionOne;
			spawnPosition[1] = spawnPositionTwo;
		}

		void SpawnObject() {
			if(currentSpawnObject == null) {
				int a = Random.Range(0,2);
				int objectType = Random.Range(0,3);
				if(a==0) {
					currentSpawnObject = (GameObject)Instantiate(Resources.Load<GameObject>(objectTypes[objectType]), spawnPosition[0], new Quaternion(0.5f, -0.5f, -0.5f, -0.5f));
				} else {
					currentSpawnObject = (GameObject)Instantiate(Resources.Load<GameObject>(objectTypes[objectType]), spawnPosition[1], new Quaternion(0.5f, 0.5f, 0.5f, -0.5f));
				}
			}
		}

		public void Update() {
			if(startSpawn == true) {
				currentRate += Time.deltaTime;
				if(currentRate > rateOfSpawn) {
					SpawnObject();
					currentRate = 0.0f;
				}
			}
		}
	};

	public class Column : TableAttribute {
		public Vector3 spawnPosition = new Vector3();
		public string[] objectTypes = new string[4];

		public void UpdateRowPositions(Vector3 spawnPositionOne) {
			spawnPosition = spawnPositionOne;
		}

		void SpawnObject() {
			int count = Random.Range(0,101); //TODO: Make certain pickups rare here
			int objectType = 0;
			if(count%2 == 0) {
				objectType = 0;
			} else if(count%3 == 0) {
				objectType = 1;
			} else if(count%7 == 0) {
				objectType = 2;
			} else if(count%11 == 0) {
				objectType = 3;
			} else {
				objectType = 0;
			}//CONTINUE TO ADD OBJECTS HERE. THE HIGHER THE REMAINDER, THE RARER THE OBJECT
			currentSpawnObject = (GameObject)Instantiate(Resources.Load<GameObject>(objectTypes[objectType]), spawnPosition, new Quaternion(0, 0, 0, 0));
		}

		public void Update() {
			if(startSpawn == true) {
				currentRate += Time.deltaTime;
				if(currentRate > rateOfSpawn) {
					SpawnObject();
					currentRate = 0.0f;
				}
			}
		}

	}

	public class EnemyRow : Row {
		public EnemyRow(float _rateOfSpawn) {
			rateOfSpawn = _rateOfSpawn;
			objectTypes[0] = "Prefabs/Geese";
			objectTypes[1] = "Prefabs/Airplane";
			objectTypes[2] = "Prefabs/UFO";
		}
	}

	public class PickUpColumn : Column {
		public PickUpColumn(float _rateOfSpawn) {
			rateOfSpawn = _rateOfSpawn;
			objectTypes[0] = "Prefabs/Coin1";
			objectTypes[1] = "Prefabs/Coin2";
			objectTypes[2] = "Prefabs/HoopHolder";
			objectTypes[3] = "Prefabs/GemHolder";
		}
	}
	#endregion
}
