using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CrossObjectEventWithData", menuName = "Cross object events/WITH DATA", order = 1)]
public class CrossObjectEventWithDataSO : ScriptableObject
{
    [SerializeField]
    private List<CrossObjectEventWithDataListener> listeners = new List<CrossObjectEventWithDataListener>();

    public void TriggerEvent(Component sender, params object[] data) {
        foreach (CrossObjectEventWithDataListener listener in new List<CrossObjectEventWithDataListener>(listeners))
        {
            listener.TriggerEvent(sender, data);
        }
    }

    public void AddListener(CrossObjectEventWithDataListener listener) {
        listeners.Add(listener);
    }

    public void RemoveListener(CrossObjectEventWithDataListener listener) {
        listeners.Remove(listener);
    }
}