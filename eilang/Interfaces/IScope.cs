namespace eilang.Interfaces
{
    public interface IScope
    {
        IValue GetVariable(string name);
        void SetVariable(string name, IValue value);
        void DefineVariable(string name, IValue value);
    }
}