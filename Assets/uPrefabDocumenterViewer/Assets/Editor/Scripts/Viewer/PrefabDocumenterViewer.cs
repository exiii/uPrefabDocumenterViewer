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

        private bool m_WriteMode;

        private Dictionary<string, string> m_GuidAndPathPair;
        private int m_SearchResultSelected;

        private Vector2 m_SearchResultValueViewPos = Vector2.zero;
        private Vector2 m_SearchResultKeyViewPos = Vector2.zero;
        private Vector2 m_DocumentViewPos = Vector2.zero;

        private string m_DescriptionTemp;
        
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
                    Debug.LogWarning("正しい形式のファイルを読み込んでください");
                    m_DocumentDatabaseObject = null;
                    return;
                }
                

                m_NameSearchBoxText = EditorGUILayout.TextField(ViewerWindowLabel.NameSearchBox, m_NameSearchBoxText);
                m_GuidSearchBoxText = EditorGUILayout.TextField(ViewerWindowLabel.GuidSearchBox, m_GuidSearchBoxText);

                m_WriteMode = EditorGUILayout.ToggleLeft(ViewerWindowLabel.WriteModeToggle, m_WriteMode);
                
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
                    
                    var descriptionData = m_TargetDbContents.Where(data =>
                        m_GuidAndPathPair.Keys.ToArray()[m_SearchResultSelected] == data.Guid);
                    
                    EditorGUI.BeginChangeCheck();

                    using (new EditorGUILayout.HorizontalScope(GUI.skin.box))
                    {

                        m_SearchResultValueViewPos = EditorGUILayout.BeginScrollView(m_SearchResultValueViewPos, GUI.skin.box);
                        {
                            m_SearchResultSelected = GUILayout.SelectionGrid(m_SearchResultSelected,
                                m_GuidAndPathPair.Select(pair => pair.Value).ToArray(), 1, "PreferencesKeysElement");
                        }
                        EditorGUILayout.EndScrollView();
                        m_SearchResultKeyViewPos = EditorGUILayout.BeginScrollView(m_SearchResultKeyViewPos, GUI.skin.box);
                        {
                            m_SearchResultSelected = GUILayout.SelectionGrid(m_SearchResultSelected,
                                m_GuidAndPathPair.Select(pair => pair.Key).ToArray(), 1, "PreferencesKeysElement");
                        }
                        EditorGUILayout.EndScrollView();

                    }

                    if (EditorGUI.EndChangeCheck())
                    {
                        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath(m_GuidAndPathPair.ElementAt(m_SearchResultSelected).Value, typeof(Object)));
                        if (descriptionData.Any())
                        {
                            m_DescriptionTemp = descriptionData.First().Description;
                        }
                        else
                        {
                            m_DescriptionTemp = "";
                        }
                    }
                    
                    using (new EditorGUILayout.VerticalScope())
                    {
                        m_DocumentViewPos = EditorGUILayout.BeginScrollView(m_DocumentViewPos, GUI.skin.box);
                        {
                            if (m_WriteMode)
                            {
                                m_DescriptionTemp = EditorGUILayout.TextArea(m_DescriptionTemp,
                                    GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
                            }
                            else
                            {
                                EditorGUILayout.LabelField(m_DescriptionTemp, GUILayout.ExpandHeight(true),
                                    GUILayout.ExpandWidth(true));
                            }
                        }
                        EditorGUILayout.EndScrollView();

                        if (GUILayout.Button(ViewerWindowLabel.ApplyButton))
                        {
                            
                        }
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