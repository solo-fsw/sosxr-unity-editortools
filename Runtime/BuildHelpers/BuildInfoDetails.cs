using UnityEngine;


namespace SOSXR.BuildHelpers
{
    [CreateAssetMenu(fileName = "BuildInfoDetails", menuName = "SOSXR/BuildInfoDetails")]
    public class BuildInfoDetails : ScriptableObject
    {
        public string FilePath = "Assets/_SOSXR/Resources/build_info.csv";
        public string InitialSemVer = "0_0_1";

        public string DevelopmentBuildIndicator = "d";
        public string ProductionBuildIndicator = "p";


        [Header("Don't change these values")]
        public string OldSemVer;
        public string NewSemVer;

        public int OldBundleVersionCode; // Android only
        public int NewBundleVersionCode; // Android only
    }
}