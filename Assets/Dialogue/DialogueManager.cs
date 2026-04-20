using System.Collections;
using System.Collections.Generic;
using SaintsField;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.InputSystem;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private DialogueObject currentDialogueObject;
    private LogDialogueManager logDialogueManager;
    
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject backdropPanel;

    [SerializeField] private Transform _leftSpritesSection;
    [SerializeField] private Transform _rightSpritesSection;

    private List<GameObject> _spawnedSprites = new List<GameObject>();
    [SerializeField] private DialogueSprite _dialogueSpritePrefab;
    
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private DialogueChoicePanel choicePanelPrefab;

    [SerializeField] private GameObject dialogueRelatedPanel;
    [SerializeField] private GameObject logPanel;
    
    [SerializeField] private Image dialogueRelatedBackdrop;
    [SerializeField] private GameObject dialogueTextPanel;
    
    private List<GameObject> spawnedChoicePanels = new List<GameObject>();

    [SerializeField] private CrossObjectEventSO endDialogue;

    
    // [SerializeField] private  SaintsDictionary<string, int> allCharactersDialogueData;

    private bool isTextFinished;

    void Start()
    {
        RememberChoiceManager.instance.Initialise();
        logDialogueManager = GetComponent<LogDialogueManager>();
        ResetEverything();
        
        currentDialogueObject.StartDialogue(this);
        backdropPanel.SetActive(true);
    }

    void ResetEverything()
    {
        backdropPanel.SetActive(false);
        choicePanel.SetActive(false);
        logPanel.SetActive(false);
        ResetChoicePanel();
        logDialogueManager.Refresh();
        ResetSprites();
        dialogueText.text = "";
        dialogueRelatedBackdrop.sprite = null;
        dialogueRelatedBackdrop.color = new Color(1, 1, 1, 0);
    }

    private void ResetSprites()
    {
        _spawnedSprites.ForEach(obj => Destroy(obj));
        _spawnedSprites.Clear();
    }

    public void StartDialogue(Component component, object dialogueObj)
    {
        backdropPanel.SetActive(true);
        currentDialogueObject = (DialogueObject)((object[])dialogueObj)[0];
        currentDialogueObject.StartDialogue(this);
    }

    public void ContinueDialogue()
    {
        if (isTextFinished)
        {
            currentDialogueObject.ContinueDialogue(this);
        }
        else
        {
            currentDialogueObject.ForceDialogueComplete(this);
        }
    }

    public void ShowChoice(string text, int index)
    {
        GameObject spawnedPanel = Instantiate(choicePanelPrefab.gameObject, choicePanel.transform);
        spawnedPanel.GetComponent<DialogueChoicePanel>().SetData(text, index, this);
        spawnedChoicePanels.Add(spawnedPanel);
    }

    public void DisplayDialogue(string text, List<Sprite> leftSprites, List<Sprite> rightSprites, Sprite backgroundImage = null)
    {
        if (backgroundImage == null)
        {
            dialogueRelatedBackdrop.sprite = null;
            dialogueRelatedBackdrop.color = new Color(1, 1, 1, 0);
        }
        else
        {
            dialogueRelatedBackdrop.sprite = backgroundImage;
            dialogueRelatedBackdrop.color = Color.white;
        }
        ResetSprites();
        leftSprites.ForEach(sprite =>
        {
            DialogueSprite dialogueSprite = Instantiate(_dialogueSpritePrefab, _leftSpritesSection);
            dialogueSprite.SetDialogueSprite(sprite);
            _spawnedSprites.Add(dialogueSprite.gameObject);
        });
        rightSprites.ForEach(sprite =>
        {
            DialogueSprite dialogueSprite = Instantiate(_dialogueSpritePrefab, _rightSpritesSection);
            dialogueSprite.SetDialogueSprite(sprite);
            _spawnedSprites.Add(dialogueSprite.gameObject);
        });
        logDialogueManager.AddSentence(text);
        StartCoroutine(WriteOutText(text));
    }

    public void ForceDisplayFullDialogueText(string text)
    {
        StopAllCoroutines();
        dialogueText.text = text;
        isTextFinished = true;
    }

    public void DisplayChoicePanel()
    {
        choicePanel.SetActive(true);
    }
    
    public void ResetChoicePanel()
    {
        foreach (GameObject spawnedChoicePanel in spawnedChoicePanels)
        {
            Destroy(spawnedChoicePanel);
        }
        choicePanel.SetActive(false);
    }

    public void EndDialogue()
    {
        ResetEverything();
        endDialogue.TriggerEvent();
    }

    public void SelectChoice(int index)
    {
        currentDialogueObject.SelectChoice(index, this);
        choicePanel.SetActive(false);
    }

    public void HideBackdrop()
    {
        backdropPanel.SetActive(false);
    }
    
    public void ShowBackdrop()
    {
        backdropPanel.SetActive(true);
    }
    
    public void HideDialogue()
    {
        dialogueTextPanel.SetActive(false);
    }
    
    public void ShowDialogue()
    {
        dialogueTextPanel.SetActive(true);
    }

    IEnumerator WriteOutText(string text)
    {
        isTextFinished = false;
        dialogueText.text = "";
        for (int i = 0; i < text.Length; i++)
        {
            dialogueText.text += text[i];
            yield return new WaitForSecondsRealtime(0.05f);
        }
        isTextFinished = true;
    }

    public void ShowLogPanel()
    {
        dialogueRelatedPanel.SetActive(false);
        logPanel.SetActive(true);
        logDialogueManager.SpawnText();
    }

    public void CloseLogPanel()
    {
        dialogueRelatedPanel.SetActive(true);
        logPanel.SetActive(false);
    }
}
