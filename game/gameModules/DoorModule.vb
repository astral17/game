Public Module DoorModule
    <Serializable()>
    Public Structure doorStruct
        Public x, y, id, tex As Integer
        Public world As String
        Public Sub New(ByVal x As Integer, y As Integer, id As Integer, tex As Integer, world As String)
            Me.x = x
            Me.y = y
            Me.id = id
            Me.tex = tex
            Me.world = world
        End Sub
    End Structure
    <Serializable()>
    Public Class DoorClass
        Public doors As New List(Of doorStruct)
        Public Function CanStep(ByVal dir As Integer) As Boolean
            Dim TX, TY As Integer
            TX = Player.x
            TY = Player.y
            Select Case dir
                Case Directions.Up
                    TY -= 1
                Case Directions.Down
                    TY += 1
                Case Directions.Left
                    TX -= 1
                Case Directions.Right
                    TX += 1
            End Select

            Dim tlist = doors.FindAll(Function(x) (x.x = TX) And (x.y = TY) And (x.world = Player.curWorld))
            For Each x As doorStruct In tlist
                If (Inv.searchItem(8) <> -1) Then 'masterkey
                    doors.Remove(x)
                    Return True
                End If
                If (Inv.searchItem(x.id) = -1) Then
                    Return False
                Else
                    doors.Remove(x)
                    Inv.subItem(x.id, 1)
                    Return True
                End If
            Next
            Return True
        End Function
        Public Sub create(ByVal x As Integer, y As Integer, vid As Integer, vtex As Integer)
            doors.Add(New doorStruct(x, y, vid, vtex, Player.curWorld))
        End Sub
        Public Sub New()

        End Sub
    End Class
End Module
