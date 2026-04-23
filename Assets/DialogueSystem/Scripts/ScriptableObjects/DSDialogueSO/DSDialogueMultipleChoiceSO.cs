using UnityEngine;
using System.Collections.Generic;

namespace DS.ScriptableObjects
{
    using Data;
    public class DSDialogueMultipleChoiceSO : DSDialogueSO
    {
        [field: SerializeField] public bool IsChoiceNode { get; set; }
        [field: SerializeField] public bool IsRememberChoiceNode { get; set; }
        [field: SerializeField] public RememberDialogueChoicesSO RememberChoiceDataSO { get; set; }
        [field: SerializeField] public List<ChoiceSO> ChoicesSO { get; set; } 
        [field: SerializeField] public Sprite BackgroundImage { get; set; }
        private bool isChoiceNodeAndClickNext = false;
        
        
        public void Initialize(
            string dialogueName,
            string text,
            List<DSDialogueChoiceData> choices,
            bool isStartingDialogue,
            List<ActionSO> actions,
            List<Sprite> leftSprites,
            List<Sprite> rightSprites,
            bool isChoiceNode,
            RememberDialogueChoicesSO rememberChoiceDataSO,
            List<ChoiceSO> choicesSO,
            Sprite backgroundImage,
            bool isRemeberChoiceNode
        )
        {
            base.Initialize(dialogueName, choices, isStartingDialogue);
            Text = text;
            Actions = actions;
            LeftSprites = leftSprites;
            RightSprites = rightSprites;
            IsChoiceNode = isChoiceNode;
            RememberChoiceDataSO = rememberChoiceDataSO;
            ChoicesSO = choicesSO;
            BackgroundImage = backgroundImage;
            IsRememberChoiceNode = isRemeberChoiceNode;
        }

        public void DisplayChoices(DialogueManager dialogueManager)
        {
            for (int i = 0; i < Choices.Count; ++i)
            {
                DSDialogueChoiceData choice = Choices[i];
                DSNodeSO nextNode = choice.NextDialogue;
                if (nextNode is DSConditionsSO conditionsSo)
                {
                    if (!conditionsSo.CheckIfAllConditionsPass())
                    {
                        continue;
                    }
                }
                dialogueManager.ShowChoice(choice.Text, i);
            }
        }
        
        public void DisplayDialogue(DialogueManager dialogueManager)
        {
            dialogueManager.DisplayDialogue(Text, LeftSprites, RightSprites, BackgroundImage);
            Actions.ForEach(action => action.InvokeAction());
        }

        public void MakeChoice(int i)
        {
            if (IsRememberChoiceNode)
            {
                RememberChoiceManager.instance.RememberChoice(RememberChoiceDataSO, ChoicesSO[i]);
            }
        }

        public override void ContinueDialogue(DialogueObject dialogueObject, DialogueManager dialogueManager)
        {
            if (isChoiceNodeAndClickNext)
            {
                dialogueObject.SetWaitForPlayer(true);
                dialogueManager.DisplayChoicePanel();
                DisplayChoices(dialogueManager);
                isChoiceNodeAndClickNext = false;
                return;
            }
            
            dialogueObject.SetCurrentText(Text);
            DisplayDialogue(dialogueManager);
            
            if (IsChoiceNode)
            {
                isChoiceNodeAndClickNext = true;
                return;
            }
            
            dialogueObject.GoToNextNode(Choices[0].NextDialogue);
        }
    }
}