using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Actions", menuName = "Action/Broadcast any event")]
public class ActionBroadcastEventWithCallback : ActionSO
{
    [SerializeField] private CrossObjectEventWithDataSO _boradcastEvent;
    
    public override void InvokeAction(Action callback = null)
    {
        _boradcastEvent.TriggerEvent(new Component(), callback);
        base.InvokeAction(callback);
    }
}
