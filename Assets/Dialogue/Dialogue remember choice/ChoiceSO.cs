using SaintsField;
using UnityEngine;

[CreateAssetMenu(fileName = "Choice SO", menuName = "Dialogue/Choice SO", order = 1)]
public class ChoiceSO : ScriptableObject, IDatabaseObject, IOnlyEqualityObjectOperatedOn
{
    [SerializeField] private string _choiceId;
    [SerializeField] private string _choiceText;
    
    public string Deserialize()
    {
        return _choiceId;
    }

    public void AddThingToDatabase(SaintsDictionary<string, IDatabaseObject> mapping)
    {
        mapping[_choiceId] = this;
    }
    
    public object RetrieveValue()
    {
        return this;
    }

    public bool Equals(IOnlyEqualityObjectOperatedOn other)
    {
        return _choiceId == (other as ChoiceSO)._choiceId;
    }
    
    public bool NotEquals(IOnlyEqualityObjectOperatedOn other)
    {
        return _choiceId != (other as ChoiceSO)._choiceId;
    }
}
