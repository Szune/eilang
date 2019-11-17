using System.Collections.Generic;
using System.Linq;
using eilang.Classes;
using eilang.Compiling;
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
        public static IValue RunFile(string path, Env environment = null)
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
                environment = new Env(new OperationCodeFactory(), new ValueFactory());
                environment.AddClassesDerivedFromClassInAssembly<Class>();
                environment.AddExportedFunctionsFrom(typeof(ExportedFunctions));
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
        
        public static IValue Run(string code, Env environment = null)
        {
            var reader = new ScriptReader(code, "eval");
            var lexer = new ScriptLexer(reader, new CommonLexer(reader));
            var parser = new Parser(lexer);
            var ast = parser.Parse();

            if (environment == null)
            {
                environment = new Env(new OperationCodeFactory(), new ValueFactory());
                environment.AddClassesDerivedFromClassInAssembly<Class>();
                environment.AddExportedFunctionsFrom(typeof(ExportedFunctions));
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
    }
}