using System.Collections.Generic;
using eilang.Compiling;

namespace eilang.Classes;

public class Class
{
    public int Id { get; internal set; }

    public Class(string name, string module)
    {
        Name = name;
        Module = module;
        CtorForMembersWithValues = new MemberFunction(".ctorForInit", "na", [], this);
    }

    public string Name { get; }
    public string Module { get; }
    private string _fullName;
    public string FullName => _fullName ??= $"{Module}::{Name}";
    // N.B.:
    // using a list because unless classes have hundreds of methods,
    // hashing and scanning dictionaries is often slower
    private readonly List<MemberFunction> _functions = new();
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
            func = _functions[i];

            if (string.Equals(name, func.Name))
            {
                return true;
            }
        }

        func = null;
        return false;
    }
}
