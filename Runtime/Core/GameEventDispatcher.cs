using System;
using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    public partial class GameEventDispatcher : MonoBehaviour
    {
        public event Action<string, GameEventArgsBase> OnGameEventRaisedEvent;

        public void OnGameEventRaised<T>(string gameEventName, T args) where T: GameEventArgsBase
        {
            OnGameEventRaisedEvent?.Invoke(gameEventName, args);
        }
    }

    [Serializable]
    public class GameEventArgsBase
    {
    }
}