using System;
using Mirzipan.Bibliotheca.Unity;
using UnityEngine;

namespace Mirzipan.Scheduler.Unity
{
    /// <summary>
    /// Example scheduler ticker
    /// </summary>
    public class SchedulerTicker: Singleton<SchedulerTicker>
    {
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("How much of the frame may be used up by the scheduler. 0 - none, 1 - whole frame")]
        private double _frameBudgetPercentage = 0.8d;

        private Ticker _ticker;
        private Updater _updater;

        #region Lifecycle

        protected override void Awake()
        {
            base.Awake();
            
            _ticker = new Ticker();
            
            double frameBudget = 1d / Mathf.Max(Application.targetFrameRate, 30) * _frameBudgetPercentage;
            _updater = new Updater(frameBudget);
        }

        private void FixedUpdate()
        {
            _ticker.Tick();
            _updater.Tick(Time.fixedUnscaledTimeAsDouble);
        }

        private void OnDestroy()
        {
            _ticker?.Dispose();
            _ticker = null;
            
            _updater?.Dispose();
            _updater = null;
        }

        #endregion Lifecycle

        #region Public

        /// <summary>
        /// Add an update that will be called each tick.
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        public IDisposable AddUpdate(TickUpdate update)
        {
            return _ticker?.Add(update);
        }

        /// <summary>
        /// Remove an update that was called each tick.
        /// </summary>
        /// <param name="update"></param>
        public void RemoveUpdate(TickUpdate update)
        {
            _ticker?.Remove(update);
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time in seconds after which to call the method</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(double dueTime, DeferredUpdate update)
        {
            return _updater?.Schedule(dueTime, update);
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
            return _updater?.Schedule(dueTime, period, update);
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time after which to call the method</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(TimeSpan dueTime, DeferredUpdate update)
        {
            return _updater?.Schedule(dueTime.TotalSeconds, update);
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
            return _updater?.Schedule(dueTime.TotalSeconds, period.TotalSeconds, update);
        }

        /// <summary>
        /// Unschedule a previously scheduled update.
        /// </summary>
        /// <param name="update">Method to unschedule</param>
        public void Unschedule(DeferredUpdate update)
        {
            _updater?.Unschedule(update);
        }

        /// <summary>
        /// Unschedule all currently scheduled updates.
        /// </summary>
        public void ClearUpdater()
        {
            _updater?.Clear();
        }

        /// <summary>
        /// Add a tick update.
        /// </summary>
        public IDisposable AddTick(TickUpdate update)
        {
            return _ticker?.Add(update, 0);
        }

        /// <summary>
        /// Add a tick update with priority.
        /// </summary>
        public IDisposable AddTick(TickUpdate update, int priority)
        {
            return _ticker?.Add(update, priority);
        }

        /// <summary>
        /// Remove a tick update.
        /// </summary>
        public void RemoveTick(TickUpdate update)
        {
            _ticker?.Remove(update);
        }

        /// <summary>
        /// Unschedule all currently scheduled ticks.
        /// </summary>
        public void ClearTicker()
        {
            _ticker?.Clear();
        }

        #endregion Public
    }
}