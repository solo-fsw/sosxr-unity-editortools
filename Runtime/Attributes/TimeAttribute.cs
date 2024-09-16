using UnityEngine;


namespace SOSXR.EditorTools
{
    public class TimeAttribute : PropertyAttribute
    {
        public readonly bool DisplayHours;


        public TimeAttribute(bool displayHours = false)
        {
            DisplayHours = displayHours;
        }
    }
}