using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactables;


namespace SOSXR.EditorTools
{
    [CustomEditor(typeof(XRGrabInteractable))]
    public class XRGrabInteractableExtendedEditor : EditorGUIHelpers
    {
        private void OnEnable()
        {
            GetInternalEditor(typeof(XRGrabInteractableEditor));
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (XRBaseInteractable) target;

            XRInteractButtons.CreateFirstLastHoverButtons(targetObject);

            XRInteractButtons.CreateHoverButtons(targetObject);

            XRInteractButtons.CreateFirstLastSelectButtons(targetObject);

            XRInteractButtons.CreateSelectButtons(targetObject);

            XRInteractButtons.CreateFirstLastFocusButtons(targetObject);

            XRInteractButtons.CreateFocusButtons(targetObject);

            XRInteractButtons.CreateActivateButtons(targetObject);
        }
    }
}