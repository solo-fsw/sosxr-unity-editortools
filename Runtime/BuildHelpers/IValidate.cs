using UnityEngine;


namespace SOSXR.BuildHelpers
{
    /// <summary>
    ///     Add this to all scripts that require a validation step
    /// </summary>
    public interface IValidate
    {
        bool IsValid { get; }


        void OnValidate();
    }


    public class ThisIsAValidationClass : MonoBehaviour, IValidate
    {
        public GameObject AnotherGameObject;

        public bool IsValid { get; private set; }


        public void OnValidate()
        {
            IsValid = AnotherGameObject != null;
        }
    }
}