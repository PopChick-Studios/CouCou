using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject advantageChart;

    private void OnEnable()
    {
        advantageChart.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        advantageChart.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        advantageChart.SetActive(false);
    }
}
