using System;
using System.Collections.Generic;

namespace CycladeUI.Utils
{
    public class SubscriptionHandlers
    {
        private readonly HashSet<Action> _handlers = new();

        public void SubscribeOnce(Action action, bool callOnSub = false)
        {
            Clear();
            Subscribe(action, callOnSub);
        }
        
        public void Subscribe(Action action, bool callOnSub = false)
        {
            _handlers.Add(action);
            if (callOnSub)
                action.Invoke();
        }

        public void Unsubscribe(Action action) => _handlers.Remove(action);

        public void InvokeAll()
        {
            foreach (var action in _handlers)
                action.Invoke();
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}