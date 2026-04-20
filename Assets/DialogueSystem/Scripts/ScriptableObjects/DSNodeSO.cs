using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    using Enumerations;
    
    public class DSNodeSO : ScriptableObject
    {
        [field: SerializeField] public string NodeName { get; set; }
        [field: SerializeField] public List<DSDialogueChoiceData> Choices { get; set; }
        [field: SerializeField] public bool IsStartingDialogue { get; set; }
        
        public virtual void Initialize(
            string nodeName,
            List<DSDialogueChoiceData> choices, 
            bool isStartingDialogue
        )
        {
            NodeName = nodeName;
            Choices = choices;
            IsStartingDialogue = isStartingDialogue;
        }

        public virtual void ContinueDialogue(DialogueObject dialogueObject, DialogueManager dialogueManager) {}

    }
}