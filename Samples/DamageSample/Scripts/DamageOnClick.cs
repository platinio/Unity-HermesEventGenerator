using UnityEngine;
using UnityEngine.InputSystem;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    // Click a Player in the game view to damage it. The clicked Player is the
    // target of the event ("to"); the OTHER referenced Player is the attacker
    // ("from"), and the damage is a random amount within the configured range.
    public class DamageOnClick : MonoBehaviour
    {
        [SerializeField] private Player player1;
        [SerializeField] private Player player2;
        [SerializeField] private int minDamage = 5;
        [SerializeField] private int maxDamage = 20;
        [SerializeField] private Camera raycastCamera;

#if HERMES_EVENTS_GENERATED
        private GameEventDispatcher dispatcher;

        private void Start()
        {
            dispatcher = FindAnyObjectByType<SceneGameEvents>().GameEventDispatcher;
            if (raycastCamera == null) raycastCamera = Camera.main;
        }

        private void Update()
        {
            // new Input System: react to a left click this frame
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

            Ray ray = raycastCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit)) return;

            Player target = hit.collider.GetComponentInParent<Player>();
            if (target == null) return;

            // whichever of the two players wasn't clicked deals the damage
            Player attacker = target == player1 ? player2 : player1;
            int damage = Random.Range(minDamage, maxDamage + 1);

            dispatcher.Test_OnDamageGameEvent.Raise(attacker, target, damage);
        }
#endif
    }
}
