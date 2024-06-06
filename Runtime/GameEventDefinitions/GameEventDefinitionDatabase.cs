using Platinio.ScriptableObjectDatabase;
using UnityEngine;

namespace Platinio.GameEventGenerator
{
    [CreateAssetMenu(menuName = "Database/Game Event Definiton")]
    public class GameEventDefinitionDatabase : ScriptableDatabase<GameEventDefinition> { }
}