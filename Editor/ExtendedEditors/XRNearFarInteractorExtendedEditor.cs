using UnityEditor;
using UnityEditor.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors;


namespace SOSXR.EditorTools
{
    [CustomEditor(typeof(NearFarInteractor))]
    public class XRNearFarInteractorExtendedEditor : EditorGUIHelpers
    {
        private void OnEnable()
        {
            GetInternalEditor(typeof(NearFarInteractorEditor));
        }


        protected override void CustomInspectorContent()
        {
            var targetObject = (NearFarInteractor) target;

            XRInteractButtons.CreateHoverButtons(targetObject);

            XRInteractButtons.CreateSelectButtons(targetObject);

            XRInteractButtons.CreateUIHoverButtons(targetObject);
        }
    }
}