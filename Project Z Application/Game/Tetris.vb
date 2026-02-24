Imports Zytonic_Framework.Utilities.Timers
Imports Zytonic_Framework.Math.Arithmetic
Imports Zytonic_Framework.Extentions
Imports System.Drawing
Imports Microsoft.Xna.Framework
Imports System.Collections.Generic

Public Class Tetris

#Region "Game Tick Events"

    Public Event NextBlockChanged(NextBlock As TetrisBlock)

    Public Sub Tick()
        Dim RowData As New Dictionary(Of Integer, Integer)

        For I As Integer = blockData.Count - 1 To 0 Step -1
            Dim TetrisBlock As TetrisBlock = blockData(I)

            'Increment Blocks Down
            If CanIncrement(TetrisBlock) Then
                TetrisBlock.Increment()
            Else
                TetrisBlock.HasReachedBottom = True
            End If

            'Check For Row Completion
            If (TetrisBlock.HasReachedBottom = True) Then
                For row As Integer = 0 To Me.Rows - 1
                    If RowData.ContainsKey(row) = False Then
                        RowData.Add(row, TetrisBlock.GetColumnCount(row))
                    Else
                        RowData(row) += TetrisBlock.GetColumnCount(row)
                        If RowData(row) = Me.Columns Then
                            RemoveRow(row)
                            GoTo ExitFor
                        End If
                    End If
                Next
            End If

        Next
ExitFor:

        'Spawn New Tetris Block
        SpawnNextBlock()
    End Sub

#End Region

#Region "Game Functions"
    Public Shared Function CalculateRectangle(point As Point, itemSize As Point) As Rectangle
        Dim X As Integer = point.X * itemSize.X
        Dim Y As Integer = point.Y * itemSize.Y
        Dim Width As Integer = itemSize.X
        Dim Height As Integer = itemSize.Y
        Return New Rectangle(X, Y, Width, Height)
    End Function

    Private Shared Function ResolvePoint(point As Point, itemSize As Point) As Point
        Dim X As Integer = CInt(point.X / itemSize.X)
        Dim Y As Integer = CInt(point.Y / itemSize.Y)
        Return New Point(X, Y)
    End Function

    Public Overloads Function CanIncrement(SelectedBlock As TetrisBlock) As Boolean
        Return CanIncrement(SelectedBlock, New Point(0, 1))
    End Function

    Public Overloads Function CanIncrement(SelectedBlock As TetrisBlock, Offset As Point) As Boolean
        'Loop through the selected block's Points
        For i As Integer = SelectedBlock.blockData.Count - 1 To 0 Step -1
            Dim sBlock As Point = SelectedBlock.blockData(i)
            'Loop through all other block's Point Data
            For Index As Integer = blockData.Count - 1 To 0 Step -1
                Dim TetrisBlock As TetrisBlock = blockData(Index)
                'Make sure we are not checking it against itself
                If TetrisBlock IsNot SelectedBlock Then
                    'Loop through all Points in TetrisBlock
                    For Each Block As Point In TetrisBlock.blockData
                        'Check if it intersects with any other blocks
                        If sBlock.Y + Offset.Y = Block.Y And sBlock.X = Block.X Then
                            Return False
                        ElseIf sBlock.Y = Block.Y And sBlock.X + Offset.X = Block.X Then
                            Return False
                        End If
                    Next
                ElseIf TetrisBlock Is SelectedBlock Then
                    'Check if it goes out of the grid
                    If sBlock.X + Offset.X < 0 Then
                        Return False
                    ElseIf sBlock.X + Offset.X > Columns - 1 Then
                        Return False
                    ElseIf sBlock.Y + Offset.Y < 0 Then
                        Return False
                    ElseIf sBlock.Y + Offset.Y > Rows - 1 Then
                        Return False
                    End If
                End If
            Next
        Next
        Return True
    End Function

    Public Function CanRotate(sBlock As TetrisBlock, Direction As TetrisBlock.Direction) As Boolean
        Dim S As Point = sBlock.CalculateSize()
        Dim nOffset As Point = sBlock.Offset(True), pOffset As Point = sBlock.Offset()
        If Direction = TetrisBlock.Direction.Left Then
            For i As Integer = sBlock.blockData.Count - 1 To 0 Step -1
                If sBlock.blockData(i).Y <= 1 Then Return False
                Dim P As New Point(sBlock.blockData(i).X + S.X, sBlock.blockData(i).Y + S.Y)
                P = P - (nOffset)
                P = New Point(P.Y, S.X - P.X)
                P = P - (pOffset)
                If sBlock.Shape = TetrisBlock.BlockType.i Then
                    If P.X > Columns + 2 Or P.Y > Rows - S.Y Then
                        Return False
                    End If
                Else
                    If P.X > Columns + S.X Or P.Y > Rows - S.Y Then
                        Return False
                    End If
                End If
                For Each Block As TetrisBlock In blockData
                    If Block IsNot sBlock Then
                        For Each point As Point In Block.blockData
                            If P = point Then Return False
                        Next
                    End If
                Next
            Next
        ElseIf Direction = TetrisBlock.Direction.Right Then
            For i As Integer = 0 To sBlock.blockData.Count - 1
                If sBlock.blockData(i).Y <= 1 Then Return False
                Dim P As New Point(sBlock.blockData(i).X - S.X, sBlock.blockData(i).Y - S.Y)
                P = P - (nOffset)
                P = New Point(S.Y - P.Y, P.X)
                P = P - (pOffset)
                If sBlock.Shape = TetrisBlock.BlockType.i Then
                    If P.X > Columns + 2 Or P.Y > Rows - S.Y Then
                        Return False
                    End If
                Else
                    If P.X > Columns + S.X Or P.Y > Rows - S.Y Then
                        Return False
                    End If
                End If
                For Each Block As TetrisBlock In blockData
                    If Block IsNot sBlock Then
                        For Each point As Point In Block.blockData
                            If P = point Then Return False
                        Next
                    End If
                Next
            Next
        End If
        Return True
    End Function

    Private Function CanSpawn() As Boolean
        For Each B As TetrisBlock In blockData
            If B.HasReachedBottom = False Then Return False
        Next
        Return True
    End Function

    Public Function GetCurrentBlock() As TetrisBlock
        For Each TetrisBlock As TetrisBlock In blockData
            If TetrisBlock.HasReachedBottom = False Then
                Return TetrisBlock
            End If
        Next
        Return Nothing
    End Function

    Private Sub GenerateSpawnList()
        spawnList.Clear()
        Dim R As New Random
        For i As Integer = 0 To 53
            Dim RandomNum As TetrisBlock.BlockType = CType(R.Next(0, 6), TetrisBlock.BlockType)
            Dim RandomColor As Color = New Color(255, R.Next(0, 255), R.Next(0, 255), R.Next(0, 255))
            Dim T As New TetrisBlock(RandomNum, RandomColor)
            spawnList.Add(T)
        Next
    End Sub

    Private Sub SpawnNextBlock()
        If CanSpawn() AndAlso spawnList.Count > 0 Then
            blockData.Add(spawnList(0))
            spawnList.RemoveAt(0)
            RaiseEvent NextBlockChanged(spawnList(0))
        ElseIf spawnList.Count = 0 Then
            blockData.Clear()
            spawnList.Clear()
            GenerateSpawnList()
        End If
    End Sub

    Private Sub RemoveRow(Row As Integer)
        'Remove The Row
        For Each B As TetrisBlock In blockData
            If B.HasReachedBottom Then
                For i As Integer = B.blockData.Count - 1 To 0 Step -1
                    Dim p As Point = B.blockData(i)
                    If p.Y = Row Then B.blockData.Remove(p)
                Next
            End If
        Next
        'Offset Blocks to fill the gap
        For Each B As TetrisBlock In blockData
            If B.HasReachedBottom Then
                For Each P As Point In B.blockData
                    If P.Y < Row Then
                        B.Increment(False)
                        Exit For
                    End If
                Next
            End If
        Next
    End Sub

#End Region

#Region "Game Properties"

    ''' <summary>
    ''' Grid Column Constant
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Columns As Integer = 12

    ''' <summary>
    ''' Grid Row Constant
    ''' </summary>
    ''' <remarks></remarks>
    Public Property Rows As Integer = 24

    ''' <summary>
    ''' Block Properties
    ''' </summary>
    ''' <remarks></remarks>
    Public Property blockData As New List(Of TetrisBlock)

    ''' <summary>
    ''' The calculated size of the blocks
    ''' </summary>
    ''' <remarks></remarks>
    Public Property blockSize As New Point(CInt(250 / Columns), CInt(300 / Rows))

    ''' <summary>
    ''' The calculated size of the grid
    ''' </summary>
    ''' <remarks></remarks>
    Public Property gridSize As Point

    ''' <summary>
    ''' The Tetris Block Spawn List
    ''' </summary>
    ''' <remarks></remarks>
    Public Property spawnList As New List(Of TetrisBlock)

#End Region

    Public Sub New(GridSize As Point)
        Me.gridSize = GridSize
        GenerateSpawnList()
    End Sub

End Class