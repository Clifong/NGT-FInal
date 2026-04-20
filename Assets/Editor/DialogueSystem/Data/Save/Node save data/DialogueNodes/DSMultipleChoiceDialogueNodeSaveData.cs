using System;
using System.Collections.Generic;
using UnityEngine;

namespace DS.Data.Save
{
    [Serializable]
    public class DSMultipleChoiceDialogueNodeSaveData : DSDialogueNodeSaveData
    {
        [field: SerializeField] public bool IsChoiceNode { get; set; }
        [field: SerializeField] public bool IsRememberChoiceNode { get; set; }
        [field: SerializeField] public RememberDialogueChoicesSO RememberSO { get; set; }
        [field: SerializeField] public List<ChoiceSO> ChoiceSO;
        [field: SerializeField] public Sprite BackgroundImage;
    }
}