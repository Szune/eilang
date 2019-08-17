using System;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace eilang
{
    public class Parser
    {
        private readonly Lexer _lexer;
        private readonly Token[] _buffer = { new Token(), new Token() };
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
            while(!Match(TokenType.RightBrace))
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
            while (!Match(TokenType.RightBrace))
            {
                switch (_buffer[0].Type)
                {
                    case TokenType.Function:
                        ParseMemberFunction(clas);
                        break;
                }
            }
            Require(TokenType.RightBrace);
            ast.Classes.Add(clas);
        }

        private void ParseMemberFunction(AstClass clas)
        {
            Require(TokenType.Function);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.LeftParenthesis);
            var args = new List<string>();
            while(!Match(TokenType.RightParenthesis))
            {
                var arg = Require(TokenType.Identifier).Text;
                args.Add(arg);
                if(Match(TokenType.Comma))
                {
                    Require(TokenType.Comma);
                }
            }
            Require(TokenType.RightParenthesis);
            var fun = new AstMemberFunction(ident, args);
            // parse code block
            ParseBlock(fun);
            clas.Functions.Add(fun);
        }

        private void ParseFunction(IHaveFunction ast) {
            Require(TokenType.Function);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.LeftParenthesis);
            var args = new List<string>();
            while(!Match(TokenType.RightParenthesis))
            {
                var arg = Require(TokenType.Identifier).Text;
                args.Add(arg);
                if(Match(TokenType.Comma))
                {
                    Require(TokenType.Comma);
                }
            }
            Require(TokenType.RightParenthesis);
            var fun = new AstFunction(ident, args);
            // parse code block
            ParseBlock(fun);
            ast.Functions.Add(fun);
        }

        private void ParseBlock(AstFunction fun)
        {
            Require(TokenType.LeftBrace);
            while (!Match(TokenType.RightBrace))
            {
                ParseLeftExpression(fun);
            }
            // parse expressions
            Require(TokenType.RightBrace);
        }

        private void ParseLeftExpression(IHaveExpression ast)
        {
            switch(_buffer[0].Type)
            {
                // case TokenType.If:
                    // ParseIf(ast);
                    // return;
                case TokenType.Var:
                    if (_inMemberAssignment)
                    {
                        throw new ParserException($"Unexpected variable declaration containing another member variable assignment at line {_buffer[0].Line}, col {_buffer[0].Col}");
                    }
                    ParseDeclarationAssignment(ast);
                    return;
                case TokenType.Identifier:
                    switch(_buffer[1].Type)
                    {
                        case TokenType.Equals:
                            if (_inMemberAssignment)
                            {
                                throw new ParserException($"Unexpected variable assignment containing another member variable assignment at line {_buffer[0].Line}, col {_buffer[0].Col}");
                            }
                            ParseAssignment(ast);
                            return;
                        default:
                            ast.Expressions.Add(ParseRightExpression());
                            Require(TokenType.Semicolon);
                            return;
                    }
                case TokenType.Semicolon:
                    Consume();
                    return;
                default:
                    ast.Expressions.Add(ParseRightExpression());
                    Require(TokenType.Semicolon);
                    return;
            }
            throw new NotImplementedException();
        }

        private AstExpression ParseMemberReferenceOrMemberFunctionCallOrMemberVariableAssignment()
        {
            var firstIdentifier = Require(TokenType.Identifier).Text;
            var identifiers = new List<string> {firstIdentifier};
            
            while (Match(TokenType.Dot))
            {
                Require(TokenType.Dot);
                var nextIdentifier = Require(TokenType.Identifier).Text;
                identifiers.Add(nextIdentifier);
                if (Match(TokenType.Semicolon))
                {
                    return new AstMemberVariableReference(identifiers);
                }
                else if (Match(TokenType.LeftParenthesis))
                {
                    return ParseMemberFunctionCall(identifiers);
                }
                else if (Match(TokenType.Equals))
                {
                    if (_inMemberAssignment)
                    {
                        throw new ParserException($"Unexpected member variable assignment containing another member variable assignment at line {_buffer[0].Line}, col {_buffer[0].Col}");
                    }
                    _inMemberAssignment = true;
                    var assignVal = ParseRightExpression();
                    var assignment = new AstMemberVariableAssignment(identifiers, assignVal);
                    _inMemberAssignment = false;
                    return assignment;
                }
            }
            throw new ParserException($"Unexpected token {_buffer[0].Type} at line {_buffer[0].Line}, col {_buffer[0].Col}");
        }

        private AstExpression ParseClassInitialization()
        {
            var identifiers = new List<Reference>();
            var firstIdentifier = Require(TokenType.Identifier).Text;
            identifiers.Add(new Reference(firstIdentifier, Match(TokenType.DoubleColon)));
            
            if (Match(TokenType.DoubleColon))
                Require(TokenType.DoubleColon);
            else if (Match(TokenType.Dot))
                Require(TokenType.Dot);
            
            while(!Match(TokenType.LeftParenthesis))
            {
                var ident = Require(TokenType.Identifier).Text;
                identifiers.Add(new Reference(ident, Match(TokenType.DoubleColon)));
                if (Match(TokenType.DoubleColon))
                    Require(TokenType.DoubleColon);
                else if (Match(TokenType.Dot))
                    Require(TokenType.Dot);
            }
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
            var value = ParseRightExpression();
            Require(TokenType.Semicolon);
            ast.Expressions.Add(new AstAssignment(ident, value));
        }

        private void ParseDeclarationAssignment(IHaveExpression ast)
        {
            Require(TokenType.Var);
            var ident = Require(TokenType.Identifier).Text;
            Require(TokenType.Equals);
            var value = ParseRightExpression();
            Require(TokenType.Semicolon);
            ast.Expressions.Add(new AstDeclarationAssignment(ident, value));
        }

        private AstExpression ParseRightExpression()
        {
            switch(_buffer[0].Type)
            {
                case TokenType.String:
                    var str = Require(TokenType.String).Text;
                    return new AstStringConstant(str);
                case TokenType.Integer:
                    var inte = Require(TokenType.Integer).Integer;
                    return new AstIntegerConstant(inte);
                case TokenType.Double:
                    var doubl = Require(TokenType.Double).Double;
                    return new AstDoubleConstant(doubl);
                case TokenType.Asterisk:
                    Require(TokenType.Asterisk);
                    return ParseClassInitialization();
                case TokenType.Identifier:
                    switch(_buffer[1].Type)
                    {
                        case TokenType.LeftParenthesis:
                            return ParseFunctionCall();
                        case TokenType.Dot:
                            return ParseMemberReferenceOrMemberFunctionCallOrMemberVariableAssignment();
                        case TokenType.DoubleColon:
                            return ParseModuleAccess();
                    }
                    var ident = Require(TokenType.Identifier).Text;
                    return new AstVariableReference(ident);
            }
            throw new NotImplementedException();
        }

        private AstExpression ParseModuleAccess()
        {
            throw new NotImplementedException();
        }


        private List<AstExpression> ParseArgumentList(TokenType listStart, TokenType listEnd)
        {
            Require(listStart);
            var args = new List<AstExpression>();
            while(!Match(listEnd))
            {
                args.Add(ParseRightExpression());
                if(Match(TokenType.Comma))
                {
                    Require(TokenType.Comma);
                }
            }
            Require(listEnd);
            return args;
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
                throw new ParserException($"Unexpected token {consumed.Type}, expected {type}");
            return consumed;
        }
    }
}