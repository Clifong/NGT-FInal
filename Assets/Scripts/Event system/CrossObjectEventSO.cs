using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrossObjectEvent", menuName = "Cross object events/NO DATA", order = 1)]
public class CrossObjectEventSO : ScriptableObject
{
    [SerializeField]
    protected List<CrossObjectEventListener> listeners = new List<CrossObjectEventListener>();

    public void TriggerEvent() {
        foreach (CrossObjectEventListener listener in new List<CrossObjectEventListener>(listeners))
        {
            listener.TriggerEvent();
        }
    }

    public void AddListener(CrossObjectEventListener listener) {
        listeners.Add(listener);
    }

    public void RemoveListener(CrossObjectEventListener listener) {
        listeners.Remove(listener);
    }
}