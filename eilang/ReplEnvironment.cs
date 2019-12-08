using System.Collections;
using System.Collections.Generic;
using eilang.Classes;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.OperationCodes;

namespace eilang
{
    public class ReplEnvironment : ScriptEnvironment
    {
        public ReplEnvironment(IOperationCodeFactory operationCodeFactory, IValueFactory valueFactory) : base(operationCodeFactory, valueFactory)
        {
            
        }

        public override IDictionary<string, Function> Functions { get; } =
            new DictionaryAddWillOverwrite<string, Function>();
        public override IDictionary<string, Class> Classes { get; } = 
            new DictionaryAddWillOverwrite<string, Class>();
        public override IDictionary<string, ExportedFunction> ExportedFunctions { get; } = 
            new DictionaryAddWillOverwrite<string, ExportedFunction>();

        public override IDictionary<string, ExtensionFunction> ExtensionFunctions { get; } =
            new DictionaryAddWillOverwrite<string, ExtensionFunction>();
    }

    public class DictionaryAddWillOverwrite<TKey, TValue> : IDictionary<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> _actual = new Dictionary<TKey, TValue>();
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _actual.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _actual[item.Key] = item.Value;
        }

        public void Clear()
        {
            _actual.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>)_actual).Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            ((IDictionary<TKey, TValue>)_actual).CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return ((IDictionary<TKey, TValue>) _actual).Remove(item);
        }

        public int Count => _actual.Count;
        public bool IsReadOnly => ((IDictionary<TKey, TValue>) _actual).IsReadOnly;
        
        public void Add(TKey key, TValue value)
        {
            _actual[key] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return _actual.ContainsKey(key);
        }

        public bool Remove(TKey key)
        {
            return _actual.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _actual.TryGetValue(key, out value);
        }

        public TValue this[TKey key]
        {
            get => ((IDictionary<TKey, TValue>) _actual)[key];
            set => ((IDictionary<TKey, TValue>) _actual)[key] = value;
        }

        public ICollection<TKey> Keys => ((IDictionary<TKey, TValue>) _actual).Keys;
        public ICollection<TValue> Values => ((IDictionary<TKey, TValue>) _actual).Values;
    }
}