using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eilang.Ast;
using eilang.Classes;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Values;

namespace eilang.Compiling
{
    public class Compiler : IVisitor
    {
        public const string GlobalFunctionAndModuleName = ".global";
        private readonly Env _env;
        private readonly TextWriter _logger;
        private readonly IValueFactory _valueFactory;
        private readonly IOperationCodeFactory _opFactory;
        private int _forDepth = 0;

        /// <summary>
        /// key = loop depth (<see cref="_forDepth"/>)
        /// </summary>
        private readonly Dictionary<int, Stack<(int Index, int Type)>> _loopControlFlowOps =
            new Dictionary<int, Stack<(int Index, int Type)>>();

        private const int Break = 0xBAD;
        private const int Continue = 0xCAB;
        public const int InLoopReturn = 0xCAF0;

        public Compiler(Env env, TextWriter logger, IValueFactory valueFactory, IOperationCodeFactory opFactory)
        {
            _env = env;
            _logger = logger;
            _valueFactory = valueFactory;
            _opFactory = opFactory;
        }

        private void Log(string msg)
        {
            _logger?.WriteLine(msg);
        }

        public static void Compile(Env env, AstRoot root, IValueFactory valueFactory = null,
            IOperationCodeFactory opFactory = null, TextWriter logger = null)
        {
            var compiler = new Compiler(env, logger, valueFactory ?? new ValueFactory(),
                opFactory ?? new OperationCodeFactory());
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

        public void Visit(AstClassInitialization init, Function function, Module mod)
        {
            var fullName = init.Identifiers.Contains("::")
                ? init.Identifiers
                : $"{GlobalFunctionAndModuleName}::{init.Identifiers}";
            Log($"Compiling instance initialization '{fullName}'");
            init.Arguments.Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(init.Arguments.Count)));
            function.Write(_opFactory.Initialize(_valueFactory.String(fullName)));
        }

        public void Visit(AstMemberVariableDeclaration member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration for '{member.Ident}'");
            clas.CtorForMembersWithValues.Write(_opFactory.Push(_valueFactory.Void()));
            clas.CtorForMembersWithValues.Write(_opFactory.Define(_valueFactory.String(member.Ident)));
        }

        public void Visit(AstMemberVariableDeclarationWithInit member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration with initial value for '{member.Ident}'");
            member.InitExpr.Accept(this, clas.CtorForMembersWithValues, mod);
            clas.CtorForMembersWithValues.Write(_opFactory.Define(_valueFactory.String(member.Ident)));
        }

        public void Visit(AstConstructor ctor, Class clas, Module mod)
        {
            Log($"Compiling ctor for '{clas.Name}'");
            var newCtor = new MemberFunction(ctor.Name, "", ctor.Arguments, clas);
            newCtor.Write(_opFactory.Define(_valueFactory.String(SpecialVariables.Me)));
            foreach (var arg in ctor.Arguments)
            {
                newCtor.Write(_opFactory.Define(_valueFactory.String(arg)));
            }

            if (ctor.Expressions.Any())
            {
                ctor.Expressions.Accept(this, newCtor, mod);
            }

            newCtor.Write(_opFactory.Return());
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
                    function.Write(_opFactory.Add());
                    break;
                case BinaryMath.Minus:
                    function.Write(_opFactory.Subtract());
                    break;
                case BinaryMath.Times:
                    function.Write(_opFactory.Multiply());
                    break;
                case BinaryMath.Division:
                    function.Write(_opFactory.Divide());
                    break;
                case BinaryMath.Modulo:
                    function.Write(_opFactory.Modulo());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstTrue tr, Function function, Module mod)
        {
            Log($"Compiling true");
            function.Write(_opFactory.Push(_valueFactory.True()));
        }

        public void Visit(AstFalse fa, Function function, Module mod)
        {
            Log($"Compiling false");
            function.Write(_opFactory.Push(_valueFactory.False()));
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
            function.Write(_opFactory.JumpIfFalse(_valueFactory.Integer(0)));
            var jmpfOpCodeIndex = function.Code.Count - 1;

            aIf.IfExpr.Accept(this, function, mod);
            function.Write(_opFactory.Jump(_valueFactory.Integer(0)));
            var jmpOpCodeIndex = function.Code.Count - 1;
            var ifEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(_opFactory.JumpIfFalse(_valueFactory.Integer(ifEndIndex)));

            if (aIf.ElseExpr != null)
            {
                aIf.ElseExpr.Accept(this, function, mod);
                var elseEndIndex = function.Code.Count;
                function[jmpOpCodeIndex] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(elseEndIndex)));
            }
            else
            {
                function[jmpOpCodeIndex] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(ifEndIndex)));
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
                    function.Write(_opFactory.Or());
                    break;
                case Compare.And:
                    function.Write(_opFactory.And());
                    break;
                case Compare.EqualsEquals:
                    function.Write(_opFactory.Equals());
                    break;
                case Compare.NotEquals:
                    function.Write(_opFactory.NotEquals());
                    break;
                case Compare.LessThan:
                    function.Write(_opFactory.LessThan());
                    break;
                case Compare.GreaterThan:
                    function.Write(_opFactory.GreaterThan());
                    break;
                case Compare.LessThanEquals:
                    function.Write(_opFactory.LessThanOrEquals());
                    break;
                case Compare.GreaterThanEquals:
                    function.Write(_opFactory.GreaterThanOrEquals());
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

            function.Write(_opFactory.Push(_valueFactory.Integer(list.InitialItems.Count)));
            function.Write(_opFactory.ListNew());
        }

        public void Visit(AstIndexerReference indexer, Function function, Module mod)
        {
            Log($"Compiling indexer reference for variable '{indexer.Identifier}'");
            if (indexer.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{indexer.Identifier}' contained no indexer expressions.");
            function.Write(_opFactory.Reference(_valueFactory.String(indexer.Identifier)));
            function.Write(_opFactory.TypeGet());
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(1))); // arg count
            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                metadata: new Metadata {Variable = indexer.Identifier, IndexerDepth = 0});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(_opFactory.TypeGet());
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(_opFactory.Push(_valueFactory.Integer(1))); // arg count
                function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                    metadata: new Metadata {Variable = indexer.Identifier, IndexerDepth = i});
            }
        }


        public void Visit(AstReturn ret, Function function, Module mod)
        {
            if (ret.RetExpr != null)
                ret.RetExpr.Accept(this, function, mod);
            if (_forDepth > 0)
                AddControlFlowOp(_forDepth, (function.Length, InLoopReturn));
            function.Write(_opFactory.Return());
        }

        public void Visit(AstAssignmentValue assign, Function function, Module mod)
        {
            assign.Value.Accept(this, function, mod);
        }

        public void Visit(AstAssignment assignment, Function function, Module mod)
        {
            switch (assignment.Set.Type)
            {
                case AssignmentSet.MemberVariable:
                    assignment.Set.RequiredReferences.Accept(this, function, mod);
                    break;
                case AssignmentSet.Array:
                    assignment.Set.RequiredReferences.Accept(this, function, mod);
                    for (int i = 0; i < assignment.Set.IndexExprs.Count - 1; i++)
                    {
                        function.Write(_opFactory.TypeGet());
                        assignment.Set.IndexExprs[i].Accept(this, function, mod);
                        function.Write(_opFactory.Push(_valueFactory.Integer(1))); // arg count
                        function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")));
                    }

                    function.Write(_opFactory.TypeGet());
                    assignment.Set.IndexExprs[assignment.Set.IndexExprs.Count - 1].Accept(this, function, mod);
                    break;
            }

            switch (assignment.Value.Type)
            {
                case Assignment.Equals:
                    assignment.Value.Accept(this, function, mod);
                    switch (assignment.Set.Type)
                    {
                        case AssignmentSet.Array:
                            function.Write(_opFactory.Push(_valueFactory.Integer(2)));
                            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_set")));
                            break;
                        case AssignmentSet.Variable:
                            if (assignment.Define)
                            {
                                function.Write(_opFactory.Define(_valueFactory.String(assignment.Set.OptionalIdentifier)));
                            }
                            else
                            {
                                function.Write(_opFactory.Set(_valueFactory.String(assignment.Set.OptionalIdentifier)));
                            }
                            break;
                        case AssignmentSet.MemberVariable:
                            function.Write(
                                _opFactory.MemberSet(_valueFactory.String(assignment.Set.OptionalIdentifier)));
                            break;
                    }

                    return;
                case Assignment.DivideEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Divide());
                    break;
                case Assignment.TimesEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Multiply());
                    break;
                case Assignment.ModuloEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Modulo());
                    break;
                case Assignment.MinusEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Subtract());
                    break;
                case Assignment.PlusEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Add());
                    break;
                case Assignment.Increment:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Increment());
                    break;
                case Assignment.Decrement:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Decrement());
                    break;
                case Assignment.IncrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Increment());
                    break;
                case Assignment.DecrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Decrement());
                    break;
                default:
                    throw new CompilerException($"Unknown assignment type {assignment.Value.Type}");
            }

            switch (assignment.Set.Type)
            {
                case AssignmentSet.Array:
                    function.Write(_opFactory.Push(_valueFactory.Integer(2)));
                    function.Write(_opFactory.MemberCall(_valueFactory.String("idx_set")));
                    break;
                case AssignmentSet.Variable:
                    function.Write(_opFactory.Set(_valueFactory.String(assignment.Set.OptionalIdentifier)));
                    break;
                case AssignmentSet.MemberVariable:
                    function.Write(_opFactory.MemberSet(_valueFactory.String(assignment.Set.OptionalIdentifier)));
                    break;
            }

            switch (assignment.Value.Type)
            {
                case Assignment.IncrementAndReference:
                case Assignment.DecrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    break;
            }
        }

        public void Visit(AstAssignmentReference memberFunc, Function function, Module mod)
        {
            memberFunc.Reference.Accept(this, function, mod);
        }

        public void Visit(AstIdentifier identifier, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String(identifier.Ident)));
        }

        public void Visit(AstIx memberFunc, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")));
        }

        public void Visit(AstBreak memberFunc, Function function, Module mod)
        {
            AddControlFlowOp(_forDepth, (function.Length, Break));
            function.Write(_opFactory.Jump(_valueFactory.Integer(-1)));
        }


        public void Visit(AstContinue memberFunc, Function function, Module mod)
        {
            AddControlFlowOp(_forDepth, (function.Length, Continue));
            function.Write(_opFactory.Jump(_valueFactory.Integer(-1)));
        }


        private void AddControlFlowOp(int forDepth, (int Index, int Type) values)
        {
            if (!_loopControlFlowOps.ContainsKey(forDepth))
            {
                _loopControlFlowOps[forDepth] = new Stack<(int Index, int Type)>();
            }

            _loopControlFlowOps[forDepth].Push(values);
        }

        public void Visit(AstRange range, Function function, Module mod)
        {
            throw new NotImplementedException("should never arrive here");
        }


        public void Visit(AstForArray forArray, Function function, Module mod)
        {
            _forDepth++;
            function.Write(_opFactory.ScopeNew());
            forArray.Array.Accept(this, function, mod);
            // save array to a temporary faster lookup location
            function.Write(_opFactory.TemporarySet(_valueFactory.String($".aval{_forDepth}")));
            function.Write(_opFactory.TemporaryReference(_valueFactory.String($".aval{_forDepth}")));
            function.Write(_opFactory.TypeGet());
            function.Write(_opFactory.Push(_valueFactory.Integer(0)));
            function.Write(_opFactory.MemberCall(_valueFactory.String("len")));
            function.Write(_opFactory.TemporarySet(_valueFactory.String($".alen{_forDepth}")));
            function.Write(_opFactory.TemporaryReference(_valueFactory.String($".alen{_forDepth}")));
            function.Write(_opFactory.JumpIfZero(_valueFactory.Integer(0)));
            var addressOfFirstJmpZ = function.Code.Count - 1;
            if (!forArray.Reversed)
            {
                // start loop with index set to 0
                function.Write(_opFactory.Push(_valueFactory.Integer(0)));
            }
            else
            {
                // or index set to last item if reversed
                function.Write(_opFactory.TemporaryReference(_valueFactory.String($".alen{_forDepth}")));
                function.Write(_opFactory.Push(_valueFactory.Integer(1)));
                function.Write(_opFactory.Subtract());
            }

            function.Write(_opFactory.Define(_valueFactory.String($".ix{_forDepth}")));
            // define 'it' variable
            function.Write(_opFactory.Push(_valueFactory.Void()));
            function.Write(_opFactory.Define(_valueFactory.String($".it{_forDepth}")));
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")));
            var addressOfCmp = function.Code.Count - 1;
            // loop for the length of the array
            if (!forArray.Reversed)
            {
                function.Write(_opFactory.TemporaryReference(_valueFactory.String($".alen{_forDepth}")));
                function.Write(_opFactory.GreaterThanOrEquals());
            }
            else
            {
                function.Write(_opFactory.Push(_valueFactory.Integer(0)));
                function.Write(_opFactory.LessThan());
            }

            function.Write(_opFactory.JumpIfTrue(_valueFactory.Integer(0)));
            var addressOfJmpT = function.Code.Count - 1;
            // set 'it' to the value of array at current index 
            function.Write(_opFactory.TemporaryReference(_valueFactory.String($".aval{_forDepth}")));
            function.Write(_opFactory.TypeGet());
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")));
            function.Write(_opFactory.Push(_valueFactory.Integer(1)));
            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")));
            function.Write(_opFactory.Set(_valueFactory.String($".it{_forDepth}")));
            forArray.Body.Accept(this, function, mod);
            var addressOfLoopStep = function.Length;
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")));
            function.Write(_opFactory.Push(_valueFactory.Integer(1)));
            // increment or decrement index variable
            if (forArray.Reversed)
                function.Write(_opFactory.Subtract());
            else
                function.Write(_opFactory.Add());
            function.Write(_opFactory.Set(_valueFactory.String($".ix{_forDepth}")));
            function.Write(_opFactory.Jump(_valueFactory.Integer(addressOfCmp)));
            var endOfLoop = function.Code.Count;
            function.Write(_opFactory.ScopePop());
            function[addressOfJmpT] = new Bytecode(_opFactory.JumpIfTrue(_valueFactory.Integer(endOfLoop)));
            function[addressOfFirstJmpZ] = new Bytecode(_opFactory.JumpIfZero(_valueFactory.Integer(endOfLoop)));
            AssignLoopControlFlowJumps(function, _forDepth, addressOfLoopStep, endOfLoop);
            _forDepth--;
        }


        public void Visit(AstForRange forRange, Function function, Module mod)
        {
            _forDepth++;
            function.Write(_opFactory.ScopeNew());
            forRange.Range.Begin.Accept(this, function, mod);
            forRange.Range.End.Accept(this, function, mod);
            if (forRange.Reversed)
                function.Write(_opFactory.LessThan());
            else
                function.Write(_opFactory.GreaterThan());
            function.Write(_opFactory.JumpIfTrue(_valueFactory.Integer(0)));
            var addressOfFirstJmpT = function.Code.Count - 1;
            forRange.Range.Begin.Accept(this, function, mod);
            function.Write(_opFactory.Define(_valueFactory.String($".ix{_forDepth}")));
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")));
            var addressOfCmp = function.Code.Count - 1;
            forRange.Range.End.Accept(this, function, mod);
            if (forRange.Reversed)
                function.Write(_opFactory.LessThan());
            else
                function.Write(_opFactory.GreaterThan());
            function.Write(_opFactory.JumpIfTrue(_valueFactory.Integer(0)));
            var addressOfJmpT = function.Code.Count - 1;
            forRange.Body.Accept(this, function, mod);
            var addressOfLoopStep = function.Length;
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")));
            function.Write(_opFactory.Push(_valueFactory.Integer(1)));
            if (forRange.Reversed)
                function.Write(_opFactory.Subtract());
            else
                function.Write(_opFactory.Add());
            function.Write(_opFactory.Set(_valueFactory.String($".ix{_forDepth}")));
            function.Write(_opFactory.Jump(_valueFactory.Integer(addressOfCmp)));
            var endOfLoop = function.Length;
            function.Write(_opFactory.ScopePop());
            function[addressOfJmpT] = new Bytecode(_opFactory.JumpIfTrue(_valueFactory.Integer(endOfLoop)));
            function[addressOfFirstJmpT] = new Bytecode(_opFactory.JumpIfTrue(_valueFactory.Integer(endOfLoop)));
            AssignLoopControlFlowJumps(function, _forDepth, addressOfLoopStep, endOfLoop);
            _forDepth--;
        }

        public void Visit(AstForInfinite forInfinite, Function function, Module mod)
        {
            _forDepth++;
            function.Write(_opFactory.ScopeNew());
            var addressOfLoopStart = function.Length;
            forInfinite.Body.Accept(this, function, mod);
            function.Write(_opFactory.Jump(_valueFactory.Integer(addressOfLoopStart)));
            var endOfLoop = function.Length;
            function.Write(_opFactory.ScopePop());
            AssignLoopControlFlowJumps(function, _forDepth, addressOfLoopStart, endOfLoop);
            _forDepth--;
        }

        public void Visit(AstIndex index, Function function, Module mod)
        {
            index.Index.Accept(this, function, mod);
        }

        public void Visit(AstMe me, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String(SpecialVariables.Me)));
        }

        public void Visit(AstTernary ternary, Function function, Module mod)
        {
            Log($"Compiling ternary operator");
            ternary.Condition.Accept(this, function, mod);
            function.Write(_opFactory.JumpIfFalse(_valueFactory.Integer(0)));
            var jmpfOpCodeIndex = function.Code.Count - 1;

            ternary.TrueExpr.Accept(this, function, mod);
            function.Write(_opFactory.Jump(_valueFactory.Integer(0)));
            var jmpOpCodeIndex = function.Code.Count - 1;
            var trueEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(_opFactory.JumpIfFalse(_valueFactory.Integer(trueEndIndex)));

            ternary.FalseExpr.Accept(this, function, mod);
            var falseEndIndex = function.Code.Count;
            function[jmpOpCodeIndex] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(falseEndIndex)));
        }

        public void Visit(AstFunctionPointer funcPointer, Function function, Module mod)
        {
            function.Write(_opFactory.Push(_valueFactory.FunctionPointer(funcPointer.Ident)));
        }

        private void AssignLoopControlFlowJumps(Function function, int forDepth, int loopStep, int loopEnd)
        {
            if (!_loopControlFlowOps.TryGetValue(forDepth, out var stack) || stack.Count <= 0)
                return;
            while (stack.Any())
            {
                var flow = stack.Pop();
                switch (flow.Type)
                {
                    case Break:
                        function.Code[flow.Index] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(loopEnd)));
                        break;
                    case Continue:
                        function.Code[flow.Index] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(loopStep)));
                        break;
                    case InLoopReturn:
                        function.Code[flow.Index] = new Bytecode(_opFactory.Return(forDepth));
                        break;
                    default:
                        throw new CompilerException("Unknown control flow type of value " + flow.Type);
                }
            }
        }

        public void Visit(AstIt it, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String($".it{_forDepth}")));
        }

        public void Visit(AstUnaryMathOperation unary, Function function, Module mod)
        {
            unary.Expr.Accept(this, function, mod);
            switch (unary.Op)
            {
                case UnaryMath.Minus:
                    function.Write(_opFactory.Negate());
                    break;
                case UnaryMath.Not:
                    function.Write(_opFactory.Not());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstMemberReference memberFunc, Function function, Module mod)
        {
            function.Write(_opFactory.MemberReference(_valueFactory.String(memberFunc.Ident)));
        }

        public void Visit(AstMultiReference memberFunc, Function function, Module mod)
        {
            memberFunc.First.Accept(this, function, mod);
            memberFunc.Second.Accept(this, function, mod);
        }

        public void Visit(AstMemberFunctionCall memberFunc, Function function, Module mod)
        {
            function.Write(_opFactory.TypeGet());
            memberFunc.Arguments.Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(memberFunc.Arguments.Count)));
            function.Write(_opFactory.MemberCall(_valueFactory.String(memberFunc.Ident)));
        }

        public void Visit(AstMemberIndexerRef indexer, Function function, Module mod)
        {
            function.Write(_opFactory.MemberReference(_valueFactory.String(indexer.Ident)));
            function.Write(_opFactory.TypeGet());
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(1))); // arg count
            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                metadata: new Metadata {Variable = indexer.Ident, IndexerDepth = 0});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(_opFactory.TypeGet());
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(_opFactory.Push(_valueFactory.Integer(1))); // arg count
                function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                    metadata: new Metadata {Variable = indexer.Ident, IndexerDepth = i});
            }
        }


        public void Visit(AstClass clas, Module mod)
        {
            Log($"Compiling class declaration '{clas.Name}'");
            var newClass = new Class(clas.Name, mod.Name);
            newClass.CtorForMembersWithValues.Write(_opFactory.Define(_valueFactory.String(SpecialVariables.Me)));
            clas.Variables.Accept(this, newClass, mod);
            newClass.CtorForMembersWithValues.Write(_opFactory.Return());
            clas.Functions.Accept(this, newClass, mod);
            clas.Constructors.Accept(this, newClass, mod);
            _env.Classes.Add(newClass.FullName, newClass);
        }

        public void Visit(AstDeclarationAssignment assignment, Function function, Module mod)
        {
            Log($"Compiling variable declaration assignment '{assignment.Ident}'");
            assignment.Value.Accept(this, function, mod);
            function.Write(_opFactory.Define(_valueFactory.String(assignment.Ident)));
        }

        public void Visit(AstFunctionCall funcCall, Function function, Module mod)
        {
            Log($"Compiling function call '{funcCall.Name}'");
            funcCall.Arguments.Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(funcCall.Arguments.Count)));
            if (_env.ExportedFunctions.ContainsKey(funcCall.Name))
            {
                function.Write(_opFactory.ExportedCall(_valueFactory.String(funcCall.Name)));
            }
            else
            {
                function.Write(_opFactory.Call(_valueFactory.String(funcCall.Name)));
            }
        }

        public void Visit(AstStringConstant constant, Function function, Module mod)
        {
            Log($"Compiling string constant '{constant.String}'");
            function.Write(_opFactory.Push(_valueFactory.String(constant.String)));
        }

        public void Visit(AstIntegerConstant constant, Function function, Module mod)
        {
            Log($"Compiling integer constant '{constant.Integer}'");
            function.Write(_opFactory.Push(_valueFactory.Integer(constant.Integer)));
        }

        public void Visit(AstDoubleConstant constant, Function function, Module mod)
        {
            Log($"Compiling double constant '{constant.Double}'");
            function.Write(_opFactory.Push(_valueFactory.Double(constant.Double)));
        }

        public void Visit(AstFunction func, Module mod)
        {
            Log($"Compiling function declaration '{func.Name}'");
            var newFunc = new Function(func.Name, mod.Name, func.Arguments);
            for (int i = func.Arguments.Count - 1; i > -1; i--)
            {
                newFunc.Write(_opFactory.Define(_valueFactory.String(func.Arguments[i])));
            }

            func.Expressions.Accept(this, newFunc, mod);
            if (newFunc.Length < 1)
            {
                newFunc.Write(_opFactory.Return());
            }
            else if (newFunc[newFunc.Length - 1].Op != _opFactory.Return())
            {
                newFunc.Write(_opFactory.Return());
            }

            if (_env.Functions.ContainsKey(newFunc.FullName))
                throw new CompilerException(
                    $"Function '{newFunc.Name}' has already been declared in namespace '{mod.Name}'.");
            _env.Functions.Add(newFunc.FullName, newFunc);
        }


        public void Visit(AstMemberFunction memberFunc, Class clas, Module mod)
        {
            Log($"Compiling member function declaration '{memberFunc.Name}'");
            var func = new MemberFunction(memberFunc.Name, clas.FullName, memberFunc.Arguments, clas);

            func.Write(_opFactory.Define(_valueFactory.String(SpecialVariables.ArgumentCount)));
            for (int i = memberFunc.Arguments.Count - 1; i > -1; i--)
            {
                func.Write(_opFactory.Define(_valueFactory.String(memberFunc.Arguments[i])));
            }

            memberFunc.Expressions.Accept(this, func, mod);
            if (func.Length < 1)
            {
                func.Write(_opFactory.Return());
            }
            else if (func[func.Length - 1].Op.GetType().Name != _opFactory.Return().GetType().Name)
            {
                func.Write(_opFactory.Return());
            }

            clas.Functions.Add(func.Name, func);
        }
    }
}