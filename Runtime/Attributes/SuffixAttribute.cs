using Codice.CM.ConfigureHelper;
using UnityEngine;


namespace SOSXR.EditorTools
{
    
public class SuffixAttribute : PropertyAttribute
{
    public readonly string Suffix;


    public SuffixAttribute(string suffix)
    {
        Suffix = suffix;
    }
}
}
