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
            
            var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            //sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.AddListener(OnDoDamage);
        }

        private void OnDestroy()
        {
            var sceneGameEvents = ServicesContainer.Resolve<ISceneGameEvents>();
            //sceneGameEvents?.GameEventDispatcher.OnDoDamageGameEvent.RemoveListener(OnDoDamage);
        }
        /*
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
        }*/
    }
}

