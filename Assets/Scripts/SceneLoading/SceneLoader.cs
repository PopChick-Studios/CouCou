using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private GameManager gameManager;
    private LightingManager lightingManager;
    private GameManager.GameState previousState;

    public GameObject findWildCouCou;
    public Animator transition;

    private void Awake()
    {
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
            previousState = gameManager.State;
        }
        
    }

    public void LoadBattleScene()
    {
        StartCoroutine(LoadScene("BattleScene"));
    }

    public IEnumerator LoadScene(string sceneName)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(1f);
        DontDestroyOnLoad(findWildCouCou);
        SceneManager.LoadScene(sceneName);
        lightingManager.Daytime();
    }
}
