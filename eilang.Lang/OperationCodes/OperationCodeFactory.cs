using eilang.Compiling;
using eilang.Parsing;
using eilang.Values;

namespace eilang.OperationCodes;

public class OperationCodeFactory : IOperationCodeFactory
{
    private readonly Equals _equals = new();
    private readonly NotEquals _notEquals = new();
    private readonly GreaterThan _greaterThan = new();
    private readonly StringSplit _stringSplit = new();
    private readonly GreaterThanOrEquals _greaterThanOrEquals = new();
    private readonly LessThan _lessThan = new();
    private readonly LessThanOrEquals _lessThanOrEquals = new();
    private readonly Add _add = new();
    private readonly Subtract _subtract = new();
    private readonly Multiply _multiply = new();
    private readonly Modulo _modulo = new();
    private readonly Division _division = new();
    private readonly Pop _pop = new();
    private readonly Return _return = new();
    private readonly TypeGet _typeGet = new();
    private readonly And _and = new();
    private readonly Or _or = new();
    private readonly ListNew _listNew = new();
    private readonly ListAdd _listAdd = new();
    private readonly ListLength _listLength = new();
    private readonly ListRemove _listRemove = new();
    private readonly ListRemoveAt _listRemoveAt = new();
    private readonly ListIndexerGet _listIndexerGet = new();
    private readonly ListIndexerSet _listIndexerSet = new();
    private readonly ListInsert _listInsert = new();
    private readonly ListClear _listClear = new();
    private readonly ListSkip _listSkip = new();
    private readonly ScopeNew _scopeNew = new();
    private readonly ScopePop _scopePop = new();
    private readonly Negate _negate = new();
    private readonly Increment _increment = new();
    private readonly Decrement _decrement = new();
    private readonly Not _not = new();
    private readonly StringLength _stringLength = new();
    private readonly StringIndexerGet _stringIndexerGet = new();
    private readonly StringIndexerSet _stringIndexerSet = new();
    private readonly StringView _stringView = new();
    private readonly StringIndexOf _stringIndexOf = new();
    private readonly StringInsert _stringInsert = new();
    private readonly StringReplace _stringReplace = new();
    private readonly StringToUpper _stringToUpper = new();
    private readonly StringToLower _stringToLower = new();
    private readonly StringToInt _stringToInt = new();
    private readonly StringToDouble _stringToDouble = new();
    private readonly StringToBool _stringToBool = new();
    private readonly Dispose _dispose = new();
    private readonly FileWrite _fileWriteLine = new(true);
    private readonly FileWrite _fileWrite = new(false);
    private readonly FileRead _fileReadEntire = new(true);
    private readonly FileRead _fileReadChar = new(false);
    private readonly FileEof _fileEof = new();
    private readonly FileClear _fileClear = new();
    private readonly MapNew _mapNew = new();
    private readonly MapLength _mapLength = new();
    private readonly MapGetValues _mapGetValues = new();
    private readonly MapGetKeys _mapGetKeys = new();
    private readonly MapGetItems _mapGetItems = new();
    private readonly MapAdd _mapAdd = new();
    private readonly MapClear _mapClear = new();
    private readonly MapRemove _mapRemove = new();
    private readonly MapIndexerGet _mapIndexerGet = new();
    private readonly MapIndexerSet _mapIndexerSet = new();
    private readonly MapContains _mapContains = new();
    private readonly StringToLong _stringToLong = new();

    public Push Push(ValueBase value)
    {
        return new Push(value);
    }

    public Define Define(string name)
    {
        return new Define(name);
    }

    public DefineAndEnsureType DefineAndEnsureType(Parameter parameter, Function function)
    {
        return new DefineAndEnsureType(parameter, function);
    }

    public Set Set(ValueBase name)
    {
        return new Set(name);
    }

    public Equals Equals()
    {
        return _equals;
    }

    public NotEquals NotEquals()
    {
        return _notEquals;
    }

    public GreaterThan GreaterThan()
    {
        return _greaterThan;
    }

    public GreaterThanOrEquals GreaterThanOrEquals()
    {
        return _greaterThanOrEquals;
    }

    public LessThan LessThan()
    {
        return _lessThan;
    }

    public LessThanOrEquals LessThanOrEquals()
    {
        return _lessThanOrEquals;
    }

    public Add Add()
    {
        return _add;
    }

    public Subtract Subtract()
    {
        return _subtract;
    }

    public Multiply Multiply()
    {
        return _multiply;
    }

    public Modulo Modulo()
    {
        return _modulo;
    }

    public Division Divide()
    {
        return _division;
    }

    public JumpIfFalse JumpIfFalse(ValueBase address)
    {
        return new JumpIfFalse(address);
    }

    public JumpIfTrue JumpIfTrue(ValueBase address)
    {
        return new JumpIfTrue(address);
    }

    public JumpIfZero JumpIfZero(ValueBase address)
    {
        return new JumpIfZero(address);
    }

    public Jump Jump(ValueBase address)
    {
        return new Jump(address);
    }

    public Call Call(ValueBase functionName)
    {
        return new Call(functionName);
    }

    public Reference Reference(string variableName)
    {
        return new Reference(variableName);
    }

    public Pop Pop()
    {
        return _pop;
    }

    public Return Return()
    {
        return _return;
    }

    public Return Return(int loopDepth)
    {
        return new Return(loopDepth);
    }

    public Initialize Initialize(ValueBase className)
    {
        return new Initialize(className);
    }

    public CallExported CallExported(ValueBase functionName)
    {
        return new CallExported(functionName);
    }

    public TypeGet TypeGet()
    {
        return _typeGet;
    }

    public IOperationCode CallMember(string functionName, int argumentCount)
    {
        return argumentCount switch
        {
            0 => new CallMemberWithoutArguments(functionName),
            1 => new CallMemberWithArgumentCount1(functionName),
            2 => new CallMemberWithArgumentCount2(functionName),
            3 => new CallMemberWithArgumentCount3(functionName),
            _ => new CallMember(functionName, argumentCount)
        };
    }

    public MemberReference MemberReference(string memberName)
    {
        return new MemberReference(memberName);
    }

    public MemberSet MemberSet(ValueBase memberName)
    {
        return new MemberSet(memberName);
    }

    public And And()
    {
        return _and;
    }

    public Or Or()
    {
        return _or;
    }

    public ListNew ListNew()
    {
        return _listNew;
    }

    public ListAdd ListAdd()
    {
        return _listAdd;
    }

    public ListLength ListLength()
    {
        return _listLength;
    }

    public ListRemove ListRemove()
    {
        return _listRemove;
    }

    public ListRemoveAt ListRemoveAt()
    {
        return _listRemoveAt;
    }

    public ListIndexerGet ListIndexerGet()
    {
        return _listIndexerGet;
    }

    public ListIndexerSet ListIndexerSet()
    {
        return _listIndexerSet;
    }

    public ListInsert ListInsert()
    {
        return _listInsert;
    }

    public ListClear ListClear()
    {
        return _listClear;
    }

    public ListSkip ListSkip()
    {
        return _listSkip;
    }

    public TemporarySet TemporarySet(ValueBase variableName)
    {
        return new TemporarySet(variableName);
    }

    public TemporaryReference TemporaryReference(ValueBase variableName)
    {
        return new TemporaryReference(variableName);
    }

    public ScopeNew ScopeNew()
    {
        return _scopeNew;
    }

    public ScopePop ScopePop()
    {
        return _scopePop;
    }

    public Negate Negate()
    {
        return _negate;
    }

    public Increment Increment()
    {
        return _increment;
    }

    public Decrement Decrement()
    {
        return _decrement;
    }

    public Not Not()
    {
        return _not;
    }

    public StringLength StringLength()
    {
        return _stringLength;
    }

    public StringIndexerGet StringIndexerGet()
    {
        return _stringIndexerGet;
    }

    public StringIndexerSet StringIndexerSet()
    {
        return _stringIndexerSet;
    }

    public StringView StringView()
    {
        return _stringView;
    }

    public StringIndexOf StringIndexOf()
    {
        return _stringIndexOf;
    }

    public StringInsert StringInsert()
    {
        return _stringInsert;
    }

    public StringReplace StringReplace()
    {
        return _stringReplace;
    }

    public StringToUpper StringToUpper()
    {
        return _stringToUpper;
    }

    public StringToLower StringToLower()
    {
        return _stringToLower;
    }

    public StringSplit StringSplit()
    {
        return _stringSplit;
    }

    public StringToInt StringToInt()
    {
        return _stringToInt;
    }

    public StringToLong StringToLong()
    {
        return _stringToLong;
    }

    public StringToDouble StringToDouble()
    {
        return _stringToDouble;
    }

    public StringToBool StringToBool()
    {
        return _stringToBool;
    }

    public Dispose Dispose()
    {
        return _dispose;
    }

    public FileWrite FileWrite(bool appendLine = false)
    {
        if (appendLine)
        {
            return _fileWriteLine;
        }

        return _fileWrite;
    }

    public FileRead FileRead(bool entireLine = false)
    {
        if (entireLine)
        {
            return _fileReadEntire;
        }
        return _fileReadChar;
    }

    public FileEof FileEOF()
    {
        return _fileEof;
    }

    public FileClear FileClear()
    {
        return _fileClear;
    }

    public MapNew MapNew()
    {
        return _mapNew;
    }

    public MapLength MapLength()
    {
        return _mapLength;
    }

    public MapGetItems MapGetItems()
    {
        return _mapGetItems;
    }

    public MapGetKeys MapGetKeys()
    {
        return _mapGetKeys;
    }

    public MapGetValues MapGetValues()
    {
        return _mapGetValues;
    }

    public MapAdd MapAdd()
    {
        return _mapAdd;
    }

    public MapClear MapClear()
    {
        return _mapClear;
    }

    public MapRemove MapRemove()
    {
        return _mapRemove;
    }

    public MapIndexerGet MapIndexerGet()
    {
        return _mapIndexerGet;
    }

    public MapIndexerSet MapIndexerSet()
    {
        return _mapIndexerSet;
    }

    public MapContains MapContains()
    {
        return _mapContains;
    }

    public InitializeStruct InitializeStruct(string structName)
    {
        return new InitializeStruct(structName);
    }
}
