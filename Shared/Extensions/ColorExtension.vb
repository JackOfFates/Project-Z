Imports System.Runtime.CompilerServices
Imports Microsoft.Xna.Framework

Namespace [Shared].Extensions

    Module ColorExtension

        <Extension>
        Public Function ToByteArray(Color As Color) As Byte()
            Return New Byte(3) {Color.R, Color.G, Color.B, Color.A}
        End Function

    End Module

End Namespace