using System;
using SaintsField;
using UnityEngine;

public abstract class ValuesOperator : Operator
{
    [SerializeField] protected SaintsInterface<INumberOperatedOn> obj1;
    [SerializeField] protected SaintsInterface<INumberOperatedOn> obj2;

    public void SetObject(IObjectOperatedOn obj1, IObjectOperatedOn obj2)
    {
        // this.obj1 =  obj1;
        // this.obj2 =  obj2;
    }
}