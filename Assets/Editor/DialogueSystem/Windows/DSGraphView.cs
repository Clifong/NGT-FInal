using System;
using System.Collections.Generic;
using SaintsField;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Windows
{
    using Data.Error;
    using Data.Save;
    using Elements;
    using Enumerations;
    using Utilities;

    public class DSGraphView : GraphView
    {
        private DSEditorWindow editorWindow;
        private DSSearchWindow searchWindow;

        private MiniMap miniMap;

        private SaintsDictionary<string, DSErrorDialogueNodeData> ungroupedDialogueNodes;
        private SaintsDictionary<string, DSErrorActionsOnlyNodeData> ungroupedActionsOnlyNodes;
        private SaintsDictionary<string, DSErrorConditionsNodeData> ungroupedConditionsNodes;
        private SaintsDictionary<string, DSGroupErrorData> groups;
        private SaintsDictionary<Group, SaintsDictionary<string, DSNodeErrorData>> groupedNodes;

        private int nameErrorsAmount;

        public int NameErrorsAmount
        {
            get
            {
                return nameErrorsAmount;
            }

            set
            {
                nameErrorsAmount = value;

                if (nameErrorsAmount == 0)
                {
                    editorWindow.EnableSaving();
                }

                if (nameErrorsAmount >= 1)
                {
                    editorWindow.DisableSaving();
                }

                CheckIfNodesAreValid();
            }
        }

        public void CheckIfNodesAreValid()
        {
            foreach (var ele in graphElements)
            {
                if (ele is DSNode node)
                {
                    if (!node.IsValidNode())
                    {
                        editorWindow.DisableSaving();
                        return;
                    }
                }   
            }
            if (nameErrorsAmount >= 1)
            {
                editorWindow.DisableSaving();
            }
            else
            {
                editorWindow.EnableSaving();
            }
        }

        public DSGraphView(DSEditorWindow dsEditorWindow)
        {
            editorWindow = dsEditorWindow;

            ungroupedDialogueNodes = new SaintsDictionary<string, DSErrorDialogueNodeData>();
            ungroupedActionsOnlyNodes = new SaintsDictionary<string, DSErrorActionsOnlyNodeData>();
            ungroupedConditionsNodes = new SaintsDictionary<string, DSErrorConditionsNodeData>();
            groups = new SaintsDictionary<string, DSGroupErrorData>();
            groupedNodes = new SaintsDictionary<Group, SaintsDictionary<string, DSNodeErrorData>>();

            AddManipulators();
            AddGridBackground();
            AddSearchWindow();
            AddMiniMap();

            OnElementsDeleted();
            OnGroupElementsAdded();
            OnGroupElementsRemoved();
            OnGroupRenamed();
            OnGraphViewChanged();

            AddStyles();
            AddMiniMapStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            
            this.AddManipulator(CreateNodeContextualMenu("Add Node (Multiple Choice)", DSDialogueType.MultipleChoice));

            this.AddManipulator(CreateMiddleManNodeContextualMenu());
            
            this.AddManipulator(CreateConditionsNodeContextualMenu());
            
            this.AddManipulator(CreateGroupContextualMenu());
        }
        
        private IManipulator CreateNodeContextualMenu(string actionTitle, DSDialogueType dialogueType)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(menuEvent =>
                menuEvent.menu.AppendAction(actionTitle,
                    actionEvent => AddElement(CreateNode("DialogueName", dialogueType,
                        GetLocalMousePosition(actionEvent.eventInfo.localMousePosition))))
            );
            return contextualMenuManipulator;
        }

        private IManipulator CreateConditionsNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Conditions Node", actionEvent => CreateConditionsNode("ConditionsNode", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        private IManipulator CreateGroupContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Group", actionEvent => CreateGroup("DialogueGroup", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }
        
        private IManipulator CreateMiddleManNodeContextualMenu()
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add Middleman Node", actionEvent => CreateMiddleManNode("MiddlemanNode", GetLocalMousePosition(actionEvent.eventInfo.localMousePosition)))
            );

            return contextualMenuManipulator;
        }

        
        public DSConditionsNode CreateConditionsNode(string nodeName, Vector2 position, bool shouldDraw = true)
        {
            DSConditionsNode node = new DSConditionsNode();

            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();   
            }
            
            AddElement(node);
            
            AddUngroupedNode(node);

            return node;
        }

        public DSActionsOnlyNode CreateMiddleManNode(string nodeName, Vector2 position, bool shouldDraw = true)
        {
            DSActionsOnlyNode node = new DSActionsOnlyNode();

            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();   
            }
            
            AddElement(node);
            
            AddUngroupedNode(node);

            return node;
        }

        public DSGroup CreateGroup(string title, Vector2 position)
        {
            DSGroup group = new DSGroup(title, position);

            AddGroup(group);

            AddElement(group);

            foreach (GraphElement selectedElement in selection)
            {
                if (!(selectedElement is DSNode))
                {
                    continue;
                }

                DSNode node = (DSNode) selectedElement;

                group.AddElement(node);
            }

            return group;
        }

        public DSDialogueNode CreateNode(string nodeName, DSDialogueType dialogueType, Vector2 position, bool shouldDraw = true)
        {
            Type nodeType = Type.GetType($"DS.Elements.DS{dialogueType}Node");

            DSDialogueNode node = (DSDialogueNode) Activator.CreateInstance(nodeType);

            node.Initialize(nodeName, this, position);

            if (shouldDraw)
            {
                node.Draw();
            }

            AddUngroupedNode(node);

            return node;
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type groupType = typeof(DSGroup);
                Type edgeType = typeof(Edge);

                List<DSGroup> groupsToDelete = new List<DSGroup>();
                List<DSNode> nodesToDelete = new List<DSNode>();
                List<Edge> edgesToDelete = new List<Edge>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is DSNode node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge) selectedElement;

                        edgesToDelete.Add(edge);

                        continue;
                    }

                    if (selectedElement.GetType() != groupType)
                    {
                        continue;
                    }

                    DSGroup group = (DSGroup) selectedElement;

                    groupsToDelete.Add(group);
                }

                foreach (DSGroup groupToDelete in groupsToDelete)
                {
                    List<DSNode> groupNodes = new List<DSNode>();

                    foreach (GraphElement groupElement in groupToDelete.containedElements)
                    {
                        if (!(groupElement is DSNode))
                        {
                            continue;
                        }

                        DSNode groupNode = (DSNode) groupElement;

                        groupNodes.Add(groupNode);
                    }

                    groupToDelete.RemoveElements(groupNodes);

                    RemoveGroup(groupToDelete);

                    RemoveElement(groupToDelete);
                }

                DeleteElements(edgesToDelete);

                foreach (DSNode nodeToDelete in nodesToDelete)
                {
                    if (nodeToDelete.Group != null)
                    {
                        nodeToDelete.Group.RemoveElement(nodeToDelete);
                    }

                    RemoveUngroupedNode(nodeToDelete);

                    nodeToDelete.DisconnectAllPorts();

                    RemoveElement(nodeToDelete);
                }
            };
        }

        private void OnGroupElementsAdded()
        {
            elementsAddedToGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup) group;
                    DSNode node = (DSNode) element;

                    RemoveUngroupedNode(node);
                    AddGroupedNode(node, dsGroup);
                }
            };
        }

        private void OnGroupElementsRemoved()
        {
            elementsRemovedFromGroup = (group, elements) =>
            {
                foreach (GraphElement element in elements)
                {
                    if (!(element is DSNode))
                    {
                        continue;
                    }

                    DSGroup dsGroup = (DSGroup) group;
                    DSNode node = (DSNode) element;

                    RemoveGroupedNode(node, dsGroup);
                    AddUngroupedNode(node);
                }
            };
        }

        private void OnGroupRenamed()
        {
            groupTitleChanged = (group, newTitle) =>
            {
                DSGroup dsGroup = (DSGroup) group;

                dsGroup.title = newTitle.RemoveWhitespaces().RemoveSpecialCharacters();

                if (string.IsNullOrEmpty(dsGroup.title))
                {
                    if (!string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        ++NameErrorsAmount;
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(dsGroup.OldTitle))
                    {
                        --NameErrorsAmount;
                    }
                }

                RemoveGroup(dsGroup);

                dsGroup.OldTitle = dsGroup.title;

                AddGroup(dsGroup);
            };
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if (changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        DSNode nextNode = (DSNode) edge.input.node;

                        DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;

                        choiceData.NodeID = nextNode.ID;
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {
                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        Edge edge = (Edge) element;

                        DSChoiceSaveData choiceData = (DSChoiceSaveData) edge.output.userData;

                        choiceData.NodeID = "";
                    }
                }

                return changes;
            };
        }

        public void AddUngroupedNode(DSNode node)
        {
            if (node is DSDialogueNode)
            {
                node.AddUngroupedNode(ungroupedDialogueNodes, this);    
            } else if (node is DSActionsOnlyNode)
            {
                node.AddUngroupedNode(ungroupedActionsOnlyNodes, this);
            } else if (node is DSConditionsNode)
            {
                node.AddUngroupedNode(ungroupedConditionsNodes, this);
            }
        }

        public void RemoveUngroupedNode(DSNode node)
        {
            if (node is DSDialogueNode)
            {
                node.RemoveUngroupedNode(ungroupedDialogueNodes, this);    
            } else if (node is DSActionsOnlyNode)
            {
                node.RemoveUngroupedNode(ungroupedActionsOnlyNodes, this);
            } else if (node is DSConditionsNode)
            {
                node.RemoveUngroupedNode(ungroupedConditionsNodes, this);
            }
        }

        private void AddGroup(DSGroup group)
        {
            string groupName = group.title.ToLower();

            if (!groups.ContainsKey(groupName))
            {
                DSGroupErrorData groupErrorData = new DSGroupErrorData();

                groupErrorData.Groups.Add(group);

                groups.Add(groupName, groupErrorData);

                return;
            }

            List<DSGroup> groupsList = groups[groupName].Groups;

            groupsList.Add(group);

            Color errorColor = groups[groupName].ErrorData.Color;

            group.SetErrorStyle(errorColor);

            if (groupsList.Count == 2)
            {
                ++NameErrorsAmount;

                groupsList[0].SetErrorStyle(errorColor);
            }
        }

        private void RemoveGroup(DSGroup group)
        {
            string oldGroupName = group.OldTitle.ToLower();

            List<DSGroup> groupsList = groups[oldGroupName].Groups;

            groupsList.Remove(group);

            group.ResetStyle();

            if (groupsList.Count == 1)
            {
                --NameErrorsAmount;

                groupsList[0].ResetStyle();

                return;
            }

            if (groupsList.Count == 0)
            {
                groups.Remove(oldGroupName);
            }
        }

        public void AddGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.NodeName.ToLower();

            node.Group = group;

            if (!groupedNodes.ContainsKey(group))
            {
                groupedNodes.Add(group, new SaintsDictionary<string, DSNodeErrorData>());
            }

            if (!groupedNodes[group].ContainsKey(nodeName))
            {
                DSNodeErrorData nodeErrorData = new DSNodeErrorData();

                nodeErrorData.Nodes.Add(node);

                groupedNodes[group].Add(nodeName, nodeErrorData);

                return;
            }

            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Add(node);

            Color errorColor = groupedNodes[group][nodeName].ErrorData.Color;

            node.SetErrorStyle(errorColor);

            if (groupedNodesList.Count == 2)
            {
                ++NameErrorsAmount;

                groupedNodesList[0].SetErrorStyle(errorColor);
            }   
        }

        public void RemoveGroupedNode(DSNode node, DSGroup group)
        {
            string nodeName = node.NodeName.ToLower();

            node.Group = null;

            List<DSNode> groupedNodesList = groupedNodes[group][nodeName].Nodes;

            groupedNodesList.Remove(node);

            node.ResetStyle();

            if (groupedNodesList.Count == 1)
            {
                --NameErrorsAmount;

                groupedNodesList[0].ResetStyle();

                return;
            }

            if (groupedNodesList.Count == 0)
            {
                groupedNodes[group].Remove(nodeName);

                if (groupedNodes[group].Count == 0)
                {
                    groupedNodes.Remove(group);
                }
            }   
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        private void AddSearchWindow()
        {
            if (searchWindow == null)
            {
                searchWindow = ScriptableObject.CreateInstance<DSSearchWindow>();
            }

            searchWindow.Initialize(this);

            nodeCreationRequest = context => SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
        }

        private void AddMiniMap()
        {
            miniMap = new MiniMap()
            {
                anchored = true
            };

            miniMap.SetPosition(new Rect(15, 50, 200, 180));

            Add(miniMap);

            miniMap.visible = false;
        }

        private void AddStyles()
        {
            this.AddStyleSheets(
                "DialogueSystem/DSGraphViewStyles.uss",
                "DialogueSystem/DSNodeStyles.uss"
            );
        }

        private void AddMiniMapStyles()
        {
            StyleColor backgroundColor = new StyleColor(new Color32(29, 29, 30, 255));
            StyleColor borderColor = new StyleColor(new Color32(51, 51, 51, 255));

            miniMap.style.backgroundColor = backgroundColor;
            miniMap.style.borderTopColor = borderColor;
            miniMap.style.borderRightColor = borderColor;
            miniMap.style.borderBottomColor = borderColor;
            miniMap.style.borderLeftColor = borderColor;
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public void ClearGraph()
        {
            graphElements.ForEach(graphElement => RemoveElement(graphElement));

            groups.Clear();
            groupedNodes.Clear();
            ungroupedDialogueNodes.Clear();
            ungroupedActionsOnlyNodes.Clear();
            ungroupedConditionsNodes.Clear();
            
            NameErrorsAmount = 0;
        }

        public void ToggleMiniMap()
        {
            miniMap.visible = !miniMap.visible;
        }
    }
}