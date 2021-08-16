using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
    private DisplayManager displayManager;

    [Header("Interaction Type")]
    public DisplayManager.InteractionTypes interactionType;

    [Header("Arrow")]
    [SerializeField] private Sprite arrowSprite;
    [SerializeField] private Sprite arrowGlowSprite;

    [SerializeField] private Image arrowPlaceholder;

    [SerializeField] private Transform originTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float bounceTime;

    private GameObject player;

    private bool isInRange = false;
    public bool canInteract = false;

    // Start is called before the first frame update
    void Start()
    {
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();

        arrowPlaceholder.sprite = arrowSprite;
        arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 2.5f, originTransform.position.z);
        arrowPlaceholder.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (isInRange && player != null)
        {
            float y = Mathf.PingPong(Time.time * bounceTime, 1);
            arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 1.5f + y, originTransform.position.z);
            arrowPlaceholder.rectTransform.transform.LookAt(cameraTransform);

            // Check the distance from the player
            if (Mathf.Abs(player.transform.position.magnitude - gameObject.transform.position.magnitude) < 2)
            {
                arrowPlaceholder.sprite = arrowGlowSprite;
                canInteract = true;
            }
            else if (!canInteract)
            {
                arrowPlaceholder.sprite = arrowSprite;
            }
        }
    }

    #region - Triggers -

    private void OnTriggerEnter(Collider other)
    {
        player = other.gameObject;

        if (other.CompareTag("Player"))
        {
            isInRange = true;
            arrowPlaceholder.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        player = null;

        if (other.CompareTag("Player"))
        {
            arrowPlaceholder.gameObject.SetActive(false);
            isInRange = false;
            arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 2.5f, originTransform.position.z);
            canInteract = false;
        }
    }

    #endregion

    private void OnDisable()
    {
        canInteract = false;
    }
}
