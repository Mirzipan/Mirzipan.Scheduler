using System;
using System.Diagnostics;

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

        public void Tick()
        {
            _sw.Start();

            _tickInProgress = true;
            _data.Sort();

            while (_data.Count > 0 && _tickInProgress)
            {
                var entry = _data[0];
                double delta = _time.Time - entry.ScheduledAt;
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
                    entry.ScheduledAt = _time.Time;
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
        /// <param name="update">Method to call</param>
        /// <param name="dueTime">Time after which to call the method</param>
        /// <param name="period">Frequency with which the method will be called.</param>
        /// <returns></returns>
        public ScheduleHandle Schedule(DeferredUpdate update, double dueTime, double period = 0d)
        {
            var entry = new ScheduledEntry(_time.Time, Math.Max(_frameBudget, dueTime), period, update);
            _data.AddOrReplace(entry);
            return new ScheduleHandle(() => Unschedule(update));
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