using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SatchelManager : MonoBehaviour
{
    private BattleManager battleManager;
    private BattlingUI battlingUI;
    private ScrollRectEnsureVisible scrollRectEnsureVisible;
    private CouCouFinder coucouFinder;
    private ItemFinder itemFinder;
    private BattleSystem battleSystem;
    private EnemyManager enemyManager;

    public CouCouDatabase coucouDatabase;
    private List<CouCouDatabase.CouCouData> coucouDataList;

    public GameObject satchelList;

    public ScrollRect scrollRect;
    public Camera blurCamera;
    public GameObject satchel;
    public GameObject dialogueBox;
    public GameObject prompt;
    public GameObject promptText;
    public GameObject dialogueText;
    public GameObject acceptButton;

    public InventoryList inventoryList;
    public SatchelSlotController satchelSlotPrefab;

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
    public Button giveBerryButton;

    [Header("Stats")]
    public GameObject statDisplay;
    public TextMeshProUGUI healthPointsText;
    public TextMeshProUGUI attackText;
    public TextMeshProUGUI resistanceText;
    public TextMeshProUGUI determinationText;
    public TextMeshProUGUI mindsetText;

    public List<SatchelSlotController> itemSlotList;
    public List<SatchelSlotController> coucouSlotList;

    private void Awake()
    {
        coucouDataList = new List<CouCouDatabase.CouCouData>();

        scrollRectEnsureVisible = satchelList.GetComponentInParent<ScrollRectEnsureVisible>();

        enemyManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>();
        battleSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleSystem>();
        coucouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Cancel.started += x => GoBack();
        playerInputActions.UI.NavigateSections.performed += x => NavigateSections(x.ReadValue<float>());

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
            SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
            ItemsDatabase.ItemData item = itemFinder.FindItem(inventoryList.itemInventory[i].itemName);
            newSatchelSlot.itemNameText.text = item.itemName;
            newSatchelSlot.itemAmountText.text = inventoryList.itemInventory[i].itemAmount.ToString();
            newSatchelSlot.itemDescription = item.itemDescription;
            newSatchelSlot.uniqueIdentifier = i;
            newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
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
            if (inventoryList.itemInventory[itemSlotList[0].uniqueIdentifier].itemAttribute == ItemsDatabase.ItemAttribute.ElementalMindset && enemyManager.wild)
            {
                giveBerryButton.gameObject.SetActive(true);
            }
            else
            {
                giveBerryButton.gameObject.SetActive(false);
            }
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

        if (battleSystem.state != BattleState.NOTBATTLING)
        {
            int onPartyCount = 0;
            for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
            {
                if (inventoryList.couCouInventory[i].onParty)
                {
                    onPartyCount++;
                    SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
                    CouCouDatabase.CouCouData coucou = coucouFinder.FindCouCou(inventoryList.couCouInventory[i].coucouName);
                    newSatchelSlot.itemNameText.text = inventoryList.couCouInventory[i].coucouName;
                    newSatchelSlot.itemAmountText.text = "Lv: " + inventoryList.couCouInventory[i].coucouLevel.ToString();
                    newSatchelSlot.uniqueIdentifier = i;
                    newSatchelSlot.itemDescription = coucou.coucouDescription;
                    newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
                    coucouSlotList.Add(newSatchelSlot);
                }
                if (onPartyCount >= 5)
                {
                    break;
                }
            }
        }
        else
        {
            for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
            {
                SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
                CouCouDatabase.CouCouData newCouCou = coucouFinder.FindCouCou(inventoryList.couCouInventory[i].coucouName);
                newSatchelSlot.itemNameText.text = inventoryList.couCouInventory[i].coucouName;
                newSatchelSlot.itemAmountText.text = "Lv: " + inventoryList.couCouInventory[i].coucouLevel.ToString();
                newSatchelSlot.uniqueIdentifier = i;
                newSatchelSlot.itemDescription = newCouCou.coucouDescription;
                newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
                coucouSlotList.Add(newSatchelSlot);
            }
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

    public void GoBack()
    {
        if (inPrompt)
        {
            submitButton.Select();
            OnSubmitCancelled();
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
            battlingUI.BackToMenuFromSatchel();
        }
    }

    public void OnCloseSatchel()
    {
        selectedSection = 0;

        blurCamera.gameObject.SetActive(false);
        ClearItems();
        ClearCouCou();
        satchel.SetActive(false);
        battlingUI.BackToMenu();
    }

    public void UpdateDescription()
    {
        if (selectedSection == 1)
        {
            descriptionName.text = currentSelectedButton.GetComponent<SatchelSlotController>().itemNameText.text;
            descriptionText.text = currentSelectedButton.GetComponent<SatchelSlotController>().itemDescription;

            if (inventoryList.itemInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].itemAttribute == ItemsDatabase.ItemAttribute.ElementalMindset && enemyManager.wild)
            {
                giveBerryButton.gameObject.SetActive(true);
            }
            else
            {
                giveBerryButton.gameObject.SetActive(false);
            }
        }
        else if (selectedSection == 2)
        {
            descriptionName.text = currentSelectedButton.GetComponent<SatchelSlotController>().itemNameText.text;
            descriptionText.text = currentSelectedButton.GetComponent<SatchelSlotController>().itemDescription;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + inventoryList.couCouInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].currentResistance;
            determinationText.text = "Det: " + inventoryList.couCouInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[currentSelectedButton.GetComponent<SatchelSlotController>().uniqueIdentifier].currentMindset;
            scrollRectEnsureVisible.CenterOnItem(currentSelectedButton.GetComponent<RectTransform>());
        }
    }

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
        battlingUI.OnFinishTurn();
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

    #region - Section Changes -

    public void NavigateSections(float direction)
    {
        Debug.Log(direction);
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
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 2;
        ClearItems();
        DisplayCouCou();
        scrollRect.enabled = true;
        scrollRect.normalizedPosition = new Vector2(0, 1);

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
