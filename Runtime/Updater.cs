using System;
using System.Diagnostics;
using Mirzipan.Bibliotheca.Disposables;

namespace Mirzipan.Scheduler
{
    public class Updater : IDisposable
    {
        private readonly SortableSet<ScheduledEntry> _data = new(32, ScheduledEqualityComparer.Comparer);
        private readonly Stopwatch _sw = new();

        /// <summary>
        /// Frame budget in milliseconds
        /// </summary>
        private long _frameBudget;
        private double _frameBudgetInSeconds;
        /// <summary>
        /// Time when tick started in seconds
        /// </summary>
        private double _tickStartedAt;

        private bool _tickInProgress;

        #region Lifecycle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frameBudget">Maximum amount of time in seconds to spend on a single Tick</param>
        public Updater(double frameBudget)
        {
            _frameBudgetInSeconds = frameBudget;
            _frameBudget = (long)(_frameBudgetInSeconds * 1000);
        }

        /// <summary>
        /// Tick the scheduler. This will call scheduled updates, if their due time has come.
        /// </summary>
        public void Tick(double time)
        {
            if (_tickInProgress)
            {
                // Multiple ticks are not allowed!
                return;
            }

            _tickInProgress = true;
            _tickStartedAt = time;
            TickDeferredUpdates();
            _tickInProgress = false;
        }

        public void Dispose()
        {
            _data.Clear();
            _tickInProgress = false;
        }

        #endregion Lifecycle

        #region Public

        /// <summary>
        /// Changes the frame budget for deferred updates to the specified value.
        /// </summary>
        /// <param name="frameBudget"></param>
        public void SetFrameBudget(double frameBudget)
        {
            _frameBudgetInSeconds = frameBudget;
            _frameBudget = (long)_frameBudgetInSeconds * 1000;
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time in seconds after which to call the method</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(double dueTime, DeferredUpdate update)
        {
            var entry = new ScheduledEntry(_tickStartedAt, Math.Max(_frameBudgetInSeconds, dueTime), 0d, update);
            _data.AddOrReplace(entry);
            return Disposable.Create(() => Remove(entry));
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time in seconds after which to call the method</param>
        /// <param name="period">Frequency (seconds) with which the method will be called.</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(double dueTime, double period, DeferredUpdate update)
        {
            var entry = new ScheduledEntry(_tickStartedAt, Math.Max(_frameBudgetInSeconds, dueTime), Math.Max(0d, period), update);
            _data.AddOrReplace(entry);
            return Disposable.Create(() => Remove(entry));
        }

        /// <summary>
        /// Unschedule a previously scheduled update.
        /// </summary>
        /// <param name="update">Method to unschedule</param>
        public void Unschedule(DeferredUpdate update)
        {
            Remove(new ScheduledEntry(update));
        }

        /// <summary>
        /// Unschedule all currently scheduled updates.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }

        #endregion Public

        #region Private

        private void TickDeferredUpdates()
        {
            _sw.Restart();
            _data.Sort();
            
            while (_sw.ElapsedMilliseconds <= _frameBudget)
            {
                if (_data.Count == 0)
                {
                    break;
                }
                
                var entry = _data[0];
                double dueTime = entry.ScheduledAt + entry.DueTime;
                if (dueTime >= _tickStartedAt)
                {
                    break;
                }
                
                double delta = _tickStartedAt - entry.ScheduledAt;
                _data.RemoveAt(0);

                if (entry.Period > double.Epsilon)
                {
                    entry.ScheduledAt = _tickStartedAt;
                    entry.DueTime = entry.Period;
                    _data.Add(entry);
                }

                try
                {
                    entry.Update.Invoke(delta);
                }
                catch (Exception e)
                {
                    continue;
                }
            }
        }

        #endregion Private

        #region Internal

        internal void Remove(ScheduledEntry entry)
        {
            _data.Remove(entry);
        }

        #endregion Internal
    }
}