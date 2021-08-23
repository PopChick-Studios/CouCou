using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SatchelSlotController : MonoBehaviour, ISelectHandler, IPointerClickHandler
{
    private SatchelManager satchelManager;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemAmountText;
    public string itemDescription;
    public int uniqueIdentifier;

    private void Awake()
    {
        satchelManager = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<SatchelManager>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!satchelManager.inSubmit)
        {
            satchelManager.currentSelectedButton = gameObject;
            satchelManager.UpdateDescription();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //satchelManager.inSubmit = false;
        gameObject.GetComponent<Button>().Select();
        satchelManager.currentSelectedButton = gameObject;
        satchelManager.UpdateDescription();
    }
}
