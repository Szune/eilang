using eilang.Exporting;
using eilang.Extensions;
using eilang.Helpers;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("http")]
    public static class HttpModule
    {
        [ExportFunction("get")]
        public static IValue Get(State state, IValue args)
        {
            const string expectedArguments = "get takes 2 arguments: string url, string headers";
            var argList = args
                .Require(EilangType.List, expectedArguments)
                .As<ListValue>()
                .RequireCount(2, expectedArguments)
                .Item;
            argList.OrderAsArguments();
            var url = argList[0]
                .Require(EilangType.String, "get requires that parameter 'url' is a string.")
                .To<string>();
            var headers = argList[1]
                .Require(EilangType.String, "get requires that parameter 'headers' is a string.")
                .To<string>();
            return HttpHelper.Get(state, url, headers);
        }
        
        [ExportFunction("post")]
        public static IValue Post(State state, IValue args)
        {
            const string expectedArguments = "post takes 3 arguments: string url, string headers, string content";
            var argList = args
                .Require(EilangType.List, expectedArguments)
                .As<ListValue>()
                .RequireCount(3, expectedArguments)
                .Item;
            argList.OrderAsArguments();
            var url = argList[0]
                .Require(EilangType.String, "post requires that parameter 'url' is a string.")
                .To<string>();
            var headers = argList[1]
                .Require(EilangType.String, "post requires that parameter 'headers' is a string.")
                .To<string>();
            var content = argList[2]
                .Require(EilangType.String, "post requires that parameter 'content' is a string.")
                .To<string>();
            return HttpHelper.Post(state, url, headers, content);
        }
    }
}