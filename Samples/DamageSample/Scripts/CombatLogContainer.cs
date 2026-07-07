using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    public class CombatLogContainer : MonoBehaviour
    {
        [SerializeField] private CombatLogEntry logPrefab;
      
        private void Start()
        {
            
#if HERMES_EVENTS_GENERATED
            FindAnyObjectByType<SceneGameEvents>().GameEventDispatcher.Test_OnDamageGameEvent.AddListener(OnDoDamage);
#endif
        }

        private void OnDestroy()
        {
            
#if HERMES_EVENTS_GENERATED
            FindAnyObjectByType<SceneGameEvents>().GameEventDispatcher.Test_OnDamageGameEvent.RemoveListener(OnDoDamage);
#endif
        }
        
#if HERMES_EVENTS_GENERATED
        private void OnDoDamage(Test_OnDamageEventArgs args)
        {
            var log = Instantiate(logPrefab, transform);
            log.Create(args.from as Player, args.to as Player, args.damage);
            log.transform.SetSiblingIndex(0);
        }
#endif
    }
}