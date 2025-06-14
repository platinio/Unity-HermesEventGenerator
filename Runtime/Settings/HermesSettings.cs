#if UNITY_EDITOR
using UnityEditor;
#endif

using System.IO;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public class HermesSettings : ScriptableObject
    {
        [SerializeField] private string argumentsModulePath = "Assets/Hermes.Generated/Arguments";
        [SerializeField] private string eventsModulePath = "Assets/Hermes.Generated/Events";
        [SerializeField] private bool useVisualScripting = false;

        public string ArgumentsModulePath => argumentsModulePath;
        public string EventsModulePath => eventsModulePath;
        public bool UseVisualScripting => useVisualScripting;
        
        #if UNITY_EDITOR
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
        
        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }
        #endif
        
        
    }
}

