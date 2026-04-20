using UnityEngine;

public abstract class BooleanOperator: Operator
{
    [SerializeField] protected Operator _leftOperator;
    [SerializeField] protected Operator _rightOperator;
}
