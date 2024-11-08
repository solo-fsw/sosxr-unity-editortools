using System;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     From Warped Imagination: https://www.youtube.com/watch?v=M3tLr3EYIiE&t=0s
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DescriptionAttribute : Attribute
    {
        public DescriptionAttribute(string description = null)
        {
            Description = description;
        }


        public string Description { get; private set; } = null;
    }
}