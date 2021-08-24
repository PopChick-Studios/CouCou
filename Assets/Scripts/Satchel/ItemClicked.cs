using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemClicked : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{
    private SatchelManager satchelManager;

    private Color defaultColour;

    private void Awake()
    {
        defaultColour = gameObject.GetComponent<Image>().color;
        satchelManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<SatchelManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        satchelManager.selectedObject = SatchelManager.SelectedObject.Slot;
        satchelManager.ChangeSlotFocusMouse(gameObject);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        satchelManager.usingMouse = true;
        satchelManager.ChangeSlotFocusMouse(gameObject);
        gameObject.GetComponent<Image>().color = new Color(defaultColour.r * 0.95f, defaultColour.g * 0.95f, defaultColour.b * 0.95f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        satchelManager.usingMouse = false;
        gameObject.GetComponent<Image>().color = defaultColour;
    }
}
