using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNQ.Events
{
    /// <summary>
    /// Interface which defines a common method for any object what will participate in the weak event mechanism defined
    /// by the <see cref="DNQ.Events.IWeakEventSource"/> interface.
    /// </summary>
    public interface IWeakEventTarget
    {
        /// <summary>
        /// Classes that implement this method should return a comparable value that identifies the target. This
        /// value should always be the same for the same target object, but it will not be used by the weak events system
        /// in identifying the target. It's only meant to help in debugging by showing a friendly ID of the target.
        /// </summary>
        IComparable TargetFriendlyID { get; }

        /// <summary>
        /// This method will be invoked automatically when the event identified by <paramref name="eventName"/> is raised
        /// by an object which implements the <see cref="DNQ.Events.IWeakEventSource"/> interface. Classes which implement 
        /// this interface should implement the logic of this method to perfom an action in response to the event they are
        /// being notified of.
        /// </summary>
        /// <param name="source">A reference to an object that implements IWeakEventSource which called the WeakEventNotification method. This may not be the real (original) source of the event.</param>
        /// <param name="eventName">The unique identifier of the event that was fired.</param>
        /// <param name="args">An additional information that may be provided by the source about this event.</param>
        void WeakEventNotification(IWeakEventSource weakSource, string eventName, WeakEventArgs args);
    }
}
