namespace eilang
{
    public interface IVisitor
    {
        void Visit(AstRoot root);
        void Visit(AstDeclarationAssignment assignment, Function function, Module mod);
        void Visit(AstAssignment assignment, Function function, Module mod);
        void Visit(AstDoubleConstant constant, Function function, Module mod);
        void Visit(AstFunctionCall funcCall, Function function, Module mod);
        void Visit(AstVariableReference reference, Function function, Module mod);
        void Visit(AstClass clas, Module mod);
        void Visit(AstFunction func, Module mod);
        void Visit(AstMemberFunction memberFunc, Class clas, Module mod);
        void Visit(AstStringConstant constant, Function function, Module mod);
        void Visit(AstIntegerConstant constant, Function function, Module mod);
        void Visit(AstModule module);
        void Visit(AstMemberVariableReference member, Function function, Module mod);
        void Visit(AstMemberVariableAssignment member, Function function, Module mod);
        void Visit(AstMemberFunctionCall memberFunc, Function function, Module mod);
        void Visit(AstClassInitialization init, Function function, Module mod);
        void Visit(AstMemberVariableDeclaration member, Class function, Module mod);
        void Visit(AstMemberVariableDeclarationWithInit member, Class function, Module mod);
        void Visit(AstConstructor ctor, Class function, Module mod);
        void Visit(AstBinaryMathOperation math, Function function, Module mod);
        void Visit(AstTrue tr, Function function, Module mod);
        void Visit(AstFalse fa, Function function, Module mod);
        void Visit(AstBlock block, Function function, Module mod);
        void Visit(AstIf aIf, Function function, Module mod);
        void Visit(AstCompare compare, Function function, Module mod);
    }
}