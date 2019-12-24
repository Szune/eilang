using eilang.Compiling;
using eilang.Interfaces;
using eilang.Parsing;

namespace eilang.OperationCodes
{
    public class OperationCodeFactory : IOperationCodeFactory
    {
        private readonly Equals _equals = new Equals();
        private readonly NotEquals _notEquals = new NotEquals();
        private readonly GreaterThan _greaterThan = new GreaterThan();
        private readonly StringSplit _stringSplit = new StringSplit();
        private readonly GreaterThanOrEquals _greaterThanOrEquals = new GreaterThanOrEquals();
        private readonly LessThan _lessThan = new LessThan();
        private readonly LessThanOrEquals _lessThanOrEquals = new LessThanOrEquals();
        private readonly Add _add = new Add();
        private readonly Subtract _subtract = new Subtract();
        private readonly Multiply _multiply = new Multiply();
        private readonly Modulo _modulo = new Modulo();
        private readonly Division _division = new Division();
        private readonly Pop _pop = new Pop();
        private readonly Return _return = new Return();
        private readonly TypeGet _typeGet = new TypeGet();
        private readonly And _and = new And();
        private readonly Or _or = new Or();
        private readonly ListNew _listNew = new ListNew();
        private readonly ListAdd _listAdd = new ListAdd();
        private readonly ListLength _listLength = new ListLength();
        private readonly ListRemove _listRemove = new ListRemove();
        private readonly ListRemoveAt _listRemoveAt = new ListRemoveAt();
        private readonly ListIndexerGet _listIndexerGet = new ListIndexerGet();
        private readonly ListIndexerSet _listIndexerSet = new ListIndexerSet();
        private readonly ListInsert _listInsert = new ListInsert();
        private readonly ListClear _listClear = new ListClear();
        private readonly ListSkip _listSkip = new ListSkip();
        private readonly ScopeNew _scopeNew = new ScopeNew();
        private readonly ScopePop _scopePop = new ScopePop();
        private readonly Negate _negate = new Negate();
        private readonly Increment _increment = new Increment();
        private readonly Decrement _decrement = new Decrement();
        private readonly Not _not = new Not();
        private readonly StringLength _stringLength = new StringLength();
        private readonly StringIndexerGet _stringIndexerGet = new StringIndexerGet();
        private readonly StringIndexerSet _stringIndexerSet = new StringIndexerSet();
        private readonly StringView _stringView = new StringView();
        private readonly StringIndexOf _stringIndexOf = new StringIndexOf();
        private readonly StringInsert _stringInsert = new StringInsert();
        private readonly StringReplace _stringReplace = new StringReplace();
        private readonly StringToUpper _stringToUpper = new StringToUpper();
        private readonly StringToLower _stringToLower = new StringToLower();
        private readonly StringToInt _stringToInt = new StringToInt();
        private readonly StringToDouble _stringToDouble = new StringToDouble();
        private readonly StringToBool _stringToBool = new StringToBool();
        private readonly Dispose _dispose = new Dispose();
        private readonly FileWrite _fileWriteLine = new FileWrite(true);
        private readonly FileWrite _fileWrite = new FileWrite(false);
        private readonly FileRead _fileReadEntire = new FileRead(true);
        private readonly FileRead _fileReadChar = new FileRead(false);
        private readonly FileEof _fileEof = new FileEof();
        private readonly FileClear _fileClear = new FileClear();
        private readonly MapNew _mapNew = new MapNew();
        private readonly MapLength _mapLength = new MapLength();
        private readonly MapGetValues _mapGetValues = new MapGetValues();
        private readonly MapGetKeys _mapGetKeys = new MapGetKeys();
        private readonly MapGetItems _mapGetItems = new MapGetItems();
        private readonly MapAdd _mapAdd = new MapAdd();
        private readonly MapClear _mapClear = new MapClear();
        private readonly MapRemove _mapRemove = new MapRemove();
        private readonly MapIndexerGet _mapIndexerGet = new MapIndexerGet();
        private readonly MapIndexerSet _mapIndexerSet = new MapIndexerSet();
        private readonly MapContains _mapContains = new MapContains();
        private readonly StringToLong _stringToLong = new StringToLong();

        public Push Push(IValue value)
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

        public Set Set(IValue name)
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

        public JumpIfFalse JumpIfFalse(IValue address)
        {
            return new JumpIfFalse(address);
        }

        public JumpIfTrue JumpIfTrue(IValue address)
        {
            return new JumpIfTrue(address);
        }

        public JumpIfZero JumpIfZero(IValue address)
        {
            return new JumpIfZero(address);
        }

        public Jump Jump(IValue address)
        {
            return new Jump(address);
        }

        public Call Call(IValue functionName)
        {
            return new Call(functionName);
        }

        public Reference Reference(IValue variableName)
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

        public Initialize Initialize(IValue className)
        {
            return new Initialize(className);
        }

        public CallExported CallExported(IValue functionName)
        {
            return new CallExported(functionName);
        }

        public TypeGet TypeGet()
        {
            return _typeGet;
        }

        public CallMember CallMember(IValue functionName)
        {
            return new CallMember(functionName);
        }

        public MemberReference MemberReference(IValue memberName)
        {
            return new MemberReference(memberName);
        }

        public MemberSet MemberSet(IValue memberName)
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

        public TemporarySet TemporarySet(IValue variableName)
        {
            return new TemporarySet(variableName);
        }

        public TemporaryReference TemporaryReference(IValue variableName)
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
}