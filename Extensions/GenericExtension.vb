Imports System.Runtime.CompilerServices

Public Module GenericExtension

        <Extension()>
        Public Sub Add(Of T)(ByRef arr As T(), item As T)
            Array.Resize(arr, arr.Length + 1)
            arr(arr.Length - 1) = item
        End Sub

    End Module
