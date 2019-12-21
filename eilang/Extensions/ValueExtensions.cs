﻿using System;
using System.Collections.Generic;
using System.Linq;
using eilang.Classes;
using eilang.Exceptions;
using eilang.Interfaces;
using eilang.Interpreting;
using eilang.Values;

namespace eilang.Extensions
{
    public static class ValueExtensions
    {
        public static IValue ToValue(this object obj, IValueFactory factory = default)
        {
            if (factory == null)
            {
                factory = new ValueFactory();
            }
            
            var type = obj.GetType();
            switch (type)
            {
                case var _ when type == typeof(string):
                    return factory.String((string) obj);
                case var _ when type == typeof(int):
                    return factory.Integer((int) obj);
                case var _ when type == typeof(long):
                    return factory.Long((long) obj);
                case var _ when type == typeof(double):
                    return factory.Double((double) obj);
                case var _ when type == typeof(bool):
                    return factory.Bool((bool) obj);
                case var _ when type == typeof(IntPtr):
                    return factory.IntPtr((IntPtr) obj);
                case var _ when !type.IsAbstract && !type.IsPrimitive: // probably going to need more guards here
                    return ConvertToEilangInstance(type, obj, factory);
                default:
                    throw new NotImplementedException();
            }
        }

        private static IValue ConvertToEilangInstance(Type type, object obj, IValueFactory factory)
        {
            var members = type.GetMemberInfos();
            var name = type.Name;
            var clas = new Class(name, SpecialVariables.Global);
            var scope = new Scope();
            foreach (var member in members)
            {
                var value = member.GetValue(obj).ToValue(factory);
                scope.DefineVariable(member.Name, value);
            }
            return factory.Instance(new Instance(scope, clas));
        }

        public static T To<T>(this IValue value)
        {
            return (T) To(value, typeof(T));
        }
        
        public static object To(this IValue value, Type type)
        {
            switch (type)
            {
                case var _ when type == typeof(string):
                    return Convert.ChangeType(value.As<StringValue>().Item, type);
                case var _ when type == typeof(IntPtr):
                    return Convert.ChangeType(value.As<IntPtrValue>().Item, type);
                case var _ when type == typeof(int):
                    return Convert.ChangeType(value.As<IntegerValue>().Item, type);
                case var _ when type == typeof(long):
                    return Convert.ChangeType(value.As<LongValue>().Item, type);
                case var _ when type == typeof(double):
                    return Convert.ChangeType(value.As<DoubleValue>().Item, type);
                case var _ when type == typeof(bool):
                    return Convert.ChangeType(value.As<BoolValue>().Item, type);
                case var _ when !type.IsAbstract && !type.IsPrimitive: // probably going to need more guards here
                    var instance = value.As<InstanceValue>();
                    var variables = instance.Item.Scope.GetAllVariables().Where(v => v.Key != SpecialVariables.Me);
                    return ConvertToCSharpObject(type, variables);
                default:
                    throw new NotImplementedException();
                // TODO: implement converting from an eilang list to C# List<T>
            }
        }

        private static object ConvertToCSharpObject(Type type, IEnumerable<KeyValuePair<string, IValue>> variables)
        {
            var instance = Activator.CreateInstance(type);
            foreach (var (name, value) in variables)
            {
                var typeVariable = type.GetMemberInfo(name);
                typeVariable.SetValue(instance, value.To(typeVariable.GetActualType()));
            }

            return instance;
        }


        public static IValue Require(this IValue value, EilangType type, string message)
        {
            if (value.Type == type) 
                return value;
            throw new InvalidValueException(message);
        }
        
        public static T Require<T>(this IValue value, string message)
        {
            if (value is T ret) 
                return ret;
            throw new InvalidValueException(message);
        }
        
        public static IValue RequireAnyOf(this IValue value, EilangType typeFlags, string message)
        {
            if ((value.Type & typeFlags) != 0) 
                return value;
            throw new InvalidValueException(message);
        }
        
        public static void OrderAsArguments(this List<IValue> value)
        {
            value.Reverse();
        }
        
        public static ListValue RequireCount(this ListValue value, int count, string message)
        {
            if (value.Item.Count == count) 
                return value;
            throw new InvalidArgumentCountException(message);
        }
        
        public static ListValue RequireAtLeast(this ListValue value, int count, string message)
        {
            if (value.Item.Count >= count) 
                return value;
            throw new InvalidArgumentCountException(message);
        }

        public static string GetStringArgument(this IValue value, string errorMessage)
        {
            return value
                .Require(EilangType.String, errorMessage)
                .To<string>();
        }
        
    }
}