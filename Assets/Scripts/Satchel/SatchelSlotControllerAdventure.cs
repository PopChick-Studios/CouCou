using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SatchelSlotControllerAdventure : MonoBehaviour, ISelectHandler, IPointerClickHandler
{
    private SatchelAdventureManager satchelAdventureManager;

    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemAmountText;
    public TextMeshProUGUI coucouOrder;
    public string itemDescription;
    public int uniqueIdentifier;

    private void Awake()
    {
        satchelAdventureManager = GameObject.FindGameObjectWithTag("AdventureUI").GetComponent<SatchelAdventureManager>();
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (!satchelAdventureManager.inSubmit)
        {
            satchelAdventureManager.currentSelectedButton = gameObject;
            satchelAdventureManager.UpdateDescription();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        gameObject.GetComponent<Button>().Select();
        satchelAdventureManager.currentSelectedButton = gameObject;
        satchelAdventureManager.UpdateDescription();
    }
}
