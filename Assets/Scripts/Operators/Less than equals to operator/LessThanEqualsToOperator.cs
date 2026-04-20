using UnityEngine;

[CreateAssetMenu(fileName = "LessThanEqualsTo", menuName = "Operators/<=", order = 1)]
public class LessThanEqualsToOperator : ValuesOperator
{
    public override bool GetResult()
    {
        return (obj1.V as INumberOperatedOn).LessThanQualTo(obj2.V as INumberOperatedOn);
    }   
}
