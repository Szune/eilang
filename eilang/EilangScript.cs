﻿using eilang.Classes;
using eilang.Compiling;
using eilang.Imports;
using eilang.Interpreting;
using eilang.Lexing;
using eilang.Parsing;
using eilang.Values;

namespace eilang
{
    public static class EilangScript
    {
        public static void RunFile(string path, Env environment = null)
        {
            
            var imports = new ImportResolver().ResolveImportsFromFile(path);
            var code = new ImportMerger().Merge(imports);

            var reader = new ScriptReader(code);
            var lexer = new ScriptLexer(reader, new CommonLexer(reader));
            var parser = new Parser(lexer);
            var ast = parser.Parse();

#if LOGGING
            var walker = new AstWalker(ast);
            walker.PrintAst();
#endif

            var env = new Env(new ValueFactory());
            env.AddClassesDerivedFromClassInAssembly<Class>();
            env.AddExportedFunctionsFrom(typeof(ExportedFunctions));

            Compiler.Compile(env, ast
#if LOGGING
                ,logger: Console.Out
#endif
            );

            var interpreter = new Interpreter(env
#if LOGGING
                ,logger: Console.Out
#endif
            );
            interpreter.Interpret();
        }
        
        public static void Run(string code, Env environment = null)
        {
            var reader = new ScriptReader(code);
            var lexer = new ScriptLexer(reader, new CommonLexer(reader));
            var parser = new Parser(lexer);
            var ast = parser.Parse();

#if LOGGING
            var walker = new AstWalker(ast);
            walker.PrintAst();
#endif

            if (environment == null)
            {
                environment = new Env(new ValueFactory());
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
            interpreter.Interpret();
        }
    }
}