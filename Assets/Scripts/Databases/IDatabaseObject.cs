using SaintsField;
using UnityEngine;

public interface IDatabaseObject
{
    public void AddThingToDatabase(SaintsDictionary<string, IDatabaseObject> mapping);
}
