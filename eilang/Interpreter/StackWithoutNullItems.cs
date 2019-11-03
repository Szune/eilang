using System;
using System.Collections.Generic;

namespace eilang
{
    public class StackWithoutNullItems<T>
    {
        private Stack<T> _internalStack = new Stack<T>();

        public T Peek() => _internalStack.Peek();
        public bool TryPeek(out T result) => _internalStack.TryPeek(out result);
        public T Pop() => _internalStack.Pop();

        public void Push(T item)
        {
            if(item == null)
                throw new ArgumentNullException(nameof(item));
            _internalStack.Push(item);
        }
    }
}