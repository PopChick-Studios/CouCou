using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public class SatchelManager : MonoBehaviour
{
    private BattleManager battleManager;
    private BattlingUI battlingUI;
    private ScrollRectEnsureVisible scrollRectEnsureVisible;
    
    public CouCouDatabase coucouDatabase;
    private List<CouCouDatabase.CouCouData> coucouDataList;

    public ScrollRect scrollRect;
    public Camera blurCamera;
    public GameObject satchel;

    public InventoryList inventoryList;
    public SatchelSlotController satchelSlotPrefab;

    private InputSystemUIInputModule inputSystemUIInputModule;
    private PlayerInputActions playerInputActions;

    private bool inSubmit;
    private Button buttonClicked;
    private GameObject currentSelected;

    [Header("Sections")]
    public Button itemsSection;
    public Button coucouSection;
    public int selectedSection;

    [Header("Description")]
    public TextMeshProUGUI descriptionName;
    public TextMeshProUGUI descriptionText;
    public Image itemSprite;
    public Button submitButton;

    public List<SatchelSlotController> itemSlotList;
    public List<SatchelSlotController> coucouSlotList;

    private void Awake()
    {
        coucouDataList = new List<CouCouDatabase.CouCouData>();

        scrollRectEnsureVisible = gameObject.GetComponentInParent<ScrollRectEnsureVisible>();

        battleManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<BattleManager>();
        inputSystemUIInputModule = GameObject.Find("EventSystem").GetComponent<InputSystemUIInputModule>();
        battlingUI = GameObject.FindGameObjectWithTag("BattlingUI").GetComponent<BattlingUI>();

        playerInputActions = new PlayerInputActions();
        playerInputActions.UI.Navigate.performed += x => StartNavigating();
        playerInputActions.UI.Navigate.canceled += x => StopNavigating();
        playerInputActions.UI.Cancel.started += x => GoBack();

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
        for(int i = 0; i < inventoryList.itemInventory.Count; i++)
        {
            SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, transform);
            newSatchelSlot.itemNameText.text = inventoryList.itemInventory[i].itemName;
            newSatchelSlot.itemAmountText.text = inventoryList.itemInventory[i].itemAmount.ToString();
            newSatchelSlot.uniqueIdentifier = i;
            newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
            itemSlotList.Add(newSatchelSlot);
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
        EventSystem.current.SetSelectedGameObject(itemSlotList[0].gameObject);
        descriptionName.text = itemSlotList[0].itemNameText.text;
    }

    public void DisplayCouCou()
    {
        for (int i = 0; i < inventoryList.couCouInventory.Count; i++)
        {
            SatchelSlotController newSatchelSlot = Instantiate(satchelSlotPrefab, transform);
            newSatchelSlot.itemNameText.text = inventoryList.couCouInventory[i].coucouName;
            newSatchelSlot.itemAmountText.text = "Lv: " + inventoryList.couCouInventory[i].coucouLevel.ToString();
            newSatchelSlot.uniqueIdentifier = i;
            newSatchelSlot.GetComponent<Button>().onClick.AddListener(delegate { GoToSubmit(newSatchelSlot.GetComponent<Button>()); });
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
        EventSystem.current.SetSelectedGameObject(coucouSlotList[0].gameObject);
        descriptionName.text = coucouSlotList[0].itemNameText.text;
    }

    public void StartNavigating()
    {
        if (selectedSection == 2)
        {
            StartCoroutine(NavigateItemUI(true));
        }
    }

    public void StopNavigating()
    {
        StopAllCoroutines();
    }

    public void GoToSubmit(Button button)
    {
        Debug.Log("In Submit");

        inSubmit = true;
        buttonClicked = button;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(submitButton.gameObject);
    }

    public void GoBack()
    {
        if (inSubmit)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(buttonClicked.gameObject);
            inSubmit = false;
            buttonClicked = null;
        }
        else
        {
            OnCloseSatchel();
        }
    }

    public void ClearItems()
    {
        descriptionName.text = "";
        descriptionText.text = "";

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

        coucouSlotList.Clear();

        foreach (Transform child in gameObject.transform)
        {
            Destroy(child.gameObject);
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

    public IEnumerator NavigateItemUI(bool navigating)
    {
        currentSelected = EventSystem.current.currentSelectedGameObject;
        scrollRectEnsureVisible.CenterOnItem(currentSelected.GetComponent<RectTransform>());
        yield return new WaitForSeconds(inputSystemUIInputModule.moveRepeatDelay);

        while (navigating)
        {
            Debug.Log(currentSelected.GetComponent<SatchelSlotController>().uniqueIdentifier + " " + (coucouSlotList.Count - 1));
            currentSelected = EventSystem.current.currentSelectedGameObject;
            scrollRectEnsureVisible.CenterOnItem(currentSelected.GetComponent<RectTransform>());

            if (currentSelected.GetComponent<SatchelSlotController>().uniqueIdentifier == coucouSlotList.Count - 1 || currentSelected.GetComponent<SatchelSlotController>().uniqueIdentifier == 0)
            {
                StopAllCoroutines();
                yield return null;
            }

            yield return new WaitForSeconds(inputSystemUIInputModule.moveRepeatRate);
        }
    }

    public void OnSubmitPressed()
    {
        if (selectedSection == 1)
        {
            battleManager.UseItem(descriptionName.text);
        }
        else if (selectedSection == 2)
        {
            battleManager.ChangeCouCou(descriptionName.text);
        }
    }

    #region - Section Changes -

    public void OnItemSection()
    {
        blurCamera.gameObject.SetActive(true);
        satchel.SetActive(true);

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

        selectedSection = 2;
        ClearItems();
        DisplayCouCou();
        scrollRect.enabled = true;

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
