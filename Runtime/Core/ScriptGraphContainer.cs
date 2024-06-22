using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Platinio.GameEventGenerator
{
    public partial class ScriptGraphContainer : MonoBehaviour
    {
        [SerializeField] private ScriptMachine scriptMachinePrefab;
        
        private Dictionary<int, ScriptMachine> scripthMahcineDict = new();
        private int scriptMachineCount = 0;

        public Variables OwnerVariables { get; set; }

        public int AddScriptGraph(ScriptGraphAsset scriptGraphAsset)
        {
            var scriptMachine = Instantiate(scriptMachinePrefab, transform);
            scriptMachine.nest.SwitchToEmbed(scriptGraphAsset.graph);

            scriptMachineCount++;
            scripthMahcineDict[scriptMachineCount] = scriptMachine;

            return scriptMachineCount;
        }

        /// <summary>
        /// Removes all script machines uisng the specified script graph asset
        /// </summary>
        public void RemoveScriptMachines(ScriptGraphAsset scriptGraphAsset)
        {
            foreach (var scriptMachineKeyValuePair in scripthMahcineDict)
            {
                if (scriptMachineKeyValuePair.Value.nest.macro == scriptGraphAsset)
                {
                    scripthMahcineDict.Remove(scriptMachineKeyValuePair.Key);
                }
            }
        }

        public object GetOwnerVariable(string variableName)
        {
            return OwnerVariables.declarations.Get(variableName);
        }

        public void SetOwnerVariable(string variableName, object value)
        {
            OwnerVariables.declarations.Set(variableName, value);
        }

        public void RemoveScriptMachine(int scriptMachineId)
        {
            scripthMahcineDict.Remove(scriptMachineId);
        }
    }
}

