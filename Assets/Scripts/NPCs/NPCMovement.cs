using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private DialogueManager dialogueManager;

    public QuestScriptable questScriptable;
    public List<Transform> positions;
    public Animator npcAnimator;
    public bool isMoving = false;

    private void Awake()
    {
        dialogueManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DialogueManager>();
    }

    public IEnumerator MoveNPC(List<Dialogue> dialogue)
    {
        isMoving = true;
        Vector3 lookPos;
        Quaternion rotation;
        Vector3 MovePos;

        // animate

        foreach (Transform newposition in positions)
        {
            lookPos = newposition.position - transform.position;
            lookPos.y = 0;
            rotation = Quaternion.LookRotation(lookPos);
            Debug.Log(transform.rotation.eulerAngles.magnitude - rotation.eulerAngles.magnitude);
            while (Mathf.Abs(transform.rotation.eulerAngles.magnitude - rotation.eulerAngles.magnitude) > 1f)
            {
                Debug.Log("ROTATING: " + (transform.rotation.eulerAngles.magnitude - rotation.eulerAngles.magnitude) + " + " + Quaternion.LerpUnclamped(transform.rotation, rotation, 3 * Time.deltaTime).eulerAngles.magnitude);
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

        isMoving = false;

        if (dialogue != null)
        {
            StartCoroutine(dialogueManager.StartDialogue(dialogue));
            yield return new WaitUntil(() => dialogueManager.dialogueFinished);
            questScriptable.subquestProgress++;
        }
    }
}
