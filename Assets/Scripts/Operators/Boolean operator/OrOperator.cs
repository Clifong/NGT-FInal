using UnityEngine;

[CreateAssetMenu(fileName = "Or", menuName = "Operators/OR", order = 1)]
public class OrOperator : BooleanOperator
{
    public override bool GetResult()
    {
        return _leftOperator.GetResult() | _rightOperator.GetResult();
    }
}
