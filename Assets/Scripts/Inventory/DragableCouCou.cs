using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragableCouCou : EventTrigger
{
    private bool startDragging;
    private Vector2 mousePosition;

    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();

        playerInputActions.UI.Point.performed += x => mousePosition = x.ReadValue<Vector2>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (startDragging)
        {
            Debug.Log(mousePosition);
            transform.position = new Vector2(Mathf.Clamp(mousePosition.x, 210, 960), Mathf.Clamp(mousePosition.y, 90, 890));
        }
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        startDragging = true;
    }

    public override void OnPointerUp(PointerEventData eventData)
    {
        startDragging = false;
    }

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
}
