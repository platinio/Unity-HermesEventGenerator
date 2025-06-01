using TMPro;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    public class CombatLogEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI label;
        
        public void Create(Player attacker, Player victim, int damage)
        {
            label.text = $"{attacker.name} did {damage} damage to {victim.name}";
        }
    }
}