using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    [RequireComponent(typeof(SceneGameEvents))]
    [DefaultExecutionOrder(int.MinValue)]
    public class SceneGameEvents : MonoBehaviour, ISceneGameEvents
    {
        [SerializeField] private GameEventDispatcher gameEventDispatcher;
        public GameEventDispatcher GameEventDispatcher => gameEventDispatcher;

        private void Awake()
        {
            gameEventDispatcher = GetComponent<GameEventDispatcher>();
        }
    }
}