using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public class SceneGameEvents : MonoBehaviour, ISceneGameEvents
    {
        [SerializeField] private GameEventDispatcher gameEventDispatcher;
        public GameEventDispatcher GameEventDispatcher => gameEventDispatcher;
    }
}