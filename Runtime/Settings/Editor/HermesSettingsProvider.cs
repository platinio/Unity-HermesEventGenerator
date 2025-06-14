using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public static class HermesSettingsProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Project Settings window.
            var provider = new SettingsProvider("Project/Hermes Settings", SettingsScope.Project)
            {
                // By default the last token of the path is used as display name if no label is provided.
                label = "Hermes Settings",
                // Create the SettingsProvider and initialize its drawing (IMGUI) function in place:
                guiHandler = (searchContext) =>
                {
                    var settings = HermesSettings.GetSerializedSettings();
                    var argumentsModulePathSP = settings.FindProperty("argumentsModulePath");
                    var eventsModulePathSP = settings.FindProperty("eventsModulePath");
                    
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(argumentsModulePathSP, new GUIContent("Arguments Module Path"));
                    GUI.enabled = true;
                    
                    if (GUILayout.Button("«", GUILayout.Width(50)))
                    {
                        string selectFolderPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
                        
                        if (selectFolderPath.StartsWith(Application.dataPath)) 
                        {
                            selectFolderPath = "Assets" + selectFolderPath.Substring(Application.dataPath.Length);
                        }

                        argumentsModulePathSP.stringValue = selectFolderPath;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.BeginHorizontal();
                    GUI.enabled = false;
                    EditorGUILayout.PropertyField(eventsModulePathSP, new GUIContent("Events Module Path"));
                    GUI.enabled = true;
                    if (GUILayout.Button("«", GUILayout.Width(50)))
                    {
                        string selectFolderPath = EditorUtility.OpenFolderPanel("Select Directory", "", "");
                        
                        if (selectFolderPath.StartsWith(Application.dataPath)) 
                        {
                            selectFolderPath = "Assets" + selectFolderPath.Substring(Application.dataPath.Length);
                        }

                        eventsModulePathSP.stringValue = selectFolderPath;
                    }
                    EditorGUILayout.EndHorizontal();
                    
                    EditorGUILayout.PropertyField(settings.FindProperty("useAssemblyDefinitions"));
                    EditorGUILayout.PropertyField(settings.FindProperty("useVisualScripting"));

                    settings.ApplyModifiedProperties();
                    
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Hermes", "Events" })
            };

            return provider;
        }
    }
}