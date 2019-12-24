using System.Collections.Generic;
using System.Linq;
using eilang.Compiling;
using eilang.Interfaces;
using eilang.Tokens;

namespace eilang.Ast
{
    public class AstStructDeclaration : IVisitableInModule, IAst
    {
        public string Identifier { get; }
        public List<StructField> Fields { get; } = new List<StructField>();

        public AstStructDeclaration(string identifier, Position position)
        {
            Identifier = identifier;
            Position = position;
        }

        public void Accept(IVisitor visitor, Module mod)
        {
            visitor.Visit(this, mod);
        }

        public string ToCode()
        {
            var fields = string.Join(",\n", Fields.Select(f =>
                $"{f.Name}{TokenValues.Colon} {f.Type}{TokenValues.LeftParenthesis}{f.ByteCount}{TokenValues.RightParenthesis}"));
            return $"{TokenValues.Struct} {Identifier} {TokenValues.LeftBrace}\n{fields}\n{TokenValues.RightBrace}";
        }

        public Position Position { get; }

        public void AddField(StructField field)
        {
            Fields.Add(field);
        }
    }
    
}