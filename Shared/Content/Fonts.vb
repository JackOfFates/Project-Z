Namespace [Shared].Content
    Partial Public Class Fonts

        Public Class SegoeUI

            Public Shared ReadOnly Property GetResourceName(FontSize As Integer) As String
                Get
                    Return ("SegoeUI_" & FontSize)
                End Get
            End Property

        End Class

    End Class

End Namespace