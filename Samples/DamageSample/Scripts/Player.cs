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
#if HERMES_EVENTS_GENERATED
            var sceneGameEvents = FindAnyObjectByType<SceneGameEvents>();
            if (sceneGameEvents == null) return;
            
            FindAnyObjectByType<SceneGameEvents>().GameEventDispatcher.Test_OnDamageGameEvent.AddListener(OnDoDamage);
#endif
        }

        private void OnDestroy()
        {
#if HERMES_EVENTS_GENERATED
            var sceneGameEvents = FindAnyObjectByType<SceneGameEvents>();
            if (sceneGameEvents == null) return;
            
            sceneGameEvents.GameEventDispatcher.Test_OnDamageGameEvent.RemoveListener(OnDoDamage);
#endif
        }

#if HERMES_EVENTS_GENERATED
        public void OnDoDamage(Test_OnDamageEventArgs args)
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
#endif
    }
}

