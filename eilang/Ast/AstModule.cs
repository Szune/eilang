﻿using System.Collections.Generic;
using eilang.Interfaces;

namespace eilang.Ast
{
    public class AstModule : IVisitable, IHaveClass, IHaveFunction
    {
        public string Name { get; }
        public List<AstClass> Classes { get; } = new List<AstClass>();
        public List<AstFunction> Functions { get; } = new List<AstFunction>();

        public AstModule(string name)
        {
            Name = name;
        }

        public void Accept(IVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}