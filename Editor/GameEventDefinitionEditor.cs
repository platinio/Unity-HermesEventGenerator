using System;
using UnityEditor;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    [CustomEditor(typeof(GameEventDefinition))]
    public class GameEventDefinitionEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
          
            if (!EditorApplication.isPlaying) return;
            if (Selection.activeGameObject == null) return;

            var sceneGameEvents = FindObjectOfType<SceneGameEvents>();
            if (sceneGameEvents == null) return;

            var gameEventDefinition = target as GameEventDefinition;
            var eventTriggerType = Type.GetType($"ArcaneOnyx.GameEventGenerator.{gameEventDefinition.name}GameEventTrigger, Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
          
            dynamic eventTrigger = sceneGameEvents.gameObject.GetComponent(eventTriggerType);
           
            if (eventTrigger == null)
            {
                if (GUILayout.Button("Add Event Trigger Component"))
                {
                    sceneGameEvents.gameObject.AddComponent(eventTriggerType);
                }
                
                return;
            }

            SerializedObject eventTriggerSO = new SerializedObject(eventTrigger as Component);

            EditorGUILayout.BeginVertical("window");
            
            var oldIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel++;
            
            EditorGUILayout.LabelField("Event Trigger", EditorStyles.boldLabel);
            
            foreach (var arg in gameEventDefinition.Args)
            {
                var eventArgsProperty = eventTriggerSO.FindProperty(arg.VariableName);
                if (eventArgsProperty == null) continue;
                
                EditorGUILayout.PropertyField(eventArgsProperty);
            }
            
            eventTriggerSO.ApplyModifiedProperties();
            EditorGUI.indentLevel = oldIndent;
            
            if (GUILayout.Button("Trigger Event"))
            {
                eventTrigger.Trigger();
            }
            
            EditorGUILayout.EndVertical();
        }
    }
}

