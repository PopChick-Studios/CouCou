using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SatchelOpenClose : MonoBehaviour
{
    private SatchelManager satchelManager;
    private BattlingUI battlingUI;
    private PlayerInputActions playerInputActions;

    public Camera blurCamera;

    private void Awake()
    {
        satchelManager = GameObject.FindGameObjectWithTag("ItemManager").GetComponent<SatchelManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
        

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.performed += x => OnCloseSatchel();

        gameObject.SetActive(false);
    }

    public void OnCloseSatchel()
    {
        satchelManager.selectedSection = 0;

        blurCamera.gameObject.SetActive(false);
        satchelManager.ClearItems();
        satchelManager.ClearCouCou();
        gameObject.SetActive(false);
        battlingUI.BackToMenu();
    }

    public void OnCouCouSection()
    {
        if (satchelManager.selectedSection == 2)
        {
            return;
        }
        satchelManager.selectedSection = 2;
        satchelManager.ClearItems();
        satchelManager.DisplayCouCou();
    }

    public void OnItemSection()
    {
        if (satchelManager.selectedSection == 1)
        {
            return;
        }
        satchelManager.selectedSection = 1;
        satchelManager.ClearCouCou();
        satchelManager.DisplayItems();
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}
