using eilang.Classes;

namespace eilang
{
    public class Instance
    {
        public Scope Scope { get; }
        public Class Owner { get; }

        public Instance(Scope scope, Class owner)
        {
            Scope = scope;
            Owner = owner;
        }
    }
}