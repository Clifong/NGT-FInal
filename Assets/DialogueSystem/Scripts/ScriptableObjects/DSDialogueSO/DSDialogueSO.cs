using System.Collections.Generic;
using UnityEngine;

namespace DS.ScriptableObjects
{
    public abstract class DSDialogueSO : DSNodeSO
    {
        [field: SerializeField] [field: TextArea()] public string Text { get; set; }
        [field: SerializeField] public List<ActionSO> Actions { get; set; }
        [field: SerializeField] public List<Sprite> LeftSprites { get; set; }
        [field: SerializeField] public List<Sprite> RightSprites { get; set; }
        
    }
}