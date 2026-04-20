using SaintsField;
using UnityEngine;

[CreateAssetMenu(fileName = "Remember dialogue SO", menuName = "Dialogue/Remember dialogue SO", order = 1)]
public class RememberDialogueChoicesSO : ScriptableObject, IDatabaseObject, IOnlyEqualityObjectOperatedOn
{
	[SerializeField] private string _rememberChoiceId;

	public string Deserialize()
	{
		return _rememberChoiceId;
	}

	public void AddThingToDatabase(SaintsDictionary<string, IDatabaseObject> mapping)
	{
		mapping[_rememberChoiceId] = this;
	}

	public object RetrieveValue()
	{
		return RememberChoiceManager.instance.GetRememberedChoice(this);
	}

	public bool Equals(IOnlyEqualityObjectOperatedOn other)
	{
		ChoiceSO retrieved = RetrieveValue() as ChoiceSO;
		if (retrieved == null) return false;
		return retrieved.Equals(other as ChoiceSO);
	}

	public bool NotEquals(IOnlyEqualityObjectOperatedOn other)
	{
		ChoiceSO retrieved = RetrieveValue() as ChoiceSO;
		if (retrieved == null) return false;
		return retrieved.NotEquals(other as ChoiceSO);
	}
}

