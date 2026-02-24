#If WINDOWS7_0 Then
Imports Microsoft.Xna.Framework.Graphics

Namespace [Shared].Util

    Public Class BitmapConverter

        Public Shared Function FileToTexture2D(GraphicsDevice As GraphicsDevice, Filename As String) As Texture2D
            Dim Bitmap As New System.Drawing.Bitmap(Filename)
            Dim bufferSize As Integer = Bitmap.Height * Bitmap.Width * 4
            Dim stream As New System.IO.MemoryStream(bufferSize)
            Bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png)
            Dim Texture As Texture2D = Texture2D.FromStream(GraphicsDevice, stream)
            Bitmap.Dispose()
            Return Texture
        End Function

        Public Shared Function FilesToTexture2D(GraphicsDevice As GraphicsDevice, Filenames As String()) As Texture2D()
            Dim Textures As New List(Of Texture2D)
            For Each Filename As String In Filenames
                Textures.Add(FileToTexture2D(GraphicsDevice, Filename))
            Next
            Return Textures.ToArray
        End Function

    End Class

End Namespace
#End If