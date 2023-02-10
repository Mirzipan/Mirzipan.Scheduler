using System;

namespace Mirzipan.Scheduler
{
    /// <summary>
    /// Disposing of this handle will unregister your deferred update from the scheduler.
    /// </summary>
    public class ScheduleHandle: IDisposable
    {
        private readonly Action _onDispose;
        
        internal ScheduleHandle(Action onDispose)
        {
            _onDispose = onDispose;
        }
        
        public void Dispose()
        {
            _onDispose?.Invoke();
        }
    }
}