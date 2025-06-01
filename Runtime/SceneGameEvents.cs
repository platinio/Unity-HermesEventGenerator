using ArcaneOnyx.ServiceLocator;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public class SceneGameEvents : MonoBehaviour, ISceneGameEvents
    {
        private GameEventDispatcher gameEventDispatcher;
        public GameEventDispatcher GameEventDispatcher => gameEventDispatcher;

        private void Awake()
        {
            ServicesContainer.Register(typeof(ISceneGameEvents), this);
            gameEventDispatcher = GetComponent<GameEventDispatcher>();
        }

        private void OnDestroy()
        {
            ServicesContainer.Remove<ISceneGameEvents>();
        }
    }
}