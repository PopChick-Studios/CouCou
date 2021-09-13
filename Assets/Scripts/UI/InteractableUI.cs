using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractableUI : MonoBehaviour
{
    private QuestMarker questMarker;
    public InventoryList playerInventory;

    [Header("Interaction Type")]
    public DisplayManager.InteractionTypes interactionType;

    public TextMeshProUGUI nameText;
    public string itemName;
    public int itemAmount;
    public float range;
    public float height;

    [Header("Arrow")]
    [SerializeField] private Sprite arrowSprite;

    [SerializeField] private Image arrowPlaceholder;

    [SerializeField] private Transform originTransform;
    [SerializeField] private Transform cameraTransform;

    [SerializeField] private float bounceTime;

    private GameObject player;

    public bool doorOpen = false;
    public bool isInRange = false;
    public bool canInteract = false;

    private void Awake()
    {
        questMarker = GetComponent<QuestMarker>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (interactionType == DisplayManager.InteractionTypes.StarterCouCou)
        {
            itemName = gameObject.name;
            nameText.text = itemName;
            if (!string.IsNullOrEmpty(playerInventory.starterCouCou.coucouName))
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            nameText.text = "";
        }

        arrowPlaceholder.sprite = arrowSprite;
        arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, height + originTransform.position.y + 2.5f, originTransform.position.z);
        arrowPlaceholder.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        nameText.rectTransform.transform.rotation = Quaternion.LookRotation(nameText.rectTransform.transform.position - cameraTransform.position);
        if (isInRange && player != null)
        {
            if (!arrowPlaceholder.gameObject.activeInHierarchy)
            {
                arrowPlaceholder.gameObject.SetActive(true);
            }
            float y = Mathf.PingPong(Time.time * bounceTime, 1);
            arrowPlaceholder.rectTransform.position = new Vector3(originTransform.position.x, height + originTransform.position.y + 1.5f + y, originTransform.position.z);
            arrowPlaceholder.rectTransform.transform.rotation = Quaternion.LookRotation(arrowPlaceholder.rectTransform.transform.position - cameraTransform.position);

            // Check the distance from the player
            if (Vector3.Distance(player.transform.position, originTransform.position) < range)
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

    public void GiveProgress()
    {
        if (questMarker)
        {
            questMarker.questScriptable.subquestProgress++;
        }
    }

    #region - Triggers -

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && enabled)
        {
            player = other.gameObject;
            isInRange = true;
            arrowPlaceholder.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && enabled)
        {
            player = null;
            arrowPlaceholder.gameObject.SetActive(false);
            isInRange = false;
        }
    }

    #endregion

    private void OnDisable()
    {
        arrowPlaceholder.gameObject.SetActive(false);
        canInteract = false;
    }
}
