using System;
using System.Collections.Generic;
using Shared.Utils.Logging;

namespace Shared.Utils
{
    public class HandlerData
    {
        public int From;
        public int To;
        public Action Method;
    }
    public class StateSwitcher<T> where T : struct, IConvertible
    {
        private static readonly Log log = new(nameof(StateSwitcher<T>));

        private int _lastState;
        private int _currentState;

        public T LastState => _enumVals[_lastState];
        public T CurrentState => _enumVals[_currentState];

        private readonly T[] _enumVals;
        private readonly Dictionary<int, HandlerData> _handlers = new Dictionary<int, HandlerData>();
        private readonly Action[] _processors;
        private readonly Action[] _singleHandlers;
        private readonly bool _notFoundProcessorLog, _notFoundHandlerLog;
        
        private Action<T> _stateSwitchHandler;
        
        public StateSwitcher(T startState,  bool notFoundProcessorLog = false,  bool notFoundHandlerLog = false)
        {
            _notFoundProcessorLog = notFoundProcessorLog;
            _notFoundHandlerLog = notFoundHandlerLog;
            
            if (!typeof(T).IsEnum) 
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            _enumVals = (T[]) Enum.GetValues(typeof(T));
            _processors = new Action[_enumVals.Length];
            _singleHandlers = new Action[_enumVals.Length];
            
            var startStateInt = startState.ToInt32(null);
            _lastState = startStateInt;
            _currentState = startStateInt;
        }

        public void SetStateSwitchHandler(Action<T> stateSwitchHandler)
        {
            _stateSwitchHandler = stateSwitchHandler;
            _stateSwitchHandler.Invoke(_enumVals[_currentState]);
        }

        public void SwitchStateIfAnother(T s)
        {
            var state = s.ToInt32(null);
            if (_currentState == state)
            {
                return;
            }

            SwitchState(s);
        }
        
        public void SwitchState(T s)
        {
            var state = s.ToInt32(null);
            if (_currentState == state)
            {
                log.Warn($"Current state already is {state}");
                return;
            }
            
            InternalSetState(state);

            ProcessHandler();
            ProcessSingleHandler();
            _processors[_currentState]?.Invoke();

            _stateSwitchHandler?.Invoke(s);
        }

        private void InternalSetState(int state)
        {
            _lastState = _currentState;
            _currentState = state;
        }

        public void SetHandler(T switchingTo, Action method)
        {
            var to = switchingTo.ToInt32(null);
            _singleHandlers[to] = method;
            if (_currentState == to)
            {
                method();
            }
        }
        
        public void SetHandler(T switchingFrom, T switchingTo, Action method)
        {
            var from = switchingFrom.ToInt32(null);
            var to = switchingTo.ToInt32(null);
            var key = GetKey(from, to);
            if (_handlers.ContainsKey(key))
            {
                log.Warn($"Setting handler. From: {switchingFrom}, To: {switchingTo}. Have key already: {key}, but set although");
            }
            _handlers[key] = new HandlerData
            {
                From = from, 
                To = to, 
                Method = method,
            };
        }

        public void SetProcessor(T processingState, Action method)
        {
            var processingStateInt = processingState.ToInt32(null);
            _processors[processingStateInt] = method;
        }
        
        public void Process()
        {
            var method = _processors[_currentState];
            if (method == null)
            {
                if (_notFoundProcessorLog)
                {
                    log.Debug($"Processor not found. Current: {CurrentState}.");    
                }
                return;
            }

            try
            {
                method.Invoke();
            }
            catch (Exception)
            {
                InternalSetState(0);
                throw;
            }
            
        }

        private void ProcessHandler()
        {
            var key = GetKey(_lastState, _currentState);
            _handlers.TryGetValue(key, out var val);
            if (val == null)
            {
                if (_notFoundHandlerLog)
                {
                    log.Debug($"Handler not found. Last: {LastState}, Current: {CurrentState}. Key: {key}");    
                }
                return;
            }
            val.Method.Invoke();
        }

        private void ProcessSingleHandler()
        {
            var method = _singleHandlers[_currentState];
            if (method == null)
            {
                if (_notFoundHandlerLog)
                {
                    log.Debug($"Handler not found. Current: {CurrentState}.");    
                }
                return;
            } 
            method.Invoke();
        }


        //https://math.stackexchange.com/a/2365780
        private int GetKey(int from, int to)
        {
            return from >= to ? from * from + from + to : from + to * to;
        }
    }
}