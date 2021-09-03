using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SatchelAdventureManager : MonoBehaviour
{
    private ScrollRectEnsureVisible scrollRectEnsureVisible;
    private CouCouFinder coucouFinder;
    private ItemFinder itemFinder;
    private InventoryManager inventoryManager;
    private AbilityDescriptions abilityDescriptions;
    private DisplayManager displayManager;

    public InventoryList playerInventory;

    public CouCouDatabase coucouDatabase;
    private List<CouCouDatabase.CouCouData> coucouDataList;

    public GameObject satchelList;
    public GameObject abilitiesDescriptionUI;
    public GameObject abilitiesElementChart;
    public GameObject elementChart;

    public ScrollRect scrollRect;
    public Camera blurCamera;
    public GameObject satchel;
    public GameObject dialogueBox;
    public GameObject dialogueText;
    public GameObject changeCouCouOrder;
    public GameObject inputCouCouChange;
    private int inputNumber;

    public InventoryList inventoryList;
    public SatchelSlotControllerAdventure satchelSlotPrefab;

    private PlayerInputActions playerInputActions;

    public bool inSubmit;
    public bool inPrompt;
    public bool changingCouCou = false;
    public bool isStuck;
    public string itemChosen;
    private Button buttonClicked;
    public GameObject currentSelectedButton;

    [Header("Sections")]
    public Button itemsSection;
    public Button coucouSection;
    public int selectedSection;

    [Header("Description")]
    public TextMeshProUGUI descriptionName;
    public TextMeshProUGUI descriptionText;
    public Image itemSprite;
    public Button submitButton;

    [Header("Stats")]
    public GameObject statDisplay;
    public TextMeshProUGUI healthPointsText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI resistanceText;
    public TextMeshProUGUI determinationText;
    public TextMeshProUGUI mindsetText;

    public List<SatchelSlotControllerAdventure> itemSlotList;
    public List<SatchelSlotControllerAdventure> coucouSlotList;

    private void Awake()
    {
        coucouDataList = new List<CouCouDatabase.CouCouData>();

        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        scrollRectEnsureVisible = satchelList.GetComponentInParent<ScrollRectEnsureVisible>();
        coucouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();
        displayManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<DisplayManager>();
        abilityDescriptions = GetComponent<AbilityDescriptions>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.NavigateSections.performed += x => NavigateSections(x.ReadValue<float>());
        playerInputActions.UI.OpenElementChart.started += x => OnElementChartToggle();
        changeCouCouOrder.SetActive(false);
        satchel.SetActive(false);
        abilitiesDescriptionUI.SetActive(false);
    }

    private void Start()
    {
        foreach (CouCouDatabase.CouCouData c in coucouDatabase.coucouData)
        {
            coucouDataList.Add(c);
        }
    }

    public void OnElementChartToggle()
    {
        if (satchel.activeInHierarchy && !abilitiesElementChart.activeInHierarchy && !elementChart.activeInHierarchy)
        {
            if (inSubmit)
            {
                abilitiesElementChart.SetActive(true);
            }
            else
            {
                elementChart.SetActive(true);
            }
        }
        else if (satchel.activeInHierarchy && (abilitiesElementChart.activeInHierarchy || elementChart.activeInHierarchy))
        {
            elementChart.SetActive(false);
            abilitiesElementChart.SetActive(false);
        }
    }

    public void DisplayItems()
    {
        statDisplay.SetActive(false);

        //scrollRect.enabled = false;

        for (int i = 0; i < inventoryList.itemInventory.Count; i++)
        {
            SatchelSlotControllerAdventure newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
            ItemsDatabase.ItemData item = itemFinder.FindItem(inventoryList.itemInventory[i].itemName);
            newSatchelSlot.itemNameText.text = item.itemName;
            newSatchelSlot.itemAmountText.text = inventoryList.itemInventory[i].itemAmount.ToString();
            newSatchelSlot.itemDescription = item.itemDescription;
            newSatchelSlot.uniqueIdentifier = i;
            newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
            itemSlotList.Insert(Mathf.Min(item.positionIndex, itemSlotList.Count), newSatchelSlot);
        }
        //scrollRect.enabled = true;
        for (int i = 0; i < itemSlotList.Count; i++)
        {
            Navigation newNav = new Navigation();
            newNav.mode = Navigation.Mode.Explicit;

            if (i != 0)
            {
                newNav.selectOnUp = itemSlotList[i - 1].GetComponent<Button>();
            }

            if (i != itemSlotList.Count - 1)
            {
                newNav.selectOnDown = itemSlotList[i + 1].GetComponent<Button>();
            }

            itemSlotList[i].GetComponent<Button>().navigation = newNav;
        }

        EventSystem.current.SetSelectedGameObject(null);
        if (itemSlotList.Count != 0)
        {
            currentSelectedButton = itemSlotList[0].gameObject;
            itemSlotList[0].GetComponent<Button>().Select();
            descriptionName.text = itemSlotList[0].itemNameText.text;
            descriptionText.text = itemSlotList[0].itemDescription;
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.itemInventory[itemSlotList[0].uniqueIdentifier].element);
            if (itemSprite.sprite == null)
            {
                itemSprite.enabled = false;
            }
            else
            {
                itemSprite.enabled = true;
            }
        }
        else
        {
            descriptionText.text = "You don't own any items right now";
            submitButton.interactable = false;
            itemSprite.enabled = false;
            submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public void DisplayCouCou()
    {
        statDisplay.SetActive(true);

        scrollRect.enabled = false;
        inventoryManager.SortCouCouInventory();
        scrollRect.enabled = true;

        for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
        {
            SatchelSlotControllerAdventure newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
            CouCouDatabase.CouCouData newCouCou = coucouFinder.FindCouCou(inventoryList.couCouInventory[i].coucouName);
            newSatchelSlot.itemNameText.text = inventoryList.couCouInventory[i].coucouName;
            newSatchelSlot.itemAmountText.text = "Lv: " + inventoryList.couCouInventory[i].coucouLevel.ToString();
            newSatchelSlot.uniqueIdentifier = i;
            if (inventoryList.couCouInventory[i].hasCollapsed)
            {
                newSatchelSlot.coucouOrder.text = "Collapsed";
                newSatchelSlot.coucouOrder.color = Color.gray;
            }
            else
            {
                newSatchelSlot.coucouOrder.text = "Position " + inventoryList.couCouInventory[i].lineupOrder;
            }
            newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
            newSatchelSlot.itemDescription = newCouCou.coucouDescription;
            coucouSlotList.Add(newSatchelSlot);
        }

        for (int i = 0; i < coucouSlotList.Count; i++)
        {
            Navigation newNav = new Navigation();
            newNav.mode = Navigation.Mode.Explicit;

            if (i != 0)
            {
                newNav.selectOnUp = coucouSlotList[i - 1].GetComponent<Button>();
            }

            if (i != coucouSlotList.Count - 1)
            {
                newNav.selectOnDown = coucouSlotList[i + 1].GetComponent<Button>();
            }
            coucouSlotList[i].GetComponent<Button>().navigation = newNav;
        }

        EventSystem.current.SetSelectedGameObject(null);

        if (coucouSlotList.Count != 0)
        {
            currentSelectedButton = coucouSlotList[0].gameObject;
            coucouSlotList[0].GetComponent<Button>().Select();
            descriptionName.text = coucouSlotList[0].itemNameText.text;
            descriptionText.text = coucouSlotList[0].itemDescription;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + Mathf.Round(inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentResistance * 1000) / 1000;
            determinationText.text = "Det: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentMindset;
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].element);

            if (isStuck)
            {
                Debug.Log("Text changed");
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Give " + itemChosen;
            }
        }
        else
        {
            descriptionText.text = "You don't own any CouCou right now";
            statDisplay.SetActive(false);
            submitButton.interactable = false;
            itemSprite.enabled = false;
            submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
            Debug.Log("Text changed");
        }
    }

    public void ClearItems()
    {
        descriptionName.text = "";
        descriptionText.text = "";

        itemSlotList.Clear();

        foreach (Transform child in satchelList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ClearCouCou()
    {
        descriptionName.text = "";
        descriptionText.text = "";

        coucouSlotList.Clear();

        foreach (Transform child in satchelList.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void GoToSubmit(Button button)
    {
        inSubmit = true;
        buttonClicked = button;
        submitButton.Select();
        if (selectedSection == 1)
        {

        }
        else if (selectedSection == 2)
        {
            abilityDescriptions.DisplayAbilityDescriptions(coucouFinder.FindCouCou(button.GetComponent<SatchelSlotControllerAdventure>().itemNameText.text));
            abilitiesDescriptionUI.SetActive(true);
        }
    }

    public void GoBack()
    {
        if (isStuck && !inPrompt && !inSubmit)
        {
            isStuck = false;
            itemsSection.enabled = true;
            OnItemSection();
        }
        else if (inPrompt)
        {
            submitButton.Select();
        }
        else if (inSubmit)
        {
            if (buttonClicked != null)
            {
                buttonClicked.Select();
            }
            buttonClicked = null;
            inSubmit = false;
            abilitiesDescriptionUI.SetActive(false);
        }
        else if (changingCouCou)
        {
            changeCouCouOrder.SetActive(false);
            inputCouCouChange.GetComponent<TMP_InputField>().text = "";
            changingCouCou = false;
            abilitiesDescriptionUI.SetActive(false);
        }
        else
        {
            OnCloseSatchel();
        }
    }

    public void OnCloseSatchel()
    {
        selectedSection = 0;

        blurCamera.gameObject.SetActive(false);
        ClearItems();
        ClearCouCou();
        satchel.SetActive(false);
    }

    public void UpdateDescription()
    {
        Vector3 defaultPosition = new Vector3(1335, 140, 0);
        submitButton.interactable = true;
        itemSprite.enabled = true;
        SatchelSlotControllerAdventure satchelSlot = currentSelectedButton.GetComponent<SatchelSlotControllerAdventure>();
        if (selectedSection == 1)
        {
            descriptionName.text = satchelSlot.itemNameText.text;
            descriptionText.text = satchelSlot.itemDescription;
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.itemInventory[satchelSlot.uniqueIdentifier].element);
            submitButton.transform.position = defaultPosition;

            ItemsDatabase.ItemData item = itemFinder.FindItem(satchelSlot.itemNameText.text);
            if (item.itemAttribute != ItemsDatabase.ItemAttribute.ElementalMindset && item.itemAttribute != ItemsDatabase.ItemAttribute.Experience && item.itemAttribute != ItemsDatabase.ItemAttribute.Health)
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "This item can't be used right now";
            }

            if (itemSprite.sprite == null)
            {
                itemSprite.enabled = false;
            }
            else
            {
                itemSprite.enabled = true;
            }
        }
        else if (selectedSection == 2)
        {
            descriptionName.text = satchelSlot.itemNameText.text;
            descriptionText.text = satchelSlot.itemDescription;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + Mathf.Round(inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentResistance * 1000) / 1000;
            determinationText.text = "Det: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentMindset;
            scrollRectEnsureVisible.CenterOnItem(currentSelectedButton.GetComponent<RectTransform>());
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].element);

            if (inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].hasCollapsed && !isStuck)
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "You can't change the position of a collasped CouCou";
                submitButton.transform.position = new Vector2(defaultPosition.x, defaultPosition.y + 50);
                submitButton.interactable = false;
            }
            else if (isStuck)
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Give " + itemChosen;
                submitButton.transform.position = defaultPosition;
            }
            else
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou Position";
                submitButton.interactable = true;
                submitButton.transform.position = defaultPosition;
            }
        }
    }

    public void OnCouCouOrderChange()
    {
        if (inputCouCouChange.GetComponent<TMP_InputField>().text == "" || inputCouCouChange.GetComponent<TMP_InputField>().text == null)
        {
            changeCouCouOrder.SetActive(false);
            return;
        }
        inputNumber = int.Parse(inputCouCouChange.GetComponent<TMP_InputField>().text);

        if (inputNumber > inventoryList.couCouInventory.Count)
        {
            inputNumber = inventoryList.couCouInventory.Count;
        }
        else if (inputNumber < 1)
        {
            inputNumber = 1;
        }

        inputCouCouChange.GetComponent<TMP_InputField>().text = "";
        for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
        {
            if (descriptionName.text == inventoryList.couCouInventory[i].coucouName)
            {
                inventoryManager.MoveCouCou(inventoryList.couCouInventory[i], inputNumber - 1);
                break;
            }
        }

        ClearCouCou();
        DisplayCouCou();
        scrollRect.normalizedPosition = new Vector2(0, 1);
        changeCouCouOrder.SetActive(false);
    }

    public void OnSubmitPressed()
    {
        if (selectedSection == 1)
        {
            itemChosen = descriptionName.text;
            OpenCouCouSelect();
        }
        else if (selectedSection == 2 && !isStuck)
        {
            changingCouCou = true;
            changeCouCouOrder.SetActive(true);
        }
        else if (isStuck)
        {
            UseItem(descriptionName.text, itemChosen);
        }
    }

    public void UseItem(string coucouName, string itemName)
    {
        ItemsDatabase.ItemData item = itemFinder.FindItem(itemName);

        InventoryList.CouCouInventory coucou = null;
        foreach (InventoryList.CouCouInventory c in playerInventory.couCouInventory)
        {
            if (c.coucouName == coucouName)
            {
                coucou = c;
            }
        }

        if (coucou == null)
        {
            Debug.LogError("CouCou Not Found");
        }

        switch (item.itemAttribute)
        {
            case ItemsDatabase.ItemAttribute.Health:
                coucou.currentHealth += Mathf.RoundToInt(coucou.maxHealth * 0.3f);
                break;
            case ItemsDatabase.ItemAttribute.ElementalMindset:
                if (item.element == coucou.element)
                {
                    coucou.coucouEXP += 300;
                }
                else
                {
                    coucou.coucouEXP += 50;
                }
                break;
            case ItemsDatabase.ItemAttribute.Experience:
                coucou.coucouEXP += 3000;
                break;
        }

        
        isStuck = false;
        itemsSection.enabled = true;
        OnItemSection();
        Debug.Log("Used " + itemName + " on " + coucouName);


    }


    #region - Section Changes -

    public void NavigateSections(float direction)
    {
        if (satchel.activeInHierarchy)
        {
            if (isStuck)
            {
                return;
            }
            if (direction > 0 && selectedSection == 1 && !inSubmit)
            {
                OnCouCouSection();
            }
            else if (direction < 0 && selectedSection == 2 && !inSubmit)
            {
                OnItemSection();
            }
        }
    }

    public void OnItemSection()
    {
        if (selectedSection == 1)
        {
            itemSlotList[0].GetComponent<Button>().Select();
            return;
        }
        else if (isStuck)
        {
            coucouSlotList[0].GetComponent<Button>().Select();
            return;
        }
        abilitiesDescriptionUI.SetActive(false);
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use Item";
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 1;
        ClearCouCou();
        DisplayItems();
        scrollRect.normalizedPosition = new Vector2(0, 1);
        scrollRect.enabled = false;
    }

    public void OnCouCouSection()
    {
        if (selectedSection == 2)
        {
            coucouSlotList[0].GetComponent<Button>().Select();
            return;
        }
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou Position";
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 2;
        ClearItems();
        DisplayCouCou();
        scrollRect.enabled = true;
        scrollRect.normalizedPosition = new Vector2(0, 1);
    }

    public void OpenCouCouSelect()
    {
        inSubmit = false;
        isStuck = true;
        itemsSection.enabled = false;
        submitButton.interactable = true;
        OnCouCouSection();
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