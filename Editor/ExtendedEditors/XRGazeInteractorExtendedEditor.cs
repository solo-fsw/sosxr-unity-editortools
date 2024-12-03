using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace SOSXR.EditorTools
{
    [CustomEditor(typeof(XRGazeInteractor))]
    public class XRGazeInteractorExtendedEditor : EditorGUIHelpers
    {
        private void OnEnable()
        {
            GetInternalEditor(typeof(XRGazeInteractorEditor));
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (XRGazeInteractor) target;

            XRInteractButtons.CreateHoverButtons(targetObject);

            XRInteractButtons.CreateSelectButtons(targetObject);

            XRInteractButtons.CreateUIHoverButtons(targetObject);
        }
    }
}