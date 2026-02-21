Namespace [Shared].Animations.Easing

    Public MustInherit Class EaseFunction

        Public MustOverride Function Ease(t As Double) As Double

        Public Property easeType As EaseType

        Public Sub New(easeType As EaseType)
            Me.easeType = easeType
        End Sub
    End Class

End Namespace