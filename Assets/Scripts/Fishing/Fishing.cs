using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fishing : MonoBehaviour
{
    private CanPlayerFish canPlayerFish;
    private PlayerInteraction playerInteraction;
    private FindWildCouCou findWildCouCou;
    private DisplayManager displayManager;
    private InventoryManager inventoryManager;
    private GameManager gameManager;
    private Transform cameraPosition;
    private Player player;

    public GameObject playerGO;
    public Animator playerAnimator;
    public GameObject fishingRod1;
    public GameObject fishingRod2;

    public LookAtPlayer displayPromptPrefab;
    private LookAtPlayer displayPrompt;

    public bool isFishing;
    public bool caughtSomething;
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
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        playerInteraction = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInteraction>();
        cameraPosition = GameObject.FindGameObjectWithTag("MainCamera").transform;

        playerInputActions = new PlayerInputActions();
        playerInputActions.Wandering.Interact.started += x => ValidateFishRequest();
        playerInputActions.Fishing.Interact.started += x => ValidateFishRequest();
        playerInputActions.Fishing.Cancel.started += x => ValidateFishRequest();
    }

    private void Start()
    {
        Debug.Log(player.HasMaxCapsules());
    }

    public void ValidateFishRequest()
    {
        if (canPlayerFish.playerCanFish && !isFishing && gameManager.State == GameManager.GameState.Wandering)
        {
            playerInputActions.Wandering.Disable();
            playerInputActions.Fishing.Enable();
            StartCoroutine(StartFishing());
        }
        else if (canPlayerFish.playerCanFish && isFishing && gameManager.State == GameManager.GameState.Fishing)
        {
            playerInputActions.Fishing.Disable();
            playerInputActions.Wandering.Enable();
            StartCoroutine(CancelFish());
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

        playerGO.transform.SetPositionAndRotation(new Vector3(displayPrompt.transform.position.x + xNormalValue, playerGO.transform.position.y, displayPrompt.transform.position.z + zNormalValue), Quaternion.LookRotation(new Vector3(-xNormalValue, 0, -zNormalValue)));

        fishingRod1.SetActive(true);
        fishingRod2.SetActive(true);

        DestroyUI();
        playerAnimator.ResetTrigger("finishFishing");
        playerAnimator.SetTrigger("startFishing");

        yield return new WaitForSeconds(2f);

        caughtSomething = false;

        while (!caughtSomething)
        {
            float rnd = Random.Range(3, 26);
            Debug.Log("waiting: " + rnd);
            yield return new WaitForSeconds(rnd);

            playerAnimator.SetTrigger("caughtFish");

            pullUp = false;
            countdownFinished = false;
            timer = 1.8f;
            StartCoroutine(CountdownTimer());
            yield return new WaitUntil(() => countdownFinished);
            if (timer > 0)
            {
                caughtSomething = true;
                break;
            }
            playerAnimator.SetTrigger("failedCatch");
        }

        int randomCatch = Random.Range(0, 5);

        Debug.Log("Successful Catch!");
        playerAnimator.SetTrigger("finishFishing");

        if (randomCatch < 2 && inventoryManager.HasPlayableCouCou())
        {
            CouCouCatch();
        }
        else
        {
            ItemCatch();
        }

        playerInputActions.Fishing.Disable();
        playerInputActions.Wandering.Enable();
        yield return new WaitForSeconds(0.7f);
        isFishing = false;
        fishingRod1.SetActive(false);
        fishingRod2.SetActive(false);
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

    public IEnumerator CancelFish()
    {
        if (timer != 0)
        {
            pullUp = true;
        }
        else
        {
            playerAnimator.SetTrigger("finishFishing");
            yield return new WaitForSeconds(0.7f);
            StopAllCoroutines();
            isFishing = false;
            gameManager.SetState(GameManager.GameState.Wandering);
            InstantiateUI();

            fishingRod1.SetActive(false);
            fishingRod2.SetActive(false);
        }
    }

    public void CouCouCatch()
    {
        Debug.Log("Catching CouCou");
        findWildCouCou.WildCouCouAttack(CouCouDatabase.Element.Aqua);
        caughtSomething = false;
    }

    public void ItemCatch()
    {
        int randomItem = Random.Range(0, 4);
        string item = "";
        int amount = 0;
        if (randomItem < 3 || player.HasMaxCapsules())
        {
            item = "Frozen Berry";
            amount = 1;
        }
        else if (randomItem > 3 && !player.HasMaxCapsules())
        {
            item = "CouCou Capsule";
            amount = 1;
        }
        
        displayManager.OnInteraction(DisplayManager.InteractionTypes.Collect, item, amount);
        inventoryManager.FoundItem(item, amount);
        Debug.Log(item + " obtain");
        playerInteraction.ChangeToFishingInput();
    }

    public void InstantiateUI()
    {
        Vector3 spawnPosition = new Vector3(canPlayerFish.collisionPoint.x, canPlayerFish.collisionPoint.y + 3, canPlayerFish.collisionPoint.z);
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
