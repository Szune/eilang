namespace eilang
{
    public enum OpCode
    {
        None = 0,
        PUSH = 1,
        DEF = 2,
        POP = 3,
        SET = 4,
        CALL = 5,
        /// <summary>
        /// Exported function call
        /// </summary>
        ECALL = 6,
        REF = 7,
        RET = 8,
        MCALL = 9,
        INIT = 10,
        MREF = 11,
        TYPEGET = 12,
    }
}
