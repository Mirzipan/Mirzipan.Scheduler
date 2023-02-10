using System;

namespace Mirzipan.Scheduler
{
    internal struct ScheduledEntry: IComparable<ScheduledEntry>
    {
        public double ScheduledAt;
        public double DueTime;
        public double Period;
        public DeferredUpdate Update;

        #region Lifecycle

        public ScheduledEntry(DeferredUpdate update) : this(0d, 0d, 0d, update)
        {
        }

        public ScheduledEntry(double scheduledAt, double dueTime, double period, DeferredUpdate update)
        {
            ScheduledAt = scheduledAt;
            DueTime = dueTime;
            Period = period;
            Update = update;
        }

        #endregion Lifecycle

        #region Comparable

        public int CompareTo(ScheduledEntry other)
        {
            return (ScheduledAt + DueTime).CompareTo((other.ScheduledAt + other.DueTime));
        }

        #endregion Comparable
    }
}