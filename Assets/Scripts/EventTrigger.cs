using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    [SerializeField] private CrossObjectEventSO eventToTrigger;

    public void TriggerEvent()
    {
        eventToTrigger.TriggerEvent();
    }
}
