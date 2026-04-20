using SaintsField;
using UnityEngine;

public abstract class CollectionOperator : Operator
{
    [SerializeField] protected SaintsInterface<ICollectionObjectOperatoredOn> obj1;
    [SerializeField] protected SaintsInterface<IObjectOperatedOn> obj2;
    
}