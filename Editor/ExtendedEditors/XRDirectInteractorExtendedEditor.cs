using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace SOSXR.EditorTools
{
    [CustomEditor(typeof(XRDirectInteractor))]
    public class XRInteractorExtendedBaseInteractorEditor : EditorGUIHelpers
    {
        private void OnEnable()
        {
            GetInternalEditor(typeof(XRDirectInteractorEditor));
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (XRDirectInteractor) target;

            XRInteractButtons.CreateHoverButtons(targetObject);

            XRInteractButtons.CreateSelectButtons(targetObject);
        }
    }
}