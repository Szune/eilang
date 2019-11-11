namespace eilang.Compiling
{
    public enum OpCode
    {
        None,
        /// <summary>
        /// Push value on stack
        /// </summary>
        PUSH,
        /// <summary>
        /// Define variable
        /// </summary>
        DEF,
        /// <summary>
        /// Pop value on stack
        /// </summary>
        POP,
        /// <summary>
        /// Set variable in local scope
        /// </summary>
        SET,
        /// <summary>
        /// Global function call
        /// </summary>
        CALL,
        /// <summary>
        /// Exported function call
        /// </summary>
        ECALL,
        /// <summary>
        /// Reference variable
        /// </summary>
        REF,
        /// <summary>
        /// Return
        /// </summary>
        RET,
        /// <summary>
        /// Member function call
        /// </summary>
        MCALL,
        /// <summary>
        /// Initialize instance
        /// </summary>
        INIT,
        /// <summary>
        /// Reference member variable
        /// </summary>
        MREF,
        /// <summary>
        /// Get class of current instance on stack
        /// </summary>
        TYPEGET,
        /// <summary>
        /// Set member variable
        /// </summary>
        MSET,
        /// <summary>
        /// Add
        /// </summary>
        ADD,
        /// <summary>
        /// Subtract
        /// </summary>
        SUB,
        /// <summary>
        /// Multiply
        /// </summary>
        MUL,
        /// <summary>
        /// Divide
        /// </summary>
        DIV,
        /// <summary>
        /// Jump if false
        /// </summary>
        JMPF,
        /// <summary>
        /// Or
        /// </summary>
        OR,
        /// <summary>
        /// And
        /// </summary>
        AND,
        /// <summary>
        /// Equals
        /// </summary>
        EQ,
        /// <summary>
        /// Not equals
        /// </summary>
        NEQ,
        /// <summary>
        /// Less than (&lt;)
        /// </summary>
        LT,
        /// <summary>
        /// Greater than (&gt;)
        /// </summary>
        GT,
        /// <summary>
        /// Less than equals (&lt;=)
        /// </summary>
        LTE,
        /// <summary>
        /// Greater than equals (&gt;=)
        /// </summary>
        GTE,
        /// <summary>
        /// Jump
        /// </summary>
        JMP,
        /// <summary>
        /// Create new list
        /// </summary>
        NLIST,
        /// <summary>
        /// Array add
        /// </summary>
        AADD,
        /// <summary>
        /// Array remove
        /// </summary>
        AREM,
        /// <summary>
        /// Array remove at
        /// </summary>
        AREMA,
        /// <summary>
        /// Array length
        /// </summary>
        ALEN,
        /// <summary>
        /// Array clear
        /// </summary>
        ACLR,
        /// <summary>
        /// Array insert
        /// </summary>
        AINS,
        /// <summary>
        /// Array indexer get
        /// </summary>
        AIDXG,
        /// <summary>
        /// Array indexer set
        /// </summary>
        AIDXS,
        /// <summary>
        /// Set temporary variable
        /// </summary>
        TMPV,
        /// <summary>
        /// Reference temporary variable 
        /// </summary>
        TMPR,
        /// <summary>
        /// Jump if true
        /// </summary>
        JMPZ,
        /// <summary>
        /// New scope
        /// </summary>
        JMPT,
        /// <summary>
        /// New scope
        /// </summary>
        NSCP,
        /// <summary>
        /// Pop scope
        /// </summary>
        PSCP,
        /// <summary>
        /// Negate (e.g. turning 5 to -5)
        /// </summary>
        NEG,
        /// <summary>
        /// Not-ing (!)
        /// </summary>
        NOT,
        /// <summary>
        /// String length
        /// </summary>
        SLEN,
        /// <summary>
        /// String indexer get
        /// </summary>
        SIDXG,
        /// <summary>
        /// String indexer set
        /// </summary>
        SIDXS,
        /// <summary>
        /// String insert
        /// </summary>
        SINS,
        /// <summary>
        /// String replace
        /// </summary>
        SRPLA,
        /// <summary>
        /// String view
        /// </summary>
        SVIEW,
        /// <summary>
        /// String index of
        /// </summary>
        SIDXO,
        /// <summary>
        /// String to lower
        /// </summary>
        SLOW,
        /// <summary>
        /// String to upper
        /// </summary>
        SUP,
        /// <summary>
        /// String split
        /// </summary>
        SPLIT,
        /// <summary>
        /// Array skip
        /// </summary>
        ASKIP,
        /// <summary>
        /// Increment unary operator
        /// </summary>
        INC,
        /// <summary>
        /// Decrement unary operator
        /// </summary>
        DEC,
        /// <summary>
        /// Modulo
        /// </summary>
        MOD,
        /// <summary>
        /// Start process
        /// </summary>
        SPROC,
        /// <summary>
        /// Kill process by process id
        /// </summary>
        KPPROC,
        /// <summary>
        /// Kill process by name
        /// </summary>
        KPROC,
        /// <summary>
        /// Get process by name
        /// </summary>
        GPROC,
        /// <summary>
        /// Get process by process id
        /// </summary>
        GPPROC
    }
}