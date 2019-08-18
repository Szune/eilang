using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
            var globalMod = new Module(GlobalFunctionAndModuleName);
            var func = new Function(GlobalFunctionAndModuleName, GlobalFunctionAndModuleName, new List<string>());
            _env.Functions.Add(func.FullName, func);
            root.Modules.Accept(this);
            root.Classes.Accept(this, globalMod);
            root.Functions.Accept(this, globalMod);
            root.Expressions.Accept(this, func, globalMod);
            Log("Compilation finished.");
        }

        public void Visit(AstModule module)
        {
            Log($"Compiling module declaration '{module.Name}'");
            var mod = new Module(module.Name);
            module.Classes.Accept(this, mod);
            module.Functions.Accept(this, mod);
            //_env.Modules.Add(mod.Name, mod);
        }

        public void Visit(AstMemberVariableReference memberFunc, Function function, Module mod)
        {
            Log($"Compiling member variable reference '{string.Join(".", memberFunc.Identifiers)}'");
            throw new NotImplementedException();
        }

        public void Visit(AstMemberVariableAssignment memberFunc, Function function, Module mod)
        {
            Log($"Compiling member variable assignment '{string.Join(".", memberFunc.Identifiers)}'");
            throw new NotImplementedException();
        }

        public void Visit(AstMemberFunctionCall memberFunc, Function function, Module mod)
        {
            Log($"Compiling member function call '{string.Join(".", memberFunc.Identifiers)}'");
            memberFunc.Arguments.Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(memberFunc.Arguments.Count));
            
            // 1st identifier = variable ref
            function.Write(OpCode.REF, _valueFactory.String(memberFunc.Identifiers[0]));
            // 2nd..n-1 identifier = member ref, but only if there are more than 2 identifiers (otherwise, 2nd is method name)
            if (memberFunc.Identifiers.Count > 2)
            {
                for (int i = 1; i < memberFunc.Identifiers.Count - 1; i++)
                    function.Write(OpCode.MREF, _valueFactory.String(memberFunc.Identifiers[i]));
            }
            
            function.Write(OpCode.TYPEGET); // get type of current value on stack, so we can operate on its class with the instance

            // last identifier = method name
            function.Write(OpCode.MCALL, _valueFactory.String(memberFunc.Identifiers[memberFunc.Identifiers.Count-1]));
        }

        public void Visit(AstClassInitialization init, Function function, Module mod)
        {
            var fullName = init.Identifiers.Count > 1
                ? GetFullName(init.Identifiers)
                : $"{GlobalFunctionAndModuleName}::{init.Identifiers[0].Ident}";
            Log($"Compiling instance initialization '{fullName}'");
            init.Arguments.Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(init.Arguments.Count));
            function.Write(OpCode.INIT, _valueFactory.String(fullName));
        }

        public void Visit(AstMemberVariableDeclaration member, Class function, Module mod)
        {
            throw new NotImplementedException();
        }

        public void Visit(AstMemberVariableDeclarationWithInit member, Class function, Module mod)
        {
            throw new NotImplementedException();
        }


        public void Visit(AstClass clas, Module mod)
        {
            Log($"Compiling class declaration '{clas.Name}'");
            var newClass = new Class(clas.Name, mod.Name);
            clas.Functions.Accept(this, newClass, mod);
            _env.Classes.Add(newClass.FullName, newClass);
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
            _env.Functions.Add(newFunc.FullName, newFunc);
        }


        public void Visit(AstMemberFunction memberFunc, Class clas, Module mod)
        {
            Log($"Compiling member function declaration '{memberFunc.Name}'");
            var func = new MemberFunction(memberFunc.Name, clas.FullName, memberFunc.Arguments, clas);
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

        private string GetFullName(List<Reference> idents)
        {
            if(idents.Count < 2)
                throw new InvalidOperationException("Can only use GetFullName when idents > 1");
            var full = idents[0].Ident + (idents[0].IsModule ? "::" : ".");
            for (int i = 1; i < idents.Count - 2; i++)
            {
                full += idents[i].Ident + (idents[i].IsModule ? "::" : ".");
            }

            full += idents[idents.Count - 1].Ident;
            return full;
        }
    }
}
