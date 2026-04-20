using UnityEngine;
using TMPro;

public class DialogueChoicePanel : MonoBehaviour
{
    public TextMeshProUGUI text;
    private int index;
    private DialogueManager dialogueManager;

    public void SetData(string text, int index, DialogueManager dialogueManager)
    {
        this.dialogueManager = dialogueManager;
        this.index = index;
        this.text.text = text;
    }

    public void SelectChoice()
    {
        dialogueManager.SelectChoice(index);
    }
}
