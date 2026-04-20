using System;
using System.Collections.Generic;
using System.Linq;
using DS.Data.Error;
using DS.ScriptableObjects;
using SaintsField;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Windows;

    public class DSNode : Node
    {
        public string NodeName { get; set; }
        public string ID { get; set; }
        public DSGroup Group { get; set; }
        public List<DSChoiceSaveData> Choices { get; set; }

        protected DSGraphView graphView;
        private Color defaultBackgroundColor;
        
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public virtual void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            ID = Guid.NewGuid().ToString();
            Choices = new List<DSChoiceSaveData>();
            NodeName = nodeName;
            
            SetPosition(new Rect(position, Vector2.zero));

            graphView = dsGraphView;
            defaultBackgroundColor = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");
        }

        public virtual void Draw() {}

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public bool IsStartingNode()
        {
            Port inputPort = (Port) inputContainer.Children().First();

            return !inputPort.connected;
        }

        public void SetErrorStyle(Color color)
        {
            mainContainer.style.backgroundColor = color;
        }

        public void ResetStyle()
        {
            mainContainer.style.backgroundColor = defaultBackgroundColor;
        }

        public virtual DSNodeSaveData GenerateNodeSaveData(List<DSChoiceSaveData> choices)
        {
            Choices = choices;
            return new DSNodeSaveData();
        }
        
        public virtual bool IsValidNode()
        {
            return true;
        }

        public virtual void SaveNodeToScriptableObject(string containerFolderPath,
            DSDialogueContainerSO dialogueContainer, SaintsDictionary<string, DSNodeSO> createdNodes,
            SaintsDictionary<string, DSGroupSO> createdGroups) { }

        public virtual void AddUngroupedNode<T>(SaintsDictionary<string, T> dictionary, DSGraphView graphView) where T : DSNodeErrorData { }

        public virtual void RemoveUngroupedNode<T>(SaintsDictionary<string, T> dictionary, DSGraphView graphView)
            where T : DSNodeErrorData { }
    }
}