using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace eilang
{
    /// <summary>
    /// Naive dictionary/map
    /// </summary>
    public class NaiveHashTable<TKey, TValue>  // actual hash table should implement IEnumerable
        where TKey : IHaveHash
    {
        private class Entry
        {
            public TKey Key;
            public TValue Value;
            public Entry Next;
            public int Hash;
        }
        
        private Entry[] _entries = new Entry[7];
        
        public void Insert(TKey key, TValue value)
        {
            var hash = TransformHash(key);
            if (_entries[hash] == null)
            {
                _entries[hash] = new Entry {Key = key, Value = value, Hash = hash};
            }
            else
            {
                InsertAtEndOfChain(hash, key, value);
            }

            for (int i = 0; i < _entries.Length; i++)
            {
                if (_entries[i] == null)
                    return;
            }
            
            // should be resized
            var newCapacity = ((3 * _entries.Length) / 2) + 15;
            Entry[] tmp = new Entry[newCapacity];
            _entries.CopyTo(tmp, 0);
            _entries = tmp;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private int TransformHash(TKey key)
        {
            return key.GetHash() % _entries.Length;
        }

        public void Remove(TKey key)
        {
            var hash = TransformHash(key);
            if (_entries[hash] == null)
                return; // nothing to remove

            if (_entries[hash].Next == null && _entries[hash].Key.Equals(key))
            { // first entry and no chained entries, just set to null
                _entries[hash] = null;
                return;
            }

            var current = _entries[hash]; // find the entry
            var next = current.Next;
            while (next != null)
            {
                if (next.Key.Equals(key)) // then current is the entry which holds the entry we're looking for
                {
                    break;
                }
                current = next;
                next = current.Next;
            }

            if (next.Next == null) // if chaining stops on the entry, just set the current entry's Next to null
            {
                current.Next = null;
                return;
            }
            // overwrite the entry we were looking for with the next one in the chain
            current.Next = next.Next;
        }

        public TValue Get(TKey key)
        {
            var hash = TransformHash(key);
            if (_entries[hash] == null)
                throw new KeyNotFoundException($"Key '{key}' was not found.");
            if (_entries[hash].Key.Equals(key))
                return _entries[hash].Value;

            var current = _entries[hash];
            while (current.Next != null)
            {
                if (current.Next.Key.Equals(key))
                    return current.Next.Value;
                current = current.Next;
            }
            
            throw new KeyNotFoundException($"Key '{key}' was not found.");
        }

        public void Clear()
        {
            _entries = new Entry[7];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void InsertAtEndOfChain(int hash, TKey key, TValue value)
        {
            var current = _entries[hash];
            while (current.Next != null)
            {
                current = current.Next;
            }
            current.Next = new Entry{Key = key, Value = value, Hash = hash};
        }
    }

    public interface IHaveHash
    {
        int GetHash();
    }
}