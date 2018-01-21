
using UnityEngine;
using System.Collections;


/// <summary>
/// Main controller for the player's ship
/// </summary>  
public class Player : MonoBehaviour
{

    //Keys
    const KeyCode LEFT_KEY = KeyCode.LeftArrow;
    const KeyCode RIGHT_KEY = KeyCode.RightArrow;
    const KeyCode UP_KEY = KeyCode.UpArrow;
    const KeyCode DOWN_KEY = KeyCode.DownArrow;
    const KeyCode LAND_KEY = KeyCode.Space;

    //Components
    private Rigidbody rb;
    [SerializeField]
    private ParticleSystem fireUp, fireLeft, fireRight;

    //Constant Variables
	public float MAX_FORCE;// = 10.0f;
	public float MAX_TORQUE;// = 1.0f;
	public float MAX_ANGLE;// = 25.0f;
	const float CAP_FORCE = 25.0f;
	const float CAP_TORQUE = 2.5f;
	const float CAP_ACCELERATION = 1.00f;
	const float CAP_TURN = 0.35f;
	private const float RAYCAST_LANDING_RANGE = 1.05f;

    //Variables
    public Vector2 currentForce = Vector2.zero;
    public Vector3 currentTorque = Vector3.zero;
	public float pushRate;// = 0.05f;
	public float turnRate;// = 0.085f;
	public float boostRate;// = 0.15f;
    private float currentRotation = 0.0f;
    private bool isMovingUpwards = false;
    private bool isRotating = false;
	[SerializeField]public float currentFuel;
	[SerializeField]private float fuelTank;
	[SerializeField]GameManager gM;
	public int currentCoins;
	public float platformsLanded;
	public float hitTimer = 0.0f;
	public bool playerHit = false;
	public bool landed = false;
    //Lerping
    private float timeTakenToLerp, timeStartedLerping;
    private Quaternion lerpStartRot, lerpEndRot;
    private bool isLerping = false;


    //Mobile Vars
    bool touchUp = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.maxAngularVelocity = 2.0f;
        rb.maxDepenetrationVelocity = 10.0f;
    }

    void Update()
    {
        if (!isLerping)
        {
            HandleInput();
        }

        HandleParticles();

		if(Input.GetKeyDown(KeyCode.K)) {
			PlayerHit(10, 10);
		}

		PlayerRecover(playerHit);

    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    void HandleParticles()
    {
        if (currentTorque.z < 0 && !fireRight.isPlaying)
        {
            fireRight.Play();
        }
        if (currentTorque.z > 0 && !fireLeft.isPlaying)
        {
            fireLeft.Play();
        }
		if (currentForce.y > 0 && currentFuel > 0)
        {
            fireUp.Emit(1);
        }

        if (!isMovingUpwards)
        {
            if (fireUp.isPlaying)
            {
                fireUp.Stop();
            }
        }

        if (!isRotating)
        {
            if (fireRight.isPlaying || fireLeft.isPlaying)
            {
                fireRight.Stop();
                fireLeft.Stop();
            }
        }

    }

    void HandleMovement()
    {
		if (currentForce != Vector2.zero && currentFuel > 0)
        {
			rb.AddRelativeForce(currentForce, ForceMode.Acceleration);
			currentFuel -= Time.deltaTime;
        }
        if (currentTorque != Vector3.zero)
        {
            transform.Rotate(currentTorque);
        }
        if (rb.velocity.magnitude > MAX_FORCE)
        {
            rb.AddForce(-rb.velocity);
        }

    }

	public void Boost(float boostPower) {
		rb.AddRelativeForce(new Vector3(0.0f, boostPower, 0.0f), ForceMode.VelocityChange);
	}

    void HandleInput()
    {
        #region MOBILE
        bool usingPhone = false;
        int touchCount = Input.touchCount;

        if (touchCount > 0)
        {
            usingPhone = true;
            for (int i = 0; i < touchCount; i++)
            {
                Touch touch = Input.GetTouch(i);
                if (touch.phase == TouchPhase.Began || touch.phase == TouchPhase.Stationary)
                {
                    if (touch.position.y < Screen.height / 2)
                    {
                        touchUp = true;
                    }
                }
                if (touch.phase == TouchPhase.Ended)
                {
                    touchUp = false;
                }
            }
        }
        else
        {
            touchUp = false;
        }

        #endregion
#if UNITY_EDITOR
        if ((Input.GetKey(LEFT_KEY) && !usingPhone))
        {
            currentTorque.z += turnRate;
            //currentForce.x -= pushRate;
            isRotating = true;
        }
        else if ((Input.GetKey(RIGHT_KEY) && !usingPhone))
        {
            currentTorque.z -= turnRate;
           // currentForce.x += pushRate;
            isRotating = true;
        }

        if (((Input.GetKeyUp(LEFT_KEY) || Input.GetKeyUp(RIGHT_KEY)) && !usingPhone))
        {
            rb.angularVelocity = Vector3.zero;
            currentTorque.z = 0;
            currentForce.x = 0;
            isRotating = false;
        }


#endif




#if UNITY_ANDROID && !UNITY_EDITOR
        currentTorque.z += turnRate * -Input.acceleration.x;
        if (Mathf.Abs(Input.acceleration.x) > 0.07f)
        {
            isRotating = true;
        }
        else
        {
            isRotating = false;
            currentTorque = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
#endif
        if ((Input.GetKey(UP_KEY) && !usingPhone) || (touchUp && usingPhone))
        {
            currentForce.y += boostRate;
            isMovingUpwards = true;
        }

        if ((Input.GetKeyUp(UP_KEY) && !usingPhone) || (!touchUp && usingPhone))
        {
            currentForce.y = 0;
            isMovingUpwards = false;
        }

        currentForce.x = Mathf.Clamp(currentForce.x, -MAX_FORCE, MAX_FORCE);
        currentForce.y = Mathf.Clamp(currentForce.y, -MAX_FORCE, MAX_FORCE);
        currentTorque.z = Mathf.Clamp(currentTorque.z, -MAX_TORQUE, MAX_TORQUE);

    }

	void OnCollisionEnter(Collision col) {
		if(col.gameObject.tag == "Land" && col.gameObject.name.Contains("Land")) {
			gM.LandedOnPlatform(col.gameObject);
		} 
	}

	void OnCollisionStay(Collision col) {
		if(col.gameObject.tag == "Land" && col.gameObject.name.Contains("Land")) {
			if(currentFuel < fuelTank) {
				currentFuel += Mathf.Pow(Time.deltaTime, 0.2f);
			}
		}
	}

	void PlayerRecover(bool a) {
		if(a) {
			if(hitTimer < 3.0f) {
				hitTimer += Time.deltaTime;
			} else {
				playerHit = false;
			}
		}
	}

	public void PlayerHit(int coinLoss, int fuelLoss) {
		//Here, player will lose fuel and coins. Sucks!
		playerHit = true;
		hitTimer = 0.0f;
		if(currentFuel >= fuelLoss) {
			currentFuel -= fuelLoss;
		}
		gM.PlayerHit(coinLoss);
	}
		
	public bool UpgradeTopSpeed() {
		if(MAX_FORCE >= CAP_FORCE) {
			return false;
		} 
		MAX_FORCE += ((CAP_FORCE-10.0f)/7.0f);
		Debug.Log(MAX_FORCE);
		return true;
	}

	public bool UpdateAcceleration() {
		if(boostRate < CAP_ACCELERATION) {
			boostRate += (((CAP_ACCELERATION-0.15f)/7.0f));
		}
		Debug.Log(boostRate);

		return boostRate < CAP_ACCELERATION;
	}

	public bool UpdateTurnRate() {
		if(turnRate < CAP_TURN) {
			turnRate += (((CAP_TURN-0.085f)/7.0f));
		}
		Debug.Log(turnRate);

		return turnRate < CAP_TURN;
	}

	public bool UpdateMaxTurn() {
		if(MAX_TORQUE < CAP_TORQUE) {
			MAX_TORQUE += ((CAP_TORQUE-1.0f)/7.0f);
		}
		Debug.Log(MAX_TORQUE);

		return MAX_TORQUE < CAP_TORQUE;
	}

	public void isOnPlatform(bool a) {
		landed = a;
	}

}


