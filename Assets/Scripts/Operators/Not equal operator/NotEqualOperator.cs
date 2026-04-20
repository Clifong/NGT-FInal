using UnityEngine;

[CreateAssetMenu(fileName = "NotEqual", menuName = "Operators/!=", order = 1)]
public class NotEqualOperator : ValuesOperator
{
    public override bool GetResult()
    {
        return (obj1.V as INumberOperatedOn).MoreThan(obj2.V as INumberOperatedOn);
    }  
}
