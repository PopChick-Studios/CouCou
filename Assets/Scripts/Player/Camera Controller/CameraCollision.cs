using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCollision : MonoBehaviour
{
    public float minDistance;
    public float maxDistance;
    public float smooth;
    private Vector3 dollyDirection;
    public float distance;


    // Start is called before the first frame update
    void Awake()
    {
        dollyDirection = transform.localPosition.normalized;
        distance = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 desiredCameraPos = transform.parent.TransformPoint(dollyDirection * maxDistance);
        RaycastHit hit;

        Debug.DrawLine(transform.parent.position, desiredCameraPos, Color.green);

        if(Physics.Linecast(transform.parent.position, desiredCameraPos, out hit) && !hit.collider.gameObject.CompareTag("Player"))
        {
            distance = Mathf.Clamp(hit.distance * 0.8f, minDistance, maxDistance);
        }
        else
        {
            distance = maxDistance;
        }

        transform.localPosition = Vector3.Lerp(transform.localPosition, dollyDirection * distance, Time.deltaTime * smooth);
    }
}
