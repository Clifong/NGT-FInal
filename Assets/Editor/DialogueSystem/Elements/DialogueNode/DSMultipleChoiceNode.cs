using System.Collections.Generic;
using System.Linq;
using DS.ScriptableObjects;
using SaintsField;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSMultipleChoiceNode : DSDialogueNode
    {
        public bool IsChoiceNode { get; set; }
        public bool IsRememberChoiceNode { get; set; }
        public Sprite BackgroundImage { get; set; }
        public RememberDialogueChoicesSO RemememberSO { get; set; }
        public List<ChoiceSO> ChoicesSO { get; set; }
        
        private Toggle _isRememberChoiceToggle;
        private Toggle _isChoiceNode;
        private DSObjectField _rememberChoiceDataField;
        private SaintsDictionary<DSChoiceSaveData, DSObjectField> _fieldMapping = new SaintsDictionary<DSChoiceSaveData, DSObjectField>();
        
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "New Choice"
            };
            
            Choices.Add(choiceData);
        }

        protected override void DrawExtensionContainer()
        {
            
            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            VisualElement toggleIsChoiceRow = DSElementUtility.CreateRow();
            VisualElement toggleIsRememberChoiceRow = DSElementUtility.CreateRow();
            
            _isChoiceNode = new Toggle()
            {
                value = IsChoiceNode
            };
            
            _isRememberChoiceToggle = new Toggle()
            {
                value = IsRememberChoiceNode
            };
            
            Label label = new Label("Is choice node?");
            label.style.marginRight = new StyleLength(5);
            toggleIsChoiceRow.Add(label);
            toggleIsChoiceRow.Add(_isChoiceNode);
            toggleIsChoiceRow.style.marginLeft = new StyleLength(10);
            
            Label label2 = new Label("Need to remember choice?");
            label.style.marginRight = new StyleLength(5);
            toggleIsRememberChoiceRow.Add(label2);
            toggleIsRememberChoiceRow.Add(_isRememberChoiceToggle);
            toggleIsRememberChoiceRow.style.marginLeft = new StyleLength(10);
            
            _rememberChoiceDataField = new DSObjectField(
                "",
                typeof(RememberDialogueChoicesSO)
                );
            _rememberChoiceDataField.value = RemememberSO;
            
            _isRememberChoiceToggle.RegisterValueChangedCallback(evl =>
            {
                if (evl.newValue == false)
                {
                    ResetAllRememberChoiceObjectField();
                }
                else
                {
                    EnableAllRememberChoiceObjectField();
                }
            });
            
            _isChoiceNode.RegisterValueChangedCallback(evl =>
            {
                if (Choices.Count > 1)
                {
                    _isChoiceNode.value = true;
                    return;
                }
            });

            if (_isRememberChoiceToggle.value)
            {
                EnableAllRememberChoiceObjectField();
            }
            else
            {
                ResetAllRememberChoiceObjectField();
            }

            VisualElement rememberChoiceRow = DSElementUtility.CreateRow();
            
            rememberChoiceRow.Add(toggleIsRememberChoiceRow);
            rememberChoiceRow.Add(_rememberChoiceDataField);
            rememberChoiceRow.style.justifyContent = Justify.SpaceBetween;
            rememberChoiceRow.style.marginTop = new StyleLength(5);
            
            customDataContainer.Add(toggleIsChoiceRow);
            customDataContainer.Add(rememberChoiceRow);
            
            /* BACKGROUND FOLDOUT */
            
            Foldout backgroundFoldout = DSElementUtility.CreateFoldout("Background");
            backgroundFoldout.style.marginTop = new StyleLength(10);
            
            VisualElement row2 = DSElementUtility.CreateRow();
            
            row2.style.justifyContent = Justify.SpaceBetween;
            
            DSObjectField backgroundSpriteField = new DSObjectField(
                "",
                typeof(Sprite),
                value: BackgroundImage
                );
            Image preview = new Image();
            preview.sprite = BackgroundImage;
            preview.scaleMode = ScaleMode.ScaleToFit;
            preview.tintColor = Color.white;
            preview.style.width = 100;
            preview.style.height = 100;

            backgroundSpriteField.RegisterValueChangedCallback(evl =>
            {
                preview.sprite = evl.newValue as Sprite;
                BackgroundImage = evl.newValue as Sprite;
            });
            
            row2.Add(backgroundSpriteField);
            row2.Add(preview);

            row2.style.marginTop = new StyleLength(15);
            
            backgroundFoldout.Add(row2);
            
            customDataContainer.Add(backgroundFoldout);
            
            extensionContainer.Add(customDataContainer);
            
            base.DrawExtensionContainer();
        }

        public override void Draw()
        {
            base.Draw();

            /* MAIN CONTAINER */

            Button addChoiceButton = DSElementUtility.CreateButton("Add Choice", () =>
            {
                DSChoiceSaveData choiceData = new DSChoiceSaveData()
                {
                    Text = "New Choice"
                };
                
                Choices.Add(choiceData);
                _isChoiceNode.value = true;

                Port choicePort = CreateChoicePort(choiceData);

                outputContainer.Add(choicePort);
            });

            addChoiceButton.AddToClassList("ds-node__button");

            mainContainer.Insert(1, addChoiceButton);

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = CreateChoicePort(choice);

                outputContainer.Add(choicePort);
            }

            InitializePortField();

            RefreshExpandedState();
        }

        private Port CreateChoicePort(object userData)
        {
            Port choicePort = this.CreatePort();

            choicePort.userData = userData;

            DSChoiceSaveData choiceData = (DSChoiceSaveData) userData;

            Button deleteChoiceButton = DSElementUtility.CreateButton("X", () =>
            {
                if (Choices.Count == 1)
                {
                    return;
                }

                if (choicePort.connected)
                {
                    graphView.DeleteElements(choicePort.connections);
                }
                
                Choices.Remove(choiceData);

                if (Choices.Count == 1)
                {
                    _isChoiceNode.value = false;
                }
                
                _fieldMapping.Remove(choiceData);
                graphView.RemoveElement(choicePort);
            });

            deleteChoiceButton.AddToClassList("ds-node__button");

            TextField choiceTextField = DSElementUtility.CreateTextField(choiceData.Text, null, callback =>
            {
                choiceData.Text = callback.newValue;
            });

            choiceTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__choice-text-field"
            );
            
            DSObjectField choiceObjectField = new DSObjectField(
                "",
                typeof(ChoiceSO)
            );

            _fieldMapping[choiceData] = choiceObjectField;
            choiceObjectField.enabledSelf = _isRememberChoiceToggle.value;

            choicePort.Add(choiceObjectField);
            choicePort.Add(choiceTextField);
            choicePort.Add(deleteChoiceButton);

            return choicePort;
        }

        private void InitializePortField()
        {
            if (!_isChoiceNode.value) return;
            
            for (var i = 0; i < _fieldMapping.Values.Count; i++)
            {
                _fieldMapping.Values.ToArray()[i].value = ChoicesSO[i];
            }
        }
        
        public override DSNodeSaveData GenerateNodeSaveData(List<DSChoiceSaveData> choices)
        {
            List<ChoiceSO> choiceSos = new List<ChoiceSO>();
            foreach (var field in _fieldMapping.Values)
            {
                choiceSos.Add(field.value as ChoiceSO);
            }
            
            return new DSMultipleChoiceDialogueNodeSaveData()
            {
                ID = ID,
                Name = NodeName,
                Choices = Choices,
                Text = Text,
                GroupID = Group?.ID,
                Position = GetPosition().position,
                Actions = Actions,
                LeftSprites = LeftSprites,
                RightSprites = RightSprites,
                IsChoiceNode = _isChoiceNode.value,
                RememberSO = _rememberChoiceDataField.value as RememberDialogueChoicesSO,
                ChoiceSO = choiceSos,
                BackgroundImage = BackgroundImage,
                IsRememberChoiceNode = _isRememberChoiceToggle.value
            };
        }
        
        public override void SaveNodeToScriptableObject(string containerFolderPath, DSDialogueContainerSO dialogueContainer, SaintsDictionary<string, DSNodeSO> createdDialogues, SaintsDictionary<string, DSGroupSO> createdGroups)
        {
            DSDialogueMultipleChoiceSO dialogue;
            
            if (Group != null)
            {
                dialogue = DSIOUtility.CreateAsset<DSDialogueMultipleChoiceSO>($"{containerFolderPath}/Groups/{Group.title}/Dialogues", NodeName);

                if (!dialogueContainer.Groups.ContainsKey(createdGroups[Group.ID]))
                {
                    dialogueContainer.Groups[createdGroups[Group.ID]] = new List<DSNodeSO>();
                }
                dialogueContainer.Groups[createdGroups[Group.ID]].Add(dialogue);
            }
            else
            {
                dialogue = DSIOUtility.CreateAsset<DSDialogueMultipleChoiceSO>($"{containerFolderPath}/Global/Dialogues", NodeName);

                dialogueContainer.UngroupedNodes.Add(dialogue);
            }
            
            List<ChoiceSO> choiceSos = new List<ChoiceSO>();
            foreach (var field in _fieldMapping.Values)
            {
                choiceSos.Add(field.value as ChoiceSO);
            }
            
            dialogue.Initialize(
                NodeName,
                Text,
                DSIOUtility.ConvertNodeChoicesToDialogueChoices(Choices),
                IsStartingNode(),
                Actions,
                LeftSprites,
                RightSprites,
                _isChoiceNode.value,
                _rememberChoiceDataField.value as RememberDialogueChoicesSO,
                choiceSos,
                BackgroundImage,
                _isRememberChoiceToggle.value
                );   
            
            createdDialogues.Add(ID, dialogue);
            if (IsStartingNode())
            {
                dialogueContainer.AddStartingNode(dialogue);
            }
            DSIOUtility.SaveAsset(dialogue); 
        }

        private void ResetAllRememberChoiceObjectField()
        {
            _isRememberChoiceToggle.value = false;
            _rememberChoiceDataField.Reset();
            _rememberChoiceDataField.SetEnabled(false);
            foreach (var field in _fieldMapping.Values)
            {
                field.Reset();
                field.SetEnabled(false);
            }
        }
        
        private void EnableAllRememberChoiceObjectField()
        {
            _isRememberChoiceToggle.value = true;
            _rememberChoiceDataField.SetEnabled(true);
            foreach (var field in _fieldMapping.Values)
            {
                field.SetEnabled(true);
            }
        }
        
    }
}