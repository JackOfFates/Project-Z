Imports Microsoft.Xna.Framework
Imports ProjectZ.Shared.Animations
Imports ProjectZ.Shared.Animations.Properties

Namespace [Shared].Animations

    Public Class Timeline

        Private ActiveAnimations As New Collections.Generic.Dictionary(Of AnimationBase, ElementProperty)
        Private InactiveAnimations As New Collections.Generic.List(Of AnimationBase)
        Private BindQueue As New Collections.Generic.List(Of Object())

        Public Property gameTime As GameTime

        Public Sub AddChild(Animation As AnimationBase, TargetProperty As ElementProperty)
            If Animation.From Is Nothing Then Animation.From = TargetProperty.GetValue
            BindQueue.Add({Animation, TargetProperty})
        End Sub

        Public Sub RemoveChild(Animation As AnimationBase)
            If isChild(Animation) AndAlso Not InactiveAnimations.Contains(Animation) Then
                InactiveAnimations.Add(Animation)
            End If
        End Sub

        Public Function isChild(Animation As AnimationBase) As Boolean
            Return ActiveAnimations.ContainsKey(Animation)
        End Function

        Public Sub Tick(gameTime As GameTime)

            ' Hack to enable the concurrent binding of animations
            For i As Integer = BindQueue.Count - 1 To 0 Step -1
                If BindQueue(i) Is Nothing Then Continue For
                Dim Animation As AnimationBase = CType(BindQueue(i)(0), AnimationBase)
                Dim TargetProperty As ElementProperty = CType(BindQueue(i)(1), ElementProperty)
                ActiveAnimations.Add(Animation, TargetProperty)
                BindQueue.RemoveAt(i)
            Next

            ' Continue the Animation
            For Each A As AnimationBase In ActiveAnimations.Keys
                If A.Running Then
                    Dim TargetProperty As ElementProperty = CType(ActiveAnimations(A), ElementProperty)
                    TargetProperty.SetValue(A.Value)
                    If Not A.Running Then A.RaiseOnFinished(Me)
                End If
            Next

            For i As Integer = InactiveAnimations.Count - 1 To 0 Step -1
                Dim A As AnimationBase = InactiveAnimations(i)
                ActiveAnimations.Remove(A)
                InactiveAnimations.RemoveAt(i)
            Next

        End Sub

        Public Sub New(gameTime As GameTime)
            Me.gameTime = gameTime
        End Sub
    End Class

End Namespace