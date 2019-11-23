using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using eilang.Compiling;
using eilang.Exceptions;
using eilang.Imports;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Lexing;
using eilang.OperationCodes;
using eilang.Parsing;
using eilang.Values;

namespace eilang
{
    public static class Eilang
    {
        public static IValue RunFile(string path, ScriptEnvironment environment = null)
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
                environment = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
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

        public static IValue Run(string code, ScriptEnvironment environment = null)
        {
            var reader = new ScriptReader(code, "eval");
            var lexer = new ScriptLexer(reader, new CommonLexer(reader));
            var parser = new Parser(lexer);
            var ast = parser.Parse();

            if (environment == null)
            {
                environment = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
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
            var environment = new ReplEnvironment(new OperationCodeFactory(), new ValueFactory());
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
                    if (eval.Type != TypeOfValue.Void)
                    {
                        ExportedFunctions.PrintLine(new ValueFactory(), eval);
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
    }
}