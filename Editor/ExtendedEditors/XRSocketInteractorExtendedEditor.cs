using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace SOSXR.EditorTools
{
    [CustomEditor(typeof(XRSocketInteractor))]
    public class XRSocketInteractorExtendedEditor : EditorGUIHelpers
    {
        private void OnEnable()
        {
            GetInternalEditor(typeof(XRSocketInteractorEditor));
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (XRSocketInteractor) target;

            XRInteractButtons.CreateHoverButtons(targetObject);

            XRInteractButtons.CreateSelectButtons(targetObject);
        }
    }
}