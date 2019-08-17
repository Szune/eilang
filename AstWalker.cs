using System;
using System.Collections.Generic;

namespace eilang
{
    public class AstWalker
    {
        private AstRoot _ast;

        public AstWalker(AstRoot ast)
        {
            _ast = ast;
        }

        public void PrintAst()
        {
            if (_ast.Expressions.Count > 0)
            {
                Console.WriteLine("Global expressions");
                PrintExpressions(_ast.Expressions, 1);
            }
            if (_ast.Classes.Count > 0)
            {
                Console.WriteLine("Global classes");
                foreach (var clas in _ast.Classes)
                {
                    PrintClass(clas, 1);
                }
            }
            if (_ast.Functions.Count > 0)
            {
                Console.WriteLine("Global functions");
                foreach(var fun in _ast.Functions)
                {
                    PrintFunction(fun, 1);
                }
            }
            if(_ast.Modules.Count > 0)
            {
                Console.WriteLine("Modules");
                foreach(var mod in _ast.Modules)
                {
                    PrintModule(mod, 1);
                }
            }
        }

        private void PrintModule(AstModule mod, int indent)
        {
            var prefix = new string('.', indent * 2);
            Console.WriteLine($"{prefix}Module {mod.Name}");
            if(mod.Classes.Count > 0)
            {
                foreach(var clas in mod.Classes)
                    PrintClass(clas, indent + 1);
            }
            if(mod.Functions.Count > 0)
            {
                foreach(var fun in mod.Functions)
                    PrintFunction(fun, indent + 1);
            }
        }

        private void PrintClass(AstClass clas, int indent)
        {
            var prefix = new string('.', indent * 2);
            Console.WriteLine($"{prefix}Class {clas.Name}");
            foreach (var fun in clas.Functions)
            {
                PrintFunction(fun, indent + 1);
            }
        }

        private void PrintFunction(AstFunction fun, int indent)
        {
            var prefix = new string('.', indent * 2);
            Console.WriteLine($"{prefix}Function {fun.Name}");
            PrintExpressions(fun.Expressions, indent + 1);
        }

        private void PrintExpressions(List<AstExpression> exprs, int indent)
        {
            var prefix = new string('.', indent * 2);
            foreach (var expr in exprs)
            {
                switch (expr)
                {
                    case AstFunctionCall call:
                        if (call.Arguments.Count > 0)
                        {
                            Console.WriteLine($"{prefix}{expr.GetType().Name} {call.Name} w/ arguments:");
                            PrintExpressions(call.Arguments, indent + 1);
                        }
                        else
                        {
                            Console.WriteLine($"{prefix}{expr.GetType().Name} {call.Name}");
                        }
                        break;
                    case AstVariableReference refe:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {refe.Ident}");
                        break;
                    case AstStringConstant str:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} '{str.String}'");
                        break;
                    case AstIntegerConstant inte:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {inte.Integer}");
                        break;
                    case AstMemberFunctionCall amfa:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {string.Join(".", amfa.Identifiers)}");
                        PrintExpressions(amfa.Arguments, indent + 1);
                        break;
                    case AstMemberVariableAssignment amva:
                        break;
                    case AstMemberVariableReference astMemberVariableReference:
                        break;
                    case AstDoubleConstant doub:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {doub.Double}");
                        break;
                    case AstDeclarationAssignment ada:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {ada.Ident}");
                        PrintExpressions(new List<AstExpression>{ada.Value}, indent + 1);
                        break;
                    case AstAssignment aa:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {aa.Ident}");
                        PrintExpressions(new List<AstExpression>{aa.Value}, indent + 1);
                        break;
                    case AstClassInitialization aci:
                        Console.WriteLine($"{prefix}{expr.GetType().Name} {GetFullName(aci.Identifiers)}");
                        PrintExpressions(aci.Arguments, indent + 1);
                        break;
                    default:
                        Console.WriteLine($"{prefix}{expr.GetType().Name}");
                        break;

                }
            }
        }

        private string GetFullName(List<Reference> idents)
        {
            if(idents.Count == 1)
                return $"{Compiler.GlobalFunctionAndModuleName}::{idents[0].Ident}";
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