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
    }
}