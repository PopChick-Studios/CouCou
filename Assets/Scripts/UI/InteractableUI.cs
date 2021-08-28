using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    private DisplayManager displayManager;

    [Header("Interaction Type")]
    public DisplayManager.InteractionTypes interactionType;

    public TextMeshProUGUI nameText;
    public string itemName;
    public int itemAmount;

    [Header("Arrow")]
    [SerializeField] private Sprite arrowSprite;

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
        if (interactionType == DisplayManager.InteractionTypes.CouCou)
        {
            itemName = gameObject.name;
            nameText.text = itemName;
        }
        else
        {
            nameText.text = "";
        }
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();

        arrowPlaceholder.sprite = arrowSprite;
        arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 2.5f, originTransform.position.z);
        arrowPlaceholder.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        nameText.rectTransform.transform.rotation = Quaternion.LookRotation(nameText.rectTransform.transform.position - cameraTransform.position);
        if (isInRange && player != null)
        {
            float y = Mathf.PingPong(Time.time * bounceTime, 1);
            arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, originTransform.position.y + 1.5f + y, originTransform.position.z);
            arrowPlaceholder.rectTransform.transform.rotation = Quaternion.LookRotation(arrowPlaceholder.rectTransform.transform.position - cameraTransform.position);

            // Check the distance from the player
            if (Vector3.Distance(player.transform.position, gameObject.transform.position) < 2.5)
            {
                arrowPlaceholder.color = new Color32(255, 212, 73, 255);
                canInteract = true;
            }
            else
            {
                arrowPlaceholder.color = Color.white;
                canInteract = false;
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
        }
    }

    #endregion

    private void OnDisable()
    {
        canInteract = false;
    }
}
