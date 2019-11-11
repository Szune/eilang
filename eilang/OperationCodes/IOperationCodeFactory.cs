﻿using eilang.Interfaces;

namespace eilang.OperationCodes
{
    public interface IOperationCodeFactory
    {
        Push Push(IValue value);
        Define Define(IValue name);
        Set Set(IValue name);
        Equals Equals();
        NotEquals NotEquals();
        GreaterThan GreaterThan();
        GreaterThanOrEquals GreaterThanOrEquals();
        LessThan LessThan();
        LessThanOrEquals LessThanOrEquals();
        Add Add();
        Subtract Subtract();
        Multiply Multiply();
        Modulo Modulo();
        Division Divide();
        JumpIfFalse JumpIfFalse(IValue address);
        JumpIfTrue JumpIfTrue(IValue address);
        JumpIfZero JumpIfZero(IValue address);
        Jump Jump(IValue address);
        Call Call(IValue functionName);
        Reference Reference(IValue variableName);
        Pop Pop();
        Return Return();
        Return Return(int loopDepth);
        Initialize Initialize(IValue className);
        ExportedCall ExportedCall(IValue functionName);
        TypeGet TypeGet();
        MemberCall MemberCall(IValue functionName);
        MemberReference MemberReference(IValue memberName);
        MemberSet MemberSet(IValue memberName);
        And And();
        Or Or();
        ListNew ListNew();
        ListAdd ListAdd();
        ListLength ListLength();
        ListRemove ListRemove();
        ListRemoveAt ListRemoveAt();
        ListIndexerGet ListIndexerGet();
        ListIndexerSet ListIndexerSet();
        ListInsert ListInsert();
        ListClear ListClear();
        ListSkip ListSkip();
        TemporarySet TemporarySet(IValue variableName);
        TemporaryReference TemporaryReference(IValue variableName);
        ScopeNew ScopeNew();
        ScopePop ScopePop();
        Negate Negate();
        Increment Increment();
        Decrement Decrement();
        Not Not();
        StringLength StringLength();
        StringIndexerGet StringIndexerGet();
        StringIndexerSet StringIndexerSet();
        StringView StringView();
        StringIndexOf StringIndexOf();
        StringInsert StringInsert();
        StringReplace StringReplace();
        StringToUpper StringToUpper();
        StringToLower StringToLower();
        StringSplit StringSplit();
    }
}