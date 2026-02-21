Namespace [Shared].Animations.Easing

    Public Class SineEase
        Inherits EaseFunction

        Public Overrides Function Ease(t As Double) As Double
            Select Case EaseType
                Case Easing.EaseType.EaseIn
                    Return 1 - (Math.Sin(1 - t) * (Math.PI / 2))
                Case Easing.EaseType.EaseOut
                    Return Math.Sin(t * Math.Max(Math.PI - (Math.PI / 2), 0))
                Case Easing.EaseType.EaseInOut
                    Return (Math.Sin(t * Math.PI - (Math.PI / 2)) + 1) / 2
                Case Else
                    Return t
            End Select
        End Function

        Public Sub New(easeType As EaseType)
            MyBase.New(easeType)
        End Sub

    End Class

End Namespace
