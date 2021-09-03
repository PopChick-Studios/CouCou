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
    public GameManager.GameState previousState = GameManager.GameState.Wandering;

    public Animator transition;

    private void Awake()
    {
        inventoryManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>();
        lightingManager = gameObject.GetComponent<LightingManager>();
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (previousState != gameManager.State)
        {
            if (gameManager.State == GameManager.GameState.Battling && SceneManager.GetActiveScene().name != "BattleScene")
            {
                LoadBattleScene();
                Debug.Log("Loading Battle Scene");
            }
            if (gameManager.State == GameManager.GameState.Wandering && SceneManager.GetActiveScene().name != "TestingScene")
            {
                LoadAdventureScene();
                Debug.Log("Loading Adventure Scene");
            }
            previousState = gameManager.State;
        }
        
    }

    public void LoadBattleScene()
    {
        if (inventoryManager.HasPlayableCouCou())
        {
            StartCoroutine(LoadScene("BattleScene"));
        }
    }

    public void LoadAdventureScene()
    {
        StartCoroutine(LoadScene("TestingScene"));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");
        inventoryManager.SaveInventory();
        PlayerPrefs.Save();
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(sceneName);
        lightingManager.Daytime();
    }
}
