﻿using System.Collections.Generic;

namespace Mirzipan.Scheduler
{
    internal class ScheduledEntryComparer: IEqualityComparer<ScheduledEntry>
    {
        public static readonly ScheduledEntryComparer Comparer = new();
        
        public bool Equals(ScheduledEntry x, ScheduledEntry y)
        {
            return Equals(x.Update, y.Update);
        }

        public int GetHashCode(ScheduledEntry obj)
        {
            return (obj.Update != null ? obj.Update.GetHashCode() : 0);
        }
    }
}