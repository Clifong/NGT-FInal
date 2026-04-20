using UnityEngine;

[CreateAssetMenu(fileName = "MoreThan", menuName = "Operators/>", order = 1)]
public class MoreThanOperator : ValuesOperator
{
    
    public override bool GetResult()
    {
        return (obj1.V as INumberOperatedOn).MoreThan(obj2.V as INumberOperatedOn);
    }  
}
