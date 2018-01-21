using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Camera))]
public class SmoothFollow : MonoBehaviour
{

    public float dampTime = 0.15f;
    public float zDistance = 0.0f;
    private Vector3 velocity = Vector3.zero;
    public Transform target;
    private Camera cam;
	public Vector3 offset;


    void Start()
    {
        cam = GetComponent<Camera>();
    }
    void Update()
    {
        if (target)
        {
            Vector3 point = cam.WorldToViewportPoint(target.position);
            Vector3 delta = target.position - cam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
            Vector3 destination = transform.position + delta;
            Vector3 final = Vector3.SmoothDamp(transform.position, destination + offset, ref velocity, dampTime);
            if (final.y < 0)
            {
                transform.position = new Vector3(final.x, 0, zDistance);
            }
            else
            {
                transform.position = new Vector3(final.x, final.y, zDistance);
            }
        }
    }
}