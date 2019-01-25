using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
        private DraftXmlDao m_DraftXmlConnector;

        private string m_NameSearchBoxText;

        private string m_GuidSearchBoxText;

        private Dictionary<string, string> m_GuidAndPathPair;
        private int m_SearchResultSelected;

        private Vector2 m_SearchResultViewPos = Vector2.zero;
        private Vector2 m_DocumentViewPos = Vector2.zero;

        private IEnumerable<IDocumentVo> m_TargetDbContents;

        [MenuItem(ViewerWindowLabel.MenuItemAttr)]
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
                    m_DraftXmlConnector = new DraftXmlDao(AssetDatabase.GetAssetPath(m_DocumentDatabaseObject));
                    m_TargetDbContents = m_DraftXmlConnector.GetAll();
                }
                catch
                {
                    Debug.LogWarning("正しい形式のデータベースを読み込んでください");
                    m_DocumentDatabaseObject = null;
                    return;
                }
                

                m_NameSearchBoxText = EditorGUILayout.TextField(ViewerWindowLabel.NameSearchBox, m_NameSearchBoxText);
                m_GuidSearchBoxText = EditorGUILayout.TextField(ViewerWindowLabel.GuidSearchBox, m_GuidSearchBoxText);

                using (new EditorGUILayout.HorizontalScope())
                {
                    m_GuidAndPathPair = AssetDatabase.FindAssets(m_NameSearchBoxText)
                        .Where(guid =>
                        {
                            if (m_GuidSearchBoxText == "" || m_GuidSearchBoxText == null)
                            {
                                return true;
                            }
                            
                            return Regex.Match(guid, m_GuidSearchBoxText).Success;
                        })
                        .Distinct()
                        .ToDictionary(guid => guid, guid => AssetDatabase.GUIDToAssetPath(guid));
                    
                    
                    m_SearchResultViewPos = EditorGUILayout.BeginScrollView(m_SearchResultViewPos, GUI.skin.box);
                    {
                        EditorGUI.BeginChangeCheck();
                        m_SearchResultSelected = GUILayout.SelectionGrid(m_SearchResultSelected, m_GuidAndPathPair.Select(pair => pair.Value + ": " + pair.Key).ToArray(), 1, "PreferencesKeysElement");
                        if (EditorGUI.EndChangeCheck())
                        {
                            EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(m_GuidAndPathPair.ElementAt(m_SearchResultSelected).Value, typeof(Object)));
                        }
                    }
                    EditorGUILayout.EndScrollView();

                    var descriptionData = m_TargetDbContents.Where(data =>
                        m_GuidAndPathPair.Keys.ToArray()[m_SearchResultSelected] == data.Guid);
                    
                    if (!descriptionData.Any())
                    {
                        return;
                    }
                    
                    using (new EditorGUILayout.VerticalScope(GUI.skin.box))
                    {
                        GUILayout.Label(descriptionData.First().Description);
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