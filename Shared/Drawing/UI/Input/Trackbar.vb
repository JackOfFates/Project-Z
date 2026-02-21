Imports Microsoft.Xna.Framework
Imports ProjectZ.Shared.Animations.Properties
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Drawing.UI.Primitives

Namespace [Shared].Drawing.UI.Input
    <Serializable>
    Public Class Trackbar
        Inherits RectangleElement

#Region "Properties"

        Public Property ShowTooltip As Boolean = True

        Public Property Value As Double
            Get
                Return _Value
            End Get
            Set(value As Double)
                _Value = Math.Max(_MinimumValue, Math.Min(value, _MaximumValue))
                RaiseEvent ValueChanged(_Value)
            End Set
        End Property
        Private _Value As Double = 0

        Public Property MinimumValue As Double
            Get
                Return _MinimumValue
            End Get
            Set(value As Double)
                _MinimumValue = value
                Me._Value = Math.Max(_MinimumValue, value)
            End Set
        End Property
        Private _MinimumValue As Double = 0

        Public Property MaximumValue As Double
            Get
                Return _MaximumValue
            End Get
            Set(value As Double)
                _MaximumValue = value
                Me._Value = Math.Min(_MaximumValue, value)
            End Set
        End Property
        Private _MaximumValue As Double = 100

        Public Property Font As String
            Get
                Return _Font
            End Get
            Set(value As String)
                _Font = value
                SetInternalChildProperties()
            End Set
        End Property
        Private _Font As String = Content.Fonts.SegoeUI.GetResourceName(12)

        Public Property FixedInterval As Boolean = True

        Public Property ShowValue As Boolean = True

        Public Overridable Function GetValueDisplayText() As String
            If FixedInterval Then
                Return String.Format("{0:00}", {_Value})
            Else
                Return String.Format("{0:00.00}", {_Value})
            End If
        End Function

#End Region

#Region "Events"

        Public Event ValueChanged(value As Double)
#End Region

#Region "Event Handlers"

        Private Sub Trackbar_MouseMove(currentPoint As Point, lastPoint As Point)
            If MouseDownCheck() Then
                Dim RelativeX As Double = CDbl(currentPoint.X - (Position.X + Slider.Size.X / 2))
                Dim TranslatedValue As Double = Animations.DoubleAnimation.Interpolate(RelativeX, Padding.Left, Size.X - (Slider.Size.X + Padding.Left + Padding.Right), MinimumValue, MaximumValue)
                If FixedInterval Then
                    TranslatedValue = Math.Round(TranslatedValue)
                    If TranslatedValue Mod 2 <> 0 Then
                        Value = TranslatedValue - 1
                    Else
                        Value = TranslatedValue
                    End If
                Else
                    Value = TranslatedValue
                End If
            End If
        End Sub

        Private Sub SetInternalChildProperties() Handles Me.RectangleChanged, Me.ValueChanged

            ' Slider Bar
            SliderBar.Size = New Vector2(Size.Y / 4)

            ' Slider
            Dim TranslatedX As Double = Animations.DoubleAnimation.Interpolate(Value, MinimumValue, MaximumValue, 0, Size.X - (Slider.Size.X + Padding.Left + Padding.Right))
            If Double.IsNaN(TranslatedX) Then TranslatedX = 8
            Slider.Size = New Vector2(36)
            Slider.Margin = New Thickness(CInt(TranslatedX), CInt(Slider.Size.Y / 2 - Size.Y / 2), 0, 0)

            ' Display Text
            If DisplayText.isVisible Then
                Dim DisplayString As String = GetValueDisplayText()
                Dim DisplayTextSize As Vector2 = Scene.MeasureText(Font, DisplayString)
                DisplayText.Margin = New Thickness(Slider.Margin.Left + Math.Abs(CInt(DisplayTextSize.X / 2 - Slider.Size.X / 2)), CInt(-DisplayTextSize.Y), 0, 0)
                DisplayText.Text = DisplayString
            End If

        End Sub

#End Region

#Region "Child Elements"

        Friend WithEvents SliderBar As New RectangleElement(Scene, spriteBatch.SpriteBatch) With {.VerticalAlign = VerticalAlignment.Center,
                                                                                                  .HorizontalAlign = HorizontalAlignment.Stretch,
                                                                                                  .zIndex = -10}
        Friend WithEvents Slider As New RectangleElement(Scene, spriteBatch.SpriteBatch) With {.BackgroundColor = New Color(100, 100, 100),
                                                                                               .VerticalAlign = VerticalAlignment.Stretch,
                                                                                               .zIndex = -5}
        Friend WithEvents DisplayText As New TextElement(Scene, spriteBatch.SpriteBatch) With {.isMouseBypassEnabled = True,
                                                                                               .OrientationReserve = DisplayReservation.FloatBoth,
                                                                                               .zIndex = 0}

#End Region

#Region "Animation Properties"

        Public ValueProperty As New TrackbarValueProperty(Me)

#End Region

#Region "Constructors"

        Public Sub New(Scene As Scene)
            MyBase.New(Scene)
            Init()
            SetInternalChildProperties()
        End Sub

        Public Sub New(Scene As Scene, spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
            Init()
            SetInternalChildProperties()
        End Sub

        Public Sub New(Scene As Scene, newSpritebatch As Boolean)
            MyBase.New(Scene, newSpritebatch)
            Init()
            SetInternalChildProperties()
        End Sub

        Private Sub Init()
            BackgroundColor = Color.Transparent
            Padding = New Thickness(4)

            ' Keep internal parts ordered relative to the Trackbar itself
            SliderBar.zIndex = Me.zIndex - 10
            Slider.zIndex = Me.zIndex - 5
            DisplayText.zIndex = Me.zIndex

            Children.AddRange({SliderBar, Slider, DisplayText})
            AddHandler Scene.OnMouseMove, AddressOf Trackbar_MouseMove
        End Sub

#End Region

        Private Function MouseDownCheck() As Boolean
            Return isMouseDown Or SliderBar.isMouseDown Or Slider.isMouseDown
        End Function

        Protected Friend Overrides Sub Draw(gameTime As GameTime)
            If ShowValue AndAlso MouseDownCheck() Then
                DisplayText.isVisible = True
            Else
                DisplayText.isVisible = False
            End If
            MyBase.Draw(gameTime)
        End Sub

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
            RemoveHandler Scene.OnMouseMove, AddressOf Trackbar_MouseMove
        End Sub
    End Class

End Namespace