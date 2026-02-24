Imports System.Runtime.CompilerServices
Imports Microsoft.Xna.Framework

Public Class ColorExtension

        Public Shared Function ToByteArray(Color As Color) As Byte()
            Return New Byte(3) {Color.R, Color.G, Color.B, Color.A}
        End Function

End Class