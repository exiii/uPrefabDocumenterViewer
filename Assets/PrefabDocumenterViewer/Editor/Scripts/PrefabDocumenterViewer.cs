using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace PrefabDocumenter.Unity
{
    public class PrefabDocumenterViewer : EditorWindow
    {
        private const string k_NameSearchBoxLabel = "FileName";
        private string m_NameSearchBoxText;

        private const string k_GuidSearchBoxLabel = "GUID";
        private string m_GuidSearchBoxText;

        private Dictionary<GUID, string> m_GuidAndPathPair;
        private int m_SearchResultSelected;

        private Vector2 m_SearchResultViewPos = Vector2.zero;

        [MenuItem("Window/PrefabDocumenterViewer")]
        private static void OpenWindow()
        {
            var window = GetWindow<PrefabDocumenterViewer>();
            window.m_GuidSearchBoxText = "";
            window.m_NameSearchBoxText = "";
        }

        private void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                m_NameSearchBoxText = EditorGUILayout.TextField(k_NameSearchBoxLabel, m_NameSearchBoxText);
                m_GuidSearchBoxText = EditorGUILayout.TextField(k_GuidSearchBoxLabel, m_GuidSearchBoxText);

                using (new EditorGUILayout.HorizontalScope())
                {
                    m_GuidAndPathPair = AssetDatabase.FindAssets(m_NameSearchBoxText)
                        .Select(guid => new GUID(guid))
                        .Where(guid =>
                        {
                            if (m_GuidSearchBoxText == "" || m_GuidSearchBoxText == null)
                            {
                                return true;
                            }

                            return Regex.Match(guid.ToString(), m_GuidSearchBoxText).Success;
                        })
                        .ToDictionary(guid => guid, guid => AssetDatabase.GUIDToAssetPath(guid.ToString()));

                    m_SearchResultViewPos = EditorGUILayout.BeginScrollView(m_SearchResultViewPos, GUI.skin.box);
                    {
                        m_SearchResultSelected = GUILayout.SelectionGrid(m_SearchResultSelected, m_GuidAndPathPair.Select(pair => pair.Value + ": " + pair.Key).ToArray(), 1, "PreferencesKeysElement");
                    }
                    EditorGUILayout.EndScrollView();

                    using (new EditorGUILayout.VerticalScope())
                    {
                        
                    }
                }
            }
        }

        private int arrowKeyToInt(KeyCode code)
        {
            switch (code)
            {
                case KeyCode.DownArrow:
                case KeyCode.RightArrow:
                    return -1;
                case KeyCode.UpArrow:
                case KeyCode.LeftArrow:
                    return 1;
            }

            return 0;
        }
    }
}