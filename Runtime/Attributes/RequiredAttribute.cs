using System;
using UnityEngine;


namespace SOSXR.EditorTools
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false) ]
    public class RequiredAttribute : PropertyAttribute
    {
       
    }
}