﻿namespace eilang.Tokens
{
    public enum TokenType
    {
        None = 0,
        EOF,
        Identifier,
        LeftParenthesis,
        RightParenthesis,
        LeftBrace,
        RightBrace,
        LeftBracket,
        RightBracket,
        String,
        Integer,
        Double,
        If,
        Else,
        Or,
        And,
        EqualsEquals,
        NotEquals,
        LessThan,
        GreaterThan,
        LessThanEquals,
        GreaterThanEquals,
        Class,
        Struct,
        Module,
        Function,
        Var,
        Comma,
        Colon,
        DoubleColon,
        Semicolon,
        Equals,
        Dot,
        DoubleDot,
        Asterisk,
        Constructor,
        Plus,
        PlusEquals,
        MinusEquals,
        TimesEquals,
        DivideEquals,
        For,
        True,
        False,
        Minus,
        Slash,
        Not,
        It,
        Ix,
        Return,
        /// <summary>
        /// Current object access
        /// </summary>
        Me,
        Continue,
        Break,
        PlusPlus,
        MinusMinus,
        QuestionMark,
        At,
        Percent,
        ModuloEquals,
        Import,
        Tilde,
        Use,
        While,
        Arrow,
        Pipe,
        Switch,
        TypeOf,
        Long,
        ElseIf,
        LambdaArrow
    }
}