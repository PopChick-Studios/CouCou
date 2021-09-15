using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class Letter
{
    public string title;
    public int questNumber;
    public int subquestNumber;
    [TextArea(6, 20)]
    public string contents;
}

public class LetterManager : MonoBehaviour
{
    public TextMeshProUGUI letterTitle;
    public TextMeshProUGUI letterText;
    public List<Letter> letterContents;

    public void DisplayQuestLetter(int quest, int subquest)
    {
        foreach (Letter letter in letterContents)
        {
            if (letter.questNumber == quest && letter.subquestNumber == subquest)
            {
                letterTitle.text = letter.title;
                letterText.text = letter.contents;
            }
        }
    }
}
