﻿/*
MIT License

Copyright (c) 2019 Wildbook

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace TwitchIntegration.Utils
{
    public static class Reflection
    {
        private static readonly BindingFlags _defaultFlags
            = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;

        #region Field

        public static TReturn GetFieldValue<TReturn>(this object instance, string fieldName) =>
            (TReturn)instance.GetType()
                             .GetField(fieldName, _defaultFlags | BindingFlags.Instance)
                             .GetValue(instance);

        public static TReturn GetFieldValue<TClass, TReturn>(string fieldName) =>
            GetFieldValue<TReturn>(typeof(TClass), fieldName);

        public static TReturn GetFieldValue<TReturn>(this Type type, string fieldName) =>
            (TReturn)type.GetField(fieldName, _defaultFlags | BindingFlags.Static)
                         .GetValue(null);

        public static void SetFieldValue(this object instance, string fieldName, object value) =>
            instance.GetType()
                    .GetField(fieldName, _defaultFlags | BindingFlags.Instance)
                    .SetValue(instance, value);

        public static void SetFieldValue<TClass>(string fieldName, object value) =>
            SetFieldValue(typeof(TClass), fieldName, value);

        public static void SetFieldValue(this Type type, string fieldName, object value) =>
            type.GetField(fieldName, _defaultFlags | BindingFlags.Static)
                .SetValue(null, value);
        #endregion

        #region Property

        public static TReturn GetPropertyValue<TReturn>(this object instance, string propName) =>
            (TReturn)instance.GetType()
                             .GetProperty(propName, _defaultFlags | BindingFlags.Instance)
                             .GetValue(instance);

        public static TReturn GetPropertyValue<TClass, TReturn>(string propName) =>
            GetPropertyValue<TReturn>(typeof(TClass), propName);

        public static TReturn GetPropertyValue<TReturn>(this Type type, string propName) =>
            (TReturn)type.GetProperty(propName, _defaultFlags | BindingFlags.Static)
                         .GetValue(null);

        public static void SetPropertyValue(this object instance, string propName, object value) =>
            instance.GetType()
                    .GetProperty(propName, _defaultFlags | BindingFlags.Instance)
                    .SetValue(instance, value);

        public static void SetPropertyValue<TClass>(string propName, object value) =>
            SetPropertyValue(typeof(TClass), propName, value);

        public static void SetPropertyValue(this Type type, string propName, object value) =>
            type.GetProperty(propName, _defaultFlags | BindingFlags.Static)
                .SetValue(null, value);
        #endregion

        #region Method

        public static TReturn InvokeMethod<TReturn>(this object instance, string methodName, params object[] methodParams) =>
            (TReturn)instance.GetType()
                             .GetMethod(methodName, _defaultFlags | BindingFlags.Instance)
                             .Invoke(instance, methodParams);

        public static TReturn InvokeMethod<TClass, TReturn>(string methodName, params object[] methodParams) =>
            InvokeMethod<TReturn>(typeof(TClass), methodName, methodParams);

        public static TReturn InvokeMethod<TReturn>(this Type type, string methodName, params object[] methodParams) =>
            (TReturn)type.GetMethod(methodName, _defaultFlags | BindingFlags.Static)
                         .Invoke(null, methodParams);

        public static void InvokeMethod(this object instance, string methodName, params object[] methodParams) =>
            instance.InvokeMethod<object>(methodName, methodParams);

        public static void InvokeMethod<TClass>(string methodName, params object[] methodParams) =>
            InvokeMethod<TClass, object>(methodName, methodParams);

        public static void InvokeMethod(this Type type, string methodName, params object[] methodParams) =>
            InvokeMethod<object>(methodName, methodParams);
        #endregion

        #region Class

        public static Type GetNestedType<TParent>(string name) =>
            GetNestedType(typeof(TParent), name);

        public static Type GetNestedType(this Type parentType, string name) =>
            parentType.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic);

        public static object Instantiate(this Type type) =>
            Activator.CreateInstance(type, true);

        public static object InstantiateGeneric<TClass>(Type typeArgument) =>
            InstantiateGeneric(typeof(TClass), typeArgument);

        public static object InstantiateGeneric(this Type genericType, Type typeArgument) =>
            genericType.MakeGenericType(typeArgument).Instantiate();

        public static object InstantiateGeneric<TClass>(Type[] typeArguments) =>
            InstantiateGeneric(typeof(TClass), typeArguments);

        public static object InstantiateGeneric(this Type genericType, Type[] typeArguments) =>
            genericType.MakeGenericType(typeArguments).Instantiate();

        public static IList InstantiateList(this Type type) =>
            (IList)typeof(List<>).MakeGenericType(type).Instantiate();

        #endregion
    }
}
