Imports Microsoft.Xna.Framework
Imports ProjectZ.Shared.Drawing
Imports ProjectZ.Shared.Drawing.UI
Imports ProjectZ.Shared.Drawing.UI.Advanced
Imports ProjectZ.Shared.Drawing.UI.Input
Imports ProjectZ.Shared.Drawing.UI.Primitives

Namespace [Shared].Animations.Properties

#Region "Property Classes"

    Public Class WidthProperty
        Inherits ElementProperty

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(TargetElement As SceneElement)
            MyBase.New(TargetElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return TargetElement.Size.X
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            TargetElement.Size = New Vector2(CSng(Value), CInt(TargetElement.Size.Y))
        End Sub
    End Class
    Public Class HeightProperty
        Inherits ElementProperty

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(TargetElement As SceneElement)
            MyBase.New(TargetElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return TargetElement.Size.Y
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            TargetElement.Size = New Vector2(CInt(TargetElement.Size.X), CSng(Value))
        End Sub
    End Class

    Public Class LeftProperty
        Inherits ElementProperty

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(TargetElement As SceneElement)
            MyBase.New(TargetElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return TargetElement.Position.X
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            TargetElement.Position = New Vector2(CSng(Value), CInt(TargetElement.Position.Y))
        End Sub
    End Class
    Public Class TopProperty
        Inherits ElementProperty

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByRef TargetElement As SceneElement)
            MyBase.New(TargetElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return TargetElement.Position.Y
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            TargetElement.Position = New Vector2(TargetElement.Position.X, CSng(Value))
        End Sub
    End Class

    Public Class BackgroundColorProperty
        Inherits ElementProperty

        Private CastedElement As RectangleElement

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByRef TargetElement As SceneElement)
            MyBase.New(TargetElement)
            CastedElement = DirectCast(TargetElement, RectangleElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.BackgroundColor
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.BackgroundColor = CType(Value, Color)
        End Sub
    End Class
    Public Class ForegroundColorProperty
        Inherits ElementProperty

        Private CastedElement As TextElement

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByRef TargetElement As TextElement)
            MyBase.New(TargetElement)
            CastedElement = CType(TargetElement, TextElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.ForegroundColor
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.ForegroundColor = CType(Value, Color)
        End Sub
    End Class
    Public Class FillColorProperty
        Inherits ElementProperty

        Private CastedElement As PolygonElement

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByRef TargetElement As SceneElement)
            MyBase.New(TargetElement)
            CastedElement = DirectCast(TargetElement, PolygonElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.FillColor
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.FillColor = CType(Value, Color)
        End Sub
    End Class

    Public Class MouseOverBackgroundColorProperty
        Inherits ElementProperty

        Private CastedElement As Button

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByRef TargetElement As SceneElement)
            MyBase.New(TargetElement)
            CastedElement = DirectCast(TargetElement, Button)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.MouseOverBackgroundColor
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.MouseOverBackgroundColor = CType(Value, Color)
        End Sub
    End Class
    Public Class MouseDownBackgroundColorProperty
        Inherits ElementProperty

        Private CastedElement As Button

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(ByRef TargetElement As SceneElement)
            MyBase.New(TargetElement)
            CastedElement = DirectCast(TargetElement, Button)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.MouseDownBackgroundColor
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.MouseDownBackgroundColor = CType(Value, Color)
        End Sub
    End Class

    Public Class SpriteProgressProperty
        Inherits ElementProperty

        Private CastedElement As SpriteElement

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(TargetElement As SceneElement)
            MyBase.New(TargetElement)
            CastedElement = DirectCast(TargetElement, SpriteElement)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.CurrentFrame
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.CurrentFrame = CInt(Value)
        End Sub
    End Class

    Public Class TrackbarValueProperty
        Inherits ElementProperty

        Private CastedElement As Trackbar

        Public Sub New()
            MyBase.New()
        End Sub

        Public Sub New(TargetElement As SceneElement)
            MyBase.New(TargetElement)
            CastedElement = DirectCast(TargetElement, Trackbar)
        End Sub

        Protected Friend Overrides Function GetValue() As Object
            Return CastedElement.Value
        End Function

        Protected Friend Overrides Sub SetValue(Value As Object)
            CastedElement.Value = CDbl(Value)
        End Sub
    End Class

#End Region

    Public MustInherit Class ElementProperty

        Friend TargetElement As SceneElement

        Protected Friend MustOverride Function GetValue() As Object

        Protected Friend MustOverride Sub SetValue(Value As Object)

        Public Sub New(TargetElement As SceneElement)
            Me.TargetElement = TargetElement
        End Sub

        Public Sub New()
        End Sub

    End Class

End Namespace
