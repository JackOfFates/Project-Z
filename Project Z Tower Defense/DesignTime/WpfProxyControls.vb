Imports System.Windows
Imports System.Windows.Controls
Imports System.Windows.Media
Imports System.Windows.Shapes
Imports System.ComponentModel

Namespace DesignTime

    ''' <summary>
    ''' WPF proxy controls that render in Visual Studio designer to preview Project Z scenes.
    ''' At runtime, SceneXamlParser creates actual Project Z controls from the XAML.
    ''' </summary>
    ''' <remarks>
    ''' These controls have the same properties as Project Z controls so XAML written for
    ''' the designer will work correctly when parsed by SceneXamlParser at runtime.
    ''' </remarks>

#Region "Scene"

    ''' <summary>
    ''' Design-time Scene container - renders as a dark canvas.
    ''' </summary>
    Public Class Scene
        Inherits Canvas

        Public Shared ReadOnly BackgroundColorProperty As DependencyProperty =
            DependencyProperty.Register("BackgroundColor", GetType(String), GetType(Scene),
                New PropertyMetadata("#000000", AddressOf OnBackgroundColorChanged))

        Public Property BackgroundColor As String
            Get
                Return CStr(GetValue(BackgroundColorProperty))
            End Get
            Set(value As String)
                SetValue(BackgroundColorProperty, value)
            End Set
        End Property

        Private Shared Sub OnBackgroundColorChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim scene = DirectCast(d, Scene)
            scene.UpdateBackground()
        End Sub

        Private Sub UpdateBackground()
            Try
                Me.Background = DirectCast(New BrushConverter().ConvertFromString(BackgroundColor), Brush)
            Catch
                Me.Background = Brushes.Black
            End Try
        End Sub

        Public Sub New()
            Me.Background = Brushes.Black
            Me.ClipToBounds = True
        End Sub
    End Class

#End Region

#Region "RectangleElement"

    ''' <summary>
    ''' Design-time RectangleElement - renders as a WPF Border.
    ''' </summary>
    Public Class RectangleElement
        Inherits Border

        Public Shared ReadOnly XProperty As DependencyProperty =
            DependencyProperty.Register("X", GetType(Double), GetType(RectangleElement),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly YProperty As DependencyProperty =
            DependencyProperty.Register("Y", GetType(Double), GetType(RectangleElement),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Property X As Double
            Get
                Return CDbl(GetValue(XProperty))
            End Get
            Set(value As Double)
                SetValue(XProperty, value)
            End Set
        End Property

        Public Property Y As Double
            Get
                Return CDbl(GetValue(YProperty))
            End Get
            Set(value As Double)
                SetValue(YProperty, value)
            End Set
        End Property

        Private Shared Sub OnPositionChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, RectangleElement)
            Canvas.SetLeft(element, element.X)
            Canvas.SetTop(element, element.Y)
        End Sub

        Public Sub New()
            Me.Background = New SolidColorBrush(Color.FromRgb(50, 50, 50))
            Canvas.SetLeft(Me, 0)
            Canvas.SetTop(Me, 0)
        End Sub
    End Class

#End Region

#Region "CircleElement"

    ''' <summary>
    ''' Design-time CircleElement - renders as a WPF Ellipse inside a Canvas.
    ''' </summary>
    Public Class CircleElement
        Inherits Canvas

        Private ReadOnly _ellipse As Ellipse

        Public Shared ReadOnly XProperty As DependencyProperty =
            DependencyProperty.Register("X", GetType(Double), GetType(CircleElement),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly YProperty As DependencyProperty =
            DependencyProperty.Register("Y", GetType(Double), GetType(CircleElement),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly FillProperty As DependencyProperty =
            DependencyProperty.Register("Fill", GetType(String), GetType(CircleElement),
                New PropertyMetadata("#FFFFFF", AddressOf OnFillChanged))

        Public Property X As Double
            Get
                Return CDbl(GetValue(XProperty))
            End Get
            Set(value As Double)
                SetValue(XProperty, value)
            End Set
        End Property

        Public Property Y As Double
            Get
                Return CDbl(GetValue(YProperty))
            End Get
            Set(value As Double)
                SetValue(YProperty, value)
            End Set
        End Property

        Public Property Fill As String
            Get
                Return CStr(GetValue(FillProperty))
            End Get
            Set(value As String)
                SetValue(FillProperty, value)
            End Set
        End Property

        Private Shared Sub OnPositionChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, CircleElement)
            Canvas.SetLeft(element, element.X)
            Canvas.SetTop(element, element.Y)
        End Sub

        Private Shared Sub OnFillChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, CircleElement)
            element.UpdateFill()
        End Sub

        Private Sub UpdateFill()
            Try
                _ellipse.Fill = DirectCast(New BrushConverter().ConvertFromString(Fill), Brush)
            Catch
                _ellipse.Fill = Brushes.White
            End Try
        End Sub

        Public Sub New()
            _ellipse = New Ellipse()
            _ellipse.Fill = Brushes.White
            Me.Children.Add(_ellipse)

            AddHandler Me.SizeChanged, Sub(s, e)
                                           _ellipse.Width = Me.Width
                                           _ellipse.Height = Me.Height
                                       End Sub
        End Sub
    End Class

#End Region

#Region "TextElement"

    ''' <summary>
    ''' Design-time TextElement - renders as a WPF TextBlock.
    ''' </summary>
    Public Class TextElement
        Inherits Canvas

        Private ReadOnly _textBlock As TextBlock

        Public Shared ReadOnly XProperty As DependencyProperty =
            DependencyProperty.Register("X", GetType(Double), GetType(TextElement),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly YProperty As DependencyProperty =
            DependencyProperty.Register("Y", GetType(Double), GetType(TextElement),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly TextProperty As DependencyProperty =
            DependencyProperty.Register("Text", GetType(String), GetType(TextElement),
                New PropertyMetadata("", AddressOf OnTextChanged))

        Public Shared ReadOnly ForegroundColorProperty As DependencyProperty =
            DependencyProperty.Register("Foreground", GetType(String), GetType(TextElement),
                New PropertyMetadata("#FFFFFF", AddressOf OnForegroundChanged))

        Public Property X As Double
            Get
                Return CDbl(GetValue(XProperty))
            End Get
            Set(value As Double)
                SetValue(XProperty, value)
            End Set
        End Property

        Public Property Y As Double
            Get
                Return CDbl(GetValue(YProperty))
            End Get
            Set(value As Double)
                SetValue(YProperty, value)
            End Set
        End Property

        Public Property Text As String
            Get
                Return CStr(GetValue(TextProperty))
            End Get
            Set(value As String)
                SetValue(TextProperty, value)
            End Set
        End Property

        Public Shadows Property Foreground As String
            Get
                Return CStr(GetValue(ForegroundColorProperty))
            End Get
            Set(value As String)
                SetValue(ForegroundColorProperty, value)
            End Set
        End Property

        Private Shared Sub OnPositionChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, TextElement)
            Canvas.SetLeft(element, element.X)
            Canvas.SetTop(element, element.Y)
        End Sub

        Private Shared Sub OnTextChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, TextElement)
            element._textBlock.Text = CStr(e.NewValue)
        End Sub

        Private Shared Sub OnForegroundChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, TextElement)
            Try
                element._textBlock.Foreground = DirectCast(New BrushConverter().ConvertFromString(CStr(e.NewValue)), Brush)
            Catch
                element._textBlock.Foreground = Brushes.White
            End Try
        End Sub

        Public Sub New()
            _textBlock = New TextBlock()
            _textBlock.Foreground = Brushes.White
            _textBlock.FontFamily = New FontFamily("Segoe UI")
            _textBlock.FontSize = 14
            Me.Children.Add(_textBlock)
        End Sub
    End Class

#End Region

#Region "Button"

    ''' <summary>
    ''' Design-time Button - renders as a styled WPF Border with text.
    ''' </summary>
    Public Class Button
        Inherits Canvas

        Private ReadOnly _border As Border
        Private ReadOnly _textBlock As TextBlock

        Public Shared ReadOnly XProperty As DependencyProperty =
            DependencyProperty.Register("X", GetType(Double), GetType(Button),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly YProperty As DependencyProperty =
            DependencyProperty.Register("Y", GetType(Double), GetType(Button),
                New PropertyMetadata(0.0, AddressOf OnPositionChanged))

        Public Shared ReadOnly ContentProperty As DependencyProperty =
            DependencyProperty.Register("Content", GetType(String), GetType(Button),
                New PropertyMetadata("Button", AddressOf OnContentChanged))

        Public Shared ReadOnly BackgroundColorProperty As DependencyProperty =
            DependencyProperty.Register("Background", GetType(String), GetType(Button),
                New PropertyMetadata("#323232", AddressOf OnBackgroundChanged))

        Public Shared ReadOnly ForegroundColorProperty As DependencyProperty =
            DependencyProperty.Register("Foreground", GetType(String), GetType(Button),
                New PropertyMetadata("#FFFFFF", AddressOf OnForegroundChanged))

        Public Property X As Double
            Get
                Return CDbl(GetValue(XProperty))
            End Get
            Set(value As Double)
                SetValue(XProperty, value)
            End Set
        End Property

        Public Property Y As Double
            Get
                Return CDbl(GetValue(YProperty))
            End Get
            Set(value As Double)
                SetValue(YProperty, value)
            End Set
        End Property

        Public Property Content As String
            Get
                Return CStr(GetValue(ContentProperty))
            End Get
            Set(value As String)
                SetValue(ContentProperty, value)
            End Set
        End Property

        Public Shadows Property Background As String
            Get
                Return CStr(GetValue(BackgroundColorProperty))
            End Get
            Set(value As String)
                SetValue(BackgroundColorProperty, value)
            End Set
        End Property

        Public Shadows Property Foreground As String
            Get
                Return CStr(GetValue(ForegroundColorProperty))
            End Get
            Set(value As String)
                SetValue(ForegroundColorProperty, value)
            End Set
        End Property

        Private Shared Sub OnPositionChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, Button)
            Canvas.SetLeft(element, element.X)
            Canvas.SetTop(element, element.Y)
        End Sub

        Private Shared Sub OnContentChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, Button)
            element._textBlock.Text = CStr(e.NewValue)
        End Sub

        Private Shared Sub OnBackgroundChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, Button)
            Try
                element._border.Background = DirectCast(New BrushConverter().ConvertFromString(CStr(e.NewValue)), Brush)
            Catch
                element._border.Background = New SolidColorBrush(Color.FromRgb(50, 50, 50))
            End Try
        End Sub

        Private Shared Sub OnForegroundChanged(d As DependencyObject, e As DependencyPropertyChangedEventArgs)
            Dim element = DirectCast(d, Button)
            Try
                element._textBlock.Foreground = DirectCast(New BrushConverter().ConvertFromString(CStr(e.NewValue)), Brush)
            Catch
                element._textBlock.Foreground = Brushes.White
            End Try
        End Sub

        Public Sub New()
            _border = New Border()
            _border.Background = New SolidColorBrush(Color.FromRgb(50, 50, 50))
            _border.BorderBrush = New SolidColorBrush(Color.FromRgb(80, 80, 80))
            _border.BorderThickness = New Thickness(1)
            _border.CornerRadius = New CornerRadius(3)

            _textBlock = New TextBlock()
            _textBlock.Text = "Button"
            _textBlock.Foreground = Brushes.White
            _textBlock.FontFamily = New FontFamily("Segoe UI")
            _textBlock.FontSize = 14
            _textBlock.HorizontalAlignment = HorizontalAlignment.Center
            _textBlock.VerticalAlignment = VerticalAlignment.Center

            _border.Child = _textBlock
            Me.Children.Add(_border)

            AddHandler Me.SizeChanged, Sub(s, e)
                                           _border.Width = Me.Width
                                           _border.Height = Me.Height
                                       End Sub
        End Sub
    End Class

#End Region

End Namespace
