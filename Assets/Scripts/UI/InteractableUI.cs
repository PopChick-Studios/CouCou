using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    private PlayerInteraction playerInteraction;

    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite arrowGlowSprite;

    [SerializeField] private Image arrowPlaceholder;

    [SerializeField] private Transform originTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float bounceTime;

    private bool isInRange = false;
    public bool canInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();

        arrowPlaceholder.sprite = arrowSprite;
        arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 2.5f, originTransform.position.z);
        arrowPlaceholder.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {


        if (isInRange)
        {
            float y = Mathf.PingPong(Time.time * bounceTime, 1);
            arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 1.5f + y, originTransform.position.z);
            arrowPlaceholder.rectTransform.transform.LookAt(cameraTransform);

            if (canInteract)
            {
                arrowPlaceholder.sprite = arrowGlowSprite;
            }
            else if (!canInteract)
            {
                arrowPlaceholder.sprite = arrowSprite;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.transform.position.magnitude - gameObject.transform.position.magnitude);

        if (other.CompareTag("Player"))
        {
            isInRange = true;
            arrowPlaceholder.gameObject.SetActive(true);

            if (Mathf.Abs(other.transform.position.magnitude - gameObject.transform.position.magnitude) < 3)
            {
                canInteract = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            arrowPlaceholder.gameObject.SetActive(false);
            isInRange = false;
            arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 2.5f, originTransform.position.z);
        }
    }
}
