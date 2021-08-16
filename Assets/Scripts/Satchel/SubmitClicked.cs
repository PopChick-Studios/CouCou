using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SubmitClicked : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerEnterHandler
{
    private SatchelManager satchelManager;
    private BattleManager battleManager;

    private Color defaultColour;

    private void Awake()
    {
        defaultColour = gameObject.GetComponent<Image>().color;

        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        satchelManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<SatchelManager>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        satchelManager.selectedObject = SatchelManager.SelectedObject.Submit;

        if (satchelManager.selectedSection == 1)
        {
            battleManager.UseItem(satchelManager.descriptionName.text);
        }
        else if (satchelManager.selectedSection == 2)
        {
            battleManager.ChangeCouCou(satchelManager.descriptionName.text);
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
