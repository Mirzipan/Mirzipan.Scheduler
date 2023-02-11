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
        private TickerTime _time;
        [SerializeField]
        private Options _options = Options.SmearUpdates;
        [SerializeField]
        [Range(0, 1)]
        [Tooltip("How much of the frame may be used up by the scheduler. 0 - none, 1 - whole frame")]
        private double _frameBudgetPercentage = 0.8d;

        private Scheduler _scheduler;

        #region Lifecycle

        private void Awake()
        {
            double frameBudget = 1d / Application.targetFrameRate * _frameBudgetPercentage;
            _scheduler = new Scheduler(TimeHelper.GetTime(_time), frameBudget, _options);
        }

        private void FixedUpdate()
        {
            _scheduler.Tick();
        }

        private void OnDestroy()
        {
            _scheduler?.Dispose();
            _scheduler = null;
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
            return _scheduler.Schedule(dueTime, update);
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
            return _scheduler.Schedule(dueTime, period, update);
        }

        /// <summary>
        /// Schedule an update, optionally a repeating one.
        /// </summary>
        /// <param name="dueTime">Time after which to call the method</param>
        /// <param name="update">Method to call</param>
        /// <returns></returns>
        public IDisposable Schedule(TimeSpan dueTime, DeferredUpdate update)
        {
            return _scheduler.Schedule(dueTime, update);
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
            return _scheduler.Schedule(dueTime, period, update);
        }

        /// <summary>
        /// Unschedule a previously scheduled update.
        /// </summary>
        /// <param name="update">Method to unschedule</param>
        public void Unschedule(DeferredUpdate update)
        {
            _scheduler.Unschedule(update);
        }

        /// <summary>
        /// Unschedule all currently scheduled updates.
        /// </summary>
        public void Clear()
        {
            _scheduler.Clear();
        }

        #endregion Public
    }
}