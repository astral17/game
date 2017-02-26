Public Module PlayerModule
    <Serializable()>
    Public Class PlayerClass
        Public version As String = "noInited"
        Public x, y As Integer
        Public stepCD As Integer
        Public money As Integer = 0
        Public gamestate As String = "normal"
        Public curWorld As String = "main"
        Public Function testVersion() As Boolean
            If MainForm.version = version Then
                Return True
            Else
                Return False
            End If
        End Function
    End Class
End Module
