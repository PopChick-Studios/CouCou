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
    public DisplayModelsInSatchel displayModelsInSatchel;

    [Header("Experience")]
    public Animator levelUpAnimator;
    public Image experienceBar;
    public Image experienceBarCover;
    public TextMeshProUGUI experienceLeft;

    [Header("Inventory")]
    public InventoryList playerInventory;
    public CouCouDatabase coucouDatabase;
    private List<CouCouDatabase.CouCouData> coucouDataList;
    public SatchelSlotControllerAdventure satchelSlotPrefab;

    [Header("Interactable UI")]
    public GameObject satchelList;
    public GameObject abilitiesDescriptionUI;
    public GameObject abilitiesElementChart;
    public GameObject elementChart;

    [Header("Set Up")]
    public ScrollRect scrollRect;
    public Camera blurCamera;
    public GameObject satchel;
    public GameObject changeCouCouOrder;
    public GameObject inputCouCouChange;
    public int previousSelectedButtonUID = 0;
    private int inputNumber;

    private PlayerInputActions playerInputActions;

    [Header("Satchel State")]
    public bool inSubmit;
    public bool inPrompt;
    public bool changingCouCou = false;
    public bool isStuck;
    private bool experienceIncrement;
    private bool finishedHealthIncrement;
    public ItemsDatabase.ItemData itemChosen;
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
        displayModelsInSatchel = FindObjectOfType<DisplayModelsInSatchel>();

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
        experienceLeft.text = "";
        for (int i = 0; i < playerInventory.itemInventory.Count; i++)
        {
            SatchelSlotControllerAdventure newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
            ItemsDatabase.ItemData item = itemFinder.FindItem(playerInventory.itemInventory[i].itemName);
            newSatchelSlot.itemNameText.text = item.itemName;
            newSatchelSlot.itemAmountText.text = playerInventory.itemInventory[i].itemAmount.ToString();
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
            currentSelectedButton = itemSlotList[0].gameObject;
            itemSlotList[0].GetComponent<Button>().Select();
            descriptionName.text = itemSlotList[0].itemNameText.text;
            descriptionText.text = itemSlotList[0].itemDescription;
            itemSprite.sprite = coucouFinder.GetElementSprite(playerInventory.itemInventory[itemSlotList[0].uniqueIdentifier].element);
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
            displayModelsInSatchel.DestroyModelDisplay();
            submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "";
        }
    }

    public void DisplayCouCou()
    {
        statDisplay.SetActive(true);

        scrollRect.enabled = false;
        inventoryManager.SortCouCouInventory();
        scrollRect.enabled = true;

        for (int i = 0; i < playerInventory.couCouInventory.Count; i++)
        {
            SatchelSlotControllerAdventure newSatchelSlot = Instantiate(satchelSlotPrefab, satchelList.transform);
            CouCouDatabase.CouCouData newCouCou = coucouFinder.FindCouCou(playerInventory.couCouInventory[i].coucouName);
            newSatchelSlot.itemNameText.text = playerInventory.couCouInventory[i].coucouName;
            newSatchelSlot.itemAmountText.text = "Lv: " + playerInventory.couCouInventory[i].coucouLevel.ToString();
            newSatchelSlot.uniqueIdentifier = i;
            if (playerInventory.couCouInventory[i].hasCollapsed)
            {
                newSatchelSlot.coucouOrder.text = "Collapsed";
                newSatchelSlot.coucouOrder.color = Color.gray;
            }
            else
            {
                newSatchelSlot.coucouOrder.text = "Position " + playerInventory.couCouInventory[i].lineupOrder;
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
            experienceBarCover.gameObject.SetActive(false);
            currentSelectedButton = coucouSlotList[previousSelectedButtonUID].gameObject;
            coucouSlotList[previousSelectedButtonUID].GetComponent<Button>().Select();
            UpdateDescription();
            previousSelectedButtonUID = 0;
        }
        else
        {
            descriptionText.text = "You don't own any CouCou right now";
            statDisplay.SetActive(false);
            submitButton.interactable = false;
            itemSprite.enabled = false;
            experienceBarCover.gameObject.SetActive(true);
            displayModelsInSatchel.DestroyModelDisplay();
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
        displayManager.PauseMenu();
    }

    public void UpdateDescription()
    {
        displayModelsInSatchel.DestroyModelDisplay();

        Vector3 defaultPosition = new Vector3(1335, 140, 0);
        submitButton.interactable = true;
        itemSprite.enabled = true;
        SatchelSlotControllerAdventure satchelSlot = currentSelectedButton.GetComponent<SatchelSlotControllerAdventure>();
        if (selectedSection == 1)
        {
            experienceBarCover.gameObject.SetActive(true);
            experienceLeft.text = "";

            descriptionName.text = satchelSlot.itemNameText.text;
            displayModelsInSatchel.DisplayItem(satchelSlot.itemNameText.text);
            descriptionText.text = satchelSlot.itemDescription;
            itemSprite.sprite = coucouFinder.GetElementSprite(playerInventory.itemInventory[satchelSlot.uniqueIdentifier].element);
            submitButton.transform.position = defaultPosition;
            ItemsDatabase.ItemData item = itemFinder.FindItem(satchelSlot.itemNameText.text);
            if (item.itemAttribute != ItemsDatabase.ItemAttribute.ElementalMindset && item.itemAttribute != ItemsDatabase.ItemAttribute.Experience && item.itemAttribute != ItemsDatabase.ItemAttribute.Health)
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "This item can't be used right now";
                submitButton.transform.position = new Vector2(defaultPosition.x, defaultPosition.y + 50);
                submitButton.interactable = false;
            }
            else
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use Item";
                submitButton.transform.position = defaultPosition;
                submitButton.interactable = true;
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
            InventoryList.CouCouInventory coucou = playerInventory.couCouInventory[satchelSlot.uniqueIdentifier];

            float maxEXP = Mathf.Pow(10 * coucou.coucouLevel, 2) / 4;

            experienceBarCover.gameObject.SetActive(false);
            experienceLeft.text = "Exp until level up: " + Mathf.RoundToInt(maxEXP - coucou.currentEXP);
            experienceBar.fillAmount = coucou.currentEXP / maxEXP;

            descriptionName.text = satchelSlot.itemNameText.text;
            displayModelsInSatchel.DisplayCouCou(satchelSlot.itemNameText.text);
            descriptionText.text = satchelSlot.itemDescription;
            healthPointsText.text = "HP: " + coucou.currentHealth + "/" + coucou.maxHealth;
            attackText.text = "Atk: " + coucou.currentAttack;
            resistanceText.text = "Res: " + Mathf.Round(coucou.currentResistance * 1000) / 1000;
            determinationText.text = "Det: " + coucou.currentDetermination;
            mindsetText.text = "Mind: " + coucou.currentMindset;
            scrollRectEnsureVisible.CenterOnItem(currentSelectedButton.GetComponent<RectTransform>());
            itemSprite.sprite = coucouFinder.GetElementSprite(coucou.element);

            if (playerInventory.couCouInventory[satchelSlot.uniqueIdentifier].hasCollapsed && !isStuck)
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "You can't change the position of a collasped CouCou";
                submitButton.transform.position = new Vector2(defaultPosition.x, defaultPosition.y + 50);
                submitButton.interactable = false;
            }
            else if (isStuck)
            {
                submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Give " + itemChosen.itemName;
                submitButton.transform.position = defaultPosition;
                switch (itemChosen.itemAttribute)
                {
                    case ItemsDatabase.ItemAttribute.Health:
                        if (coucou.currentHealth == coucou.maxHealth)
                        {
                            submitButton.GetComponentInChildren<TextMeshProUGUI>().text = coucou.coucouName + " is already full HP";
                            submitButton.interactable = false;
                        }
                        else
                        {
                            healthPointsText.text = "HP: " + coucou.currentHealth + " + " + Mathf.Min(coucou.maxHealth - coucou.currentHealth, Mathf.CeilToInt(coucou.maxHealth * 0.3f)) + "/" + coucou.maxHealth;
                        }
                        break;
                    case ItemsDatabase.ItemAttribute.ElementalMindset:
                        if (itemChosen.element == coucou.element)
                        {
                            experienceLeft.text += " - 3000";
                        }
                        else
                        {
                            experienceLeft.text += " - 900";
                        }
                        break;
                    case ItemsDatabase.ItemAttribute.Experience:
                        experienceLeft.text += " - 15000";
                        break;
                }
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

        if (inputNumber > playerInventory.couCouInventory.Count)
        {
            inputNumber = playerInventory.couCouInventory.Count;
        }
        else if (inputNumber < 1)
        {
            inputNumber = 1;
        }

        inputCouCouChange.GetComponent<TMP_InputField>().text = "";
        for (int i = 0; i < playerInventory.couCouInventory.Count; i++)
        {
            if (descriptionName.text == playerInventory.couCouInventory[i].coucouName)
            {
                inventoryManager.MoveCouCou(playerInventory.couCouInventory[i], inputNumber - 1);
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
            itemChosen = itemFinder.FindItem(descriptionName.text);
            OpenCouCouSelect();
        }
        else if (selectedSection == 2 && !isStuck)
        {
            changingCouCou = true;
            changeCouCouOrder.SetActive(true);
        }
        else if (isStuck)
        {
            StartCoroutine(UseItem(descriptionName.text));
        }
    }

    public IEnumerator UseItem(string coucouName)
    {
        playerInputActions.Disable();

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

        switch (itemChosen.itemAttribute)
        {
            case ItemsDatabase.ItemAttribute.Health:
                if (coucou.hasCollapsed)
                {
                    inventoryManager.SortCouCouInventory();
                    coucou.hasCollapsed = false;
                    coucouSlotList[currentSelectedButton.GetComponent<SatchelSlotControllerAdventure>().uniqueIdentifier].coucouOrder.text = "";
                }
                finishedHealthIncrement = false;
                StartCoroutine(IncrementallyIncreaseHealth(coucou.currentHealth + (coucou.maxHealth * 0.3f), coucou));
                yield return new WaitUntil(() => finishedHealthIncrement);
                break;
            case ItemsDatabase.ItemAttribute.ElementalMindset:
                if (itemChosen.element == coucou.element)
                {
                    experienceIncrement = false;
                    StartCoroutine(IncrementallyIncreaseEXP(coucou.currentEXP + 3000, coucou, 10f));
                    yield return new WaitUntil(() => experienceIncrement);
                }
                else
                {
                    experienceIncrement = false;
                    StartCoroutine(IncrementallyIncreaseEXP(coucou.currentEXP + 900, coucou, 5f));
                    yield return new WaitUntil(() => experienceIncrement);
                }
                break;
            case ItemsDatabase.ItemAttribute.Experience:
                experienceIncrement = false;
                StartCoroutine(IncrementallyIncreaseEXP(coucou.currentEXP + 15000, coucou, 20f));
                yield return new WaitUntil(() => experienceIncrement);
                break;
        }
        inventoryManager.UsedItem(itemChosen.itemName);
        playerInputActions.Enable();
        isStuck = false;
        inSubmit = false;
        itemsSection.enabled = true;
        abilitiesDescriptionUI.SetActive(false);
        ClearCouCou();
        selectedSection = 1000;
        previousSelectedButtonUID = currentSelectedButton.GetComponent<SatchelSlotControllerAdventure>().uniqueIdentifier;
        OnCouCouSection();
    }

    public IEnumerator IncrementallyIncreaseEXP(float desiredEXP, InventoryList.CouCouInventory coucou, float speed)
    {
        float increase = coucou.currentEXP;
        float maxEXP = Mathf.Pow(10 * coucou.coucouLevel, 2) / 4;
        Debug.Log(desiredEXP);
        while (increase != desiredEXP && coucou.currentEXP < maxEXP)
        {
            increase = Mathf.MoveTowards(increase, desiredEXP, Time.unscaledDeltaTime * 400f * speed);
            experienceLeft.text = "Exp until level up: " + Mathf.Max(0, Mathf.RoundToInt(maxEXP - increase));
            experienceBar.fillAmount = increase / maxEXP;
            coucou.currentEXP = increase;
            yield return new WaitForSecondsRealtime(0.005f);
        }

        levelUpAnimator.SetBool("hasLeveledUp", false);

        if (coucou.currentEXP >= maxEXP)
        {
            Debug.Log("level up");
            coucou.coucouLevel++;
            experienceBar.fillAmount = 0;
            coucou.currentEXP = 0;
            levelUpAnimator.SetBool("hasLeveledUp", true);
            yield return new WaitForSecondsRealtime(1.5f);
            coucouSlotList[currentSelectedButton.GetComponent<SatchelSlotControllerAdventure>().uniqueIdentifier].itemAmountText.text = coucou.coucouLevel.ToString();
            UpdateCouCouStats(coucou);

            if (coucou.currentEXP < desiredEXP)
            {
                StartCoroutine(IncrementallyIncreaseEXP(desiredEXP - increase, coucou, speed));
            }
        }
        else
        {
            experienceIncrement = true;
        }
        yield break;
    }

    public IEnumerator IncrementallyIncreaseHealth(float desiredHealth, InventoryList.CouCouInventory coucou)
    {
        float increase = coucou.currentHealth;

        while (increase != desiredHealth && coucou.currentHealth < coucou.maxHealth)
        {
            increase = Mathf.MoveTowards(increase, desiredHealth, Time.unscaledDeltaTime * 400f);
            healthPointsText.text = "HP: " + (int)increase + "/" + coucou.maxHealth;
            coucou.currentHealth = (int)increase;
            yield return new WaitForSecondsRealtime(0.05f);
        }
        finishedHealthIncrement = true;
        if (coucou.currentHealth >= coucou.maxHealth)
        {
            coucou.currentHealth = coucou.maxHealth;
        }
        yield break;
    }

    public void UpdateCouCouStats(InventoryList.CouCouInventory coucou)
    {
        int level;
        int bonusStatsPer5;
        int bonusStatsPer1;

        for (int i = 0; i < coucouDatabase.coucouVariant.Count; i++)
        {
            level = coucou.coucouLevel;
            bonusStatsPer5 = Mathf.FloorToInt(level / 5);
            bonusStatsPer1 = level - bonusStatsPer5 - 1;

            if (coucou.coucouVariant == coucouDatabase.coucouVariant[i].variant)
            {
                int previousMaxHealth = coucou.maxHealth;
                coucou.maxHealth = coucouDatabase.coucouVariant[i].hp + (coucouDatabase.coucouVariant[i].bonusHP * bonusStatsPer1) + (coucouDatabase.coucouVariant[i].bonusHPPer5 * bonusStatsPer5);
                coucou.currentAttack = coucouDatabase.coucouVariant[i].attack + (coucouDatabase.coucouVariant[i].bonusAttack * bonusStatsPer1) + (coucouDatabase.coucouVariant[i].bonusAttackPer5 * bonusStatsPer5);
                coucou.currentResistance = coucouDatabase.coucouVariant[i].resistance + (coucouDatabase.coucouVariant[i].bonusResistance * bonusStatsPer1) + (coucouDatabase.coucouVariant[i].bonusResistancePer5 * bonusStatsPer5);
                coucou.currentMindset = coucouDatabase.coucouVariant[i].mindset;
                coucou.currentDetermination = coucouDatabase.coucouVariant[i].determination;
                coucou.currentHealth = Mathf.Min(coucou.maxHealth, coucou.currentHealth + (coucou.maxHealth - previousMaxHealth));
            }
        }
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
        experienceBarCover.gameObject.SetActive(true);
        abilitiesDescriptionUI.SetActive(false);
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Use Item";
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 1;
        scrollRect.normalizedPosition = new Vector2(0, 1);
        ClearCouCou();
        DisplayItems();
    }

    public void OnCouCouSection()
    {
        if (selectedSection == 2)
        {
            coucouSlotList[0].GetComponent<Button>().Select();
            return;
        }
        experienceBarCover.gameObject.SetActive(false);
        submitButton.GetComponentInChildren<TextMeshProUGUI>().text = "Change CouCou Position";
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);
        submitButton.interactable = true;
        selectedSection = 2;
        scrollRect.normalizedPosition = new Vector2(0, 1);
        ClearItems();
        DisplayCouCou();
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