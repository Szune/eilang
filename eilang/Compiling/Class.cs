using System.Collections.Generic;
using System.Threading;
using eilang.Compiling;
using eilang.Values;

namespace eilang.Classes;

public class Class
{
    private static int _typeIdAcc = -1;
    public readonly int Id;
    public Class(string name, string module)
    {
        Id = Interlocked.Increment(ref _typeIdAcc);
        ValueFactory._classes.Add(new ClassValue(this));
        Name = name;
        Module = module;
        CtorForMembersWithValues = new MemberFunction(".ctorForInit", "na", new List<string>(), this);
    }

    public string Name { get; }
    public string Module { get; }
    private string _fullName;
    public string FullName => _fullName ??= $"{Module}::{Name}";
    // N.B.:
    // using a list because unless classes have hundreds of methods,
    // hashing and scanning dictionaries is often slower
    private List<MemberFunction> _functions = new();
    public IEnumerable<MemberFunction> Functions => _functions;
    public List<MemberFunction> Constructors {get;} = new();
    public MemberFunction CtorForMembersWithValues { get; }

    public void AddMethod(MemberFunction function)
    {
        _functions.Add(function);
    }

    public bool TryGetFunction(string name, out MemberFunction func)
    {
        // N.B.:
        // using a list because unless classes have hundreds of methods,
        // hashing and scanning dictionaries is often slower
        for (var i = 0; i < _functions.Count; i++)
        {
            var f = _functions[i];

            if (string.Equals(name, f.Name))
            {
                func = f;
                return true;
            }
        }

        func = null;
        return false;
    }
}
