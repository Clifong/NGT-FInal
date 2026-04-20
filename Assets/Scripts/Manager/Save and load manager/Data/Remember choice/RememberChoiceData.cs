using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Newtonsoft.Json;
using SaintsField;
using UnityEngine.Serialization;

public class RememberChoiceDataSerializer : GameDataSerializer<GameData> {
    
    public override GameData ReadJson(JsonReader reader, Type objectType, GameData existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        GameData res = base.ReadJson(reader, objectType, existingValue, hasExistingValue, serializer);
        if (res != null) return res;
        RememberChoiceData newData = new RememberChoiceData();
        newData.Deserialize(reader);
        return newData;
    }
}

[System.Serializable]
public class RememberChoiceData : GameData
{
    private class RememberChoiceDataJsonForm
    {
        [SerializeField] private string[] _rememberChoiceSoId;
        [SerializeField] private string[] _choiceSoId;

        public RememberChoiceDataJsonForm()
        {
        }

        public RememberChoiceDataJsonForm(string[] rememberChoiceSoId, string[] choiceSoId)
        {
            _rememberChoiceSoId = rememberChoiceSoId;
            _choiceSoId = choiceSoId;
        }

        public string Serialize()
        {
            return JsonUtility.ToJson(this);
        }

        public void Deserialize(RememberChoiceData rememberChoiceData, string json)
        {
            JsonUtility.FromJsonOverwrite(json, this);
            SaintsDictionary<string, string> mapping = new SaintsDictionary<string, string>();
            for (int i = 0; i < _rememberChoiceSoId.Length; i++)
            {
                mapping[_rememberChoiceSoId[i]] = _choiceSoId[i];
            }
            rememberChoiceData.UpdateFields(mapping);
            // List<CombatCharacterSO> allPartyMembers = ConvertJsonToCombatCharacterSOList(_allPartyMembers);
            // List<CombatCharacterSO> allActivePartyMembers = ConvertJsonToCombatCharacterSOList(_allActivePartyMembers);
            // List<CombatCharacterSO> allSupportPartyMembers = ConvertJsonToCombatCharacterSOList(_allSupportPartyMembers);
            // partyData.UpdateFields(allPartyMembers, allActivePartyMembers, allSupportPartyMembers);
        }

    }

    [SerializeField] private SaintsDictionary<RememberDialogueChoicesSO, ChoiceSO> _rememberDataSOToChoiceSO = new SaintsDictionary<RememberDialogueChoicesSO, ChoiceSO>();

    public RememberChoiceData()
    {
        customSerializer = new RememberChoiceDataSerializer();
    }

    public override void Serialize(JsonWriter writer)
    {
        List<string> rememberId = new List<string>();
        List<string> choiceId = new List<string>();
        foreach (var rememberDialogueChoicesSo in _rememberDataSOToChoiceSO.Keys)
        {
            rememberId.Add(rememberDialogueChoicesSo.Deserialize());
            choiceId.Add(_rememberDataSOToChoiceSO[rememberDialogueChoicesSo].Deserialize());
        }
        RememberChoiceDataJsonForm rememberChoiceDataJsonForm = new RememberChoiceDataJsonForm(
            rememberId.ToArray(),
            choiceId.ToArray()
        );
        writer.WriteValue(rememberChoiceDataJsonForm.Serialize());
    }

    public void UpdateFields(SaintsDictionary<string, string> mapping)
    {
        foreach (var keyValuePair in mapping)
        {
            _rememberDataSOToChoiceSO[RememberChoiceDatabase.instance.RetrieveScriptableObject(keyValuePair.Key)] = ChoiceDatabase.instance.RetrieveScriptableObject(keyValuePair.Value);
        }
    }

    public override void Deserialize(JsonReader reader)
    {
        RememberChoiceDataJsonForm rememberChoiceDataJsonForm = new RememberChoiceDataJsonForm();
        rememberChoiceDataJsonForm.Deserialize(this, reader.Value as string);
    }

    public void AddRememberDataAndChoice(RememberDialogueChoicesSO rememberSo, ChoiceSO choiceSo)
    {
        _rememberDataSOToChoiceSO[rememberSo] = choiceSo;
    }

    public ChoiceSO GetRememberedChoice(RememberDialogueChoicesSO rememberSo)
    {
        if (!_rememberDataSOToChoiceSO.ContainsKey(rememberSo))
        {
            return null;
        }
        return _rememberDataSOToChoiceSO[rememberSo];
    }

}
