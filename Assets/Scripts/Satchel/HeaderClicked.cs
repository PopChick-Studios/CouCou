using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HeaderClicked : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{
    private SatchelManager satchelManager;

    public GameObject itemButton;
    public GameObject coucouButton;

    private Color defaultColour;

    private void Awake()
    {
        defaultColour = gameObject.GetComponent<Image>().color;
        satchelManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<SatchelManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        satchelManager.selectedObject = SatchelManager.SelectedObject.Header;

        if (gameObject == itemButton)
        {
            satchelManager.OnItemSection();
        }
        else if (gameObject == coucouButton)
        {
            satchelManager.OnCouCouSection();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        satchelManager.usingMouse = true;
        gameObject.GetComponent<Image>().color = new Color(defaultColour.r * 0.95f, defaultColour.g * 0.95f, defaultColour.b * 0.95f, 1);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        satchelManager.usingMouse = false;
        gameObject.GetComponent<Image>().color = defaultColour;
    }
}
