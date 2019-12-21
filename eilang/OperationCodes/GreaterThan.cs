using eilang.Interpreting;
using eilang.Values;

namespace eilang.OperationCodes
{
    public class GreaterThan : IOperationCode
    {
        public void Execute(State state)
        {
            var right = state.Stack.Pop();
            var left = state.Stack.Pop();
            switch (left.Type)
            {
                case EilangType.Integer:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(left.Get<int>() > right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Long:
                            state.Stack.Push(left.Get<int>() > right.Get<long>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Double:
                            state.Stack.Push(left.Get<int>() > right.Get<double>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                    }

                    break;
                case EilangType.Long:
                    switch (right.Type)
                    {
                        case EilangType.Integer:
                            state.Stack.Push(left.Get<long>() > right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Long:
                            state.Stack.Push(left.Get<long>() > right.Get<long>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Double:
                            state.Stack.Push(left.Get<long>() > right.Get<double>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                    }

                    break;
                case EilangType.Double:
                    switch (right.Type)
                    {
                        case EilangType.Double:
                            state.Stack.Push(left.Get<double>() > right.Get<double>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Integer:
                            state.Stack.Push(left.Get<double>() > right.Get<int>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                        case EilangType.Long:
                            state.Stack.Push(left.Get<double>() > right.Get<long>()
                                ? state.ValueFactory.True()
                                : state.ValueFactory.False());
                            break;
                    }

                    break;
            }
        }
    }
}