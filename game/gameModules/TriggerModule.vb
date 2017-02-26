Public Module TriggerModule
    <Serializable()>
    Public Structure action
        Public id, val, val2 As Integer
        Public valS As String
        Public Sub New(ByVal id1 As Integer, val1 As Integer, val21 As Integer, valS1 As String)
            id = id1
            val = val1
            val2 = val21
            valS = valS1
        End Sub
    End Structure
    <Serializable()>
    Public Structure triggerStruct
        Public x, y, x2, y2 As Integer
        Public ac As action
        Public world As String
        Public Sub New(ByVal x As Integer, y As Integer, x2 As Integer, y2 As Integer, world As String, ac As action)
            Me.x = x
            Me.y = y
            Me.x2 = x2
            Me.y2 = y2
            Me.world = world
            Me.ac = ac
        End Sub
    End Structure
    <Serializable()>
    Public Class TriggerClass
        Public triggers As New List(Of triggerStruct)
        Public update As Boolean = True
        Public Sub runPlate(ByVal n As Integer)
            If (triggers(n).ac.id = 0) And (triggers(n).ac.val = 0) Then
                MainForm.unpressarrows()
                MessageBox.Show("testbox")
            End If

            Select Case triggers(n).ac.id
                Case 0 'remove messageBox with val id
                    triggers.RemoveAt(n)
                Case 1 'set step chance
                    Enemy.stepChance = triggers(n).ac.val
                Case 2 'set step difficult
                    Enemy.stepDif = triggers(n).ac.val
                Case 3 'teleport to val world
                    Dim tmp1, tmp2 As Integer
                    tmp1 = triggers(n).ac.val
                    tmp2 = triggers(n).ac.val2
                    If tmp1 = -2 Then
                        tmp1 = Player.x
                    End If
                    If tmp1 = -3 Then
                        tmp1 = Player.y
                    End If
                    If tmp2 = -2 Then
                        tmp2 = Player.x
                    End If
                    If tmp2 = -3 Then
                        tmp2 = Player.y
                    End If
                    MainForm.loadmap(triggers(n).ac.valS, tmp1, tmp2)
            End Select
        End Sub
        Public Sub create(ByVal x As Integer, y As Integer, x2 As Integer, y2 As Integer, id As Integer, val As Integer, Optional ByVal val2 As Integer = 0, Optional ByVal valS As String = "")
            triggers.Add(New triggerStruct(x, y, x2, y2, Player.curWorld, New action(id, val, val2, valS)))
        End Sub
        Public Sub test(ByVal x As Integer, y As Integer)
            If update Then
                update = False
                For i As Integer = triggers.Count - 1 To 0 Step -1
                    If (MainForm.inRect(x, y, triggers(i).x, triggers(i).y, triggers(i).x2, triggers(i).y2)) And (triggers(i).world = Player.curWorld) Then
                        runPlate(i)
                    End If
                Next
            End If
        End Sub
    End Class
End Module
