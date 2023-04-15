using System;

namespace Mirzipan.Scheduler
{
    internal struct TickEntry : IComparable<TickEntry>
    {
        public int Priority;
        public TickUpdate Update;

        public TickEntry(TickUpdate update) : this(0, update)
        {
        }

        public TickEntry(int priority, TickUpdate update)
        {
            Priority = priority;
            Update = update;
        }

        public int CompareTo(TickEntry other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
}