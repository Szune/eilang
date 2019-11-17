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
            function.Write(_opFactory.Push(_valueFactory.Integer(init.Arguments.Count)), new Metadata { Ast = init});
            function.Write(_opFactory.Initialize(_valueFactory.String(fullName)), new Metadata {Ast = init});
        }

        public void Visit(AstMemberVariableDeclaration member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration for '{member.Ident}'");
            clas.CtorForMembersWithValues.Write(_opFactory.Push(_valueFactory.Uninitialized()), new Metadata{Ast = member});
            clas.CtorForMembersWithValues.Write(_opFactory.Define(_valueFactory.String(member.Ident)), new Metadata{Ast = member});
        }

        public void Visit(AstMemberVariableDeclarationWithInit member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration with initial value for '{member.Ident}'");
            member.InitExpr.Accept(this, clas.CtorForMembersWithValues, mod);
            clas.CtorForMembersWithValues.Write(_opFactory.Define(_valueFactory.String(member.Ident)), new Metadata{Ast = member});
        }

        public void Visit(AstConstructor ctor, Class clas, Module mod)
        {
            Log($"Compiling ctor for '{clas.Name}'");
            var newCtor = new MemberFunction(ctor.Name, "", ctor.Arguments, clas);
            newCtor.Write(_opFactory.Define(_valueFactory.String(SpecialVariables.Me)), new Metadata{Ast = ctor});
            foreach (var arg in ctor.Arguments)
            {
                newCtor.Write(_opFactory.Define(_valueFactory.String(arg)), new Metadata{Ast = ctor});
            }

            if (ctor.Expressions.Any())
            {
                ctor.Expressions.Accept(this, newCtor, mod);
            }

            newCtor.Write(_opFactory.Return(), new Metadata{Ast = ctor});
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
                    function.Write(_opFactory.Add(), new Metadata{Ast = math});
                    break;
                case BinaryMath.Minus:
                    function.Write(_opFactory.Subtract(), new Metadata{Ast = math});
                    break;
                case BinaryMath.Times:
                    function.Write(_opFactory.Multiply(), new Metadata{Ast = math});
                    break;
                case BinaryMath.Division:
                    function.Write(_opFactory.Divide(), new Metadata{Ast = math});
                    break;
                case BinaryMath.Modulo:
                    function.Write(_opFactory.Modulo(), new Metadata{Ast = math});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstTrue tr, Function function, Module mod)
        {
            Log($"Compiling true");
            function.Write(_opFactory.Push(_valueFactory.True()), new Metadata{Ast = tr});
        }

        public void Visit(AstFalse fa, Function function, Module mod)
        {
            Log($"Compiling false");
            function.Write(_opFactory.Push(_valueFactory.False()), new Metadata{Ast = fa});
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
            function.Write(_opFactory.JumpIfFalse(_valueFactory.Integer(0)), new Metadata{Ast = aIf});
            var jmpfOpCodeIndex = function.Code.Count - 1;

            aIf.IfExpr.Accept(this, function, mod);
            function.Write(_opFactory.Jump(_valueFactory.Integer(0)), new Metadata{Ast = aIf});
            var jmpOpCodeIndex = function.Code.Count - 1;
            var ifEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(_opFactory.JumpIfFalse(_valueFactory.Integer(ifEndIndex)), new Metadata{Ast = aIf});

            if (aIf.ElseExpr != null)
            {
                aIf.ElseExpr.Accept(this, function, mod);
                var elseEndIndex = function.Code.Count;
                function[jmpOpCodeIndex] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(elseEndIndex)), new Metadata{Ast = aIf});
            }
            else
            {
                function[jmpOpCodeIndex] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(ifEndIndex)), new Metadata{Ast = aIf});
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
                    function.Write(_opFactory.Or(), new Metadata{Ast = compare});
                    break;
                case Compare.And:
                    function.Write(_opFactory.And(), new Metadata{Ast = compare});
                    break;
                case Compare.EqualsEquals:
                    function.Write(_opFactory.Equals(), new Metadata{Ast = compare});
                    break;
                case Compare.NotEquals:
                    function.Write(_opFactory.NotEquals(), new Metadata{Ast = compare});
                    break;
                case Compare.LessThan:
                    function.Write(_opFactory.LessThan(), new Metadata{Ast = compare});
                    break;
                case Compare.GreaterThan:
                    function.Write(_opFactory.GreaterThan(), new Metadata{Ast = compare});
                    break;
                case Compare.LessThanEquals:
                    function.Write(_opFactory.LessThanOrEquals(), new Metadata{Ast = compare});
                    break;
                case Compare.GreaterThanEquals:
                    function.Write(_opFactory.GreaterThanOrEquals(), new Metadata{Ast = compare});
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

            function.Write(_opFactory.Push(_valueFactory.Integer(list.InitialItems.Count)), new Metadata{Ast = list});
            function.Write(_opFactory.ListNew(), new Metadata{Ast = list});
        }

        public void Visit(AstIndexerReference indexer, Function function, Module mod)
        {
            Log($"Compiling indexer reference for variable '{indexer.Identifier}'");
            if (indexer.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{indexer.Identifier}' contained no indexer expressions.");
            function.Write(_opFactory.Reference(_valueFactory.String(indexer.Identifier)), new Metadata{Ast = indexer});
            function.Write(_opFactory.TypeGet(), new Metadata{Ast = indexer});
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = indexer}); // arg count
            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                new Metadata {Variable = indexer.Identifier, IndexerDepth = 0, Ast = indexer});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(_opFactory.TypeGet(), new Metadata{Ast = indexer});
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = indexer}); // arg count
                function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                    new Metadata {Variable = indexer.Identifier, IndexerDepth = i, Ast = indexer});
            }
        }


        public void Visit(AstReturn ret, Function function, Module mod)
        {
            if (ret.RetExpr != null)
                ret.RetExpr.Accept(this, function, mod);
            if (_forDepth > 0)
                AddControlFlowOp(_forDepth, (function.Length, InLoopReturn));
            function.Write(_opFactory.Return(), new Metadata{Ast = ret});
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
                        function.Write(_opFactory.TypeGet(), new Metadata{Ast = assignment});
                        assignment.Set.IndexExprs[i].Accept(this, function, mod);
                        function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = assignment}); // arg count
                        function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")), new Metadata{Ast = assignment});
                    }

                    function.Write(_opFactory.TypeGet(), new Metadata{Ast = assignment});
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
                            function.Write(_opFactory.Push(_valueFactory.Integer(2)), new Metadata{Ast = assignment});
                            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_set")), new Metadata{Ast = assignment});
                            break;
                        case AssignmentSet.Variable:
                            if (assignment.Define)
                            {
                                function.Write(_opFactory.Define(_valueFactory.String(assignment.Set.OptionalIdentifier)), new Metadata{Ast = assignment});
                            }
                            else
                            {
                                function.Write(_opFactory.Set(_valueFactory.String(assignment.Set.OptionalIdentifier)), new Metadata{Ast = assignment});
                            }
                            break;
                        case AssignmentSet.MemberVariable:
                            function.Write(
                                _opFactory.MemberSet(_valueFactory.String(assignment.Set.OptionalIdentifier)), new Metadata{Ast = assignment});
                            break;
                    }

                    return;
                case Assignment.DivideEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Divide(), new Metadata{Ast = assignment});
                    break;
                case Assignment.TimesEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Multiply(), new Metadata{Ast = assignment});
                    break;
                case Assignment.ModuloEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Modulo(), new Metadata{Ast = assignment});
                    break;
                case Assignment.MinusEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Subtract(), new Metadata{Ast = assignment});
                    break;
                case Assignment.PlusEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(_opFactory.Add(), new Metadata{Ast = assignment});
                    break;
                case Assignment.Increment:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Increment(), new Metadata{Ast = assignment});
                    break;
                case Assignment.Decrement:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Decrement(), new Metadata{Ast = assignment});
                    break;
                case Assignment.IncrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Increment(), new Metadata{Ast = assignment});
                    break;
                case Assignment.DecrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(_opFactory.Decrement(), new Metadata{Ast = assignment});
                    break;
                default:
                    throw new CompilerException($"Unknown assignment type {assignment.Value.Type}");
            }

            switch (assignment.Set.Type)
            {
                case AssignmentSet.Array:
                    function.Write(_opFactory.Push(_valueFactory.Integer(2)), new Metadata{Ast = assignment});
                    function.Write(_opFactory.MemberCall(_valueFactory.String("idx_set")), new Metadata{Ast = assignment});
                    break;
                case AssignmentSet.Variable:
                    function.Write(_opFactory.Set(_valueFactory.String(assignment.Set.OptionalIdentifier)), new Metadata{Ast = assignment});
                    break;
                case AssignmentSet.MemberVariable:
                    function.Write(_opFactory.MemberSet(_valueFactory.String(assignment.Set.OptionalIdentifier)), new Metadata{Ast = assignment});
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
            function.Write(_opFactory.Reference(_valueFactory.String(identifier.Ident)), new Metadata{Ast = identifier});
        }

        public void Visit(AstIx ix, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = ix});
        }

        public void Visit(AstBreak astBreak, Function function, Module mod)
        {
            AddControlFlowOp(_forDepth, (function.Length, Break));
            function.Write(_opFactory.Jump(_valueFactory.Integer(-1)), new Metadata{Ast = astBreak});
        }


        public void Visit(AstContinue astContinue, Function function, Module mod)
        {
            AddControlFlowOp(_forDepth, (function.Length, Continue));
            function.Write(_opFactory.Jump(_valueFactory.Integer(-1)), new Metadata{Ast = astContinue});
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
            function.Write(_opFactory.ScopeNew(), new Metadata{Ast = forArray});
            forArray.Array.Accept(this, function, mod);
            // save array to a temporary faster lookup location
            function.Write(_opFactory.TemporarySet(_valueFactory.String($".aval{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.TemporaryReference(_valueFactory.String($".aval{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.TypeGet(), new Metadata{Ast = forArray});
            function.Write(_opFactory.Push(_valueFactory.Integer(0)), new Metadata{Ast = forArray});
            function.Write(_opFactory.MemberCall(_valueFactory.String("len")), new Metadata{Ast = forArray});
            function.Write(_opFactory.TemporarySet(_valueFactory.String($".alen{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.TemporaryReference(_valueFactory.String($".alen{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.JumpIfZero(_valueFactory.Integer(0)), new Metadata{Ast = forArray});
            var addressOfFirstJmpZ = function.Code.Count - 1;
            if (!forArray.Reversed)
            {
                // start loop with index set to 0
                function.Write(_opFactory.Push(_valueFactory.Integer(0)), new Metadata{Ast = forArray});
            }
            else
            {
                // or index set to last item if reversed
                function.Write(_opFactory.TemporaryReference(_valueFactory.String($".alen{_forDepth}")), new Metadata{Ast = forArray});
                function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = forArray});
                function.Write(_opFactory.Subtract(), new Metadata{Ast = forArray});
            }

            function.Write(_opFactory.Define(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forArray});
            // define 'it' variable
            function.Write(_opFactory.Push(_valueFactory.Uninitialized()), new Metadata{Ast = forArray});
            function.Write(_opFactory.Define(_valueFactory.String($".it{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forArray});
            var addressOfCmp = function.Code.Count - 1;
            // loop for the length of the array
            if (!forArray.Reversed)
            {
                function.Write(_opFactory.TemporaryReference(_valueFactory.String($".alen{_forDepth}")), new Metadata{Ast = forArray});
                function.Write(_opFactory.GreaterThanOrEquals(), new Metadata{Ast = forArray});
            }
            else
            {
                function.Write(_opFactory.Push(_valueFactory.Integer(0)), new Metadata{Ast = forArray});
                function.Write(_opFactory.LessThan(), new Metadata{Ast = forArray});
            }

            function.Write(_opFactory.JumpIfTrue(_valueFactory.Integer(0)), new Metadata{Ast = forArray});
            var addressOfJmpT = function.Code.Count - 1;
            // set 'it' to the value of array at current index 
            function.Write(_opFactory.TemporaryReference(_valueFactory.String($".aval{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.TypeGet(), new Metadata{Ast = forArray});
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = forArray});
            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")), new Metadata{Ast = forArray});
            function.Write(_opFactory.Set(_valueFactory.String($".it{_forDepth}")), new Metadata{Ast = forArray});
            forArray.Body.Accept(this, function, mod);
            var addressOfLoopStep = function.Length;
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = forArray});
            // increment or decrement index variable
            if (forArray.Reversed)
                function.Write(_opFactory.Subtract(), new Metadata{Ast = forArray});
            else
                function.Write(_opFactory.Add(), new Metadata{Ast = forArray});
            function.Write(_opFactory.Set(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forArray});
            function.Write(_opFactory.Jump(_valueFactory.Integer(addressOfCmp)), new Metadata{Ast = forArray});
            var endOfLoop = function.Code.Count;
            function.Write(_opFactory.ScopePop(), new Metadata{Ast = forArray});
            function[addressOfJmpT] = new Bytecode(_opFactory.JumpIfTrue(_valueFactory.Integer(endOfLoop)), new Metadata{Ast = forArray});
            function[addressOfFirstJmpZ] = new Bytecode(_opFactory.JumpIfZero(_valueFactory.Integer(endOfLoop)), new Metadata{Ast = forArray});
            AssignLoopControlFlowJumps(forArray, function, _forDepth, addressOfLoopStep, endOfLoop);
            _forDepth--;
        }


        public void Visit(AstForRange forRange, Function function, Module mod)
        {
            _forDepth++;
            function.Write(_opFactory.ScopeNew(), new Metadata{Ast = forRange});
            forRange.Range.Begin.Accept(this, function, mod);
            forRange.Range.End.Accept(this, function, mod);
            if (forRange.Reversed)
                function.Write(_opFactory.LessThan(), new Metadata{Ast = forRange});
            else
                function.Write(_opFactory.GreaterThan(), new Metadata{Ast = forRange});
            function.Write(_opFactory.JumpIfTrue(_valueFactory.Integer(0)), new Metadata{Ast = forRange});
            var addressOfFirstJmpT = function.Code.Count - 1;
            forRange.Range.Begin.Accept(this, function, mod);
            function.Write(_opFactory.Define(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forRange});
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forRange});
            var addressOfCmp = function.Code.Count - 1;
            forRange.Range.End.Accept(this, function, mod);
            if (forRange.Reversed)
                function.Write(_opFactory.LessThan(), new Metadata{Ast = forRange});
            else
                function.Write(_opFactory.GreaterThan(), new Metadata{Ast = forRange});
            function.Write(_opFactory.JumpIfTrue(_valueFactory.Integer(0)), new Metadata{Ast = forRange});
            var addressOfJmpT = function.Code.Count - 1;
            forRange.Body.Accept(this, function, mod);
            var addressOfLoopStep = function.Length;
            function.Write(_opFactory.Reference(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forRange});
            function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = forRange});
            if (forRange.Reversed)
                function.Write(_opFactory.Subtract(), new Metadata{Ast = forRange});
            else
                function.Write(_opFactory.Add(), new Metadata{Ast = forRange});
            function.Write(_opFactory.Set(_valueFactory.String($".ix{_forDepth}")), new Metadata{Ast = forRange});
            function.Write(_opFactory.Jump(_valueFactory.Integer(addressOfCmp)), new Metadata{Ast = forRange});
            var endOfLoop = function.Length;
            function.Write(_opFactory.ScopePop(), new Metadata{Ast = forRange});
            function[addressOfJmpT] = new Bytecode(_opFactory.JumpIfTrue(_valueFactory.Integer(endOfLoop)), new Metadata{Ast = forRange});
            function[addressOfFirstJmpT] = new Bytecode(_opFactory.JumpIfTrue(_valueFactory.Integer(endOfLoop)), new Metadata{Ast = forRange});
            AssignLoopControlFlowJumps(forRange, function, _forDepth, addressOfLoopStep, endOfLoop);
            _forDepth--;
        }

        public void Visit(AstForInfinite forInfinite, Function function, Module mod)
        {
            _forDepth++;
            function.Write(_opFactory.ScopeNew(), new Metadata{Ast = forInfinite});
            var addressOfLoopStart = function.Length;
            forInfinite.Body.Accept(this, function, mod);
            function.Write(_opFactory.Jump(_valueFactory.Integer(addressOfLoopStart)), new Metadata{Ast = forInfinite});
            var endOfLoop = function.Length;
            function.Write(_opFactory.ScopePop(), new Metadata{Ast = forInfinite});
            AssignLoopControlFlowJumps(forInfinite, function, _forDepth, addressOfLoopStart, endOfLoop);
            _forDepth--;
        }

        public void Visit(AstIndex index, Function function, Module mod)
        {
            index.Index.Accept(this, function, mod);
        }

        public void Visit(AstMe me, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String(SpecialVariables.Me)), new Metadata{Ast = me});
        }

        public void Visit(AstTernary ternary, Function function, Module mod)
        {
            Log($"Compiling ternary operator");
            ternary.Condition.Accept(this, function, mod);
            function.Write(_opFactory.JumpIfFalse(_valueFactory.Integer(0)), new Metadata{Ast = ternary});
            var jmpfOpCodeIndex = function.Code.Count - 1;

            ternary.TrueExpr.Accept(this, function, mod);
            function.Write(_opFactory.Jump(_valueFactory.Integer(0)), new Metadata{Ast = ternary});
            var jmpOpCodeIndex = function.Code.Count - 1;
            var trueEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(_opFactory.JumpIfFalse(_valueFactory.Integer(trueEndIndex)), new Metadata{Ast = ternary});

            ternary.FalseExpr.Accept(this, function, mod);
            var falseEndIndex = function.Code.Count;
            function[jmpOpCodeIndex] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(falseEndIndex)), new Metadata{Ast = ternary});
        }

        public void Visit(AstFunctionPointer funcPointer, Function function, Module mod)
        {
            function.Write(_opFactory.Push(_valueFactory.FunctionPointer(funcPointer.Ident)), new Metadata{Ast = funcPointer});
        }

        public void Visit(AstParenthesized parenthesized, Function function, Module mod)
        {
            parenthesized.Expr.Accept(this, function, mod);
        }

        private void AssignLoopControlFlowJumps(IAst ast, Function function, int forDepth, int loopStep, int loopEnd)
        {
            if (!_loopControlFlowOps.TryGetValue(forDepth, out var stack) || stack.Count <= 0)
                return;
            while (stack.Any())
            {
                var flow = stack.Pop();
                switch (flow.Type)
                {
                    case Break:
                        function.Code[flow.Index] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(loopEnd)), new Metadata{Ast = ast});
                        break;
                    case Continue:
                        function.Code[flow.Index] = new Bytecode(_opFactory.Jump(_valueFactory.Integer(loopStep)), new Metadata{Ast = ast});
                        break;
                    case InLoopReturn:
                        function.Code[flow.Index] = new Bytecode(_opFactory.Return(forDepth), new Metadata{Ast = ast});
                        break;
                    default:
                        throw new CompilerException("Unknown control flow type of value " + flow.Type);
                }
            }
        }

        public void Visit(AstIt it, Function function, Module mod)
        {
            function.Write(_opFactory.Reference(_valueFactory.String($".it{_forDepth}")), new Metadata{Ast = it});
        }

        public void Visit(AstUnaryMathOperation unary, Function function, Module mod)
        {
            unary.Expr.Accept(this, function, mod);
            switch (unary.Op)
            {
                case UnaryMath.Minus:
                    function.Write(_opFactory.Negate(), new Metadata{Ast = unary});
                    break;
                case UnaryMath.Not:
                    function.Write(_opFactory.Not(), new Metadata{Ast = unary});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstMemberReference memberReference, Function function, Module mod)
        {
            function.Write(_opFactory.MemberReference(_valueFactory.String(memberReference.Ident)), new Metadata{Ast = memberReference});
        }

        public void Visit(AstMultiReference memberFunc, Function function, Module mod)
        {
            memberFunc.First.Accept(this, function, mod);
            memberFunc.Second.Accept(this, function, mod);
        }

        public void Visit(AstMemberFunctionCall memberFuncCall, Function function, Module mod)
        {
            function.Write(_opFactory.TypeGet(), new Metadata{Ast = memberFuncCall});
            memberFuncCall.Arguments.Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(memberFuncCall.Arguments.Count)), new Metadata{Ast = memberFuncCall});
            function.Write(_opFactory.MemberCall(_valueFactory.String(memberFuncCall.Ident)), new Metadata{Ast = memberFuncCall});
        }

        public void Visit(AstMemberIndexerRef indexer, Function function, Module mod)
        {
            function.Write(_opFactory.MemberReference(_valueFactory.String(indexer.Ident)), new Metadata{Ast = indexer});
            function.Write(_opFactory.TypeGet(), new Metadata{Ast = indexer});
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = indexer}); // arg count
            function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                new Metadata {Variable = indexer.Ident, IndexerDepth = 0, Ast = indexer});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(_opFactory.TypeGet(), new Metadata{Ast = indexer});
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(_opFactory.Push(_valueFactory.Integer(1)), new Metadata{Ast = indexer}); // arg count
                function.Write(_opFactory.MemberCall(_valueFactory.String("idx_get")),
                    new Metadata {Variable = indexer.Ident, IndexerDepth = i, Ast = indexer});
            }
        }


        public void Visit(AstClass clas, Module mod)
        {
            Log($"Compiling class declaration '{clas.Name}'");
            var newClass = new Class(clas.Name, mod.Name);
            newClass.CtorForMembersWithValues.Write(_opFactory.Define(_valueFactory.String(SpecialVariables.Me)), new Metadata{Ast = clas});
            clas.Variables.Accept(this, newClass, mod);
            newClass.CtorForMembersWithValues.Write(_opFactory.Return(), new Metadata{Ast = clas});
            clas.Functions.Accept(this, newClass, mod);
            clas.Constructors.Accept(this, newClass, mod);
            InsertMemberValueInitializationInConstructors(newClass, clas);
            _env.Classes.Add(newClass.FullName, newClass);
        }

        private void InsertMemberValueInitializationInConstructors(Class clas, AstClass ast)
        {
            foreach (var ctor in clas.Constructors)
            {
                if (ConstructorInitializesAllMembers(ctor.Arguments, ast.Variables))
                { 
                    // skip constructors that already initialize all member variables
                    continue;
                }
                // figure out which member variables still need to be initialized and find the code that initializes them
                // and inject it into the start of the current constructor
                for (int i = 0; i < clas.CtorForMembersWithValues.Length; i++)
                {
                    ctor.Code.Insert(i, clas.CtorForMembersWithValues[i]);
                }
            }
        }

        private bool ConstructorInitializesAllMembers(List<string> constructorArguments, List<AstMemberVariableDeclaration> memberVariables)
        {
            // if constructorArguments contains all the members in memberVariables, we don't need to initialize any more variables,
            // because they will have been initialized by the constructor anyway, which would overwrite any pre-initialized value
            // it's of course fine if the constructor takes additional parameters that are not pre-defined class members though
            return memberVariables.TrueForAll(v => constructorArguments.Contains(v.Ident));
        }

        public void Visit(AstDeclarationAssignment assignment, Function function, Module mod)
        {
            Log($"Compiling variable declaration assignment '{assignment.Ident}'");
            assignment.Value.Accept(this, function, mod);
            function.Write(_opFactory.Define(_valueFactory.String(assignment.Ident)), new Metadata{Ast = assignment});
        }

        public void Visit(AstFunctionCall funcCall, Function function, Module mod)
        {
            Log($"Compiling function call '{funcCall.Name}'");
            funcCall.Arguments.Accept(this, function, mod);
            function.Write(_opFactory.Push(_valueFactory.Integer(funcCall.Arguments.Count)), new Metadata{Ast = funcCall});
            if (_env.ExportedFunctions.ContainsKey(funcCall.Name))
            {
                function.Write(_opFactory.ExportedCall(_valueFactory.String(funcCall.Name)), new Metadata{Ast = funcCall});
            }
            else
            {
                function.Write(_opFactory.Call(_valueFactory.String(funcCall.Name)), new Metadata{Ast = funcCall});
            }
        }

        public void Visit(AstStringConstant constant, Function function, Module mod)
        {
            Log($"Compiling string constant '{constant.String}'");
            function.Write(_opFactory.Push(_valueFactory.String(constant.String)), new Metadata{Ast = constant});
        }

        public void Visit(AstIntegerConstant constant, Function function, Module mod)
        {
            Log($"Compiling integer constant '{constant.Integer}'");
            function.Write(_opFactory.Push(_valueFactory.Integer(constant.Integer)), new Metadata{Ast = constant});
        }

        public void Visit(AstDoubleConstant constant, Function function, Module mod)
        {
            Log($"Compiling double constant '{constant.Double}'");
            function.Write(_opFactory.Push(_valueFactory.Double(constant.Double)), new Metadata{Ast = constant});
        }

        public void Visit(AstFunction func, Module mod)
        {
            Log($"Compiling function declaration '{func.Name}'");
            var newFunc = new Function(func.Name, mod.Name, func.Arguments);
            for (int i = func.Arguments.Count - 1; i > -1; i--)
            {
                newFunc.Write(_opFactory.Define(_valueFactory.String(func.Arguments[i])), new Metadata{Ast = func});
            }

            func.Expressions.Accept(this, newFunc, mod);
            if (newFunc.Length < 1)
            {
                newFunc.Write(_opFactory.Return(), new Metadata{Ast = func});
            }
            else if (newFunc[newFunc.Length - 1].Op != _opFactory.Return())
            {
                newFunc.Write(_opFactory.Return(), new Metadata{Ast = func});
            }

            if (_env.Functions.ContainsKey(newFunc.FullName))
                throw new CompilerException(
                    $"Function '{newFunc.Name}' has already been declared in namespace '{mod.Name}'.\nNear code: {func.ToCode()}");
            _env.Functions.Add(newFunc.FullName, newFunc);
        }


        public void Visit(AstMemberFunction memberFunc, Class clas, Module mod)
        {
            Log($"Compiling member function declaration '{memberFunc.Name}'");
            var func = new MemberFunction(memberFunc.Name, clas.FullName, memberFunc.Arguments, clas);

            func.Write(_opFactory.Define(_valueFactory.String(SpecialVariables.ArgumentCount)), new Metadata{Ast = memberFunc});
            for (int i = memberFunc.Arguments.Count - 1; i > -1; i--)
            {
                func.Write(_opFactory.Define(_valueFactory.String(memberFunc.Arguments[i])), new Metadata{Ast = memberFunc});
            }

            memberFunc.Expressions.Accept(this, func, mod);
            if (func.Length < 1)
            {
                func.Write(_opFactory.Return(), new Metadata{Ast = memberFunc});
            }
            else if (func[func.Length - 1].Op.GetType().Name != _opFactory.Return().GetType().Name)
            {
                func.Write(_opFactory.Return(), new Metadata{Ast = memberFunc});
            }

            clas.Functions.Add(func.Name, func);
        }
    }
}