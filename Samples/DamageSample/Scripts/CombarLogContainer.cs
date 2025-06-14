using ArcaneOnyx.ServiceLocator;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    public class CombarLogContainer : MonoBehaviour
    {
        [SerializeField] private CombatLogEntry logPrefab;
        
        private void Start()
        {
#if HERMES_EVENTS_GENERATED
            var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.AddListener(OnDoDamage);
#endif
        }

        private void OnDestroy()
        {
#if HERMES_EVENTS_GENERATED
            var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.RemoveListener(OnDoDamage);
#endif
        }
        
#if HERMES_EVENTS_GENERATED
        private void OnDoDamage(OnDoDamageEventArgs args)
        {
            var log = Instantiate(logPrefab, transform);
            log.Create(args.from as Player, args.to as Player, args.damage);
            log.transform.SetSiblingIndex(0);
        }
#endif
    }
}