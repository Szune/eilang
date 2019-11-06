namespace eilang.Interfaces
{
    public interface IScope
    {
        IValue GetVariable(string name);
    }
}