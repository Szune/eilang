using eilang.ArgumentBuilders;
using eilang.Exporting;
using eilang.Helpers;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules;

[ExportModule("http")]
public static class HttpModule
{
    [ExportFunction("get")]
    public static ValueBase Get(State state, Arguments args)
    {
        var argList = args.List().With
            .Argument(EilangType.String, "url")
            .OptionalArgument(EilangType.String, "headers", "")
            .Build();
        var url = argList.Get<string>(0);
        var headers = argList.Get<string>(1);
        return HttpHelper.Get(state, url, headers);
    }

    [ExportFunction("post")]
    public static ValueBase Post(State state, Arguments args)
    {
        var argList = args.List().With
            .Argument(EilangType.String, "url")
            .Argument(EilangType.String, "headers")
            .Argument(EilangType.String, "content")
            .Build();
        var url = argList.Get<string>(0);
        var headers = argList.Get<string>(1);
        var content = argList.Get<string>(2);
        return HttpHelper.Post(state, url, headers, content);
    }
}
