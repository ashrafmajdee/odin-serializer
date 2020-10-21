﻿//-----------------------------------------------------------------------
// <copyright file="ArrayFormatterLocator.cs" company="Sirenix IVS">
// Copyright (c) 2018 Sirenix IVS
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//-----------------------------------------------------------------------

using OdinSerializer;

[assembly: RegisterFormatterLocator(typeof(ArrayFormatterLocator), -80)]

namespace OdinSerializer
{
    using Utilities;
    using System;

    internal class ArrayFormatterLocator : IFormatterLocator
    {
        public bool TryGetFormatter(Type type, FormatterLocationStep step, ISerializationPolicy policy, out IFormatter formatter)
        {
            if (!type.IsArray)
            {
                formatter = null;
                return false;
            }

            var elementType = type.GetElementType();

            if (type.GetArrayRank() == 1)
            {
                if (FormatterUtilities.IsPrimitiveArrayType(elementType))
                {
                    if (EmitUtilities.CanEmit)
                    {
                        formatter = (IFormatter)Activator.CreateInstance(typeof(PrimitiveArrayFormatter<>).MakeGenericType(elementType));
                    }
                    else
                    {
                        formatter = new WeakPrimitiveArrayFormatter(type, elementType);
                    }
                }
                else
                {
                    if (EmitUtilities.CanEmit)
                    {
                        formatter = (IFormatter)Activator.CreateInstance(typeof(ArrayFormatter<>).MakeGenericType(elementType));
                    }
                    else
                    {
                        formatter = new WeakArrayFormatter(type, elementType);
                    }
                }
            }
            else
            {
                if (EmitUtilities.CanEmit)
                {
                    formatter = (IFormatter)Activator.CreateInstance(typeof(MultiDimensionalArrayFormatter<,>).MakeGenericType(type, type.GetElementType()));
                }
                else
                {
                    formatter = new WeakMultiDimensionalArrayFormatter(type, elementType);
                }
            }

            return true;
        }
    }
}