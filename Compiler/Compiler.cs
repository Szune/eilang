using System.Collections.Generic;
using System.IO;

namespace eilang
{
    public class Compiler : IVisitor
    {
        private readonly Env _env;
        private readonly TextWriter _logger;

        public Compiler(Env env, TextWriter logger)
        {
            _env = env;
            _logger = logger;
        }

        private void Log(string msg)
        {
            _logger?.WriteLine(msg);
        }

        public static void Compile(Env env, AstRoot root, TextWriter logger = null)
        {
            var compiler = new Compiler(env, logger);
            compiler.Visit(root);
        }

        public void Visit(AstRoot root)
        {
            Log("Visiting root");
            _env.Global = new Module(".global");
            var func = new Function(".global", new List<string>());
            _env.Global.Functions.Add(func);
            root.Modules.Accept(this);
            root.Classes.Accept(this, _env.Global);
            root.Functions.Accept(this, _env.Global);
            root.Expressions.Accept(this, func);
        }

        public void Visit(AstModule module)
        {
            Log($"Visiting module {module.Name}");
            var mod = new Module(module.Name);
            module.Classes.Accept(this, mod);
            module.Functions.Accept(this, mod);
        }

        public void Visit(AstClass clas, Module mod)
        {
            var newClass = new Class(clas.Name);
            clas.Functions.Accept(this, newClass);
            mod.Classes.Add(newClass);
        }

        public void Visit(AstDeclarationAssignment assignment, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstAssignment assignment, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstDoubleConstant constant, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstFunctionCall funcCall, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstVariableReference reference, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstStringConstant constant, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstIntegerConstant constant, Function function)
        {
            throw new System.NotImplementedException();
        }

        public void Visit(AstFunction func, Module mod)
        {
            var newFunc = new Function(func.Name, func.Arguments);
            func.Expressions.Accept(this, newFunc);
            mod.Functions.Add(newFunc);
        }

        public void Visit(AstMemberFunction memberFunc, Class clas)
        {
            var func = new Function(memberFunc.Name, memberFunc.Arguments);
            memberFunc.Expressions.Accept(this, func);
            clas.Functions.Add(func);
        }
    }
}
