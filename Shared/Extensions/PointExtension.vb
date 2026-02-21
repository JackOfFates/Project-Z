Imports System.Runtime.CompilerServices
Imports Microsoft.Xna.Framework

Namespace [Shared].Extensions

    Public Module PointExtension

        <Extension()>
        Public Function Subtract(p1 As Point, p2 As Point) As Point
            Return New Point(p1.X - p2.X, p1.Y - p2.Y)
        End Function

        <Extension()>
        Public Function Subtract(p1 As Point, p2 As Vector2) As Point
            Return New Point(CInt(p1.X - p2.X), CInt(p1.Y - p2.Y))
        End Function

        <Extension()>
        Public Function Subtract(p1 As Vector2, p2 As Point) As Vector2
            Return New Vector2(CInt(p1.X - p2.X), CInt(p1.Y - p2.Y))
        End Function

        <Extension()>
        Public Function Subtract(p1 As Vector2, p2 As Vector2) As Vector2
            Return New Vector2(CInt(p1.X - p2.X), CInt(p1.Y - p2.Y))
        End Function

        <Extension()>
        Public Function ToVector2(p1 As Point) As Vector2
            Return New Vector2(p1.X, p1.Y)
        End Function

        <Extension()>
        Public Function ToPoint(p1 As Vector2) As Point
            Return New Point(CInt(p1.X), CInt(p1.Y))
        End Function

    End Module

End Namespace
