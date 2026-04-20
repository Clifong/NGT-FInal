using UnityEngine;

[CreateAssetMenu(fileName = "Contains", menuName = "Operators/Collection/CONTAINS", order = 1)]
public class ContainsOperator : CollectionOperator
{
    public override bool GetResult()
    {
        return (obj1.V as ICollectionObjectOperatoredOn).Contains(obj2.V as IObjectOperatedOn);
    }
}
