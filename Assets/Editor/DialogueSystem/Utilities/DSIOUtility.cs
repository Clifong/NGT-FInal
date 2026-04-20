using System;
using System.Collections.Generic;
using System.Linq;
using DS.Data.Save.ConditionsNode;
using DS.Enumerations;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using SaintsField;

namespace DS.Utilities
{
    using Data;
    using Data.Save;
    using Elements;
    using ScriptableObjects;
    using Windows;

    public static class DSIOUtility
    {
        private static DSGraphView graphView;

        private static string graphFileName;
        private static string containerFolderPath;

        private static List<DSNode> nodes;
        private static List<DSGroup> groups;

        private static SaintsDictionary<string, DSGroupSO> createdGroups;
        private static SaintsDictionary<string, DSNodeSO> createdNodes;

        private static SaintsDictionary<string, DSGroup> loadedGroups;
        private static SaintsDictionary<string, DSNode> loadedNodes;

        public static void Initialize(DSGraphView dsGraphView, string graphName)
        {
            graphView = dsGraphView;

            graphFileName = graphName;
            containerFolderPath = $"Assets/DialogueSystem/Dialogues/{graphName}";

            nodes = new List<DSNode>();
            groups = new List<DSGroup>();

            createdGroups = new SaintsDictionary<string, DSGroupSO>();
            createdNodes = new SaintsDictionary<string, DSNodeSO>();

            loadedGroups = new SaintsDictionary<string, DSGroup>();
            loadedNodes = new SaintsDictionary<string, DSNode>();
        }

        public static void Save()
        {
            CreateDefaultFolders();

            GetElementsFromGraphView();

            DSGraphSaveDataSO graphData = CreateAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", $"{graphFileName}Graph");

            graphData.Initialize(graphFileName);

            DSDialogueContainerSO dialogueContainer = CreateAsset<DSDialogueContainerSO>(containerFolderPath, graphFileName);

            dialogueContainer.Initialize(graphFileName);

            SaveGroups(graphData, dialogueContainer);
            SaveNodes(graphData, dialogueContainer);

            SaveAsset(graphData);
            SaveAsset(dialogueContainer);
        }

        private static void SaveGroups(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            List<string> groupNames = new List<string>();

            foreach (DSGroup group in groups)
            {
                SaveGroupToGraph(group, graphData);
                SaveGroupToScriptableObject(group, dialogueContainer);

                groupNames.Add(group.title);
            }

            UpdateOldGroups(groupNames, graphData);
        }

        private static void SaveGroupToGraph(DSGroup group, DSGraphSaveDataSO graphData)
        {
            DSGroupSaveData groupData = new DSGroupSaveData()
            {
                ID = group.ID,
                Name = group.title,
                Position = group.GetPosition().position
            };

            graphData.Groups.Add(groupData);
        }

        private static void SaveGroupToScriptableObject(DSGroup group, DSDialogueContainerSO dialogueContainer)
        {
            string groupName = group.title;

            CreateFolder($"{containerFolderPath}/Groups", groupName);
            CreateFolder($"{containerFolderPath}/Groups/{groupName}", "Dialogues");

            DSGroupSO dialogueGroup = CreateAsset<DSGroupSO>($"{containerFolderPath}/Groups/{groupName}", groupName);

            dialogueGroup.Initialize(groupName);

            createdGroups.Add(group.ID, dialogueGroup);

            dialogueContainer.Groups.Add(dialogueGroup, new List<DSDialogueSO>());

            SaveAsset(dialogueGroup);
        }

        private static void UpdateOldGroups(List<string> currentGroupNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupNames != null && graphData.OldGroupNames.Count != 0)
            {
                List<string> groupsToRemove = graphData.OldGroupNames.Except(currentGroupNames).ToList();

                foreach (string groupToRemove in groupsToRemove)
                {
                    RemoveFolder($"{containerFolderPath}/Groups/{groupToRemove}");
                }
            }

            graphData.OldGroupNames = new List<string>(currentGroupNames);
        }

        private static void SaveNodes(DSGraphSaveDataSO graphData, DSDialogueContainerSO dialogueContainer)
        {
            SaintsDictionary<string, List<string>> groupedNodeNames = new SaintsDictionary<string, List<string>>();
            List<string> ungroupedNodeNames = new List<string>();

            foreach (DSNode node in nodes)
            {
                SaveNodeToGraph(node, graphData);
                SaveNodeToScriptableObject(node, dialogueContainer);

                if (node.Group != null)
                {
                    if (!groupedNodeNames.ContainsKey(node.Group.title))
                    {
                        groupedNodeNames[node.Group.title] = new List<string>();
                    }

                    groupedNodeNames[node.Group.title].Add(node.NodeName);
                    
                    continue;
                }

                ungroupedNodeNames.Add(node.NodeName);
            }

            UpdateDialoguesChoicesConnections();

            UpdateOldGroupedNodes(groupedNodeNames, graphData);
            UpdateOldUngroupedNodes(ungroupedNodeNames, graphData);
        }

        private static void SaveNodeToGraph(DSNode graphNode, DSGraphSaveDataSO graphData)
        {
            DSNodeSaveData nodeData = graphNode.GenerateNodeSaveData(CloneNodeChoices(graphNode.Choices));
            graphData.AddNodeSaveData(nodeData);
        }

        private static void SaveNodeToScriptableObject(DSNode node, DSDialogueContainerSO dialogueContainer)
        {
            node.SaveNodeToScriptableObject(containerFolderPath, dialogueContainer, createdNodes, createdGroups);
            // if (node.IsStartingNode())
            // {
            //     dialogueContainer.SetStartingNode(node);
            // }
            //node.SaveNodeToScriptableObject();
        }

        private static void UpdateDialoguesChoicesConnections()
        {
            foreach (DSNode node in nodes)
            {
                DSNodeSO dialogue = createdNodes[node.ID];

                for (int choiceIndex = 0; choiceIndex < node.Choices.Count; ++choiceIndex)
                {
                    DSChoiceSaveData nodeChoice = node.Choices[choiceIndex];

                    if (string.IsNullOrEmpty(nodeChoice.NodeID))
                    {
                        continue;
                    }

                    dialogue.Choices[choiceIndex].NextDialogue = createdNodes[nodeChoice.NodeID];

                    SaveAsset(dialogue);
                }
            }
        }

        private static void UpdateOldGroupedNodes(SaintsDictionary<string, List<string>> currentGroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldGroupedNodeNames != null && graphData.OldGroupedNodeNames.Count != 0)
            {
                foreach (KeyValuePair<string, List<string>> oldGroupedNode in graphData.OldGroupedNodeNames)
                {
                    List<string> nodesToRemove = new List<string>();

                    if (currentGroupedNodeNames.ContainsKey(oldGroupedNode.Key))
                    {
                        nodesToRemove = oldGroupedNode.Value.Except(currentGroupedNodeNames[oldGroupedNode.Key]).ToList();
                    }

                    foreach (string nodeToRemove in nodesToRemove)
                    {
                        RemoveAsset($"{containerFolderPath}/Groups/{oldGroupedNode.Key}/Dialogues", nodeToRemove);
                    }
                }
            }

            foreach (var pair in currentGroupedNodeNames)
            {
                graphData.OldGroupedNodeNames.TryAdd(pair.Key, pair.Value);
            }
        }

        private static void UpdateOldUngroupedNodes(List<string> currentUngroupedNodeNames, DSGraphSaveDataSO graphData)
        {
            if (graphData.OldUngroupedNodeNames != null && graphData.OldUngroupedNodeNames.Count != 0)
            {
                List<string> nodesToRemove = graphData.OldUngroupedNodeNames.Except(currentUngroupedNodeNames).ToList();

                foreach (string nodeToRemove in nodesToRemove)
                {
                    RemoveAsset($"{containerFolderPath}/Global/Dialogues", nodeToRemove);
                }
            }

            graphData.OldUngroupedNodeNames = new List<string>(currentUngroupedNodeNames);
        }

        public static void Load()
        {
            DSGraphSaveDataSO graphData = LoadAsset<DSGraphSaveDataSO>("Assets/Editor/DialogueSystem/Graphs", graphFileName);

            if (graphData == null)
            {
                EditorUtility.DisplayDialog(
                    "Could not find the file!",
                    "The file at the following path could not be found:\n\n" +
                    $"\"Assets/Editor/DialogueSystem/Graphs/{graphFileName}\".\n\n" +
                    "Make sure you chose the right file and it's placed at the folder path mentioned above.",
                    "Thanks!"
                );

                return;
            }

            DSEditorWindow.UpdateFileName(graphData.FileName);

            LoadGroups(graphData.Groups);
            LoadDialogueNodes(graphData.DialogueNodes);
            LoadActionsOnlyNodes(graphData.ActionsOnlyNodes);
            LoadConditionsNodes(graphData.ConditionsOnlyNodes);
            LoadNodesConnections();
        }

        private static void LoadGroups(List<DSGroupSaveData> groups)
        {
            foreach (DSGroupSaveData groupData in groups)
            {
                DSGroup group = graphView.CreateGroup(groupData.Name, groupData.Position);

                group.ID = groupData.ID;

                loadedGroups.Add(group.ID, group);
            }
        }

        private static void LoadDialogueNodes(List<DSMultipleChoiceDialogueNodeSaveData> nodes)
        {
            foreach (DSMultipleChoiceDialogueNodeSaveData data in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(data.Choices);
                
                DSMultipleChoiceNode node = graphView.CreateNode(data.Name, DSDialogueType.MultipleChoice, data.Position, false) as DSMultipleChoiceNode;
                    
                node.Text = data.Text;
                node.Actions = new List<ActionSO>(data.Actions);
                node.LeftSprites = new List<Sprite>(data.LeftSprites);
                node.RightSprites = new List<Sprite>(data.RightSprites);
                node.IsChoiceNode = data.IsChoiceNode;
                
                node.ID = data.ID;
                node.Choices = choices;
                node.RemememberSO = data.RememberSO;
                node.ChoicesSO = data.ChoiceSO;
                node.BackgroundImage = data.BackgroundImage;
                node.IsRememberChoiceNode = data.IsRememberChoiceNode;
                
                node.Draw();
                
                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(data.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[data.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }
        
        private static void LoadActionsOnlyNodes(List<DSActionsOnlyNodeSaveData> nodes)
        {
            foreach (DSActionsOnlyNodeSaveData data in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(data.Choices);
                
                DSActionsOnlyNode node = graphView.CreateMiddleManNode(data.Name, data.Position, false);
                
                node.Actions = new List<ActionSO>(data.Actions);
                
                node.ID = data.ID;
                node.Choices = choices;
                
                node.Draw();
                
                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(data.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[data.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }
        
        private static void LoadConditionsNodes(List<DSConditionsNodeSaveData> nodes)
        {
            foreach (DSConditionsNodeSaveData data in nodes)
            {
                List<DSChoiceSaveData> choices = CloneNodeChoices(data.Choices);
                
                DSConditionsNode node = graphView.CreateConditionsNode(data.Name, data.Position, false);
                
                node.Operators = new List<Operator>(data.Operators);
                
                node.ID = data.ID;
                node.Choices = choices;
                
                node.Draw();
                
                graphView.AddElement(node);

                loadedNodes.Add(node.ID, node);

                if (string.IsNullOrEmpty(data.GroupID))
                {
                    continue;
                }

                DSGroup group = loadedGroups[data.GroupID];

                node.Group = group;

                group.AddElement(node);
            }
        }

        private static void LoadNodesConnections()
        {
            foreach (KeyValuePair<string, DSNode> loadedNode in loadedNodes)
            {
                foreach (Port choicePort in loadedNode.Value.outputContainer.Children())
                {
                    DSChoiceSaveData choiceData = (DSChoiceSaveData) choicePort.userData;

                    if (string.IsNullOrEmpty(choiceData.NodeID))
                    {
                        continue;
                    }

                    DSNode nextNode = loadedNodes[choiceData.NodeID];

                    Port nextNodeInputPort = (Port) nextNode.inputContainer.Children().First();

                    Edge edge = choicePort.ConnectTo(nextNodeInputPort);

                    graphView.AddElement(edge);

                    loadedNode.Value.RefreshPorts();
                }
            }
        }

        private static void CreateDefaultFolders()
        {
            CreateFolder("Assets/Editor/DialogueSystem", "Graphs");

            CreateFolder("Assets", "DialogueSystem");
            CreateFolder("Assets/DialogueSystem", "Dialogues");

            CreateFolder("Assets/DialogueSystem/Dialogues", graphFileName);
            CreateFolder(containerFolderPath, "Global");
            CreateFolder(containerFolderPath, "Groups");
            CreateFolder($"{containerFolderPath}/Global", "Dialogues");
            CreateFolder($"{containerFolderPath}/Global", "Middleman");
            CreateFolder($"{containerFolderPath}/Global", "Conditions");
        }

        private static void GetElementsFromGraphView()
        {
            Type groupType = typeof(DSGroup);

            graphView.graphElements.ForEach(graphElement =>
            {
                if (graphElement is DSNode node)
                {
                    nodes.Add(node);

                    return;
                }

                if (graphElement.GetType() == groupType)
                {
                    DSGroup group = (DSGroup) graphElement;

                    groups.Add(group);

                    return;
                }
            });
        }

        public static void CreateFolder(string parentFolderPath, string newFolderName)
        {
            if (AssetDatabase.IsValidFolder($"{parentFolderPath}/{newFolderName}"))
            {
                return;
            }

            AssetDatabase.CreateFolder(parentFolderPath, newFolderName);
        }

        public static void RemoveFolder(string path)
        {
            FileUtil.DeleteFileOrDirectory($"{path}.meta");
            FileUtil.DeleteFileOrDirectory($"{path}/");
        }

        public static T CreateAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            T asset = LoadAsset<T>(path, assetName);

            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                AssetDatabase.CreateAsset(asset, fullPath);
            }

            return asset;
        }

        public static T LoadAsset<T>(string path, string assetName) where T : ScriptableObject
        {
            string fullPath = $"{path}/{assetName}.asset";

            return AssetDatabase.LoadAssetAtPath<T>(fullPath);
        }

        public static void SaveAsset(UnityEngine.Object asset)
        {
            EditorUtility.SetDirty(asset);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void RemoveAsset(string path, string assetName)
        {
            AssetDatabase.DeleteAsset($"{path}/{assetName}.asset");
        }

        private static List<DSChoiceSaveData> CloneNodeChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSChoiceSaveData> choices = new List<DSChoiceSaveData>();

            foreach (DSChoiceSaveData choice in nodeChoices)
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = choice.Text,
                    NodeID = choice.NodeID
                };

                choices.Add(choiceData);
            }

            return choices;
        }
        
        public static List<DSDialogueChoiceData> ConvertNodeChoicesToDialogueChoices(List<DSChoiceSaveData> nodeChoices)
        {
            List<DSDialogueChoiceData> dialogueChoices = new List<DSDialogueChoiceData>();

            foreach (DSChoiceSaveData nodeChoice in nodeChoices)
            {
                DSDialogueChoiceData choiceData = new DSDialogueChoiceData()
                {
                    Text = nodeChoice.Text
                };

                dialogueChoices.Add(choiceData);
            }

            return dialogueChoices;
        }
    }
}