using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractableUI : MonoBehaviour
{
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

            if (Mathf.Abs(player.transform.position.magnitude - gameObject.transform.position.magnitude) < 3)
            {
                arrowPlaceholder.sprite = arrowGlowSprite;
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

        Debug.Log(other.transform.position.magnitude - gameObject.transform.position.magnitude);

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
        }
    }

    #endregion
}
