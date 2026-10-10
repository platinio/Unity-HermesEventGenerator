using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using System.IO;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public class HermesSettings : ScriptableObject
    {
        [SerializeField] private string argumentsModulePath = "Assets/Hermes.Generated/Arguments";
        [SerializeField] private string eventsModulePath = "Assets/Hermes.Generated/Events";
        [SerializeField] private bool useVisualScripting = false;
        [Tooltip("The assemblies whose types your events carry. Both generated assemblies, Hermes.EventArgs and Hermes.Events, get a reference to each of them.")]
        [SerializeField] private List<AssemblyDefinitionAsset> extraAssemblyReferences = new();

        public string ArgumentsModulePath => argumentsModulePath;
        public string EventsModulePath => eventsModulePath;
        public bool UseVisualScripting => useVisualScripting;

        /// <summary>
        /// The names are read from the assembly definition files on every call, so a renamed assembly is followed.
        /// </summary>
        public IReadOnlyList<string> ReadExtraAssemblyReferenceNames()
        {
            var extraAssemblyReferenceNames = new List<string>();

            foreach (AssemblyDefinitionAsset assemblyDefinition in extraAssemblyReferences)
            {
                bool isEmptyEntry = assemblyDefinition == null;
                if (isEmptyEntry)
                {
                    continue;
                }

                extraAssemblyReferenceNames.Add(ProjectAssemblyDefinitions.ReadName(assemblyDefinition));
            }

            return extraAssemblyReferenceNames;
        }
       
        public static HermesSettings GetOrCreateSettings()
        {
            var guids = AssetDatabase.FindAssets($"t:{typeof(HermesSettings).Name}");
            HermesSettings settings = null;
            
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                settings = AssetDatabase.LoadAssetAtPath(path, typeof(HermesSettings)) as HermesSettings;
                if (settings == null) continue;

                return settings;
            }

           
            string directoyPath = "Assets/Editor";
            string fileName = "HermesSettings.asset";

            if (!Directory.Exists(directoyPath))
            {
                Directory.CreateDirectory(directoyPath);
            }
            
            settings = CreateInstance<HermesSettings>();
            AssetDatabase.CreateAsset(settings, $"{directoyPath}/{fileName}");
            AssetDatabase.SaveAssets();
            
            return settings;
        }
        
        public static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
    }
}