using System;

namespace OodlesParty
{ 
    public static class TickManager
    {
        private static event Action OnTick;

        public static void Tick()
        {
            if (OnTick != null)
            {
                OnTick();
            }
        }

        public static void Register(Action tickFunc)
        {
            OnTick += tickFunc;
        }

        public static void UnRegister(Action tickFunc)
        {
            OnTick -= tickFunc;
        }
    }
}
