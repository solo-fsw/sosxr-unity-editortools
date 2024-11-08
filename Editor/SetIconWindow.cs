using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace SOSXR.EditorTools
{
    /// <summary>
    ///     From Warped Imagination: https://www.youtube.com/watch?v=SY_SdcPW8vE
    ///     With great help from ChatGPT
    /// </summary>
    public class SetIconWindow : EditorWindow
    {
        private List<Texture2D> _iconTextures;
        private int _selectedIconIndex;
        private GUIStyle _iconButtonStyle;

        private Vector2 scrollPos = default;
        private int _iconsPerRow; // Icons displayed per 
        private const string MENU_PATH = "SOSXR/Set Icon... _%i"; // Shortcut: Ctrl/Cmd + I
        private const string ICON_LABEL = "ScriptIcon";
        private const int ICON_WIDTH = 200;
        private const int ICON_HEIGHT = 200;
        private const int BUTTON_WIDTH = 200;


        [MenuItem(MENU_PATH, priority = 0)]
        public static void ShowMenuItem()
        {
            var window = (SetIconWindow) GetWindow(typeof(SetIconWindow));
            window.titleContent = new GUIContent("Icons with Label: " + ICON_LABEL);
            window.Show();
        }


        [MenuItem(MENU_PATH, validate = true)]
        public static bool ValidateMenuItem() // Validate menu item - only show editor window when MonoScript is selected
        {
            foreach (var asset in Selection.objects)
            {
                if (asset.GetType() != typeof(MonoScript))
                {
                    return false;
                }
            }

            return true;
        }


        private void OnGUI()
        {
            InitializeIconTextures();

            if (_iconTextures == null || _iconTextures.Count == 0)
            {
                DisplayNoIconsFoundMessage();

                return;
            }

            InitializeGUIStyle();
            DisplayIconSelectionGrid();
            ProcessKeyboardInput();
            DisplayApplyButton();
            DisplayDefaultIconButton();
        }


        private void InitializeIconTextures()
        {
            if (_iconTextures != null)
            {
                return;
            }

            _iconTextures = new List<Texture2D>();
            var assetGuids = AssetDatabase.FindAssets("t:texture2D, l:" + ICON_LABEL);

            foreach (var assetGuid in assetGuids)
            {
                var path = AssetDatabase.GUIDToAssetPath(assetGuid);
                _iconTextures.Add(AssetDatabase.LoadAssetAtPath<Texture2D>(path));
            }
        }


        private void DisplayNoIconsFoundMessage()
        {
            GUILayout.Label("No icons found");

            if (GUILayout.Button("Close", GUILayout.Width(BUTTON_WIDTH)))
            {
                Close();
            }
        }


        private void InitializeGUIStyle()
        {
            _iconButtonStyle ??= new GUIStyle(GUI.skin.button)
            {
                fixedWidth = ICON_WIDTH,
                fixedHeight = ICON_HEIGHT,
                margin = new RectOffset(5, 5, 5, 5),
                alignment = TextAnchor.MiddleCenter
            };
        }


        private void DisplayIconSelectionGrid()
        {
            var iconContents = new GUIContent[_iconTextures.Count];

            for (var i = 0; i < _iconTextures.Count; i++)
            {
                var scaledIcon = ScaleTexture(_iconTextures[i], ICON_WIDTH, ICON_HEIGHT);
                iconContents[i] = new GUIContent(scaledIcon);
            }

            // Calculate how many icons can fit in the current window width
            var windowWidth = EditorGUIUtility.currentViewWidth;
            _iconsPerRow = Mathf.Max(1, Mathf.FloorToInt(windowWidth / (ICON_WIDTH + _iconButtonStyle.margin.left + _iconButtonStyle.margin.right))) - 1;

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, false);

            _selectedIconIndex = GUILayout.SelectionGrid(
                _selectedIconIndex,
                iconContents,
                _iconsPerRow, // Use dynamic number of icons per row
                _iconButtonStyle
            );

            GUILayout.EndScrollView();
        }


        private void ProcessKeyboardInput()
        {
            if (Event.current == null || Event.current.type != EventType.KeyDown)
            {
                return;
            }

            switch (Event.current.keyCode)
            {
                case KeyCode.RightArrow:
                    SelectNextIcon();

                    break;
                case KeyCode.LeftArrow:
                    SelectPreviousIcon();

                    break;
                case KeyCode.UpArrow:
                    SelectIconAbove();

                    break;
                case KeyCode.DownArrow:
                    SelectIconBelow();

                    break;
                case KeyCode.Return:
                    ApplySelectedIcon();

                    break;
                case KeyCode.Escape:
                    Close();

                    break;
            }
        }


        private void SelectNextIcon()
        {
            if (_selectedIconIndex >= _iconTextures.Count - 1)
            {
                return;
            }

            _selectedIconIndex++;
            Event.current.Use();
        }


        private void SelectPreviousIcon()
        {
            if (_selectedIconIndex <= 0)
            {
                return;
            }

            _selectedIconIndex--;
            Event.current.Use();
        }


        private void SelectIconAbove()
        {
            if (_selectedIconIndex < _iconsPerRow)
            {
                return;
            }

            _selectedIconIndex -= _iconsPerRow;
            Event.current.Use();
        }


        private void SelectIconBelow()
        {
            if (_selectedIconIndex + _iconsPerRow >= _iconTextures.Count)
            {
                return;
            }

            _selectedIconIndex += _iconsPerRow;
            Event.current.Use();
        }


        private void ApplySelectedIcon()
        {
            ApplyIcon(_iconTextures[_selectedIconIndex]);
            Close();
        }


        private void DisplayApplyButton()
        {
            if (GUILayout.Button("Apply New Icon", GUILayout.Width(BUTTON_WIDTH)))
            {
                ApplySelectedIcon();
            }
        }


        private void DisplayDefaultIconButton()
        {
            if (GUILayout.Button("Reset to Default Icon", GUILayout.Width(BUTTON_WIDTH)))
            {
                ApplyIcon(null);
                Close();
            }
        }


        private void ApplyIcon(Texture2D icon)
        {
            AssetDatabase.StartAssetEditing();

            foreach (var asset in Selection.objects)
            {
                var path = AssetDatabase.GetAssetPath(asset);
                var importer = AssetImporter.GetAtPath(path) as MonoImporter;
                importer?.SetIcon(icon);
                AssetDatabase.ImportAsset(path);
            }

            AssetDatabase.StopAssetEditing();
            AssetDatabase.Refresh();
        }


        private Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            var rt = new RenderTexture(targetWidth, targetHeight, 24);
            Graphics.Blit(source, rt);
            RenderTexture.active = rt;

            var result = new Texture2D(targetWidth, targetHeight);
            result.ReadPixels(new Rect(0, 0, targetWidth, targetHeight), 0, 0);
            result.Apply();

            RenderTexture.active = null;
            rt.Release();
            DestroyImmediate(rt);

            return result;
        }
    }
}