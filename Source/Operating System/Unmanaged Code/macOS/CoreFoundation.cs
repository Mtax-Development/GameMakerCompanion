#if macOS
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

//|Adapted from: https://github.com/lithiumtoast/pinvoke-macos-CGWindowListCreate
namespace GameMakerCompanion.OperatingSystem.UnmanagedCode
{
    /// <summary> Platform invoke interoperability of macOS Core Foundation framework. </summary>
    /// <see href="https://developer.apple.com/documentation/corefoundation"/>
    internal unsafe partial class CoreFoundation
    {
        private const string Framework = @"/System/Library/Frameworks/CoreGraphics.framework/Versions/Current/CoreGraphics";
        private const uint kCFNumberIntType = 9;
        private static readonly Dictionary<uint, CString> StringHashesToPointers = [];
        private static readonly Dictionary<nint, string> PointersToStrings = [];
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void CFRelease(void* pointer);
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CBool CFNumberGetValue(CFNumberRef reference, CFNumberType type, void* pointer);
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CFStringRef CFStringCreateWithCString(CFAllocatorRef reference, CString cString, CFStringEncoding encoding);

        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CString CFStringGetCStringPtr(CFStringRef reference, CFStringEncoding encoding);

        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CFIndex CFArrayGetCount(CFArrayRef reference);

        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial void* CFArrayGetValueAtIndex(CFArrayRef reference, CFIndex index);

        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CBool CFDictionaryGetValueIfPresent(CFDictionaryRef reference, void* key, void** value);
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFIndex
        {
            [FieldOffset(0)]
            public long Data;
            public static implicit operator long(CFIndex data) => data.Data;
            public static implicit operator CFIndex(long data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFNumberRef
        {
            [FieldOffset(0)]
            public void* Data;
            public static implicit operator void*(CFNumberRef data) => data.Data;
            public static implicit operator CFNumberRef(void* data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFNumberType
        {
            [FieldOffset(0)]
            public CFIndex Data;
            public static implicit operator CFIndex(CFNumberType data) => data.Data;
            public static implicit operator CFNumberType(CFIndex data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFStringRef
        {
            [FieldOffset(0)]
            public void* Data;
            public static implicit operator void*(CFStringRef data) => data.Data;
            public static implicit operator CFStringRef(void* data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 4)]
        public struct CFStringEncoding
        {
            [FieldOffset(0)]
            public uint Data;
            public static implicit operator uint(CFStringEncoding data) => data.Data;
            public static implicit operator CFStringEncoding(uint data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFArrayRef
        {
            [FieldOffset(0)]
            public void* Data;
            public static implicit operator void*(CFArrayRef data) => data.Data;
            public static implicit operator CFArrayRef(void* data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFDictionaryRef
        {
            [FieldOffset(0)]
            public void* Data;
            public static implicit operator void*(CFDictionaryRef data) => data.Data;
            public static implicit operator CFDictionaryRef(void* data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 8, Pack = 8)]
        public struct CFAllocatorRef
        {
            [FieldOffset(0)]
            public void* Data;
            public static implicit operator void*(CFAllocatorRef data) => data.Data;
            public static implicit operator CFAllocatorRef(void* data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct CBool(bool value)
        {
            private readonly byte _value = Convert.ToByte(value);
            
            public static implicit operator bool(CBool value)
            {
                return Convert.ToBoolean(value._value);
            }
        }
        
        internal static CFStringRef CFString(string text)
        {
            static uint HashDjb2(string text)
            {
                uint hash = 5381;
                
                foreach (char character in text)
                {
                    hash = ((hash << 5) + hash + character);
                }
                
                return hash;
            }
            
            uint hash = HashDjb2(text);
            CString cString;
            
            if (StringHashesToPointers.TryGetValue(hash, out CString result))
            {
                cString = result;
            }
            else
            {
                nint pointer = Marshal.StringToHGlobalAnsi(text);
                StringHashesToPointers.Add(hash, new CString(pointer));
                PointersToStrings.Add(pointer, text);
                
                cString = new CString(pointer);
            }
            
            return CFStringCreateWithCString(default, cString, default);
        }

        [StructLayout(LayoutKind.Sequential)]
        public readonly unsafe struct CString(nint value)
        {
            internal readonly nint _pointer = value;
            
            public bool IsNull {get {return (_pointer == 0);}}
            
            public static implicit operator string(CString value)
            {
                static uint HashDjb2(byte* str)
                {
                    uint hash = 5381;
                    unchecked
                    {
                        uint c;
                        while ((c = *str++) != 0)
                        {
                            hash = ((hash << 5) + hash + c);
                        }
                    }
                    
                    return hash;
                }
                
                if (value.IsNull)
                {
                    return string.Empty;
                }
                
                if (PointersToStrings.TryGetValue(value._pointer, out string? result))
                {
                    return result;
                }
                
                uint hash = HashDjb2((byte*)value._pointer);
                
                if (StringHashesToPointers.TryGetValue(hash, out CString pointer2))
                {
                    result = PointersToStrings[pointer2._pointer];
                    
                    return result;
                }
                
                result = Marshal.PtrToStringAnsi(value._pointer);
                
                if (string.IsNullOrEmpty(result))
                {
                    return string.Empty;
                }
                
                StringHashesToPointers.Add(hash, value);
                PointersToStrings.Add(value._pointer, result);
                
                return result;
            }
        }
        
        internal static unsafe int? DictionaryReadCInt(CFDictionaryRef dictionary, CFStringRef key)
        {
            void* dictionaryValue;
            CBool containsKey = CFDictionaryGetValueIfPresent(dictionary, key, &dictionaryValue);
            
            if (!containsKey)
            {
                return null;
            }
            
            CFNumberRef number = (CFNumberRef)dictionaryValue;
            CFNumberType type;
            type.Data = kCFNumberIntType;
            int value;
            CFNumberGetValue(number, type, &value);
            
            return value;
        }
        
        internal static unsafe string? DictionaryReadString(CFDictionaryRef dictionary, CFStringRef key)
        {
            void* dictionaryValue;
            CBool containsKey = CFDictionaryGetValueIfPresent(dictionary, key, &dictionaryValue);
            
            if (!containsKey)
            {
                return null;
            }
            
            CFStringRef cfString = (CFStringRef)dictionaryValue;
            CString cString = CFStringGetCStringPtr(cfString, default);
            
            return (string)cString;
        }
    }
}
#endif
