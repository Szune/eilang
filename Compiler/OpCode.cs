namespace eilang
{
    public enum OpCode
    {
        None,
        PUSH,
        DEF,
        POP,
        SET,
        CALL,

        /// <summary>
        /// Exported function call
        /// </summary>
        ECALL,
        REF,
        RET,
        MCALL,
        INIT,
        MREF,
        TYPEGET,
        MSET,
        ADD,
        SUB,
        MUL,
        DIV,
        JMPF,
        OR,
        AND,
        EQ,
        NEQ,
        LT,
        GT,
        LTE,
        GTE,
        JMP
    }
}