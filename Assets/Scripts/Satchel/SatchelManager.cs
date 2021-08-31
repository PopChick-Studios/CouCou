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
    private InventoryManager inventoryManager;
    private AbilityDescriptions abilityDescriptions;

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
    public GameObject prompt;
    public GameObject promptText;
    public GameObject dialogueText;
    public GameObject acceptButton;

    public InventoryList inventoryList;
    public SatchelSlotController satchelSlotPrefab;

    private PlayerInputActions playerInputActions;

    public bool inSubmit;
    public bool inPrompt;
    public bool isStuck;
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

        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        enemyManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<EnemyManager>();
        battleSystem = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleSystem>();
        coucouFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<CouCouFinder>();
        itemFinder = GameObject.FindGameObjectWithTag("GameManager").GetComponent<ItemFinder>();
        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();
        abilityDescriptions = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<AbilityDescriptions>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.NavigateSections.performed += x => NavigateSections(x.ReadValue<float>());
        playerInputActions.UI.OpenElementChart.started += x => OnElementChartToggle();

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

        int onPartyCount = 0;
        for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
        {
            if (i < 6)
            {
                onPartyCount++;
                SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
                CouCouDatabase.CouCouData coucou = coucouFinder.FindCouCou(inventoryList.couCouInventory[i].coucouName);
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
                newSatchelSlot.itemDescription = coucou.coucouDescription;
                newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
                coucouSlotList.Add(newSatchelSlot);
            }
            if (onPartyCount >= 5)
            {
                break;
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
            descriptionText.text = coucouSlotList[0].itemDescription;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + Mathf.Round(inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentResistance * 10000) / 10000;
            determinationText.text = "Det: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].currentMindset;
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.couCouInventory[coucouSlotList[0].uniqueIdentifier].element);

            if (descriptionName.text == battleSystem.player.coucouName)
            {
                submitButton.interactable = false;
            }
            else
            {
                submitButton.interactable = true;
            }
        }
        else
        {
            descriptionText.text = "You don't own any CouCou right now";
            statDisplay.SetActive(false);
            submitButton.interactable = false;
            itemSprite.enabled = false;
            submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
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
        if (selectedSection == 2)
        {
            abilityDescriptions.DisplayAbilityDescriptions(coucouFinder.FindCouCou(descriptionName.text));
            abilitiesDescriptionUI.SetActive(true);
        }
    }

    public void GoBack()
    {
        if ((isStuck && !inPrompt && !inSubmit) || battleSystem.state != BattleState.PLAYERTURN)
        {
            return;
        }
        else if (inPrompt)
        {
            submitButton.Select();
            OnSubmitCancelled();
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
        else if (battlingUI.inSatchel)
        {
            selectedSection = 0;
            blurCamera.gameObject.SetActive(false);
            ClearItems();
            ClearCouCou();
            satchel.SetActive(false);
            battlingUI.OnCloseSatchel();
        }
    }

    public void UpdateDescription()
    {
        SatchelSlotController satchelSlot = currentSelectedButton.GetComponent<SatchelSlotController>();
        if (selectedSection == 1)
        {
            descriptionName.text = satchelSlot.itemNameText.text;
            descriptionText.text = satchelSlot.itemDescription;
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.itemInventory[satchelSlot.uniqueIdentifier].element);
            if (itemSprite.sprite == null)
            {
                itemSprite.enabled = false;
            }
            else
            {
                itemSprite.enabled = true;
            }

            if (inventoryList.itemInventory[satchelSlot.uniqueIdentifier].itemAttribute == ItemsDatabase.ItemAttribute.ElementalMindset && enemyManager.wild)
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
            descriptionName.text = satchelSlot.itemNameText.text;
            descriptionText.text = satchelSlot.itemDescription;
            healthPointsText.text = "HP: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentHealth + "/" + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].maxHealth;
            attackText.text = "Atk: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentAttack;
            resistanceText.text = "Res: " + Mathf.Round(inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentResistance * 10000) / 10000;
            determinationText.text = "Det: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentDetermination;
            mindsetText.text = "Mind: " + inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].currentMindset;
            scrollRectEnsureVisible.CenterOnItem(currentSelectedButton.GetComponent<RectTransform>());
            itemSprite.sprite = coucouFinder.GetElementSprite(inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].element);

            if (descriptionName.text == battleSystem.player.coucouName || inventoryList.couCouInventory[satchelSlot.uniqueIdentifier].hasCollapsed)
            {
                submitButton.interactable = false;
            }
            else
            {
                submitButton.interactable = true;
            }
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
            StartCoroutine(enemyManager.GiveBerry(itemFinder.FindItem(descriptionName.text).element, descriptionName.text));
        }
        selectedSection = 0;
        blurCamera.gameObject.SetActive(false);
        ClearItems();
        ClearCouCou();
        satchel.SetActive(false);
        battlingUI.OnCloseSatchel();
        battlingUI.OnFinishTurn();
    }

    public void OnSubmitCancelled()
    {
        if (selectedSection == 3)
        {
            selectedSection = 1;
        }
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
        if (selectedSection == 1 || isStuck)
        {
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
            return;
        }
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou";
        giveBerryButton.gameObject.SetActive(false);
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
        itemsSection.enabled = false;
        giveBerryButton.gameObject.SetActive(false);
        isStuck = true;
        selectedSection = 2;
        satchel.SetActive(true);
        submitButton.interactable = true;
        blurCamera.gameObject.SetActive(true);
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou";
        DisplayCouCou();
        scrollRect.enabled = true;
        scrollRect.normalizedPosition = new Vector2(0, 1);
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
