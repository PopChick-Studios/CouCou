using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using TMPro;
using System;

public class SatchelManager : MonoBehaviour
{
    private BattleManager battleManager;
    private ScrollRectEnsureVisible scrollRectEnsureVisible;
    public ScrollRect scrollRect;

    public enum SelectedObject
    {
        Header,
        Slot,
        Submit
    }

    public CouCouDatabase coucouDatabase;
    private List<CouCouDatabase.CouCouData> coucouDataList;

    public InventoryList inventoryList;
    public SatchelSlotController satchelSlotPrefab;

    private InputSystemUIInputModule inputSystemUIInputModule;
    private PlayerInputActions playerInputActions;
    private SatchelOpenClose satchelOpenClose;

    private bool isNavigatingUI;
    public int currentFocusedItem;
    public int currentFocusedCouCou;
    private int navigateDirection;
    public bool usingMouse = false;

    [Header("Sections")]
    public GameObject itemsSection;
    public GameObject coucouSection;
    public int selectedSection;
    public Color defaultButtonColour;
    public int headerSelection = 1;
    public bool possibleSubmitFocus = false;
    public bool submitFocusedPrevious = false;
    public SelectedObject selectedObject;

    [Header("Description")]
    public TextMeshProUGUI descriptionName;
    public TextMeshProUGUI descriptionText;
    public Image itemSprite;
    public Image submitButton;

    [Header("Colour")]
    public Color defaultColour;

    public List<SatchelSlotController> itemSlotList;
    public List<SatchelSlotController> coucouSlotList;

    private void Awake()
    {
        coucouDataList = new List<CouCouDatabase.CouCouData>();

        scrollRectEnsureVisible = gameObject.GetComponentInParent<ScrollRectEnsureVisible>();
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        satchelOpenClose = GameObject.FindGameObjectWithTag("Satchel").GetComponent<SatchelOpenClose>();
        inputSystemUIInputModule = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
        playerInputActions = new PlayerInputActions();

        playerInputActions.UI.Navigate.started += x => RequestToNavigate(x.ReadValue<Vector2>());
        playerInputActions.UI.Navigate.canceled += x => RequestToNavigate(new Vector2(0, 0));
        playerInputActions.UI.Navigate.started += x => ChangeSection();
        playerInputActions.UI.NavigateSections.performed += x => ChangeSection();

        playerInputActions.UI.Submit.performed += x => OnSelect();
    }

    private void Start()
    {
        foreach (CouCouDatabase.CouCouData c in coucouDatabase.coucouData)
        {
            coucouDataList.Add(c);
        }
    }

    public void RequestToNavigate(Vector2 input)
    {
        usingMouse = false;

        // Stops creating multiple coroutines
        StopAllCoroutines();

        if (Mathf.Abs(input.y) > 0.5)
        {
            if (input.y > 0) // Going Up
            {
                navigateDirection = -1;
                possibleSubmitFocus = false;
            }
            else if (input.y < 0) // Going Down
            {
                navigateDirection = 1;
                possibleSubmitFocus = false;
            }
            isNavigatingUI = true;

            if (selectedSection == 1)
            {
                StartCoroutine(NavigateItemUI());
            }
            else if (selectedSection == 2)
            {
                StartCoroutine(NavigateCouCouUI());
            }
            
        }
        else if (Mathf.Abs(input.x) > 0.5)
        {
            isNavigatingUI = false;

            if (input.x > 0) // Going Right
            {
                headerSelection++;
                possibleSubmitFocus = true;
            }
            else if (input.x < 0) // Going Left
            {
                headerSelection--;
                possibleSubmitFocus = false;
            }

            if (headerSelection > 2)
            {
                headerSelection = 2;
            }
            else if (headerSelection < 1)
            {
                headerSelection = 1;
            }
        }
        else
        {
            isNavigatingUI = false;
            StopAllCoroutines();
        }

        if (submitFocusedPrevious != possibleSubmitFocus)
        {
            FocusSubmitButton();
            submitFocusedPrevious = possibleSubmitFocus;
        }
    }

    public void OnSelect()
    {
        switch (selectedObject)
        {
            case SelectedObject.Header:
                if (headerSelection == 1)
                {
                    OnItemSection();
                }
                else if (headerSelection == 2)
                {
                    OnCouCouSection();
                }
                break;

            case SelectedObject.Slot:
                possibleSubmitFocus = true;
                FocusSubmitButton();
                break;

            case SelectedObject.Submit:
                if (selectedSection == 1)
                {
                    battleManager.UseItem(itemSlotList[currentFocusedItem].itemNameText.text);
                }
                else if (selectedSection == 2)
                {
                    battleManager.ChangeCouCou(coucouSlotList[currentFocusedCouCou].itemNameText.text);
                }
                break;
        }

        submitFocusedPrevious = possibleSubmitFocus;
    }

    public void DisplayItems()
    {
        for(int i = 0; i < inventoryList.itemInventory.Count; i++)
        {
            SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, transform);
            newSatchelSlot.itemNameText.text = inventoryList.itemInventory[i].itemName;
            newSatchelSlot.itemAmountText.text = inventoryList.itemInventory[i].itemAmount.ToString();
            newSatchelSlot.uniqueIdentifier = i;
            itemSlotList.Add(newSatchelSlot);
        }

        // Focus the first item
        itemSlotList[0].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);
        descriptionName.text = itemSlotList[0].itemNameText.text;
        currentFocusedItem = 0;
        selectedObject = SelectedObject.Slot;
    }

    public void DisplayCouCou()
    {
        for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
        {
            SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, transform);
            newSatchelSlot.itemNameText.text = inventoryList.couCouInventory[i].coucouName;
            newSatchelSlot.itemAmountText.text = "Lv: " + inventoryList.couCouInventory[i].coucouLevel.ToString();
            newSatchelSlot.uniqueIdentifier = i;
            coucouSlotList.Add(newSatchelSlot);
        }

        // Focus the first item
        coucouSlotList[0].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);
        descriptionName.text = coucouSlotList[0].itemNameText.text;
        currentFocusedCouCou = 0;
        selectedObject = SelectedObject.Slot;
    }

    // Change items in view with mouse
    public void ChangeSlotFocusMouse(GameObject uiPressed)
    {
        if (selectedSection == 1)
        {
            ChangeFocusedItem(currentFocusedItem, uiPressed.GetComponent<SatchelSlotController>().uniqueIdentifier);
            currentFocusedItem = uiPressed.GetComponent<SatchelSlotController>().uniqueIdentifier;
        }
        else if (selectedSection == 2)
        {
            ChangeFocusedCouCou(currentFocusedCouCou, uiPressed.GetComponent<SatchelSlotController>().uniqueIdentifier);
            currentFocusedCouCou = uiPressed.GetComponent<SatchelSlotController>().uniqueIdentifier;
        }
    }

    public void ClearItems()
    {
        descriptionName.text = "";
        descriptionText.text = "";

        currentFocusedItem = 0;
        itemSlotList.Clear();

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearCouCou()
    {
        descriptionName.text = "";
        descriptionText.text = "";

        currentFocusedCouCou = 0;
        coucouSlotList.Clear();

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ChangeFocusedItem(int before, int after)
    {
        if (itemSlotList.Count - 1 < after || after < -1)
        {
            isNavigatingUI = false;
            return;
        }
        else if (before == -1 && after == -1)
        {
            currentFocusedItem = after;

            descriptionName.text = "";
            descriptionText.text = "";

            // Check which button
            if (headerSelection == 1)
            {
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            else if (headerSelection == 2)
            {
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            return;
        }
        else if (after == -1)
        {
            selectedObject = SelectedObject.Header;

            // Reset the colours
            itemSlotList[before].GetComponent<Image>().color = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 1);
            currentFocusedItem = after;

            descriptionName.text = "";
            descriptionText.text = "";

            // Check which button
            if (headerSelection == 1)
            {
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            else if (headerSelection == 2)
            {
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            return;
        }
        else if (before == -1)
        {
            selectedObject = SelectedObject.Slot;

            currentFocusedItem = after;
            itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
            coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
            itemSlotList[after].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);
        }
        else
        {
            currentFocusedItem = after;
            itemSlotList[before].GetComponent<Image>().color = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 1);
        }

        itemSlotList[after].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);

        // Change text
        descriptionName.text = itemSlotList[after].itemNameText.text;
        descriptionText.text = inventoryList.itemInventory[after].itemDescription;
    }

    public void ChangeFocusedCouCou(int before, int after)
    {
        if (coucouSlotList.Count - 1 < after || after < -1)
        {
            isNavigatingUI = false;
            return;
        }
        else if (before == -1 && after == -1)
        {
            currentFocusedCouCou = after;

            descriptionName.text = "";
            descriptionText.text = "";

            // Check which button
            if (headerSelection == 1)
            {
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            else if (headerSelection == 2)
            {
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            return;
        }
        else if (after == -1)
        {
            selectedObject = SelectedObject.Header;

            // Reset the colours
            coucouSlotList[before].GetComponent<Image>().color = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 1);
            currentFocusedCouCou = after;

            descriptionName.text = "";
            descriptionText.text = "";

            // Check which button
            if (headerSelection == 1)
            {
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            else if (headerSelection == 2)
            {
                itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
                coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            }
            return;
        }
        else if (before == -1)
        {
            selectedObject = SelectedObject.Slot;

            currentFocusedCouCou = after;
            itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
            coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r, defaultButtonColour.g, defaultButtonColour.b, 1);
            coucouSlotList[after].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);
        }
        else
        {
            currentFocusedCouCou = after;
            if (!usingMouse)
            {
                scrollRectEnsureVisible.CenterOnItem(coucouSlotList[after].gameObject.GetComponent<RectTransform>());
            }
            coucouSlotList[before].GetComponent<Image>().color = new Color(defaultColour.r, defaultColour.g, defaultColour.b, 1);
        }

        coucouSlotList[after].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);

        // Change text
        descriptionName.text = coucouSlotList[after].itemNameText.text;

        for (int i = 0; i < coucouDataList.Count - 1; i++)
        {
            if (inventoryList.couCouInventory[after].coucouName == coucouDataList[i].coucouName)
            {
                descriptionText.text = coucouDataList[i].coucouDescription;
            }
        }
    }

    public void ChangeSection()
    {
        if ((currentFocusedItem == -1 || currentFocusedCouCou == -1) && headerSelection == 1)
        {
            coucouSection.GetComponent<Image>().color = defaultButtonColour;
            itemsSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
        }
        else if ((currentFocusedItem == -1 || currentFocusedCouCou == -1) && headerSelection == 2)
        {
            itemsSection.GetComponent<Image>().color = defaultButtonColour;
            coucouSection.GetComponent<Image>().color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
        }
    }

    public void FocusSubmitButton()
    {
        if (possibleSubmitFocus && currentFocusedItem != -1 && selectedSection == 1)
        {
            selectedObject = SelectedObject.Submit;
            submitButton.color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            itemSlotList[currentFocusedItem].GetComponent<Image>().color = new Color(defaultColour.r * 0.9f, defaultColour.g * 0.9f, defaultColour.b * 0.9f, 1);
        }
        else if (possibleSubmitFocus && currentFocusedCouCou != -1 && selectedSection == 2)
        {
            selectedObject = SelectedObject.Submit;
            submitButton.color = new Color(defaultButtonColour.r * 0.85f, defaultButtonColour.g * 0.85f, defaultButtonColour.b * 0.85f, 1);
            coucouSlotList[currentFocusedCouCou].GetComponent<Image>().color = new Color(defaultColour.r * 0.9f, defaultColour.g * 0.9f, defaultColour.b * 0.9f, 1);
        }
        else if (currentFocusedCouCou != -1 && currentFocusedItem != -1)
        {
            selectedObject = SelectedObject.Slot;
            
            submitButton.color = defaultButtonColour;
            if (selectedSection == 1)
            {
                itemSlotList[currentFocusedItem].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);
            }
            else
            {
                coucouSlotList[currentFocusedCouCou].GetComponent<Image>().color = new Color(defaultColour.r * 0.85f, defaultColour.g * 0.85f, defaultColour.b * 0.85f, 1);
            }
        }
        else if (selectedObject != SelectedObject.Submit)
        {
            submitButton.color = defaultButtonColour;
        }
    }

    public IEnumerator NavigateItemUI()
    {
        // Wait for delay
        if (currentFocusedItem + navigateDirection == -1)
        {
            headerSelection = 1;
        }
        ChangeFocusedItem(currentFocusedItem, currentFocusedItem + navigateDirection);
        yield return new WaitForSeconds(inputSystemUIInputModule.moveRepeatDelay);

        // Proceed through UI
        while (isNavigatingUI)
        {
            ChangeFocusedItem(currentFocusedItem, currentFocusedItem + navigateDirection);
            if (currentFocusedItem + navigateDirection <= -1)
            {
                headerSelection = 1;
                isNavigatingUI = false;
                yield return null;
            }
            yield return new WaitForSeconds(inputSystemUIInputModule.moveRepeatRate);
        }
    }

    public IEnumerator NavigateCouCouUI()
    {
        // Wait for delay
        if (currentFocusedCouCou + navigateDirection == -1)
        {
            headerSelection = 2;
        }
        ChangeFocusedCouCou(currentFocusedCouCou, currentFocusedCouCou + navigateDirection);
        yield return new WaitForSeconds(inputSystemUIInputModule.moveRepeatDelay);

        // Proceed through UI
        while (isNavigatingUI)
        {
            ChangeFocusedCouCou(currentFocusedCouCou, currentFocusedCouCou + navigateDirection);
            if (currentFocusedCouCou + navigateDirection <= -1)
            {
                headerSelection = 2;
                isNavigatingUI = false;
                yield return null;
            }
            yield return new WaitForSeconds(inputSystemUIInputModule.moveRepeatRate);
        }
    }

    #region - Section Changes -

    public void OnItemSection()
    {
        headerSelection = 1;
        currentFocusedCouCou = 0;
        satchelOpenClose.OnItemSection();
        ChangeFocusedItem(currentFocusedItem, -1);
        scrollRect.normalizedPosition = new Vector2(0, 1);
        scrollRect.enabled = false;

        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use Item";
    }

    public void OnCouCouSection()
    {
        headerSelection = 2;
        currentFocusedItem = 0;
        satchelOpenClose.OnCouCouSection();
        ChangeFocusedCouCou(currentFocusedCouCou, -1);
        gameObject.GetComponentInParent<ScrollRect>().enabled = true;

        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou";
    }

    #endregion

    #region - Enable / Disable -

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
