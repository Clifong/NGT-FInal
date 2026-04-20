using UnityEngine;

[CreateAssetMenu(fileName = "Equals", menuName = "Operators/==", order = 1)]
public class EqualOperator : ValuesOperator
{
    public override bool GetResult()
    {
        if (obj1.V is IOnlyEqualityObjectOperatedOn)
        {
            return (obj1.V as IOnlyEqualityObjectOperatedOn).Equals(obj2.V as IOnlyEqualityObjectOperatedOn);
        } else if (obj1.V is INumberOperatedOn)
        {
            return (obj1.V as INumberOperatedOn).Equals(obj2.V as INumberOperatedOn);
        }
        return (obj1.V as IObjectOperatedOn).Equals(obj2.V as IObjectOperatedOn);
    }
}
