using eilang.Interfaces;

namespace eilang.ArgumentBuilders
{
    public class ArgumentListBuilder
    {
        private readonly IValue _value;
        private readonly string _function;

        public ArgumentListBuilder(IValue value, string function)
        {
            _value = value;
            _function = function;
        }
        
        public ArgumentListBuilderWithArguments With => new ArgumentListBuilderWithArguments(_value, _function);
    }
}