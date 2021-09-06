using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBook : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Contents")]
    public List<QuestPage> pages;

    [Header("Animation")]
    public Animator page1Animator;
    public Animator page2Animator;
    public Animator page3Animator;
    public Animator page4Animator;
    public Animator page5Animator;
    public Animator page6Animator;
    public Animator page7Animator;

    public int PageOpen = 0;

    PlayerInputActions playerInputActions;

    private void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        playerInputActions = new PlayerInputActions();

        playerInputActions.UI.Navigate.started += x => RequestToNavigate(x.ReadValue<Vector2>());
        playerInputActions.UI.Navigate.canceled += x => RequestToNavigate(new Vector2(0, 0));
    }

    // Start is called before the first frame update
    void Start()
    {
        ResetPages();
        foreach (QuestPage page in pages)
        {
            page.titleText.text = page.questTitle;
            page.contentText.text = page.contents;
        }
    }

    public void ResetPages()
    {
        // Set all the pages to closed at the start.
        page1Animator.SetBool("TurnCount1", false);
        page2Animator.SetBool("TurnCount2", false);
        page3Animator.SetBool("TurnCount3", false);
        page4Animator.SetBool("TurnCount4", false);
        page5Animator.SetBool("TurnCount5", false);
        page6Animator.SetBool("TurnCount6", false);
        page7Animator.SetBool("TurnCount7", false);
        PageOpen = 0;
    }

    public void OpenBook()
    {
        playerInputActions.Wandering.Disable();
        playerInputActions.UI.Enable();
        StartCoroutine(PageChecker());
    }

    public void CloseBook()
    {
        ResetPages();
        playerInputActions.Wandering.Enable();
        playerInputActions.UI.Disable();
        StopAllCoroutines();
    }

    // Update is called once per frame
    public IEnumerator PageChecker()
    {
        while (true)
        {
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
                    page1Animator.SetBool("TurnCount1", false);
                    break;

                case 1:
                    page1Animator.SetBool("TurnCount1", true);
                    page2Animator.SetBool("TurnCount2", false);
                    break;

                case 2:
                    page2Animator.SetBool("TurnCount2", true);
                    page3Animator.SetBool("TurnCount3", false);
                    break;

                case 3:
                    page3Animator.SetBool("TurnCount3", true);
                    page4Animator.SetBool("TurnCount4", false);
                    break;

                case 4:
                    page4Animator.SetBool("TurnCount4", true);
                    page5Animator.SetBool("TurnCount5", false);
                    break;

                case 5:
                    page5Animator.SetBool("TurnCount5", true);
                    page6Animator.SetBool("TurnCount6", false);
                    break;

                case 6:
                    page6Animator.SetBool("TurnCount6", true);
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
