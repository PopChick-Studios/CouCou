using TMPro;
using UnityEngine;

[System.Serializable]
public class QuestPage
{
    public string questTitle;

    [Space]
    public int pageNumber;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI contentText;

    [TextArea(3, 20)]
    public string contents;
}
