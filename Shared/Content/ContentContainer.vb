Imports System.Collections.Generic
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework.Content

Namespace [Shared].Content

    Public Class ContentContainer
        'Implements IDisposable

        Private GraphicsDevice As GraphicsDevice

        Public TexturesInstance As Textures

        Public Property Content As ContentManager

        Public Property Fonts As New Dictionary(Of String, SpriteFont)

        Public Property Textures As New Dictionary(Of String, Texture2D)

        Public Property AllCollections As New List(Of Collections.IDictionary) From {Fonts, Textures}


#Region "Content Methods"

        Public Sub LoadAllContent()
            '#If WINDOWS Then
            Dim ContentDirectory As String = AppDomain.CurrentDomain.BaseDirectory & "Content"

            For Each FontDirectory As String In IO.Directory.GetDirectories(ContentDirectory & "\Fonts")
                For Each FontPath As String In IO.Directory.GetFiles(FontDirectory)
                    Dim RelativeFontPath As String = FontPath.Remove(0, ContentDirectory.Length + 1)
                    Dim Extension As String = IO.Path.GetExtension(RelativeFontPath)
                    Dim Font As SpriteFont = Content.Load(Of SpriteFont)(RelativeFontPath.Replace(Extension, Nothing))
                    Fonts.Add(IO.Path.GetFileNameWithoutExtension(FontPath), Font)
                Next
            Next
            '#End If
        End Sub
#If WINDOWS Then
        Public Function GetContentResource(Name As String) As Byte()
            Return DirectCast(My.Resources.ResourceManager.GetObject(Name), Byte())
        End Function
#End If
#End Region

        Public Sub New(Content As ContentManager, GraphicsDevice As GraphicsDevice)
            Me.Content = Content
            Me.GraphicsDevice = GraphicsDevice
            TexturesInstance = New Textures(GraphicsDevice)
        End Sub

        '#Region "IDisposable Support"
        '        Private disposedValue As Boolean ' To detect redundant calls

        '        ' IDisposable
        '        Protected Overridable Sub Dispose(disposing As Boolean)
        '            If Not disposedValue Then
        '                If disposing Then
        '                    ' TODO: dispose managed state (managed objects).
        '                    For Each Texture As Texture2D In Textures.Values
        '                        Texture.Dispose()
        '                    Next
        '                    Content.Dispose()
        '                End If

        '                ' TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
        '                ' TODO: set large fields to null.
        '                Fonts = Nothing
        '                Textures = Nothing
        '                AllCollections = Nothing
        '            End If
        '            disposedValue = True
        '        End Sub

        '        ' TODO: override Finalize() only if Dispose(ByVal disposing As Boolean) above has code to free unmanaged resources.
        '        'Protected Overrides Sub Finalize()
        '        '    ' Do not change this code.  Put cleanup code in Dispose(ByVal disposing As Boolean) above.
        '        '    Dispose(False)
        '        '    MyBase.Finalize()
        '        'End Sub

        '        ' This code added by Visual Basic to correctly implement the disposable pattern.
        '        Public Sub Dispose() Implements IDisposable.Dispose
        '            ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        '            Dispose(True)
        '            GC.SuppressFinalize(Me)
        '        End Sub
        '#End Region

    End Class

End Namespace
