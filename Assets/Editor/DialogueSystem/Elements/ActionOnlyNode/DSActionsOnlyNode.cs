using System.Collections.Generic;
using System.Linq;
using SaintsField;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Error;
    using DSListview;
    using ScriptableObjects;
    using Utilities;
    using Windows;
    using Data.Save;
    
    public class DSActionsOnlyNode : DSNode
    {
        public List<ActionSO> Actions { get; set; }
        private Color defaultBackgroundColor;
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);
            Actions = new List<ActionSO>();
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);
            NodeName = nodeName;
            
            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            /* TITLE CONTAINER */
            
            TextField actionNameTextField = DSElementUtility.CreateTextField(NodeName, null, callback =>
            {
                TextField target = (TextField) callback.target;
            
                target.value = callback.newValue.RemoveWhitespaces().RemoveSpecialCharacters();
            
                if (string.IsNullOrEmpty(target.value))
                {
                    if (!string.IsNullOrEmpty(NodeName))
                    {
                        ++graphView.NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(NodeName))
                    {
                        --graphView.NameErrorsAmount;
                    }
                }
            
                if (Group == null)
                {
                    graphView.RemoveUngroupedNode(this);
            
                    NodeName = target.value;
            
                    graphView.AddUngroupedNode(this);
            
                    return;
                }
            
                DSGroup currentGroup = Group;
            
                graphView.RemoveGroupedNode(this, Group);
            
                NodeName = target.value;
            
                graphView.AddGroupedNode(this, currentGroup);
            });
            
            actionNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, actionNameTextField);

            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);
            
            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort(choice.Text);

                choicePort.userData = choice;

                outputContainer.Add(choicePort);
            }
            
            /* EXTENSION CONTAINER */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");
            
            Foldout actionsToInvokeFoldout = DSElementUtility.CreateFoldout("Actions to run during dialogue");

            VisualElement actionsListView = CreateActionList();
            
            actionsToInvokeFoldout.Add(actionsListView);
            
            customDataContainer.Add(actionsToInvokeFoldout);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }
        
        private VisualElement CreateActionList()
        {
            ListView listView = new ListView();
            SaintsDictionary<VisualElement, int> listViewMap =  new SaintsDictionary<VisualElement, int>();
            SaintsDictionary<int, DSObjectField> fieldMapping = new  SaintsDictionary<int, DSObjectField>();
            DSListViewController dsListViewController = new DSListViewController();
            listView.SetViewController(dsListViewController);
            listView.bindItem = (e, i) =>
            {
                listViewMap[e] = i;

                DSObjectField objectField = e.Children().ToArray()[1] as DSObjectField;
                fieldMapping[i] = objectField;
                
                if (Actions[i] != null)
                {
                    objectField.value = Actions[i];
                }
                else
                {
                    Actions[i] = objectField.value as ActionSO;
                }

                graphView.CheckIfNodesAreValid();
            };
            listView.onAdd = view =>
            {
                view.itemsSource.Add(null);
                view.RefreshItems();
            };
            listView.showAddRemoveFooter =  true;
            listView.onRemove = view =>
            {
                fieldMapping[Actions.Count - 1].Reset();
                fieldMapping.Remove(Actions.Count - 1);
                view.itemsSource.RemoveAt(Actions.Count - 1);
                view.RefreshItems();
                graphView.CheckIfNodesAreValid();
            };
            
            listView.itemsSource = Actions;
            
            listView.makeItem = () =>
            {
                VisualElement row =  new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                DSObjectField dsObjectField = new DSObjectField("", typeof(ActionSO));
                dsObjectField.RegisterValueChangedCallback(evl =>
                {
                    Actions[listViewMap[row]] = evl.newValue as ActionSO;
                    listView.RefreshItems();
                    graphView.CheckIfNodesAreValid();
                });
                
                Button button = new Button();
                button.text = "-";
                button.clicked += () =>
                {
                    foreach (var field in fieldMapping.Values)
                    {
                        field.Reset();
                    }
                    int i = listViewMap[row];
                    Actions.RemoveAt(i);
                    listView.RefreshItems();
                };
                
                row.Add(button);
                row.Add(dsObjectField);
                
                return row;
            };
            
            listView.AddClasses(
                "ds-node__list-view"
                );
            return listView;
        }
        
        public override DSNodeSaveData GenerateNodeSaveData(List<DSChoiceSaveData> choices)
        {
            base.GenerateNodeSaveData(choices);
            return new DSActionsOnlyNodeSaveData()
            {
                ID = ID,
                Name = NodeName,
                Choices = Choices,
                GroupID = Group?.ID,
                Position = GetPosition().position,
                Actions = Actions,
            };
        }
        
        public override void SaveNodeToScriptableObject(string containerFolderPath, DSDialogueContainerSO dialogueContainer, SaintsDictionary<string, DSNodeSO> createdNodes, SaintsDictionary<string, DSGroupSO> createdGroups)
        {
            DSActionsOnlySO actionsOnlySo;
            
            if (Group != null)
            {
                actionsOnlySo = DSIOUtility.CreateAsset<DSActionsOnlySO>($"{containerFolderPath}/Groups/{Group.title}/Middleman", NodeName);

                if (!dialogueContainer.Groups.ContainsKey(createdGroups[Group.ID]))
                {
                    dialogueContainer.Groups[createdGroups[Group.ID]] = new List<DSNodeSO>();
                }
                dialogueContainer.Groups[createdGroups[Group.ID]].Add(actionsOnlySo);
            }
            else
            {
                actionsOnlySo = DSIOUtility.CreateAsset<DSActionsOnlySO>($"{containerFolderPath}/Global/Middleman", NodeName);

                dialogueContainer.UngroupedNodes.Add(actionsOnlySo);
            }
            
            actionsOnlySo.Initialize(
                NodeName,
                DSIOUtility.ConvertNodeChoicesToDialogueChoices(Choices),
                IsStartingNode(),
                Actions
            );   
            
            if (IsStartingNode())
            {
                dialogueContainer.AddStartingNode(actionsOnlySo);
            }
            
            createdNodes.Add(ID, actionsOnlySo);
            DSIOUtility.SaveAsset(actionsOnlySo); 
        }
        
        public override bool IsValidNode()
        {
            foreach (var i in Actions)
            {
                if (i == null) return false;
            }

            return true;
        }
        
        public override void AddUngroupedNode<T>(SaintsDictionary<string, T> dictionary, DSGraphView graphView)
        {
            if (!dictionary.ContainsKey(NodeName))
            {
                DSErrorActionsOnlyNodeData nodeErrorData = new DSErrorActionsOnlyNodeData();

                nodeErrorData.Nodes.Add(this);

                (dictionary as SaintsDictionary<string, DSErrorActionsOnlyNodeData>).Add(NodeName, nodeErrorData);

                return;
            }

            List<DSNode> ungroupedNodesList = dictionary[NodeName].Nodes;
            
            ungroupedNodesList.Add(this);
            
            Color errorColor = dictionary[NodeName].ErrorData.Color;

            SetErrorStyle(errorColor);

            if (ungroupedNodesList.Count == 2)
            {
                ++graphView.NameErrorsAmount;

                ungroupedNodesList[0].SetErrorStyle(errorColor);
            }  
        }
        
        public override void RemoveUngroupedNode<T>(SaintsDictionary<string, T> dictionary, DSGraphView graphView)
        {
            List<DSNode> ungroupedNodesList = dictionary[NodeName].Nodes;

            ungroupedNodesList.Remove(this);

            ResetStyle();

            if (ungroupedNodesList.Count == 1)
            {
                --graphView.NameErrorsAmount;

                ungroupedNodesList[0].ResetStyle();

                return;
            }
            
            if (ungroupedNodesList.Count == 0)
            {
                (dictionary as SaintsDictionary<string, DSErrorActionsOnlyNodeData>).Remove(NodeName);
            }
        }
    }
    
}