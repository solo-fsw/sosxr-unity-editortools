using UnityEngine;
using UnityEngine.Events;


[CreateAssetMenu(menuName = "SOSXR/UnityEventVisualizer/Scriptable Object Test")]
public class ScriptableObjectEventTest : ScriptableObject
{
    public UnityEvent OnTest;


    [ContextMenu(nameof(DoTest))]
    public void DoTest()
    {
        OnTest?.Invoke();
    }
}