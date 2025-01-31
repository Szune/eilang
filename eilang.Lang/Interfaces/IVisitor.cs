using eilang.Ast;
using eilang.Classes;
using eilang.Compiling;

namespace eilang.Interfaces
{
    public interface IVisitor
    {
        void Visit(AstRoot root);
        void Visit(AstDeclarationAssignment assignment, Function function, Module mod);
        void Visit(AstDoubleConstant constant, Function function, Module mod);
        void Visit(AstFunctionCall funcCall, Function function, Module mod);
        void Visit(AstClass clas, Module mod);
        void Visit(AstFunction func, Module mod);
        void Visit(AstExtensionFunction func, Module mod);
        void Visit(AstMemberFunction memberFunc, Class clas, Module mod);
        void Visit(AstStringConstant constant, Function function, Module mod);
        void Visit(AstIntegerConstant constant, Function function, Module mod);
        void Visit(AstModule module);
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
        void Visit(AstReturn ret, Function function, Module mod);
        void Visit(AstRange range, Function function, Module mod);
        void Visit(AstForRange forRange, Function function, Module mod);
        void Visit(AstIt it, Function function, Module mod);
        void Visit(AstUnaryMathOperation unary, Function function, Module mod);
        void Visit(AstMemberReference memberReference, Function function, Module mod);
        void Visit(AstMultiReference memberFunc, Function function, Module mod);
        void Visit(AstMemberFunctionCall memberFuncCall, Function function, Module mod);
        void Visit(AstMemberIndexerRef memberFunc, Function function, Module mod);
        void Visit(AstForArray forArray, Function function, Module mod);
        void Visit(AstAssignmentValue assign, Function function, Module mod);
        void Visit(AstAssignment assignment, Function function, Module mod);
        void Visit(AstAssignmentReference memberFunc, Function function, Module mod);
        void Visit(AstIdentifier identifier, Function function, Module mod);
        void Visit(AstIx ix, Function function, Module mod);
        void Visit(AstBreak astBreak, Function function, Module mod);
        void Visit(AstContinue astContinue, Function function, Module mod);
        void Visit(AstForInfinite memberFunc, Function function, Module mod);
        void Visit(AstIndex memberFunc, Function function, Module mod);
        void Visit(AstMe me, Function function, Module mod);
        void Visit(AstTernary ternary, Function function, Module mod);
        void Visit(AstFunctionPointer funcPointer, Function function, Module mod);
        void Visit(AstParenthesized parenthesized, Function function, Module mod);
        void Visit(AstUninitialized uninit, Function function, Module mod);
        void Visit(AstUse astUse, Function function, Module mod);
        void Visit(AstWhile astWhile, Function function, Module mod);
        void Visit(AstNestedExpression nested, Function function, Module mod);
        void Visit(AstIndexerOnReturnedValue indexer, Function function, Module mod);
        void Visit(AstAnonymousTypeInitialization anon, Function function, Module mod);
        void Visit(AstNewMap newMap, Function function, Module mod);
        void Visit(AstTypeConstant typeConstant, Function function, Module mod);
        void Visit(AstLongConstant longConstant, Function function, Module mod);
        void Visit(AstStructDeclaration astStructDeclaration, Module mod);
        void Visit(AstStructInitialization astStructInit, Function function, Module mod);
        void Visit(AstLambda astLambda, Function function, Module mod);
        void Visit(AstParameterlessLambda astLambda, Function function, Module mod);
    }
}
