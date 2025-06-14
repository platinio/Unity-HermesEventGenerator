using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace ArcaneOnyx.GameEventGenerator
{
    public class GameEventDefinitionEditorWindow : DatabaseEditorWindow<GameEventDefinitionDatabase, GameEventDefinition>
    {
        public TextAsset GameEventDispatcherExtensionTemplate;
        public TextAsset GameEventArgsTemplate;
        public TextAsset GameEventDispatcherTemplate;
        public TextAsset GameEvenTypeEnumTemplate;
        public TextAsset GameEventTriggerTemplate;
        public TextAsset VisualScriptEventUnitTemplate;
        public TextAsset GameAddListenerTemplate;
        public TextAsset GameRemoveListenerTemplate;
        public TextAsset GameEventListenerMethodTemplate;
        public TextAsset ScriptGraphContainerTemplate;

        //event args dependencies
        public TextAsset GameEventArgsBase;

        private List<NamedBuildTarget> AllBuildTargets = new()
        {
            NamedBuildTarget.Standalone,
            NamedBuildTarget.Server,
            NamedBuildTarget.iOS,
            NamedBuildTarget.Android,
            NamedBuildTarget.WebGL,
            NamedBuildTarget.WindowsStoreApps,
            NamedBuildTarget.PS4,
            NamedBuildTarget.PS5,
            NamedBuildTarget.XboxOne,
            NamedBuildTarget.tvOS,
            NamedBuildTarget.VisionOS,
            NamedBuildTarget.NintendoSwitch,
            NamedBuildTarget.LinuxHeadlessSimulation,
            NamedBuildTarget.EmbeddedLinux,
            NamedBuildTarget.QNX
        };
        
        public List<TextAsset> EventArgsDependencies => new()
        {
            GameEventArgsBase
        };

        private const string EVENT_DEFINE_SYMBOL = "HERMES_EVENTS_GENERATED";

        //events dependencies
        public TextAsset EventTriggerBase;
        public TextAsset GameEventDispatcher;
        public TextAsset ISceneGameEvents;
        public TextAsset ScriptGraphContainer;
        
        public List<TextAsset> EventDependencies => new()
        {
            EventTriggerBase,
            GameEventDispatcher,
            ISceneGameEvents,
            ScriptGraphContainer
        };

        public AssemblyDefinitionAsset EventsAssemblyDefinitionTemplate;
        public AssemblyDefinitionAsset EventArgsAssemblyDefinitionTemplate;
       
        private readonly string BaseGenerationPath = $"{Path.GetDirectoryName(Application.dataPath)}";
        
        [MenuItem("Window/General/Hermes Editor")]
        public static void OpenEditor()
        {
            GameEventDefinitionEditorWindow wnd = GetWindow<GameEventDefinitionEditorWindow>();
            wnd.titleContent = new GUIContent(wnd.GetWindowTitle());
        }
        
        public override string GetWindowTitle() => "Hermes Editor";

        protected override IReadOnlyList<GameEventDefinition> FilterEntries(IReadOnlyList<GameEventDefinition> entries) => entries;

        protected override void SetupToolBar(ToolbarMenu toolbarMenu)
        {
            AddCreateItemOptions(toolbarMenu);
            toolbarMenu.menu.AppendAction("Regenerate Events", RegenerateEvents);
            toolbarMenu.menu.AppendAction("Duplicate Selected Item", DuplicateEntry);
            toolbarMenu.menu.AppendAction("Remove Selected Item", RemoveEntry);
            toolbarMenu.menu.AppendAction("Move Up", (_) => MoveSelectedItem(-1));
            toolbarMenu.menu.AppendAction("Move Down", (_) => MoveSelectedItem(1));
            toolbarMenu.menu.AppendAction("Save", Save);
        }

        private void RegenerateEvents(DropdownMenuAction menuAction)
        {
            if (EditorApplication.isPlaying)
            {
                Debug.LogError("Can't regenerate events while playing");
                return;
            }
           
            DeleteEventGenerationCode();
            CreateEmptyFolders();
            
            var gameEventDefinitions = GetFilteredEntries();

            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                CreateDispatcherExtension(gameEventDefinition);
                CreateEventArgs(gameEventDefinition);
                CreateEventTrigger(gameEventDefinition);
                CreateVisualScriptEventUnit(gameEventDefinition);
            }
            
            CreateDispatcher(gameEventDefinitions);
            CreateGameEventEnum(gameEventDefinitions);
            CreateVisualScriptingEventListener(gameEventDefinitions);
            
            MoveEventScripts();
            MoveEventArgsScripts();
            RegenrateAssemblyDefinitions();
            UpdateScriptingDefineSymbols();
            
            AssetDatabase.Refresh();
        }

        private void UpdateScriptingDefineSymbols()
        {
            foreach (var buildTarget in AllBuildTargets)
            {
                if (DefineSymbolAlreadyExist(buildTarget, EVENT_DEFINE_SYMBOL)) continue;
                
                PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);
                
                string[] newDefines = new string[defines.Length + 1];
                Array.Copy(defines, newDefines, defines.Length);
                newDefines[newDefines.Length - 1] = EVENT_DEFINE_SYMBOL;
                
                PlayerSettings.SetScriptingDefineSymbols(buildTarget, newDefines);
            }
        }

        private bool DefineSymbolAlreadyExist(NamedBuildTarget buildTarget, string defineSymbol)
        {
            PlayerSettings.GetScriptingDefineSymbols(buildTarget, out string[] defines);

            foreach (var define in defines)
            {
                if (define == defineSymbol) return true;
            }

            return false;
        }

        private void DeleteEventGenerationCode()
        {
            var hermesSettings = HermesSettings.GetOrCreateSettings();

            if (Directory.Exists($"{BaseGenerationPath}\\{hermesSettings.ArgumentsModulePath}"))
            {
                Directory.Delete($"{BaseGenerationPath}\\{hermesSettings.ArgumentsModulePath}", true);
            }
            
            if (Directory.Exists($"{BaseGenerationPath}\\{hermesSettings.EventsModulePath}"))
            {
                Directory.Delete($"{BaseGenerationPath}\\{hermesSettings.EventsModulePath}", true);
            }
        }

        private void CreateEmptyFolders()
        {
            var hermesSettings = HermesSettings.GetOrCreateSettings();
            
            Directory.CreateDirectory($"{BaseGenerationPath}\\{hermesSettings.ArgumentsModulePath}");
            Directory.CreateDirectory($"{BaseGenerationPath}\\{hermesSettings.EventsModulePath}");
        }

        private void CreateVisualScriptEventUnit(GameEventDefinition gameEventDefinition)
        {
            var hermerSettings = HermesSettings.GetOrCreateSettings();
            if (!hermerSettings.UseVisualScripting) return;
            
            string eventName = gameEventDefinition.name;

            string scriptTemplate = VisualScriptEventUnitTemplate.text;
            string script = string.Format(scriptTemplate, eventName);
          
            string path = $"{BaseGenerationPath}\\{hermerSettings.EventsModulePath}\\{gameEventDefinition.name}GameEventUnit.cs";
            File.WriteAllText(path, script);
        }

        private void CreateDispatcherExtension(GameEventDefinition gameEventDefinition)
        {
            string eventName = gameEventDefinition.name;
            string eventArgsSignature = gameEventDefinition.GetArgsSignature();
            string eventArgsNameSignature = gameEventDefinition.GetArgsNameSignature();
            string namespaces  = gameEventDefinition.GetNamespaces();

            string scriptTemplate = GameEventDispatcherExtensionTemplate.text;
            string script = string.Format(scriptTemplate, eventName, eventArgsSignature, eventArgsNameSignature, namespaces);

            string path = $"{BaseGenerationPath}\\{HermesSettings.GetOrCreateSettings().EventsModulePath}\\GameEventDispatcher.{gameEventDefinition.name}.cs";
            File.WriteAllText(path, script);
        }

        private void CreateEventArgs(GameEventDefinition gameEventDefinition)
        {
            string eventName = gameEventDefinition.name;
            string eventArgsSignature = gameEventDefinition.GetArgsSignature();
            string eventArgsDeclaration = gameEventDefinition.GetArgsDeclaration();
            string eventArgsAssignment  = gameEventDefinition.GetArgsAssignment();
            string namespaces  = gameEventDefinition.GetNamespaces();

            string scriptTemplate = GameEventArgsTemplate.text;
            string script = string.Format(scriptTemplate, eventName, eventArgsSignature, eventArgsDeclaration, eventArgsAssignment, namespaces);
           
            string path = $"{BaseGenerationPath}\\{HermesSettings.GetOrCreateSettings().ArgumentsModulePath}\\{gameEventDefinition.name}EventArgs.cs";
            File.WriteAllText(path, script);
        }

        private void CreateEventTrigger(GameEventDefinition gameEventDefinition)
        {
            string eventName = gameEventDefinition.name;
            string argsDeclaration = gameEventDefinition.GetArgsDeclaration();

            argsDeclaration = argsDeclaration.Replace("IEnumerable", "List");
            
            string eventArgsNameSignature = gameEventDefinition.GetArgsNameSignatureEVentTrigger();
            string namespaces  = gameEventDefinition.GetNamespaces();

            string scriptTemplate = GameEventTriggerTemplate.text;
            string script = string.Format(scriptTemplate, eventName, argsDeclaration, eventArgsNameSignature, namespaces);

            string path = $"{BaseGenerationPath}\\{HermesSettings.GetOrCreateSettings().EventsModulePath}\\{gameEventDefinition.name}GameEventTrigger.cs";
            File.WriteAllText(path, script);
        }

        private void CreateDispatcher(IReadOnlyList<GameEventDefinition> gameEventDefinitions)
        {
            string scriptTemplate = GameEventDispatcherTemplate.text;
            string awakeInitialization = string.Empty;

            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                awakeInitialization += $"{gameEventDefinition.name}GameEvent = new {gameEventDefinition.name}Event(this);\n";
            }

            string script = string.Format(scriptTemplate, awakeInitialization);
            string path = $"{BaseGenerationPath}\\{HermesSettings.GetOrCreateSettings().EventsModulePath}\\GameEventDispatcherExtension.cs";
          
            File.WriteAllText( path, script );
        }
        
        private void CreateGameEventEnum(IReadOnlyList<GameEventDefinition> gameEventDefinitions)
        {
            string scriptTemplate = GameEvenTypeEnumTemplate.text;
            string enumValues = string.Empty;

            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                enumValues += $"{gameEventDefinition.name},\n";
            }

            string script = string.Format(scriptTemplate, enumValues);
            string path = $"{BaseGenerationPath}\\{HermesSettings.GetOrCreateSettings().EventsModulePath}\\GameEventTypeEnum.cs";
          
            File.WriteAllText(path, script);
        }

        private void CreateVisualScriptingEventListener(IReadOnlyList<GameEventDefinition> gameEventDefinitions)
        {
            var hermesSettings = HermesSettings.GetOrCreateSettings();
            if (!hermesSettings.UseVisualScripting) return;
            
            string addListeners = string.Empty;
            
            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                string addListener = GameAddListenerTemplate.text;
                addListener = string.Format(addListener, gameEventDefinition.name);

                addListeners += addListener + "\n";
            }
            
            string removeListeners = string.Empty;
            
            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                string removeListener = GameAddListenerTemplate.text;
                removeListener = string.Format(removeListener, gameEventDefinition.name);

                removeListeners += removeListener + "\n";
            }

            string listenerMethods = string.Empty;

            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                string listenerMethod = GameEventListenerMethodTemplate.text;
                listenerMethod = string.Format(listenerMethod, gameEventDefinition.name);

                listenerMethods += listenerMethod + "\n";
            }

            string scriptGraphContainer = ScriptGraphContainerTemplate.text;
            scriptGraphContainer = string.Format(scriptGraphContainer, addListeners, removeListeners, listenerMethods);
           
            string path = $"{BaseGenerationPath}\\{hermesSettings.EventsModulePath}\\ScriptGraphContainer.GameEvents.cs";
            File.WriteAllText(path, scriptGraphContainer);
        }

        private void MoveEventArgsScripts()
        {
            foreach (var script in EventArgsDependencies)
            {
                string path = $"{HermesSettings.GetOrCreateSettings().ArgumentsModulePath}/{script.name}.cs".Replace("/", "\\");
                
                using (FileStream fs = File.Create(path))
                {
                    string newScript = script.text;

                    if (newScript.Contains($"#if !{EVENT_DEFINE_SYMBOL}"))
                    {
                        string conditionalEnd = "#endif";
                        newScript = newScript.Replace($"#if !{EVENT_DEFINE_SYMBOL}", string.Empty);
                        int index = newScript.IndexOf(conditionalEnd, StringComparison.Ordinal);
                        if (index >= 0)
                        {
                            newScript = newScript.Remove(index, conditionalEnd.Length);
                        }
                    }
                   
                    Byte[] content = new UTF8Encoding(true).GetBytes(newScript);
                    fs.Write(content, 0, newScript.Length);
                }
                
                string currentPath = AssetDatabase.GetAssetPath(script).Replace("/", "\\");
                string text = File.ReadAllText(currentPath);

                if (!text.Contains($"#if !{EVENT_DEFINE_SYMBOL}"))
                {
                    text = $"#if !{EVENT_DEFINE_SYMBOL}\n{text}\n#endif";
                }
                
                File.WriteAllText(currentPath, text);
            }
        }
        
        private void MoveEventScripts()
        {
            foreach (var script in EventDependencies)
            {
                //skip visual scripting if visual scripting is disabled
                if (script == ScriptGraphContainer && 
                    !HermesSettings.GetOrCreateSettings().UseVisualScripting)
                {
                    continue;
                }

                string path = $"{HermesSettings.GetOrCreateSettings().EventsModulePath}/{script.name}.cs".Replace("/", "\\");
                
                using (FileStream fs = File.Create(path))
                {
                    string newScript = script.text;

                    if (newScript.Contains($"#if !{EVENT_DEFINE_SYMBOL}"))
                    {
                        string conditionalEnd = "#endif";
                        newScript = newScript.Replace($"#if !{EVENT_DEFINE_SYMBOL}", string.Empty);
                        int index = newScript.IndexOf(conditionalEnd, StringComparison.Ordinal);
                        if (index >= 0)
                        {
                            newScript = newScript.Remove(index, conditionalEnd.Length);
                        }
                    }
                 
                    Byte[] content = new UTF8Encoding(true).GetBytes(newScript);
                    fs.Write(content, 0, newScript.Length);
                }
                
                string currentPath = AssetDatabase.GetAssetPath(script).Replace("/", "\\");
                string text = File.ReadAllText(currentPath);

                if (!text.Contains($"#if !{EVENT_DEFINE_SYMBOL}"))
                {
                    text = $"#if !{EVENT_DEFINE_SYMBOL}\n{text}\n#endif";
                }
               
                File.WriteAllText(currentPath, text);
            }
        }

        private void RegenrateAssemblyDefinitions()
        {
            var hermesSettings = HermesSettings.GetOrCreateSettings();
            
            string eventPath = $"{hermesSettings.EventsModulePath}/Hermes.Events.asmdef".Replace("/", "\\");
            using (FileStream fs = File.Create(eventPath))
            {
                string assemblyContent = EventsAssemblyDefinitionTemplate.text.Replace("Hermes.Events.Template", "Hermes.Events");
                Byte[] content = new UTF8Encoding(true).GetBytes(assemblyContent);
                fs.Write(content, 0, assemblyContent.Length);
            }

            string eventArgsPath = $"{hermesSettings.ArgumentsModulePath}/Hermes.EventArgs.asmdef".Replace("/", "\\");
            using (FileStream fs = File.Create(eventArgsPath))
            {
                string assemblyContent = EventArgsAssemblyDefinitionTemplate.text.Replace("Hermes.EventArgs.Template", "Hermes.EventArgs");
                Byte[] content = new UTF8Encoding(true).GetBytes(assemblyContent);
                fs.Write(content, 0, assemblyContent.Length);
            }
        }
    }
}