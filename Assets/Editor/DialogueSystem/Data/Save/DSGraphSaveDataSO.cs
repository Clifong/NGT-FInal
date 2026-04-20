using System.Collections.Generic;
using DS.Data.Save.ConditionsNode;
using SaintsField;
using UnityEngine;
using UnityEngine.Serialization;

namespace DS.Data.Save
{
    public class DSGraphSaveDataSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public List<DSGroupSaveData> Groups { get; set; }
        [field: SerializeField] public List<DSMultipleChoiceDialogueNodeSaveData> DialogueNodes { get; set; }
        [field: SerializeField] public List<DSActionsOnlyNodeSaveData> ActionsOnlyNodes { get; set; }
        [field: SerializeField] public List<DSConditionsNodeSaveData> ConditionsOnlyNodes { get; set; }
        [field: SerializeField] public List<string> OldGroupNames { get; set; }
        [field: SerializeField] public List<string> OldUngroupedNodeNames { get; set; }
        [field: SerializeField] public SaintsDictionary<string, List<string>> OldGroupedNodeNames { get; set; }

        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new List<DSGroupSaveData>();
            DialogueNodes = new List<DSMultipleChoiceDialogueNodeSaveData>();
            ActionsOnlyNodes = new List<DSActionsOnlyNodeSaveData>();
            ConditionsOnlyNodes = new List<DSConditionsNodeSaveData>();
        }

        public void AddNodeSaveData(DSNodeSaveData nodeSaveData)
        {
            if (nodeSaveData is DSMultipleChoiceDialogueNodeSaveData)
            {
                DialogueNodes.Add(nodeSaveData as DSMultipleChoiceDialogueNodeSaveData);
            } else if (nodeSaveData is DSActionsOnlyNodeSaveData)
            {
                ActionsOnlyNodes.Add(nodeSaveData as DSActionsOnlyNodeSaveData);
            } else if (nodeSaveData is DSConditionsNodeSaveData)
            {
                ConditionsOnlyNodes.Add(nodeSaveData as DSConditionsNodeSaveData);
            }
        }
    }
}