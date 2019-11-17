using System;

namespace eilang.Values
{
    public class FileHandleValue : DisposableObjectValue
    {
        public FileHandleValue (Instance value) : base(value)
        {
        }

        public override void Dispose()
        {
            Get<Instance>().GetVariable(SpecialVariables.FileWrite).Get<IDisposable>().Dispose();
            Get<Instance>().GetVariable(SpecialVariables.FileRead).Get<IDisposable>().Dispose();
            Get<Instance>().GetVariable(SpecialVariables.Disposable).Get<IDisposable>().Dispose();
        }
    }
}