using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    // References to other scripts
    private DisplayManager displayManager;
    private InventoryManager inventoryManager;
    private SceneLoader sceneLoader;
    public QuestScriptable questScriptable;

    public Animator crossfadeAnimator;
    public TextMeshProUGUI endingText;
    [TextArea(3, 10)]
    public string badEndingText;
    [TextArea(3, 10)]
    public string goodEndingText;
    public InventoryList playerInventory;
    public bool typingEnding;
    public bool questRewardFinish;

    // Create the states of the game
    public enum GameState
    {
        TitleScreen,
        Wandering,
        Paused,
        Fishing,
        Interacting,
        Dialogue,
        Battling
    }
    [SerializeField] private GameState gameState;
    public GameState State { get { return gameState; } }

    private void Awake()
    {
        inventoryManager = GetComponent<InventoryManager>();
        displayManager = GetComponent<DisplayManager>();
        sceneLoader = GameObject.FindGameObjectWithTag("SceneManager").GetComponent<SceneLoader>();
    }

    private void Start()
    {
        PlayerData data = SaveSystem.LoadPlayer();
        if(data == null)
        {
            questScriptable.questProgress = 1;
            questScriptable.subquestProgress = 1;
        }
    }

    public void SetState(GameState state)
    {
        gameState = state;
    }

    public IEnumerator CompleteQuest(int questNumber)
    {
        string item = "";
        int amount = 0;
        List<string> berries = new List<string>() { "Burnt Berry", "Frozen Berry", "Fruitful Berry", "Dark Berry", "Bright Berry" };
        switch (questNumber)
        {
            case 1:
                questScriptable.questProgress = 2;
                questScriptable.subquestProgress = 0;
                item = "CouCou Capsule";
                amount = 10 - inventoryManager.GetCurrentAmount(item) - playerInventory.couCouInventory.Count;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                break;

            case 2:
                questScriptable.questProgress = 3;
                questScriptable.subquestProgress = 0;
                item = "Mystic Berry";
                amount = 1;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                break;

            case 3:
                questScriptable.questProgress = 4;
                questScriptable.subquestProgress = 0;
                item = "Mystic Berry";
                amount = 2;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                yield return new WaitForSeconds(0.5f);
                item = "CouCou Capsule";
                amount = 15 - inventoryManager.GetCurrentAmount(item) - playerInventory.couCouInventory.Count;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                break;

            case 4:
                questScriptable.questProgress = 5;
                questScriptable.subquestProgress = 0;
                amount = 5;
                for (int i = 0; i < berries.Count; i++)
                {
                    displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, berries[i], amount);
                    inventoryManager.FoundItem(berries[i], amount);
                    yield return new WaitForSeconds(0.5f);
                }

                item = "CouCou Capsule";
                amount = 26 - inventoryManager.GetCurrentAmount(item) - playerInventory.couCouInventory.Count;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                break;

            case 5:
                questScriptable.questProgress = 6;
                questScriptable.subquestProgress = 0;
                item = "Mystic Berry";
                amount = 2;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                break;
            
            case 6:
                questScriptable.questProgress = 7;
                questScriptable.subquestProgress = 0;
                item = "Mystic Berry";
                amount = 2;
                displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
                inventoryManager.FoundItem(item, amount);
                break;
            
            case 7:
                questScriptable.questProgress = 8;
                questScriptable.subquestProgress = 0;
                GoodEnding();
                break;
        }
        questRewardFinish = true;
    }

    public IEnumerator BadEnding()
    {
        crossfadeAnimator.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        typingEnding = true;
        StartCoroutine(TypeSentence(badEndingText));
        yield return new WaitWhile(() => typingEnding);
        StartCoroutine(sceneLoader.TitleScreen());
    }

    public void GoodEnding()
    {

    }

    public IEnumerator TypeSentence(string sentence)
    {
        endingText.text = "";

        foreach (char letter in sentence.ToCharArray())
        {
            endingText.text += letter;
            yield return new WaitForSeconds(0.04f);
        }

        typingEnding = false;
    }

    public void OnQuit()
    {
        Debug.Log("Application Quit");
        Application.Quit();
    }
}
