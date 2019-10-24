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
        JMP,
        NLIST,
        AADD,
        AREM,
        AREMA,
        ALEN,
        ACLR,
        AINS,
        /// <summary>
        /// list indexer get
        /// </summary>
        AIDXG,
        /// <summary>
        /// list indexer set
        /// </summary>
        AIDXS,
        TMPV,
        TMPC,
        TMPR,
        JMPT,
        NSCP,
        PSCP,
        NEG,
        NOT,
        SLEN,
        SIDXG,
        SIDXS,
        SINS,
        SRPLA,
        SVIEW,
        SIDXO
    }
}