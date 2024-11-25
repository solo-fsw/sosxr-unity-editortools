using SOSXR.EditorTools;
using UnityEngine;


public class SuffixDemo : MonoBehaviour
{
    [Suffix("SOSXR CAN MAKE LONG SUFFIX")]
    public float Weight;

    [Suffix("m")]
    public float Length;

    [Suffix("Name")]
    public string Name;
    
    [Suffix("Trees")]
    public Vector2 Trees;
}