using System;
using System.Collections.Generic;

namespace eilang.Interpreting;

public class StackWithoutNullItems<T>
{
    private readonly Stack<T> _internalStack;

    public StackWithoutNullItems()
    {
        _internalStack = new Stack<T>();
    }

    public StackWithoutNullItems(int capacity)
    {
        _internalStack = new Stack<T>(capacity);
    }

    public T Peek() => _internalStack.Peek();
    public bool TryPeek(out T result) => _internalStack.TryPeek(out result);
    public bool TryPop(out T result) => _internalStack.TryPop(out result);
    public T Pop() => _internalStack.Pop();

    public void Push(T item)
    {
        if(item == null)
            throw new ArgumentNullException(nameof(item));
        _internalStack.Push(item);
    }
}
