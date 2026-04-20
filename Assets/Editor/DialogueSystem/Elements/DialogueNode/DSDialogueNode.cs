using System.Collections.Generic;
using System.Linq;
using DS.Data.Error;
using DS.Elements.DSListview;
using SaintsField;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data;
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;
    using ScriptableObjects;
    
    public class DSDialogueNode : DSNode
    {
        public string Text { get; set; }
        public List<ActionSO> Actions { get; set; }
        public List<Sprite> LeftSprites { get; set; }
        public List<Sprite> RightSprites { get; set; }

        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);
            Text = "Dialogue text.";
            Actions = new List<ActionSO>();
            LeftSprites = new List<Sprite>();
            RightSprites = new List<Sprite>();
        }

        protected virtual void DrawTitleContainer()
        {
            /* TITLE CONTAINER */

            TextField dialogueNameTextField = DSElementUtility.CreateTextField(NodeName, null, callback =>
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

            dialogueNameTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__text-field__hidden",
                "ds-node__filename-text-field"
            );

            titleContainer.Insert(0, dialogueNameTextField);
        }

        protected virtual void DrawInputContainer()
        {
            /* INPUT CONTAINER */

            Port inputPort = this.CreatePort("Dialogue Connection", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);

            inputContainer.Add(inputPort);
        }
        
        protected virtual void DrawExtensionContainer()
        {
            /* EXTENSION CONTAINER */

            VisualElement customDataContainer = new VisualElement();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            Foldout textFoldout = DSElementUtility.CreateFoldout("Dialogue Text");

            TextField textTextField = DSElementUtility.CreateTextArea(Text, null, callback => Text = callback.newValue);

            textTextField.AddClasses(
                "ds-node__text-field",
                "ds-node__quote-text-field"
            );

            textFoldout.Add(textTextField);
            
            Foldout actionsToInvokeFoldout = DSElementUtility.CreateFoldout("Actions to run during dialogue");
            
            VisualElement row = new VisualElement();

            VisualElement actionsListView = CreateActionList();
            
            actionsToInvokeFoldout.Add(actionsListView);

            VisualElement leftSpritesListView =  CreateSpritesList(LeftSprites);
            VisualElement rightSpritesListView = CreateSpritesList(RightSprites);
            
            Foldout spritesFoldout = DSElementUtility.CreateFoldout("Sprites to display on screen");
            
            row.Add(leftSpritesListView);
            row.Add(rightSpritesListView);

            row.AddClasses("ds-node__sprites_list-view");
            
            spritesFoldout.Add(row);
            
            customDataContainer.Add(spritesFoldout);
            customDataContainer.Add(textFoldout);
            customDataContainer.Add(actionsToInvokeFoldout);

            extensionContainer.Add(customDataContainer);
        }

        public override void Draw()
        {
            DrawTitleContainer();
            DrawInputContainer();
            DrawExtensionContainer();
        }
        
        private VisualElement CreateSpritesList(List<Sprite> sprites)
        {
            ListView spriteListView = new ListView();
            SaintsDictionary<VisualElement, int> listViewMap =  new SaintsDictionary<VisualElement, int>();
            SaintsDictionary<int, VisualElement[]> fieldMapping = new  SaintsDictionary<int, VisualElement[]>();
            DSListViewController dsListViewController = new DSListViewController();
            spriteListView.SetViewController(dsListViewController);
            
            spriteListView.itemsSource = sprites;
            
            spriteListView.onAdd = view =>
            {
                view.itemsSource.Add(null);
                view.RefreshItems();
            };
            spriteListView.showAddRemoveFooter =  true;
            
            spriteListView.bindItem = (e, i) =>
            {
                listViewMap[e] = i;
                VisualElement[] children = e.Children().ToArray();
                DSObjectField objectField = children[1] as DSObjectField;
                fieldMapping[i] = new []{objectField, children[2]};
        
                if (sprites[i] != null)
                {
                    objectField.value = sprites[i];
                }
                else
                {
                    sprites[i] = objectField.value as Sprite;
                }
        
                graphView.CheckIfNodesAreValid();
            };
            spriteListView.onRemove = view =>
            {
                VisualElement[] children = fieldMapping[sprites.Count - 1];
                DSObjectField objectField = children[0] as DSObjectField;
                Image previewImage = children[1] as Image;
                objectField.Reset();
                previewImage.sprite = null;
                fieldMapping.Remove(sprites.Count - 1);
                view.itemsSource.RemoveAt(sprites.Count - 1);
                view.RefreshItems();
                graphView.CheckIfNodesAreValid();
            };
            
            spriteListView.fixedItemHeight = 100.0f;
            spriteListView.makeItem = () =>
            {
                DSObjectField imageField = new DSObjectField("", typeof(Sprite));
                VisualElement row = new VisualElement();
                row.style.flexDirection = FlexDirection.Row;
                Image preview = new Image();
                preview.scaleMode = ScaleMode.ScaleToFit;
                preview.tintColor = Color.white;
                preview.style.width = 100;
                
                Button button = new Button();
                button.text = "-";
                button.clicked += () =>
                {
                    foreach (var row in fieldMapping.Values)
                    {
                        DSObjectField objectField = row[0] as DSObjectField;
                        Image previewImage = row[1] as Image;
                        objectField.Reset();
                        previewImage.sprite = null;
                    }
                    int i = listViewMap[row];
                    spriteListView.itemsSource.RemoveAt(i);
                    spriteListView.RefreshItems();
                };
                
                row.Add(button);
                row.Add(imageField);
                row.Add(preview);
                
                imageField.RegisterValueChangedCallback(evl =>
                {
                    sprites[listViewMap[row]] = evl.newValue as Sprite;
                    preview.sprite = evl.newValue as Sprite;  
                    spriteListView.RefreshItems();
                    graphView.CheckIfNodesAreValid();
                });
                return row;
            };

            return spriteListView;
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
        
        public override bool IsValidNode()
        {
            foreach (var i in Actions)
            {
                if (i == null) return false;
            }

            foreach (var sprite in LeftSprites)
            {
                if (sprite == null) return false;
            }
            
            foreach (var sprite in RightSprites)
            {
                if (sprite == null) return false;
            }

            return true;
        }

        public override void AddUngroupedNode<T>(SaintsDictionary<string, T> dictionary, DSGraphView graphView)
        {
            if (!dictionary.ContainsKey(NodeName))
            {
                DSErrorDialogueNodeData nodeErrorData = new DSErrorDialogueNodeData();

                nodeErrorData.Nodes.Add(this);
                
                (dictionary as SaintsDictionary<string, DSErrorDialogueNodeData>).Add(NodeName, nodeErrorData);

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
                (dictionary as SaintsDictionary<string, DSErrorDialogueNodeData>).Remove(NodeName);
            }
        }
    }
}