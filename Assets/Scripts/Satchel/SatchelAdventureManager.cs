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

    public CouCouDatabase coucouDatabase;
    private List<CouCouDatabase.CouCouData> coucouDataList;

    public GameObject satchelList;

    public ScrollRect scrollRect;
    public Camera blurCamera;
    public GameObject satchel;
    public GameObject dialogueBox;
    //public GameObject prompt;
    //public GameObject promptText;
    public GameObject dialogueText;
    public GameObject changeCouCouOrder;
    public GameObject inputCouCouChange;
    private int inputNumber;

    public InventoryList inventoryList;
    public SatchelSlotControllerAdventure satchelSlotPrefab;

    private PlayerInputActions playerInputActions;

    public bool inSubmit;
    public bool inPrompt;
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

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.started += x => GoBack();
        playerInputActions.UI.NavigateSections.performed += x => NavigateSections(x.ReadValue<float>());
        changeCouCouOrder.SetActive(false);
        satchel.SetActive(false);
        
    }

    private void Start()
    {
        foreach (CouCouDatabase.CouCouData c in coucouDatabase.coucouData)
        {
            coucouDataList.Add(c);
        }
    }

    public void DisplayItems()
    {
        statDisplay.SetActive(false);

        for (int i = 0; i < inventoryList.itemInventory.Count; i++)
        {
            SatchelSlotControllerAdventure newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
            ItemsDatabase.ItemData item = itemFinder.FindItem(inventoryList.itemInventory[i].itemName);
            newSatchelSlot.itemNameText.text = item.itemName;
            newSatchelSlot.itemAmountText.text = inventoryList.itemInventory[i].itemAmount.ToString();
            newSatchelSlot.itemDescription = item.itemDescription;
            newSatchelSlot.uniqueIdentifier = i;
            //newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
            itemSlotList.Insert(Mathf.Min(item.positionIndex, itemSlotList.Count), newSatchelSlot);
        }
        
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
            itemSlotList[0].GetComponent<Button>().Select();
            descriptionName.text = itemSlotList[0].itemNameText.text;
        }
        else
        {
            descriptionText.text = "You don't own any items right now";
            submitButton.interactable = false;
        }
    }

    public void DisplayCouCou()
    {
        statDisplay.SetActive(true);

        inventoryManager.SortCouCouInventory();

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
            coucouSlotList[0].GetComponent<Button>().Select();
            descriptionName.text = coucouSlotList[0].itemNameText.text;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentResistance;
            determinationText.text = "Det: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentMindset;
        }
        else
        {
            descriptionText.text = "You don't own any CouCou right now";
            submitButton.interactable = false;
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
    /*
    public void GoToSubmit(Button button)
    {
        inPrompt = false;
        prompt.SetActive(false);
        dialogueText.SetActive(true);
        dialogueBox.SetActive(false);
        inSubmit = true;
        buttonClicked = button;
        submitButton.Select();
    }
    */
    public void GoBack()
    {
        if (inPrompt)
        {
            submitButton.Select();
            //OnSubmitCancelled();
        }
        else if (inSubmit)
        {
            buttonClicked.Select();
            buttonClicked = null;
            inSubmit = false;
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
        SatchelSlotControllerAdventure satchelSlot = currentSelectedButton.GetComponent<SatchelSlotControllerAdventure>();
        if (selectedSection == 1)
        {
            descriptionName.text = satchelSlot.itemNameText.text;
            descriptionText.text = satchelSlot.itemDescription;
        }
        else if (selectedSection == 2)
        {
            descriptionName.text = satchelSlot.itemNameText.text;
            descriptionText.text = satchelSlot.itemDescription;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentResistance;
            determinationText.text = "Det: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentMindset;
            scrollRectEnsureVisible.CenterOnItem(currentSelectedButton.GetComponent<RectTransform>());
        }
    }

    public void OnSubmitPressed()
    {
        if (selectedSection == 1)
        {
            
        }
        else if (selectedSection == 2)
        {
            changeCouCouOrder.SetActive(true);
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

    /*
    public void OnSubmitPressed()
    {
        inPrompt = true;
        if (selectedSection == 1)
        {
            promptText.GetComponent<TextMeshProUGUI>().text = "Do you want to use a " + descriptionName.text + "?";
        }
        else if (selectedSection == 2)
        {
            promptText.GetComponent<TextMeshProUGUI>().text = "Do you want to swap " + battleManager.activeCouCou.coucouName + " with " + descriptionName.text + "?";
        }
        else if (selectedSection == 3)
        {
            promptText.GetComponent<TextMeshProUGUI>().text = "Do you want to give a " + descriptionName.text + " to the wild " + battleSystem.enemy.coucouName + "?";
        }
        dialogueBox.SetActive(true);
        dialogueText.SetActive(false);
        prompt.SetActive(true);
        acceptButton.GetComponent<Button>().Select();
    }

    
    public void OnSubmitAccepted()
    {
        inPrompt = false;
        inSubmit = false;
        prompt.SetActive(false);
        dialogueText.SetActive(true);

        if (selectedSection == 1)
        {
            dialogueText.GetComponent<TextMeshProUGUI>().text = "You use " + descriptionName.text;
            StartCoroutine(battleManager.UseItem(descriptionName.text));
        }
        else if (selectedSection == 2)
        {
            StartCoroutine(battleManager.ChangeCouCou(descriptionName.text));
        }
        else if (selectedSection == 3)
        {
            dialogueText.GetComponent<TextMeshProUGUI>().text = "You use " + descriptionName.text;
            StartCoroutine(enemyManager.GiveBerry(itemFinder.FindItem(descriptionName.text).element));
        }
        selectedSection = 0;
    }

    public void OnSubmitCancelled()
    {
        submitButton.Select();
        inPrompt = false;
        prompt.SetActive(false);
        dialogueText.SetActive(true);
        dialogueBox.SetActive(false);
    }

    public void OnGiveBerry()
    {
        selectedSection = 3;
        OnSubmitPressed();
    }
    */

    #region - Section Changes -

    public void NavigateSections(float direction)
    {
        if (direction > 0 && selectedSection == 1 && !inSubmit)
        {
            OnCouCouSection();
        }
        else if (direction < 0 && selectedSection == 2 && !inSubmit)
        {
            OnItemSection();
        }
    }

    public void OnItemSection()
    {
        if (selectedSection == 1)
        {
            return;
        }
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 1;
        ClearCouCou();
        DisplayItems();
        scrollRect.normalizedPosition = new Vector2(0, 1);
        scrollRect.enabled = false;
        
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use Item";
    }

    public void OnCouCouSection()
    {
        if (selectedSection == 2)
        {
            return;
        }
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 2;
        ClearItems();
        DisplayCouCou();
        scrollRect.enabled = true;
        scrollRect.normalizedPosition = new Vector2(0, 1);

        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou Position";
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
