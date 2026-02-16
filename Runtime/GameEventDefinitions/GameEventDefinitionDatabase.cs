using ArcaneOnyx.ScriptableObjectDatabase;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    [CreateAssetMenu(menuName = "Database/Game Event Definiton")]
    public class GameEventDefinitionDatabase : ScriptableDatabase<GameEventDefinition>
    {
        [SerializeField] private bool silenced = false;

        public bool Silenced => silenced;
    }
}