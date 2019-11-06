using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using eilang.Ast;
using eilang.Interfaces;

namespace eilang
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private readonly Token[] _buffer = {new Token(), new Token()};
        private bool InLoop => _forDepth > 0;
        private int _forDepth;
        private bool _inClass;

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

        private void ParseModule(AstRoot root, string outerModule = "")
        {
            Require(TokenType.Module);
            var ident = outerModule + Require(TokenType.Identifier).Text;
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
                    case TokenType.Module:
                        ParseModule(root, $"{ident}::");
                        break;
                    default:
                        ThrowParserException("Unknown token");
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
            _inClass = true;
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
            _inClass = false;
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
            // ⚠Important⚠ initial expression, cannot allow +=, -= etc
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

            // ⚠Important⚠ initial expression, cannot allow +=, -= etc
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
                    ParseDeclarationAssignment(ast);
                    return;
                case TokenType.Identifier:
                    ParseLeftIdentifierExpression(ast);
                    break;
                case TokenType.Semicolon:
                    Consume();
                    return;
                default:
                    ast.Expressions.Add(ParseOr());
                    Require(TokenType.Semicolon);
                    return;
            }
        }

        private void ParseLeftIdentifierExpression(IHaveExpression ast)
        {
            var references = ParseDots();
            AstExpression expression;
            if (references is AstMultiReference members)
            {
                expression = ParseLeftMemberVariableExpression(members);
            }
            else
            {
                expression = ParseLeftLocalOrGlobalVariableExpression(references);
            }
            ast.Expressions.Add(expression);
        }

        /// <summary>
        /// For local/global variables
        /// </summary>
        /// <param name="references"></param>
        /// <returns></returns>
        private AstExpression ParseLeftLocalOrGlobalVariableExpression(AstExpression references)
        {
            if (references is AstFunctionCall || 
                references is AstClassInitialization || 
                references is AstUnaryMathOperation || 
                references is AstAssignment)
            {
                Require(TokenType.Semicolon);
                return references;
            }

            if (!IsNextAssignment())
            {
                return ThrowParserException("Expecting an assignment or function call for left expression");
            }

            var value = ParseAssignmentValue();
            var set = GetAssignmentSet(references);

            return new AstAssignment(
                new AstAssignmentReference(references), value, set);
        }

        /// <summary>
        /// For member variables
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        private AstExpression ParseLeftMemberVariableExpression(AstMultiReference members)
        {
            ThrowIfMultiReferenceContainsFunctionCallButDoesNotEndWithFunctionCall(members);
            var lastExpr = members.GetLastExpression();
            if (lastExpr is AstMemberFunctionCall)
            {
                Require(TokenType.Semicolon);
                return members;
            }

            // expecting an assignment if it's not a function call
            if (!IsNextAssignment())
                ThrowParserException("Expecting an assignment or function call for left expression");
            var value = ParseAssignmentValue();
            var set = GetAssignmentSet(members);

            return new AstAssignment(
                new AstAssignmentReference(members), value, set);
        }

        private void ThrowIfMultiReferenceContainsFunctionCallButDoesNotEndWithFunctionCall(AstMultiReference multi)
        {
            // TODO: implement
        }

        private void ParseReturn(IHaveExpression ast)
        {
            Require(TokenType.Return);
            if (Match(TokenType.Semicolon))
            {
                ast.Expressions.Add(new AstReturn(null));
                return;
            }
            var retExpr = ParseTernaryOperator();
            ast.Expressions.Add(new AstReturn(retExpr));
        }

        private AstMemberIndexerRef ParseMemberIndexerExpression(string ident)
        {
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(new AstIndex(expr));
            }
            return new AstMemberIndexerRef(ident, idxExprs);
        }

        private AstExpression ParseIndexerExpression(string name)
        {
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(new AstIndex(expr));
            }
            return new AstIndexerReference(name, idxExprs);
        }

        private void ParseFor(IHaveExpression ast)
        {
            Require(TokenType.For);
            if (_buffer[0].Match(TokenType.LeftBrace) || 
                (_buffer[0].Match(TokenType.LeftParenthesis) && 
                 _buffer[1].Match(TokenType.RightParenthesis)))
            {
                // infinite loop
                if (Match(TokenType.LeftParenthesis))
                {
                    Consume();
                    Consume();
                }
                var forBlock = new AstBlock();
                _forDepth++;
                ParseBlock(forBlock);
                ast.Expressions.Add(new AstForInfinite(forBlock));
                _forDepth--;
                return;
            }
            Require(TokenType.LeftParenthesis);
            var expression = ParsePlusAndMinus();
            if (Match(TokenType.DoubleDot))
            {
                // for (0..1) -> range
                Require(TokenType.DoubleDot);
                var end = ParsePlusAndMinus();
                Require(TokenType.RightParenthesis);
                var forBlock = new AstBlock();
                _forDepth++;
                ParseBlock(forBlock);
                ast.Expressions.Add(new AstForRange(new AstRange(expression, end), forBlock));
            }
            else
            {
                // for (array)
                Require(TokenType.RightParenthesis);
                var forBlock = new AstBlock();
                _forDepth++;
                ParseBlock(forBlock);
                ast.Expressions.Add(new AstForArray(expression, forBlock));
            }
            _forDepth--;
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


        private AstExpression ParseClassInitialization()
        {
            var identifiers = GetModuledClassName();
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstClassInitialization(identifiers, args);
        }

        private AstFunctionCall ParseFunctionCall(string name)
        {
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstFunctionCall(name, args);
        }

        private void ParseDeclarationAssignment(IHaveExpression ast)
        {
            Require(TokenType.Var);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.Equals);
            // ⚠Important⚠ initial expression, cannot allow +=, -= etc
            var value = ParseTernaryOperator();
            Require(TokenType.Semicolon);
            ast.Expressions.Add(new AstDeclarationAssignment(ident, value));
        }

        private AstExpression ParseTernaryOperator()
        {
            var expression = ParseOr();
            if (!Match(TokenType.QuestionMark))
                return expression;
            Consume(); // Consume question mark
            var trueExpr = ParseTernaryOperator();
            Require(TokenType.Colon);
            var falseExpr = ParseTernaryOperator();
            return new AstTernary(expression, trueExpr, falseExpr);
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


        private AstExpression ParsePlusAndMinus()
        {
            var expression = ParseMultiplicationAndDivision();
            while (Match(TokenType.Plus) || Match(TokenType.Minus))
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
                else
                {
                    throw new NotImplementedException();
                }
            }

            return expression;
        }

        private AstExpression ParseMultiplicationAndDivision()
        {
            var expression = ParseIncrementDecrementUnaryOperators();
            while (Match(TokenType.Asterisk) || Match(TokenType.Slash))
            {
                if (Match(TokenType.Asterisk))
                {
                    Consume();
                    var right = ParseIncrementDecrementUnaryOperators();
                    expression = new AstBinaryMathOperation(BinaryMath.Times, expression, right);
                }
                else if (Match(TokenType.Slash))
                {
                    Consume();
                    var right = ParseIncrementDecrementUnaryOperators();
                    expression = new AstBinaryMathOperation(BinaryMath.Division, expression, right);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }

            return expression;
        }

        private AstExpression ParseIncrementDecrementUnaryOperators()
        {
            var expression = ParseDots();
            if (Match(TokenType.PlusPlus))
            {
                Consume();
                var assignmentSet = GetAssignmentSet(expression);
                expression = new AstAssignment(
                    new AstAssignmentReference(expression),
                    new AstAssignmentValue(AstEmpty.Value, Assignment.IncrementAndReference), 
                    assignmentSet);
            }
            else if (Match(TokenType.MinusMinus))
            {
                Consume();
                var assignmentSet = GetAssignmentSet(expression);
                expression = new AstAssignment(
                    new AstAssignmentReference(expression),
                    new AstAssignmentValue(AstEmpty.Value, Assignment.DecrementAndReference), 
                    assignmentSet);
            }

            return expression;
        }

        private AstAssignmentSet GetAssignmentSet(AstExpression expression)
        {
            switch (expression)
            {
                case AstIdentifier id:
                    return new AstAssignmentSet(id.Ident, AssignmentSet.Variable, id);
                case AstIndexerReference refer:
                    return new AstAssignmentSet("", AssignmentSet.Array, new AstIdentifier(refer.Identifier),
                        refer.IndexExprs);
                case AstMultiReference members:
                {
                    switch (members.GetLastExpression())
                    {
                        case AstMemberIndexerRef membIdx:
                            return new AstAssignmentSet("",
                                AssignmentSet.Array,
                                new AstMultiReference(members.First, new AstMemberReference(membIdx.Ident)),
                                membIdx.IndexExprs);
                        case AstMemberReference membId:
                            return new AstAssignmentSet(membId.Ident, AssignmentSet.MemberVariable, members.First);
                        default:
                            return ThrowParserException("Unknown assignment type");
                    }
                }
                default:
                    return ThrowParserException("Unknown assignment type");
            }
        }

        private AstExpression ParseDots()
        {
            var expression = ParseReferences();
            while (Match(TokenType.Dot))
            {
                expression = new AstMultiReference(expression, ParseMemberAccess());
            }

            return expression;
        }

        private AstExpression ParseMemberAccess()
        {
            AstExpression expression;
            Require(TokenType.Dot);
            var ident = Require(TokenType.Identifier).Text;
            if (Match(TokenType.LeftParenthesis))
            {
                // member func call
                var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
                expression = new AstMemberFunctionCall(ident, args);
            }
            else if (Match(TokenType.LeftBracket))
            {
                expression = ParseMemberIndexerExpression(ident);
            }
            else
            {
                // member variable ref + more
                expression = new AstMemberReference(ident);
            }

            return expression;
        }

        private bool IsNextAssignment()
        {
            return Match(TokenType.Equals) ||
                   Match(TokenType.DivideEquals) ||
                   Match(TokenType.TimesEquals) ||
                   Match(TokenType.PlusEquals) ||
                   Match(TokenType.PlusPlus) ||
                   Match(TokenType.MinusMinus) ||
                   Match(TokenType.MinusEquals);
        }

        private AstAssignmentValue ParseAssignmentValue()
        {
            switch (_buffer[0].Type)
            {
                case TokenType.Equals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.Equals);
                }
                case TokenType.DivideEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.DivideEquals);
                }
                case TokenType.TimesEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.TimesEquals);
                }
                case TokenType.PlusEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.PlusEquals);
                }
                case TokenType.MinusEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.MinusEquals);
                }
                case TokenType.PlusPlus:
                {
                    Consume();
                    return new AstAssignmentValue(AstEmpty.Value, Assignment.Increment);
                }
                case TokenType.MinusMinus:
                {
                    Consume();
                    return new AstAssignmentValue(AstEmpty.Value, Assignment.Decrement);
                }
            }

            return ThrowParserException("Assignment expression expected");
        }

        private dynamic ThrowParserException(string s)
        {
            throw new ParserException($"{s} near line {_buffer[0].Line}, pos {_buffer[0].Col}.\n_buffer[0] = {_buffer[0].Type}, _buffer[1] = {_buffer[1].Type}");
            return null;
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
                case TokenType.Continue:
                    if (!InLoop)
                    {
                        ThrowParserException("'continue' is only allowed in loops");
                    }
                    Consume();
                    return new AstContinue();
                case TokenType.Break:
                    if (!InLoop)
                    {
                        ThrowParserException("'break' is only allowed in loops");
                    }
                    Consume();
                    return new AstBreak();
                case TokenType.String:
                    var str = Require(TokenType.String).Text;
                    return new AstStringConstant(str);
                case TokenType.Integer:
                    var inte = Require(TokenType.Integer).Integer;
                    return new AstIntegerConstant(inte);
                case TokenType.Double:
                    var doubl = Require(TokenType.Double).Double;
                    return new AstDoubleConstant(doubl);
                case TokenType.Me:
                    if (!_inClass)
                    {
                        ThrowParserException($"Token '{nameof(TokenType.Me)}' may only live inside classes");
                    }
                    Consume();
                    return new AstMe();
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
                case TokenType.Not:
                    Consume();
                    var notExpr = ParseReferences();
                    return new AstUnaryMathOperation(UnaryMath.Not, notExpr);
                case TokenType.Asterisk:
                    Consume();
                    return ParseClassInitialization();
                case TokenType.It:
                    Consume();
                    return new AstIt();
                case TokenType.Ix:
                    Consume();
                    return new AstIx();
                case TokenType.Identifier:
                    return ParseModuleAccess();
            }

            return ThrowParserException("not implemented");
        }
        
        private AstExpression ParseModuleAccess()
        {
            var name = GetModuledClassName();
            switch (_buffer[0].Type)
            {
                case TokenType.LeftBracket:
                    return ParseIndexerExpression(name);
                case TokenType.LeftParenthesis:
                    return ParseFunctionCall(name);
                default:
                    return new AstIdentifier(name);
            }
        }

        private AstExpression ParseListInit()
        {
            var args = ParseArgumentList(TokenType.LeftBracket, TokenType.RightBracket);
            return new AstNewList(args);
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


        private string GetModuledClassName()
        {
            var name = Require(TokenType.Identifier).Text;

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
                    $"Unexpected token {consumed.Type}, expected {type} at line {consumed.Line + 1}, col {consumed.Col}");
            return consumed;
        }
    }
}