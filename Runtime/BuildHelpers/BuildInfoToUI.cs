using System;
using System.IO;
using TMPro;
using UnityEngine;


namespace SOSXR.BuildHelpers
{
    public class BuildInfoToUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_buildInfoText;

        private string _currentTime;
        private float _elapsedTime;
        private TextAsset _buildInfoTextAsset;
        private const float _updateInterval = 10f;


        private void Awake()
        {
            _buildInfoTextAsset = Resources.Load<TextAsset>("build_info");
        }


        private void Start()
        {
            InvokeRepeating(nameof(SetBuildText), 0, _updateInterval);
        }


        private void SetBuildText()
        {
            if (_buildInfoTextAsset == null)
            {
                m_buildInfoText.text = "No build information available.";

                return;
            }
            
            var lastLine = ReadLastLineOfFile(_buildInfoTextAsset.text);

            if (lastLine != null)
            {
                m_buildInfoText.text = FormatBuildInfo(lastLine);
            }
            else
            {
                m_buildInfoText.text = "No build information available.";
            }
        }


        public static string ReadLastLineOfFile(string fileContent)
        {
            string lastLine = null;

            using var sr = new StringReader(fileContent);

            while (sr.ReadLine() is { } line)
            {
                lastLine = line;
            }

            return lastLine;
        }


        private string FormatBuildInfo(string line)
        {
            var parts = line.Split(',');

            // Assuming the format is "BuildDate,BuildTime,DevBuild,SemVer,BundleCode"
            if (parts.Length >= 5)
            {
                var buildDate = parts[0];
                var buildTime = parts[1];
                var devBuild = parts[2];
                var semVer = parts[3];
                var bundleCode = parts[4];

                var buildDateTime = DateTime.ParseExact($"{buildDate} {buildTime}", "yyyy-MM-dd HH:mm", null);
                var now = DateTime.Now;
                _currentTime = now.ToString("HH:mm");

                var daysAgo = GetRelativeDateDescription(buildDateTime.Date, now.Date);

                return $"DevBuild: {devBuild}\n" +
                       $"SemVer: {semVer}\n" +
                       $"BundleCode: {bundleCode}\n" +
                       $"Build Date: {daysAgo} at {buildTime}\n" +
                       $"Current Time: {_currentTime}";
            }

            return "Invalid build information format.";
        }


        private string GetRelativeDateDescription(DateTime buildDate, DateTime currentDate)
        {
            var daysDifference = (currentDate - buildDate).Days;

            return daysDifference switch
                   {
                       0 => "today",
                       1 => "yesterday",
                       _ => $"{daysDifference} days ago"
                   };
        }
    }
}