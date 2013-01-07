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

        protected WeakEventSource(string[] events)
        {
            _sourceID = System.Threading.Interlocked.Increment(ref _sourceSequenceNumber);            
            _weakEvents = (string[])events.Clone();

            _listeners = new List<WeakReference>[events.Length];
            for (int i = 0; i < events.Length; i++)
            {
                _listeners[i] = new List<WeakReference>();
            }
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
#if DEBUG
                            Console.WriteLine("---- WEAK_EVT_SRC {0}: Attmepting to attach listener for event {1}... OK (target added)", _sourceID, eventName);
#endif
                        }
                        else
                        {
#if DEBUG
                            Console.WriteLine("---- WEAK_EVT_SRC {0}: Attmepting to attach listener for event {1}... IGNORED (target already listening)", _sourceID, eventName);
#endif
                        }
                        break;
                    }
                }

                if (!eventFound)
                {
                    Console.WriteLine("---- WEAK_EVT_SRC {0}: Attmepting to attach listener for event {1}... FAILED (no such event)", _sourceID, eventName);
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
#if DEBUG
                    Console.WriteLine("---- WEAK_EVT_SRC {0}: Attmepting to detach listener for event {1}... OK (target removed {2} times)", _sourceID, eventName, targetRemoved);
#endif
                }
                else
                {
#if DEBUG
                    Console.WriteLine("---- WEAK_EVT_SRC {0}: Attmepting to detach listener for event {1}... FAILED (either target did not exist or no such event)", _sourceID, eventName);
#endif
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
#if DEBUG
                            Console.WriteLine(" * WEAK_EVT_SRC ERROR NOTIFYING TARGET: " + exc.ToString());
#endif
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
