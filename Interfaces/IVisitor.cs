namespace eilang
{
    public interface IVisitor
    {
        void Visit(AstRoot root);
        void Visit(AstDeclarationAssignment assignment, Function function);
        void Visit(AstAssignment assignment, Function function);
        void Visit(AstDoubleConstant constant, Function function);
        void Visit(AstFunctionCall funcCall, Function function);
        void Visit(AstVariableReference reference, Function function);
        void Visit(AstClass clas, Module mod);
        void Visit(AstFunction func, Module mod);
        void Visit(AstMemberFunction memberFunc, Class clas);
        void Visit(AstStringConstant constant, Function function);
        void Visit(AstIntegerConstant constant, Function function);
        void Visit(AstModule module);
    }
}