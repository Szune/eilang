using eilang.Ast;
using eilang.Classes;

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
        void Visit(AstNewList list, Function function, Module mod);
        void Visit(AstIndexerReference indexer, Function function, Module mod);
        void Visit(AstIndexerAssignment assign, Function function, Module mod);
        void Visit(AstReturn ret, Function function, Module mod);
        void Visit(AstMemberIndexerReference indexer, Function function, Module mod);
        void Visit(AstMemberIndexerAssignment assign, Function function, Module mod);
        void Visit(AstRange range, Function function, Module mod);
        void Visit(AstForRange forRange, Function function, Module mod);
        void Visit(AstIt it, Function function, Module mod);
        void Visit(AstUnaryMathOperation unary, Function function, Module mod);
        void Visit(AstMemberReference memberFunc, Function function, Module mod);
        void Visit(AstMultiReference memberFunc, Function function, Module mod);
        void Visit(AstMemberCall memberFunc, Function function, Module mod);
        void Visit(AstMemberAssignment memberFunc, Function function, Module mod);
        void Visit(AstMemberIndexerRef memberFunc, Function function, Module mod);
    }
}