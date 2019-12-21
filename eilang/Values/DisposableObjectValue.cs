using System;

namespace eilang.Values
{
    public class DisposableObjectValue : ValueBase<IDisposable>
    {
        public DisposableObjectValue(Instance value) : base(EilangType.Disposable, value)
        {
        }
        
        public override IDisposable Item => Get<Instance>().GetVariable(SpecialVariables.Disposable).Get<IDisposable>();

        public override string ToString()
        {
            return "<file>";
        }

        public virtual void Dispose()
        {
            Item.Dispose();
        }
    }
}