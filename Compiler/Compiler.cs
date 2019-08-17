using System;
using System.Collections.Generic;
using System.IO;

namespace eilang
{
    public class Compiler : IVisitor
    {
        public const string GlobalFunctionAndModuleName = ".global";

        private readonly Env _env;
        private readonly TextWriter _logger;
        private readonly IValueFactory _valueFactory;

        public Compiler(Env env, TextWriter logger, IValueFactory valueFactory)
        {
            _env = env;
            _logger = logger;
            _valueFactory = valueFactory;
        }

        private void Log(string msg)
        {
            _logger?.WriteLine(msg);
        }

        public static void Compile(Env env, AstRoot root, IValueFactory valueFactory = null, TextWriter logger = null)
        {
            var compiler = new Compiler(env, logger, valueFactory ?? new ValueFactory());
            compiler.Visit(root);
        }

        public void Visit(AstRoot root)
        {
            Log("Compiling...");
            Log("Compiling ast root");
            _env.Global = new Module(GlobalFunctionAndModuleName);
            var func = new Function(GlobalFunctionAndModuleName, GlobalFunctionAndModuleName, new List<string>());
            _env.Global.Functions.Add(func.Name, func);
            root.Modules.Accept(this);
            root.Classes.Accept(this, _env.Global);
            root.Functions.Accept(this, _env.Global);
            root.Expressions.Accept(this, func, _env.Global);
            Log("Compilation finished.");
        }

        public void Visit(AstModule module)
        {
            Log($"Compiling module declaration '{module.Name}'");
            var mod = new Module(module.Name);
            module.Classes.Accept(this, mod);
            module.Functions.Accept(this, mod);
            _env.Modules.Add(mod.Name, mod);
        }


        public void Visit(AstClass clas, Module mod)
        {
            Log($"Compiling class declaration '{clas.Name}'");
            var newClass = new Class(clas.Name, mod.Name);
            clas.Functions.Accept(this, newClass, mod);
            mod.Classes.Add(newClass.Name, newClass);
        }

        public void Visit(AstDeclarationAssignment assignment, Function function, Module mod)
        {
            Log($"Compiling variable declaration assignment '{assignment.Ident}'");
            assignment.Value.Accept(this, function, mod);
            function.Write(OpCode.DEF, _valueFactory.String(assignment.Ident));
        }

        public void Visit(AstAssignment assignment, Function function, Module mod)
        {
            Log($"Compiling variable assignment '{assignment.Ident}'");
            assignment.Value.Accept(this, function, mod);
            function.Write(OpCode.SET, _valueFactory.String(assignment.Ident));
        }

        public void Visit(AstFunctionCall funcCall, Function function, Module mod)
        {
            Log($"Compiling function call '{funcCall.Name}'");
            funcCall.Arguments.Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(funcCall.Arguments.Count));
            if(_env.ExportedFuncs.ContainsKey(funcCall.Name))
            {
                function.Write(OpCode.ECALL, _valueFactory.String(funcCall.Name));
            }
            else
            {
                function.Write(OpCode.CALL, _valueFactory.String(funcCall.Name));
            }
        }

        public void Visit(AstVariableReference reference, Function function, Module mod)
        {
            Log($"Compiling variable reference '{reference.Ident}'");
            function.Write(OpCode.REF, _valueFactory.String(reference.Ident));
        }

        public void Visit(AstStringConstant constant, Function function, Module mod)
        {
            Log($"Compiling string constant '{constant.String}'");
            function.Write(OpCode.PUSH, _valueFactory.String(constant.String));
        }

        public void Visit(AstIntegerConstant constant, Function function, Module mod)
        {
            Log($"Compiling integer constant '{constant.Integer}'");
            function.Write(OpCode.PUSH, _valueFactory.Integer(constant.Integer));
        }

        public void Visit(AstDoubleConstant constant, Function function, Module mod)
        {
            Log($"Compiling double constant '{constant.Double}'");
            function.Write(OpCode.PUSH, _valueFactory.Double(constant.Double));
        }

        public void Visit(AstFunction func, Module mod)
        {
            Log($"Compiling function declaration '{func.Name}'");
            var newFunc = new Function(func.Name, mod.Name, func.Arguments);
            func.Expressions.Accept(this, newFunc, mod);
            if(newFunc.Length < 1)
            {
                newFunc.Write(OpCode.RET);
            }
            else if(newFunc[newFunc.Length-1].Op != OpCode.RET)
            {
                newFunc.Write(OpCode.RET);
            }
            mod.Functions.Add(func.Name, newFunc);
        }


        public void Visit(AstMemberFunction memberFunc, Class clas, Module mod)
        {
            Log($"Compiling member function declaration '{memberFunc.Name}'");
            var func = new Function(memberFunc.Name, clas.FullName, memberFunc.Arguments);
            memberFunc.Expressions.Accept(this, func, mod);
            if(func.Length < 1)
            {
                func.Write(OpCode.RET);
            }
            else if(func[func.Length-1].Op != OpCode.RET)
            {
                func.Write(OpCode.RET);
            }
            clas.Functions.Add(func.Name, func);
        }
    }
}
