using ArcaneOnyx.ServiceLocator;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public class SceneGameEvents : MonoBehaviour, ISceneGameEvents
    {
        [SerializeField] private GameEventDispatcher gameEventDispatcher;
        public GameEventDispatcher GameEventDispatcher => gameEventDispatcher;

        private void Awake()
        {
            ServicesContainer.Register(typeof(ISceneGameEvents), this);
        }

        private void OnDestroy()
        {
            ServicesContainer.Remove<ISceneGameEvents>();
        }
    }
}