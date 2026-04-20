using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class ActionSO : ScriptableObject
{
    [SerializeField] private UnityEvent _additionalEvents;

    public virtual void InvokeAction(Action action = null)
    {
        _additionalEvents.Invoke();
    }
}
