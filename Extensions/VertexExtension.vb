Imports System.Runtime.CompilerServices
Imports Microsoft.Xna.Framework


Public Module VertexExtension

        <Extension()>
        Public Function Multiply(v As Vector2, d As Double) As Vector2
            Return New Vector2(CSng(v.X * d), CSng(v.Y * d))
        End Function

        <Extension()>
        Public Function Multiply(v As Vector2, x As Double, y As Double) As Vector2
            Return New Vector2(CSng(v.X * x), CSng(v.Y * y))
        End Function

    <Extension()>
    Public Function ToXNAVector2(v As TriangleNet.Geometry.Vertex) As Vector2
        Return New Vector2(CSng(v.X), CSng(v.Y))
    End Function

    <Extension()>
    Public Function ToXNAVectorArray(t As TriangleNet.Topology.Triangle) As Vector2()
        Dim P0 As Vector2 = t.GetVertex(0).ToXNAVector2
        Dim P1 As Vector2 = t.GetVertex(1).ToXNAVector2
        Dim P2 As Vector2 = t.GetVertex(2).ToXNAVector2
        Return {P0, P1, P2}
    End Function

    <Extension()>
    Public Function ToTriangleNetVertex(t As Vector2) As TriangleNet.Geometry.Vertex
        Return New TriangleNet.Geometry.Vertex(t.X, t.Y)
    End Function

End Module

