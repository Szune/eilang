namespace eilang;

public static class SpecialVariables
{
    public const string Me = ".me";
    public const string Disposable = ".disposable";
    public const string FileRead = ".file_read";
    public const string FileWrite = ".file_write";
    public const string Map = ".map";
    public const string List = ".list";
    public const string String = ".string";
    public const string Function = ".func";
    public const string ArgumentCount = ".argc";
    public const string Internal = ".internal";
    public const string Global = ".global";

    public const string AnyType = "any";
}

// TODO: implement the identifiers system
public class SpecialVariables2
{
    private IdentifierScope _scope;

    public SpecialVariables2(IdentifierScope scope)
    {
        _scope = scope;
        Me = scope.Create(".me");
        Disposable = scope.Create(".disposable");
        FileRead = scope.Create(".file_read");
        FileWrite = scope.Create(".file_write");
        Map = scope.Create(".map");
        List = scope.Create(".list");
        String = scope.Create(".string");
        Function = scope.Create(".func");
        ArgumentCount = scope.Create(".argc");
        Internal = scope.Create(".internal");
        Global = scope.Create(".global");
        AnyType = scope.Create("any");
    }

    public readonly Identifier Me;
    public readonly Identifier Disposable;
    public readonly Identifier FileRead;
    public readonly Identifier FileWrite;
    public readonly Identifier Map;
    public readonly Identifier List;
    public readonly Identifier String;
    public readonly Identifier Function;
    public readonly Identifier ArgumentCount;
    public readonly Identifier Internal;
    public readonly Identifier Global;
    public readonly Identifier AnyType;
}
