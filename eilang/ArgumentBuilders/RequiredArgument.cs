using eilang.Extensions;
using eilang.Interfaces;

namespace eilang.ArgumentBuilders
{
    public class RequiredArgument : IArgument
    {
        private readonly IValue _value;

        public RequiredArgument(IValue value)
        {
            _value = value;
        }


        public T Get<T>()
        {
            return _value.To<T>();
        }
    }
}