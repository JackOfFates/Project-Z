Namespace [Shared].Drawing

    Public Class Thickness

        Public Sub New(Left As Integer, Top As Integer, Right As Integer, Bottom As Integer)
            _Left = Left
            _Top = Top
            _Right = Right
            _Bottom = Bottom
        End Sub

        Public Sub New(UniformLength As Integer)
            _Left = UniformLength
            _Top = UniformLength
            _Bottom = UniformLength
            _Right = UniformLength
        End Sub


        Public Sub New()
            Dim UniformLength As Integer = 3
            _Left = UniformLength
            _Top = UniformLength
            _Bottom = UniformLength
            _Right = UniformLength
        End Sub

        Public Property Left As Integer = 0
        Public Property Top As Integer = 0
        Public Property Right As Integer = 0
        Public Property Bottom As Integer = 0

    End Class

End Namespace