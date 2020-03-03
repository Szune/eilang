using System;
using System.Collections.Generic;
using eilang.ArgumentBuilders;
using eilang.Exceptions;
using eilang.Extensions;
using eilang.Interfaces;
using eilang.Values;
using Xunit;

namespace eilang.Tests
{
    public class ArgumentBuilderTests
    {
        private static IValueFactory _fac = new ValueFactory();
        public class ArgumentsTests
        {
            [Fact]
            public void VoidArgumentThrowsExceptionIfArgumentWasSupplied()
            {
                var sut = Arguments.Create(_fac.Integer(1), "test");
                void Act() => sut.Void();
                Assert.Throws<InvalidArgumentCountException>(Act);
            }
            
            [Fact]
            public void VoidArgumentDoesNotThrowIfNoArgumentWasSupplied()
            {
                var sut = Arguments.Create(_fac.Void(), "test");
                void Act() => sut.Void();
                var exception = Record.Exception(Act);
                Assert.Null(exception);
            }

            [Fact]
            public void SingleThrowsIfWrongType()
            {
                var sut = Arguments.Create(_fac.String("hallo wurld"), "test");
                void Act() => sut.Single<int>(EilangType.Integer, "test");
                Assert.Throws<InvalidValueException>(Act);
            }
            
            [Fact]
            public void SingleReturnsValueConvertedToCSharpIfCorrectType()
            {
                var sut = Arguments.Create(_fac.IntPtr(new IntPtr(0xFFFFFF)), "test");
                var single = sut.Single<IntPtr>(EilangType.IntPtr, "test");
                Assert.Equal(new IntPtr(0xFFFFFF), single);
            }
            
            [Fact]
            public void SingleReturnsValueConvertedToCSharpIfConstraintIsAnyType()
            {
                var sut = Arguments.Create(_fac.Double(39.51d), "test");
                var single = sut.Single<double>(EilangType.Any, "test");
                Assert.Equal(39.51d, single, 2);
            }

            [Fact]
            public void SingleThrowsIfGenericTypeIsWrongButEilangTypeIsCorrect()
            {
                var sut = Arguments.Create(_fac.Long(1050L), "test");
                void Act() => sut.Single<string>(EilangType.Long, "test");
                Assert.Throws<InvalidArgumentTypeException>(Act);
            }

            [Fact]
            public void EilangValueWithTypeFlagsAllowsAnyOfTheTypes()
            {
                var sut = Arguments.Create(_fac.Bool(true), "test");
                var value = sut.EilangValue(EilangType.Double | EilangType.Long | EilangType.Bool | EilangType.String, "test");
                Assert.True(value.To<bool>());
            }
            
            [Fact]
            public void EilangValueWithTypeFlagsThrowsIfTypeOfValueIsNotInFlags()
            {
                var sut = Arguments.Create(_fac.Bool(true), "test");
                void Act() => sut.EilangValue(EilangType.Double | EilangType.Long | EilangType.String, "test");
                Assert.Throws<InvalidValueException>(Act);
            }

            [Fact]
            public void ListWithArgumentConstraints()
            {
                const string devNull = "/dev/null";
                const string devRandom = "/dev/random";
                var args = GetArgumentList(_fac.String(devNull), _fac.String(devRandom), _fac.Bool(true));
                var sut = Arguments.Create(_fac.List(args), "test");
                var argList = sut.List().With
                    .Argument(EilangType.String, "currentLocation")
                    .Argument(EilangType.String, "newLocation")
                    .OptionalArgument(EilangType.Bool, "overwrite", false)
                    .Build();
                var currentLocation = argList.Get<string>(0);
                var newLocation = argList.Get<string>(1);
                var overwrite = argList.Get<bool>(2);
                Assert.Equal(devNull, currentLocation);
                Assert.Equal(devRandom, newLocation);
                Assert.True(overwrite);
            }
            
            [Fact]
            public void ListWithArgumentConstraintsWhereOneIsConstrainedToAnyType()
            {
                var args = GetArgumentList(_fac.Integer(13), _fac.Long(50L), _fac.False());
                var sut = Arguments.Create(_fac.List(args), "test");
                var argList = sut.List().With
                    .Argument(EilangType.Integer, "val1")
                    .Argument(EilangType.Any, "val2")
                    .OptionalArgument(EilangType.Bool, "val3", true)
                    .Build();
                var val1 = argList.Get<int>(0);
                var val2 = argList.Get<long>(1);
                var val3 = argList.Get<bool>(2);
                Assert.Equal(13, val1);
                Assert.Equal(50L, val2);
                Assert.False(val3);
            }
            
            [Fact]
            public void ListWithArgumentConstraintsThrowsIfArgumentTypeIsWrong()
            {
                var args = GetArgumentList(_fac.Integer(100), _fac.String("random"));
                var sut = Arguments.Create(_fac.List(args), "test");
                void Act() => sut.List().With
                    .Argument(EilangType.String, "old")
                    .Argument(EilangType.String, "new")
                    .Build();

                Assert.Throws<ArgumentValidationFailedException>(Act);
            }
            
            [Fact]
            public void ListWithEilangTypeArgumentConstraint()
            {
                var testType = _fac.Type("TestType").As<TypeValue>();
                var args = GetArgumentList(_fac.String("hi world"), testType);
                var sut = Arguments.Create(_fac.List(args), "test");
                var argList = sut.List().With
                    .Argument(EilangType.String, "old")
                    .Argument(EilangType.Type, "new")
                    .Build();

                var type = argList.Get<TypeValue>(1);
                Assert.Equal(testType.Value, type.Value);
            }
            
            [Fact]
            public void ListWithArgumentConstraintsThrowsIfSuppliedRequiredArgumentCountIsTooLow()
            {
                var args = GetArgumentList(_fac.Integer(100), _fac.String("random"));
                var sut = Arguments.Create(_fac.List(args), "test");
                void Act() => sut.List().With
                    .Argument(EilangType.String, "old")
                    .Argument(EilangType.String, "new")
                    .Argument(EilangType.Any, "oh_no")
                    .Build();

                Assert.Throws<ArgumentMismatchException>(Act);
            }
            
            [Fact]
            public void ListWithArgumentConstraintsThrowsIfSuppliedRequiredArgumentCountIsTooHigh()
            {
                var args = GetArgumentList(_fac.Integer(100), _fac.String("random"), _fac.String("bye world"));
                var sut = Arguments.Create(_fac.List(args), "test");
                void Act() => sut.List().With
                    .Argument(EilangType.Integer, "old")
                    .Argument(EilangType.String, "new")
                    .Build();

                Assert.Throws<ArgumentMismatchException>(Act);
            }
            
            [Fact]
            public void ListWithArgumentConstraintsDoesNotThrowsIfOptionalArgumentsAreNotSupplied()
            {
                var args = GetArgumentList(_fac.String("testo"), _fac.String("presto"), _fac.Integer(-1));
                var sut = Arguments.Create(_fac.List(args), "test");
                var argList = sut.List().With
                    .Argument(EilangType.String, "old")
                    .OptionalArgument(EilangType.String, "new", "")
                    .OptionalArgument(EilangType.Integer, "oh_no", 100)
                    .OptionalArgument(EilangType.Long, "oh_no_ok", 150L)
                    .Build();
                var values = new List<object>
                {
                    argList.Get<string>(0),
                    argList.Get<string>(1),
                    argList.Get<int>(2),
                    argList.Get<long>(3)
                };
                Assert.Equal(new List<object>{ "testo", "presto", -1, 150L}, values);
            }
            
            [Fact]
            public void ListWithParamsArgument()
            {
                var args = GetArgumentList(_fac.String("testo"), _fac.String("nothing"), _fac.String("presto"), _fac.Integer(-1376));
                var sut = Arguments.Create(_fac.List(args), "test");
                var argList = sut.List().With
                    .Argument(EilangType.String, "old")
                    .Argument(EilangType.String, "new")
                    .Params()
                    .Build();
                var values = new List<object>
                {
                    argList.Get<string>(0),
                    argList.Get<string>(1),
                };
                var paramList = argList.Get<List<IValue>>(2);
                values.Add(paramList[0].To<string>());
                values.Add(paramList[1].To<int>());
                Assert.Equal(new List<object>{ "testo", "nothing", "presto", -1376}, values);
            }
            
            [Fact]
            public void ListWithNonSuppliedParamsArgument()
            {
                var args = GetArgumentList(_fac.String("testo"), _fac.String("nothing"));
                var sut = Arguments.Create(_fac.List(args), "test");
                var argList = sut.List().With
                    .Argument(EilangType.String, "old")
                    .Argument(EilangType.String, "new")
                    .Params()
                    .Build();
                var values = new List<object>
                {
                    argList.Get<string>(0),
                    argList.Get<string>(1),
                };
                Assert.Equal(new List<object>{ "testo", "nothing"}, values);
            }
            
            [Fact]
            public void ListWithParamsArgumentAndTooFewSuppliedRequiredArguments()
            {
                var args = GetArgumentList(_fac.String("testo"));
                var sut = Arguments.Create(_fac.List(args), "test");
                void Act() => sut.List().With
                    .Argument(EilangType.String, "old")
                    .Argument(EilangType.String, "new")
                    .Params()
                    .Build();

                Assert.Throws<ArgumentMismatchException>(Act);
            }
            
            [Fact]
            public void ListWithParamsArgumentThrowsOnTypeConstraintFailure()
            {
                var args = GetArgumentList(_fac.String("testo"), _fac.String("nothing"), _fac.String("presto"), _fac.Integer(-1376));
                var sut = Arguments.Create(_fac.List(args), "test");
                void Act() => sut.List().With
                    .Argument(EilangType.Any, "old")
                    .Argument(EilangType.Long, "new")
                    .Params()
                    .Build();

                Assert.Throws<ArgumentValidationFailedException>(Act);
            }
        }

        /// <summary>
        /// Returns a list of the arguments in the order they would be in if they were coming from the interpreter.
        /// </summary>
        private static List<IValue> GetArgumentList(params IValue[] values)
        {
            var list = new List<IValue>(values);
            list.Reverse();
            return list;
        }
    }
}