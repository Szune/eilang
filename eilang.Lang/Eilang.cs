using System.Text.RegularExpressions;
using eilang.ArgumentBuilders;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Imports;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Lexing;
using eilang.OperationCodes;
using eilang.Parsing;
using eilang.Values;

namespace eilang;

public static class Eilang
{
    public static ValueBase RunFunction(IEnvironment compiledEnvironment, string functionName, params object[] arguments)
    {
        var main = new Function("main", SpecialVariables.Global, new List<string>());
        var args = arguments.ToList();
        foreach(var arg in args)
        {
            main.Write(compiledEnvironment.OperationCodeFactory.Push(arg.ToValue(compiledEnvironment)));
        }
        main.Write(compiledEnvironment.OperationCodeFactory.Push(compiledEnvironment.ValueFactory.Integer(args.Count)));
        main.Write(compiledEnvironment.OperationCodeFactory.Call(compiledEnvironment.ValueFactory.String(functionName)));
        main.Write(compiledEnvironment.OperationCodeFactory.Return());

        compiledEnvironment.Functions[$"{SpecialVariables.Global}::main"] = main;

        var interpreter = new Interpreter(compiledEnvironment);
        return interpreter.Interpret();
    }

    public static ValueBase RunFile(string path, ScriptEnvironment environment = null)
    {
        var imports = Enumerable.Empty<string>();
        try
        {
            imports = new ImportResolver().ResolveImportsFromFile(path);
        }
        catch (FileNotFoundException e)
        {
            var folder = Path.GetDirectoryName(e.FileName);
            var file = Path.GetFileName(e.FileName);
            var scriptsInFolder = new DirectoryInfo(folder).GetFiles("*.ei")
                .Select(f => f.Name).Where(f => Math.Abs(f.Length - file.Length) < 3);

            throw new FileNotFoundException($"Could not find '{file}'. Did you mean:\n{string.Join('\n', scriptsInFolder)}", e.FileName);
        }

        var code = new ImportMerger().Merge(imports);

        ILexer finalLexer;
        if (code.Count > 1)
        {
            var lexers = new List<ILexer>();
            foreach (var imported in code)
            {
                var reader = new ScriptReader(imported.Code, imported.Path, imported.LineOffset);
                var lexer = new ScriptLexer(reader, new CommonLexer(reader));
                lexers.Add(lexer);
            }

            finalLexer = new MultifileLexer(lexers);
        }
        else
        {
            var imported = code.First();
            var reader = new ScriptReader(imported.Code, imported.Path, imported.LineOffset);
            finalLexer = new ScriptLexer(reader, new CommonLexer(reader));
        }

        var parser = new Parser(finalLexer);
        var ast = parser.Parse();

        if (environment == null)
        {
            environment = new ScriptEnvironment(new OperationCodeFactory());
            environment.AddClassesDerivedFromClassInAssembly(typeof(Eilang));
            environment.AddExportedFunctionsFromAssembly(typeof(Eilang));
            environment.AddExportedModulesFromAssembly(typeof(Eilang));
        }

        Compiler.Compile(environment, ast
#if LOGGING
                ,logger: Console.Out
#endif
        );

        var interpreter = new Interpreter(environment
#if LOGGING
                ,logger: Console.Out
#endif
        );
        return interpreter.Interpret();
    }

    public static ValueBase Eval(string code, ScriptEnvironment environment = null)
    {
        var reader = new ScriptReader(code, "eval");
        var lexer = new ScriptLexer(reader, new CommonLexer(reader));
        var parser = new Parser(lexer);
        var ast = parser.Parse();

        if (environment == null)
        {
            environment = new ScriptEnvironment(new OperationCodeFactory());
            environment.AddClassesDerivedFromClassInAssembly(typeof(Eilang));
            environment.AddExportedFunctionsFromAssembly(typeof(Eilang));
            environment.AddExportedModulesFromAssembly(typeof(Eilang));
        }

        Compiler.Compile(environment, ast
#if LOGGING
                ,logger: Console.Out
#endif
        );

        var interpreter = new Interpreter(environment
#if LOGGING
                ,logger: Console.Out
#endif
        );
        return interpreter.Interpret();
    }

    public static void ReplMode()
    {
        var environment = new ReplEnvironment(new OperationCodeFactory());
        var interpreter = new ReplInterpreter();
        environment.AddClassesDerivedFromClassInAssembly(typeof(Eilang));
        environment.AddExportedFunctionsFromAssembly(typeof(Eilang));
        environment.AddExportedModulesFromAssembly(typeof(Eilang));

        while (true)
        {
            Console.Write(">");
            var code = Console.ReadLine() ?? "";
            if (string.IsNullOrWhiteSpace(code))
                continue;
            try
            {
                code = Simplify(code);
                var reader = new ScriptReader(code, "repl");
                var lexer = new ScriptLexer(reader, new CommonLexer(reader));
                var parser = new Parser(lexer);
                var ast = parser.Parse();

                Compiler.Compile(environment, ast);

                var eval = interpreter.Interpret(environment);
                if (eval.Type != EilangType.Void)
                {
                    ExportedFunctions.PrintLine(new State(environment, null, null, null, null), Arguments.Create(eval, "eval"));
                }
            }
            catch (ExitException e)
            {
                if (!string.IsNullOrWhiteSpace(e.Message))
                {
                    var oldColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(e.Message);
                    Console.ForegroundColor = oldColor;
                }
                Environment.Exit(e.ExitCode);
            }
            catch (ErrorMessageException)
            {
                throw;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }

    private static string Simplify(string code)
    {
        if (code.Trim().EndsWith(";") || code.Trim().EndsWith("}"))
        {
            return code;
        }
        else
        {
            if (Regex.IsMatch(code, "\bret\b"))
            {
                return $"{code};";
            }
            else
            {
                return $"({code});";
            }
        }
    }

    // TODO: make another method to compile from file and allow imports
    public static IEnvironment Compile(string code, IEnvironment environment = null)
    {
        var reader = new ScriptReader(code, "compiled");
        var lexer = new ScriptLexer(reader, new CommonLexer(reader));
        var parser = new Parser(lexer);
        var ast = parser.Parse();

        if (environment == null)
        {
            var env = new ScriptEnvironment(new OperationCodeFactory());
            // TODO: turn the following methods into extension methods on IEnvironment
            env.AddClassesDerivedFromClassInAssembly(typeof(Eilang));
            env.AddExportedFunctionsFromAssembly(typeof(Eilang));
            env.AddExportedModulesFromAssembly(typeof(Eilang));
            environment = env;
        }

        Compiler.Compile(environment, ast);

        return environment;
    }

    public static IEnvironment CompileWithImports(string path, ScriptEnvironment environment = null)
    {
        var imports = new ImportResolver().ResolveImportsFromFile(path);
        var code = new ImportMerger().Merge(imports);

        ILexer finalLexer;
        if (code.Count > 1)
        {
            var lexers = new List<ILexer>();
            foreach (var imported in code)
            {
                var reader = new ScriptReader(imported.Code, imported.Path, imported.LineOffset);
                var lexer = new ScriptLexer(reader, new CommonLexer(reader));
                lexers.Add(lexer);
            }

            finalLexer = new MultifileLexer(lexers);
        }
        else
        {
            var imported = code.First();
            var reader = new ScriptReader(imported.Code, imported.Path, imported.LineOffset);
            finalLexer = new ScriptLexer(reader, new CommonLexer(reader));
        }

        var parser = new Parser(finalLexer);
        var ast = parser.Parse();

        if (environment == null)
        {
            environment = new ScriptEnvironment(new OperationCodeFactory());
            environment.AddClassesDerivedFromClassInAssembly(typeof(Eilang));
            environment.AddExportedFunctionsFromAssembly(typeof(Eilang));
            environment.AddExportedModulesFromAssembly(typeof(Eilang));
        }

        Compiler.Compile(environment, ast
        #if LOGGING
            ,logger: Console.Out
        #endif
        );
        return environment;
    }

    public static void DumpBytecode(IEnvironment environment, string path, bool skipExportedModules = true)
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"Classes ({environment.Classes.Count} in total)");
        sb.AppendLine("-----------------------------------------------");

        foreach (var c in environment.Classes.Values)
        {
            if (skipExportedModules && c.Name.StartsWith("."))
            {
                continue;
            }
            sb.AppendLine($"typ {c.FullName} {{");
            var ind = 1;
            foreach (var ctor in c.Constructors)
            {
                sb.AppendIndentedLine(ind, $"ctor({string.Join(", ", ctor.Arguments)}) {{");
                ind++;
                foreach (var bc in ctor.Code)
                {
                    sb.AppendIndentedLine(ind, bc.ToString());
                }

                ind--;
                sb.AppendIndentedLine(ind, "}");
            }

            foreach (var f in c.Functions)
            {
                sb.AppendIndentedLine(ind, $"fun {f.Name}({string.Join(", ", f.Arguments)}) {{");
                ind++;
                foreach (var bc in f.Code)
                {
                    sb.AppendIndentedLine(ind, bc.ToString());
                }

                ind--;
                sb.AppendIndentedLine(ind, "}");
            }

            sb.AppendLine("}");
        }

        sb.AppendLine("Global functions");
        sb.AppendLine("-----------------------------------------------");
        foreach (var f in environment.Functions.Values)
        {
            var ind = 0;
            sb.AppendIndentedLine(ind, $"fun {f.Name}({string.Join(", ", f.Arguments)}) {{");
            ind++;
            foreach (var bc in f.Code)
            {
                sb.AppendIndentedLine(ind, bc.ToString());
            }

            ind--;
            sb.AppendIndentedLine(ind, "}");
        }

        File.WriteAllText(path, sb.ToString());
    }

}
