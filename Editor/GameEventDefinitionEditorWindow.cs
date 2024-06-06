using System.Collections.Generic;
using System.IO;
using Platinio.ScriptableObjectDatabase;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Platinio.GameEventGenerator.Editor
{
    public class GameEventDefinitionEditorWindow : DatabaseEditorWindow<GameEventDefinitionDatabase, GameEventDefinition>
    {
        public TextAsset GameEventDispatcherExtensionTemplate;
        public TextAsset GameEventDispatcherTemplate;
        public TextAsset GameEvenTypeEnumTemplate;
        public TextAsset GameEventTriggerTemplate;
        public TextAsset VisualScriptEventUnitTemplate;
        public TextAsset GameAddListenerTemplate;
        public TextAsset GameRemoveListenerTemplate;
        public TextAsset GameEventListenerMethodTemplate;
        public TextAsset ScriptGraphContainerTemplate;
     

        private readonly string GenerateFolderPath = $"{Path.GetDirectoryName(Application.dataPath)}\\Assets\\Platinio.GameEvents.Generated";
        
        [MenuItem("Window/RPG Editors/GameEvent Definition Editor Window")]
        public static void OpenSkillEditor()
        {
            GameEventDefinitionEditorWindow wnd = GetWindow<GameEventDefinitionEditorWindow>();
            wnd.titleContent = new GUIContent(wnd.GetWindowTitle());
        }
        
        public override string GetWindowTitle() => "Game Event Definition Editor";

        protected override IReadOnlyList<GameEventDefinition> FilterEntries(IReadOnlyList<GameEventDefinition> entries) => entries;

        protected override void SetupToolBar(ToolbarMenu toolbarMenu)
        {
            toolbarMenu.menu.AppendAction("Create New Event", CreateNewEntry);
            toolbarMenu.menu.AppendAction("Regenerate Events", RegenerateEvents);
            toolbarMenu.menu.AppendAction("Duplicate Selected Event", DuplicateEntry);
            toolbarMenu.menu.AppendAction("Remove Selected Event", RemoveEntry);
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

            if (Directory.Exists(GenerateFolderPath))
            {
                Directory.Delete(GenerateFolderPath, true);
            }
            
            Directory.CreateDirectory(GenerateFolderPath);
            
            var gameEventDefinitions = GetFilteredEntries();

            foreach (var gameEventDefinition in gameEventDefinitions)
            {
                CreateDispatcherExtension(gameEventDefinition);
                CreateEventTrigger(gameEventDefinition);
                CreateVisualScriptEventUnit(gameEventDefinition);
            }
            
            CreateDispatcher(gameEventDefinitions);
            CreateGameEventEnum(gameEventDefinitions);
            CreateVisualScriptingEventListener(gameEventDefinitions);
            
            AssetDatabase.Refresh();
        }

        private void CreateVisualScriptEventUnit(GameEventDefinition gameEventDefinition)
        {
            string eventName = gameEventDefinition.name;

            string scriptTemplate = VisualScriptEventUnitTemplate.text;
            string script = string.Format(scriptTemplate, eventName);

            string path = $"{GenerateFolderPath}\\{gameEventDefinition.name}GameEventUnit.cs";
            File.WriteAllText(path, script);
        }

        private void CreateDispatcherExtension(GameEventDefinition gameEventDefinition)
        {
            string eventName = gameEventDefinition.name;
            string eventArgsSignature = gameEventDefinition.GetArgsSignature();
            string eventArgsDeclaration = gameEventDefinition.GetArgsDeclaration();
            string eventArgsNameSignature = gameEventDefinition.GetArgsNameSignature();
            string eventArgsAssignment  = gameEventDefinition.GetArgsAssignment();
            string namespaces  = gameEventDefinition.GetNamespaces();

            string scriptTemplate = GameEventDispatcherExtensionTemplate.text;
            string script = string.Format(scriptTemplate, eventName, eventArgsSignature, eventArgsDeclaration, eventArgsNameSignature, eventArgsAssignment, namespaces);

            string path = $"{GenerateFolderPath}\\GameEventDispatcher.{gameEventDefinition.name}.cs";
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

            string path = $"{GenerateFolderPath}\\{gameEventDefinition.name}GameEventTrigger.cs";
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
            string fullPath = $"{GenerateFolderPath}\\GameEventDispatcherExtension.cs";
          
            File.WriteAllText( fullPath, script );
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
            string fullPath = $"{GenerateFolderPath}\\GameEventTypeEnum.cs";
          
            File.WriteAllText( fullPath, script );
        }

        private void CreateVisualScriptingEventListener(IReadOnlyList<GameEventDefinition> gameEventDefinitions)
        {
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
            
            string fullPath = $"{GenerateFolderPath}\\ScriptGraphContainer.GameEvents.cs";
          
            File.WriteAllText( fullPath, scriptGraphContainer );
            
        }
    }
}