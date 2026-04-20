using UnityEngine;

[CreateAssetMenu(fileName = "Not contains", menuName = "Operators/Collection/NOT CONTAINS", order = 1)]
public class NotContainsOperator : CollectionOperator
{
    public override bool GetResult()
    {
        return (obj1.V as ICollectionObjectOperatoredOn).NotContains(obj2.V as IObjectOperatedOn);
    }
}
