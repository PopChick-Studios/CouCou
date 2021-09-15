using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private GameManager gameManager;
    private InventoryManager inventoryManager;
    private LightingManager lightingManager;
    private DialogueManager dialogueManager;
    private Player player;
    public QuestScriptable questScriptable;
    public GameManager.GameState previousState;
    public InventoryList playerInventory;
    public Animator transition;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "TestingScene" || SceneManager.GetActiveScene().name == "CouCou")
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
        else
        {
            player = null;
        }
        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        lightingManager = gameObject.GetComponent<LightingManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (previousState != gameManager.State && SceneManager.GetActiveScene().name != "TitleScreen")
        {
            if (gameManager.State == GameManager.GameState.Battling && SceneManager.GetActiveScene().name != "BattleScene")
            {
                LoadBattleScene();
            }
            if (gameManager.State == GameManager.GameState.Wandering && SceneManager.GetActiveScene().name != "CouCou" && SceneManager.GetActiveScene().name != "TestingScene")
            {
                LoadAdventureScene();
            }
            previousState = gameManager.State;
        }
    }

    public void LoadBattleScene()
    {
        if (inventoryManager.HasPlayableCouCou())
        {
            inventoryManager.SaveInventory();
            SaveSystem.SavePlayer(player, questScriptable);
            StartCoroutine(LoadScene("BattleScene"));
        }
    }

    public void LoadAdventureScene()
    {
        inventoryManager.SaveInventory();
        StartCoroutine(LoadScene("CouCou"));
    }

    public void LoadNewGame()
    {
        SaveSystem.DeleteAllSaveFiles();
        playerInventory.starterCouCou = null;
        playerInventory.couCouInventory.Clear();
        playerInventory.itemInventory.Clear();
        StartCoroutine(LoadScene("CouCou"));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");
        PlayerPrefs.Save();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        lightingManager.Daytime();
        if (player != null)
        {
            SaveSystem.LoadPlayer();
        }
    }

    public IEnumerator TitleScreen()
    {
        transition.SetTrigger("Start");
        PlayerPrefs.Save();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("TitleScreen");
    }
}
