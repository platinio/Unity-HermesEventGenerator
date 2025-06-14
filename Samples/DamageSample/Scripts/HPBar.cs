using ArcaneOnyx.ServiceLocator;
using UnityEngine;
using UnityEngine.UI;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    [DefaultExecutionOrder(int.MaxValue)]
    public class HPBar : MonoBehaviour
    {
        [SerializeField] private Player owner;
        [SerializeField] private Image bar;

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
            //if this damage event is not for me ignore
            if (args.to != owner) return;

            bar.fillAmount = owner.CurrentHP / (float) owner.MaxHP;
        }
#endif
    }
}