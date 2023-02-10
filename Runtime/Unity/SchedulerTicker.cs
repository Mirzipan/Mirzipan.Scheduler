#if MIRZIPAN_BIBLIOTHECA

using Mirzipan.Bibliotheca;
using UnityEngine;

namespace Mirzipan.Scheduler.Unity
{
    /// <summary>
    /// Example scheduler ticker
    /// </summary>
    public class SchedulerTicker: Singleton<SchedulerTicker>
    {
        public static SchedulerTicker Instance;
        
        [SerializedField]
        private TickerTime _time;

        private Runtime.Scheduler _scheduler;

        #region Lifecycle

        private void Awake()
        {
            _scheduler = new Scheduler(TimeHelper.GetTime(_time), 1d / Application.targetFrameRate, Options.SmearUpdates);
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
    }
}

#endif