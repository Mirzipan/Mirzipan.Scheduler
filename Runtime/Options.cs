using System;

namespace Mirzipan.Scheduler
{
    [Flags]
    public enum Options
    {
        None = 0,
        SmearUpdates = 1 << 0,
    }
}