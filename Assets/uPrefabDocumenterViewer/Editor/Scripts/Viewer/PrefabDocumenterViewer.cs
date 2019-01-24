using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.Runtime.Remoting;
using UnityEngine;
using UnityEditor;

namespace PrefabDocumenter.Unity
{
    public class PrefabDocumenterViewer : EditorWindow
    {
        private Object m_DocumentDatabaseObject;
        private SqliteDatabase m_DocumentDatabase;

        private string m_NameSearchBoxText;

        private string m_GuidSearchBoxText;

        private Dictionary<GUID, string> m_GuidAndPathPair;
        private int m_SearchResultSelected;

        private Vector2 m_SearchResultViewPos = Vector2.zero;

        [MenuItem(ViewerWindowLabel.MENU_ITEM_ATTR)]
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
                m_DocumentDatabaseObject = EditorGUILayout.ObjectField(m_DocumentDatabaseObject, typeof(Object), false);

                if (m_DocumentDatabaseObject == null)
                {
                    return;
                }

                try
                {
                    m_DocumentDatabase = new SqliteDatabase(m_DocumentDatabaseObject.name);
                }
                catch
                {
                    Debug.LogWarning("正しい形式のデータベースを読み込んでください");
                    m_DocumentDatabaseObject = null;
                    return;
                }

                m_NameSearchBoxText = EditorGUILayout.TextField(ViewerWindowLabel.NAME_SEARCH_BOX, m_NameSearchBoxText);
                m_GuidSearchBoxText = EditorGUILayout.TextField(ViewerWindowLabel.GUID_SEARCH_BOX, m_GuidSearchBoxText);

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
                        // [TODO]
                        EditorGUI.BeginChangeCheck();
                        m_SearchResultSelected = GUILayout.SelectionGrid(m_SearchResultSelected, m_GuidAndPathPair.Select(pair => pair.Value + ": " + pair.Key).ToArray(), 1, "PreferencesKeysElement");
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(m_GuidAndPathPair.ElementAt(m_SearchResultSelected).Value, typeof(Object)));
                        }
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