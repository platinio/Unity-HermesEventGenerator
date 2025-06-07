using UnityEngine;

namespace ArcaneOnyx.GameEventGenerator
{
    [System.Serializable]
    public class OnDoDamageEventArgs : GameEventArgsBase
    {        
        public Component from;
        public Component to;
        public int damage;


        public OnDoDamageEventArgs(Component from, Component to, int damage)
        {
            this.from = from;
            this.to = to;
            this.damage = damage;
        }
    }
}