using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestBook : MonoBehaviour
{
    public Animator Page1;
    public Animator Page2;
    public Animator Page3;
    public Animator Page4;
    public Animator Page5;
    public Animator Page6;
    public Animator Page7;

    public int PageOpen = 0;

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

    // Update is called once per frame
    void Update()
    {
        /*
        if(input.getbuttondown("q"))
        {
            if(gameObject.enabled == true)
            {
            gameObject.SetActive(false);
            }
            else
            {
            gameObject.SetActive(true);
            }
        }
        */

        // Makes sure the page number won't go below 0.
        if(PageOpen <= -1)
        {
            PageOpen = 0;
        }
        // Makes sure the page number won't go above 6.
        if(PageOpen >= 7)
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
}
