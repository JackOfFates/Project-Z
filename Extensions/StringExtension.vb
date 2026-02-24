Imports System.Collections.Generic
Imports System.Runtime.CompilerServices


Module StringExtension

    ''' <summary>
    ''' Searches for a string within another string.
    ''' </summary>
    ''' <param name="Input">The String Containing the String You are Searching for.</param>
    ''' <param name="SearchFor"></param>
    ''' <param name="Startindex">Start Index Modifier.</param>
    ''' <returns>Indexies in an array.</returns>
    ''' <remarks></remarks>
    <Extension()>
    Public Function Search(Input As String, SearchFor As String, Startindex As Integer) As Integer()
        Dim Indexies As Integer() = {Input.IndexOf(SearchFor, Startindex)}
        Dim textEnd As Integer = Input.Length
        Dim index As Integer = Startindex
        Dim lastIndex As Integer = Input.LastIndexOf(SearchFor)
        While (index < lastIndex)
            index = Input.IndexOf(SearchFor, index) + 1
            Indexies.Add(index)
        End While
        Return Indexies.ToArray
    End Function
End Module

