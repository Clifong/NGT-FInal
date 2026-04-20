using UnityEngine;

[CreateAssetMenu(fileName = "LessThan", menuName = "Operators/<", order = 1)]
public class LessThanOperator : ValuesOperator
{
    public override bool GetResult()
    {
        return (obj1.V as INumberOperatedOn).LessThan(obj2.V as INumberOperatedOn);
    }    
}
