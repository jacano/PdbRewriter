using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication7
{
    [TypeIdentifier]
    [CompilerGenerated]
    [Guid("79F1BB5F-B66E-48E5-B6A9-1545C323CA3D")]
    [CoClass(typeof(object))]
    [ComImport]
    public interface DiaSource : IDiaDataSource
    {
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("79F1BB5F-B66E-48E5-B6A9-1545C323CA3D")]
    [TypeIdentifier]
    [CompilerGenerated]
    [ComImport]
    public interface IDiaDataSource
    {
        [SpecialName]
        void _VtblGap1_1();

        void loadDataFromPdb([MarshalAs(UnmanagedType.LPWStr), In] string pdbPath);

        [SpecialName]
        void _VtblGap2_3();

        void openSession([MarshalAs(UnmanagedType.Interface)] out IDiaSession ppSession);
    }

    [Guid("10F3DBD9-664F-4469-B808-9471C7A50538")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeIdentifier]
    [CompilerGenerated]
    [ComImport]
    public interface IDiaEnumSourceFiles
    {
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        IEnumerator GetEnumerator();
    }

    [TypeIdentifier]
    [Guid("C65C2B0A-1150-4D7A-AFCC-E05BF3DEE81E")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CompilerGenerated]
    [ComImport]
    public interface IDiaEnumTables
    {
        [return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "System.Runtime.InteropServices.CustomMarshalers.EnumeratorToEnumVariantMarshaler, CustomMarshalers, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
        IEnumerator GetEnumerator();
    }

    [CompilerGenerated]
    [Guid("6FC5D63F-011E-40C2-8DD2-E6486E9D6B68")]
    [TypeIdentifier]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [ComImport]
    public interface IDiaSession
    {
        [SpecialName]
        void _VtblGap1_3();

        void getEnumTables([MarshalAs(UnmanagedType.Interface)] out IDiaEnumTables ppEnumTables);
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [TypeIdentifier]
    [Guid("A2EF5353-F5A8-4EB3-90D2-CB526ACB3CDD")]
    [CompilerGenerated]
    [ComImport]
    public interface IDiaSourceFile
    {
        [SpecialName]
        void _VtblGap1_1();

        string fileName { get; }
    }

    [Guid("4A59FB77-ABAC-469B-A30B-9ECC85BFEF14")]
    [TypeIdentifier]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CompilerGenerated]
    [ComImport]
    public interface IDiaTable : IEnumUnknown
    {
    }

    [TypeIdentifier]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [CompilerGenerated]
    [Guid("00000100-0000-0000-C000-000000000046")]
    [ComImport]
    public interface IEnumUnknown
    {
    }
}
