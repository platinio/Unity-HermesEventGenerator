using ArcaneOnyx.ServiceLocator;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    public class CombarLogContainer : MonoBehaviour
    {
        [SerializeField] private CombatLogEntry logPrefab;
        
        private void Start()
        {
            var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.AddListener(OnDoDamage);
        }

        private void OnDestroy()
        {
            var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.RemoveListener(OnDoDamage);
        }
        
        private void OnDoDamage(OnDoDamageEventArgs args)
        {
            var log = Instantiate(logPrefab, transform);
            log.Create(args.from as Player, args.to as Player, args.damage);
            log.transform.SetSiblingIndex(0);
        }
    }
}