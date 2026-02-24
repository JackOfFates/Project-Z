Imports Microsoft.Xna.Framework
Imports System.Collections.Generic

Public Class TetrisBlock

#Region "Block Definitions"
    Private j_Block As New List(Of Point) From {New Point(0, 2), New Point(1, 0), New Point(1, 1), New Point(1, 2)}
    Private l_Block As New List(Of Point) From {New Point(0, 0), New Point(0, 1), New Point(0, 2), New Point(1, 2)}
    Private s_Block As New List(Of Point) From {New Point(0, 1), New Point(1, 1), New Point(1, 0), New Point(2, 0)}
    Private z_Block As New List(Of Point) From {New Point(0, 0), New Point(1, 1), New Point(1, 0), New Point(2, 1)}
    Private o_Block As New List(Of Point) From {New Point(0, 0), New Point(0, 1), New Point(1, 0), New Point(1, 1)}
    Private t_Block As New List(Of Point) From {New Point(0, 0), New Point(1, 0), New Point(2, 0), New Point(1, 1)}
    Private i_Block As New List(Of Point) From {New Point(0, 0), New Point(0, 1), New Point(0, 2), New Point(0, 3)}
#End Region

    Public Property Shape As BlockType = BlockType.i
    Public Property blockData As New List(Of Point)
    Public Property Columns As Integer = 12
    Public Property Rows As Integer = 24
    Public Property Color As Color = Color.Red
    Public Property HasReachedBottom As Boolean = False
    Dim isLeft As Boolean = False, isRight As Boolean = False
    Public Sub New(Block As BlockType, Color As Color)
        Me.Color = Color
        Shape = Block
        CreateShape()
    End Sub
    Public Sub New(Block As BlockType, Color As Color, GridSize As Point)
        Columns = GridSize.X
        Rows = GridSize.Y
        Me.Color = Color
        Shape = Block
        CreateShape()
    End Sub
    Private Sub CreateShape()
        Dim BlockPoints As Point()
        Select Case Shape
            Case BlockType.i
                BlockPoints = i_Block.ToArray
            Case BlockType.j
                BlockPoints = j_Block.ToArray
            Case BlockType.l
                BlockPoints = l_Block.ToArray
            Case BlockType.o
                BlockPoints = o_Block.ToArray
            Case BlockType.s
                BlockPoints = s_Block.ToArray
            Case BlockType.t
                BlockPoints = t_Block.ToArray
            Case BlockType.z
                BlockPoints = z_Block.ToArray
        End Select

        AddBlockData(BlockPoints)
    End Sub
    Private Sub AddBlockData(Points As Point())
        For Each P As Point In Points
            P = P + (New Point(CInt(Math.Floor(Columns / 2) - 1), 0))
            blockData.Add(P)
        Next
    End Sub
    Public Overloads Sub Increment()
        Increment(True)
    End Sub
    Public Overloads Sub Increment(CheckForBottom As Boolean)
        For i As Integer = blockData.Count - 1 To 0 Step -1
            Dim P As New Point(blockData(i).X, blockData(i).Y)
            P = P + (New Point(0, 1))
            If Not pointIsBottom(P) AndAlso (HasReachedBottom = False Or CheckForBottom = False) Then
                blockData(i) = P
            ElseIf pointIsBottom(P) Then
                If CheckForBottom Then
                    HasReachedBottom = True
                    Exit For
                End If
            End If
        Next
    End Sub
    Public Sub Rotate(Direction As Direction)
        If Shape = BlockType.o Then Exit Sub
        Dim S As Point = CalculateSize()
        Dim nOffset As Point = Offset(True), pOffset As Point = Offset()
        If Direction = TetrisBlock.Direction.Left Then
            For i As Integer = blockData.Count - 1 To 0 Step -1
                DoLeftRotation(i, S, nOffset, pOffset)
            Next
            'If Shape = BlockType.i Then
            '    If isLeft Then
            '        isLeft = False
            '        blockData.Reverse()
            '    Else
            '        isLeft = True
            '    End If
            'ElseIf Shape = BlockType.j Then
            '    If isLeft Then
            '        isLeft = False
            '    Else
            '        isLeft = True
            '        blockData.Reverse()
            '    End If
            'Else
            '    If isLeft Then
            '        isLeft = False
            '    Else
            '        isLeft = True
            '        blockData.Reverse()
            '    End If
            'End If
        ElseIf Direction = TetrisBlock.Direction.Right Then
            For i As Integer = 0 To blockData.Count - 1
                DoRightRotation(i, S, nOffset, pOffset)
            Next
            'If isRight Then
            '    isRight = False
            '    blockData.Reverse()
            'Else
            '    isRight = True
            'End If
        End If
    End Sub
    Private Sub DoLeftRotation(I As Integer, S As Point, nOffset As Point, pOffset As Point)
        Dim P As Point = blockData(I)
        P = P + (nOffset)
        P = New Point(P.Y, S.X - P.X)
        P = P + (pOffset)
        blockData(I) = P
    End Sub
    Private Sub DoRightRotation(I As Integer, S As Point, nOffset As Point, pOffset As Point)
        Dim P As Point = blockData(I)
        P = P + (nOffset)
        P = New Point(S.Y - P.Y, P.X)
        P = P + (pOffset)
        blockData(I) = P
    End Sub
    Public Function CalculateSize() As Point
        Dim ClosestY As Integer = Rows, FurthestY As Integer = 0, ClosestX As Integer = Rows, FurthestX As Integer = 0
        For Each Block As Point In blockData
            If Block.Y < ClosestY Then ClosestY = Block.Y
            If Block.Y > FurthestY Then FurthestY = Block.Y
            If Block.X < ClosestX Then ClosestX = Block.X
            If Block.X > FurthestX Then FurthestX = Block.X
        Next
        Dim Height As Integer = (FurthestY - ClosestY)
        Dim Width As Integer = (FurthestX - ClosestX)
        Return New Point(Width, Height)
    End Function
    Public Overloads Function Offset() As Point
        Return Offset(False)
    End Function
    Public Overloads Function Offset(Negative As Boolean) As Point
        Dim ClosestY As Integer = Rows, ClosestX As Integer = Columns
        For Each Block As Point In blockData
            If Block.Y < ClosestY Then ClosestY = Block.Y
            If Block.X < ClosestX Then ClosestX = Block.X
        Next
        If Negative = True Then
            Return New Point(ClosestX * -1, ClosestY * -1)
        Else
            Return New Point(ClosestX, ClosestY)
        End If
    End Function
    Public Function OffsetFromBottom() As Point
        Dim FurthestY As Integer = 0, FurthestX As Integer = 0
        For Each Block As Point In blockData
            If Block.Y > FurthestY Then FurthestY = Block.Y
            If Block.X > FurthestX Then FurthestX = Block.X
        Next
        Return New Point(Columns - FurthestX, Rows - FurthestY)
    End Function
    Public Overloads Sub Move(Direction As Direction)
        If HasReachedBottom = False Then
            Select Case Direction
                Case Direction.Left
                    For i As Integer = 0 To blockData.Count - 1
                        Dim p As Point = blockData(i), o As New Point(-1, 0)
                        p = p + (o)
                        blockData(i) = p
                    Next
                Case Direction.Right
                    For i As Integer = 0 To blockData.Count - 1
                        Dim p As Point = blockData(i), o As New Point(1, 0)
                        p = p + (o)
                        blockData(i) = p
                    Next
                Case Direction.Down
                    For i As Integer = 0 To blockData.Count - 1
                        Dim p As Point = blockData(i), o As New Point(0, 1)
                        p = p + (o)
                        blockData(i) = p
                    Next
            End Select
        End If
    End Sub
    Public Overloads Sub Move(Direction As Direction, RowNumber As Integer)
        Select Case Direction
            Case Direction.Up
                For i As Integer = 0 To blockData.Count - 1
                    Dim p As Point = blockData(i), o As New Point(-1, 0)
                    If p.Y = RowNumber Then
                        p = p + (o)
                        blockData(i) = p
                    End If
                Next
            Case Direction.Down
                For i As Integer = 0 To blockData.Count - 1
                    Dim p As Point = blockData(i), o As New Point(1, 0)
                    If p.Y = RowNumber Then
                        p = p + (o)
                        blockData(i) = p
                    End If
                Next

        End Select
    End Sub
    Public Function GetColumnCount(Row As Integer) As Integer
        Dim Result As Integer = 0
        For Each p As Point In blockData
            If p.Y = Row Then Result += 1
        Next
        Return Result
    End Function
    Private Function pointIsBottom(P As Point) As Boolean
        If P.Y > Rows - 1 Then
            Return True
        Else
            Return False
        End If
    End Function
    Public Enum Direction
        Left
        Right
        Up
        Down
    End Enum
    Public Enum BlockType
        i
        j
        l
        o
        s
        t
        z
    End Enum
End Class