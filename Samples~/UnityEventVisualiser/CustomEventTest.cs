using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


public class CustomEventTest : MonoBehaviour
{
    public UnityEvent simpleEvent;
    public CustomComplexEvent complexEvent;

    public ScriptableObjectEventTest soTest;


    [ContextMenu(nameof(TriggerSimpleEvent))]
    public void TriggerSimpleEvent()
    {
        simpleEvent?.Invoke();
    }


    [ContextMenu(nameof(TriggerComplexEvent))]
    public void TriggerComplexEvent()
    {
        complexEvent?.Invoke("a", 1, 2);

        StartCoroutine(DelayedTrigger("Test", 0, 1));
    }


    private IEnumerator DelayedTrigger(string message, int a, int b)
    {
        for (var i = 0; i < 20; i++)
        {
            yield return new WaitForSeconds(0.1f);

            complexEvent?.Invoke(message, a, b);
        }
    }
}


[Serializable]
public class CustomComplexEvent : UnityEvent<string, int, int>
{
}