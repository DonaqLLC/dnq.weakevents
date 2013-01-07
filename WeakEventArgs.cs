using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DNQ.Events
{
    public class WeakEventArgs
        : EventArgs
    {
        public WeakEventArgs(object realSource, string eventName)
        {
            RealSource = realSource;
            EventName = eventName;
        }

        public WeakEventArgs(object realSource, string eventName, WeakEventArgs originalEventArgs)
        {
            RealSource = realSource;
            EventName = eventName;
            OriginalEventArgs = originalEventArgs;
        }

        public object RealSource
        {
            get;
            private set;
        }

        public string EventName
        {
            get;
            private set;
        }

        public WeakEventArgs OriginalEventArgs
        {
            get;
            private set;
        }
    }
}
