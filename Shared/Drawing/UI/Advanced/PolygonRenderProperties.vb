Imports Microsoft.Xna.Framework

Namespace [Shared].Drawing.UI.Advanced

    Public Class PolygonRenderProperties

        Public Event OnWireFrameColorChanged(c As Color)

        Public Property WireFrame As Boolean = False

        Public Property FillPolygon As Boolean = False

        Public Property HighlightOnMouseOver As Boolean = False

        Public Property HighlightColor As Color = Color.Red

        Public Property HighQuality As Boolean = False

        Public Property MinAngle As Double = 20

        Public Property WireFrameColor As Color
            Get
                Return _WireFrameColor
            End Get
            Set(value As Color)
                _WireFrameColor = value
                RaiseEvent OnWireFrameColorChanged(_WireFrameColor)
            End Set
        End Property
        Private _WireFrameColor As Color = Color.Cyan

    End Class

End Namespace