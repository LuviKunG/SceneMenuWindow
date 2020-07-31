using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace LuviKunG.SceneMenu
{
    public class SceneMenuWindow : EditorWindow
    {
        private const float BUTTON_HEIGHT = 24.0f;
        private const float TOOLBAR_SCENE_MODE_SELECTION_WIDTH = 170.0f;
        private const string EDITORPREFS_IS_SAVE_ON_CHANGE = "LuviKunG.Editor.SceneMenu.IsSaveOnChange";
        private const string EDITORPREFS_OPEN_SCENE_MODE = "LuviKunG.Editor.SceneMenu.OpenSceneMode";

        [MenuItem("Window/LuviKunG/Scene Menu")]
        public static SceneMenuWindow OpenWindow()
        {
            SceneMenuWindow window = GetWindow<SceneMenuWindow>(false, "Scene Menu", true);
            window.Show();
            return window;
        }

        private Vector2 scrollPosition;
        private bool isSaveOnChange;
        private OpenSceneMode openSceneMode;

        private void OnEnable()
        {
            if (EditorPrefs.HasKey(EDITORPREFS_IS_SAVE_ON_CHANGE))
                isSaveOnChange = EditorPrefs.GetBool(EDITORPREFS_IS_SAVE_ON_CHANGE);
            else
            {
                isSaveOnChange = true;
                EditorPrefs.SetBool(EDITORPREFS_IS_SAVE_ON_CHANGE, isSaveOnChange);
            }
            if (EditorPrefs.HasKey(EDITORPREFS_OPEN_SCENE_MODE))
                openSceneMode = (OpenSceneMode)EditorPrefs.GetInt(EDITORPREFS_OPEN_SCENE_MODE);
            else
            {
                openSceneMode = OpenSceneMode.Single;
                EditorPrefs.SetInt(EDITORPREFS_OPEN_SCENE_MODE, (int)openSceneMode);
            }
        }

        private void OnGUI()
        {
            using (var toolBarScope = new GUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    isSaveOnChange = GUILayout.Toggle(isSaveOnChange, GUIContent.none);
                    if (changeScope.changed)
                        EditorPrefs.SetBool(EDITORPREFS_IS_SAVE_ON_CHANGE, isSaveOnChange);
                }
                GUILayout.Label("Save on change", EditorStyles.label);
                using (var changeScope = new EditorGUI.ChangeCheckScope())
                {
                    openSceneMode = (OpenSceneMode)EditorGUILayout.EnumPopup(openSceneMode, EditorStyles.toolbarDropDown, GUILayout.MaxWidth(TOOLBAR_SCENE_MODE_SELECTION_WIDTH));
                    if (changeScope.changed)
                        EditorPrefs.SetInt(EDITORPREFS_OPEN_SCENE_MODE, (int)openSceneMode);
                }
                GUILayout.FlexibleSpace();
            }
            using (var scrollScope = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollScope.scrollPosition;
                var scenes = EditorBuildSettings.scenes;
                if (scenes != null && scenes.Length > 0)
                {
                    for (int i = 0; i < scenes.Length; i++)
                    {
                        using (var disabledScope = new EditorGUI.DisabledGroupScope(!scenes[i].enabled))
                        {
                            if (GUILayout.Button(Path.GetFileNameWithoutExtension(scenes[i].path), GUILayout.Height(BUTTON_HEIGHT)))
                            {
                                if (isSaveOnChange)
                                {
                                    if (EditorSceneManager.SaveOpenScenes())
                                        EditorSceneManager.OpenScene(scenes[i].path, openSceneMode);
                                }
                                else
                                    EditorSceneManager.OpenScene(scenes[i].path, openSceneMode);
                            }
                        }
                    }
                }
                else
                {
                    EditorGUILayout.HelpBox("No scene was set in build settings.", MessageType.Info, true);
                }
            }
        }
    }
}