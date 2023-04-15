using System.Collections.Generic;

namespace Mirzipan.Scheduler
{
    internal class TickEqualityComparer: IEqualityComparer<TickEntry>
    {
        public static readonly TickEqualityComparer Comparer = new();
        
        public bool Equals(TickEntry x, TickEntry y)
        {
            return Equals(x.Update, y.Update);
        }

        public int GetHashCode(TickEntry obj)
        {
            return (obj.Update != null ? obj.Update.GetHashCode() : 0);
        }
    }
}