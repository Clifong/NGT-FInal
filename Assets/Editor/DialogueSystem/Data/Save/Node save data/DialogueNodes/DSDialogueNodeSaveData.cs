using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    using Enumerations;
    
    public abstract class DSDialogueNodeSaveData : DSNodeSaveData
    {
        [field: SerializeField] public string Text { get; set; }
        [field: SerializeField] public List<ActionSO> Actions { get; set; }
        [field: SerializeField] public List<Sprite> LeftSprites { get; set; }
        [field: SerializeField] public List<Sprite> RightSprites { get; set; }
    }
}