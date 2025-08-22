using ArcaneOnyx.ServiceLocator;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator.Samples
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private int HP;

        public int MaxHP { get; private set; }
        public int CurrentHP => HP;

        private void Start()
        {
            MaxHP = HP;

            //JAMES FIX EVENTS
#if HERMES_EVENTS_GENERATED
            //var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            //sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.AddListener(OnDoDamage);
#endif
        }

        private void OnDestroy()
        {
#if HERMES_EVENTS_GENERATED
            //var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            //sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.RemoveListener(OnDoDamage);
#endif
        }
/*
#if HERMES_EVENTS_GENERATED
        public void OnDoDamage(OnDoDamageEventArgs args)
        {
            //if this damage event is not for me ignore
            if (args.to != this) return;
            
            HP -= args.damage;
            if (HP <= 0)
            {
                HP = 0;
                Destroy(gameObject);
            }
        }
#endif*/
    }
}

