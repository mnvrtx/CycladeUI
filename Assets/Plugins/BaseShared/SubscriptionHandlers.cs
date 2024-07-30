using System;
using System.Collections.Generic;
using BaseShared;
using Shared.Utils.Logging;

namespace Shared
{
    public class SubscriptionHandlers
    {
        private static readonly Log log = new(nameof(SubscriptionHandlers));

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
                action.TryCatch(log);
        }

        public void Unsubscribe(Action action) => _handlers.Remove(action);

        public void InvokeAll()
        {
            foreach (var action in _handlers) 
                action.TryCatch(log);
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}