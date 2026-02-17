#if HERMES_EVENTS_GENERATED
using ArcaneOnyx.GameEventGenerator;
#endif
using UnityEngine;
using Zenject;

namespace ArcaneOnyx.EditorExtension
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

