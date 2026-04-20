using UnityEngine;

[CreateAssetMenu(fileName = "MoreThanEqualsTo", menuName = "Operators/>=", order = 1)]
public class MoreThanEqualsToOperator : ValuesOperator
{
    public override bool GetResult()
    {
        return (obj1.V as INumberOperatedOn).MoreThanEqualTo(obj2.V as INumberOperatedOn);
    }        
}
