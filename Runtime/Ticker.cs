using System;
using Mirzipan.Bibliotheca.Disposables;

namespace Mirzipan.Scheduler
{
    public class Ticker : IDisposable
    {
        private readonly SortableSet<TickEntry> _data = new(32, TickEqualityComparer.Comparer);

        private bool _tickInProgress;
        
        #region Lifecycle

        public Ticker()
        {
        }

        public void Tick()
        {
            if (_tickInProgress)
            {
                // Multiple ticks are not allowed!
                return;
            }

            _tickInProgress = true;
            TickUpdates();
            _tickInProgress = false;
            
        }
        
        public void Dispose()
        {
        }

        #endregion Lifecycle

        #region Public

        /// <summary>
        /// Register an update.
        /// </summary>
        /// <param name="update"></param>
        public IDisposable Add(TickUpdate update) => Add(update, 0);

        /// <summary>
        /// Register an update.
        /// </summary>
        /// <param name="update"></param>
        /// <param name="priority">update priority - higher means sooner</param>
        public IDisposable Add(TickUpdate update, int priority)
        {
            var entry = new TickEntry(priority, update);
            _data.AddOrReplace(entry);
            return Disposable.Create(() => Remove(entry));
        }

        /// <summary>
        /// Unregister an update.
        /// </summary>
        /// <param name="update"></param>
        public void Remove(TickUpdate update)
        {
            Remove(new TickEntry(update));
        }

        /// <summary>
        /// Remove all currently registered updates.
        /// </summary>
        public void Clear()
        {
            _data.Clear();
        }
        #endregion Public

        #region Private

        private void TickUpdates()
        {
            _data.Sort();
            
            for (int i = 0; i < _data.Count; i++)
            {
                var entry = _data[i];

                try
                {
                    entry.Update?.Invoke();
                }
                catch (Exception e)
                {
                    _data.RemoveAt(i);
                    throw;
                }
            }
        }

        #endregion Private

        #region Internal

        internal void Remove(TickEntry entry)
        {
            _data.Remove(entry);
        }

        #endregion Internal
    }
}