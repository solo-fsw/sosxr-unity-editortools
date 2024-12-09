using UnityEngine;
using UnityEngine.Events;


public class UnityEventDemo : MonoBehaviour
{
    public UnityEvent OnEvent;
    public UnityEvent<string> OnEventString;
    public UnityEvent<int> OnEventInt;
    public UnityEvent<float> OnEventFloat;
    public UnityEvent<Vector2> OnEventVector2;
    public UnityEvent<Vector3> OnEventVector3;
}