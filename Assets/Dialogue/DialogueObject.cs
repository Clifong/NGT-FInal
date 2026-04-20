using DS.Data;
using DS.ScriptableObjects;
using UnityEngine;

[CreateAssetMenu(fileName = "Dialogue", menuName = "Dialogue SO", order = 1)]
public class DialogueObject : ScriptableObject
{
    [SerializeField] private DSDialogueContainerSO _dialogueSo;
    private string _currentText;
    private DSNodeSO _currentDialogue;
    protected bool waitForPlayer;

    public void StartDialogue(DialogueManager dialogueManager)
    {
        waitForPlayer = false;
        _currentDialogue = _dialogueSo.GetStartingNode();
        ContinueDialogue(dialogueManager);
    }

    public void ContinueDialogue(DialogueManager dialogueManager)
    {
        if (_currentDialogue == null)
        {
            dialogueManager.EndDialogue();
            return;
        }
        
        if (waitForPlayer)
        {
            return;
        }
        
        _currentDialogue.ContinueDialogue(this, dialogueManager);
    }

    public void ForceDialogueComplete(DialogueManager dialogueManager)
    {
        dialogueManager.ForceDisplayFullDialogueText(_currentText);
    }
    
    public void GoToNextNode(DSNodeSO nextNode)
    {
        _currentDialogue = nextNode;
    }

    public void SetWaitForPlayer(bool waitForPlayer)
    {
        this.waitForPlayer = waitForPlayer;
    }
    
    public void SetCurrentText(string text)
    {
        _currentText = text;
    }

    public void SelectChoice(int index, DialogueManager dialogueManager)
    {
        DSNodeSO nextnode = _currentDialogue.Choices[index].NextDialogue;
        if (_currentDialogue is DSDialogueMultipleChoiceSO _dialogueSo)
        {
            _dialogueSo.MakeChoice(index);
        }
        _currentDialogue = nextnode;   
        dialogueManager.ResetChoicePanel();
        waitForPlayer = false;
        ContinueDialogue(dialogueManager);
    }
}
