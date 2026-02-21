Imports System.Collections.Generic
Imports ProjectZ.Shared.Animations.Properties
Imports ProjectZ.Shared.Drawing.UI.Primitives
Imports ProjectZ.Shared.Animations
Imports Microsoft.Xna.Framework.Graphics
Imports Microsoft.Xna.Framework

Namespace [Shared].Drawing.UI.Advanced

    Public Class PolygonElement
        Inherits SceneElement

#Region "Properties"

        Public Property Vectors As Vector2()
            Get
                Return _vectorCache
            End Get
            Set(value As Vector2())
                ClearVectorPoints()
                AddVectorPoints(value)
            End Set
        End Property

        Public Property Segments As Integer()()
            Get
                Return _segmentCache
            End Get
            Set(value As Integer()())
                ClearSegmentPoints()
                AddSegments(value)
            End Set
        End Property

        Public ReadOnly Property MeshData As TriangleNet.Mesh
            Get
                Return _MeshData
            End Get
        End Property
        Private _MeshData As TriangleNet.Mesh
        Private _FallbackMeshData As TriangleNet.Mesh

        Friend Property Texture As Texture2D
            Get
                Return primitiveBatch.Texture
            End Get
            Set(value As Texture2D)
                primitiveBatch.Texture = value
            End Set
        End Property

        Public Property FillColor As Color
            Get
                Return FillColorCache
            End Get
            Set(value As Color)
                FillColorCache = value
            End Set
        End Property
        Private FillColorCache As Color

        Public Property Bounds As Vector2

        Public RenderProperties As New PolygonRenderProperties()

#End Region

#Region "Events"

        Public Event Trangulated(newMesh As TriangleNet.Mesh)

#End Region

#Region "Animation Properties"

        Public FillColorProperty As New FillColorProperty(Me)

#End Region

#Region "Internals"

        Private WhitePlain As Texture2D = Nothing
        Private wireFrameHighlightPrimitiveBatchList As New List(Of PrimitiveBatch)
        Private wireFramePrimitiveBatch As PrimitiveBatch
        Private primitiveBatch As PrimitiveBatch
        Private Triangulated As Boolean = False

#Region "Vector Methods"
        Private _vectors As New List(Of Vector2)
        Private _vectorCache As Vector2() = {}

        Public Function GetVectorPoint(Index As Integer) As Vector2
            Return Vectors(Index)
        End Function

        Public Sub SetVectorPoint(Index As Integer, Vector As Vector2)
            SyncLock _vectors
                _vectors(Index) = Vector
            End SyncLock
        End Sub

        Public Sub AddVectorPoint(Vector As Vector2)
            SyncLock _vectors
                _vectors.Add(Vector)
            End SyncLock
        End Sub

        Public Sub AddVectorPoints(Vectors As Vector2())
            If Vectors Is Nothing OrElse Vectors.Length < 3 Then Return

            Dim anyNonZero As Boolean = False
            Dim first As Vector2 = Vectors(0)
            Dim anyDifferent As Boolean = False
            For i As Integer = 0 To Vectors.Length - 1
                If Vectors(i).X <> 0 OrElse Vectors(i).Y <> 0 Then
                    anyNonZero = True
                End If
                If Not anyDifferent AndAlso (Vectors(i).X <> first.X OrElse Vectors(i).Y <> first.Y) Then
                    anyDifferent = True
                End If
            Next

            If Not anyNonZero OrElse Not anyDifferent Then
                Triangulated = False
                Return
            End If
            SyncLock _vectors
                ClearSegmentPoints()
                ClearVectorPoints()
                _vectors.AddRange(Vectors)
            End SyncLock
            SyncLock _segments
                Dim segmentOffset As Integer = _segments.Count
                boundaryMarkers.Add(_segments.Count)
                For i As Integer = 0 To Vectors.Length - 2
                    _segments.Add({i + segmentOffset, i + 1 + segmentOffset})
                Next
                _segments.Add({Vectors.Length - 1 + segmentOffset, segmentOffset})
            End SyncLock
        End Sub

        Public Sub RemoveVectorPoint(Vector As Vector2)
            SyncLock _vectors
                _vectors.Remove(Vector)
            End SyncLock
        End Sub

        Public Sub RemoveVectorPoints(Vectors As Vector2())
            SyncLock _vectors
                For i As Integer = Vectors.Length - 1 To 0 Step -1
                    _vectors.Remove(Vectors(i))
                Next
            End SyncLock
        End Sub

        Public Function ContainsVectorPoint(Vector As Vector2) As Boolean
            Return _vectors.Contains(Vector)
        End Function

        Public Sub ClearVectorPoints()
            SyncLock _vectors
                _vectors.Clear()
            End SyncLock
        End Sub

#End Region

#Region "Segment Methods"

        Private _segments As New List(Of Integer())
        Private _segmentCache As Integer()() = {}
        Private boundaryMarkers As New List(Of Integer)

        Public Function GetSegment(Index As Integer) As Integer()
            SyncLock _segments
                Return _segments(Index)
            End SyncLock
        End Function

        Public Sub SetSegment(Index As Integer, Segment As Integer())
            SyncLock _segments
                _segments(Index) = Segment
            End SyncLock
        End Sub

        Public Sub AddSegment(Segment As Integer())
            SyncLock _segments
                _segments.Add(Segment)
            End SyncLock
        End Sub

        Public Sub AddSegments(Segments As Integer()())
            SyncLock _segments
                For Each Segment As Integer() In Segments
                    _segments.Add(Segment)
                Next
            End SyncLock
        End Sub

        Public Sub RemoveSegment(Segment As Integer())
            SyncLock _segments
                _segments.Remove(Segment)
            End SyncLock
        End Sub

        Public Sub RemoveSegments(Segments As Integer()())
            SyncLock _segments
                For i As Integer = Segments.Length - 1 To 0 Step -1
                    _segments.Remove(Segments(i))
                Next
            End SyncLock
        End Sub

        Public Function ContainsSegment(Segment As Integer()) As Boolean
            Return _segments.Contains(Segment)
        End Function

        Public Sub ClearSegmentPoints()
            SyncLock _segments
                _segments.Clear()
            End SyncLock
            boundaryMarkers.Clear()
            Triangulated = False
        End Sub

#End Region

        Private Sub Triangulate()
            If _vectors.Count < 3 Then
                Return
            End If

            ' Convert _vectors to List(Of Vector2)
            Dim points As New List(Of Vector2)
            For Each v As Vector2 In _vectors
                points.Add(New Vector2(v.X, v.Y))
            Next

            ' Convert _segments to List(Of Tuple(Of Integer, Integer))
            ' Each segment is expected to be a pair of vertex indices: {fromIndex, toIndex}.
            Dim segments As New List(Of Tuple(Of Integer, Integer))
            For Each s As Integer() In _segments
                If s Is Nothing OrElse s.Length < 2 Then
                    Continue For
                End If

                Dim a As Integer = s(0)
                Dim b As Integer = s(1)
                If a < 0 OrElse b < 0 OrElse a >= points.Count OrElse b >= points.Count Then
                    Continue For
                End If

                segments.Add(Tuple.Create(a, b))
            Next
            ' Triangulate using TriangleNet
            Dim polygon As New TriangleNet.Geometry.Polygon()
            Dim vertices As New List(Of TriangleNet.Geometry.Vertex)(points.Count)

            For i As Integer = 0 To points.Count - 1
                Dim vtx As New TriangleNet.Geometry.Vertex(points(i).X, points(i).Y)
                vertices.Add(vtx)
                polygon.Add(vtx)
            Next

            For Each s In segments
                polygon.Add(New TriangleNet.Geometry.Segment(vertices(s.Item1), vertices(s.Item2)))
            Next

            Dim mesher As New TriangleNet.Meshing.GenericMesher()
            Dim mesh As TriangleNet.Mesh = CType(mesher.Triangulate(polygon), TriangleNet.Mesh)
            _MeshData = mesh
            _FallbackMeshData = mesh

            ' Calculate bounds
            Dim minX As Single = Single.MaxValue
            Dim minY As Single = Single.MaxValue
            Dim maxX As Single = Single.MinValue
            Dim maxY As Single = Single.MinValue
            For Each pt In points
                If pt.X < minX Then minX = pt.X
                If pt.Y < minY Then minY = pt.Y
                If pt.X > maxX Then maxX = pt.X
                If pt.Y > maxY Then maxY = pt.Y
            Next
            Bounds = New Vector2(maxX - minX, maxY - minY)

            Triangulated = True

            RaiseEvent Trangulated(_MeshData)
        End Sub

#End Region

#Region "Constructors"

        Public Sub New()
            MyBase.New()

        End Sub

        Public Shadows Sub UpdateScene(Scene As Scene)
            InitializePolygon({})
            WhitePlain = Content.Textures.CreateSolidTexture(Scene.graphicsDevice, Color.White)
        End Sub


        Public Sub New(Scene As Scene)
            MyBase.New(Scene, True)
            InitializePolygon({})
        End Sub

        Public Sub New(Scene As Scene, Vectors As Vector2())
            MyBase.New(Scene, True)
            InitializePolygon(Vectors)
        End Sub

        Public Sub New(Scene As Scene, Vectors As Vector2(), newSpriteBatch As Boolean)
            MyBase.New(Scene, newSpriteBatch)
            InitializePolygon(Vectors)
        End Sub

        Public Sub New(Scene As Scene, newSpriteBatch As Boolean)
            MyBase.New(Scene, newSpriteBatch)
            InitializePolygon({})
        End Sub

        Public Sub New(Scene As Scene, Vectors As Vector2(), spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
            InitializePolygon(Vectors)
        End Sub

        Public Sub New(Scene As Scene, spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
            InitializePolygon({})
        End Sub

        Private Sub InitializePolygon(Vectors As Vector2())

            Size = New Vector2(spriteBatch.GraphicsDevice.Viewport.Bounds.Width,
                               spriteBatch.GraphicsDevice.Viewport.Bounds.Height)

            primitiveBatch = New PrimitiveBatch(spriteBatch.GraphicsDevice,
                                                           WhitePlain,
                                                           Color.White, 1000)
            wireFramePrimitiveBatch = New PrimitiveBatch(spriteBatch.GraphicsDevice,
                                                           WhitePlain,
                                                           RenderProperties.WireFrameColor, Vectors.Length * 2)

            AddHandler RenderProperties.OnWireFrameColorChanged, Sub(c) wireFramePrimitiveBatch.Color = c
            Triangulated = False
            SyncLock _vectors
                _vectors.Clear()
                _vectorCache = _vectors.ToArray
            End SyncLock
            AddVectorPoints(Vectors)
            Triangulate()
        End Sub

#End Region

        Public Sub ApplyGeometryChanges()
            Triangulate()
        End Sub

        Protected Friend Overrides Sub Draw(gameTime As GameTime)

            If Not Triangulated Then Triangulate()
            If MeshData Is Nothing Then Return
            'Fill Normally
            primitiveBatch.Color = New Color(FillColorCache, FillColorCache.A)
            primitiveBatch.Begin(PrimitiveType.TriangleList)
            For Each tri In MeshData.Triangles
                For i As Integer = 0 To 2
                    Dim vt = tri.GetVertex(i)
                    Dim v As New Vector2(CSng(vt.X), CSng(vt.Y))
                    primitiveBatch.AddVertex(Vector2.Subtract(v, New Vector2(-Position.X, -Position.Y)))
                Next
            Next
            primitiveBatch.End()

        End Sub

        Protected Friend Overloads Function ContainsPoint(p As Vector2) As Boolean
            Dim RelativePoint As Vector2 = Vector2.Subtract(p, Position)
            If MeshData IsNot Nothing Then
                For Each tri In MeshData.Triangles
                    ' Simple point-in-triangle test
                    Return True
                    'If pts.Length = 3 Then
                    '    If PointInTriangle(RelativePoint, pts(0), pts(1), pts(2)) Then

                    '    End If
                    'End If
                Next
            End If
            Return False
        End Function

        Protected Friend Overloads Function ContainsPoint(p As Vector2, a As Vector2, b As Vector2, c As Vector2) As Boolean
            Dim px = p.X, py = p.Y
            Dim ax = a.X, ay = a.Y
            Dim bx = b.X, by = b.Y
            Dim cx = c.X, cy = c.Y
            Dim v0x = cx - ax, v0y = cy - ay
            Dim v1x = bx - ax, v1y = by - ay
            Dim v2x = px - ax, v2y = py - ay
            Dim dot00 = v0x * v0x + v0y * v0y
            Dim dot01 = v0x * v1x + v0y * v1y
            Dim dot02 = v0x * v2x + v0y * v2y
            Dim dot11 = v1x * v1x + v1y * v1y
            Dim dot12 = v1x * v2x + v1y * v2y
            Dim invDenom = 1 / (dot00 * dot11 - dot01 * dot01)
            Dim u = (dot11 * dot02 - dot01 * dot12) * invDenom
            Dim v = (dot00 * dot12 - dot01 * dot02) * invDenom
            Return (u >= 0) And (v >= 0) And (u + v < 1)
        End Function

    End Class

End Namespace