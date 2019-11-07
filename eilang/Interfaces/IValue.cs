using eilang.Values;

namespace eilang.Interfaces
{
    public interface IValue
    {
        TypeOfValue Type { get; }
        T Get<T>();
        object Value { get; }
        T As<T>() where T : class;
    }
}