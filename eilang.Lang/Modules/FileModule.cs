using System;
using System.IO;
using eilang.ArgumentBuilders;
using eilang.Exporting;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules;

[ExportModule("file")]
public static class FileModule
{
    [ExportFunction("rename")]
    [ExportFunction("move")]
    public static ValueBase RenameFile(State state, Arguments args)
    {
        return IoModule.Move("file::move", state, args,
            File.Move);
    }

    [ExportFunction("copy")]
    public static ValueBase CopyFile(State state, Arguments args)
    {

        // TODO: implement with optional overwrite argument
        throw new NotImplementedException();
        return state.ValueFactory.Void();
    }

    [ExportFunction("make")]
    public static ValueBase MakeFile(State state, Arguments args)
    {
        var name = args.Single<string>(EilangType.String, "fileName");
        try
        {
            File.Create(name).Dispose(); // dispose the returned FileStream
            return state.ValueFactory.True();
        }
        catch(Exception ex)
        {
#if DEBUG
Console.WriteLine(ex.ToString());
#endif
            return state.ValueFactory.False();
        }
    }

    [ExportFunction("delete")]
    public static ValueBase DeleteFile(State state, Arguments args)
    {
        var argList = args.List().With
            .Argument(EilangType.String, "path")
            .Argument(EilangType.String, "patternOrName")
            .OptionalArgument(EilangType.Bool, "recurse", false)
            .Build();
        var path = argList.Get<string>(0);
        var patternOrName = argList.Get<string>(1);
        var recurse = argList.Get<bool>(2);

        try
        {
            var files = new DirectoryInfo(path).GetFiles(patternOrName,
                recurse
                    ? SearchOption.AllDirectories
                    : SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                file.Delete();
            }
            return state.ValueFactory.Bool(true);
        }
        catch
        {
            return state.ValueFactory.Bool(false);
        }
    }

    [ExportFunction("open")]
    public static ValueBase OpenFile(State state, Arguments arg)
    {
        if (arg.Type == EilangType.List)
        {
            var list = arg.List().With
                .Argument(EilangType.String, "fileName")
                .OptionalArgument(EilangType.Bool, "append", false)
                .Build();
            return OpenFileInner(state, list.Get<string>(0), list.Get<bool>(1));
        }

        var fileName = arg.Single<string>(EilangType.String, "fileName");
        return OpenFileInner(state, fileName, false);
    }

    private static ValueBase OpenFileInner(State state, string fileName, bool append)
    {
        // TODO: add encoding options
        FileStream fileStream;
        TextReader reader;
        if (append)
        {
            fileStream = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.None);
            reader = new StringReader("Cannot read from file that was opened with 'append' set to true.");
        }
        else
        {
            fileStream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            reader = new StreamReader(fileStream);
        }

        var writer = new StreamWriter(fileStream);
        return state.ValueFactory.FileHandle(fileStream, reader, writer);
    }
}
