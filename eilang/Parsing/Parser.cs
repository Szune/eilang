using System.Collections.Generic;
using System.IO;
using System.Linq;
using eilang.Ast;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Lexing;
using eilang.Tokens;
using eilang.Values;

namespace eilang.Parsing
{
    public class Parser
    {
        private readonly ILexer _lexer;
        private readonly Token[] _buffer = {new Token(), new Token()};
        private bool InLoop => _forDepth > 0;
        private int _forDepth;
        private bool _inClass;
        private bool _inExtensionFunction;
        private Stack<VariableScope> _scopes = new Stack<VariableScope>();
        private Token _lastConsumed = new Token();
        private bool _inConstructor;

        public Parser(ILexer lexer)
        {
            _lexer = lexer;
            Consume();
            Consume();
            _scopes.Push(new VariableScope());
        }

        public AstRoot Parse()
        {
            var root = new AstRoot(new Position(0, 0));
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
            var pos = _lastConsumed.Position;
            var ident = outerModule + Require(TokenType.Identifier).Text;
            var module = new AstModule(ident, pos);
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
            var clas = new AstClass(ident, _lastConsumed.Position);
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
                        ThrowParserException($"Unknown token {_buffer[0].Type} in class {ident}'s scope");
                        return;
                }
            }

            Require(TokenType.RightBrace);
            _inClass = false;
            ast.Classes.Add(clas);
        }

        private void ParseConstructor(AstClass clas)
        {
            Require(TokenType.Constructor);
            var pos = _lastConsumed.Position;
            var args = ParseParameterList();
            
            if (Match(TokenType.Semicolon)) // constructor without code block
            {
                Consume();
                clas.Constructors.Add(new AstConstructor($".ctor::{clas.Name}", args, pos));
                return;
            }
            var ctor = new AstConstructor($".ctor::{clas.Name}", args, pos);
            _inConstructor = true;
            ParseBlock(ctor); // constructor with code block
            _inConstructor = false;
            clas.Constructors.Add(ctor);
        }

        private void ParseMemberVariable(AstClass clas)
        {
            var ident = Require(TokenType.Identifier).Text;
            var pos = _lastConsumed.Position;
            Require(TokenType.Colon);
            var type = GetIdentifierWithModule();
            if (Match(TokenType.Semicolon)) // no initialization
            {
                Consume();
                clas.Variables.Add(new AstMemberVariableDeclaration(ident, type, pos));
                return;
            }

            Require(TokenType.Equals);
            // ⚠Important⚠ initial expression, cannot allow +=, -= etc
            var initExpr = ParseOr(); 
            Require(TokenType.Semicolon);
            clas.Variables.Add(new AstMemberVariableDeclarationWithInit(ident, type, initExpr, pos));
        }

        private void ParseMemberVariableList(AstClass clas)
        {
            if (!_buffer[1].Match(TokenType.Comma))
            {
                ParseMemberVariable(clas);
                return;
            }

            var idents = new List<(string Name, Position Position)>();
            do
            {
                var ident = Require(TokenType.Identifier).Text;
                idents.Add((ident, _lastConsumed.Position));
                if (Match(TokenType.Comma))
                    Consume();
                else
                    break;
            } while (Match(TokenType.Identifier));

            Require(TokenType.Colon);
            var type = GetIdentifierWithModule();

            if (Match(TokenType.Semicolon)) // no initialization
            {
                Consume();
                foreach (var ide in idents)
                    clas.Variables.Add(new AstMemberVariableDeclaration(ide.Name, type, ide.Position));
                return;
            }

            Require(TokenType.Equals);

            // ⚠Important⚠ initial expression, cannot allow +=, -= etc
            var initExpr = ParseOr();
            Require(TokenType.Semicolon);
            foreach (var ide in idents)
                clas.Variables.Add(new AstMemberVariableDeclarationWithInit(ide.Name, type, initExpr, ide.Position));
        }

        private void ParseMemberFunction(AstClass clas)
        {
            Require(TokenType.Function);
            var pos = _lastConsumed.Position;
            var ident = Require(TokenType.Identifier).Text;
            var args = ParseParameterList();
            var fun = new AstMemberFunction(ident, args, pos);
            // parse code block
            ParseBlock(fun);
            clas.Functions.Add(fun);
        }

        private void ParseFunction(IHaveFunction ast)
        {
            Require(TokenType.Function);
            if (_buffer[1].Match(TokenType.DoubleColon) || // moduled, only allow extension methods
                _buffer[0].Match(TokenType.LeftBrace) || // anonymous type, only allow extension methods
                _buffer[1].Match(TokenType.Arrow))
            {
                ParseExtensionFunction(ast);
                return;
            }
            var pos = _lastConsumed.Position;
            var ident = Require(TokenType.Identifier).Text;
            var paramz = ParseParameterList();
            var fun = new AstFunction(ident, paramz, pos);
            // parse code block
            ParseBlock(fun);
            ast.Functions.Add(fun);
        }

        private void ParseExtensionFunction(IHaveFunction ast) // TODO: stop doing this and return the function instead 
        {
            var pos = _lastConsumed.Position;
            // the type we're extending
            _inExtensionFunction = true;
            if (Match(TokenType.LeftBrace))
            {
                ParseAnonymousExtensionFunction(ast);
                return;
            }
            var extending = GetIdentifierWithModule();
            Require(TokenType.Arrow);
            // the function we're adding
            var adding = Require(TokenType.Identifier).Text;

            var paramz = ParseParameterList();
            var fun = new AstExtensionFunction(extending, adding, paramz, pos);
            ParseBlock(fun);
            ast.Functions.Add(fun);
            _inExtensionFunction = false;
        }

        private void ParseAnonymousExtensionFunction(IHaveFunction ast)
        {
            var pos = _lastConsumed.Position;
            var extending = ParseAnonymousTypeArgumentName();
            Require(TokenType.Arrow);
            // the function we're adding
            var adding = Require(TokenType.Identifier).Text;
            var paramz = ParseParameterList();
            var fun = new AstExtensionFunction(extending, adding, paramz, pos);
            ParseBlock(fun);
            ast.Functions.Add(fun);
            _inExtensionFunction = false;
        }

        private string ParseAnonymousTypeArgumentName()
        {
            var members = new List<string>();
            Require(TokenType.LeftBrace);
            while (!Match(TokenType.RightBrace) && !EOF())
            {
                var identifier = Require(TokenType.Identifier).Text;
                members.Add(identifier);
                if (Match(TokenType.Comma))
                {
                    Consume();
                }
            }

            Require(TokenType.RightBrace);
            return GetAnonymousTypeName(members);
        }

        private List<Parameter> ParseParameterList()
        {
            Require(TokenType.LeftParenthesis);
            var paramz = new List<Parameter>();
            while (!Match(TokenType.RightParenthesis) && !Match(TokenType.EOF))
            {
                var arg = Require(TokenType.Identifier).Text;
                Parameter paramType;
                if (!Match(TokenType.Colon))
                {
                    paramType = new Parameter(arg, new List<ParameterType>
                    {
                        new ParameterType(SpecialVariables.AnyType, TypeOfValue.Any)
                    });
                }
                else
                {
                    var parameterTypes = new List<ParameterType>();
                    Consume(); // consume ':'
                    do
                    {
                        var ident = "";
                        if (Match(TokenType.LeftBrace))
                        {
                            ident = ParseAnonymousTypeArgumentName();
                        }
                        else
                        {
                            ident = GetIdentifierWithModule();
                        }
                        var type = Types.GetType(ident);
                        if (type == TypeOfValue.Class && !ident.Contains("::"))
                        {
                            ident = $"{SpecialVariables.Global}::{ident}";
                        }
                        parameterTypes.Add(new ParameterType(ident, type));
                        if (Match(TokenType.Pipe))
                        {
                            Consume(); // consume | and continue
                        }
                        else
                        {
                            break;
                        }
                    } while (!EOF());
                    
                    paramType = new Parameter(arg, parameterTypes);
                }
                paramz.Add(paramType);
                if (Match(TokenType.Comma))
                {
                    Consume();
                }
                else
                {
                    break;
                }
            }

            Require(TokenType.RightParenthesis);
            return paramz;
        }

        private void ParseBlock(IHaveExpression fun)
        {
            Require(TokenType.LeftBrace);
            var currentScope = _scopes.Peek();
            _scopes.Push(new VariableScope(currentScope));
            while (!Match(TokenType.RightBrace) && !Match(TokenType.EOF))
            {
                ParseLeftExpression(fun);
            }
            _scopes.Pop();

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
                case TokenType.Use:
                    ParseUse(ast);
                    return;
                case TokenType.For:
                    ParseFor(ast);
                    return;
                case TokenType.While:
                    ParseWhile(ast);
                    return;
                case TokenType.Return:
                    ParseReturn(ast);
                    return;
                case TokenType.Me:
                    ParseSelfAssignment(ast);
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

        private void ParseUse(IHaveExpression ast)
        {
            // compile to dispose the value in 'ident' at the end of the use block
            Require(TokenType.Use);
            var pos = _lastConsumed.Position;
            Require(TokenType.LeftParenthesis);
            Require(TokenType.Var);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.Equals);
            var expr = ParseDots();
            Require(TokenType.RightParenthesis);
            var block = new AstBlock(_lastConsumed.Position);
            ParseBlock(block);
            ast.Expressions.Add(new AstUse(ident, expr, block, _lastConsumed.Position));
        }

        private void ParseSelfAssignment(IHaveExpression ast)
        {
            if (!(ParseDots() is AstMultiReference multi))
            {
                ThrowParserException("Lone 'me' statement is not allowed.");
                return;
            }

            var lastExpr = multi.GetLastExpression();
            if (lastExpr is AstMemberFunctionCall)
            {
                Require(TokenType.Semicolon);
                ast.Expressions.Add(multi);
                return;
            }

            // expecting an assignment if it's not a function call
            if (!IsNextAssignment())
                ThrowParserException("Expecting an assignment or function call for left 'me' expression.");
            var value = ParseAssignmentValue();
            var set = GetAssignmentSet(multi);

            var define = false;
            if (multi.First is AstMe && multi.Second is AstMemberReference)
            {
                if (_inConstructor)
                {
                    set.ChangeType(AssignmentSet.Variable);
                }
                else
                {
                    set.ChangeType(AssignmentSet.MemberVariable);
                    set.ChangeRequiredReferences(new AstMe(_lastConsumed.Position));
                }
                define = true;
            }

            ast.Expressions.Add(new AstAssignment(new AstAssignmentReference(multi, multi.Position), value, set, multi.Position, define));
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
                if (references is AstIdentifier && Match(TokenType.Semicolon))
                {
                    Consume();
                    return references;
                }
                return ThrowParserException("Expecting an assignment or function call for left expression");
            }

            var value = ParseAssignmentValue();
            var set = GetAssignmentSet(references);

            return new AstAssignment(
                new AstAssignmentReference(references, references.Position), value, set, references.Position);
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
            {
                if (Match(TokenType.Semicolon))
                {
                    Consume();
                    return members;
                }
                ThrowParserException("Expecting an assignment or function call for left member expression");
            }

            var value = ParseAssignmentValue();
            var set = GetAssignmentSet(members);

            return new AstAssignment(
                new AstAssignmentReference(members, members.Position), value, set, members.Position);
        }

        private void ThrowIfMultiReferenceContainsFunctionCallButDoesNotEndWithFunctionCall(AstMultiReference multi)
        {
            // TODO: implement
        }

        private void ParseReturn(IHaveExpression ast)
        {
            Require(TokenType.Return);
            var pos = _lastConsumed.Position;
            if (Match(TokenType.Semicolon))
            {
                ast.Expressions.Add(new AstReturn(null, pos));
                return;
            }
            var retExpr = ParseTernaryOperator();
            ast.Expressions.Add(new AstReturn(retExpr, pos));
        }

        private AstMemberIndexerRef ParseMemberIndexerExpression(string ident)
        {
            var idxExprs = new List<AstExpression>();
            var outerPos = _lastConsumed.Position;
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var pos = _lastConsumed.Position;
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(new AstIndex(expr, pos));
            }
            return new AstMemberIndexerRef(ident, idxExprs, outerPos);
        }

        private AstExpression ParseIndexerExpression(string name)
        {
            var outerPos = _lastConsumed.Position;
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var pos = _lastConsumed.Position;
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(new AstIndex(expr, pos));
            }
            return new AstIndexerReference(name, idxExprs, outerPos);
        }
        
        private AstExpression ParseIndexerOnReturnedValue()
        {
            var outerPos = _lastConsumed.Position;
            var idxExprs = new List<AstExpression>();
            while (Match(TokenType.LeftBracket))
            {
                Consume();
                var pos = _lastConsumed.Position;
                var expr = ParseOr();
                Require(TokenType.RightBracket);
                idxExprs.Add(new AstIndex(expr, pos));
            }
            return new AstIndexerOnReturnedValue(idxExprs, outerPos);
        }
        
        
        private void ParseWhile(IHaveExpression ast)
        {
            Require(TokenType.While);
            var pos = _lastConsumed.Position;
            Require(TokenType.LeftParenthesis);
            var condition = ParseOr();
            Require(TokenType.RightParenthesis);
            var block = new AstBlock(_lastConsumed.Position);
            _forDepth++;
            ParseBlock(block);
            _forDepth--;
            ast.Expressions.Add(new AstWhile(condition, block, pos));
        }


        private void ParseFor(IHaveExpression ast)
        {
            Require(TokenType.For);
            var pos = _lastConsumed.Position;
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
                var forBlock = new AstBlock(_lastConsumed.Position);
                _forDepth++;
                ParseBlock(forBlock);
                ast.Expressions.Add(new AstForInfinite(forBlock, pos));
                _forDepth--;
                return;
            }

            var reversed = false;

            if (Match(TokenType.Tilde))
            {
                reversed = true;
                Consume();
            }
            Require(TokenType.LeftParenthesis);
            var rangePos = _lastConsumed.Position;
            var expression = ParsePlusAndMinus();
            if (Match(TokenType.DoubleDot))
            {
                // for (0..1) -> range
                Require(TokenType.DoubleDot);
                var end = ParsePlusAndMinus();
                Require(TokenType.RightParenthesis);
                var forBlock = new AstBlock(_lastConsumed.Position);
                _forDepth++;
                ParseBlock(forBlock);
                ast.Expressions.Add(new AstForRange(new AstRange(expression, end, rangePos), forBlock, reversed, pos));
            }
            else
            {
                // for (array)
                Require(TokenType.RightParenthesis);
                var forBlock = new AstBlock(_lastConsumed.Position);
                _forDepth++;
                ParseBlock(forBlock);
                ast.Expressions.Add(new AstForArray(expression, forBlock, reversed, pos));
            }
            _forDepth--;
        }

        private void ParseIf(IHaveExpression ast)
        {
            Require(TokenType.If);
            var pos = _lastConsumed.Position;
            Require(TokenType.LeftParenthesis);
            var condition = ParseOr();
            Require(TokenType.RightParenthesis);
            var ifPart = new AstBlock(_lastConsumed.Position);
            ParseBlock(ifPart);
            
            if (!Match(TokenType.Else))
            {
                ast.Expressions.Add(new AstIf(condition, ifPart, null, pos));
                return;
            }

            var firstIf = new AstIf(condition, ifPart, null, pos);
            var currentIf = firstIf;
            while (Match(TokenType.Else))
            {
                Consume();
                if (Match(TokenType.If))
                {
                    Consume();
                    var cond = ParseOr();
                    var ifBlock = new AstBlock(_lastConsumed.Position);
                    ParseBlock(ifBlock);
                    var newIf = new AstIf(cond, ifBlock, null, pos);
                    currentIf.SetElse(newIf);
                    currentIf = newIf;
                }
                else
                {
                    var elseBlock = new AstBlock(_lastConsumed.Position);
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
            if (Match(TokenType.LeftBrace))
            {
                return ParseAnonymousTypeInitialization();
            }
            var identifiers = GetIdentifierWithModule();
            var pos = _lastConsumed.Position;
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstClassInitialization(identifiers, args, pos);
        }

        private AstExpression ParseAnonymousTypeInitialization()
        {
            var pos = _lastConsumed.Position;
            var members = new List<MemberInitialization>();
            Require(TokenType.LeftBrace);
            while (!Match(TokenType.RightBrace) && !EOF())
            {
                var identifier = Require(TokenType.Identifier).Text;
                Require(TokenType.Equals);
                var expr = ParseTernaryOperator();
                members.Add(new MemberInitialization(identifier, expr));
                if (Match(TokenType.Comma))
                {
                    Consume();
                }
            }
            Require(TokenType.RightBrace);

            var name = GetAnonymousTypeName(members.Select(x => x.Name));
            return new AstAnonymousTypeInitialization(name, members, pos);
        }

        private string GetAnonymousTypeName(IEnumerable<string> memberNames)
        {
            return string.Join("`", memberNames);
        }

        private bool EOF()
        {
            return Match(TokenType.EOF);
        }

        private AstFunctionCall ParseFunctionCall(string name)
        {
            var pos = _lastConsumed.Position;
            var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
            return new AstFunctionCall(name, args, pos);
        }

        private void ParseDeclarationAssignment(IHaveExpression ast)
        {
            Require(TokenType.Var);
            var pos = _lastConsumed.Position;
            var ident = Require(TokenType.Identifier).Text;
            _scopes.Peek().DefineVariable(ident, _buffer[0].Position.Line, _buffer[0].Position.Col);
            Require(TokenType.Equals);
            // ⚠Important⚠ initial expression, cannot allow +=, -= etc
            var value = ParseTernaryOperator();
            Require(TokenType.Semicolon);
            ast.Expressions.Add(new AstDeclarationAssignment(ident, value, pos));
        }

        private AstExpression ParseTernaryOperator()
        {
            var pos = _lastConsumed.Position;
            var expression = ParseOr();
            if (!Match(TokenType.QuestionMark))
                return expression;
            Consume(); // Consume question mark
            var trueExpr = ParseTernaryOperator();
            Require(TokenType.Colon);
            var falseExpr = ParseTernaryOperator();
            return new AstTernary(expression, trueExpr, falseExpr, pos);
        }

        private AstExpression ParseOr()
        {
            var pos = _lastConsumed.Position;
            var expr = ParseAnd();
            while (Match(TokenType.Or))
            {
                Consume();
                var right = ParseAnd();
                expr = new AstCompare(Compare.Or, expr, right, pos);
            }

            return expr;
        }

        private AstExpression ParseAnd()
        {
            var pos = _lastConsumed.Position;
            var expr = ParseEqualityChecks();
            while (Match(TokenType.And))
            {
                Consume();
                var right = ParseEqualityChecks();
                expr = new AstCompare(Compare.And, expr, right, pos);
            }

            return expr;
        }

        private AstExpression  ParseEqualityChecks()
        {
            var pos = _lastConsumed.Position;
            var expr = ParseDifferenceChecks();
            while (Match(TokenType.EqualsEquals) || Match(TokenType.NotEquals))
            {
                if (Match(TokenType.EqualsEquals))
                {
                    Consume();
                    var right = ParseDifferenceChecks();
                    expr = new AstCompare(Compare.EqualsEquals, expr, right, pos);
                }
                else if (Match(TokenType.NotEquals))
                {
                    Consume();
                    var right = ParseDifferenceChecks();
                    expr = new AstCompare(Compare.NotEquals, expr, right, pos);
                }
            }

            return expr;
        }
        
        
        private AstExpression  ParseDifferenceChecks()
        {
            var pos = _lastConsumed.Position;
            var expr = ParsePlusAndMinus();
            while (Match(TokenType.LessThan) || Match(TokenType.GreaterThan) || Match(TokenType.LessThanEquals) || Match(TokenType.GreaterThanEquals))
            {
                if (Match(TokenType.LessThan))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.LessThan, expr, right, pos);
                }
                else if (Match(TokenType.GreaterThan))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.GreaterThan, expr, right, pos);
                }
                else if (Match(TokenType.LessThanEquals))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.LessThanEquals, expr, right, pos);
                }
                else if (Match(TokenType.GreaterThanEquals))
                {
                    Consume();
                    var right = ParsePlusAndMinus();
                    expr = new AstCompare(Compare.GreaterThanEquals, expr, right, pos);
                }
            }

            return expr;
        }


        private AstExpression ParsePlusAndMinus()
        {
            var pos = _lastConsumed.Position;
            var expression = ParseMultiplicationAndDivisionAndModulo();
            while (Match(TokenType.Plus) || Match(TokenType.Minus))
            {
                if (Match(TokenType.Plus))
                {
                    Consume();
                    var right = ParseMultiplicationAndDivisionAndModulo();
                    expression = new AstBinaryMathOperation(BinaryMath.Plus, expression, right, pos);
                }
                else if (Match(TokenType.Minus))
                {
                    Consume();
                    var right = ParseMultiplicationAndDivisionAndModulo();
                    expression = new AstBinaryMathOperation(BinaryMath.Minus, expression, right, pos);
                }
                else
                {
                    ThrowParserException("Not implemented");
                }
            }

            return expression;
        }

        private AstExpression ParseMultiplicationAndDivisionAndModulo()
        {
            var pos = _lastConsumed.Position;
            var expression = ParseIncrementDecrementUnaryOperators();
            while (Match(TokenType.Asterisk) || Match(TokenType.Slash) || Match(TokenType.Percent))
            {
                if (Match(TokenType.Asterisk))
                {
                    Consume();
                    var right = ParseIncrementDecrementUnaryOperators();
                    expression = new AstBinaryMathOperation(BinaryMath.Times, expression, right, pos);
                }
                else if (Match(TokenType.Slash))
                {
                    Consume();
                    var right = ParseIncrementDecrementUnaryOperators();
                    expression = new AstBinaryMathOperation(BinaryMath.Division, expression, right, pos);
                }
                else if (Match(TokenType.Percent))
                {
                    Consume();
                    var right = ParseIncrementDecrementUnaryOperators();
                    expression = new AstBinaryMathOperation(BinaryMath.Modulo, expression, right, pos);
                }
                else
                {
                    ThrowParserException("Not implemented");
                }
            }

            return expression;
        }

        private AstExpression ParseIncrementDecrementUnaryOperators()
        {
            var pos = _lastConsumed.Position;
            var expression = ParseDots();
            if (Match(TokenType.PlusPlus))
            {
                Consume();
                var assignmentSet = GetAssignmentSet(expression);
                expression = new AstAssignment(
                    new AstAssignmentReference(expression, pos),
                    new AstAssignmentValue(AstEmpty.Value, Assignment.IncrementAndReference, pos), 
                    assignmentSet, pos);
            }
            else if (Match(TokenType.MinusMinus))
            {
                Consume();
                var assignmentSet = GetAssignmentSet(expression);
                expression = new AstAssignment(
                    new AstAssignmentReference(expression, pos),
                    new AstAssignmentValue(AstEmpty.Value, Assignment.DecrementAndReference, pos), 
                    assignmentSet, pos);
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
                    return new AstAssignmentSet("", AssignmentSet.Array, new AstIdentifier(refer.Identifier, refer.Position),
                        refer.IndexExprs);
                case AstMultiReference members:
                {
                    switch (members.GetLastExpression())
                    {
                        case AstMemberIndexerRef membIdx:
                            return new AstAssignmentSet("",
                                AssignmentSet.Array,
                                new AstMultiReference(members.First, new AstMemberReference(membIdx.Ident, membIdx.Position), members.Position),
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
            var pos = _lastConsumed.Position;
            var expression = ParseReferences();
            while (Match(TokenType.Dot))
            {
                expression = new AstMultiReference(expression, ParseMemberAccess(), pos);
            }

            return expression;
        }

        private AstExpression ParseMemberAccess()
        {
            var pos = _lastConsumed.Position;
            AstExpression expression;
            Require(TokenType.Dot);
            var ident = Require(TokenType.Identifier).Text;
            if (Match(TokenType.LeftParenthesis))
            {
                // member func call
                var args = ParseArgumentList(TokenType.LeftParenthesis, TokenType.RightParenthesis);
                expression = new AstMemberFunctionCall(ident, args, pos);
                if (Match(TokenType.LeftBracket))
                {
                    expression = new AstNestedExpression(expression, ParseIndexerOnReturnedValue(), pos);
                }
            }
            else if (Match(TokenType.LeftBracket))
            {
                expression = ParseMemberIndexerExpression(ident);
            }
            else
            {
                // member variable ref + more
                expression = new AstMemberReference(ident, pos);
            }

            return expression;
        }

        private bool IsNextAssignment()
        {
            return Match(TokenType.Equals) ||
                   Match(TokenType.DivideEquals) ||
                   Match(TokenType.TimesEquals) ||
                   Match(TokenType.PlusEquals) ||
                   Match(TokenType.ModuloEquals) ||
                   Match(TokenType.PlusPlus) ||
                   Match(TokenType.MinusMinus) ||
                   Match(TokenType.MinusEquals);
        }

        private AstAssignmentValue ParseAssignmentValue()
        {
            var pos = _lastConsumed.Position;
            switch (_buffer[0].Type)
            {
                case TokenType.Equals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.Equals, pos);
                }
                case TokenType.DivideEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.DivideEquals, pos);
                }
                case TokenType.TimesEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.TimesEquals, pos);
                }
                case TokenType.ModuloEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.ModuloEquals, pos);
                }
                case TokenType.PlusEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.PlusEquals, pos);
                }
                case TokenType.MinusEquals:
                {
                    Consume();
                    var value = ParseTernaryOperator();
                    return new AstAssignmentValue(value, Assignment.MinusEquals, pos);
                }
                case TokenType.PlusPlus:
                {
                    Consume();
                    return new AstAssignmentValue(AstEmpty.Value, Assignment.Increment, pos);
                }
                case TokenType.MinusMinus:
                {
                    Consume();
                    return new AstAssignmentValue(AstEmpty.Value, Assignment.Decrement, pos);
                }
            }

            return ThrowParserException("Assignment expression expected");
        }


        private AstExpression ParseReferences()
        {
            var pos = _lastConsumed.Position;
            switch (_buffer[0].Type)
            {
                case TokenType.LeftBracket:
                    return ParseListInit();
                case TokenType.LeftParenthesis:
                    Consume();
                    if (Match(TokenType.RightParenthesis))
                    {
                        Consume();
                        return new AstUninitialized(pos);
                    }
                    var expr = ParseTernaryOperator();
                    Require(TokenType.RightParenthesis);
                    return new AstParenthesized(expr, pos);
                case TokenType.Continue:
                    if (!InLoop)
                    {
                        ThrowParserException("'continue' is only allowed in loops");
                    }
                    Consume();
                    return new AstContinue(pos);
                case TokenType.Break:
                    if (!InLoop)
                    {
                        ThrowParserException("'break' is only allowed in loops");
                    }
                    Consume();
                    return new AstBreak(pos);
                case TokenType.String:
                    var str = Require(TokenType.String).Text;
                    return new AstStringConstant(str, pos);
                case TokenType.Integer:
                    var inte = Require(TokenType.Integer).Integer;
                    return new AstIntegerConstant(inte, pos);
                case TokenType.Double:
                    var doubl = Require(TokenType.Double).Double;
                    return new AstDoubleConstant(doubl, pos);
                case TokenType.Me:
                    if (!_inClass && !_inExtensionFunction)
                    {
                        ThrowParserException($"Token '{nameof(TokenType.Me)}' may only live inside classes");
                    }
                    Consume();
                    if (Match(TokenType.LeftBracket))
                    {
                        return ParseIndexerExpression(SpecialVariables.Me);
                    }
                    return new AstMe(pos);
                case TokenType.True:
                    Consume();
                    return new AstTrue(pos);
                case TokenType.False:
                    Consume();
                    return new AstFalse(pos);
                case TokenType.Minus:
                    Consume();
                    var minusExpr = ParseDots();
                    return new AstUnaryMathOperation(UnaryMath.Minus, minusExpr, pos);
                case TokenType.Not:
                    Consume();
                    var notExpr = ParseDots();
                    return new AstUnaryMathOperation(UnaryMath.Not, notExpr, pos);
                case TokenType.Asterisk:
                    Consume();
                    return ParseClassInitialization();
                case TokenType.It:
                    Consume();
                    return new AstIt(pos);
                case TokenType.Ix:
                    Consume();
                    return new AstIx(pos);
                case TokenType.Identifier:
                    return ParseModuleAccess();
                case TokenType.At:
                    Consume();
                    return new AstFunctionPointer(GetIdentifierWithModule(), pos);
            }

            return ThrowParserException("not implemented");
        }
        
        private AstExpression ParseModuleAccess()
        {
            var pos = _lastConsumed.Position;
            var name = GetIdentifierWithModule();
            switch (_buffer[0].Type)
            {
                case TokenType.LeftBracket:
                    return ParseIndexerExpression(name);
                case TokenType.LeftParenthesis:
                    var call = ParseFunctionCall(name);
                    if (Match(TokenType.LeftBracket))
                        return new AstNestedExpression(call, ParseIndexerOnReturnedValue(), pos);
                    return call;
                default:
                    return new AstIdentifier(name, pos);
            }
        }

        private AstExpression ParseListInit()
        {
            var pos = _lastConsumed.Position;
            var args = ParseArgumentList(TokenType.LeftBracket, TokenType.RightBracket);
            return new AstNewList(args, pos);
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
                else
                {
                    break;
                }
            }

            Require(listEnd);
            return args;
        }


        private string GetIdentifierWithModule()
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
            _lastConsumed = consumed;
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
            {
                var fileName = Path.GetFileName(_lexer.CurrentScript);
                throw new ParserException(
                    $"Unexpected token {consumed.Type}, expected {type} at line {consumed.Position.Line + 1}, col {consumed.Position.Col}\nInside script '{fileName}': {_lexer.CurrentScript}");
            }

            return consumed;
        }
        
        private dynamic ThrowParserException(string s)
        {
            var value0 = _buffer[0].GetValue();
            var value1 = _buffer[1].GetValue();
            value0 = string.IsNullOrWhiteSpace(value0) ? "" : ": " + value0;
            value1 = string.IsNullOrWhiteSpace(value1) ? "" : ": " + value1;
            var fileName = Path.GetFileName(_lexer.CurrentScript);
            throw new ParserException($"{s} near line {_lastConsumed.Position.Line}, pos {_lastConsumed.Position.Col}.\nNear code: \"{_buffer[0].GetTextBack()}{_buffer[1].GetTextBack()}\"\nInside script '{fileName}': {_lexer.CurrentScript}\n_buffer[0] = {_buffer[0].Type}{value0}, _buffer[1] = {_buffer[1].Type}{value1}");
            return null;
        }
    }
}