using System;
using System.Diagnostics;
using Mirzipan.Bibliotheca.Disposables;

namespace Mirzipan.Scheduler
{
    public class Scheduler : IDisposable
    {
        private readonly SortableSet<ScheduledEntry> _data;
        private readonly IProvideTime _time;
        private readonly Stopwatch _sw;
        
        private readonly double _frameBudget;
        private readonly bool _smearUpdates;
        
        private bool _tickInProgress;

        #region Lifecycle

        /// <summary>
        /// 
        /// </summary>
        /// <param name="time">Time used for scheduling</param>
        /// <param name="frameBudget">Maximum amount of time to spend on a single Tick</param>
        /// <param name="options">Modification for the scheduler behaviour</param>
        public Scheduler(IProvideTime time, double frameBudget, Options options = Options.SmearUpdates)
        {
            _data = new SortableSet<ScheduledEntry>(32, ScheduledEntryComparer.Comparer);
            _frameBudget = frameBudget;
            _time = time;
            
            _sw = new Stopwatch();

            _smearUpdates = (options & Options.SmearUpdates) != 0;
        }

        /// <summary>
        /// Tick the scheduler. This will call scheduled updates, if their due time has come.
        /// </summary>
        public void Tick()
        {
            _sw.Start();

            _tickInProgress = true;
            _data.Sort();

            while (_data.Count > 0 && _tickInProgress)
            {
                var entry = _data[0];
                double delta = _time.Now - entry.ScheduledAt;
                if (delta >= entry.DueTime)
                {
                    goto EndTick;
                }

                _data.RemoveAt(0);

                try
                {
                    entry.Update.Invoke(delta);
                }
                catch (Exception e)
                {
                    continue;
                }

                if (Math.Abs(entry.Period) > double.Epsilon)
                {
                    entry.ScheduledAt = _time.Now;
                    entry.DueTime = entry.Period;
                    _data.Add(entry);
                }

                if (_smearUpdates && _sw.ElapsedMilliseconds >= _frameBudget)
                {
                    goto EndTick;
                }
            }
            
            EndTick:
            _tickInProgress = false;
            _sw.Stop();
            _sw.Reset();
        }

        public void Dispose()
        {
            _data.Clear();
            _tickInProgress = false;
        }

        #endregion Lifecycle

        #region Public

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time in seconds after which to call the method</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(double dueTime, DeferredUpdate update)
        {
            var entry = new ScheduledEntry(_time.Now, Math.Max(_frameBudget, dueTime), 0d, update);
            _data.AddOrReplace(entry);
            return Disposable.Create(() => Unschedule(update));
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
            var entry = new ScheduledEntry(_time.Now, Math.Max(_frameBudget, dueTime), Math.Max(0d, period), update);
            _data.AddOrReplace(entry);
            return Disposable.Create(() => Unschedule(update));
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time after which to call the method</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(TimeSpan dueTime, DeferredUpdate update)
        {
            return Schedule(dueTime.TotalSeconds, update);
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time after which to call the method</param>
        /// <param name="period">Frequency with which the method will be called.</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(TimeSpan dueTime, TimeSpan period, DeferredUpdate update)
        {
            return Schedule(dueTime.TotalSeconds, period.TotalSeconds, update);
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

        #region Internal

        internal void Remove(ScheduledEntry entry)
        {
            _data.Remove(entry);
        }

        #endregion Internal
    }
}