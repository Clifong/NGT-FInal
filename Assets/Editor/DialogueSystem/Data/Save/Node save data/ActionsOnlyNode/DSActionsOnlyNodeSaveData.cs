using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    
    [Serializable]
    public class DSActionsOnlyNodeSaveData : DSNodeSaveData
    {
        [field: SerializeField] public List<ActionSO> Actions { get; set; }
    }
}