Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Animations.Properties

Namespace [Shared].Drawing.UI.Primitives

    <Serializable>
    Public Class RectangleElement
        Inherits SceneElement

#Region "Properties"

        Protected Friend Texture As Texture2D
        Public Overridable Property BackgroundColor As New Color(60, 60, 60)
        Public Overridable Property isResizable As AlignmentType = AlignmentType.None

        Protected Friend Property Color As Color
            Get
                Return _Color
            End Get
            Set(value As Color)
                _Color = value
                Try
                    Texture = Content.Textures.CreateSolidTexture(Scene.graphicsDevice, _Color)
                Catch ex As Exception
                End Try
            End Set
        End Property
        Private _Color As Color = Color.White

#End Region

#Region "Animation Properties"

        Friend BackgroundProperty As New BackgroundColorProperty(Me)

#End Region

#Region "Constructors"

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(Scene As Scene)
            MyBase.New(Scene)
            Try
                Texture = Content.Textures.CreateSolidTexture(Scene.graphicsDevice, Color.White)
            Catch ex As Exception

            End Try
        End Sub

        Public Sub New(Scene As Scene, spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
            Texture = Content.Textures.CreateSolidTexture(Scene.graphicsDevice, Color.White)
        End Sub

        Public Sub New(Scene As Scene, newSpritebatch As Boolean)
            MyBase.New(Scene, newSpritebatch)
            Texture = Content.Textures.CreateSolidTexture(Scene.graphicsDevice, Color.White)
        End Sub

#End Region

        Protected Friend Overrides Sub Draw(gameTime As Microsoft.Xna.Framework.GameTime)
            If Texture Is Nothing Then Texture = Content.Textures.CreateSolidTexture(Scene.graphicsDevice, Color.White)
            spriteBatch.Draw(Texture, Rectangle, BackgroundColor)
        End Sub

        Private Sub RectangleElement_MouseMove(currentPoint As Point, lastPoint As Point) Handles Me.MouseMove
            alignment(currentPoint, lastPoint)
        End Sub

        Private Sub alignment(currentPoint As Point, lastPoint As Point)
            If isResizable = AlignmentType.Horizontal Then
                If currentPoint.X >= Size.X - 4 Then
                    EnableResizeCursor(AlignmentType.Horizontal, 1)
                ElseIf currentPoint.X <= 4 Then
                    EnableResizeCursor(AlignmentType.Horizontal, -1)
                Else
                    Scene.ChangeCursorType(CursorType.Default)
                End If
            ElseIf isResizable = AlignmentType.Vertical Then
                If currentPoint.Y >= Size.Y - 4 Then
                    EnableResizeCursor(AlignmentType.Vertical, 1)
                ElseIf currentPoint.Y <= 4 Then
                    EnableResizeCursor(AlignmentType.Vertical, -1)
                End If
            Else
                Scene.ChangeCursorType(CursorType.Default)
            End If
        End Sub

        Private Sub EnableResizeCursor(type As AlignmentType, location As Integer)
            If location < 0 AndAlso type = AlignmentType.Horizontal Then
                Scene.ChangeCursorType(CursorType.ResizeLeft)

            ElseIf location < 0 AndAlso type = AlignmentType.Vertical Then
                Scene.ChangeCursorType(CursorType.ResizeTop)
            End If
            If location > 0 AndAlso type = AlignmentType.Horizontal Then
                Scene.ChangeCursorType(CursorType.ResizeRight)
            ElseIf location > 0 AndAlso type = AlignmentType.Vertical Then
                Scene.ChangeCursorType(CursorType.ResizeBottom)
            End If
        End Sub

        Private Sub RectangleElement_Loaded() Handles Me.Loaded

        End Sub
    End Class

End Namespace
