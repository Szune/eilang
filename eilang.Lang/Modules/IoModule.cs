using System;
using System.IO;
using System.Linq;
using eilang.ArgumentBuilders;
using eilang.Exporting;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Modules;

[ExportModule("io")]
public static class IoModule
{
    public static ValueBase Move(string func, State state, Arguments args, Action<string, string, bool> moveAction)
    {
        var argList = args.List().With
            .Argument(EilangType.String, "currentName")
            .Argument(EilangType.String, "newName")
            .OptionalArgument(EilangType.Bool, "overwrite", false)
            .Build();

        var currentName = argList.Get<string>(0);
        var newName = argList.Get<string>(1);
        var overwrite = argList.Get<bool>(2);

        try
        {
            moveAction(currentName, newName, overwrite);
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

    [ExportFunction("get_drives")]
    public static ValueBase GetDrives(State state, Arguments args)
    {
        args.Void();
        var fac = state.ValueFactory; // capture ValueFactory for lambda
        var drives = DriveInfo.GetDrives().Select(d => fac.String(d.Name)).ToList();
        return state.ValueFactory.List(drives);
    }

    [ExportFunction("is_file")]
    public static ValueBase IsFile(State state, Arguments args)
    {
        var fileName = args.Single<string>(EilangType.String, "fileName");
        return state.ValueFactory.Bool(File.Exists(fileName));
    }

    [ExportFunction("is_dir")]
    public static ValueBase IsDirectory(State state, Arguments args)
    {
        var dirName = args.Single<string>(EilangType.String, "directory");
        var factory = state.ValueFactory;
        return factory.Bool(Directory.Exists(dirName));
    }

    [ExportFunction("get_items")]
    public static ValueBase GetItems(State state, Arguments args)
    {
        var dirName = args.Single<string>(EilangType.String, "directory");

        var dir = new DirectoryInfo(dirName);
        var factory = state.ValueFactory;
        var dirs = dir.GetFileSystemInfos().Select(d => factory.String(d.Name)).ToList();
        return factory.List(dirs);
    }

    [ExportFunction("get_dirs")]
    public static ValueBase GetDirectories(State state, Arguments args)
    {
        var dirName = args.Single<string>(EilangType.String, "directory");

        var dir = new DirectoryInfo(dirName);
        var factory = state.ValueFactory;
        var dirs = dir.GetDirectories().Select(d => factory.String(d.Name)).ToList();
        return factory.List(dirs);
    }
    [ExportFunction("get_files")]
    public static ValueBase GetFiles(State state, Arguments args)
    {
        var dirName = args.Single<string>(EilangType.String, "directory");

        var dir = new DirectoryInfo(dirName);
        var factory = state.ValueFactory;
        var files = dir.GetFiles().Select(d => factory.String(d.Name)).ToList();
        return factory.List(files);
    }

}
