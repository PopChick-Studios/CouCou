using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private GameManager gameManager;
    private InventoryManager inventoryManager;
    private LightingManager lightingManager;
    private GameManager.GameState previousState = GameManager.GameState.Wandering;

    public GameObject findWildCouCou;
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
            }
            if (gameManager.State == GameManager.GameState.Wandering && SceneManager.GetActiveScene().name != "TestingScene")
            {
                LoadAdventureScene();
            }
            previousState = gameManager.State;
        }
        
    }

    public void LoadBattleScene()
    {
        StartCoroutine(LoadScene("BattleScene"));
    }

    public void LoadAdventureScene()
    {
        StartCoroutine(LoadScene("TestingScene"));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");
        inventoryManager.SaveInventory();
        yield return new WaitForSeconds(1f);
        DontDestroyOnLoad(findWildCouCou);
        SceneManager.LoadScene(sceneName);
        lightingManager.Daytime();
    }
}
