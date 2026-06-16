using UnityEngine;
using Zenject;

namespace ArcaneOnyx.GameEventGenerator
{
    public class UnityEvents : MonoBehaviour
    {
        #if HERMES_EVENTS_GENERATED
        [Inject] 
        private ISceneGameEvents sceneGameEvents;
        
        private void OnApplicationQuit()
        {
            sceneGameEvents?.GameEventDispatcher.OnApplicationQuitGameEvent.Raise();
        }
        #endif
    }
}

