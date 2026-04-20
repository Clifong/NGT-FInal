using UnityEngine;

[CreateAssetMenu(fileName = "And", menuName = "Operators/AND", order = 1)]
public class AndOperator : BooleanOperator
{
    public override bool GetResult()
    {
        return _leftOperator.GetResult() & _rightOperator.GetResult();
    }
}
