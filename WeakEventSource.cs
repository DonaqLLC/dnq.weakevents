using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNQ.Events
{
    public abstract class WeakEventSource
        : DNQ.Events.IWeakEventSource
    {
        private static int _sourceSequenceNumber = 0;
        static WeakEventSource()
        {
            
        }

        private readonly string[] _weakEvents;
        private readonly List<System.WeakReference>[] _listeners;
        private readonly int _sourceID;

        private readonly Action<int, string> _logger;

        protected WeakEventSource(string[] events, Action<int, string> logger)
        {            
            _sourceID = System.Threading.Interlocked.Increment(ref _sourceSequenceNumber);            
            _weakEvents = (string[])events.Clone();
            _logger = logger;

            _listeners = new List<WeakReference>[events.Length];
            for (int i = 0; i < events.Length; i++)
            {
                _listeners[i] = new List<WeakReference>();
            }
        }

        protected WeakEventSource(string[] events)
            : this(events, null)
        {
        }

        public IEnumerable<string> EnumerateWeakEvents()
        {
            return _weakEvents;
        }
        
        public virtual void AttachEvent(string eventName, IWeakEventTarget target)
        {
            bool eventFound = false;
            if (target != null)
            {                
                for (int i = 0; i < _weakEvents.Length; i++)
                {
                    if (string.Compare(eventName, _weakEvents[i], true) == 0)
                    {
                        eventFound = true;
                        if (HasTarget(_listeners[i], target) == null)
                        {                            
                            _listeners[i].Add(new WeakReference(target));
                            if (_logger != null)
                            {
                                _logger(5, string.Format("WEAK_EVT_SRC {0}. Attmept to attach listener for event {1}... OK [target added]; Target ID = {2}", _sourceID, eventName, target.TargetFriendlyID));
                            }
                        }
                        else
                        {
                            if (_logger != null)
                            {
                                _logger(4, string.Format("WEAK_EVT_SRC {0}: Attmept to attach listener for event {1}... IGNORED [target already listening]; Target ID = {2}", _sourceID, eventName, target.TargetFriendlyID));
                            }
                        }
                        break;
                    }
                }

                if (!eventFound && _logger != null)
                {
                    _logger(2, string.Format("WEAK_EVT_SRC {0}: Attmept to attach listener for event {1}... FAILED [no such event]; Target ID = {2}", _sourceID, eventName, target.TargetFriendlyID));
                }
            }            
        }

        private WeakReference HasTarget(IList<WeakReference> listeners, IWeakEventTarget target)
        {
            WeakReference r = null;
            for (int i = listeners.Count - 1; i >= 0; i--)
            {
                if (ReferenceEquals(listeners[i].Target, target))
                {
                    r = listeners[i];
                    break;
                }
                else if (!listeners[i].IsAlive || listeners[i].Target == null)
                {
                    listeners.RemoveAt(i);
                }
            }
            return r;
        }

        public virtual void DetachEvent(string eventName, IWeakEventTarget target)
        {
            if (target != null)
            {
                int targetRemoved = 0;
                for (int i = 0; i < _weakEvents.Length; i++)
                {
                    if (string.Compare(eventName, _weakEvents[i], true) == 0)
                    {
                        WeakReference targetRef = null;
                        do
                        {
                            targetRef = HasTarget(_listeners[i], target);
                            if (targetRef != null)
                            {
                                _listeners[i].Remove(targetRef);
                                targetRemoved++;
                            }
                        } while (_listeners[i].Count > 0 && targetRef != null);
                        
                        break;
                    }
                }

                if (targetRemoved > 0)
                {
                    if (_logger != null)
                    {
                        _logger(4, string.Format("WEAK_EVT_SRC {0}: Attmept to detach listener for event {1}... OK [target removed {2} time(s)]; Target ID = {3}", _sourceID, eventName, targetRemoved, target.TargetFriendlyID));
                    }
                }
                else
                {
                    if (_logger != null)
                    {
                        _logger(2, string.Format("WEAK_EVT_SRC {0}: Attmept to detach listener for event {1}... FAILED [target did not exist or no such event]; Target ID = {2}", _sourceID, eventName, target.TargetFriendlyID));
                    }
                }
            }
        }        

        protected void InvokeListenersForWeakEvent(int eventIndex, WeakEventArgs args)
        {
            List<WeakReference> listeners = _listeners[eventIndex];
            if (listeners.Count > 0)
            {
                for (int i = listeners.Count - 1; i >= 0; i--)
                {
                    if (listeners[i].IsAlive && listeners[i].Target != null)
                    {
                        try
                        {
                            ((IWeakEventTarget)listeners[i].Target).WeakEventNotification(this, _weakEvents[eventIndex], args);
                        }
                        catch(Exception exc)
                        {
                            if (_logger != null)
                            {
                                _logger(1, string.Format("WEAK_EVT_SRC {0}: ERROR NOTIFYING TARGET ({1}) {2}; StackTrace: {3}", _sourceID, exc.GetType(), exc.Message, (exc.StackTrace != null ? exc.StackTrace.Replace("\r", "").Replace("\n", " ") : " -- STACK TRACE NOT AVAILABLE -- ")));                                
                            }
                        }
                    }
                    else
                    {
                        listeners.RemoveAt(i);
                    }
                }
            }
        }
    }
}
