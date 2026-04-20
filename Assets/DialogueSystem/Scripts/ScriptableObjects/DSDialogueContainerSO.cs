using System.Collections.Generic;
using SaintsField;
using UnityEngine;

namespace DS.ScriptableObjects
{
    public class DSDialogueContainerSO : ScriptableObject
    {
        [field: SerializeField] public string FileName { get; set; }
        [field: SerializeField] public SaintsDictionary<DSGroupSO, List<DSNodeSO>> Groups { get; set; }
        [field: SerializeField] public List<DSNodeSO> UngroupedNodes { get; set; }
        [field: SerializeField] public List<DSNodeSO> StartingNodes { get; set; }
        
        public void AddStartingNode(DSNodeSO startingNode = null)
        {
            StartingNodes.Add(startingNode);
        }

        public DSNodeSO GetStartingNode()
        {
            foreach (var dsNodeSo in StartingNodes)
            {
                if (dsNodeSo is DSConditionsSO conditionsSo)
                {
                    if (!conditionsSo.CheckIfAllConditionsPass()) continue;
                }
                return dsNodeSo;
            }

            //Will not happened
            return null;
        }
        
        public void Initialize(string fileName)
        {
            FileName = fileName;

            Groups = new SaintsDictionary<DSGroupSO, List<DSNodeSO>>();
            UngroupedNodes = new List<DSNodeSO>();
            StartingNodes = new List<DSNodeSO>();
        }

        public List<string> GetGroupNames()
        {
            List<string> dialogueGroupNames = new List<string>();

            foreach (DSGroupSO dialogueGroup in Groups.Keys)
            {
                dialogueGroupNames.Add(dialogueGroup.GroupName);
            }

            return dialogueGroupNames;
        }

        public List<string> GetGroupedDialogueNames(DSGroupSO dialogueGroup, bool startingDialoguesOnly)
        {
            List<DSNodeSO> groupedNodes = Groups[dialogueGroup];

            List<string> groupedNodesNames = new List<string>();

            foreach (DSNodeSO groupedNode in groupedNodes)
            {
                if (startingDialoguesOnly && !groupedNode.IsStartingDialogue)
                {
                    continue;
                }

                groupedNodesNames.Add(groupedNode.NodeName);
            }

            return groupedNodesNames;
        }

        public List<string> GetUngroupedDialogueNames(bool startingDialoguesOnly)
        {
            List<string> ungroupedNodeNames = new List<string>();

            foreach (DSNodeSO ungroupedNode in UngroupedNodes)
            {
                if (startingDialoguesOnly && !ungroupedNode.IsStartingDialogue)
                {
                    continue;
                }

                ungroupedNodeNames.Add(ungroupedNode.NodeName);
            }

            return ungroupedNodeNames;
        }
        
    }
}