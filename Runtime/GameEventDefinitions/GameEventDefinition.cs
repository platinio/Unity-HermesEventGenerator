using System.Collections.Generic;
using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public class GameEventDefinition : ScriptableItem
    {
        [SerializeField] [TextArea] private string description;
        [SerializeField] private List<VariableDefinition> args;
        [SerializeField] private List<string> namespaces;

        public IEnumerable<VariableDefinition> Args => args;
        public IEnumerable<string> Namespaces => namespaces;
        
        
        public string GetArgsSignature()
        {
            string eventArgsSignature = string.Empty;

            foreach (var variableDefinition in Args)
            {
                string variableName = variableDefinition.VariableName;
                variableName = $"{char.ToLower(variableName[0])}{variableName.Substring(1)}";
                
                eventArgsSignature += $"{variableDefinition.VariableType} {variableName}, ";
            }

            if (eventArgsSignature.Length > 0)
            {
                eventArgsSignature = eventArgsSignature.Remove(eventArgsSignature.Length - 2, 2);
            }
            return eventArgsSignature;
        }
        
        public string GetArgsDeclaration()
        {
            string eventArgsDeclaration = string.Empty;

            foreach (var variableDefinition in Args)
            {
                eventArgsDeclaration += $"public {variableDefinition.VariableType} {variableDefinition.VariableName};\n";
            }
           
            return eventArgsDeclaration;
        }
        
        public string GetArgsNameSignature()
        {
            string eventArgsNameSignature = string.Empty;

            foreach (var variableDefinition in Args)
            {
                string variableName = variableDefinition.VariableName;
                variableName = $"{char.ToLower(variableName[0])}{variableName.Substring(1)}";
                
                eventArgsNameSignature += $"{variableName}, ";
            }

            if (eventArgsNameSignature.Length > 0)
            {
                eventArgsNameSignature = eventArgsNameSignature.Remove(eventArgsNameSignature.Length - 2, 2);
            }
            
            return eventArgsNameSignature;
        }
        
        public string GetArgsNameSignatureEVentTrigger()
        {
            string eventArgsNameSignature = string.Empty;

            foreach (var variableDefinition in Args)
            {
                eventArgsNameSignature += $"{variableDefinition.VariableName}, ";
            }

            if (eventArgsNameSignature.Length > 0)
            {
                eventArgsNameSignature = eventArgsNameSignature.Remove(eventArgsNameSignature.Length - 2, 2);
            }
            
            return eventArgsNameSignature;
        }
        
        public string GetArgsAssignment ()
        {
            string eventArgsAssignment = string.Empty;

            foreach (var variableDefinition in Args)
            {
                string localVariableName = variableDefinition.VariableName;
                localVariableName = $"{char.ToLower(localVariableName[0])}{localVariableName.Substring(1)}";
                
                eventArgsAssignment += $"this.{variableDefinition.VariableName} = {localVariableName};\n";
            }
          
            return eventArgsAssignment;
        }
        
        public string GetNamespaces ()
        {
            string eventNamespaces = string.Empty;
            eventNamespaces += $"using System;\n";

            foreach (var namespaceStr in Namespaces)
            {
                if (namespaceStr == "System") continue;
                eventNamespaces += $"using {namespaceStr};\n";
            }
          
            return eventNamespaces;
        }
    }
    
    [System.Serializable]
    public class VariableDefinition
    {
        [SerializeField] private string variableType;
        [SerializeField] private string variableName;

        public string VariableType => variableType;
        public string VariableName => variableName;
    }
}

