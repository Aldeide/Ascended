using System;

namespace Systems.Events
{
    public class EventBase<T> where T : EventArgs
    {
        public event EventHandler<T> EventHandler;

        public void Publish(T args)
        {
            EventHandler?.Invoke(this, args);
        }

        public void Subscribe(EventHandler<T> handler)
        {
            EventHandler += handler;
        }

        public void Unsubscribe(EventHandler<T> handler)
        {
            EventHandler -= handler;
        }
    }
}