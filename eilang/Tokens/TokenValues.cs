namespace eilang.Tokens
{
    public static class TokenValues
    {
        #region keywords
        public const string If = "if";
        public const string Switch = "switch";
        public const string Else = "else";
        public const string ElseIf = "elif";
        public const string Class = "typ";
        public const string Struct = "struct";
        public const string Module = "modu";
        public const string Function = "fun";
        public const string Var = "var";
        public const string Constructor = "ctor";
        public const string For = "for";
        public const string While = "while";
        public const string True = "true";
        public const string False = "false";
        public const string It = "it";
        public const string Ix = "ix";
        public const string Return = "ret";
        public const string Me = "me";
        public const string Continue = "continue";
        public const string Break = "break";
        public const string Import = "import";
        public const string Use = "use";
        public const string TypeOf = "typeof";
        #endregion
        
        #region multichar tokens
        public const string Or = "||";
        public const string And = "&&";
        public const string DoubleColon = "::";
        public const string DoubleDot = "..";
        public const string PlusEquals = "+=";
        public const string MinusEquals = "-=";
        public const string TimesEquals = "*=";
        public const string DivideEquals = "/=";
        public const string PlusPlus = "++";
        public const string MinusMinus = "--";
        public const string ModuloEquals = "%=";
        public const string EqualsEquals = "==";
        public const string NotEquals = "!=";
        public const string LessThanEquals = "<=";
        public const string GreaterThanEquals = ">=";
        public const string Arrow = "->";
        #endregion
        
        #region 'pure' tokens
        public const char LeftParenthesis = '(';
        public const char RightParenthesis = ')';
        public const char LeftBrace = '{';
        public const char RightBrace = '}';
        public const char LeftBracket = '[';
        public const char RightBracket = ']';
        public const char LessThan = '<';
        public const char GreaterThan = '>';
        public const char Comma = ',';
        public const char Colon = ':';
        public const char Semicolon = ';';
        public const char EqualsAssign = '=';
        public const char Dot = '.';
        public const char Asterisk = '*';
        public const char Plus = '+';
        public const char Minus = '-';
        public const char Slash = '/';
        public const char Not = '!';
        public const char QuestionMark = '?';
        public const char At = '@';
        public const char Percent = '%';
        public const char Tilde = '~';
        #endregion

    }
}