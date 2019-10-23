using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Emit;
using eilang.Ast;
using eilang.Classes;

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

        public void Visit(AstMemberVariableReference member, Function function, Module mod)
        {
            Log($"Compiling member variable reference '{string.Join(".", member.Identifiers)}'");
            
            // 1st identifier = variable ref
            function.Write(OpCode.REF, _valueFactory.String(member.Identifiers[0]));
            // 2nd..n identifier = member ref
            for (int i = 1; i < member.Identifiers.Count; i++)
                function.Write(OpCode.MREF, _valueFactory.String(member.Identifiers[i]));
        }

        public void Visit(AstMemberVariableAssignment member, Function function, Module mod)
        {
            Log($"Compiling member variable assignment '{string.Join(".", member.Identifiers)}'");
            // 1st identifier = variable ref
            function.Write(OpCode.REF, _valueFactory.String(member.Identifiers[0]));
            // 2nd..n-1 identifier = member ref
            for (int i = 1; i < member.Identifiers.Count - 1; i++)
                function.Write(OpCode.MREF, _valueFactory.String(member.Identifiers[i]));
            
            member.Value.Accept(this, function, mod);
            function.Write(OpCode.MSET, _valueFactory.String(member.Identifiers[member.Identifiers.Count-1]));
        }

        public void Visit(AstMemberFunctionCall memberFunc, Function function, Module mod)
        {
            Log($"Compiling member function call '{string.Join(".", memberFunc.Identifiers)}'");
            // 1st identifier = variable ref
            function.Write(OpCode.REF, _valueFactory.String(memberFunc.Identifiers[0]));
            // 2nd..n-1 identifier = member ref, but only if there are more than 2 identifiers (otherwise, 2nd is method name)
            if (memberFunc.Identifiers.Count > 2)
            {
                for (int i = 1; i < memberFunc.Identifiers.Count - 1; i++)
                    function.Write(OpCode.MREF, _valueFactory.String(memberFunc.Identifiers[i]));
            }
            
            function.Write(OpCode.TYPEGET); // get type of current value on stack, so we can operate on its class with the instance
            
            memberFunc.Arguments.Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(memberFunc.Arguments.Count));
            
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

        public void Visit(AstMemberVariableDeclaration member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration for '{member.Ident}'");
            clas.CtorForMembersWithValues.Write(OpCode.PUSH, _valueFactory.Void());
            clas.CtorForMembersWithValues.Write(OpCode.DEF, _valueFactory.String(member.Ident));
        }

        public void Visit(AstMemberVariableDeclarationWithInit member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration with initial value for '{member.Ident}'");
            member.InitExpr.Accept(this, clas.CtorForMembersWithValues, mod);
            clas.CtorForMembersWithValues.Write(OpCode.DEF, _valueFactory.String(member.Ident));
        }

        public void Visit(AstConstructor ctor, Class clas, Module mod)
        {
            Log($"Compiling ctor for '{clas.Name}'");
            var newCtor = new MemberFunction(ctor.Name, "", ctor.Arguments, clas);
            foreach (var arg in ctor.Arguments)
            {
                newCtor.Write(OpCode.DEF, _valueFactory.String(arg));
            }
            newCtor.Write(OpCode.RET);
            clas.Constructors.Add(newCtor);
        }

        public void Visit(AstBinaryMathOperation math, Function function, Module mod)
        {
            Log($"Compiling binary math '{math.Op}'");
            math.Left.Accept(this, function, mod);
            math.Right.Accept(this, function, mod);
            switch (math.Op)
            {
                case BinaryMath.Plus:
                    function.Write(OpCode.ADD);
                    break;
                case BinaryMath.Minus:
                    function.Write(OpCode.SUB);
                    break;
                case BinaryMath.Times:
                    function.Write(OpCode.MUL);
                    break;
                case BinaryMath.Division:
                    function.Write(OpCode.DIV);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
        }

        public void Visit(AstTrue tr, Function function, Module mod)
        {
            Log($"Compiling true");
            function.Write(OpCode.PUSH, _valueFactory.True());
        }

        public void Visit(AstFalse fa, Function function, Module mod)
        {
            Log($"Compiling false");
            function.Write(OpCode.PUSH, _valueFactory.False());
        }

        public void Visit(AstBlock block, Function function, Module mod)
        {
            Log($"Compiling block");
            block.Expressions.Accept(this, function, mod);
        }

        public void Visit(AstIf aIf, Function function, Module mod)
        {
            Log($"Compiling compare if statement");
            aIf.Condition.Accept(this, function, mod);
            function.Write(OpCode.JMPF, _valueFactory.Integer(0));
            var jmpfOpCodeIndex = function.Code.Count - 1;
            
            aIf.IfExpr.Accept(this, function, mod);
            function.Write(OpCode.JMP, _valueFactory.Integer(0));
            var jmpOpCodeIndex = function.Code.Count - 1;
            var ifEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(OpCode.JMPF, _valueFactory.Integer(ifEndIndex));

            if (aIf.ElseExpr != null)
            {
                aIf.ElseExpr.Accept(this, function, mod);
                var elseEndIndex = function.Code.Count;
                function[jmpOpCodeIndex] = new Bytecode(OpCode.JMP, _valueFactory.Integer(elseEndIndex));
            }
            else
            {
                function[jmpOpCodeIndex] = new Bytecode(OpCode.JMP, _valueFactory.Integer(ifEndIndex));
            }
        }

        public void Visit(AstCompare compare, Function function, Module mod)
        {
            Log($"Compiling compare '{compare.Comparison}'");
            compare.Left.Accept(this, function, mod);
            compare.Right.Accept(this, function, mod);

            switch (compare.Comparison)
            {
                case Compare.Or:
                    function.Write(OpCode.OR);
                    break;
                case Compare.And:
                    function.Write(OpCode.AND);
                    break;
                case Compare.EqualsEquals:
                    function.Write(OpCode.EQ);
                    break;
                case Compare.NotEquals:
                    function.Write(OpCode.NEQ);
                    break;
                case Compare.LessThan:
                    function.Write(OpCode.LT);
                    break;
                case Compare.GreaterThan:
                    function.Write(OpCode.GT);
                    break;
                case Compare.LessThanEquals:
                    function.Write(OpCode.LTE);
                    break;
                case Compare.GreaterThanEquals:
                    function.Write(OpCode.GTE);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstNewList list, Function function, Module mod)
        {
            Log($"Compiling new list with '{list.InitialItems.Count}' items");
            for (int i = list.InitialItems.Count - 1; i > -1; i--)
            {
                list.InitialItems[i].Accept(this, function, mod);
            }
            function.Write(OpCode.PUSH, _valueFactory.Integer(list.InitialItems.Count));
            function.Write(OpCode.NLIST);
        }

        public void Visit(AstIndexerReference indexer, Function function, Module mod)
        {
            Log($"Compiling indexer reference for variable '{indexer.Identifier}'");
            if (indexer.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{indexer.Identifier}' contained no indexer expressions.");
            function.Write(OpCode.REF, _valueFactory.String(indexer.Identifier));
            function.Write(OpCode.TYPEGET);
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
            function.Write(OpCode.MCALL, _valueFactory.String("idx_get"), 
                metadata: new Metadata{ Variable = indexer.Identifier, IndexerDepth = 0});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                // set tmp var to top of stack
                //function.Write(OpCode.TMPV, _valueFactory.String(indexer.Identifier));
                // ref tmp var
                //function.Write(OpCode.TMPR, _valueFactory.String(indexer.Identifier));
                function.Write(OpCode.TYPEGET);
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata {Variable = indexer.Identifier, IndexerDepth = i});
            }

            if (indexer.IndexExprs.Count > 1)
            {
                // clear tmp var
                //function.Write(OpCode.TMPC, _valueFactory.String(indexer.Identifier));
            }
        }

        public void Visit(AstIndexerAssignment assign, Function function, Module mod)
        {
            Log($"Compiling indexer assignment for variable '{assign.Identifier}'");
            if (assign.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{assign.Identifier}' contained no indexer expressions.");
            function.Write(OpCode.REF, _valueFactory.String(assign.Identifier));
            function.Write(OpCode.TYPEGET);
            
            assign.IndexExprs[0].Accept(this, function, mod);
            
            if(assign.IndexExprs.Count == 1)
                assign.ValueExpr.Accept(this, function, mod);
            
            if (assign.IndexExprs.Count > 1)
            {
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
            }
            else
            {
                // only one reference, so assign to first
                function.Write(OpCode.PUSH, _valueFactory.Integer(2)); // arg count
            }
            
            if (assign.IndexExprs.Count > 1)
            {
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata {Variable = assign.Identifier, IndexerDepth = 0});
            }
            else
            {
                function.Write(OpCode.MCALL, _valueFactory.String("idx_set"));
                return;
            }
            for (int i = 1; i < assign.IndexExprs.Count - 1; i++)
            {
                // set tmp var to top of stack
                //function.Write(OpCode.TMPV, _valueFactory.String(assign.Identifier));
                // ref tmp var
                //function.Write(OpCode.TMPR, _valueFactory.String(assign.Identifier));
                function.Write(OpCode.TYPEGET);
                assign.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata {Variable = assign.Identifier, IndexerDepth = i});
            }
            
            // setup idx_set call
            //function.Write(OpCode.TMPV, _valueFactory.String(assign.Identifier));
            // ref tmp var
            //function.Write(OpCode.TMPR, _valueFactory.String(assign.Identifier));
            
            function.Write(OpCode.TYPEGET);
            assign.ValueExpr.Accept(this, function, mod);
            assign.IndexExprs[assign.IndexExprs.Count-1].Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(2)); // arg count
            function.Write(OpCode.MCALL, _valueFactory.String("idx_set"));

            // clear tmp var
            //function.Write(OpCode.TMPC, _valueFactory.String(assign.Identifier));
        }

        public void Visit(AstReturn ret, Function function, Module mod)
        {
            if(ret.RetExpr != null)
                ret.RetExpr.Accept(this, function, mod);
            function.Write(OpCode.RET);
        }

        public void Visit(AstMemberIndexerReference indexer, Function function, Module mod)
        {
            Log($"Compiling member indexer reference for variable '{indexer.Identifiers[0]}'");
            // 1st identifier = variable ref
            function.Write(OpCode.REF, _valueFactory.String(indexer.Identifiers[0]));
            // 2nd..n identifier = member ref
            for (int i = 1; i < indexer.Identifiers.Count; i++)
                function.Write(OpCode.MREF, _valueFactory.String(indexer.Identifiers[i]));
            if (indexer.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{indexer.Identifiers[0]}' contained no indexer expressions.");
            //function.Write(OpCode.TMPV, _valueFactory.String(indexer.Identifiers[0]));
            //function.Write(OpCode.TMPR, _valueFactory.String(indexer.Identifiers[0]));
            function.Write(OpCode.TYPEGET);
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
            function.Write(OpCode.MCALL, _valueFactory.String("idx_get"), 
                metadata: new Metadata{Variable = indexer.Identifiers[0], IndexerDepth = 0});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                // set tmp var to top of stack
                //function.Write(OpCode.TMPV, _valueFactory.String(indexer.Identifiers[0]));
                // ref tmp var
                //function.Write(OpCode.TMPR, _valueFactory.String(indexer.Identifiers[0]));
                function.Write(OpCode.TYPEGET);
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata{Variable = indexer.Identifiers[0], IndexerDepth = 0});
            }

            if (indexer.IndexExprs.Count > 1)
            {
                // clear tmp var
                //function.Write(OpCode.TMPC, _valueFactory.String(indexer.Identifiers[0]));
            }
        }

        public void Visit(AstMemberIndexerAssignment assign, Function function, Module mod)
        {
            Log($"Compiling member indexer assignment for variable '{assign.Identifiers[0]}'");
            // 1st identifier = variable ref
            function.Write(OpCode.REF, _valueFactory.String(assign.Identifiers[0]));
            // 2nd..n identifier = member ref
            for (int i = 1; i < assign.Identifiers.Count; i++)
                function.Write(OpCode.MREF, _valueFactory.String(assign.Identifiers[i]));
            if (assign.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{assign.Identifiers[0]}' contained no indexer expressions.");
            //function.Write(OpCode.TMPV, _valueFactory.String(assign.Identifiers[0]));
            //function.Write(OpCode.TMPR, _valueFactory.String(assign.Identifiers[0]));
            function.Write(OpCode.TYPEGET);
            assign.IndexExprs[0].Accept(this, function, mod);
            if(assign.IndexExprs.Count == 1)
                assign.ValueExpr.Accept(this, function, mod);
            if (assign.IndexExprs.Count > 1)
            {
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
            }
            else
            {
                // only one reference, so assign to first
                function.Write(OpCode.PUSH, _valueFactory.Integer(2)); // arg count
            }
            if (assign.IndexExprs.Count > 1)
            {
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata{Variable = assign.Identifiers[0], IndexerDepth = 0});
            }
            else
            {
                function.Write(OpCode.MCALL, _valueFactory.String("idx_set"));
                return;
            }
            for (int i = 1; i < assign.IndexExprs.Count - 1; i++)
            {
                // set tmp var to top of stack
                //function.Write(OpCode.TMPV, _valueFactory.String(assign.Identifiers[0]));
                // ref tmp var
                //function.Write(OpCode.TMPR, _valueFactory.String(assign.Identifiers[0]));
                function.Write(OpCode.TYPEGET);
                assign.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata{Variable = assign.Identifiers[0], IndexerDepth = 0});
            }
            
            // setup idx_set call
            //function.Write(OpCode.TMPV, _valueFactory.String(assign.Identifiers[0]));
            // ref tmp var
            //function.Write(OpCode.TMPR, _valueFactory.String(assign.Identifiers[0]));
            function.Write(OpCode.TYPEGET);
            assign.ValueExpr.Accept(this, function, mod);
            assign.IndexExprs[assign.IndexExprs.Count-1].Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(2)); // arg count
            function.Write(OpCode.MCALL, _valueFactory.String("idx_set"));

            // clear tmp var
            //function.Write(OpCode.TMPC, _valueFactory.String(assign.Identifiers[0]));
        }

        public void Visit(AstRange range, Function function, Module mod)
        {
            throw new NotImplementedException();
        }

        public void Visit(AstForRange forRange, Function function, Module mod)
        {
            function.Write(OpCode.NSCP);
            forRange.Range.Begin.Accept(this, function, mod);
            forRange.Range.End.Accept(this, function, mod);
            function.Write(OpCode.GT);
            function.Write(OpCode.JMPT, _valueFactory.Integer(0));
            var addressOfFirstJmpT = function.Code.Count - 1;
            forRange.Range.Begin.Accept(this, function, mod);
            function.Write(OpCode.DEF, _valueFactory.String(".it"));
            function.Write(OpCode.REF, _valueFactory.String(".it"));
            var addressOfCmp = function.Code.Count - 1;
            forRange.Range.End.Accept(this, function, mod);
            function.Write(OpCode.GT);
            function.Write(OpCode.JMPT, _valueFactory.Integer(0));
            var addressOfJmpT = function.Code.Count - 1;
            forRange.Body.Accept(this, function, mod);
            function.Write(OpCode.REF, _valueFactory.String(".it"));
            function.Write(OpCode.PUSH, _valueFactory.Integer(1));
            function.Write(OpCode.ADD);
            function.Write(OpCode.SET, _valueFactory.String(".it"));
            function.Write(OpCode.JMP, _valueFactory.Integer(addressOfCmp));
            var endOfLoop = function.Code.Count;
            function.Write(OpCode.PSCP);
            function[addressOfJmpT] = new Bytecode(OpCode.JMPT, _valueFactory.Integer(endOfLoop));
            function[addressOfFirstJmpT] = new Bytecode(OpCode.JMPT, _valueFactory.Integer(endOfLoop));
        }

        public void Visit(AstIt it, Function function, Module mod)
        {
            function.Write(OpCode.REF, _valueFactory.String(".it"));
        }

        public void Visit(AstUnaryMathOperation unary, Function function, Module mod)
        {
            unary.Expr.Accept(this, function, mod);
            switch (unary.Op)
            {
                case UnaryMath.Minus:
                    function.Write(OpCode.NEG);
                    break;
                case UnaryMath.Not:
                    function.Write(OpCode.NOT);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstMemberReference memberFunc, Function function, Module mod)
        {
            function.Write(OpCode.MREF, _valueFactory.String(memberFunc.Ident));
        }

        public void Visit(AstMultiReference memberFunc, Function function, Module mod)
        {
            memberFunc.First.Accept(this, function, mod);
            memberFunc.Second.Accept(this, function, mod);
        }

        public void Visit(AstMemberCall memberFunc, Function function, Module mod)
        {
            function.Write(OpCode.TYPEGET);
            memberFunc.Arguments.Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(memberFunc.Arguments.Count));
            function.Write(OpCode.MCALL, _valueFactory.String(memberFunc.Ident));
        }

        public void Visit(AstMemberAssignment memberFunc, Function function, Module mod)
        {
            memberFunc.Assignment.Accept(this, function, mod);
            function.Write(OpCode.MSET, _valueFactory.String(memberFunc.Ident));
        }

        public void Visit(AstMemberIndexerRef indexer, Function function, Module mod)
        {
            function.Write(OpCode.MREF, _valueFactory.String(indexer.Ident));
            function.Write(OpCode.TYPEGET);
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
            function.Write(OpCode.MCALL, _valueFactory.String("idx_get"), 
                metadata: new Metadata{ Variable = indexer.Ident, IndexerDepth = 0});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(OpCode.TYPEGET);
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpCode.PUSH, _valueFactory.Integer(1)); // arg count
                function.Write(OpCode.MCALL, _valueFactory.String("idx_get"),
                    metadata: new Metadata {Variable = indexer.Ident, IndexerDepth = i});
            }
        }

        public void Visit(AstClass clas, Module mod)
        {
            Log($"Compiling class declaration '{clas.Name}'");
            var newClass = new Class(clas.Name, mod.Name);
            clas.Variables.Accept(this, newClass, mod);
            newClass.CtorForMembersWithValues.Write(OpCode.RET);
            clas.Functions.Accept(this, newClass, mod);
            clas.Constructors.Accept(this, newClass, mod);
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
            for (int i = func.Arguments.Count - 1; i > -1; i--)
            {
                newFunc.Write(OpCode.DEF, _valueFactory.String(func.Arguments[i]));
            }

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
            for (int i = memberFunc.Arguments.Count - 1; i > -1; i--)
            {
                func.Write(OpCode.DEF, _valueFactory.String(memberFunc.Arguments[i]));
            }

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
