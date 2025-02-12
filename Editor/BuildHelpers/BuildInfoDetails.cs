using System.IO;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    [CreateAssetMenu(fileName = "BuildInfoDetails", menuName = "SOSXR/BuildInfoDetails")]
    public class BuildInfoDetails : ScriptableObject
    {
        [SerializeField] private string m_filePath = "Assets/_SOSXR/Resources/build_info.csv";

        [Tooltip("The initial semantic version of the project, set if the semVer is not similar to this format")]
        public string InitialSemVer = "0_0_1";

        public string DevelopmentBuildIndicator = "d";
        public string ProductionBuildIndicator = "p";

        [HideInInspector] public string OldSemVer;
        [HideInInspector] public string NewSemVer;
        [HideInInspector] public int OldBundleVersionCode; // Android only
        [HideInInspector] public int NewBundleVersionCode; // Android only

        public string FilePath
        {
            get
            {
                if (!Directory.Exists(Path.GetDirectoryName(m_filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(m_filePath) ?? string.Empty);
                }

                if (!File.Exists(m_filePath))
                {
                    Debug.LogWarningFormat("File does not exist: {0}. It should be created upon your first build.", m_filePath);
                }

                return m_filePath;
            }
        }
    }
}