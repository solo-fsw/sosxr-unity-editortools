#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace SOSXR.BuildHelpers
{
    public class SceneBuildValidation : IProcessSceneWithReport
    {
        private static readonly bool RunOnlyOnBuilding = true;

        /// <summary>
        ///     The order in relationship to other IProcessSceneWithReport callbacks. Lower numbers are called first.
        /// </summary>
        public int callbackOrder => 0;


        public void OnProcessScene(Scene scene, BuildReport report)
        {
            if (!BuildPipeline.isBuildingPlayer && RunOnlyOnBuilding)
            {
                return;
            }

            if (!AreAllSceneValidationComponentsValid(scene))
            {
                throw new BuildFailedException("Scene " + scene.name + " has validation errors");
            }
        }


        private static bool AreAllSceneValidationComponentsValid(Scene scene)
        {
            var isValid = true;

            foreach (var gameObject in scene.GetRootGameObjects())
            {
                var validators = gameObject.GetComponentsInChildren<IValidate>();

                if (validators is not {Length: > 0})
                {
                    continue;
                }

                foreach (var validator in validators)
                {
                    if (validator.IsValid)
                    {
                        continue;
                    }

                    isValid = false;
                    Debug.LogError("SceneBuildValidation" + " Detected Validation issue with " + validator.GetType().Name);
                }
            }

            return isValid;
        }
    }
}
#endif