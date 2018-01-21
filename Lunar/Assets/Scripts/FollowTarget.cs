using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

    [SerializeField]
	private GameObject target;
    [SerializeField]
    private Vector3 offset;

    void Start()
    {

    }


	void Update()
    {
        transform.position = target.transform.position + offset;// + new Vector3(0.0f, (target.GetComponent<Player>().desiredSpeed * Time.deltaTime), 0.0f);
    }
}
