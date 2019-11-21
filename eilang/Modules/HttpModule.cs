using eilang.Exporting;
using eilang.Extensions;
using eilang.Helpers;
using eilang.Interfaces;
using eilang.Values;

namespace eilang.Modules
{
    [ExportModule("http")]
    public static class HttpModule
    {
        [ExportFunction("get")]
        public static IValue Get(IValueFactory fac, IValue args)
        {
            var argList = args
                .Require(TypeOfValue.List,"get takes 2 arguments: string url, string headers")
                .As<ListValue>()
                .RequireCount(2, "get takes 2 arguments: string url, string headers")
                .Item;
            argList.OrderAsArguments();
            var url = argList[0]
                .Require(TypeOfValue.String, "get requires that parameter 'url' is a string.")
                .To<string>();
            var headers = argList[1]
                .Require(TypeOfValue.String, "get requires that parameter 'headers' is a string.")
                .To<string>();
            return HttpHelper.Get(fac, url, headers);
        }
        
        [ExportFunction("post")]
        public static IValue Post(IValueFactory fac, IValue args)
        {
            var argList = args
                .Require(TypeOfValue.List,"post takes 3 arguments: string url, string headers, string content")
                .As<ListValue>()
                .RequireCount(3, "post takes 3 arguments: string url, string headers, string content")
                .Item;
            argList.OrderAsArguments();
            var url = argList[0]
                .Require(TypeOfValue.String, "post requires that parameter 'url' is a string.")
                .To<string>();
            var headers = argList[1]
                .Require(TypeOfValue.String, "post requires that parameter 'headers' is a string.")
                .To<string>();
            var content = argList[2]
                .Require(TypeOfValue.String, "post requires that parameter 'content' is a string.")
                .To<string>();
            return HttpHelper.Post(fac, url, headers, content);
        }
    }
}