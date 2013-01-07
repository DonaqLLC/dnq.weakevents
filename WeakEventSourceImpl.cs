using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNQ.Events
{
    internal class WeakEventSourceImpl
        : WeakEventSource
    {
        internal WeakEventSourceImpl(string[] events)
            : base(events)
        {
        }

    }
}
