using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public List<Transform> positions;
    public Animator npcAnimator;

    private void Start()
    {
        StartCoroutine(MoveNPC());
    }

    public IEnumerator MoveNPC()
    {
        Vector3 lookPos;
        Quaternion rotation;
        Vector3 MovePos;

        // animate

        foreach (Transform newposition in positions)
        {
            lookPos = newposition.position - transform.position;
            lookPos.y = 0;
            rotation = Quaternion.LookRotation(lookPos);
            while (transform.rotation.eulerAngles.magnitude - rotation.eulerAngles.magnitude > 1f)
            {
                //Debug.Log("ROTATING: " + (transform.rotation.eulerAngles.magnitude - rotation.eulerAngles.magnitude) + " + " + Quaternion.LerpUnclamped(startRotation, rotation, 3 * Time.deltaTime).eulerAngles.magnitude);
                transform.rotation = Quaternion.LerpUnclamped(transform.rotation, rotation, 3 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }

            MovePos = new Vector3(newposition.position.x, transform.position.y, newposition.position.z);
            MovePos.y = transform.position.y;
            npcAnimator.SetBool("isWalking", true);
            while (Vector3.Distance(MovePos, transform.position) > 0.5)
            {
                //Debug.Log("MOVING: " + Vector3.Distance(MovePos, transform.position));
                transform.position = Vector3.MoveTowards(transform.position, MovePos, 4 * Time.deltaTime);
                yield return new WaitForEndOfFrame();
            }
            npcAnimator.SetBool("isWalking", false);
        }
    }
}
