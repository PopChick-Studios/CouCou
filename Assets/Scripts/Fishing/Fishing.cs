using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    private CanPlayerFish canPlayerFish;
    private FindWildCouCou findWildCouCou;
    private DisplayManager displayManager;
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    private Transform cameraPosition;

    public GameObject player;

    public LookAtPlayer displayPromptPrefab;
    private LookAtPlayer displayPrompt;

    public bool isFishing;
    public bool pullUp;
    public bool countdownFinished;
    public float timer = 0;

    PlayerInputActions playerInputActions;

    private void Awake()
    {
        gameManager = GetComponent<GameManager>();
        findWildCouCou = GetComponent<FindWildCouCou>();
        displayManager = GetComponent<DisplayManager>();
        inventoryManager = GetComponent<InventoryManager>();
        canPlayerFish = GameObject.FindGameObjectWithTag("Player").GetComponent<CanPlayerFish>();
        cameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Wandering.Interact.started += x => ValidateFishRequest();
        playerInputActions.Fishing.Interact.started += x => ValidateFishRequest();
        playerInputActions.Fishing.Cancel.started += x => ValidateFishRequest();
    }

    public void ValidateFishRequest()
    {
        Debug.Log("validating");
        if (canPlayerFish.playerCanFish && !isFishing && gameManager.State == GameManager.GameState.Wandering)
        {
            playerInputActions.Wandering.Disable();
            playerInputActions.Fishing.Enable();
            StartCoroutine(StartFishing());
        }
        else if (canPlayerFish.playerCanFish && isFishing && gameManager.State == GameManager.GameState.Fishing)
        {
            // pull back animation;
            playerInputActions.Fishing.Disable();
            playerInputActions.Wandering.Enable();
            CancelFish();
        }
    }

    public IEnumerator StartFishing()
    {
        if (displayPrompt == null)
        {
            yield break;
        }
        Debug.Log("Started Fishing");

        gameManager.SetState(GameManager.GameState.Fishing);
        isFishing = true;

        int xNormalValue = 0;
        int zNormalValue = 0;

        if (canPlayerFish.collisionNormal.x == -1 || canPlayerFish.collisionNormal.x == 1)
        {
            xNormalValue = (int)canPlayerFish.collisionNormal.x;
        }
        else if (canPlayerFish.collisionNormal.z == -1 || canPlayerFish.collisionNormal.z == 1)
        {
            zNormalValue = (int)canPlayerFish.collisionNormal.z;
        }

        Debug.Log(xNormalValue);
        Debug.Log(zNormalValue);

        player.transform.SetPositionAndRotation(new Vector3(displayPrompt.transform.position.x + xNormalValue, player.transform.position.y, displayPrompt.transform.position.z + zNormalValue), Quaternion.LookRotation(new Vector3(-xNormalValue, 0, -zNormalValue)));

        Debug.Log("Vector3: " + new Vector3(xNormalValue, player.transform.position.y, zNormalValue));

        DestroyUI();

        // animate fishing

        yield return new WaitForSeconds(2f);

        bool caughtSomething = false;

        while (!caughtSomething)
        {
            float rnd = Random.Range(5, 61);
            Debug.Log("waiting: " + rnd);
            yield return new WaitForSeconds(rnd);

            // tug animation

            pullUp = false;
            countdownFinished = false;
            timer = 3f;
            StartCoroutine(CountdownTimer());
            yield return new WaitUntil(() => countdownFinished);
            if (timer > 0)
            {
                caughtSomething = true;
                break;
            }
        }

        int randomCatch = Random.Range(0, 5);

        Debug.Log("Successful Catch!");

        if (randomCatch < 2)
        {
            CouCouCatch();
        }
        else
        {
            ItemCatch();
        }

        isFishing = false;
    }

    public IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            Debug.Log("timer: " + timer);
            if (pullUp)
            {
                break;
            }
            timer -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        countdownFinished = true;
    }

    public void CancelFish()
    {
        if (timer != 0)
        {
            pullUp = true;
        }
        else
        {
            StopAllCoroutines();
            isFishing = false;
            gameManager.SetState(GameManager.GameState.Wandering);
        }
    }

    public void CouCouCatch()
    {
        Debug.Log("Catching CouCou");
        findWildCouCou.WildCouCouAttack(CouCouDatabase.Element.Aqua);
    }

    public void ItemCatch()
    {
        int randomItem = Random.Range(0, 4);
        if (randomItem < 3 || PlayerPrefs.GetInt("currentCapsules") == PlayerPrefs.GetInt("maxCapsules"))
        {
            displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, "Frozen Berry", 1);
            inventoryManager.FoundItem("Frozen Berry", 1);
            Debug.Log("Frozen Berry obtain");
        }
        else if (randomItem > 3 && PlayerPrefs.GetInt("currentCapsules") != PlayerPrefs.GetInt("maxCapsules"))
        {
            displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, "CouCou Capsule", 1);
            inventoryManager.FoundItem("CouCou Capsule", 1);
            Debug.Log("CouCou Capsule obtain");
        }
    }

    public void InstantiateUI(Vector3 collisionPoint)
    {
        Vector3 spawnPosition = new Vector3(collisionPoint.x, collisionPoint.y + 3, collisionPoint.z);
        displayPrompt = Instantiate(displayPromptPrefab, spawnPosition, Quaternion.identity);
        StartCoroutine(displayPrompt.StartLooking(cameraPosition));
    }

    public void DestroyUI()
    {
        if (displayPrompt != null)
        {
            Destroy(displayPrompt.gameObject);
        }
    }

    private void OnEnable()
    {
        playerInputActions.Wandering.Enable();
    }
    private void OnDisable()
    {
        playerInputActions.Wandering.Disable();
    }
}
