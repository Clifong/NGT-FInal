using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    public class DSConditionsSO : DSNodeSO
    {
        [field: SerializeField] public List<Operator> Operators { get; set; }
        
        public void Initialize(
            string dialogueName,
            List<DSDialogueChoiceData> choices, 
            bool isStartingDialogue, 
            List<Operator> operators
        )
        {
            base.Initialize(dialogueName, choices, isStartingDialogue);
            Operators = operators;
        }

        public bool CheckIfAllConditionsPass()
        {
            foreach (var op in Operators)
            {
                if (!op.GetResult()) return false;
            }

            return true;
        }
        
        public override void ContinueDialogue(DialogueObject dialogueObject, DialogueManager dialogueManager)
        {
            dialogueObject.GoToNextNode(Choices[0].NextDialogue);
            dialogueManager.ForceDisplayFullDialogueText("");
            dialogueObject.ContinueDialogue(dialogueManager);
        }
    }
}