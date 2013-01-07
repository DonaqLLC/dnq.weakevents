using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNQ.Events
{
    /// <summary>
    /// Intereface that defines common methods for objects that support raising weak events.
    /// </summary>
    public interface IWeakEventSource
    {
        /// <summary>
        /// Classes that implement this method should enumerate all the weak events available for targets to attach to.
        /// </summary>
        /// <returns>An enumberable collection of strings that uniquely identify each event.</returns>
        IEnumerable<string> EnumerateWeakEvents();

        /// <summary>
        /// Classes that implement this method expose a way for targets to register themselves as interested in being notified
        /// when the event identified by <paramref name="eventName"/> is raised.
        /// </summary>
        /// <param name="eventName">Unique identifier of the event</param>
        /// <param name="target">A target that is registering itself to receive notifications when an event occurs.</param>
        void AttachEvent(string eventName, IWeakEventTarget target);

        /// <summary>
        /// Classes that implement this mehtod expose a way for targets to specify that they are no longer interested in being
        /// notified when the event identified by <paramref name="eventName"/> is raised.
        /// </summary>
        /// <param name="eventName">Unique identifier of the event</param>
        /// <param name="target">A target that is registering itself to receive notifications when an event occurs.</param>
        void DetachEvent(string eventName, IWeakEventTarget target);
    }
}
