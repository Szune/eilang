using eilang.Extensions;
using eilang.Interfaces;

namespace eilang.ArgumentBuilders
{
    public class OptionalArgument : IArgument
    {
        private readonly IValue _value;
        private readonly object _default;

        public OptionalArgument(IValue value, object @default)
        {
            _value = value;
            _default = @default;
        }


        public T Get<T>()
        {
            return _value != null ? _value.To<T>() : (T) _default;
        }
    }
}