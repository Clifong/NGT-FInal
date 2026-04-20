using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save.ConditionsNode
{
    [Serializable]
    public class DSConditionsNodeSaveData: DSNodeSaveData
    {
        [field: SerializeField] public List<Operator> Operators { get; set; }
    }
}