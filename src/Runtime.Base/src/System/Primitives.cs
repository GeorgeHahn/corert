// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This file contains the basic primitive type definitions (int etc)
// These types are well known to the compiler and the runtime and are basic interchange types that do not change

// CONTRACT with Runtime
// Each of the data types has a data contract with the runtime. See the contract in the type definition
//

using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace System
{
    // CONTRACT with Runtime
    // Place holder type for type hierarchy, Compiler/Runtime requires this class
    public abstract class ValueType
    {
    }

    // CONTRACT with Runtime, Compiler/Runtime requires this class
    // Place holder type for type hierarchy
    public abstract class Enum : ValueType
    {
    }

    /*============================================================
    **
    ** Class:  Boolean
    **
    **
    ** Purpose: The boolean class serves as a wrapper for the primitive
    ** type boolean.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The Boolean type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type bool

    public struct Boolean
    {
        // Disable compile warning about unused m_value field
#pragma warning disable 0169
        private bool _value;
#pragma warning restore 0169
    }


    /*============================================================
    **
    ** Class:  Char
    **
    **
    ** Purpose: This is the value class representing a Unicode character
    **
    **
    ===========================================================*/



    // CONTRACT with Runtime
    // The Char type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type char
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Char
    {
        private char _value;
    }


    /*============================================================
    **
    ** Class:  SByte
    **
    **
    ** Purpose: A representation of a 8 bit 2's complement integer.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The SByte type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type sbyte
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct SByte
    {
        private sbyte _value;
    }


    /*============================================================
    **
    ** Class:  Byte
    **
    **
    ** Purpose: A representation of a 8 bit integer (byte)
    **
    ** 
    ===========================================================*/


    // CONTRACT with Runtime
    // The Byte type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type bool
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Byte
    {
        private byte _value;
    }


    /*============================================================
    **
    ** Class:  Int16
    **
    **
    ** Purpose: A representation of a 16 bit 2's complement integer.
    **
    ** 
    ===========================================================*/


    // CONTRACT with Runtime
    // The Int16 type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type short
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Int16
    {
        private short _value;
    }

    /*============================================================
    **
    ** Class:  UInt16
    **
    **
    ** Purpose: A representation of a short (unsigned 16-bit) integer.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The Uint16 type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type ushort
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct UInt16
    {
        private ushort _value;
    }

    /*============================================================
    **
    ** Class:  Int32
    **
    **
    ** Purpose: A representation of a 32 bit 2's complement integer.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The Int32 type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type int
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Int32
    {
        private int _value;
    }


    /*============================================================
    **
    ** Class:  UInt32
    **
    **
    ** Purpose: A representation of a 32 bit unsigned integer.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The Uint32 type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type uint
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct UInt32
    {
        private uint _value;
    }


    /*============================================================
    **
    ** Class:  Int64
    **
    **
    ** Purpose: A representation of a 64 bit 2's complement integer.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The Int64 type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type long
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Int64
    {
        private long _value;
    }


    /*============================================================
    **
    ** Class:  UInt64
    **
    **
    ** Purpose: A representation of a 64 bit unsigned integer.
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The UInt64 type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type ulong
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct UInt64
    {
        private ulong _value;
    }


    /*============================================================
    **
    ** Class:  Single
    **
    **
    ** Purpose: A wrapper class for the primitive type float.
    **
    **
    ===========================================================*/

    // CONTRACT with Runtime
    // The Single type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type float
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Single
    {
        private float _value;
    }


    /*============================================================
    **
    ** Class:  Double
    **
    **
    ** Purpose: A representation of an IEEE double precision
    **          floating point number.
    **
    **
    ===========================================================*/

    // CONTRACT with Runtime
    // The Double type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type double
    // This type is LayoutKind Sequential

    [StructLayout(LayoutKind.Sequential)]
    public struct Double
    {
        private double _value;
    }



    /*============================================================
    **
    ** Class:  IntPtr
    **
    **
    ** Purpose: Platform independent integer
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The IntPtr type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type void *

    // This type implements == without overriding GetHashCode, Equals, disable compiler warning
#pragma warning disable 0660, 0661
    public struct IntPtr
    {
        unsafe private void* _value; // The compiler treats void* closest to uint hence explicit casts are required to preserve int behavior

        [Intrinsic]
        public static readonly IntPtr Zero;

        [Intrinsic]
        public unsafe IntPtr(void* value)
        {
            _value = value;
        }

        [Intrinsic]
        public static unsafe explicit operator IntPtr(void* value)
        {
            return new IntPtr(value);
        }

        [Intrinsic]
        public static unsafe explicit operator void* (IntPtr value)
        {
            return value._value;
        }

        [Intrinsic]
        public unsafe static bool operator ==(IntPtr value1, IntPtr value2)
        {
            return value1._value != value2._value;
        }

        [Intrinsic]
        public unsafe static bool operator !=(IntPtr value1, IntPtr value2)
        {
            return value1._value != value2._value;
        }
    }
#pragma warning restore 0660, 0661
#pragma warning restore 0661


    /*============================================================
    **
    ** Class:  UIntPtr
    **
    **
    ** Purpose: Platform independent integer
    **
    ** 
    ===========================================================*/

    // CONTRACT with Runtime
    // The UIntPtr type is one of the primitives understood by the compilers and runtime
    // Data Contract: Single field of type void *

    // This type implements == without overriding GetHashCode, Equals, disable compiler warning
#pragma warning disable 0660, 0661
    public struct UIntPtr
    {
        unsafe private void* _value;

        [Intrinsic]
        public static readonly UIntPtr Zero;

        [Intrinsic]
        public unsafe UIntPtr(void* value)
        {
            _value = value;
        }

        [Intrinsic]
        public static unsafe explicit operator UIntPtr(void* value)
        {
            return new UIntPtr(value);
        }

        [Intrinsic]
        public static unsafe explicit operator void* (UIntPtr value)
        {
            return value._value;
        }

        [Intrinsic]
        public unsafe static bool operator ==(UIntPtr value1, UIntPtr value2)
        {
            return value1._value == value2._value;
        }

        [Intrinsic]
        public unsafe static bool operator !=(UIntPtr value1, UIntPtr value2)
        {
            return value1._value != value2._value;
        }
    }
#pragma warning restore 0660, 0661
}

