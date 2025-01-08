using System.Collections.Generic;

namespace eilang;

// TODO: implement arguments as numbered locals in the call frame
// TODO:  so that they do not require checking a dictionary to find a value
// TODO:  i.e. let the compiler/parser number the args also when they are used
// TODO:  keep 1-3 args as respective fields so that they can be fetched faster
// TODO:  setlocal1, getlocal1, setlocali 5, getlocali 5, etc

// TODO: implement this for all identifiers (variables, class names, etc), remember to fix in reflection as well
/// <summary>
/// An identifier that can be used in switches for cheap by using the integer id instead of a string.
/// </summary>
/// <param name="Id">Id mainly used by interpreter.</param>
/// <param name="Text">Name mainly used by compiler.</param>
public record Identifier(int Id, string Text)
{
    public override int GetHashCode()
    {
        return Id;
    }
}

public class IdentifierScope
{
    private readonly List<Identifier> _all = new(50);
    public Identifier Create(string text)
    {
        var id = new Identifier(_all.Count, text);
        _all.Add(id);
        return id;
    }

    public Identifier GetOrCreate(string text)
    {
        var count = _all.Count;
        for (var i = 0; i < count; i++)
        {
            if (string.Equals(_all[i].Text, text))
            {
                return _all[i];
            }
        }

        return Create(text);
    }

    public Identifier Get(int id)
    {
        return _all[id];
    }
}
