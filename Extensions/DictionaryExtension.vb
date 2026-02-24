Imports System.Collections.Generic
Imports System.Runtime.CompilerServices
Imports Microsoft.Xna.Framework

Public Module DictionaryExtension

        <Extension>
        Public Function Copy(Of TKey, TValue)(original As Dictionary(Of TKey, TValue)) As Dictionary(Of TKey, TValue)
            Dim ret As New Dictionary(Of TKey, TValue)(original.Count, original.Comparer)
            For Each entry As KeyValuePair(Of TKey, TValue) In original
                ret.Add(entry.Key, DirectCast(entry.Value, TValue))
            Next
            Return ret
        End Function

    End Module