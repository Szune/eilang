using eilang.Classes;
using eilang.Compiling;
using eilang.Exceptions;

namespace eilang.Tests;

public static class Helpers
{
    public static MemberFunction GetMemberFunctionOrThrow(this Class it, string name)
    {
        return it.TryGetFunction(name, out var func)
            ? func
            : throw new AssertionException($"Could not find function {name} in class {it.FullName}");
    }
}
