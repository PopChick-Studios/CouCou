using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBook : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject questBook;

    public Animator Page1;
    public Animator Page2;
    public Animator Page3;
    public Animator Page4;
    public Animator Page5;
    public Animator Page6;
    public Animator Page7;

    public int PageOpen = 0;

    PlayerInputActions playerInputActions;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        playerInputActions = new PlayerInputActions();

        playerInputActions.Wandering.QuestBook.started += x => OpenAndCloseBook();
        playerInputActions.UI.Cancel.started += x => OpenAndCloseBook();
        playerInputActions.UI.Navigate.started += x => RequestToNavigate(x.ReadValue<Vector2>());
        playerInputActions.UI.Navigate.canceled += x => RequestToNavigate(new Vector2(0, 0));

        questBook = gameObject.transform.GetChild(0).gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {


        // Set all the pages to closed at the start.
        Page1.SetBool("TurnCount1", false);
        Page2.SetBool("TurnCount2", false);
        Page3.SetBool("TurnCount3", false);
        Page4.SetBool("TurnCount4", false);
        Page5.SetBool("TurnCount5", false);
        Page6.SetBool("TurnCount6", false);
        Page7.SetBool("TurnCount7", false);
    }

    public void OpenAndCloseBook()
    {
        Debug.Log("Open and Close");
        if (questBook.activeInHierarchy && gameManager.State == GameManager.GameState.Wandering)
        {
            Debug.Log("Deactivating");
            questBook.SetActive(false);
            playerInputActions.Wandering.Enable();
            playerInputActions.UI.Disable();
            StopAllCoroutines();
        }
        else if (!questBook.activeInHierarchy && gameManager.State == GameManager.GameState.Wandering)
        {
            Debug.Log("Activating");
            questBook.SetActive(true);
            playerInputActions.Wandering.Disable();
            playerInputActions.UI.Enable();
            StartCoroutine(PageChecker());
        }
    }

    // Update is called once per frame
    public IEnumerator PageChecker()
    {
        Debug.Log("Coroutine started");
        while (questBook.activeInHierarchy)
        {
            Debug.Log("While loop fun");
            // Makes sure the page number won't go below 0.
            if (PageOpen <= -1)
            {
                PageOpen = 0;
            }
            // Makes sure the page number won't go above 6.
            if (PageOpen >= 7)
            {
                PageOpen = 6;
            }

            // Open certain pages depending on the PageOpen number.
            switch (PageOpen)
            {
                case 0:
                    Page1.SetBool("TurnCount1", false);
                    break;

                case 1:
                    Page1.SetBool("TurnCount1", true);
                    Page2.SetBool("TurnCount2", false);
                    break;

                case 2:
                    Page2.SetBool("TurnCount2", true);
                    Page3.SetBool("TurnCount3", false);
                    break;

                case 3:
                    Page3.SetBool("TurnCount3", true);
                    Page4.SetBool("TurnCount4", false);
                    break;

                case 4:
                    Page4.SetBool("TurnCount4", true);
                    Page5.SetBool("TurnCount5", false);
                    break;

                case 5:
                    Page5.SetBool("TurnCount5", true);
                    Page6.SetBool("TurnCount6", false);
                    break;

                case 6:
                    Page6.SetBool("TurnCount6", true);
                    break;
            }

            yield return new WaitForEndOfFrame();
        }
        
    }

    public void RequestToNavigate(Vector2 input)
    {
        if (Mathf.Abs(input.x) > 0.5)
        {
            if (input.x > 0) // Going Right
            {
                PreviousPage();
            }
            else if (input.x < 0) // Going Left
            {
                NextPage();
            }
        }
    }

    // Increase PageOpen number.
    public void NextPage()
    {
        PageOpen++;
    }

    // Decrease PageOpen number.
    public void PreviousPage()
    {
        PageOpen--;
    }

    #region - Enable/Disable -

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
