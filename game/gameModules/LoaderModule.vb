Public Module LoaderModule
    <Serializable()>
    Public Class LoaderClass
        Public loaded As New List(Of String)
        Public bW As New List(Of Integer)
        Public bH As New List(Of Integer)
        Public bX As New List(Of Integer)
        Public bY As New List(Of Integer)
        Public Function isLoaded(ByVal name As String) As Boolean
            For i As Integer = 0 To loaded.Count - 1
                If name = loaded(i) Then
                    Return True
                End If
            Next
            Return False
        End Function
        Public Function getNumLoaded(ByVal name As String) As Integer
            For i As Integer = 0 To loaded.Count - 1
                If name = loaded(i) Then
                    Return i
                End If
            Next
            Return -1
        End Function
        Public Function load(ByVal name As String) As Integer
            loaded.Add(name)
            Return loaded.Count
        End Function
        Public Sub addInfo(ByVal sx, sy, px, py)
            bW.Add(sx)
            bH.Add(sy)
            bX.Add(px)
            bY.Add(py)
        End Sub
    End Class
End Module
