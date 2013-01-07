using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNQ.Events
{
    /// <summary>
    /// Helper class for attaching and detaching weak events 
    /// </summary>
    public static class Helper
    {
        /// <summary>
        /// Attempts to attach the object <paramref name="target"/> as an event target for the event identified by <paramref name="eventName"/>
        /// on the object <paramref name="evtSrcObject"/>, if that object implements the <see cref="TriTeq.AutoTraq.Events.IWeakEventSource"/> interface
        /// </summary>
        /// <param name="evtSrcObject">An object which implements the TriTeq.AutoTraq.Events.IWeakEventSource interface.</param>
        /// <param name="eventName">The name of the event the target is interested in.</param>
        /// <param name="target">An instance of a class which implements the <see cref="TriTeq.AutoTraq.Events.IWeakEventTarget"/> interface.</param>
        /// <returns>Returns True if the object implements the <see cref="TriTeq.AutoTraq.Events.IWeakEventSource"/> and the method was succesful in attaching the event.</returns>
        public static bool TryAttachEvent(object evtSrcObject, string eventName, DNQ.Events.IWeakEventTarget target)
        {
            if (evtSrcObject is DNQ.Events.IWeakEventSource)
            {
                ((DNQ.Events.IWeakEventSource)evtSrcObject).AttachEvent(eventName, target);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Attempts to detach the object <paramref name="target"/> as an event target for the event identified by <paramref name="eventName"/>
        /// on the object <paramref name="evtSrcObject"/>, if that object implements the <see cref="TriTeq.AutoTraq.Events.IWeakEventSource"/> interface.
        /// </summary>
        /// <param name="evtSrcObject">An object which implements the TriTeq.AutoTraq.Events.IWeakEventSource interface.</param>
        /// <param name="eventName">The name of the event the target is interested in.</param>
        /// <param name="target">An instance of a class which implements the <see cref="TriTeq.AutoTraq.Events.IWeakEventTarget"/> interface.</param>
        /// <returns>Returns True if the object implements the <see cref="TriTeq.AutoTraq.Events.IWeakEventSource"/> and the method was succesful in detaching the event.</returns>
        public static bool TryDetachEvent(object evtSrcObject, string eventName, DNQ.Events.IWeakEventTarget target)
        {
            if (evtSrcObject is DNQ.Events.IWeakEventSource)
            {
                ((DNQ.Events.IWeakEventSource)evtSrcObject).DetachEvent(eventName, target);
                return true;
            }
            return false;
        }
    }
}
