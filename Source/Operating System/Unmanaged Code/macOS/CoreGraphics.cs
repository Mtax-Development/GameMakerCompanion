#if macOS
using System.Runtime.InteropServices;
using static GameMakerCompanion.OperatingSystem.UnmanagedCode.CoreFoundation;

//|Adapted from: https://github.com/lithiumtoast/pinvoke-macos-CGWindowListCreate
namespace GameMakerCompanion.OperatingSystem.UnmanagedCode
{
    /// <summary> Platform invoke interoperability of macOS Core Graphics framework. </summary>
    /// <see href="https://developer.apple.com/documentation/coregraphics"/>
    internal unsafe partial class CoreGraphics
    {
        private const string Framework = @"/System/Library/Frameworks/CoreGraphics.framework/Versions/Current/CoreGraphics";
        public const uint kCGWindowListOptionAll = 0;
        public static CGWindowID kCGNullWindowID = 0;
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CBool CGPreflightScreenCaptureAccess();
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CBool CGRequestScreenCaptureAccess();
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CFArrayRef CGWindowListCreate(CGWindowListOption option, CGWindowID relativeToWindow);
        
        [LibraryImport(Framework)]
        [UnmanagedCallConv(CallConvs = [typeof(System.Runtime.CompilerServices.CallConvCdecl)])]
        public static partial CFArrayRef CGWindowListCreateDescriptionFromArray(CFArrayRef window);
        
        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 4)]
        public struct CGWindowListOption
        {
            [FieldOffset(0)]
            public uint Data;
            public static implicit operator uint(CGWindowListOption data) => data.Data;
            public static implicit operator CGWindowListOption(uint data) => new() {Data = data};
        }
        
        [StructLayout(LayoutKind.Explicit, Size = 4, Pack = 4)]
        public struct CGWindowID
        {
            [FieldOffset(0)]
            public uint Data;
            public static implicit operator uint(CGWindowID data) => data.Data;
            public static implicit operator CGWindowID(uint data) => new() {Data = data};
        }
    }
}
#endif
