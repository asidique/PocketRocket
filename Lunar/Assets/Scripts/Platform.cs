using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour
{

	void OnTriggerEnter(Collider col) {
		if(col.tag == "Enemy") {
			Destroy(col.gameObject);
		}
		if(col.tag == "Player") {
			col.GetComponent<Player>().isOnPlatform(true);
		
			if(gameObject.name == "Bottom") {
				Physics.IgnoreLayerCollision(9,10,true);
			}
		}
	}

	void OnTriggerExit(Collider col) {
		if(col.tag == "Player") {
			col.GetComponent<Player>().isOnPlatform(false);
			if(gameObject.name == "Bottom") {
				Physics.IgnoreLayerCollision(9,10,false);
			}
		}
	}
//    void OnTriggerStay(Collider col)
//    {
//        Player player = col.gameObject.GetComponent<Player>();
//        if (player != null)
//        {
//            if (player.IsGrounded() && !player.HasLanded())
//            {
//                Vector3 landPosition = transform.position;
//                landPosition.y += 2f;
//                player.Land(landPosition);
//            }
//        }
//    }
//
//    void OnTriggerExit(Collider col)
//    {
//        Player player = col.gameObject.GetComponent<Player>();
//        if (player != null)
//        {
//            player.SetLanded(false);
//        }
//    }
}
