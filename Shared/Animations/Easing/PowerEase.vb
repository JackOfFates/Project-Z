Namespace [Shared].Animations.Easing

    Public Class PowerEase
        Inherits EaseFunction

        Public Property Power As Double = 2

        Public Overrides Function Ease(t As Double) As Double
            Select Case easeType
                Case Easing.EaseType.EaseIn
                    Return t ^ Power
                Case Easing.EaseType.EaseOut
                    Return t ^ (1 - (t ^ Power))
                Case Else
                    Return t
            End Select
        End Function

        Public Sub New(easeType As EaseType)
            MyBase.New(easeType)
        End Sub

        Public Sub New(easeType As EaseType, Power As Double)
            MyBase.New(easeType)
            Me.Power = Power
        End Sub

    End Class

End Namespace
