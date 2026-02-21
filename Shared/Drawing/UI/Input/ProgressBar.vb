Imports Microsoft.Xna.Framework
Imports Microsoft.Xna.Framework.Graphics
Imports ProjectZ.Shared.Content
Imports ProjectZ.Shared.Drawing.UI.Primitives

Namespace [Shared].Drawing.UI.Input

    ''' <summary>
    ''' A progress bar control similar to WPF ProgressBar.
    ''' Displays progress as a filled portion of a bar.
    ''' </summary>
    <Serializable>
    Public Class ProgressBar
        Inherits RectangleElement

#Region "Properties"

        ''' <summary>
        ''' Gets or sets the current value of the progress bar.
        ''' </summary>
        Public Property Value As Double
            Get
                Return _Value
            End Get
            Set(value As Double)
                _Value = Math.Max(_Minimum, Math.Min(value, _Maximum))
                UpdateProgress()
                RaiseEvent ValueChanged(_Value)
            End Set
        End Property
        Private _Value As Double = 0

        ''' <summary>
        ''' Gets or sets the minimum value.
        ''' </summary>
        Public Property Minimum As Double
            Get
                Return _Minimum
            End Get
            Set(value As Double)
                _Minimum = value
                UpdateProgress()
            End Set
        End Property
        Private _Minimum As Double = 0

        ''' <summary>
        ''' Gets or sets the maximum value.
        ''' </summary>
        Public Property Maximum As Double
            Get
                Return _Maximum
            End Get
            Set(value As Double)
                _Maximum = value
                UpdateProgress()
            End Set
        End Property
        Private _Maximum As Double = 100

        ''' <summary>
        ''' Gets or sets whether the progress bar is indeterminate (shows animation without specific progress).
        ''' </summary>
        Public Property IsIndeterminate As Boolean
            Get
                Return _IsIndeterminate
            End Get
            Set(value As Boolean)
                _IsIndeterminate = value
                UpdateProgress()
            End Set
        End Property
        Private _IsIndeterminate As Boolean = False

        ''' <summary>
        ''' Gets or sets the fill color of the progress indicator.
        ''' </summary>
        Public Property FillColor As Color
            Get
                Return _FillColor
            End Get
            Set(value As Color)
                _FillColor = value
                ProgressFill.BackgroundColor = _FillColor
            End Set
        End Property
        Private _FillColor As New Color(0, 120, 215)

        ''' <summary>
        ''' Gets or sets the track (background) color.
        ''' </summary>
        Public Property TrackColor As Color
            Get
                Return _TrackColor
            End Get
            Set(value As Color)
                _TrackColor = value
                BackgroundColor = _TrackColor
            End Set
        End Property
        Private _TrackColor As New Color(40, 40, 40)

        ''' <summary>
        ''' Gets or sets the border color.
        ''' </summary>
        Public Property BorderColor As Color
            Get
                Return _BorderColor
            End Get
            Set(value As Color)
                _BorderColor = value
                BorderElement.BackgroundColor = _BorderColor
            End Set
        End Property
        Private _BorderColor As New Color(80, 80, 80)

        ''' <summary>
        ''' Gets or sets the border thickness.
        ''' </summary>
        Public Property BorderThickness As Single = 1.0F

        ''' <summary>
        ''' Gets or sets whether to show the percentage text.
        ''' </summary>
        Public Property ShowPercentage As Boolean
            Get
                Return _ShowPercentage
            End Get
            Set(value As Boolean)
                _ShowPercentage = value
                PercentageText.isVisible = _ShowPercentage
                UpdateProgress()
            End Set
        End Property
        Private _ShowPercentage As Boolean = False

        ''' <summary>
        ''' Gets or sets the font for the percentage text.
        ''' </summary>
        Public Property Font As String
            Get
                Return _Font
            End Get
            Set(value As String)
                _Font = value
                PercentageText.Font = _Font
            End Set
        End Property
        Private _Font As String = Fonts.SegoeUI.GetResourceName(10)

        ''' <summary>
        ''' Gets the current progress percentage (0-100).
        ''' </summary>
        Public ReadOnly Property Percentage As Double
            Get
                If _Maximum = _Minimum Then Return 0
                Return ((_Value - _Minimum) / (_Maximum - _Minimum)) * 100.0
            End Get
        End Property

#End Region

#Region "Events"

        ''' <summary>
        ''' Raised when the Value property changes.
        ''' </summary>
        Public Event ValueChanged(value As Double)

#End Region

#Region "Child Elements"

        Private BorderElement As RectangleElement
        Private ProgressFill As RectangleElement
        Private PercentageText As TextElement

        ' For indeterminate animation
        Private _indeterminateOffset As Single = 0
        Private ReadOnly _indeterminateWidth As Single = 0.3F ' 30% of bar width

#End Region

#Region "Constructors"

        Public Sub New(Scene As Scene)
            MyBase.New(Scene)
            Init()
        End Sub

        Public Sub New(Scene As Scene, spriteBatch As SpriteBatch)
            MyBase.New(Scene, spriteBatch)
            Init()
        End Sub

        Public Sub New(Scene As Scene, newSpritebatch As Boolean)
            MyBase.New(Scene, newSpritebatch)
            Init()
        End Sub

        Private Sub Init()
            BackgroundColor = _TrackColor

            ' Create child elements after base constructor has set Scene and spriteBatch
            BorderElement = New RectangleElement(Scene, spriteBatch.SpriteBatch) With {
                .isMouseBypassEnabled = True,
                .BackgroundColor = _BorderColor
            }

            ProgressFill = New RectangleElement(Scene, spriteBatch.SpriteBatch) With {
                .isMouseBypassEnabled = True,
                .BackgroundColor = _FillColor
            }

            PercentageText = New TextElement(Scene, spriteBatch.SpriteBatch) With {
                .isMouseBypassEnabled = True,
                .HorizontalAlign = HorizontalAlignment.Center,
                .VerticalAlign = VerticalAlignment.Center,
                .ForegroundColor = Color.White,
                .isVisible = False
            }

            Children.Add(BorderElement)
            Children.Add(ProgressFill)
            Children.Add(PercentageText)
            UpdateProgress()
        End Sub

#End Region

#Region "Methods"

        Private Sub UpdateProgress() Handles Me.RectangleChanged
            ' Skip if child elements are not yet initialized
            If BorderElement Is Nothing Then Return

            ' Update border
            BorderElement.Position = Vector2.Zero
            BorderElement.Size = Size

            ' Calculate fill width
            Dim innerX As Single = BorderThickness
            Dim innerWidth As Single = Size.X - (BorderThickness * 2)
            Dim innerHeight As Single = Size.Y - (BorderThickness * 2)

            If _IsIndeterminate Then
                ' Indeterminate: animated sliding bar
                Dim barWidth As Single = innerWidth * _indeterminateWidth
                ProgressFill.Position = New Vector2(innerX + _indeterminateOffset * innerWidth, BorderThickness)
                ProgressFill.Size = New Vector2(barWidth, innerHeight)
            Else
                ' Determinate: fill based on value
                Dim fillRatio As Single = CSng((_Value - _Minimum) / Math.Max(1, _Maximum - _Minimum))
                fillRatio = Math.Max(0, Math.Min(1, fillRatio))
                Dim fillWidth As Single = innerWidth * fillRatio

                ProgressFill.Position = New Vector2(innerX, BorderThickness)
                ProgressFill.Size = New Vector2(fillWidth, innerHeight)
            End If

            ' Update percentage text
            If _ShowPercentage AndAlso Not _IsIndeterminate Then
                PercentageText.Text = String.Format("{0:0}%", Percentage)
                PercentageText.Position = New Vector2(
                    (Size.X - PercentageText.Size.X) / 2.0F,
                    (Size.Y - PercentageText.Size.Y) / 2.0F
                )
            End If
        End Sub

        Public Overrides Sub Tick(gameTime As GameTime)
            MyBase.Tick(gameTime)
            
            ' Animate indeterminate progress
            If _IsIndeterminate Then
                Dim deltaSeconds As Single = CSng(gameTime.ElapsedGameTime.TotalSeconds)
                _indeterminateOffset += deltaSeconds * 0.5F ' Speed of animation
                If _indeterminateOffset > 1.0F + _indeterminateWidth Then
                    _indeterminateOffset = -_indeterminateWidth
                End If
                UpdateProgress()
            End If
        End Sub

        ''' <summary>
        ''' Increases the value by the specified amount.
        ''' </summary>
        Public Sub Increment(Optional amount As Double = 1)
            Value = _Value + amount
        End Sub

        ''' <summary>
        ''' Decreases the value by the specified amount.
        ''' </summary>
        Public Sub Decrement(Optional amount As Double = 1)
            Value = _Value - amount
        End Sub

#End Region

    End Class

End Namespace
