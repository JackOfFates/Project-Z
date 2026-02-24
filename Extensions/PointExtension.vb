Imports Microsoft.Xna.Framework
Imports System.Runtime.CompilerServices

Public Module PointExtension

    <Extension()>
    Public Function Subtract(p1 As Microsoft.Xna.Framework.Point, p2 As Microsoft.Xna.Framework.Point) As Microsoft.Xna.Framework.Point
        Return New Microsoft.Xna.Framework.Point(p1.X - p2.X, p1.Y - p2.Y)
    End Function

    <Extension()>
    Public Function Subtract(p1 As Microsoft.Xna.Framework.Point, p2 As Vector2) As Microsoft.Xna.Framework.Point
        Return New Microsoft.Xna.Framework.Point(CInt(p1.X - p2.X), CInt(p1.Y - p2.Y))
    End Function

    <Extension()>
    Public Function Subtract(p1 As Vector2, p2 As Microsoft.Xna.Framework.Point) As Vector2
        Return New Vector2(p1.X - p2.X, p1.Y - p2.Y)
    End Function

    <Extension()>
    Public Function Subtract(p1 As Vector2, p2 As Vector2) As Vector2
        Return New Vector2(p1.X - p2.X, p1.Y - p2.Y)
    End Function

    <Extension()>
    Public Function ToVector2(p1 As Microsoft.Xna.Framework.Point) As Vector2
        Return New Vector2(p1.X, p1.Y)
    End Function

    <Extension()>
    Public Function ToPoint(p1 As Vector2) As Microsoft.Xna.Framework.Point
        Return New Microsoft.Xna.Framework.Point(CInt(p1.X), CInt(p1.Y))
    End Function

End Module

