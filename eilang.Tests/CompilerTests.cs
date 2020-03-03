using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Ast;
using eilang.Compiling;
using eilang.OperationCodes;
using eilang.Parsing;
using eilang.Tokens;
using eilang.Values;
using Xunit;

namespace eilang.Tests
{
    public class CompilerTests
    {
        public class ImplicitReturnOnFunction
        {
            [Fact]
            public void ShouldBeAppliedIfFunctionIsEmpty()
            {
                const string name = "test";
                var funcAst = new AstFunction(name, new List<Parameter>(), new Position(0, 0));
                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0)) {Functions = {funcAst}});
                Assert.Equal(typeof(Return), env.Functions[GetGlobalName(name)].Code.Last().Op.GetType());
            }

            [Fact]
            public void ShouldBeAppliedIfFunctionDoesNotEndWithReturn()
            {
                const string name = "test";
                var funcAst = new AstFunction(name, new List<Parameter>(), new Position(0, 0));
                funcAst.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstBinaryMathOperation(BinaryMath.Plus,
                            new AstIntegerConstant(5, null),
                            new AstIntegerConstant(5, null),
                            null)
                    }, null));
                funcAst.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstStringConstant("hello world", null)
                    }, null)
                );

                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0)) {Functions = {funcAst}});
                Assert.Equal(typeof(Return), env.Functions[GetGlobalName(name)].Code.Last().Op.GetType());
            }
        }

        public class ImplicitReturnOnLambda
        {
            [Fact]
            public void ShouldBeAppliedIfFunctionIsEmpty()
            {
                var lambdaAst = new AstParameterlessLambda(new AstBlock(null), null);
                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0)) {Expressions = {lambdaAst}});
                Assert.Equal(typeof(Return),
                    env.Functions[GetGlobalName(Compiler.GetAnonymousFunctionName(1))].Code.Last().Op.GetType());
            }

            [Fact]
            public void ShouldBeAppliedIfFunctionDoesNotEndWithReturn()
            {
                var block = new AstBlock(null);
                var lambdaAst = new AstParameterlessLambda(block, null);
                block.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstBinaryMathOperation(BinaryMath.Plus,
                            new AstIntegerConstant(5, null),
                            new AstIntegerConstant(5, null),
                            null)
                    }, null));
                block.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstStringConstant("hello world", null)
                    }, null)
                );

                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0)) {Expressions = { lambdaAst }});
                Assert.Equal(typeof(Return),
                    env.Functions[GetGlobalName(Compiler.GetAnonymousFunctionName(1))].Code.Last().Op.GetType());
            }
        }
        
        public class ImplicitReturnOnMemberFunction
        {
            [Fact]
            public void ShouldBeAppliedIfFunctionIsEmpty()
            {
                const string name = "test";
                var funcAst = new AstMemberFunction(name, new List<Parameter>(), new Position(0, 0));
                var klass = new AstClass(name, null);
                klass.Functions.Add(funcAst);
                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0))
                    {
                        Classes = {klass}
                    });
                Assert.Equal(typeof(Return), 
                    env.Classes[GetGlobalName(name)].Functions[name].Code.Last().Op.GetType());
            }

            [Fact]
            public void ShouldBeAppliedIfFunctionDoesNotEndWithReturn()
            {
                const string name = "test";
                var funcAst = new AstMemberFunction(name, new List<Parameter>(), new Position(0, 0));
                funcAst.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstBinaryMathOperation(BinaryMath.Plus,
                            new AstIntegerConstant(5, null),
                            new AstIntegerConstant(5, null),
                            null)
                    }, null));
                funcAst.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstStringConstant("hello world", null)
                    }, null)
                );
                var klass = new AstClass(name, null);
                klass.Functions.Add(funcAst);
                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0))
                    {
                        Classes = {klass}
                    });
                Assert.Equal(typeof(Return), 
                    env.Classes[GetGlobalName(name)].Functions[name].Code.Last().Op.GetType());
            }
        }
        
        public class ImplicitReturnOnExtensionFunction
        {
            [Fact]
            public void ShouldBeAppliedIfFunctionIsEmpty()
            {
                const string name = "test";
                var funcAst = new AstExtensionFunction(name, name, new List<Parameter>(), new Position(0, 0));
                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0))
                    {
                        Functions = { funcAst }
                    });
                Assert.Equal(typeof(Return), env.ExtensionFunctions[$"{GetGlobalName(name)}->{name}"].Code.Last().Op.GetType());
            }

            [Fact]
            public void ShouldBeAppliedIfFunctionDoesNotEndWithReturn()
            {
                const string name = "test";
                var funcAst = new AstExtensionFunction(name, name, new List<Parameter>(), new Position(0, 0));
                funcAst.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstBinaryMathOperation(BinaryMath.Plus,
                            new AstIntegerConstant(5, null),
                            new AstIntegerConstant(5, null),
                            null)
                    }, null));
                funcAst.Expressions.Add(
                    new AstFunctionCall("println", new List<AstExpression>
                    {
                        new AstStringConstant("hello world", null)
                    }, null)
                );
                var env = new ScriptEnvironment(new OperationCodeFactory(), new ValueFactory());
                Compiler.Compile(env,
                    new AstRoot(new Position(0, 0))
                    {
                        Functions = { funcAst }
                    });
                Assert.Equal(typeof(Return), env.ExtensionFunctions[$"{GetGlobalName(name)}->{name}"].Code.Last().Op.GetType());
            }
        }

        private static string GetGlobalName(string func) => $"{SpecialVariables.Global}::{func}";
    }
}