Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics

Namespace [Shared].Content

    Public Class Textures

        Public Property GraphicsDevice As GraphicsDevice

        Public Overloads Shared Function CreateSolidTexture(GraphicsDevice As GraphicsDevice, Color As Color) As Texture2D
            Dim t As New Texture2D(GraphicsDevice, 1, 1)
            t.SetData(Of Color)({Color})
            Return t
        End Function

        Public Sub New(GraphicsDevice As GraphicsDevice)
            Me.GraphicsDevice = GraphicsDevice
        End Sub
    End Class

End Namespace