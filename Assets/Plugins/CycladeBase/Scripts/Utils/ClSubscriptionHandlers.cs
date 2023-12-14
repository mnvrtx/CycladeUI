using System;
using System.Collections.Generic;
using CycladeBase.Utils.Logging;

namespace CycladeBase.Utils
{
    public class ClSubscriptionHandlers
    {
        private static readonly Log log = new(nameof(ClSubscriptionHandlers));

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
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    log.Exception(e);
                }
            }
        }

        public void Unsubscribe(Action action) => _handlers.Remove(action);

        public void InvokeAll()
        {
            foreach (var action in _handlers)
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    log.Exception(e);
                }
            }
        }

        public void Clear()
        {
            _handlers.Clear();
        }
    }
}