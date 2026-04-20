using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    using Data;
    public class DSActionsOnlySO : DSNodeSO
    {
        [field: SerializeField] public List<ActionSO> Actions { get; set; }
        private int _counter;
        private DialogueObject _dialogueObject;
        private DialogueManager _manager;
        
        public void Initialize(
            string dialogueName,
            List<DSDialogueChoiceData> choices, 
            bool isStartingDialogue, 
            List<ActionSO> actions
        )
        {
            base.Initialize(dialogueName, choices, isStartingDialogue);
            Actions = actions;
        }

        public override void ContinueDialogue(DialogueObject dialogueObject, DialogueManager dialogueManager)
        {
            _counter = 0;
            _dialogueObject = dialogueObject;
            _manager =  dialogueManager;
            _manager.HideDialogue();
            if (Actions.Count == 0)
            {
                _manager.ShowDialogue();
                _dialogueObject.GoToNextNode(Choices[0].NextDialogue);
                _dialogueObject.ContinueDialogue(_manager);
            }
            else
            {
                Actions.ForEach(action => action.InvokeAction(() => RegisterActionAsCompleted()));
            }
        }

        public void RegisterActionAsCompleted()
        {
            _counter++;
            if (_counter == Actions.Count)
            {
                _manager.ShowDialogue();
                _dialogueObject.GoToNextNode(Choices[0].NextDialogue);
                _dialogueObject.ContinueDialogue(_manager);
            }
        }
    }
}