// Copyright © 2013-2017 - GuQiang
// Licensed under the LGPL-3.0 license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.Diagnostics;
#if (NET_35_UP || NETSTANDARD)
using System.Linq.Expressions;
#else 
using XPatchLib.NoLinq;
#endif
using System.Reflection;
using System.Reflection.Emit;

namespace XPatchLib
{
    internal static class ClrHelper
    {
        private static readonly Type[] GetValueParameterTypes = {typeof(object)};

        private static readonly Type[] SetValueParameterTypes = {typeof(object), typeof(object)};

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Reflection.Emit.DynamicMethod.#.ctor(System.String,System.Type,System.Type[],System.Boolean)")]
        public static Func<T> CreateInstanceFunc<T>(Type pType)
        {
            Guard.ArgumentNotNull(pType, "type");
            ConstructorInfo emptyConstructor = pType.GetConstructor(Type.EmptyTypes);
            if (emptyConstructor == null)
                return null;
#if NET
            var dynamicMethod = new DynamicMethod("CreateInstance", pType, Type.EmptyTypes, true);
            ILGenerator ilGenerator = dynamicMethod.GetILGenerator();
            ilGenerator.Emit(OpCodes.Nop);
            ilGenerator.Emit(OpCodes.Newobj, emptyConstructor);
            ilGenerator.Emit(OpCodes.Ret);
            return (Func<T>) dynamicMethod.CreateDelegate(typeof(Func<T>));
#elif NETSTANDARD
            
            if (pType.IsAbstract())
            {
                return () => (T)Activator.CreateInstance(pType);
            }

            try
            {
                Type resultType = typeof(T);

                Expression expression = Expression.New(emptyConstructor);

                expression = EnsureCastExpression(expression, resultType);

                LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<T>), expression);

                Func<T> compiled = (Func<T>)lambdaExpression.Compile();
                return compiled;
            }
            catch
            {
                // an error can be thrown if constructor is not valid on Win8
                // will have INVOCATION_FLAGS_NON_W8P_FX_API invocation flag
                return () => (T)Activator.CreateInstance(pType);
            }
#endif
        }

#if NETSTANDARD

        private static Expression EnsureCastExpression(Expression expression, Type targetType, bool allowWidening = false)
        {
            Type expressionType = expression.Type;

            // check if a cast or conversion is required
            if (expressionType == targetType || (!expressionType.IsValueType() && targetType.IsAssignableFrom(expressionType)))
            {
                return expression;
            }

            if (targetType.IsValueType())
            {
                Expression convert = Expression.Unbox(expression, targetType);

                if (allowWidening && targetType.IsPrimitive())
                {
                    MethodInfo toTargetTypeMethod = typeof(Convert)
                        .GetMethod("To" + targetType.Name, new[] { typeof(object) });

                    if (toTargetTypeMethod != null)
                    {
                        convert = Expression.Condition(
                            Expression.TypeIs(expression, targetType),
                            convert,
                            Expression.Call(toTargetTypeMethod, expression));
                    }
                }

                return Expression.Condition(
                    Expression.Equal(expression, Expression.Constant(null, typeof(object))),
                    Expression.Default(targetType),
                    convert);
            }

            return Expression.Convert(expression, targetType);
        }
#endif

        /// <summary>
        /// </summary>
        /// <param name="pProperty">
        /// </param>
        /// <returns>
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Reflection.Emit.DynamicMethod.#.ctor(System.String,System.Type,System.Type[],System.Boolean)")]
        public static Func<T, object> GetValueFunc<T>(this PropertyInfo pProperty)
        {
            try
            {
                if(!pProperty.IsPublic())
                    return null;

                MethodInfo getMethod = pProperty.GetGetMethod(true);
                if (getMethod == null || !getMethod.IsPublic)
                    return null;
#if NET
                DynamicMethod dynamicGetMethod = new DynamicMethod("GetValue", typeof(object), GetValueParameterTypes,
                    true);
                ILGenerator ilGenerator = dynamicGetMethod.GetILGenerator();
                ilGenerator.Emit(OpCodes.Ldarg_0);
                Debug.Assert(pProperty.DeclaringType != null, "pProperty.DeclaringType != null");
                ilGenerator.Emit(OpCodes.Castclass, pProperty.DeclaringType);
                ilGenerator.Emit(OpCodes.Callvirt, getMethod);
                if (pProperty.PropertyType.IsValueType())
                    ilGenerator.Emit(OpCodes.Box, pProperty.PropertyType);
                ilGenerator.Emit(OpCodes.Ret);
                return (Func<T, Object>) dynamicGetMethod.CreateDelegate(typeof(Func<T, Object>));
#elif NETSTANDARD
                Type instanceType = typeof(T);
                Type resultType = typeof(object);

                ParameterExpression parameterExpression = Expression.Parameter(instanceType, "instance");
                Expression resultExpression;

                if (getMethod.IsStatic)
                {
                    resultExpression = Expression.MakeMemberAccess(null, pProperty);
                }
                else
                {
                    Expression readParameter = EnsureCastExpression(parameterExpression, pProperty.DeclaringType);

                    resultExpression = Expression.MakeMemberAccess(readParameter, pProperty);
                }

                resultExpression = EnsureCastExpression(resultExpression, resultType);

                LambdaExpression lambdaExpression = Expression.Lambda(typeof(Func<T, object>), resultExpression, parameterExpression);

                Func<T, object> compiled = (Func<T, object>)lambdaExpression.Compile();
                return compiled;
#endif
            }
#if (NET || NETSTANDARD_1_3_UP)
            catch (InvalidProgramException)
#else
            catch(Exception)
#endif
            {
                return null;
            }
        }


#if NET
        private static DynamicMethod CreateDynamicMethod(string name, Type returnType, Type[] parameterTypes, Type owner)
        {
            DynamicMethod dynamicMethod = !owner.IsInterface()
                ? new DynamicMethod(name, returnType, parameterTypes, owner, true)
                : new DynamicMethod(name, returnType, parameterTypes, owner.Module, true);

            return dynamicMethod;
        }
        
        internal static void GenerateCreateSetPropertyIL(PropertyInfo propertyInfo, ILGenerator generator)
        {
            MethodInfo setMethod = propertyInfo.GetSetMethod(true);
            if (!setMethod.IsStatic)
            {
                generator.PushInstance(propertyInfo.DeclaringType);
            }

            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(propertyInfo.PropertyType);
            generator.CallMethod(setMethod);
            generator.Return();
        }

        private static void GenerateCreateGetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
        {
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.DeclaringType);
                generator.Emit(OpCodes.Ldfld, fieldInfo);
            }
            else
            {
                generator.Emit(OpCodes.Ldsfld, fieldInfo);
            }

            generator.BoxIfNeeded(fieldInfo.FieldType);
            generator.Return();
        }


        internal static void GenerateCreateSetFieldIL(FieldInfo fieldInfo, ILGenerator generator)
        {
            if (!fieldInfo.IsStatic)
            {
                generator.PushInstance(fieldInfo.DeclaringType);
            }

            generator.Emit(OpCodes.Ldarg_1);
            generator.UnboxIfNeeded(fieldInfo.FieldType);

            if (!fieldInfo.IsStatic)
            {
                generator.Emit(OpCodes.Stfld, fieldInfo);
            }
            else
            {
                generator.Emit(OpCodes.Stsfld, fieldInfo);
            }

            generator.Return();
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "fieldInfo")]
        public static Func<T, Object> GetValueFunc<T>(this FieldInfo fieldInfo)
        {
            try
            {
                Guard.ArgumentNotNull(fieldInfo, nameof(fieldInfo));
#if NET
                if (fieldInfo.IsLiteral)
                {
                    object constantValue = fieldInfo.GetValue(null);
                    Func<T, object> getter = o => constantValue;
                    return getter;
                }

                DynamicMethod dynamicMethod = CreateDynamicMethod("Get" + fieldInfo.Name, typeof(T), new[] { typeof(object) }, fieldInfo.DeclaringType);
                ILGenerator generator = dynamicMethod.GetILGenerator();

                GenerateCreateGetFieldIL(fieldInfo, generator);

                return (Func<T, object>)dynamicMethod.CreateDelegate(typeof(Func<T, object>));
#elif NETSTANDARD

                ParameterExpression sourceParameter = Expression.Parameter(typeof(T), "source");

                Expression fieldExpression;
                if (fieldInfo.IsStatic)
                {
                    fieldExpression = Expression.Field(null, fieldInfo);
                }
                else
                {
                    Expression sourceExpression = EnsureCastExpression(sourceParameter, fieldInfo.DeclaringType);

                    fieldExpression = Expression.Field(sourceExpression, fieldInfo);
                }

                fieldExpression = EnsureCastExpression(fieldExpression, typeof(object));

                return Expression.Lambda<Func<T, object>>(fieldExpression, sourceParameter).Compile();
#endif
            }
#if (NET || NETSTANDARD_1_3_UP)
            catch (InvalidProgramException)
#else
            catch (Exception)
#endif
            {
                return null;
            }
        }

#if (!NET && !NETSTANDARD_2_0_UP)
        internal delegate TResult MethodCall<T, TResult>(T target, params object[] args);

        public static MethodCall<T, object> CreateMethodCall<T>(this Type pType,MethodBase method)
        {
            Guard.ArgumentNotNull(method, nameof(method));

            ParameterExpression targetParameterExpression = Expression.Parameter(pType, "target");
            ParameterExpression argsParameterExpression = Expression.Parameter(typeof(object[]), "args");

            Expression callExpression = BuildMethodCall(method, pType, targetParameterExpression, argsParameterExpression);

            LambdaExpression lambdaExpression = Expression.Lambda(typeof(MethodCall<T, object>), callExpression, targetParameterExpression, argsParameterExpression);

            MethodCall<T, object> compiled = (MethodCall<T, object>)lambdaExpression.Compile();
            return compiled;
        }
        private class ByRefParameter
        {
            public Expression Value;
            public ParameterExpression Variable;
            public bool IsOut;
        }

        private static Expression BuildMethodCall(MethodBase method, Type type,
            ParameterExpression targetParameterExpression, ParameterExpression argsParameterExpression)
        {
            ParameterInfo[] parametersInfo = method.GetParameters();

            Expression[] argsExpression;
            IList<ByRefParameter> refParameterMap;

            argsExpression = new Expression[parametersInfo.Length];
            refParameterMap = new List<ByRefParameter>();
            if (parametersInfo.Length > 0)
            {
                argsExpression = new Expression[parametersInfo.Length];
                refParameterMap = new List<ByRefParameter>();

                for (int i = 0; i < parametersInfo.Length; i++)
                {
                    ParameterInfo parameter = parametersInfo[i];
                    Type parameterType = parameter.ParameterType;
                    bool isByRef = false;
                    if (parameterType.IsByRef)
                    {
                        parameterType = parameterType.GetElementType();
                        isByRef = true;
                    }

                    Expression indexExpression = Expression.Constant(i);

                    Expression paramAccessorExpression = Expression.ArrayIndex(argsParameterExpression, indexExpression);

                    Expression argExpression = EnsureCastExpression(paramAccessorExpression, parameterType, !isByRef);

                    if (isByRef)
                    {
                        ParameterExpression variable = Expression.Variable(parameterType);
                        refParameterMap.Add(new ByRefParameter
                        {
                            Value = argExpression,
                            Variable = variable,
                            IsOut = parameter.IsOut
                        });

                        argExpression = variable;
                    }

                    argsExpression[i] = argExpression;
                }
            }

            Expression callExpression;
            if (method.IsConstructor)
            {
                callExpression = Expression.New((ConstructorInfo) method, argsExpression);
            }
            else if (method.IsStatic)
            {
                callExpression = Expression.Call((MethodInfo) method, argsExpression);
            }
            else
            {
                Expression readParameter = EnsureCastExpression(targetParameterExpression, method.DeclaringType);

                callExpression = Expression.Call(readParameter, (MethodInfo) method, argsExpression);
            }

            MethodInfo m = method as MethodInfo;
            if (m != null)
            {
                if (m.ReturnType != typeof(void))
                {
                    callExpression = EnsureCastExpression(callExpression, type);
                }
                else
                {
                    callExpression = Expression.Block(callExpression, Expression.Constant(null));
                }
            }
            else
            {
                callExpression = EnsureCastExpression(callExpression, type);
            }

            if (refParameterMap.Count > 0)
            {
                IList<ParameterExpression> variableExpressions = new List<ParameterExpression>();
                IList<Expression> bodyExpressions = new List<Expression>();
                foreach (ByRefParameter p in refParameterMap)
                {
                    if (!p.IsOut)
                    {
                        bodyExpressions.Add(Expression.Assign(p.Variable, p.Value));
                    }

                    variableExpressions.Add(p.Variable);
                }

                bodyExpressions.Add(callExpression);

                callExpression = Expression.Block(variableExpressions, bodyExpressions);
            }

            return callExpression;
        }
#endif



        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Portability", "CA1903:UseOnlyApiFromTargetedFramework", MessageId = "System.Reflection.Emit.DynamicMethod.#.ctor(System.String,System.Type,System.Type[])")]
        public static Action<T, object> SetValueFunc<T>(this PropertyInfo pProperty)
        {
            try
            {
                if (!pProperty.IsPublic())
                    return null;

                MethodInfo setMethod = pProperty.GetSetMethod(true);
                if (setMethod == null || !setMethod.IsPublic)
                    return null;
#if NET
                DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + pProperty.Name, null, new[] { typeof(T), typeof(object) }, pProperty.DeclaringType);
                ILGenerator generator = dynamicMethod.GetILGenerator();

                GenerateCreateSetPropertyIL(pProperty, generator);

                return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
#elif NETSTANDARD

                // use reflection for structs
                // expression doesn't correctly set value
                if (pProperty.DeclaringType.IsValueType())
                {
                    return(o, v) => pProperty.SetValue(o, v, null);
                }

                Type instanceType = typeof(T);
                Type valueType = typeof(object);

                ParameterExpression instanceParameter = Expression.Parameter(instanceType, "instance");

                ParameterExpression valueParameter = Expression.Parameter(valueType, "value");
                Expression readValueParameter = EnsureCastExpression(valueParameter, pProperty.PropertyType);
                

                Expression setExpression;
                if (setMethod.IsStatic)
                {
                    setExpression = Expression.Call(setMethod, readValueParameter);
                }
                else
                {
                    Expression readInstanceParameter = EnsureCastExpression(instanceParameter, pProperty.DeclaringType);

                    setExpression = Expression.Call(readInstanceParameter, setMethod, readValueParameter);
                }

                LambdaExpression lambdaExpression = Expression.Lambda(typeof(Action<T, object>), setExpression, instanceParameter, valueParameter);

                return (Action<T, object>)lambdaExpression.Compile();
#endif
            }
#if (NET || NETSTANDARD_1_3_UP)
            catch (InvalidProgramException)
#else
            catch (Exception)
#endif
            {
                return null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters",
            MessageId = "fieldInfo")]
        public static Action<T, Object> SetValueFunc<T>(this FieldInfo fieldInfo)
        {
            try
            {
                Guard.ArgumentNotNull(fieldInfo, nameof(fieldInfo));
#if NET
                DynamicMethod dynamicMethod = CreateDynamicMethod("Set" + fieldInfo.Name, null, new[] { typeof(T), typeof(object) }, fieldInfo.DeclaringType);
                ILGenerator generator = dynamicMethod.GetILGenerator();

                GenerateCreateSetFieldIL(fieldInfo, generator);

                return (Action<T, object>)dynamicMethod.CreateDelegate(typeof(Action<T, object>));
#elif NETSTANDARD

                // use reflection for structs
                // expression doesn't correctly set value
                if (fieldInfo.DeclaringType.IsValueType() || fieldInfo.IsInitOnly)
                {
                    return (o, v) => fieldInfo.SetValue(o, v);
                }

                ParameterExpression sourceParameterExpression = Expression.Parameter(typeof(T), "source");
                ParameterExpression valueParameterExpression = Expression.Parameter(typeof(object), "value");

                Expression fieldExpression;
                if (fieldInfo.IsStatic)
                {
                    fieldExpression = Expression.Field(null, fieldInfo);
                }
                else
                {
                    Expression sourceExpression = EnsureCastExpression(sourceParameterExpression,
                        fieldInfo.DeclaringType);

                    fieldExpression = Expression.Field(sourceExpression, fieldInfo);
                }

                Expression valueExpression = EnsureCastExpression(valueParameterExpression, fieldExpression.Type);

                BinaryExpression assignExpression = Expression.Assign(fieldExpression, valueExpression);

                LambdaExpression lambdaExpression = Expression.Lambda(typeof(Action<T, object>), assignExpression,
                    sourceParameterExpression, valueParameterExpression);

                return (Action<T, object>) lambdaExpression.Compile();
#endif
            }
#if (NET || NETSTANDARD_1_3_UP)
            catch (InvalidProgramException)
#else
            catch (Exception)
#endif
            {
                return null;
            }
        }
    }
}