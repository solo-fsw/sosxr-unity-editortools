using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    /// <summary>
    ///     This allows you to copy the HTML attribution from FlatIcon and append it to a Markdown file, in Markdown format, without losing the original data.
    ///     Flaticon provides HTML attribution for icons (<a href="https://www.flaticon.com/free-icons/sushi" title="sushi icons">Sushi icons created by nawicon - Flaticon</a>), which can be cumbersome to include in Markdown files.
    ///     Right click on a Markdown file in the Project window, select "SOSXR/Append FlatIcon attribution to Markdown", paste the HTML attribution, and it will be converted to Markdown format and appended to the file.
    /// </summary>
    public static class MarkdownAttributionAppender
    {
        [MenuItem("Assets/Create/SOSXR/Append FlatIcon attribution to Markdown", true)]
        private static bool ValidateMenu()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);

            return path.EndsWith(".md");
        }


        [MenuItem("Assets/Create/SOSXR/Append FlatIcon attribution to Markdown")]
        private static void ShowPopup()
        {
            var path = AssetDatabase.GetAssetPath(Selection.activeObject);
            AttributionPopup.Show(path);
        }


        private class AttributionPopup : EditorWindow
        {
            private string _htmlInput = "";
            private string _filePath;


            public static void Show(string filePath)
            {
                var window = CreateInstance<AttributionPopup>();
                window._filePath = filePath;
                window.titleContent = new GUIContent("Paste Flaticon Attribution");
                window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 150);
                window.ShowUtility();
            }


            private void OnGUI()
            {
                EditorGUILayout.LabelField("Paste Flaticon HTML below:", EditorStyles.boldLabel);
                _htmlInput = EditorGUILayout.TextArea(_htmlInput, GUILayout.Height(60));

                GUILayout.Space(10);

                if (GUILayout.Button("Append to Markdown"))
                {
                    var markdown = ConvertHtmlToMarkdown(_htmlInput);
                    File.AppendAllText(_filePath, "\n" + "- " + markdown + "\n");
                    AssetDatabase.Refresh();
                    Close();
                }
            }


            private static string ConvertHtmlToMarkdown(string html)
            {
                var pattern = @"<a\s+href=""(?<url>[^""]+)""\s+title=""(?<title>[^""]+)"">(?<text>[^<]+)</a>";
                var match = Regex.Match(html, pattern);

                if (!match.Success)
                {
                    return html;
                }

                var url = match.Groups["url"].Value;
                var title = match.Groups["title"].Value;
                var text = match.Groups["text"].Value;

                return $"[{text}]({url} \"{title}\")";
            }
        }
    }
}