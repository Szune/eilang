using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using eilang.Ast;
using eilang.Classes;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.OperationCodes;
using eilang.Tokens;
using eilang.Values;

namespace eilang.Compiling
{
    public class Compiler : IVisitor
    {
        protected readonly IEnvironment ScriptEnvironment;
        protected readonly TextWriter Logger;
        protected readonly IValueFactory ValueFactory;
        protected readonly IOperationCodeFactory OpFactory;
        protected int ForDepth = 0;

        /// <summary>
        /// key = loop depth (<see cref="ForDepth"/>)
        /// </summary>
        protected readonly IDictionary<int, Stack<(int Index, int Type)>> LoopControlFlowOps =
            new Dictionary<int, Stack<(int Index, int Type)>>();

        protected const int Break = 0xBAD;
        protected const int Continue = 0xCAB;
        public const int InLoopReturn = 0xCAF0;

        public Compiler(IEnvironment scriptEnvironment, TextWriter logger, IValueFactory valueFactory,
            IOperationCodeFactory opFactory)
        {
            ScriptEnvironment = scriptEnvironment;
            Logger = logger;
            ValueFactory = valueFactory;
            OpFactory = opFactory;
        }

        private void Log(string msg)
        {
            Logger?.WriteLine(msg);
        }

        public static void Compile(IEnvironment scriptEnvironment, AstRoot root, IValueFactory valueFactory = null,
            IOperationCodeFactory opFactory = null, TextWriter logger = null)
        {
            var compiler = new Compiler(scriptEnvironment, logger, valueFactory ?? new ValueFactory(),
                opFactory ?? new OperationCodeFactory());
            compiler.Visit(root);
        }

        public void Visit(AstRoot root)
        {
            Log("Compiling...");
            Log("Compiling ast root");
            var globalMod = new Module(SpecialVariables.Global);
            var func = new Function(SpecialVariables.Global, SpecialVariables.Global, new List<string>());
            ScriptEnvironment.Functions.Add(func.FullName, func);
            root.Modules.Accept(this);
            root.Classes.Accept(this, globalMod);
            root.Structs.Accept(this, globalMod);
            root.Functions.Accept(this, globalMod);
            root.Expressions.Accept(this, func, globalMod);
            Log("Compilation finished.");
        }

        public void Visit(AstModule module)
        {
            Log($"Compiling module declaration '{module.Name}'");
            var mod = new Module(module.Name);
            module.Classes.Accept(this, mod);
            module.Structs.Accept(this, mod);
            module.Functions.Accept(this, mod);
            //_env.Modules.Add(mod.Name, mod);
        }

        public void Visit(AstClassInitialization init, Function function, Module mod)
        {
            var fullName = init.Class.Contains("::")
                ? init.Class
                : $"{SpecialVariables.Global}::{init.Class}";
            Log($"Compiling instance initialization '{fullName}'");
            init.Arguments.Accept(this, function, mod);
            function.Write(OpFactory.Push(ValueFactory.Integer(init.Arguments.Count)), new Metadata {Ast = init});
            function.Write(OpFactory.Initialize(ValueFactory.String(fullName)), new Metadata {Ast = init});
        }

        public void Visit(AstMemberVariableDeclaration member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration for '{member.Ident}'");
            clas.CtorForMembersWithValues.Write(OpFactory.Push(ValueFactory.Uninitialized()),
                new Metadata {Ast = member});
            clas.CtorForMembersWithValues.Write(OpFactory.Define(member.Ident), new Metadata {Ast = member});
        }

        public void Visit(AstMemberVariableDeclarationWithInit member, Class clas, Module mod)
        {
            Log($"Compiling member variable declaration with initial value for '{member.Ident}'");
            member.InitExpr.Accept(this, clas.CtorForMembersWithValues, mod);
            clas.CtorForMembersWithValues.Write(OpFactory.Define(member.Ident), new Metadata {Ast = member});
        }

        public void Visit(AstConstructor ctor, Class clas, Module mod)
        {
            Log($"Compiling ctor for '{clas.Name}'");
            var newCtor = new MemberFunction(ctor.Name, "", ctor.Arguments.Select(a => a.Name).ToList(), clas);
            newCtor.Write(OpFactory.Define(SpecialVariables.Me), new Metadata {Ast = ctor});
            foreach (var arg in ctor.Arguments)
            {
                newCtor.Write(OpFactory.DefineAndEnsureType(arg, newCtor), new Metadata {Ast = ctor});
            }

            if (ctor.Expressions.Any())
            {
                ctor.Expressions.Accept(this, newCtor, mod);
            }

            newCtor.Write(OpFactory.Return(), new Metadata {Ast = ctor});
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
                    function.Write(OpFactory.Add(), new Metadata {Ast = math});
                    break;
                case BinaryMath.Minus:
                    function.Write(OpFactory.Subtract(), new Metadata {Ast = math});
                    break;
                case BinaryMath.Times:
                    function.Write(OpFactory.Multiply(), new Metadata {Ast = math});
                    break;
                case BinaryMath.Division:
                    function.Write(OpFactory.Divide(), new Metadata {Ast = math});
                    break;
                case BinaryMath.Modulo:
                    function.Write(OpFactory.Modulo(), new Metadata {Ast = math});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstTrue tr, Function function, Module mod)
        {
            Log($"Compiling true");
            function.Write(OpFactory.Push(ValueFactory.True()), new Metadata {Ast = tr});
        }

        public void Visit(AstFalse fa, Function function, Module mod)
        {
            Log($"Compiling false");
            function.Write(OpFactory.Push(ValueFactory.False()), new Metadata {Ast = fa});
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
            function.Write(OpFactory.JumpIfFalse(ValueFactory.Integer(0)), new Metadata {Ast = aIf});
            var jmpfOpCodeIndex = function.Code.Count - 1;

            aIf.IfExpr.Accept(this, function, mod);
            function.Write(OpFactory.Jump(ValueFactory.Integer(0)), new Metadata {Ast = aIf});
            var jmpOpCodeIndex = function.Code.Count - 1;
            var ifEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(OpFactory.JumpIfFalse(ValueFactory.Integer(ifEndIndex)),
                new Metadata {Ast = aIf});

            if (aIf.ElseExpr != null)
            {
                aIf.ElseExpr.Accept(this, function, mod);
                var elseEndIndex = function.Code.Count;
                function[jmpOpCodeIndex] = new Bytecode(OpFactory.Jump(ValueFactory.Integer(elseEndIndex)),
                    new Metadata {Ast = aIf});
            }
            else
            {
                function[jmpOpCodeIndex] = new Bytecode(OpFactory.Jump(ValueFactory.Integer(ifEndIndex)),
                    new Metadata {Ast = aIf});
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
                    function.Write(OpFactory.Or(), new Metadata {Ast = compare});
                    break;
                case Compare.And:
                    function.Write(OpFactory.And(), new Metadata {Ast = compare});
                    break;
                case Compare.EqualsEquals:
                    function.Write(OpFactory.Equals(), new Metadata {Ast = compare});
                    break;
                case Compare.NotEquals:
                    function.Write(OpFactory.NotEquals(), new Metadata {Ast = compare});
                    break;
                case Compare.LessThan:
                    function.Write(OpFactory.LessThan(), new Metadata {Ast = compare});
                    break;
                case Compare.GreaterThan:
                    function.Write(OpFactory.GreaterThan(), new Metadata {Ast = compare});
                    break;
                case Compare.LessThanEquals:
                    function.Write(OpFactory.LessThanOrEquals(), new Metadata {Ast = compare});
                    break;
                case Compare.GreaterThanEquals:
                    function.Write(OpFactory.GreaterThanOrEquals(), new Metadata {Ast = compare});
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

            function.Write(OpFactory.Push(ValueFactory.Integer(list.InitialItems.Count)), new Metadata {Ast = list});
            function.Write(OpFactory.ListNew(), new Metadata {Ast = list});
        }

        public void Visit(AstNewMap newMap, Function function, Module mod)
        {
            for (int i = newMap.InitialItems.Count - 1; i > -1; i--)
            {
                newMap.InitialItems[i].Key.Accept(this, function, mod);
                newMap.InitialItems[i].Value.Accept(this, function, mod);
            }

            function.Write(OpFactory.Push(ValueFactory.Integer(newMap.InitialItems.Count)),
                new Metadata {Ast = newMap});
            function.Write(OpFactory.MapNew(), new Metadata {Ast = newMap});
        }

        public void Visit(AstTypeConstant typeConstant, Function function, Module mod)
        {
            function.Write(OpFactory.Push(ValueFactory.Type(typeConstant.Type)));
        }

        public void Visit(AstLongConstant longConstant, Function function, Module mod)
        {
            function.Write(OpFactory.Push(ValueFactory.Long(longConstant.Long)));
        }

        public void Visit(AstStructDeclaration astStructDeclaration, Module mod)
        {
            var strut = new Struct(astStructDeclaration.Identifier, mod.Name, astStructDeclaration.Fields);
            ScriptEnvironment.Structs.Add(strut.FullName, strut);
        }

        public void Visit(AstStructInitialization astStructInit, Function function, Module mod)
        {
            var fullName = astStructInit.StructName.Contains("::")
                ? astStructInit.StructName
                : $"{SpecialVariables.Global}::{astStructInit.StructName}";
            function.Write(OpFactory.InitializeStruct(fullName));
        }

        public void Visit(AstIndexerReference indexer, Function function, Module mod)
        {
            Log($"Compiling indexer reference for variable '{indexer.Identifier}'");
            if (indexer.IndexExprs.Count == 0)
                throw new CompilerException(
                    $"Indexer on variable '{indexer.Identifier}' contained no indexer expressions.");
            function.Write(OpFactory.Reference(ValueFactory.String(indexer.Identifier)), new Metadata {Ast = indexer});
            function.Write(OpFactory.TypeGet(), new Metadata {Ast = indexer});
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = indexer}); // arg count
            function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")),
                new Metadata {Variable = indexer.Identifier, IndexerDepth = 0, Ast = indexer});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(OpFactory.TypeGet(), new Metadata {Ast = indexer});
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = indexer}); // arg count
                function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")),
                    new Metadata {Variable = indexer.Identifier, IndexerDepth = i, Ast = indexer});
            }
        }


        public void Visit(AstReturn ret, Function function, Module mod)
        {
            if (ret.RetExpr != null)
                ret.RetExpr.Accept(this, function, mod);
            if (ForDepth > 0)
                AddControlFlowOp(ForDepth, (function.Length, InLoopReturn));
            function.Write(OpFactory.Return(), new Metadata {Ast = ret});
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
                        function.Write(OpFactory.TypeGet(), new Metadata {Ast = assignment});
                        assignment.Set.IndexExprs[i].Accept(this, function, mod);
                        function.Write(OpFactory.Push(ValueFactory.Integer(1)),
                            new Metadata {Ast = assignment}); // arg count
                        function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")),
                            new Metadata {Ast = assignment});
                    }

                    function.Write(OpFactory.TypeGet(), new Metadata {Ast = assignment});
                    assignment.Set.IndexExprs[^1].Accept(this, function, mod);
                    break;
            }

            switch (assignment.Value.Type)
            {
                case Assignment.Equals:
                    assignment.Value.Accept(this, function, mod);
                    switch (assignment.Set.Type)
                    {
                        case AssignmentSet.Array:
                            function.Write(OpFactory.Push(ValueFactory.Integer(2)), new Metadata {Ast = assignment});
                            function.Write(OpFactory.CallMember(ValueFactory.String("idx_set")),
                                new Metadata {Ast = assignment});
                            break;
                        case AssignmentSet.Variable:
                            if (assignment.Define)
                            {
                                function.Write(OpFactory.Define(assignment.Set.OptionalIdentifier),
                                    new Metadata {Ast = assignment});
                            }
                            else
                            {
                                function.Write(OpFactory.Set(ValueFactory.String(assignment.Set.OptionalIdentifier)),
                                    new Metadata {Ast = assignment});
                            }

                            break;
                        case AssignmentSet.MemberVariable:
                            function.Write(
                                OpFactory.MemberSet(ValueFactory.String(assignment.Set.OptionalIdentifier)),
                                new Metadata {Ast = assignment});
                            break;
                    }

                    return;
                case Assignment.DivideEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(OpFactory.Divide(), new Metadata {Ast = assignment});
                    break;
                case Assignment.TimesEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(OpFactory.Multiply(), new Metadata {Ast = assignment});
                    break;
                case Assignment.ModuloEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(OpFactory.Modulo(), new Metadata {Ast = assignment});
                    break;
                case Assignment.MinusEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(OpFactory.Subtract(), new Metadata {Ast = assignment});
                    break;
                case Assignment.PlusEquals:
                    assignment.Reference.Accept(this, function, mod);
                    assignment.Value.Accept(this, function, mod);
                    function.Write(OpFactory.Add(), new Metadata {Ast = assignment});
                    break;
                case Assignment.Increment:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(OpFactory.Increment(), new Metadata {Ast = assignment});
                    break;
                case Assignment.Decrement:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(OpFactory.Decrement(), new Metadata {Ast = assignment});
                    break;
                case Assignment.IncrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(OpFactory.Increment(), new Metadata {Ast = assignment});
                    break;
                case Assignment.DecrementAndReference:
                    assignment.Reference.Accept(this, function, mod);
                    function.Write(OpFactory.Decrement(), new Metadata {Ast = assignment});
                    break;
                default:
                    throw new CompilerException($"Unknown assignment type {assignment.Value.Type}");
            }

            switch (assignment.Set.Type)
            {
                case AssignmentSet.Array:
                    function.Write(OpFactory.Push(ValueFactory.Integer(2)), new Metadata {Ast = assignment});
                    function.Write(OpFactory.CallMember(ValueFactory.String("idx_set")),
                        new Metadata {Ast = assignment});
                    break;
                case AssignmentSet.Variable:
                    function.Write(OpFactory.Set(ValueFactory.String(assignment.Set.OptionalIdentifier)),
                        new Metadata {Ast = assignment});
                    break;
                case AssignmentSet.MemberVariable:
                    function.Write(OpFactory.MemberSet(ValueFactory.String(assignment.Set.OptionalIdentifier)),
                        new Metadata {Ast = assignment});
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
            function.Write(OpFactory.Reference(ValueFactory.String(identifier.Ident)), new Metadata {Ast = identifier});
        }

        public void Visit(AstIx ix, Function function, Module mod)
        {
            function.Write(OpFactory.Reference(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = ix});
        }

        public void Visit(AstBreak astBreak, Function function, Module mod)
        {
            AddControlFlowOp(ForDepth, (function.Length, Break));
            function.Write(OpFactory.Jump(ValueFactory.Integer(-1)), new Metadata {Ast = astBreak});
        }


        public void Visit(AstContinue astContinue, Function function, Module mod)
        {
            AddControlFlowOp(ForDepth, (function.Length, Continue));
            function.Write(OpFactory.Jump(ValueFactory.Integer(-1)), new Metadata {Ast = astContinue});
        }


        private void AddControlFlowOp(int forDepth, (int Index, int Type) values)
        {
            if (!LoopControlFlowOps.ContainsKey(forDepth))
            {
                LoopControlFlowOps[forDepth] = new Stack<(int Index, int Type)>();
            }

            LoopControlFlowOps[forDepth].Push(values);
        }

        public void Visit(AstRange range, Function function, Module mod)
        {
            throw new NotImplementedException("should never arrive here");
        }

        public void Visit(AstWhile astWhile, Function function, Module mod)
        {
            ForDepth++;
            function.Write(OpFactory.ScopeNew(), new Metadata {Ast = astWhile});
            var addressOfCmp = function.Code.Count;
            astWhile.Condition.Accept(this, function, mod);
            function.Write(OpFactory.JumpIfFalse(ValueFactory.Integer(0)), new Metadata {Ast = astWhile});
            var addressOfJmpF = function.Code.Count - 1;
            astWhile.Body.Accept(this, function, mod);
            function.Write(OpFactory.Jump(ValueFactory.Integer(addressOfCmp)), new Metadata {Ast = astWhile});
            var endOfLoop = function.Length;
            function.Write(OpFactory.ScopePop(), new Metadata {Ast = astWhile});
            function[addressOfJmpF] = new Bytecode(OpFactory.JumpIfFalse(ValueFactory.Integer(endOfLoop)),
                new Metadata {Ast = astWhile});
            AssignLoopControlFlowJumps(astWhile, function, ForDepth, addressOfCmp, endOfLoop);
            ForDepth--;
        }

        public void Visit(AstNestedExpression nested, Function function, Module mod)
        {
            nested.First.Accept(this, function, mod);
            nested.Second.Accept(this, function, mod);
        }

        public void Visit(AstIndexerOnReturnedValue indexer, Function function, Module mod)
        {
            for (int i = 0; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(OpFactory.TypeGet(), new Metadata {Ast = indexer});
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = indexer}); // arg count
                function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")),
                    new Metadata {IndexerDepth = i, Ast = indexer});
            }
        }

        public void Visit(AstAnonymousTypeInitialization anon, Function function, Module mod)
        {
            var className = $"{SpecialVariables.Global}::{anon.Name}";
            if (!ScriptEnvironment.Classes.ContainsKey(className))
            {
                var clas = new Class(anon.Name, SpecialVariables.Global);
                clas.CtorForMembersWithValues.Write(OpFactory.Define(SpecialVariables.Me));
                clas.CtorForMembersWithValues.Write(OpFactory.Return());
                var ctor = new MemberFunction($".ctor::{className}",
                    SpecialVariables.Global,
                    anon.Members.Select(m => m.Name).ToList(),
                    clas);
                ctor.Write(OpFactory.Define(SpecialVariables.Me));
                foreach (var member in anon.Members)
                {
                    ctor.Write(OpFactory.Define(member.Name));
                }

                ctor.Write(OpFactory.Return());
                clas.Constructors.Add(ctor);
                ScriptEnvironment.Classes[className] = clas;
            }

            foreach (var member in anon.Members)
            {
                member.Expr.Accept(this, function, mod);
            }

            function.Write(OpFactory.Push(ValueFactory.Integer(anon.Members.Count)), new Metadata {Ast = anon});
            function.Write(OpFactory.Initialize(ValueFactory.String(className)), new Metadata {Ast = anon});
        }


        public void Visit(AstForArray forArray, Function function, Module mod)
        {
            ForDepth++;
            function.Write(OpFactory.ScopeNew(), new Metadata {Ast = forArray});
            forArray.Array.Accept(this, function, mod);
            // save array to a temporary faster lookup location
            function.Write(OpFactory.TemporarySet(ValueFactory.String($".aval{ForDepth}")),
                new Metadata {Ast = forArray});
            function.Write(OpFactory.TemporaryReference(ValueFactory.String($".aval{ForDepth}")),
                new Metadata {Ast = forArray});
            function.Write(OpFactory.TypeGet(), new Metadata {Ast = forArray});
            function.Write(OpFactory.Push(ValueFactory.Integer(0)), new Metadata {Ast = forArray});
            function.Write(OpFactory.CallMember(ValueFactory.String("len")), new Metadata {Ast = forArray});
            function.Write(OpFactory.TemporarySet(ValueFactory.String($".alen{ForDepth}")),
                new Metadata {Ast = forArray});
            function.Write(OpFactory.TemporaryReference(ValueFactory.String($".alen{ForDepth}")),
                new Metadata {Ast = forArray});
            function.Write(OpFactory.JumpIfZero(ValueFactory.Integer(0)), new Metadata {Ast = forArray});
            var addressOfFirstJmpZ = function.Code.Count - 1;
            if (!forArray.Reversed)
            {
                // start loop with index set to 0
                function.Write(OpFactory.Push(ValueFactory.Integer(0)), new Metadata {Ast = forArray});
            }
            else
            {
                // or index set to last item if reversed
                function.Write(OpFactory.TemporaryReference(ValueFactory.String($".alen{ForDepth}")),
                    new Metadata {Ast = forArray});
                function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = forArray});
                function.Write(OpFactory.Subtract(), new Metadata {Ast = forArray});
            }

            function.Write(OpFactory.Define($".ix{ForDepth}"), new Metadata {Ast = forArray});
            // define 'it' variable
            function.Write(OpFactory.Push(ValueFactory.Uninitialized()), new Metadata {Ast = forArray});
            function.Write(OpFactory.Define($".it{ForDepth}"), new Metadata {Ast = forArray});
            function.Write(OpFactory.Reference(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forArray});
            var addressOfCmp = function.Code.Count - 1;
            // loop for the length of the array
            if (!forArray.Reversed)
            {
                function.Write(OpFactory.TemporaryReference(ValueFactory.String($".alen{ForDepth}")),
                    new Metadata {Ast = forArray});
                function.Write(OpFactory.GreaterThanOrEquals(), new Metadata {Ast = forArray});
            }
            else
            {
                function.Write(OpFactory.Push(ValueFactory.Integer(0)), new Metadata {Ast = forArray});
                function.Write(OpFactory.LessThan(), new Metadata {Ast = forArray});
            }

            function.Write(OpFactory.JumpIfTrue(ValueFactory.Integer(0)), new Metadata {Ast = forArray});
            var addressOfJmpT = function.Code.Count - 1;
            // set 'it' to the value of array at current index 
            function.Write(OpFactory.TemporaryReference(ValueFactory.String($".aval{ForDepth}")),
                new Metadata {Ast = forArray});
            function.Write(OpFactory.TypeGet(), new Metadata {Ast = forArray});
            function.Write(OpFactory.Reference(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forArray});
            function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = forArray});
            function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")), new Metadata {Ast = forArray});
            function.Write(OpFactory.Set(ValueFactory.String($".it{ForDepth}")), new Metadata {Ast = forArray});
            forArray.Body.Accept(this, function, mod);
            var addressOfLoopStep = function.Length;
            function.Write(OpFactory.Reference(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forArray});
            function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = forArray});
            // increment or decrement index variable
            if (forArray.Reversed)
                function.Write(OpFactory.Subtract(), new Metadata {Ast = forArray});
            else
                function.Write(OpFactory.Add(), new Metadata {Ast = forArray});
            function.Write(OpFactory.Set(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forArray});
            function.Write(OpFactory.Jump(ValueFactory.Integer(addressOfCmp)), new Metadata {Ast = forArray});
            var endOfLoop = function.Code.Count;
            function.Write(OpFactory.ScopePop(), new Metadata {Ast = forArray});
            function[addressOfJmpT] = new Bytecode(OpFactory.JumpIfTrue(ValueFactory.Integer(endOfLoop)),
                new Metadata {Ast = forArray});
            function[addressOfFirstJmpZ] = new Bytecode(OpFactory.JumpIfZero(ValueFactory.Integer(endOfLoop)),
                new Metadata {Ast = forArray});
            AssignLoopControlFlowJumps(forArray, function, ForDepth, addressOfLoopStep, endOfLoop);
            ForDepth--;
        }


        public void Visit(AstForRange forRange, Function function, Module mod)
        {
            ForDepth++;
            function.Write(OpFactory.ScopeNew(), new Metadata {Ast = forRange});
            forRange.Range.Begin.Accept(this, function, mod);
            forRange.Range.End.Accept(this, function, mod);
            if (forRange.Reversed)
                function.Write(OpFactory.LessThanOrEquals(), new Metadata {Ast = forRange});
            else
                function.Write(OpFactory.GreaterThanOrEquals(), new Metadata {Ast = forRange});
            function.Write(OpFactory.JumpIfTrue(ValueFactory.Integer(0)), new Metadata {Ast = forRange});
            var addressOfFirstJmpT = function.Code.Count - 1;
            forRange.Range.Begin.Accept(this, function, mod);
            function.Write(OpFactory.Define($".ix{ForDepth}"), new Metadata {Ast = forRange});
            function.Write(OpFactory.Reference(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forRange});
            var addressOfCmp = function.Code.Count - 1;
            forRange.Range.End.Accept(this, function, mod);
            if (forRange.Reversed)
                function.Write(OpFactory.LessThanOrEquals(), new Metadata {Ast = forRange});
            else
                function.Write(OpFactory.GreaterThanOrEquals(), new Metadata {Ast = forRange});
            function.Write(OpFactory.JumpIfTrue(ValueFactory.Integer(0)), new Metadata {Ast = forRange});
            var addressOfJmpT = function.Code.Count - 1;
            forRange.Body.Accept(this, function, mod);
            var addressOfLoopStep = function.Length;
            function.Write(OpFactory.Reference(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forRange});
            function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = forRange});
            if (forRange.Reversed)
                function.Write(OpFactory.Subtract(), new Metadata {Ast = forRange});
            else
                function.Write(OpFactory.Add(), new Metadata {Ast = forRange});
            function.Write(OpFactory.Set(ValueFactory.String($".ix{ForDepth}")), new Metadata {Ast = forRange});
            function.Write(OpFactory.Jump(ValueFactory.Integer(addressOfCmp)), new Metadata {Ast = forRange});
            var endOfLoop = function.Length;
            function.Write(OpFactory.ScopePop(), new Metadata {Ast = forRange});
            function[addressOfJmpT] = new Bytecode(OpFactory.JumpIfTrue(ValueFactory.Integer(endOfLoop)),
                new Metadata {Ast = forRange});
            function[addressOfFirstJmpT] = new Bytecode(OpFactory.JumpIfTrue(ValueFactory.Integer(endOfLoop)),
                new Metadata {Ast = forRange});
            AssignLoopControlFlowJumps(forRange, function, ForDepth, addressOfLoopStep, endOfLoop);
            ForDepth--;
        }

        public void Visit(AstForInfinite forInfinite, Function function, Module mod)
        {
            ForDepth++;
            function.Write(OpFactory.ScopeNew(), new Metadata {Ast = forInfinite});
            var addressOfLoopStart = function.Length;
            forInfinite.Body.Accept(this, function, mod);
            function.Write(OpFactory.Jump(ValueFactory.Integer(addressOfLoopStart)), new Metadata {Ast = forInfinite});
            var endOfLoop = function.Length;
            function.Write(OpFactory.ScopePop(), new Metadata {Ast = forInfinite});
            AssignLoopControlFlowJumps(forInfinite, function, ForDepth, addressOfLoopStart, endOfLoop);
            ForDepth--;
        }

        public void Visit(AstIndex index, Function function, Module mod)
        {
            index.Index.Accept(this, function, mod);
        }

        public void Visit(AstMe me, Function function, Module mod)
        {
            function.Write(OpFactory.Reference(ValueFactory.String(SpecialVariables.Me)), new Metadata {Ast = me});
        }

        public void Visit(AstTernary ternary, Function function, Module mod)
        {
            Log($"Compiling ternary operator");
            ternary.Condition.Accept(this, function, mod);
            function.Write(OpFactory.JumpIfFalse(ValueFactory.Integer(0)), new Metadata {Ast = ternary});
            var jmpfOpCodeIndex = function.Code.Count - 1;

            ternary.TrueExpr.Accept(this, function, mod);
            function.Write(OpFactory.Jump(ValueFactory.Integer(0)), new Metadata {Ast = ternary});
            var jmpOpCodeIndex = function.Code.Count - 1;
            var trueEndIndex = function.Code.Count;
            function[jmpfOpCodeIndex] = new Bytecode(OpFactory.JumpIfFalse(ValueFactory.Integer(trueEndIndex)),
                new Metadata {Ast = ternary});

            ternary.FalseExpr.Accept(this, function, mod);
            var falseEndIndex = function.Code.Count;
            function[jmpOpCodeIndex] = new Bytecode(OpFactory.Jump(ValueFactory.Integer(falseEndIndex)),
                new Metadata {Ast = ternary});
        }

        public void Visit(AstFunctionPointer funcPointer, Function function, Module mod)
        {
            function.Write(OpFactory.Push(ValueFactory.FunctionPointer(funcPointer.Ident)),
                new Metadata {Ast = funcPointer});
        }

        public void Visit(AstParenthesized parenthesized, Function function, Module mod)
        {
            parenthesized.Expr.Accept(this, function, mod);
        }

        public void Visit(AstUninitialized uninit, Function function, Module mod)
        {
            function.Write(OpFactory.Push(ValueFactory.Uninitialized()), new Metadata {Ast = uninit});
        }

        public void Visit(AstUse astUse, Function function, Module mod)
        {
            astUse.Expression.Accept(this, function, mod);
            function.Write(OpFactory.Define(astUse.Identifier));
            astUse.Body.Accept(this, function, mod);
            function.Write(OpFactory.Reference(ValueFactory.String(astUse.Identifier)));
            function.Write(OpFactory.Dispose());
        }


        private void AssignLoopControlFlowJumps(IAst ast, Function function, int forDepth, int loopStep, int loopEnd)
        {
            if (!LoopControlFlowOps.TryGetValue(forDepth, out var stack) || stack.Count <= 0)
                return;
            while (stack.Any())
            {
                var flow = stack.Pop();
                switch (flow.Type)
                {
                    case Break:
                        function.Code[flow.Index] = new Bytecode(OpFactory.Jump(ValueFactory.Integer(loopEnd)),
                            new Metadata {Ast = ast});
                        break;
                    case Continue:
                        function.Code[flow.Index] = new Bytecode(OpFactory.Jump(ValueFactory.Integer(loopStep)),
                            new Metadata {Ast = ast});
                        break;
                    case InLoopReturn:
                        function.Code[flow.Index] = new Bytecode(OpFactory.Return(forDepth), new Metadata {Ast = ast});
                        break;
                    default:
                        throw new CompilerException("Unknown control flow type of value " + flow.Type);
                }
            }
        }

        public void Visit(AstIt it, Function function, Module mod)
        {
            function.Write(OpFactory.Reference(ValueFactory.String($".it{ForDepth}")), new Metadata {Ast = it});
        }

        public void Visit(AstUnaryMathOperation unary, Function function, Module mod)
        {
            unary.Expr.Accept(this, function, mod);
            switch (unary.Op)
            {
                case UnaryMath.Minus:
                    function.Write(OpFactory.Negate(), new Metadata {Ast = unary});
                    break;
                case UnaryMath.Not:
                    function.Write(OpFactory.Not(), new Metadata {Ast = unary});
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Visit(AstMemberReference memberReference, Function function, Module mod)
        {
            function.Write(OpFactory.MemberReference(ValueFactory.String(memberReference.Ident)),
                new Metadata {Ast = memberReference});
        }

        public void Visit(AstMultiReference memberFunc, Function function, Module mod)
        {
            memberFunc.First.Accept(this, function, mod);
            memberFunc.Second.Accept(this, function, mod);
        }

        public void Visit(AstMemberFunctionCall memberFuncCall, Function function, Module mod)
        {
            function.Write(OpFactory.TypeGet(), new Metadata {Ast = memberFuncCall});
            memberFuncCall.Arguments.Accept(this, function, mod);
            function.Write(OpFactory.Push(ValueFactory.Integer(memberFuncCall.Arguments.Count)),
                new Metadata {Ast = memberFuncCall});
            function.Write(OpFactory.CallMember(ValueFactory.String(memberFuncCall.Ident)),
                new Metadata {Ast = memberFuncCall});
        }

        public void Visit(AstMemberIndexerRef indexer, Function function, Module mod)
        {
            function.Write(OpFactory.MemberReference(ValueFactory.String(indexer.Ident)), new Metadata {Ast = indexer});
            function.Write(OpFactory.TypeGet(), new Metadata {Ast = indexer});
            indexer.IndexExprs[0].Accept(this, function, mod);
            function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = indexer}); // arg count
            function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")),
                new Metadata {Variable = indexer.Ident, IndexerDepth = 0, Ast = indexer});
            for (int i = 1; i < indexer.IndexExprs.Count; i++)
            {
                function.Write(OpFactory.TypeGet(), new Metadata {Ast = indexer});
                indexer.IndexExprs[i].Accept(this, function, mod);
                function.Write(OpFactory.Push(ValueFactory.Integer(1)), new Metadata {Ast = indexer}); // arg count
                function.Write(OpFactory.CallMember(ValueFactory.String("idx_get")),
                    new Metadata {Variable = indexer.Ident, IndexerDepth = i, Ast = indexer});
            }
        }


        public void Visit(AstClass clas, Module mod)
        {
            Log($"Compiling class declaration '{clas.Name}'");
            var newClass = new Class(clas.Name, mod.Name);
            newClass.CtorForMembersWithValues.Write(OpFactory.Define(SpecialVariables.Me), new Metadata {Ast = clas});
            clas.Variables.Accept(this, newClass, mod);
            newClass.CtorForMembersWithValues.Write(OpFactory.Return(), new Metadata {Ast = clas});
            clas.Functions.Accept(this, newClass, mod);
            clas.Constructors.Accept(this, newClass, mod);
            InsertMemberValueInitializationInConstructors(newClass, clas);
            ScriptEnvironment.Classes.Add(newClass.FullName, newClass);
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
                var notYetInitialized = GetMembersNotInConstructor(ctor.Arguments, ast.Variables);
                foreach (var uninitialized in notYetInitialized)
                {
                    var initializationStartIndex =
                        GetIndexOfInitializationStart(uninitialized, clas.CtorForMembersWithValues);
                    if (initializationStartIndex == -1)
                        throw new CompilerException(
                            $"Could not find start of initialization of '{uninitialized}' in main initialization constructor of class {clas.FullName}");
                    for (int i = initializationStartIndex; i < clas.CtorForMembersWithValues.Length; i++)
                    {
                        ctor.Code.Insert(i - initializationStartIndex, clas.CtorForMembersWithValues[i]);
                        if (clas.CtorForMembersWithValues[i].Op is Define)
                            break;
                    }
                }
            }
        }

        private int GetIndexOfInitializationStart(string uninitialized, MemberFunction ctorForMembersWithValues)
        {
            var start = 0;
            for (int i = 0; i < ctorForMembersWithValues.Length; i++)
            {
                if (ctorForMembersWithValues[i].Op is Define def)
                {
                    if (def.Name == uninitialized)
                    {
                        return start;
                    }
                    else
                    {
                        start = i + 1;
                        if (start > ctorForMembersWithValues.Length - 1)
                        {
                            throw new CompilerException(
                                $"Could not find start of initialization of '{uninitialized}' in main initialization constructor of class {ctorForMembersWithValues.Owner.FullName}");
                        }
                    }
                }
            }

            return -1;
        }

        private List<string> GetMembersNotInConstructor(List<string> constructorArguments,
            List<AstMemberVariableDeclaration> memberVariables)
        {
            return memberVariables.Where(v => !constructorArguments.Contains(v.Ident)).Select(s => s.Ident).ToList();
        }

        private bool ConstructorInitializesAllMembers(List<string> constructorArguments,
            List<AstMemberVariableDeclaration> memberVariables)
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
            function.Write(OpFactory.Define(assignment.Ident), new Metadata {Ast = assignment});
        }

        public void Visit(AstFunctionCall funcCall, Function function, Module mod)
        {
            Log($"Compiling function call '{funcCall.Name}'");
            funcCall.Arguments.Accept(this, function, mod);
            function.Write(OpFactory.Push(ValueFactory.Integer(funcCall.Arguments.Count)),
                new Metadata {Ast = funcCall});
            if (ScriptEnvironment.ExportedFunctions.ContainsKey(funcCall.Name))
            {
                function.Write(OpFactory.CallExported(ValueFactory.String(funcCall.Name)),
                    new Metadata {Ast = funcCall});
            }
            else
            {
                function.Write(OpFactory.Call(ValueFactory.String(funcCall.Name)), new Metadata {Ast = funcCall});
            }
        }

        public void Visit(AstStringConstant constant, Function function, Module mod)
        {
            Log($"Compiling string constant '{constant.String}'");
            function.Write(OpFactory.Push(ValueFactory.String(constant.String)), new Metadata {Ast = constant});
        }

        public void Visit(AstIntegerConstant constant, Function function, Module mod)
        {
            Log($"Compiling integer constant '{constant.Integer}'");
            function.Write(OpFactory.Push(ValueFactory.Integer(constant.Integer)), new Metadata {Ast = constant});
        }

        public void Visit(AstDoubleConstant constant, Function function, Module mod)
        {
            Log($"Compiling double constant '{constant.Double}'");
            function.Write(OpFactory.Push(ValueFactory.Double(constant.Double)), new Metadata {Ast = constant});
        }

        public void Visit(AstFunction func, Module mod)
        {
            Log($"Compiling function declaration '{func.Name}'");
            var newFunc = new Function(func.Name, mod.Name, func.Arguments.Select(a => a.Name).ToList());
            for (int i = func.Arguments.Count - 1; i > -1; i--)
            {
                newFunc.Write(OpFactory.DefineAndEnsureType(func.Arguments[i], newFunc), new Metadata {Ast = func});
            }

            func.Expressions.Accept(this, newFunc, mod);
            if (newFunc.Length < 1)
            {
                newFunc.Write(OpFactory.Return(), new Metadata {Ast = func});
            }
            else if (newFunc[^1].Op != OpFactory.Return())
            {
                newFunc.Write(OpFactory.Return(), new Metadata {Ast = func});
            }

//            if (ScriptEnvironment.Functions.ContainsKey(newFunc.FullName))
//                throw new CompilerException(
//                    $"Function '{newFunc.Name}' has already been declared in namespace '{mod.Name}'.\nNear code: {func.ToCode()}");
            ScriptEnvironment.Functions.Add(newFunc.FullName, newFunc);
        }

        public void Visit(AstExtensionFunction memberFunc, Module mod)
        {
            var extendingModule = mod.Name;
            var extendingFullName = "";
            if (memberFunc.Extending.Contains(TokenValues.DoubleColon))
            {
                extendingModule = memberFunc.Extending.Substring(0,
                    memberFunc.Extending.LastIndexOf("::", StringComparison.InvariantCulture));
                extendingFullName = memberFunc.Extending;
            }
            else
            {
                extendingFullName = $"{extendingModule}::{memberFunc.Extending}";
            }

            var func = new ExtensionFunction(memberFunc.Name,
                extendingModule,
                memberFunc.Arguments.Select(a => a.Name).ToList(),
                memberFunc.Extending);

            func.Write(OpFactory.Define(SpecialVariables.ArgumentCount), new Metadata {Ast = memberFunc});
            for (int i = memberFunc.Arguments.Count - 1; i > -1; i--)
            {
                func.Write(OpFactory.DefineAndEnsureType(memberFunc.Arguments[i], func),
                    new Metadata {Ast = memberFunc});
            }

            memberFunc.Expressions.Accept(this, func, mod);
            if (func.Length < 1)
            {
                func.Write(OpFactory.Return(), new Metadata {Ast = memberFunc});
            }
            else if (func[^1].Op.GetType().Name != OpFactory.Return().GetType().Name)
            {
                func.Write(OpFactory.Return(), new Metadata {Ast = memberFunc});
            }

            ScriptEnvironment.ExtensionFunctions.Add($"{extendingFullName}->{memberFunc.Name}", func);
        }


        public void Visit(AstMemberFunction memberFunc, Class clas, Module mod)
        {
            Log($"Compiling member function declaration '{memberFunc.Name}'");
            var func = new MemberFunction(memberFunc.Name, clas.FullName,
                memberFunc.Arguments.Select(a => a.Name).ToList(), clas);

            func.Write(OpFactory.Define(SpecialVariables.ArgumentCount), new Metadata {Ast = memberFunc});
            for (int i = memberFunc.Arguments.Count - 1; i > -1; i--)
            {
                func.Write(OpFactory.DefineAndEnsureType(memberFunc.Arguments[i], func),
                    new Metadata {Ast = memberFunc});
            }

            memberFunc.Expressions.Accept(this, func, mod);
            if (func.Length < 1)
            {
                func.Write(OpFactory.Return(), new Metadata {Ast = memberFunc});
            }
            else if (func[^1].Op.GetType().Name != OpFactory.Return().GetType().Name)
            {
                func.Write(OpFactory.Return(), new Metadata {Ast = memberFunc});
            }

            clas.Functions.Add(func.Name, func);
        }
    }
}