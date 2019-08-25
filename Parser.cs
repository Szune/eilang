using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using eilang.Ast;

namespace eilang
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private readonly Token[] _buffer = {new Token(), new Token()};
        private bool _inMemberAssignment;

        public Parser(Lexer lexer)
        {
            _lexer = lexer;
            Consume();
            Consume();
        }

        public AstRoot Parse()
        {
            var root = new AstRoot();
            while (!Match(TokenType.EOF))
            {
                switch (_buffer[0].Type)
                {
                    case TokenType.Module:
                        ParseModule(root);
                        break;
                    case TokenType.Class:
                        ParseClass(root);
                        break;
                    case TokenType.Function:
                        ParseFunction(root);
                        break;
                    default:
                        ParseLeftExpression(root);
                        break;
                }
            }

            return root;
        }

        private void ParseModule(AstRoot root)
        {
            Require(TokenType.Module);
            var ident = Require(TokenType.Identifier).Text;
            var module = new AstModule(ident);
            root.Modules.Add(module);
            Require(TokenType.LeftBrace);
            while (!Match(TokenType.RightBrace) && !Match(TokenType.EOF))
            {
                switch (_buffer[0].Type)
                {
                    case TokenType.Class:
                        ParseClass(module);
                        break;
                    case TokenType.Function:
                        ParseFunction(module);
                        break;
                }
            }

            Require(TokenType.RightBrace);
        }

        private void ParseClass(IHaveClass ast)
        {
            Require(TokenType.Class);
            var ident = Require(TokenType.Identifier).Text;
            var clas = new AstClass(ident);
            Require(TokenType.LeftBrace);
            while (!Match(TokenType.RightBrace) && !Match(TokenType.EOF))
            {
                switch (_buffer[0].Type)
                {
                    case TokenType.Function:
                        ParseMemberFunction(clas);
                        break;
                    case TokenType.Constructor:
                        ParseConstructor(clas);
                        break;

                    case TokenType.Identifier:
                        ParseMemberVariableList(clas);
                        break;
                    default:
                        throw new ParserException($"Unknown token {_buffer[0].Type} in class {ident}'s scope");
                }
            }

            Require(TokenType.RightBrace);
            ast.Classes.Add(clas);
        }

        private void ParseConstructor(AstClass clas)
        {
            Require(TokenType.Constructor);
            Require(TokenType.LeftParenthesis);
            var args = new List<string>();
            while (!Match(TokenType.RightParenthesis) && !Match(TokenType.EOF))
            {
                var ident = Require(TokenType.Identifier).Text;
                args.Add(ident);
                if (Match(TokenType.Comma))
                    Consume();
            }

            Require(TokenType.RightParenthesis);
            Require(TokenType.Semicolon);
            clas.Constructors.Add(new AstConstructor($".ctor::{clas.Name}", args));
        }

        private void ParseMemberVariable(AstClass clas)
        {
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.Colon);
            var type = GetModuledClassName();
            if (Match(TokenType.Semicolon)) // no initialization
            {
                Consume();
                clas.Variables.Add(new AstMemberVariableDeclaration(ident, type));
                return;
            }

            Require(TokenType.Equals);

            var initExpr = ParseOr();
            Require(TokenType.Semicolon);
            clas.Variables.Add(new AstMemberVariableDeclarationWithInit(ident, type, initExpr));
        }

        private void ParseMemberVariableList(AstClass clas)
        {
            if (!_buffer[1].Match(TokenType.Comma))
            {
                ParseMemberVariable(clas);
                return;
            }

            var idents = new List<string>();
            do
            {
                var ident = Require(TokenType.Identifier).Text;
                idents.Add(ident);
                if (Match(TokenType.Comma))
                    Consume();
            } while (Match(TokenType.Identifier));

            Require(TokenType.Colon);
            var type = GetModuledClassName();

            if (Match(TokenType.Semicolon)) // no initialization
            {
                Consume();
                foreach (var ide in idents)
                    clas.Variables.Add(new AstMemberVariableDeclaration(ide, type));
                return;
            }

            Require(TokenType.Equals);

            var initExpr = ParseOr();
            Require(TokenType.Semicolon);
            foreach (var ide in idents)
                clas.Variables.Add(new AstMemberVariableDeclarationWithInit(ide, type, initExpr));
        }

        private void ParseMemberFunction(AstClass clas)
        {
            Require(TokenType.Function);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.LeftParenthesis);
            var args = new List<string>();
            while (!Match(TokenType.RightParenthesis) && !Match(TokenType.EOF))
            {
                var arg = Require(TokenType.Identifier).Text;
                args.Add(arg);
                if (Match(TokenType.Comma))
                {
                    Consume();
                }
            }

            Require(TokenType.RightParenthesis);
            var fun = new AstMemberFunction(ident, args);
            // parse code block
            ParseBlock(fun);
            clas.Functions.Add(fun);
        }

        private void ParseFunction(IHaveFunction ast)
        {
            Require(TokenType.Function);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.LeftParenthesis);
            var args = new List<string>();
            while (!Match(TokenType.RightParenthesis) && !Match(TokenType.EOF))
            {
                var arg = Require(TokenType.Identifier).Text;
                args.Add(arg);
                if (Match(TokenType.Comma))
                {
                    Consume();
                }
            }

            Require(TokenType.RightParenthesis);
            var fun = new AstFunction(ident, args);
            // parse code block
            ParseBlock(fun);
            ast.Functions.Add(fun);
        }

        private void ParseBlock(IHaveExpression fun)
        {
            Require(TokenType.LeftBrace);
            while (!Match(TokenType.RightBrace) && !Match(TokenType.EOF))
            {
                ParseLeftExpression(fun);
            }

            // parse expressions
            Require(TokenType.RightBrace);
        }

        private void ParseLeftExpression(IHaveExpression ast)
        {
            switch (_buffer[0].Type)
            {
                case TokenType.If:
                    ParseIf(ast);
                    return;
                case TokenType.For:
                    ParseFor(ast);
                    return;
                case TokenType.Return:
                    ParseReturn(ast);
                    return;
                case TokenType.Var:
                    if (_inMemberAssignment)
                    {
                        throw new ParserException(
                            $"Unexpected variable declaration containing another member variable assignment at line {_buffer[0].Line}, col {_buffer[0].Col}");
                    }

                    ParseDeclarationAssignment(ast);
                    return;
                case TokenType.Identifier:
                    switch (_buffer[1].Type)
                    {
                        case TokenType.LeftBracket:
                            ParseIndexerAssignmentOrRef(ast);
                            return;
                        case TokenType.Equals:
                            if (_inMemberAssignment)
                            {
                                throw new ParserException(
                                    $"Unexpected variable assignment containing another member variable assignment at line {_buffer[0].Line}, col {_buffer[0].Col}");
                            }

                            ParseAssignment(ast);
                            return;
                        default:
                            ast.Expressions.Add(ParseOr());
                            Require(TokenType.Semicolon);
                            return;
                    }
                case TokenType.Semicolon:
                    Consume();
                    return;
                default:
                    ast.Expressions.Add(ParseOr());
                    Require(TokenType.Semicolon);
                    return;
            }

            throw new NotImplementedException();
        }

        private void ParseReturn(IHaveExpression ast)
        {
            Require(TokenType.Return);
            if (Match(TokenType.Semicolon))
            {
                ast.Expressions.Add(new AstReturn(null));
                return;
            }
            var retExpr = ParseOr();
            ast.Expressions.Add(new AstReturn(retExpr));
        }

        private void ParseIndexerAssignmentOrRef(IHaveExpression ast)
        {
            var ident = Require(TokenType.Identifier).Text;
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(expr);
            }

//            if (Match(TokenType.Dot))
//            {
//                // call function or reference variable on the returned type
//                ast.Expressions.Add(new AstIndexerReference(ident, idxExprs));
//                // parse dot-identifier-list
//                // if ends with left parenthesis, parse func call
//                ast.Expressions.Add(new AstMemberFunctionCallOnStackValue());
//                // if ends with anything else, parse member reference
//                ast.Expressions.Add(new AstMemberReferenceOnStackValue());
//                // TODO: refactor some parsing to other parsers
//                return;
//            }
            if (!Match(TokenType.Equals))
            {
                ast.Expressions.Add(new AstIndexerReference(ident, idxExprs));
                return;
            }
            Consume();
            var assignExpr = ParseOr();
            var idxAssignExpr = new AstIndexerAssignment(ident, idxExprs, assignExpr);
            ast.Expressions.Add(idxAssignExpr);
        }

        private AstExpression ParseIndexerExpression()
        {
            var ident = Require(TokenType.Identifier).Text;
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(expr);
            }
            return new AstIndexerReference(ident, idxExprs);
        }
        
        private AstExpression ParseMemberIndexerExpression(List<string> identifiers)
        {
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(expr);
            }
            if (!Match(TokenType.Equals))
            {
                return new AstMemberIndexerReference(identifiers, idxExprs);
            }
            
            Consume();
            var assignExpr = ParseOr();
            var idxAssignExpr = new AstMemberIndexerAssignment(identifiers, idxExprs, assignExpr);
            return idxAssignExpr;
        }

        private void ParseFor(IHaveExpression ast)
        {
            Require(TokenType.For);
            Require(TokenType.LeftParenthesis);
            if (Match(TokenType.Identifier))
            {
                if (_buffer[1].Match(TokenType.Colon))
                {
                    // for(idx : <loopExpr>)
                    return;
                }
                else if (_buffer[1].Match(TokenType.Comma))
                {
                    // for(val, idx : <loopExpr>)
                    return;
                }
                else if (_buffer[1].Match(TokenType.RightParenthesis))
                {
                    // for(array)
                    return;
                }
                else if (_buffer[1].Match(TokenType.Dot))
                {
                    // for(array.of.some.type) or for(array.of.some.type..something)
                }
            }
            
            var range = ParseRanges();
            Require(TokenType.RightParenthesis);
            var forBlock = new AstBlock();
            ParseBlock(forBlock);
            ast.Expressions.Add(new AstForRange(range, forBlock));
        }

        private void ParseIf(IHaveExpression ast)
        {
            Require(TokenType.If);
            Require(TokenType.LeftParenthesis);
            var condition = ParseOr();
            Require(TokenType.RightParenthesis);
            var ifPart = new AstBlock();
            ParseBlock(ifPart);
            
            if (!Match(TokenType.Else))
            {
                ast.Expressions.Add(new AstIf(condition, ifPart, null));
                return;
            }

            var firstIf = new AstIf(condition, ifPart, null);
            var currentIf = firstIf;
            while (Match(TokenType.Else))
            {
                Consume();
                if (Match(TokenType.If))
                {
                    Consume();
                    var cond = ParseOr();
                    var ifBlock = new AstBlock();
                    ParseBlock(ifBlock);
                    var newIf = new AstIf(cond, ifBlock, null);
                    currentIf.SetElse(newIf);
                    currentIf = newIf;
                }
                else
                {
                    var elseBlock = new AstBlock();
                    ParseBlock(elseBlock);
                    currentIf.SetElse(elseBlock);
                    ast.Expressions.Add(firstIf);
                    return;
                }
            }
            ast.Expressions.Add(firstIf); // this part is reached if there were else ifs without an ending else
        }

        private AstExpression ParseMemberReferenceOrMemberFunctionCallOrMemberVariableAssignmentOrIndexer()
        {
            var firstIdentifier = Require(TokenType.Identifier).Text;
            var identifiers = new List<string> {firstIdentifier};

            while (Match(TokenType.Dot) && !Match(TokenType.EOF))
            {
                Require(TokenType.Dot);
                var nextIdentifier = Require(TokenType.Identifier).Text;
                identifiers.Add(nextIdentifier);
                if (Match(TokenType.Semicolon))
                {
                    Require(TokenType.Semicolon);
                    return new AstMemberVariableReference(identifiers);
                }
                else if (Match(TokenType.LeftParenthesis))
                {
                    return ParseMemberFunctionCall(identifiers);
                }
                else if (Match(TokenType.LeftBracket))
                {
                    return ParseMemberIndexerExpression(identifiers);
                }
                else if (Match(TokenType.Equals))
                {
                    if (_inMemberAssignment)
                    {
                        throw new ParserException(
                            $"Unexpected member variable assignment containing another member variable assignment at line {_buffer[0].Line}, col {_buffer[0].Col}");
                    }

                    _inMemberAssignment = true;
                    Require(TokenType.Equals);
                    var assignVal = ParseOr();
                    var assignment = new AstMemberVariableAssignment(identifiers, assignVal);
                    _inMemberAssignment = false;
                    return assignment;
                }
            }

            return new AstMemberVariableReference(identifiers);
            //throw new ParserException($"Unexpected token {_buffer[0].Type} at line {_buffer[0].Line}, col {_buffer[0].Col}");
        }



        private AstExpression ParseClassInitialization()
        {
            var identifiers = GetIdentifierList();
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstClassInitialization(identifiers, args);
        }

        private AstFunctionCall ParseFunctionCall()
        {
            var function = Require(TokenType.Identifier).Text;
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstFunctionCall(function, args);
        }

        private AstMemberFunctionCall ParseMemberFunctionCall(List<string> identifiers)
        {
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstMemberFunctionCall(identifiers, args);
        }

        private void ParseAssignment(IHaveExpression ast)
        {
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.Equals);
            var value = ParseOr();
            Require(TokenType.Semicolon);
            ast.Expressions.Add(new AstAssignment(ident, value));
        }

        private void ParseDeclarationAssignment(IHaveExpression ast)
        {
            Require(TokenType.Var);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.Equals);
            var value = ParseOr();
            Require(TokenType.Semicolon);
            ast.Expressions.Add(new AstDeclarationAssignment(ident, value));
        }

        private AstExpression ParseOr()
        {
            var expr = ParseAnd();
            while (Match(TokenType.Or))
            {
                Consume();
                var right = ParseAnd();
                expr = new AstCompare(Compare.Or, expr, right);
            }

            return expr;
        }

        private AstExpression ParseAnd()
        {
            var expr = ParseEqualityChecks();
            while (Match(TokenType.And))
            {
                Consume();
                var right = ParseEqualityChecks();
                expr = new AstCompare(Compare.And, expr, right);
            }

            return expr;
        }

        private AstExpression  ParseEqualityChecks()
        {
            var expr = ParseDifferenceChecks();
            while (Match(TokenType.EqualsEquals) || Match(TokenType.NotEquals))
            {
                if (Match(TokenType.EqualsEquals))
                {
                    Consume();
                    var right = ParseDifferenceChecks();
                    expr = new AstCompare(Compare.EqualsEquals, expr, right);
                }
                else if (Match(TokenType.NotEquals))
                {
                    Consume();
                    var right = ParseDifferenceChecks();
                    expr = new AstCompare(Compare.NotEquals, expr, right);
                }
            }

            return expr;
        }
        
        
        private AstExpression  ParseDifferenceChecks()
        {
            var expr = ParsePlusAndMinus();
            while (Match(TokenType.LessThan) || Match(TokenType.GreaterThan) || Match(TokenType.LessThanEquals) || Match(TokenType.GreaterThanEquals))
            {
                if (Match(TokenType.LessThan))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.LessThan, expr, right);
                }
                else if (Match(TokenType.GreaterThan))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.GreaterThan, expr, right);
                }
                else if (Match(TokenType.LessThanEquals))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.LessThanEquals, expr, right);
                }
                else if (Match(TokenType.GreaterThanEquals))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.GreaterThanEquals, expr, right);
                }
            }

            return expr;
        }

        
        private AstRange ParseRanges()
        {
            var expression = ParsePlusAndMinus();
            while (Match(TokenType.DoubleDot))
            {
                if (Match(TokenType.DoubleDot))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expression = new AstRange(expression, right);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            if (!(expression is AstRange))
            {
                throw new ParserException($"For loop expression expected range near line {_buffer[0].Line}, pos {_buffer[0].Col}.");
            }

            return (AstRange)expression;
        }
        
        private AstExpression ParsePlusAndMinus()
        {
            var expression = ParseMultiplicationAndDivision();
            while (Match(TokenType.Plus) || Match(TokenType.Minus) || Match(TokenType.DoubleDot))
            {
                if (Match(TokenType.Plus))
                {
                    Consume();
                    var right = ParseMultiplicationAndDivision();
                    expression = new AstBinaryMathOperation(BinaryMath.Plus, expression, right);
                }
                else if (Match(TokenType.Minus))
                {
                    Consume();
                    var right = ParseMultiplicationAndDivision();
                    expression = new AstBinaryMathOperation(BinaryMath.Minus, expression, right);
                }
                else if (Match(TokenType.DoubleDot))
                {
                    Consume();
                    var right = ParseMultiplicationAndDivision();
                    expression = new AstRange(expression, right);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return expression;
        }

        private AstExpression ParseMultiplicationAndDivision()
        {
            var expression = ParseReferences();
            while (Match(TokenType.Asterisk) || Match(TokenType.Slash))
            {
                if (Match(TokenType.Asterisk))
                {
                    Consume();
                    var right = ParseReferences();
                    expression = new AstBinaryMathOperation(BinaryMath.Times, expression, right);
                }
                else if (Match(TokenType.Slash))
                {
                    Consume();
                    var right = ParseReferences();
                    expression = new AstBinaryMathOperation(BinaryMath.Division, expression, right);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return expression;
        }

        private AstExpression ParseReferences()
        {
            switch (_buffer[0].Type)
            {
                case TokenType.LeftBracket:
                    return ParseListInit();
                case TokenType.LeftParenthesis:
                    Consume();
                    var expr = ParseOr();
                    Require(TokenType.RightParenthesis);
                    return expr;
                case TokenType.String:
                    var str = Require(TokenType.String).Text;
                    return new AstStringConstant(str);
                case TokenType.Integer:
                    var inte = Require(TokenType.Integer).Integer;
                    return new AstIntegerConstant(inte);
                case TokenType.Double:
                    var doubl = Require(TokenType.Double).Double;
                    return new AstDoubleConstant(doubl);
                case TokenType.True:
                    Consume();
                    return new AstTrue();
                case TokenType.False:
                    Consume();
                    return new AstFalse();
                case TokenType.Minus:
                    Consume();
                    var minusExpr = ParseReferences();
                    return new AstUnaryMathOperation(UnaryMath.Minus, minusExpr);
                case TokenType.Dot:
                    Consume();
                    var notExpr = ParseReferences();
                    return new AstUnaryMathOperation(UnaryMath.Not, notExpr);
                case TokenType.Asterisk:
                    Consume();
                    return ParseClassInitialization();
                case TokenType.It:
                    Consume();
                    return new AstIt();
                case TokenType.Identifier:
                    switch (_buffer[1].Type)
                    {
                        case TokenType.LeftBracket:
                            return ParseIndexerExpression();
                        case TokenType.LeftParenthesis:
                            return ParseFunctionCall();
                        case TokenType.Dot:
                            return ParseMemberReferenceOrMemberFunctionCallOrMemberVariableAssignmentOrIndexer();
                        case TokenType.DoubleColon:
                            return ParseModuleAccess();
                    }

                    var ident = Require(TokenType.Identifier).Text;
                    return new AstVariableReference(ident);
            }

            throw new NotImplementedException();
        }

        private AstExpression ParseListInit()
        {
            var args = ParseArgumentList(TokenType.LeftBracket, TokenType.RightBracket);
            return new AstNewList(args);
        }

        private AstExpression ParseModuleAccess()
        {
            throw new NotImplementedException();
        }


        private List<AstExpression> ParseArgumentList(TokenType listStart, TokenType listEnd)
        {
            Require(listStart);
            var args = new List<AstExpression>();
            while (!Match(listEnd) && !Match(TokenType.EOF))
            {
                args.Add(ParseOr());
                if (Match(TokenType.Comma))
                {
                    Consume();
                }
            }

            Require(listEnd);
            return args;
        }


        private List<Reference> GetIdentifierList()
        {
            var identifiers = new List<Reference>();
            var firstIdentifier = Require(TokenType.Identifier).Text;
            identifiers.Add(new Reference(firstIdentifier, Match(TokenType.DoubleColon)));

            if (Match(TokenType.DoubleColon))
                Consume();
            else if (Match(TokenType.Dot))
                Consume();

            while (Match(TokenType.Identifier) && !Match(TokenType.EOF))
            {
                var ident = Require(TokenType.Identifier).Text;
                identifiers.Add(new Reference(ident, Match(TokenType.DoubleColon)));
                if (Match(TokenType.DoubleColon))
                    Consume();
                else if (Match(TokenType.Dot))
                    Consume();
            }

            return identifiers;
        }

        private string GetModuledClassName()
        {
            var name = Require(TokenType.Identifier).Text;
            if (!Match(TokenType.DoubleColon))
            {
                return name;
            }

            while (Match(TokenType.DoubleColon))
            {
                Consume();
                name += "::";
                name += Require(TokenType.Identifier).Text;
            }

            return name;
        }

        private Token Consume()
        {
            var consumed = _buffer[0];
            _buffer[0] = _buffer[1];
            _buffer[1] = _lexer.NextToken();
            return consumed;
        }

        private bool Match(TokenType type)
        {
            return _buffer[0].Type == type;
        }

        private Token Require(TokenType type)
        {
            var consumed = Consume();
            if (consumed.Type != type)
                throw new ParserException(
                    $"Unexpected token {consumed.Type}, expected {type} at line {consumed.Line}, col {consumed.Col}");
            return consumed;
        }
    }

}