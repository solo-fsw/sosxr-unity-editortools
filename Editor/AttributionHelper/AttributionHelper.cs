using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorSpice.EditorScripts
{
    /// <summary>
    /// Appends FlatIcon attribution to Markdown files by converting HTML attribution to Markdown format.
    /// Use Case: When you need to include Flaticon HTML attributions in Markdown files without manual formatting.
    /// How It Works: Presents a small popup to paste the HTML attribution, converts it to Markdown, and appends it to the target file.
    /// Integration: Exposes a context menu item under Assets/Create/SOSXR/Append FlatIcon attribution to Markdown.
    /// Related Classes: AttributionPopup inner class, ConvertHtmlToMarkdown helper.
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


            /// <summary>
            /// Opens a small utility window to paste Flaticon HTML attribution and append it as Markdown to the target file.
            /// Use Case: Invoked from the editor via the context menu after selecting a Markdown file.
            /// How It Works: Creates an AttributionPopup window bound to the provided file path and shows it as a utility window.
            /// Integration: Part of the Markdown attribution workflow; relies on ConvertHtmlToMarkdown for conversion.
            /// Related Classes: AttributionPopup inner class, MarkdownAttributionAppender.
            /// </summary>
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
