Imports Microsoft.Xna.Framework
Imports ProjectZ.Shared.Animations.Easing

Namespace [Shared].Animations

    Public Class ColorAnimation
        Inherits AnimationBase

        Public Sub New(EaseFunction As EaseFunction, [From] As Color, [To] As Color, Duration As TimeSpan, gameTime As GameTime)
            Me.Init(EaseFunction, From, [To], Duration, gameTime, False)
        End Sub

        Public Sub New(EaseFunction As EaseFunction, [From] As Color, [To] As Color, Duration As TimeSpan, gameTime As GameTime, Autostart As Boolean)
            Me.Init(EaseFunction, From, [To], Duration, gameTime, Autostart)
        End Sub

        Public Overrides Function Value(t As Double) As Object
            If Running Then
                lastValue = Color.Lerp(CType(From, Color), CType([To], Color), CSng(easeFunction.Ease(t)))
                Return lastValue
            Else
                Return lastValue
            End If
        End Function

    End Class

End Namespace