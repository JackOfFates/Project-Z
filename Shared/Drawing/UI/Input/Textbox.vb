Imports Microsoft.Xna.Framework
Imports ProjectZ.Shared.Animations.Properties
Imports Microsoft.Xna.Framework.Input
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Animations.Easing
Imports ProjectZ.Shared.Animations
Imports ProjectZ.Shared.Drawing.UI.Primitives

Imports System.Diagnostics
Imports ProjectZ.Shared.XNA
Imports ProjectZ.Shared.Drawing.UI.Advanced
Imports System.Collections.Generic

Namespace [Shared].Drawing.UI.Input
    <Serializable>
    Public Class Textbox
        Inherits RectangleElement

#Region "Properties"

        Public Property TextPadding As Vector2
            Get
                Return -_TextPadding
            End Get
            Set(value As Vector2)
                _TextPadding = -value
            End Set
        End Property
        Private _TextPadding As New Vector2(-2, -1)
        Public Property HorizontalTextAlignment As HorizontalAlignment
            Get
                Return _HorizontalTextAlignment
            End Get
            Set(value As HorizontalAlignment)
                _HorizontalTextAlignment = CType(value, HorizontalAlignment)
                UpdateTextbox()
            End Set
        End Property
        Private _HorizontalTextAlignment As HorizontalAlignment = HorizontalAlignment.Left
        Public Property VerticalTextAlignment As VerticalAlignment
            Get
                Return _VerticalTextAlignment
            End Get
            Set(value As VerticalAlignment)
                _VerticalTextAlignment = CType(value, VerticalAlignment)
                UpdateTextbox()
            End Set
        End Property
        Private _VerticalTextAlignment As VerticalAlignment = VerticalAlignment.Top
        Public Property AcceptsReturn As Boolean = True
        Public Property SelectionLength As Integer
            Get
                Return _SelectionLength
            End Get
            Set(value As Integer)
                _SelectionLength = value
                UpdateTextbox()
            End Set
        End Property
        Private _SelectionLength As Integer = 0
        Public Property SelectionStart As Integer
            Get
                Return _SelectionStart
            End Get
            Set(value As Integer)
                _SelectionStart = Math.Max(value, 0)
                UpdateTextbox()
            End Set
        End Property
        Private _SelectionStart As Integer = 0
        Public Property CaretPosition As Integer
            Get
                Return _CaretPosition
            End Get
            Set(value As Integer)
                _CaretPosition = Math.Min(Text.Length, Math.Max(0, value))
                UpdateTextbox()
            End Set
        End Property
        Private _CaretPosition As Integer = 0
        Public Property Text As String
            Get
                Return _Text
            End Get
            Set(value As String)
                RaiseEvent OnTextChanged(New TextChangedEventArgs(_Text, value))
                _Text = value
                UpdateTextbox()
            End Set
        End Property
        Private _Text As String = String.Empty
        Public Property Font As String
            Get
                Return _Font
            End Get
            Set(value As String)
                _Font = value
                _FontCharHeight = Scene.MeasureText(_Font, "A").Y
                UpdateTextbox()
            End Set
        End Property
        Private _Font As String = Content.Fonts.SegoeUI.GetResourceName(12)
        Private _FontCharHeight As Single = 0
        Public Property ForegroundColor As Color
            Get
                Return _ForegroundColor
            End Get
            Set(value As Color)
                _ForegroundColor = value
                TextElement.ForegroundColor = _ForegroundColor
                Caret.BackgroundColor = _ForegroundColor
            End Set
        End Property
        Private _ForegroundColor As Color = Color.White
        Public Overrides Property BackgroundColor As Color
            Get
                Return _BackgroundColor
            End Get
            Set(value As Color)
                _BackgroundColor = value
            End Set
        End Property
        Private _BackgroundColor As New Color(50, 50, 50)

#End Region

#Region "Internals"

        Private LastCaretPos As Single = 0
        Private canNotUpdate As Boolean = False
        Private Function GetCaretPosition() As Vector2
            Dim ResolvedCaretPoint As Point = TextElement.CharIndexToPoint(CaretPosition)

            Dim v As New Vector2(ResolvedCaretPoint.X, ResolvedCaretPoint.Y)

            If LastCaretPos <> v.Y Then
                Dim diff As Single = v.Y - LastCaretPos
                LastCaretPos = v.Y
            End If

            Return v
        End Function

        Private Function GetSelectionVectors() As Vector2()
            Dim Vectors As New List(Of Vector2)
            Dim ResolvedStartPoint As Vector2 = TextElement.CharIndexToPoint(SelectionStart).ToVector2.Subtract(-TextPadding)
            Dim CurrentPosition As New Vector2(ResolvedStartPoint.X, ResolvedStartPoint.Y)



            Return Vectors.ToArray
        End Function

        Private Function GetAlignment(RelativeTo As SceneElement) As Vector2
            Dim RelativeSizeY As Single = Math.Max(_FontCharHeight, RelativeTo.Size.Y)
            Select Case VerticalTextAlignment
                Case VerticalAlignment.Center
                    GetAlignment.Y = (Size.Y / 2 - RelativeSizeY / 2)
                Case VerticalAlignment.Bottom
                    GetAlignment.Y = (Size.Y - RelativeSizeY)
                Case Else
                    GetAlignment.Y = 0
            End Select
            Select Case HorizontalTextAlignment
                Case HorizontalAlignment.Center
                    GetAlignment.X = (Size.X / 2 - RelativeTo.Size.X / 2)
                Case HorizontalAlignment.Right
                    GetAlignment.X = (Size.X - RelativeTo.Size.X)
                Case Else
                    GetAlignment.X = 0
            End Select
        End Function

        Private Function GetLastLine() As String
            Dim LastReturn As Integer = Text.LastIndexOf(vbLf)
            Return If(LastReturn < 0, Nothing, Text.Remove(0, Text.LastIndexOf(vbLf)))
        End Function

        Private Function IsNewLineNeeded(widthControl As Single, text As String) As Boolean
            Dim textSize As Vector2 = Scene.MeasureText(Font, text)

            If textSize.X > widthControl - 20 Then
                Return True
            Else
                Return False
            End If

        End Function

        Private Sub UpdateTextbox() Handles Me.RectangleChanged
            If canNotUpdate Then Return

            canNotUpdate = True

            ' Set Text Properties
            TextElement.Text = Text
            TextElement.Font = Font
            TextElement.Position = GetAlignment(TextElement)

            ' Set Caret Properties
            Caret.Position = GetCaretPosition()
            Caret.Size = New Vector2(1, Scene.MeasureText(Font, "A").Y)
            Caret.BackgroundColor = Color.Transparent

            ' Set Selection Properties
            Selection.ClearVectorPoints()
            If SelectionLength <> 0 Then
                Selection.AddVectorPoints(GetSelectionVectors())
            End If

            canNotUpdate = False

        End Sub

#End Region

#Region "Events"

        Public Event OnPreTextInput(e As PreTextInputEventArgs)

        Public Event OnTextChanged(e As TextChangedEventArgs)

#End Region

#Region "Animation Properties"

        Public ForegroundProperty As ForegroundColorProperty

#Region "Animation Instances"
        Private WithEvents CaretFadeInAnimation As ColorAnimation
        Private WithEvents CaretFadeOutAnimation As ColorAnimation

        Private Sub ShowCaret()
            If CaretFadeOutAnimation IsNot Nothing Then
                If CaretFadeOutAnimation.Running Then
                    CaretFadeOutAnimation.Stop()
                End If
            End If
            If CaretFadeInAnimation Is Nothing Then
                CaretFadeInAnimation = New ColorAnimation(
                    New SineEase(EaseType.EaseInOut),
                    Caret.BackgroundColor, _ForegroundColor,
                    TimeSpan.FromSeconds(0.5), Scene.gameTime)
                BindAnimation(Caret.BackgroundProperty, CaretFadeInAnimation)
            End If
            CaretFadeInAnimation.Start()
        End Sub

        Private Sub HideCaret()
            If CaretFadeInAnimation IsNot Nothing Then
                If CaretFadeInAnimation.Running Then
                    CaretFadeInAnimation.Stop()
                End If
            End If
            If CaretFadeOutAnimation Is Nothing Then
                CaretFadeOutAnimation = New ColorAnimation(
                    New SineEase(EaseType.EaseInOut),
                     Color.Transparent, Color.Transparent,
                    TimeSpan.FromSeconds(0.25), Scene.gameTime)
                BindAnimation(Caret.BackgroundProperty, CaretFadeOutAnimation)
            End If
            CaretFadeOutAnimation.Start()
        End Sub

        Private Sub CaretFadeOutAnimation_OnAnimationFinished(sender As Object) Handles CaretFadeOutAnimation.OnAnimationFinished
            If CanSelect And isSelected Then
                ShowCaret()
            End If
        End Sub

        Private Sub CaretFadeInAnimation_OnAnimationFinished(sender As Object) Handles CaretFadeInAnimation.OnAnimationFinished
            HideCaret()
        End Sub

#End Region

#End Region

#Region "Child Elements"

        Protected Friend TextElement As New TextElement(Scene, spriteBatch.SpriteBatch)
        Protected Friend Caret As New RectangleElement(Scene, spriteBatch.SpriteBatch)
        Protected Friend Selection As New PolygonElement(Scene, {}, spriteBatch.SpriteBatch)

#End Region

#Region "Constructors"

        Public Sub New(Scene As Scene)
            MyBase.New(Scene, True)

            BackgroundProperty = New BackgroundColorProperty(Me)
            ForegroundProperty = New ForegroundColorProperty(TextElement)
            CanSelect = True
            _FontCharHeight = Scene.MeasureText(_Font, "A").Y

            Selection.spriteBatch.Settings = New SpriteBatchPropertySet(SpriteSortMode.Immediate, BlendState.Additive)
#If WINDOWS Then
            Selection.FillColor = New Color(0, 0, 1, 0.4)
#ElseIf LINUX Then
            Selection.FillColor = New Color(0, 0, 1, 102)
#End If
            spriteBatch.Settings = New SpriteBatchPropertySet(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                                                             Nothing, Nothing,
                                                             New RasterizerState() With {.ScissorTestEnable = True})
            Children.AddRange({TextElement, Caret, Selection})
            Clip = True
        End Sub

#End Region

        ' Disable automatic spriteBatch Initialization
        Protected Friend Overrides Sub doDraw(gameTime As GameTime)

            ' Draw Background normally
            If isElementSpriteBatch Then
                spriteBatch.Begin()
                MyBase.Draw(gameTime)
                spriteBatch.End()
            Else
                MyBase.Draw(gameTime)
            End If

            spriteBatch.Begin()
            Draw(gameTime)
            spriteBatch.End()
        End Sub

        Public Overrides Sub Tick(gameTime As GameTime)
            MyBase.Tick(gameTime)
        End Sub

        Private Sub Textbox_OnKeyPress(Key As Keys, KeyboardState As KeyboardState) Handles Me.OnKeyPress
            Dim PreInputEvent As New PreTextInputEventArgs(Key, KeyboardState)
            RaiseEvent OnPreTextInput(PreInputEvent)
            If Not PreInputEvent.Cancel Then
                Dim control As Boolean = (KeyboardState.IsKeyDown(Keys.LeftControl) Or KeyboardState.IsKeyDown(Keys.RightControl))
                Dim Execute As Boolean = True

                If control Then
                    Select Case Key
                        Case Keys.A
                            SelectionStart = 0
                            SelectionLength = Text.Length
                            CaretPosition = SelectionStart + SelectionLength
                            Execute = False
                    End Select
                End If

                If Execute Then
                    If Key = Keys.Enter AndAlso Not AcceptsReturn Then Return
                    If SelectionLength + SelectionStart > Text.Length Then
                        SelectionStart = 0
                        SelectionLength = 0
                    End If
                    Select Case Key
                        Case Keys.Back
                            If SelectionLength <> 0 Then
                                Text = Text.Remove(SelectionStart, Math.Min(SelectionLength, Text.Length))
                                CaretPosition = SelectionStart
                            Else
                                If Text.Length > 0 And CaretPosition > 0 Then
                                    Dim CharCount As Integer = If(Text(CaretPosition - 1) = vbLf, 2, 1)
                                    Text = Text.Remove(CaretPosition - CharCount, CharCount)
                                    CaretPosition -= CharCount
                                End If
                            End If
                        Case Keys.Delete
                            If SelectionLength <> 0 Then
                                Text = Text.Remove(SelectionStart, Math.Min(SelectionLength, Text.Length))
                                CaretPosition = SelectionStart
                            Else
                                If Text.Length > 0 And CaretPosition < Text.Length Then
                                    Dim CharCount As Integer = If(Text(CaretPosition) = vbLf, 2, 1)
                                    Text = Text.Remove(CaretPosition, CharCount)
                                End If
                            End If
                        Case Keys.Left
                            If CaretPosition = 0 Then Exit Select
                            If SelectionLength <> 0 Then
                                SelectionLength = 0
                            End If
                            If KeyboardState.IsKeyDown(Keys.LeftControl) Or KeyboardState.IsKeyDown(Keys.RightControl) Then
                                CaretPosition = SelectionStart
                                Exit Select
                            Else
                                CaretPosition -= If(Text(CaretPosition - 1) = vbLf, 2, 1)
                            End If
                        Case Keys.Right
                            If Text = Nothing Then Exit Select
                            If SelectionLength <> 0 Then
                                SelectionLength = 0
                            End If
                            If KeyboardState.IsKeyDown(Keys.LeftControl) Or KeyboardState.IsKeyDown(Keys.RightControl) Then
                                CaretPosition = Text.Length
                                Exit Select
                            ElseIf CaretPosition < Text.Length Then
                                Dim CharCount As Integer = If(Text(Math.Min(Text.Length, CaretPosition)) = vbCr, 2, 1)
                                CaretPosition += CharCount
                            End If
                        Case Else
                            Dim newInput As String = SceneManager.TryConvertKeyboardInput(Key, KeyboardState)
                            If newInput <> String.Empty Then
                                If SelectionLength <> 0 Then
                                    Text = Text.Remove(SelectionStart, Math.Min(SelectionLength, Text.Length))
                                    CaretPosition = SelectionStart
                                    Text = Text.Insert(CaretPosition, newInput)
                                    CaretPosition += newInput.Length
                                    SelectionStart = 0
                                    SelectionLength = 0
                                Else
                                    Dim LastLine As String = GetLastLine()

                                    If LastLine <> Nothing AndAlso IsNewLineNeeded(Size.X, LastLine) Then
                                        newInput &= vbLf
                                    End If
                                    Text = Text.Insert(Math.Min(CaretPosition, Text.Length), newInput)
                                    CaretPosition += newInput.Length
                                End If
                            End If
                    End Select
                End If

            End If
        End Sub

        Private Sub Textbox_OnMouseLeftClick(p As Point) Handles Me.MouseLeftClick
            Dim RelativePoint As Point = RelativeTo(TextElement)
            Dim FixedPoint As New Point(RelativePoint.X + p.X + 3, RelativePoint.Y + p.Y)
            CaretPosition = TextElement.PointToCharIndex(FixedPoint)
            SelectionStart = 0
            SelectionLength = 0
        End Sub

        Private Sub Textbox_OnMouseDrag(currentPoint As Point, startPoint As Point) Handles Me.MouseDrag
            Dim RelativePoint As Point = RelativeTo(TextElement)
            Dim FixedStartPoint As New Point(RelativePoint.X + startPoint.X + 3, RelativePoint.Y + startPoint.Y)
            Dim StartIndex As Integer = TextElement.PointToCharIndex(FixedStartPoint)

            Dim FixedEndPoint As New Point(RelativePoint.X + currentPoint.X + 3, RelativePoint.Y + currentPoint.Y)
            Dim EndIndex As Integer = TextElement.PointToCharIndex(FixedEndPoint)

            If StartIndex > EndIndex Then
                SelectionStart = EndIndex
                CaretPosition = StartIndex
                SelectionLength = StartIndex - EndIndex
            Else
                SelectionStart = StartIndex
                CaretPosition = EndIndex
                SelectionLength = EndIndex - StartIndex
            End If

        End Sub

        Private Sub Textbox_OnSelect() Handles Me.Selected
            ShowCaret()
        End Sub

        Private Sub Textbox_OnDeselect() Handles Me.Deselected
            HideCaret()
        End Sub

    End Class

    Public Class PreTextInputEventArgs

        Public ReadOnly Property Key As Keys
            Get
                Return _Key
            End Get
        End Property
        Private _Key As Keys

        Public ReadOnly Property KeyboardState As KeyboardState
            Get
                Return _KeyboardState
            End Get
        End Property
        Private _KeyboardState As KeyboardState

        Public Property Cancel As Boolean = False

        Public Sub New(Key As Keys, KeyboardState As KeyboardState)
            _Key = Key
            _KeyboardState = KeyboardState
        End Sub

    End Class

    Public Class TextChangedEventArgs

        Public ReadOnly Property OldText As String
            Get
                Return _OldText
            End Get
        End Property
        Private _OldText As String

        Public ReadOnly Property NewText As String
            Get
                Return _NewText
            End Get
        End Property
        Private _NewText As String

        Public Sub New(Oldtext As String, NewText As String)
            _OldText = Oldtext
            _NewText = NewText
        End Sub

    End Class

End Namespace