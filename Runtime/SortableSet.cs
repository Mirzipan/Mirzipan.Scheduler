using System.Collections;
using System.Collections.Generic;

namespace Mirzipan.Scheduler
{
    internal class SortableSet<T> : ICollection<T>
    {
        private List<T> _data;
        private IComparer<T> _comparer;
        private IEqualityComparer<T> _equalityComparer;

        public int Count => _data.Count;
        public int Capacity => _data.Capacity;
        public bool IsReadOnly => false;

        public T this[int index] => _data[index];

        #region Lifecycle

        public SortableSet(int capacity, IEqualityComparer<T> equalityComparer) 
            : this(capacity, Comparer<T>.Default, equalityComparer)
        {
        }

        public SortableSet(int capacity, IComparer<T> comparer, IEqualityComparer<T> equalityComparer)
        {
            _data = new List<T>(capacity);
            _comparer = comparer;
            _equalityComparer = equalityComparer;
        }

        #endregion Lifecycle

        #region Queries

        public IEnumerator<T> GetEnumerator() => _data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool Contains(T item) => Contains(item, out _);

        public bool Contains(T item, out int index)
        {
            index = _data.FindIndex(e => _equalityComparer.Equals(e, item));
            return index >= 0;
        }

        public void CopyTo(T[] array, int arrayIndex) => _data.CopyTo(array, arrayIndex);

        #endregion Queries

        #region Manipulation

        public void Add(T item)
        {
            if (Contains(item))
            {
                return;
            }
            
            _data.Add(item);
        }

        public void AddOrReplace(T item)
        {
            // TODO: this could introduce side-effects
            if (Contains(item, out var index))
            {
                _data[index] = item;
                return;
            }
            
            _data.Add(item);
        }

        public bool Remove(T item)
        {
            if (Contains(item, out int index))
            {
                return false;
            }
            
            _data.RemoveAt(index);
            return true;
        }

        public void RemoveAt(int index) => _data.RemoveAt(index);

        public void Clear() => _data.Clear();

        public void Sort() => _data.Sort(_comparer);

        public void Sort(IComparer<T> comparer) => _data.Sort(comparer);

        #endregion Manipulation
    }
}