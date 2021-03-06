using System.Collections.Generic;
using System.Linq;
using eilang.Interfaces;
using eilang.Parsing;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstRoot : IHaveClass, IHaveFunction, IHaveStruct, IHaveExpression, IAst
    {
        public Position Position { get; }

        public AstRoot(Position position)
        {
            Position = position;
        }
        public List<AstModule> Modules {get; } = new List<AstModule>();

        public List<AstFunction> Functions { get;} = new List<AstFunction>();

        public List<AstClass> Classes {get;} = new List<AstClass>();

        public List<AstExpression> Expressions {get;} = new List<AstExpression>();
        public List<AstStructDeclaration> Structs { get; } = new List<AstStructDeclaration>();

        public string ToCode()
        {
            var modules = string.Join("\n", Modules.Select(m => m.ToCode()));
            modules = string.IsNullOrWhiteSpace(modules) ? "" : modules + "\n";
            var classes = string.Join("\n", Classes.Select(m => m.ToCode()));
            classes = string.IsNullOrWhiteSpace(classes) ? "" : classes + "\n";
            var structs = string.Join("\n", Structs.Select(m => m.ToCode()));
            structs = string.IsNullOrWhiteSpace(structs) ? "" : structs + "\n";
            var funcs = string.Join("\n", Functions.Select(m => m.ToCode()));
            funcs = string.IsNullOrWhiteSpace(funcs) ? "" : funcs + "\n";
            var exprs = string.Join("\n", Expressions.Select(m => m.ToCode()));
            return $"{modules}{classes}{structs}{funcs}{exprs}";
        }
    }
}