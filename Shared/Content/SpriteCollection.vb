Imports Microsoft.Xna.Framework.Graphics
Imports System.Collections.Generic

Namespace [Shared].Content

    Public Class SpriteCollection
        Implements IDisposable

#Region "Properties"

        Private Textures As New SortedList(Of Integer, Texture2D)

        Public Property Frame(Index As Integer) As Texture2D
            Get
                Return Textures.Values(Index)
            End Get
            Set(value As Texture2D)
                Textures.Values(Index) = value
            End Set
        End Property

        Public ReadOnly Property Count() As Integer
            Get
                Return Textures.Count
            End Get
        End Property

#End Region

        Public Overloads Sub LoadSprite(Texture As Texture2D)
            Me.Textures.Add(Me.Textures.Count, Texture)
        End Sub

        Public Overloads Sub LoadSprite(Index As Integer, Texture As Texture2D)
            If Textures.ContainsKey(Index) Then
                Throw New Exception("Key already exists in collection.")
            Else
                Me.Textures.Add(Index, Texture)
            End If
        End Sub

        Public Sub LoadSprites(Textures As Texture2D())
            For Each Texture As Texture2D In Textures
                LoadSprite(Texture)
            Next
        End Sub

        Public Sub RemoveSprite(Index As Integer)
            Textures.Remove(Index)
        End Sub

#Region "IDisposable Support"
        Private disposedValue As Boolean ' To detect redundant calls

        ' IDisposable
        Protected Overridable Sub Dispose(disposing As Boolean)
            If Not Me.disposedValue Then
                If disposing Then
                    ' TODO: dispose managed state (managed objects).
                    For Each Texture As Texture2D In Textures.Values
                        Texture.Dispose()
                    Next
                    Textures.Clear()
                End If

                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                ' TODO: set large fields to null.
                Textures = Nothing
            End If
            Me.disposedValue = True
        End Sub

        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        'Protected Overrides Sub Finalize()
        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '    Dispose(False)
        '    MyBase.Finalize()
        'End Sub

        ' This code added by Visual Basic to correctly implement the disposable pattern.
        Public Sub Dispose() Implements IDisposable.Dispose
            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(True)
            GC.SuppressFinalize(Me)
        End Sub
#End Region

    End Class

End Namespace
